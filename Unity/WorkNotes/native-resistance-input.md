# Bölgesel direnç: gerçek açıklama ve yerel karar girdisi

**Sonuç:** sonraki denetim bölümünde050250 koşusu PARTIAL/native0,44 kontrol ve gerçek RU/TR/vergi/ekmek tamamlanmıştır. Aşağıdaki ilk hazırlık planı tarihçedir; nihai script üç75 saniyelik pencere kullanır.

6 Eylül 2026. `tools/native-resistance.script` yalnız kaynak olarak hazırlandı; henüz bu yeni native koşu çalıştırılmadı. Root yeni kısaltılmış neden metninin derlemesini ve gerçek oyuncuyu yönetecek. Bu ajan Assets veya native helper değiştirmedi, test/derleme/oyuncu başlatmadı.

İlk kurulum: new, RU, Champagne, mevcut journal. Başlangıç direnci1114, ordu1200, Gold840, Food360; U69 / Control60.5. `00-start` JSON/PNG, helper'ın12 saniyelik hazır olma kontrolüne hizmet eder. İlk wait0.8 dışındaki uzun beklemeler yalnız bundan sonradır.

Üç55 saniyelik gerçek girdi penceresi:

1. Root neden okunu gerçek fareyle açar. Tam campaign başlangıçla `same`, LanguageRU;01 gerçek görünüm karesi. Script'te açıklamayı açan semantic komut yoktur.
2. Root gerçek RU/TR, tekrar ok ve gerekirse gerçek scroll kullanır. Tam campaign yine aynı, LanguageTR;02 görünüm karesi. `lang tr` komutu yoktur; sadece gerçekten seçilen dil denetlenir. Açıklamanın okunması otomatik campaign eşitliğiyle ispatlanmaz, root görüntüyü ayrıca incelemelidir.
3. Root gerçek Tax tıklaması yapar ve1106'ya geçmeden önce1234 düşman/U81 görüntüsünü ayrı native inputPNG olarak kaydeder; sonra gerçek Bread tıklar. Script yalnız sonunda direnci1106, U66 / Control62.5, Gold940 / Food320 ve değişmeyen1200 askeri denetler. Tax veya Bread sonucu script tarafından uygulanmaz.

Toplam5 PNG /5 state JSON.01 ve02,00 ile tam eşit olmalıdır.03→04 gerçek save/load ile tam eşit olmalıdır; yüklemeden önce Normandy seçilerek eski Champagne seçiminin gerçekten dönmesi de sınanır. Takvim, hareket, stok, ordu konumu, commission/accord ve resolved battle alanları üçüncü pencerede beklenmeyen başka bir oyun eylemini yakalamak için denetlenir.

Fiilen kabul edilen son değişiklikler sonraki audit'te ayrı okunmalı: Tax100 Gold getirir, U+12 / EliteLoyalty−4 / urban−3 / crown+1; Bread40 Food harcar, U−15 / Control+2 / urban+2 / Lefevre Relationship+2. Script son1106'yı denetler; aradaki1234 yalnız root'un gerçek ara PNG'siyle kanıtlanacaktır. Bu senaryoda march/battle yoktur; eski savaş kayıplarıyla sonuç tahmin edilmez.

Root `PlayerTimeoutSeconds240` geçirecek. Üç pencere165 saniye, kurulum/kareler/kayıt için kalan75 saniye vardır. Gerçek native tuş/fare zamanları başlangıç duvar saatinden tahmin edilmez; root her aşamanın PNG checkpoint'ini bekler. UI henüz yeni derlemeyle görülmediği için sabit click/scroll koordinatları bu nota kabul edilmiş değer olarak yazılmadı.

## Gerçek başarısız ilk deneme ve yeni tamamlanmış koşu

İlk plan yukarıda değişmeden korunur. `native-input-20260906-045843-7f73e665` gerçek RED/native1, timeout=false,171.46 saniyedir. Protocol35 komut/19 assertion sonrası `Expected ResistanceTroops1106, observed1114` ile durdu. Root açıklaması: Wheel25 helper'ın izin verdiği en çok10 çentiği aştığı için giriş reddedildi; root düzeltmesi sonraki politik tıklamalara zaman bırakmadı. Tax/Bread oyuna gönderilmemişti. Bu bir oyunun uygulanmış vergiyi/ekmeği yanlış hesapladığı kanıtı değildir; başarısız girdi zamanlaması kaydı olarak kalır. Eski receipt ve script değiştirilmedi.

