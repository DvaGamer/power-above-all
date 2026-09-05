# Power Above All — v0.2 tasarım tanımı

**Güncel durum: gelecekteki kapsamlı tasarım.** Son kullanıcı kararıyla motor **Unity**, yeni oyunun dilleri **Rusça ve Türkçe** olarak seçildi. İlk temel aktarım sürüyor; Unity çalışma zamanı henüz doğrulanmadı. **Etkin sonraki aşama 0.2 Visual & Feel Polish Pass; yeni mekanik veya ekonomi genişletmesi yok.** Bu belgedeki dört bağlı sistem, gelecekteki geliştirmeler için korunur; hemen uygulanacak görev listesi değildir.

Belge, önceki kapsamlı talebi Türkçe olarak kaydeder: siyasi güçler ve karakterler, bölgesel harita katmanları, moral ve düzen içeren alay savaşları, ordu ikmali. Önceki “yalnızca siyaset, savaş veya ekonomi dallarından birini seçme” yaklaşımı artık kullanılmaz.

Buradaki kapsam, istenen tasarımın tamamını korur. Sonraki uygulama adımları ve v0.2'de hangi ayrıntıların hangi sırada yapılacağı [ROADMAP.md](ROADMAP.md) içinde belirlenir. Uzun vadeli kapsamın burada bulunması, tüm ayrıntıların ilk v0.2 tesliminde tamamlanmış olacağı anlamına gelmez; yol haritasındaki aşamalar da bu tasarım taleplerini iptal etmez.

**Henüz kararlaştırılmamış konu:** oyuncunun somut siyasi kimliği. İlk taslaktaki kraliyet konseyi rolü, nihai oyuncu kimliği olarak kesinleşmiş değildir. Motor seçimi tamamlandı: Unity. Doğrulanmış tarayıcı 0.1 kullanılabilir kalır; bu belgenin kapsamı Unity'de tamamlanmış özellikleri göstermez.

## 1. Kimlik, dönem ve temel ilke

Oyun Fransa'da, **5 Mayıs 1789** tarihinde başlar. Görsel kimlik, eski düzen çözülürken bir araya gelen **kraliyet kabinesi, tarih atlası ve askerî raporlar** üzerinden kurulur. Tarihsel başlangıcın kaynağı [REFERENCES.md](REFERENCES.md) içinde bulunur; aşağıdaki karakter durumları, olaylar ve rakamlı örnekler ayrıca doğrulanmış tarihsel olaylar değildir.

Oyuncunun amacı yalnızca Fransa'yı başarılı yönetmek değildir. **Kendi siyasi varlığını, iktidarını ve kontrolünü korumak** da temel hedeftir. Devlet için yararlı bir karar, oyuncunun konumunu zayıflatabilir; başarılı bir komutan aynı zamanda siyasi tehdit oluşturabilir.

Temel neden-sonuç zinciri:

**Siyasi karar → ekonomik sonuç → bölgesel etki → askerî ikmal veya hareket imkânı → savaş → siyasi güç değişimi.**

Her yeni mekanik, en az bir başka ana sistemle gerçek bir etkileşim kurmalıdır. Ekrana yeni bir sayı eklemek tek başına sistemler arası bağlantı sayılmaz. Bir kararın üretim, bölgesel kontrol, ordu, karakter veya siyasi güç üzerindeki sonucu hesaplanmalı ve oyuncuya açıklanmalıdır.

## 2. Harita ve görsel dil

Fransa haritası ana çalışma alanı olarak kalır. Hedef, 18. yüzyıl sonunu çağrıştıran bir atlas düzenidir:

- Soluk bölge renkleri, hafif kâğıt dokusu ve ince sınırlar.
- Serif yer adları, küçük şehir işaretleri ve ordu sancakları.
- Hareket güzergâhları, ikmal hatları ve konuya bağlı harita katmanları.
- Haritanın üzerinde açılan belge panelleri; ilgili bölgenin ve çevresinin bağlamı görünür kalır.
- Korunacak palet: orman yeşili, fildişi/kâğıt tonları ve mat altın; tehlike ve olumsuz sonuçlarda kontrollü kırmızı kullanımı.

