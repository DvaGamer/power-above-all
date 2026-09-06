# Power Above All — 0.2 Visual & Feel Polish Pass

## Çalışma durumu

**6 Eylül 2026: Kullanıcı on saatlik özerk geliştirmeyi başlattı.** Ana kaynak baseline-visible güvenli kapısından geçti: 23 Unity testi, taze player, 20 gerçek kare ve 26 durum kontrolü. Önceki shader düzeltmesi taşındı. Yeni aydınlık/koyu, hoş ve stilize görsel yön `ART_DIRECTION.md` başındadır. Polish'in bütünü henüz tamamlanmış sayılmaz; canlı sıra `NIGHT_QUEUE.md`, kanıt `NIGHT_LOG.md` ve `NOTES.md` içindedir.

Unity 6000.3.23f1 seçildi ve kuruldu. Proje `Unity/`, başlangıç sahnesi `Assets/Scenes/Main.unity`; açıcı `OPEN_UNITY.cmd`. Yeni oyun Rusça ve Türkçe, ekip belgeleri Türkçedir. GitHub deposunun **Public kalması** son kullanıcı kararıdır. Tarayıcı 0.1 karşılaştırma kaynağı olarak korunur.

## Bu aşamanın sınırı

**Yeni mekanik eklemeden mevcut harita → emir → hafta → olay → yürüyüş → savaş → sonuç döngüsünü keyifli hâle getirmek.** Ekonomi kuralları ve dengeyi değiştirmemek; yeni birlik türü, kaynak, diplomasi, olay yönetmeni veya başka alt sistem eklememek. Var olan davranış hatalarını düzeltmek ve mevcut olayı Unity'ye taşımak yeni sistem genişletmesi değildir.

Amaç yalnızca güzellik eklemek değil; geçici, standart veya rastgele görünen çözümleri kaldırmaktır. Ekranları ticari oyun kalitesi hedefiyle incelemek, hedefe ulaşıldığını kanıt olmadan söylememek.

## Kimlik ve tasarım disiplini

- Fransa, **5 Mayıs 1789**, Fransız Devrimi. Fransa ana harita ve içerik odağıdır.
- Kraliyet çalışma odası, devlet atlası, askerî haritalar ve eski düzenin çöküşü sırasında yazılmış siyasi belgeler.
- Koyu orman yeşili, fildişi kâğıt, mat altın, ölçülü kırmızı; yaklaşık 6–8 anlamlı ana renk. Dünya ve tarih için bir serif, sayılar ve kontrol için bir sans.
- Gravür, mürekkep, litografi, mum mühür, kumaş, pirinç, el notları ve askerî kartografya. Fantastik parşömen, Orta Çağ ahşabı, parlak mobil renkler ve hazır strateji UI-kit görünümü yok.
- Yarı stilize tarihî diorama; yaşayan minyatür ordular. Doğal yumuşak renkler, açık siluetler ve uzun oturumlarda rahat okunabilirlik. Fotogerçekçilik veya çizgi film hedeflenmez.
- Her büyük karar için Power Above All'a özgü gerekçe olmalı. Referansların ilkeleri alınır; varlıkları veya ekran düzenleri kopyalanmaz.
- Sabit talimat: **Never implement the first obvious solution. For major features, generate 3 materially different concepts internally and implement the one that best reinforces Power Above All's identity.**
- Kontrol soruları: İlginç mi? Bu oyuna özgü mü? Başka sistemle bağlantılı mı? Oyuncu bundan bir hikâye anlatabilir mi? Tek ekran veya 30 saniyede tanınabilir mi? İlk hazır şablon gibi görünüyorsa yeniden ele alınmalı.

## Harita ve bölge seçimi

