#!/usr/bin/env python3
"""RU/TR tablolarını ve kaynakta çözülebilen çağrıları salt okunur denetler.

Bu tarama bir C# derleyicisi değildir. Değişken olarak taşınan anahtarları
çekirdeğin mesaj üreticileri, veri kimlikleri ve savaş enum aileleriyle tamamlar.
Unity'de font, taşma ve gerçek string.Format doğrulamasının yerine geçmez.
"""

import json
import re
import sys
from pathlib import Path


UNITY = Path(__file__).resolve().parents[2]
STRING = r'"(?:\\.|[^"\\])*"'
KEY = re.compile(r"(?:app|ui|battle|log|error|march|region|city|character|faction|petition|shortage|core)\.[a-z0-9_.]+$")
errors = []
tables = {}
locations = {}
formats = {}
referenced = set()
calls_checked = 0
messages_checked = 0


def report(message):
    errors.append(message)


def literals(expression):
    for match in re.finditer(STRING, expression):
        try:
            yield json.loads(match.group())
        except json.JSONDecodeError:
            continue


def placeholders(text, context):
    """.NET bileşik biçim indeksleri, hizalama ve N0 gibi formatları okur."""
    result = set()
    i = 0
    while i < len(text):
        if text[i : i + 2] in ("{{", "}}"):
            i += 2
        elif text[i] == "{":
            end = text.find("}", i + 1)
            if end < 0:
                report(f"{context}: kapanmayan biçim alanı")
                break
            token = text[i + 1 : end]
            match = re.fullmatch(r"(\d+)(?:\s*,\s*-?\d+)?(?::[^{}]+)?", token)
            if not match:
                report(f"{context}: geçersiz .NET biçim alanı {{{token}}}")
            else:
                result.add(int(match.group(1)))
            i = end + 1
        elif text[i] == "}":
            report(f"{context}: eşleşmeyen kapanış ayracı")
            i += 1
        else:
            i += 1
    return result


def split_arguments(source, opening):
    """Dizge ve iç çağrıları atlayıp bir çağrının üst seviye argümanlarını bulur."""
    parts, stack = [], [")"]
    start, i = opening + 1, opening + 1
    pairs = {"(": ")", "[": "]", "{": "}"}
    while i < len(source):
        char = source[i]
        if char in ('"', "'"):
            quote = char
            i += 1
            while i < len(source):
                if source[i] == "\\":
                    i += 2
                    continue
                if source[i] == quote:
                    break
                i += 1
        elif char in pairs:
            stack.append(pairs[char])
        elif char == stack[-1]:
            stack.pop()
            if not stack:
                parts.append(source[start:i].strip())
                return parts
        elif char == "," and len(stack) == 1:
            parts.append(source[start:i].strip())
            start = i + 1
        i += 1
    return []


def require(key, context):
    referenced.add(key)
    if key not in tables:
        report(f"{context}: eksik anahtar {key}")


def check_call(key_expression, argument_count, context):
    # Birleştirilen aileler aşağıda kaynak kimlikleriyle genişletilir.
    keys = [key for key in literals(key_expression) if KEY.fullmatch(key) and not key.endswith(".")]
    for key in keys:
        require(key, context)
        if key in formats and formats[key] and max(formats[key]) >= argument_count:
            report(f"{context}: {key}, {{{max(formats[key])}}} istiyor; yalnızca {argument_count} argüman var")
    return len(keys)


paths = sorted((UNITY / "Assets/Resources/Localization").glob("*.json"))
for path in paths:
    try:
        payload = json.loads(path.read_text(encoding="utf-8-sig"))
    except (OSError, json.JSONDecodeError) as error:
        report(f"{path.name}: {error}")
        continue
    if not isinstance(payload, dict) or not isinstance(payload.get("entries"), list):
        report(f"{path.name}: entries dizisi yok")
        continue
    for index, row in enumerate(payload["entries"]):
        context = f"{path.name}[{index}]"
        if not isinstance(row, dict) or not isinstance(row.get("key"), str) or not row["key"]:
            report(f"{context}: geçersiz anahtar")
            continue
        key = row["key"]
        if key in tables:
            report(f"{context}: yinelenen {key}; önceki {locations[key]}")
        tables[key], locations[key] = row, context
        language_formats = {}
        for language in ("ru", "tr"):
            value = row.get(language)
            if not isinstance(value, str) or not value.strip():
                report(f"{context}: {language} çevirisi boş")
                continue
            language_formats[language] = placeholders(value, f"{key}/{language}")
        if language_formats.get("ru") != language_formats.get("tr"):
            report(f"{key}: RU/TR biçim indeksleri farklı: {language_formats}")
        formats[key] = language_formats.get("ru", set())

