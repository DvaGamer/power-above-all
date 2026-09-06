# Ana oyun metnindeki üç çalışma-notu kalıntısı

6 Eylül 2026. Salt okunur inceleme; yalnız bu not yazıldı. Localization ve gerçek çağrı yerleri kontrol edildi. Aşağıdaki üç öneri henüz kaynak değişikliği değildir.

1. **İlk günlük kaydı — `core.json / log.begin`.** RU metin “Персонажи и численные значения этого наброска вымышлены”, TR “Bu taslaktaki karakterler ve sayısal değerler kurgusaldır” diyerek ilk karar ekranını geliştirme notuna çeviriyor. `CampaignCore.Create` gerçekten bu kaydı üretir; son atlas karelerinde de görüldü. Yeni oyun içi cümle önerisi: RU `Совет ждёт ваших первых распоряжений.` / TR `Konsey ilk emirlerinizi bekliyor.` Tarih zaten günlüğün tarih satırında sunulur. Tarihî kurgu ve model sınırlamaları proje dokümantasyonunda tutulabilir.

2. **Dört danışmanın görev satırı — `core.json / character.{valcourt,morel,lefevre,dumas}.position`.** Her görevin sonundaki `· вымышленный персонаж` / `· kurgusal karakter`, portreye ve kişinin siyasi gündemine bakarken tekrar görünür. `CabinetHud.Council`, gerçek `PositionKey` değerini portrenin yanında çiziyor. Yalnız bu eki kaldırıp mevcut görev adını korumak yeterli: `Советник короны` / `Kraliyet danışmanı`, vb. Karakterin oyun rolü veya oyuncuya verilen bilgi kaybolmaz; portre yanındaki satır da kısalır.

3. **Yardımın son paragrafı — `cabinet.json / ui.help.body`.** RU `Эпоха и названия исторические. Границы, советники и числовые модели упрощены или вымышлены.` ve TR eşdeğeri, oyuncu gerçek kontrol ve karar döngüsünü okuduktan sonra tekrar model açıklamasına döndürüyor. Bu son paragrafın belgelerde kalması yeterli; yardım gövdesindeki gerçek klavye kontrolleri, rolün henüz söz olmadığı ayrımı, bakım gideri, siyasi sonuçlar ve savaş kuralları korunmalı.

Bu temizlik, oyuncunun doğru karar vermesini sağlayan mekanik açıklamaları kaldırmak anlamına gelmez. Örneğin savaş minyatürlerinin birden fazla askeri temsil etmesi, ikmalin cephaneyi etkilemesi ve dekoratif yol/çitin koruma vermemesi, görünüşten çıkarılamayan gerçek oyun bilgisidir.

## Yetkili dar düzenleme — SOURCE FREEZE

Root üç öneriyi onayladı. Yalnız altı mevcut localization girdisinin RU/TR değerleri değiştirildi: `log.begin`, dört `character.*.position` ve `ui.help.body`.

- Günlükte mevcut `5 мая 1789. Начало кампании.` / `5 Mayıs 1789. Sefer başlıyor.` başlangıcı aynen korundu; son cümle Konsey'in ilk emirleri beklemesine dönüştü. Bu girdide tarih placeholder'ı yoktu; key/args yapısı değiştirilmedi.
- Dört görev satırında yalnız kurgusal karakter eki kaldırıldı; mevcut görev adı korundu.
- Yardımda yalnız son tarihî/model paragrafı ve önündeki boş paragraf ayıracı kaldırıldı. Rol/söz ayrımı, bütün sayılı adımlar, 1–4/Shift/sağ tık/Boşluk kontrolleri ve cephanesiz piyade açıklaması aynı kaldı. Savaş footer'ı veya başka localization girdisi düzenlenmedi.
- `GAME_VISION_DRAFT.md` başına “Tarihî çerçeve ve oyun kurgusu” bölümü eklendi. 1789 Fransa esini, sadeleştirilmiş/kurgulanmış sınırlar ve sayısal modeller, tarihî bina rekonstrüksiyonu iddiası olmadığı ve dört danışmanın kurgusal olduğu adlarıyla birlikte korundu.

Patch sonrası yalnız değişen metinler okundu. Test, localization validator, Unity, derleme, oyuncu veya Git komutu çalıştırılmadı. Root'un yeni tablo/Unity/görsel kapısı bekleniyor; kaynak donuk.

Root'un ek onayıyla `GAME_VISION_DRAFT.md / Üretim ve kabul` içindeki tek eski üretim paragrafı da güncellendi: üç rol/vadeler, bölgesel anlaşmalar, kademeli ordu mevcudu ve subay atama yetkisi, bölgesel direniş ve olağan taktik savaş mevcut çalışan kesit olarak belirtildi. Geniş coğrafya, diplomasi, kariyer ve rejim yolları gelecek kapsamı olarak kaldı; bütün oyunun/vizyonun tamamlandığı iddia edilmedi. Paragraf diskten tekrar okundu. Son SOURCE FREEZE; başka Assets veya launch yok.
