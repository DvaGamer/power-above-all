# Tek ve sürekli dünya — uygulama kararı

6 Eylül 2026 kullanıcı talimatı haftalık tur ve ayrı savaş alanı yönünün yerine geçer. Bu belge tamamlanma raporu değildir. Paris'teki karakter, tarihli bilgi ve insan üzerinden emir ilkeleri korunur.

## Okunan mimaride bağlar

- `CampaignCore` ülke stokları, tek `ArmyRegionId`, `Troops`, iki `Moves` ve hafta sonu bütünleşik işlemini taşır. Hareket konum hesabı yapmaz. `CampaignMap` varıştan sonra .85 saniyelik süs hareketi oynatır.
- `GameApp.March` düşman bölgeye girerken haritayı saklar, kamerayı askıya alır ve `TacticalBattle.Begin` içine sayıları kopyalar. Sonuç yalnız kayıp/moral/zafer olarak geri döner. `Busy` bu sırada kampanyayı durdurur.
- `TacticalBattleSimulation` yararlı bir eşzamanlı karar/hasar/panik yaklaşımı içerir. Ancak özel `Regiment`, sabit yapay dere/bahçe, konvoy hedefi ve görsel efekt çağrılarıyla bağlıdır. Bunlar yeni dünya coğrafyası değildir.
- Arşiv v9, hafta2 dilekçesinin hemen çözülmesini, vadesi geçmiş borcun bulunmamasını, emrin gününün7katı olmasını ve savaş kimliğinin hareket sayısına uymasını zorunlu kılar. Sürekli zaman için bu koşullar ayrı ele alınmalıdır.
- Reform dört uygun ekonomik dönemde hazırlanır; Dumas girişimi ve terhis gerçek politik maliyet içerir. Bunları çöpe atmak yerine takvim işlerine uyarlıyoruz. Haftalık muhasebe kalabilir; oyuncunun zamanı açmak için tuşa basması kalamaz.
- HUD haftayı simüle edilmiş kopyada önizler; ordu düğmesi `CanMarch/Moves` kullanır. AutoShots eski haftalı protokolü ayrıca taşır. Yeni gerçek-zaman kabulü eski senaryoların yeşil olmasına indirgenemez.

## Üç yaklaşım

1. Eski NextWeek'i zamanlayıcıya bağlamak: kolay, fakat hareket kotaları ve kopya savaş kalır. Elendi.
2. Bütün oyunu ECS/fizik tabanlı anlık rewrite: eşzamanlı sistemler için uygun olabilir, ancak çalışan siyaset, kayıt ve sanatı gereksiz yere riske atar. Elendi.
3. Saf veri tabanlı WorldSimulation + takvim sınırları + yerel sabit savaş adımı + yalnız okuyan atlas temsili: seçildi.

## Sahiplik ve zaman

`CampaignState.World` isteğe bağlı yeni kayıt köküdür. Yeni oynanabilir kampanyada zorunludur; eski formül testleri ve arşiv geçiş okuyucusu eski modeli doğrulamaya devam eder. WorldClock tamsayı milisaniye taşır; hız0/1/3600/86400 oyun saniyesi/gerçek saniyedir. Unity Time.timeScale değiştirilmez.

WorldSimulation aynı saat üzerinde rota varışı, günlük haberleşme, dönem muhasebesi ve çatışmayı işler. Uzun sakin zaman aralıkları hesaplanarak geçilir; dövüş sınırlı sabit adımlarla yürür. Kare başına iş bütçesi vardır, biriken süre kaybolmaz. Büyük temas uyarısında eski yüksek hızla hesaplanan kalan süre iptal edilir: bildirim anından sonrasını oyuncunun görmediği hızda simüle etmeyiz.

Army, Unit, Commander ve Headquarters ayrı kimliklidir. Unit listeleri tek gerçek kaynaktır; harita bayrağı, alay izi ve yakın figür aynı kaydın temsilleridir. BattleInstance yalnız katılımcı kimlikleri, başlama/bitiş zamanı ve deterministik rastgele durum taşır. Yeni sahne, arena veya birlik kopyası taşımaz.

Konumlar atlas pikseli değil projeksiyon metreleridir. GIS WGS84 koordinatları var olan 46° standart paralelli atlas projeksiyonuna çevrilir. Bu ilk Fransa kesitinde yerel mesafe yaklaşımıdır; küresel sefer rotaları için jeodezik mesafe ayrıca gerekir. Mevcut yollar şematiktir; gerçek1789 güzergâhı diye sunulmaz.

## İlk kabul senaryosu

