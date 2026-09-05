# Power Above All — v0.2 uygulama planı

**Durum: taslak; teknoloji ve oyuncu kimliği için proje sahibinin seçimi bekleniyor.** Yeni oyun özelliklerinin uygulanmasına henüz başlanmadı. Türkçe ekip belgeleri ve tasarım gereksinimlerinin kaydı mevcut kapsamda tamamlanır.

## Kararların güncel durumu

- Başlangıç: Fransa, 5 Mayıs 1789.
- Kimlik: kraliyet çalışma odası + tarihî atlas + eski düzen çökerken askerî raporlar.
- Oyuncunun amacı kişisel siyasi hayatta kalma ve devlet üzerindeki kontrolünü artırmaktır.
- Harita sürekli bağlam sağlar; ayrıntılar belge benzeri panellerde açılır.
- Savaş kontrol birimi alaydır. Warcraft III yalnızca emirlerin tepkiselliği için referanstır.
- Bir sonraki aşama dört bağlantılı geliştirmeyi kapsar: siyasi güçler/karakterler, bölgeler/harita kipleri, alay savaşı/moral/düzenler ve ikmal.
- Önceki “yalnızca siyaset veya savaş veya ekonomi” yol ayrımı, yeni v0.2 hedefiyle geçersiz olmuştur.

Ana gereksinimler [DESIGN_V0.2.md](DESIGN_V0.2.md), görsel kurallar [ART_DIRECTION.md](ART_DIRECTION.md), referans görevleri [REFERENCES.md](REFERENCES.md) içindedir.

## Açık kalan seçimler

1. **Teknoloji:** mevcut tarayıcıda sınırlı v0.2 veya şimdi Unity'ye geçiş.
2. **Oyuncu kimliği:** kurgusal nüfuzlu konsey üyesi, XVI. Louis veya Jacques Necker.

Öneri: **bir sınırlı tarayıcı v0.2 + kurgusal konsey üyesi**. Böylece siyasi amaç ve nedensel döngü sınanır. Unity'ye geçildiğinde arayüz ve uygulama kodunun bir kısmının yeniden yazılacağı kabul edilmelidir. Hemen Unity seçilirse ilk teslimat, açılıp derlenebilen proje ve mevcut çalışan döngünün taşınması olur.

## Önerilen ilk kapsam

Aşağıdakiler plan önerisidir; uzun vadeli tasarım belgesinin kapsamını silmez:

- Mevcut 12 bölge üzerinde çalışma; Paris–Champagne ekseninde odaklı bir öğretici kriz.
- Başlangıç için dört siyasi güç: taht, temsilciler, kent halkı, ordu. Her birinin lideri; ayrıca oyuncu karakteri.
- Soylular, ruhban ve kırsal halk daha sonraki genişletmede ayrı güçler hâline gelir. Kırsal nüfus ekonomik modelde bulunabilir; ayrı siyasi aktör olarak tamamlanmış gösterilmez.
- Dört kaynak: hazine, gıda, askerî ikmal malzemesi, insan gücü.
- Temel bölge alanları ve altı kip: siyasi kontrol, huzursuzluk, gıda, vergi, ordu/ikmal, siyasi nüfuz.
- Tek bir savaş alanı, toplam yaklaşık 8–12 alaylık başlangıç hedefi. Piyade, milis, süvari ve topçu; hat, kol ve kare düzenleri.
- Moral, ikmal ve komutan tutumu savaş sonucuna etki eder. Görsel asker sayısı ve gerçek mevcudun ölçeği açıklanır.
- Paris krizi ve ordu sadakati için koşullardan hesaplanan iki baskı göstergesi.
- Kısa bir seferde baştan sona gösterilebilen en az bir neden-sonuç zinciri ve savaşı önleyebilen siyasi bir alternatif.

Kesin asker sayıları, denge değerleri ve senaryo süresi ilk ölçüm ve rol seçiminden sonra belirlenir. Hanedan ağacı, yüzlerce ürün, tam dış politika ve çok oyunculu mod bu teslimata dahil edilmez.

## Uygulama sırası ve kontrol noktaları