sources = {}
for path in sorted((UNITY / "Assets/Scripts").rglob("*.cs")):
    raw = path.read_text(encoding="utf-8-sig")
    # Açıklamalar taramaya katılmaz; satır sayıları değişmez.
    source = re.sub(
        STRING + r"|//[^\n]*|/\*[\s\S]*?\*/",
        lambda match: match.group() if match.group().startswith('"') else re.sub(r"[^\n]", " ", match.group()),
        raw,
    )
    sources[path.name] = source
    for key in literals(source):
        if KEY.fullmatch(key) and not key.endswith("."):
            require(key, path.name)
    specs = [(r"(?<![\w.])(?:L\.Text|T)\s*\(", 0, 1, "text")]
    if path.name == "TacticalBattle.cs":
        specs += [(r"(?<![\w.])Text\s*\(", 1, 3, "text"), (r"(?<![\w.])Button\s*\(", 1, 2, "text")]
    if path.name == "CampaignCore.cs":
        specs += [(r"\b(?:Record|Result)\s*\(", 1, 2, "message")]
    for pattern, key_index, first_value, kind in specs:
        for match in re.finditer(pattern, source):
            arguments = split_arguments(source, source.index("(", match.start()))
            if len(arguments) <= key_index:
                continue
            context = f"{path.name}:{source.count(chr(10), 0, match.start()) + 1}"
            checked = check_call(arguments[key_index], len(arguments) - first_value, context)
            if kind == "message":
                messages_checked += checked
            else:
                calls_checked += checked


def array_values(source, name):
    match = re.search(r"\b" + name + r"\s*=\s*\{([^}]+)\}", source)
    if not match:
        report(f"Kaynak ailesi bulunamadı: {name}")
        return []
    return list(literals(match.group(1)))


core = sources.get("CampaignCore.cs", "")
hud = sources.get("CabinetHud.cs", "")
battle = sources.get("TacticalBattle.cs", "")
regions = re.findall(r'\bDef\("([a-z]+)"', core)
if len(regions) != 12:
    report(f"Bölge kaynak listesi 12 olmalı, bulunan {len(regions)}")
families = {
    "region.": regions,
    "city.": regions,
    "faction.": array_values(core, "FactionIds"),
    "character.": array_values(core, "CharacterIds"),
    "ui.mode.": array_values(hud, "ModeNames"),
    "ui.legend.": array_values(hud, "ModeNames"),
    "ui.tab.": ["council", "economy", "journal"],
    "ui.month.": [str(month) for month in range(1, 13)],
    "petition.choice.": ["relief", "negotiate", "refuse"],
    "petition.effects.": ["relief", "negotiate", "refuse"],
    "log.petition.": ["relief", "negotiate", "refuse"],
}
for enum_name in ("Kind", "Formation", "Condition"):
    match = re.search(r"enum\s+" + enum_name + r"\s*\{([^}]+)\}", battle)
    if not match:
        report(f"Savaş enum bulunamadı: {enum_name}")
        continue
    families["battle." + enum_name.lower() + "."] = [value.strip().split("=")[0].strip().lower() for value in match.group(1).split(",") if value.strip()]

family_count = 0
for prefix, values in families.items():
    for value in values:
        suffixes = (".name", ".position", ".agenda") if prefix == "character." else ("", ".demand") if prefix == "faction." else ("",)
        for suffix in suffixes:
            key = prefix + value + suffix
            require(key, "Dinamik aile")
            family_count += 1
            if formats.get(key):
                report(f"Dinamik aile {key}: argümansız kullanımda biçim alanı var")

# Kayıtların değişken anahtarlarla yeniden çevrildiği iki kritik şablon.
for key, expected in {"log.week": {0, 1, 2, 3}, "log.shortage": {0, 1}, "log.battle.victory": {0, 1, 2}, "log.battle.defeat": {0, 1, 2}, "battle.result_count": {0}, "battle.result_field_morale": {0}, "battle.result_return_morale": {0}, "battle.result_convoy": {0}, "app.dispatch": {0}}.items():
    require(key, "Rapor sözleşmesi")
    if formats.get(key) != expected:
        report(f"Rapor sözleşmesi {key}: beklenen {expected}, bulunan {formats.get(key)}")

print(f"Tablo: {len(paths)}; anahtar: {len(tables)}; RU/TR çeviri: {len(tables) * 2}.")
print(f"Doğrudan metin/askerî sarmalayıcı çağrısı: {calls_checked}; çekirdek mesaj çağrısı: {messages_checked}; dinamik aile anahtarı: {family_count}.")
print(f"Eksik anahtar, yinelenme, boş çeviri veya biçim hatası: {len(errors)}.")
for error in errors:
    print("HATA: " + error)
print("Sınır: bu statik doğrulamadır; Unity ekran ve ses incelemesini kapsamaz.")
sys.exit(1 if errors else 0)