Paris ordusuna Champagne yönünde yürüyüş ver; zaman ilerlerken yol ve kalan mesafe değişsin. Champagne'daki ikinci mevcut kuvvetle temas aynı koordinatlarda başlasın; hız I veya duraklatma seçeneği uygulanabilsin. Yakında alay seçimi, hareket, düzen ve geri çekilme çalışsın. Birimler mevcut dünya konumlarından açılıp yürüsün; kazanmak yeni ordu üretmesin. Diğer takvim işleri aynı anda sürsün. Yol ortası, emir beklerken ve çatışma sırasında kayıt/geri yükleme sonucu aynı olsun.

## Bilinçli sınırlar

Yeni muharebe yönü: en yakın düşmana otomatik yürüyüş kaldırılır. Centre/Left/Right/Reserve/Battery/Screen görevleri ortak cepheye yerleşir; aynı birim emir teslimini ve yeniden düzenlenmeyi bekler. Görüş, dost ateş koridoru, açık geri yol, HQ/cephane arabası, moral/düzen ve yerel yedek kararı aynı dünya verisini kullanır. Dünya şeması3/arşiv13; v12 hedefsiz eski sefer olarak açılır. Schema3'ten önceki sürekli prototip kaydı sessizce tahmin edilmez. Eski haftalı arşiv okuyucusu korunur; GameApp desteklenmeyen dünya için yeni kampanya gerektiğini açıklar.

İlk rol raporu ayrı sahne veya dünya saati değildir. CampaignState.Commissions içindeki isteğe bağlı tek kayıt, 28 günlük sınırı WorldSimulation'ın aynı zamanlayıcısına verir. Mevcut mândatın vadesi + iki günlük ek süre de zaman sınırıdır. Yüksek hız bu sınırların üzerinden atlayamaz; sonuç bir kez oluşur, küçük siyasi etkisini uygular ve okumak için saati duraklatır. Rapor verisi daha sonra canlı göstergelerden yeniden üretilmez. Eski v12 kayıtlarına geriye dönük görev veya tutulmuş söz uydurulmaz.

İkmal sahibi ayrı WorldDepot ve WorldConvoy'dur. Sonlu yük çıkışta stoktan düşer, aynı yol grafiğinde yürür, yalnız gerçek temas mesafesinde alıcıya eklenir. Rotalar iki fiziksel konumu yol parçalarına bağlar; eski ArmyRegionId'ye dönmez. Taşınan rasyon15dk aralıkla azalır; boşluk önce kondisyonu, üçüncü aç günden sonra asker sayısını etkiler. Haftalık merkez hesabı asker yemeğini yeniden düşmez. Askerin cephanesi biterse amacı saklanarak kendi arabasına gider; kaynak yoksa örgütlü çekilir. Sivil üretim ve ulusal teçhizat henüz merkez hesabındadır. Paris merkez ambarı→depo aktarımı yerel soyutlamadır; bütün Fransa'da fiziksel üretim/kurye ağı iddiası değildir.

Mevcut iki-ordu kesitinde muharebeden sonra bir günlük yeniden temas koruması vardır; gerçek takip/peşine düşme ve üçüncü ordunun müdahalesi tamamlanmadı. Yerel yol erişimindeki5km bağlantı tam arazi pathfinding değildir. Yeni büyüme bu sınırları kaldırmalı; gizli zafer bonusları eklememeli.

Town footprint ve yerel ağaç grupları PAA sanatsal yorumudur; gerçek1789kadastro iddiası değildir. Nehir merkez çizgileri mevcut Natural Earth katmanından gelir; aynı nehrin ayrı parçaları ayrı feature kimliği taşır. Henüz köprü geçitleri doğrulanmadı. Yükseklik/orman testi sentetik veride doğrulandı; haritada olmayan bir tepeye gizli avantaj verilmez.

İlk geçişte küresel ekonomi, bütün limanlar, fiziksel ticaret, köprü yıkımı ve tüm bina sınıfları bitmiş sayılmaz. Bunlar yeni veri sahipliği üzerinden sonraki geçişlerdir. Eski arena sınıfı ilk aşamada regresyon/formül karşılaştırması için kalabilir; normal GameApp onu başlatmaz. Bölgesel harita verisi gerçek GIS, askerî senaryo kurgusal oyun içeriğidir.

## Kaynak ilkesi

[Glenn Fiedler: Fix Your Timestep](https://gafferongames.com/post/fix_your_timestep/) 6 Eylül2026'da açıldı: sabit küçük savaş adımı, görsel kareden ayrılmış saat ve kontrollü yakalama bütçesi. Yazı yüksek hızdaki devlet ekonomisine hazır çözüm sunmaz; aralık muhasebesi ve olay sınırları PAA'nın ihtiyacıdır. Kullanıcının WarcraftIII örneğinden alınan ilke yalnız sürekli haritada ortak varlık sahipliğidir. [Komuta referansı](References/Gameplay/Combat/SOW_Courier_Command.md), [rapor referansı](References/Gameplay/Campaign/Radio_General_Reports.md) ve [görsel kurallar](ART_PRODUCTION_RULES.md) geçerlidir.