- Harita ana çalışma alanı olarak kalır; belgeler onu tamamen kapatmaz. Daha doğal kıyı, bölge şekilleri, ince sınırlar, okunur coğrafi yazılar, başkentler ve şehir işaretleri.
- Hafif kâğıt dokusu, rölyef/tarama; nehir ve yollar bu aşamada görsel bağlamdır, yeni taşıma sistemi değildir. SVG taslağı hissini azaltmak.
- Bölgelerde görsel karakter: uygun peyzaj, mimari, kısa yerel tanım veya özgün simge. Paris, liman ve kırsal alan aynı görünmemeli. Tarihî arma veya üniforma ayrıntıları doğrulanmalı; kurgu gerçek gibi sunulmamalı.
- Seçilen bölge hafif yükselme, kontur ve yumuşak aydınlanmayla hissedilir; yan belgeyle bağlantısı açıktır. Hover tıklamadan önce etkileşimi gösterir.
- Harita kipleri, seçilme ve huzursuzluk gibi mevcut durumlar arasında sakin geçiş. Mevcut veri dışında işleyen sistem varmış izlenimi veren katman yok.
- Ordu belirgin ve dünyaya ait bir sancak/standart olmalı. Yürüyüş **0,5–1 saniyelik** fiziksel hareket olarak gösterilir; oyun hesabı animasyona bağlanmaz.
- Ekmek, vergi veya başka emir sonrası etki ilgili bölgede görünür; sadece üstteki sayı değişmez.

## Arayüz, tempo ve tutarlılık

- Bölge seçildiğinde yapılabilecek emirler hemen görünür. Gereksiz tıklama, bekleme ve kaydırma azaltılır; savaş sonundan sefer haritasına dönüş kısa tutulur.
- Kullanılamayan her eylem nedenini söyler: gıda yetersiz, emir kullanıldı, ordu burada değil, komşu değil, hareket hakkı bitti vb. Sadece soluk düğme yeterli değildir.
- Düğmelerde açık hover/pressed hâli, ölçülü çerçeve veya küçük basılma tepkisi ve kısa ses. Harita, bölge, ordu emri ve savaş için tutarlı bağlamsal imleçler değerlendirilir.
- Farklı bilgi türlerine farklı kompozisyon: defter, emir, siyasi not, askerî rapor. Her şey aynı dikdörtgen karta dönüştürülmez.
- Tipografi, başlık boyları, aralıklar, çizgiler, köşeler, gölgeler ve simgeler tek bir düzen izler. Hafif asimetri ve bilinçli boş alan; sırf boşluğu doldurmak için süs veya metin yok.
- İlk, ikinci ve üçüncü bakış noktaları belli olmalı. Her ekranın tek güçlü odağı: haritada Fransa, savaşta hat, olayda belge/illüstrasyon, sonuçta askerî rapor.
- Masaüstünde ferah ve okunaklı düzen. Mobil tarayıcı referansı çalışır kalabilir; Unity tasarımı mobil ekran etrafında kurulmaz.
- Varsayılan web düğmeleri/select/focus ve kaydırma çubuğu hissini kaldırmak; oyun dünyasına uygun kontrol dili.
- Tek sayı standardı: aynı +/− işaretleri, yuvarlama, yerel RU/TR biçimi ve renk anlamları. Değişen sayı kısa süre yönüne göre vurgulanır, sakin şekilde yeni değere gelir.
- Gereksiz tekrarlar, anlamsız renk, kullanılmayan düğme ve açıklamalar kaldırılır. Renk ve simge yeterliyse aynı durumu uzun metinle yinelememek.

## Hafta, ekonomi ve günlük

- Ekonomi mekaniğini değiştirmeden hazine, yiyecek ve huzursuzluğun neden değiştiğini açıklamak: şimdi, değişimin kaynağı, gelecek hafta tahmini.
- Gelir, üretim, sivil/ordu tüketimi, maaş ve mevcut yardımın etkileri izlenebilir olmalı; gösterilen hesap çekirdekle aynı olmalı.
- Hafta sonunda kısa bir ritim: para, gıda ve huzursuzluk değişimi okunur sırayla görülür. Animasyon simülasyonu yeniden çalıştırmaz veya ekstra tıklama istemez.
- Vergi, ekmek, asker toplama, yürüyüş, savaş başlangıcı, zafer/yenilgi ve hafta sonu farklı ağırlıkta geri bildirim verir.
- Yeni günlük kaydı yumuşak biçimde belirir; önemli haber rutin kayıttan ayrılır. Okunan konum sebepsiz sıçramaz, günlük eşit ağırlıklı yazı duvarı olmaz.
- Mesajlar kısa ve doğal: yapılan işi ve yerel sonucu söyler. Örneğin ekmek dağıtımı, kalabalığın dağılması ve gerçek sayısal etki birlikte görülebilir; gerçekleşmemiş davranış uydurulmaz.