Fantastik arayüz, ahşap ortaçağ düğmeleri, parlak mobil oyun estetiği, cilalı kartlar, aşırı renk geçişleri ve siber gösterge panelleri kullanılmaz. Daha zengin görünüm, süs yoğunluğundan değil bilgi hiyerarşisinden ve malzeme tutarlılığından doğar.

Harita, hesaplanan duruma görünür karşılık verir: protestolar, tehlikeli yollar, kıtlık simgeleri, ordu sancağı, ikmal hattı ve isyan hâli. Bir simge, simülasyonda karşılığı olmayan bir durumu varmış gibi göstermez.

## 3. Siyasi güçler ve kişisel iktidar

### Hedeflenen siyasi güçler

Tam tasarımda yedi güç bulunur:

| Güç | Oyunda görünür olması gereken siyasi kimlik |
| --- | --- |
| Kraliyet | Saray ve kraliyet iktidarı. |
| Soylular | Soylu çıkarları ve bölgesel nüfuz. |
| Ruhban sınıfı | Dinî kurumlar ve bunların siyasi ağırlığı. |
| Üçüncü Zümre / Meclis | Temsilciler ve meclis içindeki talepler. |
| Kentli halk | Şehirlerin siyasi ve ekonomik talepleri. |
| Kırsal halk | Köylerin ve kırsal üretimin talepleri. |
| Ordu | Askerî kurum, komutanlar ve askerlerin sadakati. |

Her gücün **nüfuzu, oyuncuya desteği/tutumu, radikalleşmesi, talebi, lideri, müttefikleri ve rakipleri** bulunur. Bunlar yalnızca ayrı göstergeler değildir: talepler karşılandığında veya reddedildiğinde bölgesel gelir, kontrol, ikmal ve karakter davranışları etkilenebilir.

### Karakterler

Her önemli karakter şu bilgilerle tanımlanır:

- Ad ve portre.
- Görev veya makam.
- Sadakat, hırs ve yetkinlik.
- Siyasi bağlılık.
- Oyuncuyla kişisel ilişki.
- Kendi gündemi ve elde etmek istediği sonuç.

Karakter rolleri arasında **bakan, general, temsilci, soylu, piskopos, finansçı ve ajitatör** bulunur. Aynı siyasi gücün içindeki kişiler aynı çıkarları veya aynı sadakati paylaşmak zorunda değildir.

Yetkin bir general, kazandığı nüfuzla oyuncu için tehlikeli olabilir. Bir bakan görevini sürdürmek için taviz isteyebilir. Bir temsilci oyuncunun kararlarına karşı muhalefet kurabilir. Bu davranışlar, kişilerin hesaplanan durumuna ve çıkarlarına bağlanır.

### Büyük kararların açıklaması

Önemli bir kararın yanında şu sorular cevaplanır:

- Kim kazanıyor, kim kaybediyor?
- Kim daha sadık oluyor, kim daha tehlikeli hâle geliyor?
- Hangi bölgenin ve hangi ekonomik göstergenin durumu değişiyor?
- Oyuncu ilerisi için hangi beklentiyi veya yükümlülüğü oluşturuyor?

**Örnek: halka ekmek dağıtımı.** Anlık örnek etkiler: gıda **−60**, Paris huzursuzluğu **−12**, kentli halkın desteği **+10**. Daha sonra hazine üzerindeki baskı, fiyat denetiminden hoşnutsuz tüccarlar, yardıma bağımlılık ve yardım kesildiğinde yeni kriz ortaya çıkabilir. Bunlar talebin neden-sonuç örnekleridir; kesin dengelenmiş sayılar, mevcut kod değerleri veya kaçınılmaz tarihsel sonuçlar değildir.

