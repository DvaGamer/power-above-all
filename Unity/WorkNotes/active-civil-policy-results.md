# Aktif sivil politika: sonuç ve mevcut açıklık

Kaynak: `output/core-probes/ActiveCivilPolicyProbe-2026-09-06T05-02-49-038Z-5645c958/probe.stdout.log` ve `result.json`. Root koşusu **873 check PASS**, compile/probe exit0;05:02:49.038–05:02:49.945 UTC. Receipt `unityOrPlayerVerified:false`; Core hash `a508a82942f3988d728cedf64847f8851e681fd296f041094abc52bbcb08d9ae`. Bu ajan yalnız çıktı ve kaynak okudu; Assets veya denge değiştirilmedi.

## Gerçekte ne uygulandı?

Her iki rota legacy+budget1000, aynı24 hafta, aynı hafta2 negotiate ve sürekli Ile garnizonu. Aktif rotada ayrıca yalnız hafta0'da Champagne tatili, Champagne'a bir bread ve Paris sübvansiyonunun açılması; ardından24 başarılı ödeme. İkinci bread, ikinci anlaşma, sübvansiyon kapatma, NPC toplama veya ret yok. Heuristic'in düşük stokta kapatma dalı bu koşuda sınanmadı.

| Hafta24 | Pasif budget1000 | Aktif sivil budget1000 |
| --- | ---: | ---: |
| Düşman bölgeler / öncelikler |11/12;3/3 |0/12;0/3 |
| Gold / Food |2292 /61 |3981 /234 |
| Troops / Manpower |1000 /2600 |1000 /2600 |
| MilitarySupplies / Power |334 /67 |334 /67 |
| Açlık, ödenemeyen hafta, kayıp |0 /0 /0 |0 /0 /0 |

Aktif dalın ilk emirleri sonrasından24. haftaya kadar her gözlemde0/12 düşman vardır. Tatil Champagne69→59, bread59→44 yaptı; burada tek başlangıç düşman bölgesi kalktı. Normandy ve Picardy'ye doğrudan hiçbir yardım verilmedi. Final Champagne27/65.5/60, Normandy13/80/60, Picardy25/74/60; Ile0/100/60. Bütün EliteLoyalty değerleri60 kaldı.

## Urban40 ve60 zinciri

Kaynak `CampaignCore.NextWeek`, satır323–331: başarılı Paris ödemesinin urban+3 etkisi **bölgelerin haftalık Unrest hesabından önce** uygulanır. Sonra bütün bölgelere şehir desteği<40 ise+2,40≤destek<60 ise0, destek≥60 ise−1 uygulanır. Garnizon−3 ve kıtlık/maaş açıkları ayrı eklenir; göstergeler0–100'e sınırlanır.

Başlangıç urban35, tek bread ile37. İlk ödeme37→40 yaptığı için daha **ilk haftada** ülke genelindeki+2 kaldırılır. Hafta7 destek58; hafta8 ödeme61'e taşır ve aynı hafta−1 başlar. Hafta8–24 arası17 hesap olduğundan garnizonsuz, ayrıca müdahale edilmeyen Normandy30→13 ve Picardy42→25 olur. Destek hafta21'de100'e ulaşır; sonraki ödemeler artık+3 gösterge artışı üretmese de mevcut Paris−4 ve yüksek destek rejimi sürer.

Pasif negotiate yalnız temsilci desteğini ve Paris'i değiştirir, urban35 kalır. Başkent dışındaki+2 sürer; bölge65'e vardığında aynı haftanın Control hesabında−3 başlar. Aktif dalda bu eşik hiç geçilmediği için yerel denetim de aşınmaz. Böylece Paris'teki devamlı ödeme, yalnız başkentteki−4 üzerinden açıklanamayacak bir **ülke çapında siyasi etki** yaratır.

Bu anlatım bread'in tek başına bütün kazancın zorunlu nedeni olduğu iddiası değildir. Kaynak aritmetiğinde bread olmadan iki ödeme35→38→41 yapardı; bunun tam alternatif rotası çalıştırılmadı. Mevcut sonuç, üç aracın birlikte uygulandığı örnektir.

## Gelir ve iaşenin gerçek hesabı

Bread gerçek40 Food harcadı; sübvansiyonlar24×20=480. Dört tatil hesabının gerçek TaxForgone değerleri **14,14,16,15; toplam59**. Bunlar tahminî dört çarpı tek bedel değildir.

İki ordunun24 haftalık maliyeti eşit: ilk iki hesap136, kalan22 hesap120; toplam2912 Gold. Başka anlık Gold emri yok. Çıktı toplamları pasif4364, aktif6053 vergi; fark **1689**, final Gold farkıyla tam aynıdır. Bu bir doğrudan sübvansiyon nakit ödülü değildir:

1. Daha düşük Unrest, `(1−U/150)` vergi tahsilatını ve `(1−U/200)` üretimini korur/artırır.
2. İsyan eşiğinin aşılmaması Control'ün düşmesini önler; `(0.5+C/200)` vergi çarpanı korunur. Champagne'da ilk bread/accord ayrıca toplam+5 Control verdi.
3. Her iki negotiate temsilci desteğini45→57 yapar. Aktif anlaşmanın hafta4 tamamlanması ayrıca+5 verir; hafta5'ten itibaren destek62'dir. Ulusal vergi çarpanı pasifte1.035, aktifte1.06 olur. Geliri yalnız urban'a bağlamak bu ayrı anlaşma sonucunu atlar.
4. Tatil dört hesabın sonunda biter; atlanan59 daha sonra borç olarak tahsil edilmez.

Aktif Gold hafta4'te hâlâ1159'a karşı1170 ile geridedir; hafta5'te1281'e karşı1261'e geçer. Hafta24 vergi279'a karşı136'dır. Bu başlangıçtan itibaren bedelsiz gelir değildir; korunmuş ülke durumunun biriken sonucudur.

Gerçek24 haftalık üretim toplamı aktif3862, pasif3169; fark693 Food. Nüfus ve ordu tüketimi eşittir. **693−480−40=173**, final Food234−61 farkını açıklar. Aktif stok önce düşer: hafta16–17'de206, ardından büyür; haftalık NetFood17'de0,18'den sonra pozitiftir. Sivil araçlar gıdayı doğrudan çoğaltan bir hile çağrısı yapmıyor; düşük huzursuzluğun üretim kaybını azaltması devamlı yardımın bedelini bu koşuda aşıyor.

## Oyuncuya bugün ne açıklanıyor?

Kaynak denetimi; yeni ekran görüntüsü veya görünürlük ölçümü yapılmadı:

- `CabinetHud.Economy`, satır579–607: mevcut→hafta sonu stokları, vergi/ordu/üretim/tüketim satırları, gerçek tatil vergi kaybı ve **ortalama ülke Unrest'inin mevcut→sonraki değerini** zaten gösteriyor. `Observe`, satır130–134, bunu derin kopyaya gerçek NextWeek uygulayarak hesaplıyor; yeni bir genel tahmin ekranı gerekmiyor.
- `cabinet.json` içindeki `ui.economy.tax.reason` ve `.food.reason`, huzursuzluk/denetim/temsilci desteğinin gelir ve üretime etkisini doğru açıklıyor. `.subsidy.active`,20 Food, başarılı Paris−4/urban+3 ve başarısız+6/−8 sayılarını veriyor. `.unrest.reason`, tahminde şehir desteği, kıtlık/maaş, Paris yardımı ve ordunun varlığı bulunduğunu açıkça söylüyor.
- Paris emir kartı `.order.subsidy.detail` yalnız20 haftalık bedel ve stoklara bağlı etkiyi özetliyor; bread kartı urban+2'yi gösteriyor. Başarılı ödeme journal'ı da Paris−4/urban+3'ü kaydediyor.
- `CabinetHud.Council`, satır532–569, urban'ın mevcut Approval göstergesini ve güvenilir ekmek talebini gösteriyor. **40/60 eşiklerini veya bunun bütün bölgelere+2/0/−1 getirdiğini göstermiyor.** Lokalizasyon taramasında bu eşik açıklaması yok. Ekonomi de ortalama toplamın hangi kısmının genel şehir desteği bileşeni olduğunu ayırmıyor. `ui.economy.subsidy.note` metni dosyada mevcut olsa da bu key için sunum çağrısı bulunmadı; varlığını oyuncuya gösterilen açıklama diye saymıyorum.

Dar okunabilirlik fırsatı, mevcut ekonomi açıklamasına ve urban kartına bu genel rejimi görünür biçimde bağlamak: gerçek sonraki destek ve ülke bileşeni, diğer garnizon/kıtlık etkileriyle karışmadan açıklanabilir. Yeni panel veya ikinci tahmin formülü önermiyorum; mevcut gerçek sonraki state kullanılmalı, bloke haftada kabul edilmiş sonuç varmış gibi sunulmamalı. Bu yalnız öneridir, Assets değişikliği yetkisi değildir.

## Sonucun sınırı

Pasif karşılaştırma önceki receipt ile tam eşleşti; güçlü fark gerçektir. Ancak barışçıl, savaşsız, aynı garnizonlu24 haftada tek birleşik politika incelendi. Ayrı subsidy-only/bread-only/accord-only deneyleri yok; kesilen yardım, kıtlık sırasında yeniden açma, pahalı savaş ve asker kaybı dalları da bu sonuçta yok. Bu nedenle “genel denge çözüldü”, “her zaman en iyi politika”, “ülke mutlaka sakin kalmalı” veya tek araca atfedilen kesin getiri değildir. Mevcut siyasi etkinin çok geniş kapsamı ve iki eşik sayesinde birikmesi anlaşılır; önce oyuncunun bu bağlantıyı görebilmesi değerlendirilmelidir. Denge sayıları değiştirilmedi.