Yeni koşu `native-input-20260906-050250-67a1117e`: **PARTIAL/native0**, timeout=false,236.19 saniye;68 komut/44 assertion/5 PNG/5 JSON, boş failures. Gerçek kopyalanmış script üç `wait75` içerir; player sınırı300, owner360 saniyedir. Bu, önceki3×55 planıyla aynı süre olarak sunulmaz. Script'te act/commission/accord komutu0; dil yazması yalnız ilk `lang ru`. Yeni kaynak/test/browser bu native koşuda çalıştırılmadı.

Player PID4392, başlangıç `2026-09-06T05:02:51.0728039Z`, native tamamlanış `05:06:42.7315212Z`; owner PID16668, başlangıç `05:02:50.6327537Z`. Protocol tamamlanışı `05:06:41.8907427Z`. Build `resistance-unified-dispatch-20260906-045736-445-e8fe43ec`; owned receipt runtime SHA256 `FB28C24721E885A2E13BD9800C1419B7A21E2A883E4212B1CAD6BA014B987B62`, script SHA256 `DEC81B6E575B4D15A5D4ACDAAAF3E4544C32C9796D51C48728D6A47B01701CBC`.

Root bütün gerçek input PNG'lerini gördüğünü bildirdi. İstanbul saatleri: RU neden oku08:03:12; gerçek dil düğmesi08:05:08 ve TR oku08:05:10.02 checkpoint sonrasında wheel+10, (110,600),08:06:00 emirleri geri gösterdi. Tax (110,367),08:06:01 ara görüntüsü1234/U81/Gold940; Bread (110,293),08:06:21 son görüntüsü1106/U66/Food320. Ara1234 ayrı root inputPNG gözlemidir; bu noktada script state JSON'u yoktur.

### Tam state ve gerçek maliyet farkları

`00-start.json`, `01-native-why-ru.json`, `02-native-why-tr.json` raw byte-eşit: SHA256 `EA33D1EE5D24AD82DF0B68CEA2003B846A78C143B4CCF7ABBA7DF7223A982394`. Açıklama, dil ve scroll hiçbir kampanya alanını değiştirmedi.01RU ve02TR dil assertion'ları da geçti; JSON eşitliği tek başına yazıların görünürlüğü kanıtı sayılmadı.

02→03 değişen üst alanlar yalnız Gold/Food/Regions/Factions/Characters/Journal. Gold840→940, Food360→320. Nested farklar tam olarak:

- Champagne Unrest69→66, Control60.5→62.5, EliteLoyalty60→56, TaxUsed ve BreadUsed false→true.
- Crown Approval65→66; urban Approval35→34.
- Lefevre Relationship50→52; diğer karakter alanları değişmedi.
- Tek `log.tax(region.champagne)` ve ardından tek `log.bread(region.champagne)` kaydı; eski journal kuyruğu aynı.

Troops1200, Manpower2400, Supplies120, Morale78, Supply100, Power55, Moves2, Fatigue0, Week0, orduÎle ve seçimChampagne değişmedi; resolved battle listesi boş. Başka bölgeye yansıyan hayalet tıklama veya üçüncü kampanya eylemi state farkında yoktur. Resistance1106 assertion'ı gerçek son bölge üzerinden geçti.

`03-native-tax-bread.json` ve `04-native-tax-bread-loaded.json` raw byte-eşit: SHA256 `73138C785E258F98184E84A2DFAD73EE8B97C48CE14B9AF1567E8E272EE10CC2`. Gerçek vergi/ekmek sonuçları, kullanılmış emirler ve seçiliChampagne save/load sonrası korundu. Bu ajan yalnız tamamlanmış kaynak/artifact okuması ve bu notu güncelledi; yeni girdi, süreç, Assets veya receipt değişikliği yapmadı.
