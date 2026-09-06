# Power Above All — bütünsel oyun vizyonu

## 6 Eylül 2026 — yeni kesin kullanıcı ilkeleri

**Tek sürekli gerçek-zaman dünyası.** Hafta tur değildir; ayrı taktik arena yoktur. Saat duraklatılır veya 1/3600/86400 oyun saniyesi hızıyla ilerler. Fiziksel ordular, yollar ve muharebeler aynı haritadadır. Ekonomi ve politika çatışma sırasında devam eder. Eski aşağıdaki görüşme seçenekleri bu kesin kullanıcı kararını geçersiz kılamaz. Öncelik artık dengeli, anlaşılır ve yeniden oynanabilir bir sefer döngüsüdür; ayrıntılı kapsam [REALTIME_ARCHITECTURE.md](REALTIME_ARCHITECTURE.md), doğrulama [STATUS.md](STATUS.md).

**PLAYER ≠ STATE.** Oyuncu fiziksel bir yerde bulunan tek kişidir; kamera konumu varlık veya bilgi değildir. Ülkeyi doğrudan değiştirmek yerine insan, yetki, emir, gecikme, yorum, eylem ve geri gelen rapor üzerinden yönetir. Bilgi kaynak ve tarih taşır. Belirsizlik açıklanabilir olmalı, gecikme karar üretmeli; kurye mikro yönetimi ve boş bekleme hedef değildir.

Uzun vadeli yön: kişisel seyahat/başkentten ayrılma bedeli; yetki ve sürekli direktifler; yetenekli ama siyasi açıdan tehlikeli generaller; fiziksel HQ, kısa okunur command delay ve son görevi sürdüren yerel komutanlar. Kullanıcı APM üstünlüğü yerine hazırlık, yedek, komuta yerleşimi ve zamanında plan değişimi istiyor. Ayrıntı ve gerçek küçük uygulama [COMMAND_SLICE.md](COMMAND_SLICE.md); hedeflerin tümü uygulanmış sayılmaz.

Coğrafi mimari küresel, oynanabilir ayrıntı Fransa ve gerekli komşularda. Gerçek GIS ve kaynaklı1789 sınırı ayrı. Final grafik constructive/manual; image generation yalnız geçici reference olabilir. Büyük yeni içerikten önce kamera, semantic zoom, harita ve RU/TR UI kalite milestone'u. Kitaplar/hatırat/mektuplar tarihî kanıt ve atmosfer olarak ayrı incelenir.

**Durum: ilk tur tamamlandı; kullanıcı on saatlik özerk geliştirme istedi.** Başlangıç: 6 Eylül 2026. Bu belge uzun vadede nasıl bir oyun istendiğini belirler. Yeni kullanıcı talimatı geliştirmeyi yeniden başlatır; görüşmenin açık seçenekleri yine karar sayılmaz. Gece boyunca gerekli tercihler çalışma varsayımı olarak ayrıca kaydedilir. Görüşme Rusça, depo kaydı Türkçe tutulur.

## Görüşmenin amacı ve yöntemi

Oyuncunun rolünü, temel deneyimini, kampanyanın işleyişini, sistemlerin ilişkisini, görünümü ve sesi tek bir tutarlı vizyonda birleştirmek. Her turda yaklaşık üç somut seçim sunulur; sonraki sorular önceki yanıtlara göre uyarlanır. Kullanıcı seçenekleri birleştirebilir veya kendi yanıtını yazabilir. Birleşimlerde birincil yön ve destekleyici yön ayrıca belirlenir.

Bir seçim kaydedilirken oyuncunun göreceği davranış, doğuracağı tasarım gereksinimi ve varsa diğer seçimlerle gerilimi belirtilir. Öneriler, kullanıcının kesin seçimlerinden ayrı tutulur. Uzun vadeli hedef, ilk oynanabilir kesit ve üretim sırası ayrı kararlaştırılır.

## Önceki kararlardan gelen başlangıç zemini

