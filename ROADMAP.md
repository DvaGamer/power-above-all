# Power Above All — 0.2 Visual & Feel Polish Pass

**Çalışma durduruldu.** Kullanıcının tüm son istekleri, kabul ölçütleri ve yeniden başlama noktası [POLISH_PLAN.md](POLISH_PLAN.md) içinde toplandı. Kullanıcının yeni talimatı olmadan uygulamaya devam edilmez. Olay sistemi polish sonrasındadır.

**Güncel durum:** Unity seçildi; ilk temel aktarım sürüyor. Yeni oyun **Rusça ve Türkçe** olacak. Unity editöründe çalışma, görseller ve oyun derlemesi henüz doğrulanmadı. Doğrulanmış tarayıcı 0.1 kullanılabilir kalır.

**Etkin kapsam: mevcut temelin görsel kalitesi ve oyun hissi. Yeni mekanik veya ekonomi genişletmesi yok.** Önceki geniş kapsamlı tasarım, aşağıda gelecekteki işler olarak korunmuştur. Kullanıcı mevcut durumun GitHub'a anlık görüntü olarak yüklenmesini istedi; yükleme sonucu STATUS.md içine tamamlandıktan sonra yazılır.

## Etkin aşamanın adımları

| Adım | İş | Doğrulama |
| --- | --- | --- |
| 1 — Unity temelini açmak | Mevcut aktarımın derleme ve entegrasyon hatalarını gidermek. | Editörde açılış ve çalışma; saf C# kontrolü tek başına yeterli değildir. |
| 2 — Harita ve belgeler | Mevcut haritanın, panellerin ve göstergelerin hiyerarşisini iyileştirmek. | Harita bağlamı korunur; etiketler okunur; örnek bilgi çalışan sistem gibi gösterilmez. |
| 3 — Savaşın görünümü ve hissi | Mevcut alay seçimlerini, emir geri bildirimini ve tarihsel diorama görünümünü iyileştirmek. | Kontroller anlaşılır; taktik kararlar görünür; yüksek tıklama hızı gerektiren sunumdan kaçınılır. |
| 4 — İki dil | Mevcut metinleri Rusça ve Türkçede kontrol etmek. | Eksik anahtar, taşma ve yanlış biçimlendirme yok; dil değiştirme seferi veya kayıtları değiştirmez. |
| 5 — Çalıştırma ve paylaşım | Unity editörü/testleri, görsel inceleme ve derleme kanıtlarını kaydetmek; kontrol edilmiş aşamayı paylaşmak. | Gerçekte yapılan kontroller ile henüz yapılmayanlar ayrı belirtilir. |

Bu aşama yeni özerk karakter, diplomasi, kaynak zinciri, ikmal ağı veya savaş sistemi eklemek için kullanılmaz. Mevcut temelde hatalı davranışı düzeltmek ve var olan bilgiyi daha iyi göstermek kapsam içindedir.

## Son görsel iyileştirme talebinin kabul ölçütleri

Aşağıdakiler hedef ve inceleme ölçütleridir; tamamlanmış özellik listesi değildir:

- Adlar, emirler, mühürler, askerî raporlar ve üniformalar Fransa/1789 kimliğine uygun olmalı; tarihsel ayrıntılar kaynakla doğrulanmalı.
- Her bölgenin görsel karakteri bulunmalı. Farklı ekranlar aynı kart dizisini tekrarlamak yerine bilgiye uygun, gerektiğinde asimetrik düzen kullanmalı; boş süs alanı eklenmemeli.
- Her ekranda tek baskın odak bulunmalı: seferde Fransa atlası, savaşta birlik hattı, olayda belge, sonuçta askerî rapor.
- Altı–sekiz tutarlı renk, serif başlıklar ve okunur sans serif yardımcı metinler; ortak çizim dilinde özgün simgeler ve imleçler kullanılmalı.
- Oyuncu metinlerinde hata ayıklama terimleri bulunmamalı; kararın etkisi ilgili bölgede görünmeli.
- Başlangıç tarihi kısa girişte anlaşılmalı; kayıt/yükleme dünyayı korumalı ve Rusça/Türkçe dil değişimi kayıtlı içeriğe doğru uygulanmalı.
- Ekran görüntüleri açıklayıcı sunum olmadan incelenmeli: odak, dönem ve bir sonraki eylem anlaşılabiliyor mu? Bu kör inceleme yapılmadan tamamlandı sayılmamalı.
- Hafif kâğıt, kalem, mühür, emir ve savaş geri bildirimleri; sessize alma ve tekrar ses yığılmasına karşı sınırlama bulunmalı. Yüksek sesli yapay epik müzik hedeflenmiyor.