## Mevcut olay ve kampanya başlangıcı

- İkinci haftadaki mevcut ekmek dilekçesi korunur. Bu aşamada yeni olay kataloğu veya tetikleyici sistem eklenmez.
- Standart üç düğmeli modal yerine güçlü başlık, belirgin kişi/çıkar, dünya içi belge, gravür/illüstrasyon ve iyi kompozisyon. Seçimlerin açık anlık bedeli hover veya seçim öncesi görünür.
- Kişilerin sesleri farklı; şablon dramatik metin ve teknik etiketler yok. “Test savaşı”, “oyun olayı”, “senaryo” gibi debug dili oyuncu yüzünden çıkarılır. Kurgu bilgisi uygun yardım/belgede dürüstçe açıklanır.
- Mevcut senaryo açılışı: tarih, Fransa haritası ve kısa doğal bir cümle; uzun zorunlu giriş veya yeni içerik eklenmez.
- Kaydetme dünya içinde “durumu kayda geçirmek/arşivlemek” hissi verir. Yeni kampanyanın mevcut kaydı değiştirmesi açıkça belirtilir; süslü ifade veri kaybını gizlemez.

## Mevcut savaşın hissi

- Yeni tür veya mekanik eklemeden grup hareketi, mevcut düzen, mesafe, atış, geri çekilme ve komut tepkisi iyileştirilir.
- Askerler nokta gibi kaymaz: görsel ağırlık, hafif atalet, tutarlı aralık ve yön; hat/kol/kare geçişi okunur. Görsel hareket, gerçek emir veya vuruş hesabını tutarsızlaştırmaz.
- Mevcut sancaklar grupları tanımlar. Sağlık/durum göstergeleri sürekli her yerde değil; seçili, yeni vurulmuş veya kritik birliklerde ağırlıklı görünür.
- Yaylım: silah kaldırma → kısa hazırlık → ölçülü eşzamanlı/parçalı parlamalar → ses → ortak duman → karşı hatta tepki. Mevcut hasar zamanlamasıyla uyumlu, küçük bir sahne gibi okunmalı.
- İsabet, küçük geri tepki veya düzende açılan yerle anlaşılır. Şiddetsiz, inandırıcı düşüş; asker aniden kapanıp yok olmaz.
- Duman birkaç an kalır, sürüklenir ve kademeli dağılır; basit sabit daire etkisi olmaz. Efektler görüşü gereksiz kirletmez.
- Kamera/ölçek, engebeler ve hatlar okunur. Yol, tepe, tarla, orman kenarı ve ufuktaki yerleşim doğal bir kompozisyon oluşturur; rastgele dekor yığını yok.
- Çok ölçülü vuruş sarsıntısı ve önemli anlarda kısa ağırlık hissi düşünülebilir; sürekli kamera sallanması veya zorunlu dramatik duraklama yok.
- Duraklatma oyun dünyasına uygun görünür, debug katmanı gibi durmaz. Duman ve görsel tepkiler duraklama mantığına uyar.
- Sonuç sıradan kart değil, kısa askerî donanımlı rapor: kazanan/kaybeden, gerçek mevcut ve kayıplar, kısa haber, sefer sonucuyla tutarlı bilgiler, açık tek dönüş düğmesi.
- Sefer → askerî emir → meydan → rapor → atlas tek oyunun parçaları gibi görünür. Sonuç iki kez uygulanmaz, dönüşte gereksiz adımlar yok.

## Ses ve mikroanimasyon