Bunlar mevcut belgelerde kayıtlı yönlerdir; bu görüşmede yeniden verilmiş yanıtlar değildir. Kullanıcının yeni açık kararları önceki tasarım tercihlerini değiştirebilir.

- Unity; oyun metinleri Rusça ve Türkçe, depo belgeleri Türkçe.
- Başlangıç sahnesi Fransa, 5 Mayıs 1789; oyuncunun kesin siyasi kimliği açık.
- Kişisel siyasi varlık ve devlet üzerindeki kontrol önemli; ülkenin yararı ile oyuncunun çıkarı çatışabilir.
- Siyaset, ekonomi, bölgeler, ordu ve savaş birbirini etkiler. Ana ağırlığı seçmek diğer sistemleri kaldırmak anlamına gelmez.
- Harita ana çalışma alanı; tarihî atlas, kabine belgeleri, koyu yeşil, kâğıt ve mat altın mevcut görsel temel.
- Savaşta alay komutası, moral, arazi, hazırlık ve zamanlama; yarı stilize tarihî diorama yönü.
- Gelecekteki olaylar dünya durumuna ve önceki kararlara tepki verir; keyfî tekrar yerine yaşayan sonuçlar hedeflenir.

Kaynaklar: [tasarım](DESIGN_V0.2.md), [sanat yönü](ART_DIRECTION.md), [referansların görevleri](REFERENCES.md), [olay yönü](EVENT_DIRECTION.md), [önceki yol haritası](ROADMAP.md). Bu belgelerdeki eski sürüm sınırları nihai oyunun büyüklüğüne otomatik sınır koymaz. Gerçek uygulama durumu [STATUS.md](STATUS.md) ve [NOTES.md](NOTES.md) üzerinden izlenir.

## Görüşme kapsamı

| Blok | Netleştirilecek konular | Beklenen çıktı |
| --- | --- | --- |
| 1. Oyuncu ve ana deneyim | Başlangıç rolü; ana haz; tarihsel akıştan sapma özgürlüğü | Oyunun kısa tanımı ve rolün anlamı |
| 2. Kampanya | Coğrafya; tarih aralığı; parti süresi; zamanın akışı | Kampanyanın ölçeği ve ritmi |
| 3. Güç ve devamlılık | Başarı, yenilgi; görevden düşme; sürgün; karakterin veya hanedanın devamı | Oyuncunun neyi riske attığı |
| 4. Günlük oynanış | Oyuncunun tekrar eden işleri; emir yetkisi; bilgi ve belirsizlik; karar sıklığı | Örnek on dakikalık oynanış ve temel döngü |
| 5. Siyaset ve kişiler | Gruplar, kişilikler, ilişkiler, pazarlık, bağımsız aktörler, olay anlatımı | Kişisel iktidarın nasıl kazanılıp kaybedildiği |
| 6. Ekonomi ve toplum | Yönetim ayrıntısı; reformlar; bölgesel farklar; lojistik; dış ilişkiler | Savaş ve siyaseti besleyen neden-sonuç bağları |
| 7. Ordu ve savaş | Savaş sıklığı; doğrudan komuta; zaman ve duraklatma; savaşın ölçeği ve sonuçları | Savaşın kampanyadaki yeri ve taktik his |
| 8. Görsel dünya | Harita, yakınlaşma, kamera, yerleşimler, portreler, savaş sahnesi, arayüz | Ekran bazında somut görsel hedef |
| 9. Anlatım ve ses | Duygusal ton; metin yoğunluğu; müzik; ortam; konuşmalar | Oyunun atmosferi ve duyusal dili |
| 10. Oyuncuya erişim | Öğrenme; zorluk; açıklamalar; arayüz okunurluğu; hedef cihaz ve giriş | Karmaşıklığın nasıl anlaşılır kalacağı |
| 11. Tekrar oynama ve sınırlar | Senaryolar; farklı roller; tek/çok oyunculu beklentisi; kapsam dışı fikirler | Kalıcı hedefler ve bilinçli sınırlar |
| 12. Üretime çeviri | Ayırt edici temel özellikler; ilk örnek senaryo; sürüm sırası; kabul ölçütleri | Tam vizyonu temsil eden küçük oynanabilir kesit |