## 4. Bölgeler ve harita katmanları

Her bölgenin temel modeli şu alanları kapsar:

- Nüfus.
- Gıda üretimi ve yerel gıda rezervi.
- Vergi geliri ve vergi toplama verimliliği.
- Huzursuzluk.
- Hükûmet kontrolü.
- Yerel seçkinlerin sadakati.
- Askerî varlık.

Gelecekte genişletilecek alanlar: **kentleşme, altyapı, dinî nüfuz ve siyasi hizalanma**. Bu uzun vadeli alanlar, hesaplanmadan etkin gösterge gibi sunulmaz.

Bilgi aşamalı açılır: harita önce ana sorunu gösterir; bölge paneli mevcut durumu ve nedenleri açıklar; araç ipucu veya ayrıntı görünümü hesabı sunar. Her sayı aynı anda ana ekrana yığılmaz.

Gerekli harita katmanları:

| Katman | Cevaplaması gereken soru |
| --- | --- |
| Kontrol | Hükûmetin otoritesi nerede güçlü veya zayıf? |
| Huzursuzluk | Nerede gerilim var ve nerede artıyor? |
| Gıda | Üretim, rezerv ve açık hangi bölgelerde? |
| Vergi | Vergi nereden geliyor, nerede verimsiz toplanıyor? |
| Ordu ikmali | Hangi ordu hangi kaynağa bağlı; hangi yol veya bölge riskli? |
| Siyasi nüfuz | Hangi siyasi güç hangi bölgede etkili? |

## 5. Ekonomi — dört kaynak havuzu

Ekonominin dört ana havuzu **hazine, gıda, askerî malzeme ve insan gücü** olarak tanımlanır. Kaynağın mevcut miktarı, nereden geldiği, nereye harcandığı ve sonraki dönem beklentisi açıklanır.

| Kaynak | Arayüzün açıklaması gereken bağlantı |
| --- | --- |
| Hazine | Gelirler, harcamalar, askerî ödemeler ve kararların maliyeti. |
| Gıda | Üretim, bölgesel rezervler, sivil tüketim, ordu tüketimi ve yardımlar. |
| Askerî malzeme | Orduyu donatma ve ikmal etme ihtiyacı; eksikliğin askerî sonucu. |
| İnsan gücü | Asker toplama imkânı ve insan gücü kullanımının bölgesel etkileri. |

Üretim ve tüketimin mekânsal sonuçları görünür olmalıdır: ülke toplamında gıda bulunması, her ordunun ve her şehrin bu gıdaya eriştiği anlamına gelmez.

**Örnek haftalık gıda hesabı:**

`341 mevcut + 186 üretim − 110 sivil tüketim − 40 ordu tüketimi − 20 yardım = 357 sonraki stok`

Bu denklem, açıklamanın biçimini gösterir; uygulanmış ekonomi değerleri veya denge taahhüdü değildir. Oyuncu ekmeğin kaynağını, açığın yerini ve gelecek haftanın neden kötüleşebileceğini görebilmelidir. Ana sorunun yaklaşık iki saniyede anlaşılması görsel tasarım hedefidir; ölçülmüş başarı iddiası değildir.

## 6. Harita üzerindeki ordular ve ikmal

Ordular haritada fiziksel konuma sahip olur; yalnızca üst çubuktaki toplam asker sayısından ibaret değildir. Ordu şu bilgileri taşır:

- Asker sayısı ve alay bileşimi.
- Moral, ikmal durumu ve deneyim.
- Komutan.
- Hareket kapasitesi ve yorgunluk.

Alay türleri: **hat piyadesi, milis, süvari ve topçu**.

Yoksul veya düşmanca bir bölgeden yürümek gıda tüketimi, yıpranma, yerel huzursuzluk ve daha yavaş hareket gibi sonuçlara bağlanır. Etkiler bölgenin ve ordunun gerçek durumundan hesaplanır. Ücreti ödenmeyen veya ikmal alamayan ordularda moral düşüşü ve firar görülebilir.