- Kâğıt, kalem, mühür, emir, yürüyüş, uzaktaki davul, tüfek yaylımı ve top gürültüsü gibi **10–15 kaliteli ses** uzun vadeli sunum hedefidir; sayıyı doldurmak için kötü efekt eklenmez.
- Mevcut 10 prosedürel ses yalnızca ilk işitsel taslaklardır; profesyonel veya tarihî kayıt olarak adlandırılmaz. Gerçek editör/oyun içinde ses seviyesi ve tekrarlar dinlenerek kontrol edilir.
- Sesler kısa, ölçülü ve tekrarda yorucu olmayan seviyede; susturma ve eşzamanlı ses sınırı. Sonuç sesi iki kez çalmaz.
- Müzik daha sonra gerekirse çok sakin atmosfer; sürekli epik orkestra değil. Bu aşamada müzik sistemi genişletmesi yapılmaz.
- Genel mikroanimasyon **100–200 ms**, yürüyüş 0,5–1 saniye; kısa, sakin hareketler. Uçuşan web kartları, parlak mobil sayaçlar ve aşırı efekt yok.

## Görsel ve davranış kabulü

- Her ekran Rusça ve Türkçede, masaüstü boyutunda incelenir: taşma, eksik anahtar, metin/harita çakışması, okunmayan sayı veya atıl alan yok.
- Tek ekran görüntüsü bağlamsız incelenir: dünyaya ait mi, tanınabilir mi, geçici veya hazır kit hissi veriyor mu? Olumsuzsa ilgili ekran yeniden ele alınır.
- Mevcut tam döngü elle oynanır; sebep-sonuç ve geçişler doğrulanır. Derleme başarısı görsel kalite veya oynanabilirlik kanıtı sayılmaz.
- Ses ve animasyon değişiklikleri ekonomi, vuruş zamanı, kayıt veya sonuç uygulama sayısını değiştirmemeli.
- Hedeflenen kalite, uygulanmış kaynak, gerçekten test edilen davranış ve kalan işler ayrı kaydedilir.

## Durdurma anındaki gerçek durum ve yeniden başlama noktası

- İlk ara kayıt `cb200f1` GitHub'a gönderildi; bu planla birlikte yeni kaynak kontrol noktası da kaydedilir.
- Unity editörü ve `Main` açıldı. Play sırasında kampanya haritası ve Türkçe arayüz gerçekten ekranda görüldü.
- Bağımsız Roslyn kontrolünde 8 Runtime dosyası ve Editor BuildTools gerçek Unity DLL'leriyle hatasız derlendi. Bu kontrol oyuncu derlemesi değildir.
- İlk Play, `TacticalBattle` alan başlatıcısındaki `MaterialPropertyBlock` için Unity yaşam döngüsü hatasını ortaya çıkardı. Kod düzeltildi; kullanıcı durdurduğu için düzeltme sonrası yeniden oynatma doğrulanmadı.
- Çekirdekte 14 NUnit testi kaynak olarak var; Unity Test Runner çalıştırılması ve Windows oyuncu derlemesi bekliyor.
- Yeniden başlandığında önce mevcut Play/duraklama/derleme durumuna bakılmalı; kullanıcı kampanyası sıfırlanmamalı. İlk iş yeni özellik değil, son başlatma düzeltmesini ve tam mevcut döngüyü doğrulamaktır.
- İncelemede kalanlar: yürüyüşün ikmal/yorgunluk bedelinin savaşa ne zaman yansıdığı; taktik moral raporuyla sefer sonrası moralin tutarlılığı; Rusça ekranların görsel kontrolü; seslerin dinlenmesi; genel okunurluk ve tipografi. Polish tamamlandı sayılmaz.

## Polish sonrasına bırakılanlar

Kullanıcı açıkça **“После polish”** dedi. Dünya koşullarına bağlı olaylar, hafıza bayrakları/sayaçlar, tepkisel/kriz/fırsat/zincir sınıfları, tekrar önleme, bölgesel bağlam ve küçük Event Director sonraki aşamadır. Ayrıntılar [EVENT_DIRECTION.md](EVENT_DIRECTION.md) içindedir.

Önceki derin siyasi güçler, karakter iradesi, ekonomi-ikmal ve taktik genişleme hedefleri silinmedi; [DESIGN_V0.2.md](DESIGN_V0.2.md) ve [ROADMAP.md](ROADMAP.md) gelecek iş listesinde durur. Referanslar [REFERENCES.md](REFERENCES.md), sanat kuralları [ART_DIRECTION.md](ART_DIRECTION.md), kalıcı çalışma ilkeleri [AGENTS.md](AGENTS.md) içindedir.