Bu tablo sabit bir soru listesi değildir; yanıtlar gereksiz soruları kaldırabilir veya yeni bir karar gerektirebilir.

### Ön incelemeden çıkan görüşme notları

- Rolün yetkileri, görevden düşünce devam edip etmeme, kampanyanın tarih aralığı ve başarı tanımı öncelikli açık kararlardır. Niyetler, yalan ve gizli bilgi ile açıklanabilir sonuçlar arasındaki denge ayrıca sorulmalıdır.
- On iki bölge, dört başlangıç siyasi gücü, tek savaş alanı ve yaklaşık 8–12 alay önceki küçük teslimat hedefleridir; nihai oyunun kesin sınırları değildir. Tam dış politika ve çok oyunculu modun önceki teslimattan çıkarılması, uzun vadeli vizyondan kesin olarak çıkarıldıkları anlamına gelmez.
- Görsel temel korunarak portre tekniği, savaş kamerasının özgürlüğü, haritada yakınlaşma derinliği, arayüz yoğunluğu ve siyasi görüşmelerin sahnelenme derecesi seçilebilir. Bunlar henüz kullanıcı kararları değildir.
- Sessiz ve kısa arayüz sesleri önceki yöndür; bütün kampanyanın müzik dramaturjisi henüz seçilmedi. On prosedürel ses taslağı bitmiş ses tasarımı değildir.
- Eski tasarım belgelerinin girişlerindeki uygulama durumu güncel olmayabilir. Son izole derleme/test kanıtları `NOTES.md` içindedir; tam görsel ve işitsel kabul tamamlanmış sayılmaz.

## Birinci tur — yanıtlandı

### 1. Başlangıç rolü

- A: Sarayda nüfuzlu, sınırlı yetkili özgün bir karakter; ittifak ve entrikalarla en yüksek iktidara yükselme.
- B: Mevcut hükümdar; kriz karşısında iktidarı koruma veya rejimi dönüştürme.
- C: Üst yönetimin dışında başlayan siyasi aktör; taraftar toplayıp devrim sırasında yükselme.
- D: Kampanya başında farklı roller seçme; hükümdar, saray aktörü, devrimci veya asker için farklı yetki ve amaçlar.

**Kullanıcı kararı — 6 Eylül 2026:** D; kampanya başlamadan farklı roller arasından seçim yapılır. Kullanıcının yanıtı: «Выбираю разные роли перед партией». Kesin rol listesi, tarihsel/özgün karakter seçimi ve rollerin mekanik farkları henüz onaylanmadı. Seçenekteki hükümdar/saray aktörü/devrimci/asker örnekleri tamamlanmış veya kesinleşmiş kadro sayılmaz.

### 2. Oyuncuyu geri getiren ana deneyim

- A: Kişisel iktidar mücadelesi, bağımlılıklar, tehlikeli pazarlıklar, ihanetler ve rakipleri aşma.
- B: Ülkeyi krizden çıkarma; reformların ekonomi, bölgeler ve toplumu değiştirmesi.
- C: Sefer hazırlığı, stratejik manevra ve taktik savaş kararları.
- D: İlişkiler, zor seçimler ve beklenmeyen sonuçlarla oluşan kişisel siyasi hikâye.

**Kullanıcı kararı — 6 Eylül 2026:** A + B + C birlikte. Kişisel iktidar mücadelesi, ülke yönetimi ve reformlar, büyük strateji ve savaşlar ana deneyimin parçalarıdır. Kullanıcı üçünü birlikte istedi; bunlardan tek birini ana oyun olarak seçip diğerlerini kaldırma yönü yoktur. Kesin zaman payları veya her rolün bütün sistemlerde doğrudan yetkili olması onaylanmış değildir. D seçeneği hakkında ayrıca karar verilmedi; bu sessizlik kişisel hikâyeyi dışlama anlamına gelmez.