Siyasi kararlar ikmali etkileyebilir; yerel direnç vergi ve kaynak akışını aksatabilir. İkmal yetersizliği savaş performansına, savaş sonucu da komutanın ve oyuncunun siyasi konumuna geri döner.

## 7. Taktik savaş — alay, düzen ve moral

### Kontrol ilkesi

Taktik yön **Napoleon: Total War** üzerinden tanımlanır. **Warcraft III yalnızca komutların tepkiselliği için referanstır; savaşın temposu için değildir.** Oyuncu tek tek askerleri değil, alayları yönetir.

Savaşın temel unsurları: **düzen, menzil ve ateş açıları, moral, birlik bütünlüğü, arazi, kanatlar ve yorgunluk**. Asker figürleri alayın durumunu görselleştirir; bireysel kontrol hedefi değildir.

### Alay türleri ve görevleri

| Tür | Güçlü yön | Sınırlama ve tehdit |
| --- | --- | --- |
| Hat piyadesi | Hat düzeninde geniş cepheden güçlü ateş. | Hareket hâlindeyken ve destek olmadan süvariye karşı savunmasızlık. |
| Milis | Ucuz kuvvet; mevzi tutmaya katkı. | Düşük moral ve düşük atış isabeti. |
| Süvari | Hız, açık kanatlara saldırı ve kaçan düşmanı takip. | Hazırlıklı savunma düzenlerine karşı zayıflık. |
| Topçu | Uzun menzil ve düşman morali üzerinde baskı. | Yavaş hareket; korunma ihtiyacı. |

### Moral ve çözülme

Moral durumları: **sağlam → sarsılmış → çözülmek üzere → kaçıyor**. Kayıplar, topçu ateşi, kanattan saldırı, yakındaki dost birliklerin kaçışı, komutan, yorgunluk, ikmal ve deneyim moral üzerinde etkili olur.

Savaşlar sıklıkla düşmanın tamamen yok edilmesiyle değil, moralinin kırılıp kaçmasıyla sona ermelidir. Moral, asker sayısından ayrı okunur; birlik bütünlüğü ve yorgunluk da kendi etkilerini taşır.

### Düzenler ve yerleştirme

| Düzen | Kullanım | Bedel veya zayıflık |
| --- | --- | --- |
| Hat | Ateş gücünü cepheye yaymak. | Hareket ve yeniden düzenlenme koşullarına duyarlılık. |
| Kol | Hareket ve hücum. | Daha zayıf ateş gücü. |
| Kare | Süvariye karşı savunma. | Topçu ve tüfek ateşine karşı zayıflık. |

Emir verirken alayın hedef düzeni yarı saydam bir önizlemeyle gösterilir. Menzil ve ateş açıları seçime bağlı, hafif çizgilerle okunur; savaş alanını sürekli çizgi kalabalığı kaplamaz.

### Ateş yönetimi

Alay düzeyindeki emirler: **serbest ateş, ateşi kes ve yaylım ateşi**. Mevcut genel yetenek yaklaşımının yerini alay bazlı ateş kontrolü alır. Yakın mesafede toplu yaylım anlamlı bir taktik seçenek olur. Ateş dumanı görüşü geçici olarak örter; sonuç, oyuncuya anlaşılır biçimde gösterilir.

### Savaş arayüzü

- Alt bölümde alay kartları: tür, asker sayısı, moral, cephane/durum ve deneyim.
- Tekli ve çoklu alay seçimi.
- Komutlar: hareket, saldırı, mevziyi tut, düzen seç, ateş modu seç ve geri çekil.
- Seçimin ve verilen emrin hızlı, açık görsel karşılığı.
- Kompakt arayüz; araziyi, düzenleri ve kanat tehditlerini görmek için yeterli alan.