| Adım | Teslimat | Tamamlanma ölçütü |
| --- | --- | --- |
| 0 — Rol ve teknik temel | Rolün yetkileri, yenilgi/başarı koşulları, veri sözleşmesi; seçilen teknolojide çalışan proje | Mevcut harita → emir → hafta → savaş sonucu → kayıt döngüsü korunur. |
| 1 — Harita ve bilgi düzeni | Harita üstü belge panelleri, kip seçimi, siyasi güç/karakter ve haftalık ekonomi görünümü | Harita bağlamı kaybolmaz. Gerçekleşen değer, değişim nedeni ve tahmin ayrılır; henüz hesaplanmayan veri çalışan özellik gibi sunulmaz. |
| 2 — Siyasi bedel ve yükümlülük | Dört güç, liderler, talepler, oyuncuya tutum, örnek ekmek sübvansiyonu ve devam yükümlülüğü | Aynı karar bir tarafın desteğini artırırken başka bir tarafın direncini veya gelecek maliyeti artırır. |
| 3 — Bölgeler ve ikmal | Bölgesel üretim/stok, vergi etkinliği, kontrol, güzergâh ve ordunun iaşesi | Bölgesel direnç veya eksiklik ikmali etkiler; oyuncu hangi kararın bu duruma yol açtığını görebilir. |
| 4 — Alay savaşı | Alay kartları, çoklu seçim, önizlemeli düzen emri, dört birlik türü, moral ve ateş emirleri | Hat/kol/kare ve yan kanatlar anlamlıdır. Bozgun, bütün askerler ölmeden savaşı bitirebilir. İkmal ve komutan önceki adımdan gelir. |
| 5 — Siyasi sonuç ve aktör iradesi | Kısa askerî rapor, nüfuz/sadakat değişimi, çıkarına göre talep veya emir reddeden aktör | Savaşın siyasi bedeli vardır. En az bir aktör oyuncunun doğrudan emri olmadan koşullara tepki verir. |
| 6 — Birleşik senaryo | Kayıt uyumluluğu, açıklamalar, olay metinleri, kısa sefer ve ekip denemesi | Karar → ekonomi → bölge → ikmal → savaş → siyasi güç zinciri baştan sona oynanır ve kayıttan sürdürülebilir. |

Adımlar ayrı kontrol noktalarıdır; dört sistemin birbirinden kopuk dört ayrı prototipe dönüşmesi hedeflenmez. Arayüz adımında geçici örnekler kullanılırsa açıkça tasarım örneği olarak işaretlenir.

## Örnek kabul senaryosu

Oyuncu Paris'te ekmeği sübvanse eder. Kent desteği artar, gelecekteki gıda/para yükümlülüğü doğar. Bunu karşılamak için Champagne üzerinde baskıyı artırır. Yerel direnç vergi veya ikmal akışını bozar. Ordu daha düşük ikmal ve moralle çatışmaya girer. Yenilgi ya da pahalı zafer komutanın tutumunu ve saraydaki rakiplerin nüfuzunu değiştirir.

Bu zincir sabit turda zorla başlatılan bir senaryo olmamalıdır. Sistem koşulları ve oyuncu tercihleri üretmelidir. Oyuncu pazarlık, vergi tercihi, sübvansiyonu değiştirme veya askerî yaklaşım yoluyla sonucu etkileyebilmelidir.

## Veri ve arayüz ilkeleri

- Bütün önemli değerler için şimdi / neden / sonraki dönem bilgisi bulunur.
- Dönemsel değişimler başlangıç stokundan ayrı gösterilir; değişimlerin toplamı tek başına mevcut değeri açıklıyormuş gibi sunulmaz.
- Tahmin, mevcut emirler ve bilinen koşullara dayanır; belirsiz olaylar garanti edilmiş sonuç gibi gösterilmez.
- Bir baskı göstergesine yalnızca modelde gerçekten hesaplanan etkenler girer. İşsizlik hesaplanmıyorsa sahte işsizlik katkısı yazılmaz.
- Savaş sonucu ve yükümlülük etkileri bir kez uygulanır; kayıt/yükleme aynı durumu geri getirir.
- Dönemsel olay veya Necker gibi bir kişinin örnek diyaloğu, tarihî alıntı olarak sunulmaz.

## Ekip iş bölümü

Teknoloji ve rol seçildikten sonra somut Issues açılır. Çekirdek simülasyon, harita/arayüz, taktik savaş ve içerik/araştırma görevleri ayrı kapsamlarla yürütülebilir. Dosya sahipliği kalıcı değildir; paylaşılan veri sözleşmesi değişiklikleri önceden kararlaştırılır.

Her kontrol noktasında oynanabilir sürüm, kısa değişiklik kaydı ve doğrulama sonucu sunulur. Aynı PR'a ilgisiz özellikler eklenmez. Takvim, seçilen teknoloji ve arkadaşların üstleneceği işler netleşmeden kesinleştirilmez.