**Vizyona etkisi:** bu üç alan ortak bir kampanyada birbirini beslemeli. Örnek tasarım zinciri: siyasi anlaşma seferi finanse eder → askerî başarı devleti güçlendirir → başarılı komutanın artan nüfuzu oyuncuya siyasi tehdit oluşturabilir. Bu bir tasarım örneğidir; uygulanmış özellik veya tek zorunlu olay değildir.

### 3. Tarihin serbestliği

- A: Tarihsel başlangıçtan sonra koşullar ve kararlarla serbest gelişim; devrimi değiştirme veya önleme olanağı.
- B: Tanınabilir büyük tarihsel krizler; sonuçları, katılımcıları ve oyuncunun konumu değişebilir.
- C: Dünya içinde gerekçelendirilen daha geniş alternatif tarih ve farklı rejim yolları.

**Kullanıcı kararı — 6 Eylül 2026:** C; geniş alternatif tarih, ancak kendi sonuçları ve gerçek tarihten gelen dengeyle. Kullanıcı büyük sapmaları, farklı rejimleri ve iddialı yolları dünya içinde ikna edici olmaları koşuluyla seçti; ardından «Пусть алтернатива будет но со своими последствиями и с балансом от реальной истории» diye netleştirdi.

**Tasarım yorumu:** dönemin imkânları, kurumlar, toplumsal çıkarlar, ekonomi ve uluslararası çevre alternatif yolların koşullarını ve maliyetlerini belirler. Farklı bir yol kazanç sağlayabilir; destek, kaynak, zaman veya meşruiyet gerektirebilir ve sonraki sorunları değiştirebilir. Bu ifade her sapmanın otomatik cezalandırılması, tarihe zorla geri dönülmesi veya her alternatif sonucun eşit güçte olması olarak yorumlanmaz. Kesin tarihsel sınırlar ve denge kuralları ileride örneklerle netleştirilecek; tarihsel veri gerektiğinde kaynakla doğrulanacak.

## İkinci tur — karakterler, kapsam ve zaman

### Görüşme sırasında gelen görsel karar

Kullanıcı 6 Eylül gecesi tam gerçekçilik istemediğini; yer yer aydınlık ve gerektiğinde karanlık, hoş ve hafif tatlı bir renk dünyası istediğini belirtti. Sonucun rastgele yapay zekâ üretimi gibi görünmemesini istedi. Çalışma sanat yönü `ART_DIRECTION.md` başında somutlaştırıldı: güneşli guaj atlas, yaşayan minyatürler, sıcak kâğıt/adaçayı/mavi/mercan ve koyu orman/şarap gölgeleri. Bu kavram ajanın uygulama tercihidir; kullanıcının birebir seçtiği ad veya tamamlanmış görsel kabul değildir.

Bu sorular tamamlanmış oyunun hedefini belirler; ilk sürümün üretim kapsamı ayrıca seçilecek. Her başlık henüz kullanıcı yanıtı bekler.

### 4. Oynanabilir kişiler

- A: Dönemin tarihsel kişileri; gerçek başlangıç konumları ve bağları, sonrasında değişebilen kader.
- B: Oyuncunun oluşturduğu özgün kişi; köken, özellikler ve başlangıç rolü seçilir, tarihsel kişiler aynı dünyada bulunur.
- C: İkisi birlikte; tarihsel bir kişi veya özgün karakterle başlama seçimi.

**Kullanıcı kararı:** bekleniyor.

### 5. Tam oyunun coğrafi kapsamı

- A: Ayrıntılı Fransa; dış dünya diplomasi, ticaret, savaş ve baskı yoluyla etkiler, aynı ayrıntıda yönetilebilir değildir.
- B: Fransa merkezli ayrıntılı Avrupa; komşular ve kıta savaşları haritada, Avrupa dışındaki bölgeler daha soyut bağlantılarla temsil edilir.
- C: Küresel harita; Avrupa yanında sömürgeler, deniz yolları ve denizaşırı çıkarlar da stratejik alan olur. Bölgesel ayrıntı düzeyi ayrıca seçilir.