## 8. Olaylar — kişi, talep ve yükümlülük

Crusader Kings III yönünde olaylar, belirli bir karakter veya siyasi güç üzerinden anlatılır. Olay panelinde **portre, konuşan kişi, siyasi bağlılık, sorun ve süre sınırı** bulunur. **Üç veya dört seçenek**, bilinen doğrudan etkileri açıklar; gelecekteki belirsiz sonuçlar kesin bilgi gibi sunulmaz.

Bir olay yalnızca genel bir “iyi/kötü” düğmesi değildir. Kim konuşuyor, ne istiyor, kimin konumu güçleniyor ve oyuncu hangi ilişkiyi üstleniyor soruları cevaplanır.

### Kurgusal Necker örneği

Hazine ve ekmek sorunu üzerine **Necker'in konuştuğu kurgusal bir oyun olayı** düşünülebilir. “Kraliyet tahıl ambarlarını aç” seçeneğinin örnek etkileri:

- Gıda **−80**.
- Paris huzursuzluğu **−15**.
- Kentli halk desteği **+12**.
- Kraliyet desteği **−3**.
- Necker ile ilişki **+5**.
- Gelecekte yardımın sürmesine yönelik beklenti.

Bu örnek, doğrulanmış bir Necker konuşması veya tarihsel olay aktarımı değildir. Sayılar mevcut kod değerleri olarak kullanılmamalıdır; amacı kişinin, politikanın, ekonominin ve bölgesel sonucun birlikte gösterilmesidir.

Olay görselleri gravürler, kazıma çizgi hissi veren portreler, mürekkep, resmî belgeler, mum mühürler, el yazısı notlar ve askerî haritalardan esinlenir. Tek renk veya hafif renklendirme tercih edilir; modern tablo estetiği temel yön değildir. Gerçek portre ve belge kullanımlarında kaynak, dönem ve eser niteliği ayrıca doğrulanır.

## 9. Krizler ve bağımsız aktörler

### Kriz göstergeleri

Hedef göstergeler: **Paris'teki baskı, hazine krizi, Üçüncü Zümre'nin meydan okuması ve ordunun sadakati**.

Bu göstergeler, keyfî bir geri sayımdan değil simülasyonun gerçek durumundan türetilir. Paris'teki baskı; gıda açığı, vergi yükü ve radikalleşme gibi nedenlerle artabilir; kontrol, tavizler ve destek gibi etkenlerle azalabilir. İşsizlik de tasarımdaki olası nedenlerdendir, ancak işsizlik sistemi uygulanmadan hesaplanan bir katkı gibi gösterilemez.

Bir kriz “hafta geldiği için” aynı sonucu vermemelidir. Oyuncunun kararları, kaynaklar, bölgeler ve aktörler baskının yönünü değiştirebilmelidir.

### Bağımsız davranışlar

Karakterler ve güçler yalnızca oyuncunun düğmesine tepki vermez. Duruma bağlı davranışlar arasında şunlar bulunur:

- Generalin emri reddetmesi.
- Bakanın istifa etmesi veya taviz istemesi.
- Temsilcinin muhalefet örgütlemesi.
- Bölgede vergiye direnç.
- Askerlerin firar etmesi.
- Yeni siyasi taleplerin ortaya çıkması.

Davranışın nedenleri kişinin çıkarı, sadakati, ilişkileri veya bölgenin ve ordunun durumu üzerinden açıklanır. Uygulanmayan davranış, rastgele bir metin olayıyla çalışan sistemmiş gibi taklit edilmez.

## 10. Her göstergenin açıklaması: şimdi, neden, sonra

Önemli bir gösterge üç soruyu cevaplar:

1. **Şimdi kaç?** Mevcut değeri ve varsa durum eşiği.
2. **Neden böyle?** Hesaplanan katkılar ve yakın zamandaki değişimler.
3. **Sonra ne bekleniyor?** Bilinen koşullara dayalı tahmin ve belirsizlikler.