`CabinetAudio.cs`, geçici olarak özgün biçimde üretilen **10 prosedürel foley taslağı** içerir. Bu, 10–15 profesyonel ses kaydının hazırlanmış olduğu veya seslerin Unity içinde dinlenerek doğrulandığı anlamına gelmez. Sahneye bağlama ve işitsel değerlendirme ayrıca doğrulanır.

Tarayıcı 0.1'deki ikinci hafta `grain-petition` kararı, Unity aktarımının mevcut özellik eşitliği kontrolüdür. Bu kararın geri getirilmesi yeni olay sistemi, yeni olay türü veya yeni süre sınırı ekleme yetkisi değildir; üç mevcut seçenek, zamanın karar beklemesi ve kayıtta tek sefer çözülmesi korunur. Doğrulama sonucu STATUS.md içinde ayrıca kaydedilir.

**Olay sistemi için zamanlama kararı kesinleşti:** dünya koşullarına tepki veren olaylar **görsel iyileştirmeden sonra** ele alınacak. Tepki, kriz, fırsat ve zincir sınıfları; öncelik/tekrar kontrolü ve gelecekteki 30–50 nitelikli durum hedefi [EVENT_DIRECTION.md](EVENT_DIRECTION.md) içinde kaydedildi. Şimdi yalnızca mevcut sabit ikinci hafta kararının eşitliği korunur; yeni olay yöneticisi eklenmez.

## Kararların güncel durumu

- Başlangıç: Fransa, 5 Mayıs 1789.
- Kimlik: kraliyet çalışma odası + tarihî atlas + eski düzen çökerken askerî raporlar.
- Oyuncunun amacı kişisel siyasi hayatta kalma ve devlet üzerindeki kontrolünü artırmaktır.
- Harita sürekli bağlam sağlar; ayrıntılar belge benzeri panellerde açılır.
- Savaş kontrol birimi alaydır. Warcraft III yalnızca emirlerin tepkiselliği için referanstır.
- Motor Unity; yeni oyunda Rusça ve Türkçe zorunlu.
- Bir sonraki aşama Visual & Feel Polish Pass; dört bağlantılı sistemin genişletilmesi gelecekteki iş listesinde.
- Önceki “yalnızca siyaset veya savaş veya ekonomi” yol ayrımı, yeni v0.2 hedefiyle geçersiz olmuştur.

Ana gereksinimler [DESIGN_V0.2.md](DESIGN_V0.2.md), görsel kurallar [ART_DIRECTION.md](ART_DIRECTION.md), referans görevleri [REFERENCES.md](REFERENCES.md) içindedir.

## Açık kalan tasarım kararı

**Oyuncu kimliği:** kurgusal nüfuzlu konsey üyesi, XVI. Louis veya Jacques Necker seçenekleri henüz kesinleşmedi. Bu durum motor seçiminin açık olduğu anlamına gelmez; mevcut görsel iyileştirme aşaması bu seçenekleri tamamlanmış oyun rolleri olarak sunmaz.

Önceki “bir tarayıcı aşaması daha” önerisi, kullanıcının hemen Unity kararıyla geçersiz oldu. Tarayıcı 0.1, mevcut doğrulanmış sürüm ve karşılaştırma kaynağı olarak tutulur.

## Gelecekteki geniş kapsam — etkin aşamaya dahil değil

Aşağıdakiler önceki kapsamlı planın korunan hedefleridir; Visual & Feel Polish Pass sırasında yeni özellik olarak uygulanmaz. Bazılarının temeli aktarılmış olabilir; bu, bütün hedefin tamamlandığı anlamına gelmez:

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

## Gelecekteki sistem genişletmesinin kontrol noktaları

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

## Gelecekteki bağlı sistemler için örnek kabul senaryosu

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

Unity seçildi. Etkin aşamada işler mevcut temelin entegrasyonu, görsel düzen, kontrol hissi ve iki dilde doğrulama olarak sınırlandırılır. Gelecekteki mekanikler ayrı görevlerde tutulur. Dosya sahipliği kalıcı değildir; paylaşılan veri sözleşmesi değişiklikleri önceden kararlaştırılır.

Her kontrol noktasında eldeki çalıştırılabilir sürüm, kısa değişiklik kaydı ve gerçek doğrulama sonucu sunulur. Unity sürümü henüz çalıştırılmadıysa bu durum açıkça belirtilir. Aynı PR'a ilgisiz özellikler eklenmez. Takvim, editör doğrulaması ve arkadaşların üstleneceği işler netleşmeden kesinleştirilmez.