**Kullanıcı kararı:** bekleniyor. Haritanın kapsamı, her ülke veya kişinin oynanabilir olmasını kendiliğinden kesinleştirmez.

### 6. Kampanya zamanının ilerleyişi

- A: Gerçek zaman, duraklatma ve hız ayarı; dünya sürekli ilerler, oyuncu duraklatınca düşünür ve emir verir.
- B: Turlar; oyuncu kararlarını hazırlar, turu bitirir ve diğer aktörlerle birlikte sonuçlar hesaplanır. Turun kaç gün/hafta olduğu ayrıca seçilir.
- C: Planlama ve uygulama evreleri; oyuncu duraklamada emir paketini hazırlar, ardından belirli bir süre dünyadaki eşzamanlı gelişmeleri izler; müdahale kuralları ayrıca seçilir.

**Kullanıcı kararı:** bekleniyor. Bu seçim kampanya içindir; taktik savaşın zaman akışı ayrıca sorulur.

## Birleştirilecek nihai vizyon

Gece geliştirmesinde kararları somutlaştıran [oynanış ve görsel dünya taslağı](GAME_VISION_DRAFT.md) oluşturuldu. Onaylı yanıtlarla çalışma önerileri açıkça ayrılır. Bu taslak aşağıdaki açık kullanıcı seçimlerini doldurulmuş saymaz.

Yanıtlar yeterince olgunlaştığında bu kayıt şu somut bölümlere dönüştürülecek:

1. Tek paragraflık oyun tanımı ve oyuncuya verilen temel vaat.
2. Üç ila beş ayırt edici tasarım ilkesi.
3. Bir partiye giriş, örnek on dakika, orta oyun ve olası bitişler.
4. Siyaset → ekonomi → bölge → askerî karar → sonuç → kişisel iktidar zincirini gösteren örnek.
5. Harita, siyaset, olay, savaş ve rapor ekranları için görünüm ve etkileşim tanımı.
6. Ses, metin, tempo, öğrenme ve zorluk yönü.
7. Tam oyun hedefi, ilk oynanabilir kesit, sonraki aşamalar ve kabul ölçütleri.
8. Açık sorular, kapsam dışı tercihler ve kararların gerekçeleri.

Kesinleşen kararlar: kampanya öncesinde rol seçimi; kişisel iktidar, ülke yönetimi ve büyük strateji/savaşların birlikte temel deneyimi oluşturması; gerçek tarihten gelen denge ve yaşayan sonuçlarla geniş alternatif tarih. Diğer başlıklar görüşmeyle doldurulacak.

## Sonraki turlar için hazırlanan soru — rollerin birbirinden farkı

Bu soru hazırlanırken kullanıcı ilk turun kalan sorularını da yanıtladı. Rol farkı sorusu henüz gönderilmedi; ikinci tur karakterler, coğrafya ve kampanya zamanını kapsar. Sonraki sorular tek tek veya kullanıcı yanıt hızına uygun küçük gruplarla gönderilir.

- A: Her rolde ortak iktidar ve kişisel siyasi varlık mücadelesi; başlangıç yetkileri, ilişkileri ve yükselme yolları farklı.
- B: Role göre belirgin farklı amaçlar ve günlük eylemler; hükümdar yönetir, muhalif hareket kurar, asker sefer ve ordudaki nüfuzla uğraşır. Roller aynı dünyada birbirini etkiler.
- C: Başlangıç rolü bir çıkış noktasıdır; oyuncu parti içinde meslek, kamp ve amaç değiştirerek kendi yolunu kurar. Sabit rol hedefi belirleyici değildir.

**Kullanıcı kararı:** bekleniyor. Bu seçenekler ağırlık merkezini sorar; A veya B seçimi kendiliğinden rol değişimini yasaklamaz, C seçimi farklı yetkileri ortadan kaldırmaz.