**Şampanya örneği:** mevcut huzursuzluk **68**; açıklamada gıda **+12**, vergi **+8**, radikaller **+5**, ordu **−7**, yerel seçkinler **−4** gösteriliyor ve sonraki dönem için **72** öngörülüyor.

Bu rakamlar yalnızca istenen açıklama biçiminin örneğidir. Verilen katkıların toplamı **+14** eder; kendi başına ne 68'i ne de 68'den 72'ye geçişi açıklar. Gerçek uygulamada başlangıç değeri, etkenlerin hangi döneme ait olduğu ve tahmin hesabındaki diğer etkiler açıkça belirtilmelidir. Eksik bir hesabı tamamlanmış formül gibi sunmak kabul edilmez.

Harita geri bildirimi ile sayısal açıklama aynı durumu kullanır. Kıtlık simgesi, protesto, riskli yol veya isyan görünümü gösteriliyorsa ayrıntı paneli bunun nedenini açıklayabilmelidir.

## 11. Kampanya ile savaş arasındaki geçiş

Geçiş zinciri:

**Kampanyada düşmanca bölge → orduya odaklanma → askerî rapor → taktik savaş.**

Askerî rapor karşılaşmanın yerini, tarafları, kuvvetleri ve bilinen ikmal koşullarını açıklar. Savaş ayrı bir oyun hissi taşısa da aynı ordunun ve siyasi kararların sonucu olduğu anlaşılır.

Dönüşte kısa rapor, ardından harita gösterilir. **Örnek rapor:** Reims'te zafer; ordu **1140 → 982**; düşman dağıldı; huzursuzluk **−22**; prestij **+4**; yerel hoşnutsuzluk **+3**.

Bu, istenen sonuç sunumunun örneğidir; gerçekleşmiş savaş veya mevcut denge değeri değildir. Huzursuzluk azalırken yerel hoşnutsuzluğun artması gibi farklı sonuçlar ayrı göstergelere ve açık nedenlere bağlanmalıdır. Sonuçlar sefere yalnızca bir kez uygulanır; haritadaki ordu, bölge ve siyasi durum aynı raporla tutarlı güncellenir.

## 12. Gelecekteki kapsamlı geliştirme hedefi

Görsel iyileştirme aşamasından sonraki planlamada değerlendirilecek dört bağlı yükseltme:

| Yükseltme | Diğer sistemlerle kurulması gereken bağlantı |
| --- | --- |
| **Siyasi güçler ve karakterler** | Kararlar; destek, kişisel ilişki, bölgesel durum ve kaynak maliyeti doğurur. |
| **Bölgesel harita katmanları** | Kontrol, huzursuzluk, gıda, vergi, ikmal ve nüfuz aynı bölgesel durumu farklı açılardan açıklar. |
| **Moral ve düzen içeren alay savaşları** | Alayın durumu, komutanı ve ikmali savaşta etkili olur; sonuçlar kampanyaya ve siyasi güce geri döner. |
| **Ordu ikmali** | Üretim, rezervler, bölgesel tutum ve güzergâhlar ordunun hareketini ve savaşma imkânını etkiler. |

Bu hedefler bağımsız menü özellikleri olarak tamamlanmış sayılmaz. Bir sonraki sürümün değerlendirmesinde siyasi bir kararın ekonomiden bölgeye, ikmalden savaşa ve tekrar siyasi güce uzanan sonucu takip edilebilmelidir.

Uygulama sırası, ilk teslimin sınırları ve test adımları [ROADMAP.md](ROADMAP.md) içinde yönetilir. Unity seçildi; oyuncunun somut siyasi kimliği karara bağlanmayı bekler. Sayısal örnekler tasarım niyetini taşır; uygulama, tarihsel doğruluk veya tamamlanmış dengeleme iddiası değildir. Etkin Visual & Feel Polish Pass yeni sistem eklemez.
