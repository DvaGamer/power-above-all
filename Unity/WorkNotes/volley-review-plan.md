# Gerçek aimed volley ve duman evreleri

Hazır fixture: `tools/volley-review.script`. Bu ajan Unity/player, derleyici veya Git çalıştırmadı; AutoShots ve savaş kaynaklarını değiştirmedi. Fixture mevcut komut dilini kullanır. Root daha sonra yalnız bir açıkça seçilmiş mevcut oyuncu ile `tools/review-player.ps1 -PlayerPath <EXE> -ScriptPath <repo>\tools\volley-review.script -Label volley-phases -VisiblePlayer` çalıştırabilir. Yeni tam build/test kanıtı değildir; başarılı player-only koşu PARTIAL olarak kalır.

Mevcut `Get-ReviewPlan` ile salt okunur ayrıştırma PASS:67 komut,13 assertion,8PNG,9JSON. Gerçek volley/phase oyuncu koşusu henüz yapılmadı. Root bu ilk görsel baseline için tek gerçek aimed atış+ammo−1+duman evrelerinin yeterli olduğunu, paused/duplicate semantiğinin mevcut anlamlı NUnit kanıtında kalacağını onayladı.

## Gerçekte ne yapılıyor

Champagne karşılaşması mevcut yürüyüşten açılır. Daha önce doğal taktik koşuda kullanılan konumlara gerçek selection/move/formation emirleri verilir: iki piyade ortak hedef−8/0, süvari−10/10, topçu14/−6. Piyade ve topçu `hold fire` durumundadır. Yakın temasın kendi kuralları ve düşman yapay zekâsı çalışmaya devam eder; hiçbir düşman pasifleştirilmez veya birlik/durum/clock/sonuç enjekte edilmez.

Bu ilk görsel örnek **tek bir topçu aimed volley** içindir. Piyade/musket cephesi veya bütün ateş türleri test edilmiş sayılmaz. Seçili topçu gerçek hareketini bitirip mevcut CanVolley koşuluna doğal olarak gelince bir kez `VolleySelected()` çağrılır. Emir gerçek kuyruğa gider; hasar/cephane/duman yalnız normal `Simulate` tick'inden doğar. Askerî malzeme, ammo, düşman konumu ve hedef sayacı yazılmaz.

## Kareler ve JSON kabulü

| Artefakt | Amaç | Gerçek kabul ölçütü |
| --- | --- | --- |
|00-deployment /00-start | Kurulum ve oyuncu atışlarının ayrılması | Seçili yalnız topçu; oyuncu ranged birliklerinde FireAtWill=false. |
|01-before-shot-paused | Gerçek hazır birliği durdurup öncesini çekmek | Önceki BattleCanVolleyTrue beklentisi geçti; JSON'da gerçek ammo/konum/selection. |
|02-pause-held |0.75 gerçek saniye ve iki kare arasında pause |01 ile elapsed, ammo, reload, hedef/konum değişmez; snapshot'lar gerçek clock ilerlemesi olmadığını göstermeli. |
|03-before-accepted-order | Atıştan hemen önce yeniden çalışır durum | İkinci bounded volley-ready geçti; aynı topçunun atış öncesi ammo değeri alınır. |
|04-early-clouds | Kabul edilen emirden nominal0.12 saniye sonra pause | Seçili topçu ammo tam1 azalmış, pendingfalse, reloadpozitif; gerçek namlu dumanı görünür. Olmazsa kabul edilmiş emirden gerçekleşen atışa geçiş kanıtlanmış sayılmaz. |
|05-clouds-pause-held | Erken bulutu0.75 gerçek saniye durdurmak |04 ile birlik/simulation değerleri aynı; duman yayılmamış/sönmemiş olmalı. |
|06-developed-clouds | Normal devamda1.1 saniye yayılma, sonra pause | Aynı atışın daha yaygın/solan bulutu; ammo04 ile aynı. |
|07-clouds-cleared-candidate |6.5 saniye daha normal devamdan sonra pause | Aynı topçunun ammo hâlâ yalnız1 düşük; kendi önceki bulutu artık görünmez. Düşman hâlâ ateş edebileceğinden bütün sahnenin dumansız olması beklenmez. |
|08-campaign-return | İncelemenin normal kapatılması | Gerçek retreat/accept sonucu kampanyaya bir kez geçer; bu doğal zafer değildir. |

AutoShots'ın mevcut `expect` dili birlik bazlı ammo veya cloud sayısı karşılaştırmaz. Bu yüzden script PASS sonucu ile bu tablodaki görsel/JSON kabulü aynı şey değildir. Root/ajan `Battle.Regiments` içinde Id/PlayerSlot ile aynı topçuyu eşler ve delta/phase incelemesini ayrıca kaydeder. Ranged `hold fire`, manuel tekrar verilmediği sürece bu birliğin ikinci normal atışını engeller; düşman dumanları ile bütün oyuncu cephaneleri birbirine karıştırılmaz.

## Kare zamanlamasının gerçek sınırları

Mevcut AutoShots **her komutun ardından bir frame yield eder**. `Shot`, `WaitForEndOfFrame` sonrasında `CaptureScreenshot(path)` planlar, sonra tamamlanmış PNG'yi en fazla10 gerçek saniye bekler. Bu yüzden iki ardışık JSON/komut aynı tick'in atomik fotoğrafı değildir. Kabul anındaki `AimedVolleyPending=true` ilk JSON'a kadar normal tick'te tüketilmiş olabilir; sırf snapshot'ta pendingfalse diye kuyruğun hiç kullanılmadığı söylenmez.

Yeni görsel kaynakta powder cloud ömrü topçuda5.2s, piyadede4.1s; namlu flaşı0.075s, gecikmeler i×0.012s. Bu fixture75ms flaşı yakaladığını iddia etmez. İlk0.12s bekleme normal tick'e fırsat verir, sonra pause duman evresini PNG yazım gecikmesinden korur. Sürümün kaynakları freeze edilmeden bu süreler kalıcı tasarım sabiti kabul edilmez.

`wait` gerçek zamandır; `visualClock` ise pause sırasında durur, aktif frame'de `min(unscaledDeltaTime,0.1)` kadar büyür. Dolayısıyla düşük FPS'de6.5 gerçek saniye, tam6.5 görsel saniyeyi garanti etmez. Son kare bu yüzden **cleared-candidate** adını taşır; gerçekten sönme görülmediyse başarılı evre kanıtı sayılmaz. Beklemeyi veya saati gizlice hızlandırmak/efekt spawn etmek çözüm değildir.

## Küçük salt okunur gözlem önerisi

Gerekirse root'un sahip olduğu API'ye `BattleSnapshot.VisualElapsedSeconds` ve `BattleRegimentSnapshot.LastVolleyVisualSeconds` eklenebilir: ikisi de zaten yaşayan `visualClock` ve `Regiment.LastVolley` değerlerinin kopyasıdır. Atış öncesi−100 sentinel, gerçek tick sonrasında zaman damgası ve son karede yaş farkı ölçülür; yeni simülasyon yolu gerekmez. Ammo farkı zaten gerçek atış sayısına kanıt verir.

Tam otomatik per-source clearance gerekiyorsa yalnız `Puff.SourceRegimentId` etiketi ve snapshot'ta `LivePowderCloudCount` sayımı eklenmesi yeterli olur. Global effect count, eşzamanlı düşman dumanı yüzünden yanlış ölçüttür. Bu alanlar gözlemdir; fixture'e efekt oluşturma/temizleme veya clock yazma API'si eklenmez. Bu öneriler henüz uygulanmadı.

## Pause sırasında emir ve çift tıklama

Var olan BattleCommand/TacticalSimulation testleri paused volley reddini ve aynı tick öncesindeki mükerrer kuyruğun birleşmesini zaten sınar. Bu dosya onlara benzer yeni sentetik test veya yeni unit/stat mutasyonu eklemez. İki ardışık `battle volley` satırı **konulmadı**: aradaki zorunlu frame yield nedeniyle birinci atış gerçekleşip ikinci emir doğru olarak reload yüzünden reddedilebilir; böyle bir fixture kararsız biçimde RED olurdu.

Oyuncudaki entegrasyon kanıtı için root iki küçük yolu seçebilir:

1. Görünür oyuncuda pause açıkken gerçek HUD volley düğmesine basmak ve ateş sonrası reload sırasında düğmeye tekrar hızlı basmak. Sonra aynı birliğin ammo/LastVolley/efekt değerlerini karşılaştırmak. Mevcut ayrı native `Click` çağrıları screenshot/CIM gecikmesi içerdiğinden, bunları fiziksel hızlı çift tıklama diye adlandırmamak gerekir. Gerekirse tek native çağrıda iki kontrollü down/up çifti dar araç uzantısı olur; odağın/pointer'ın her down öncesi denetimi ve finally release korunur.
2. AutoShots'ta yalnız gerçek ortak API'yi çağıran küçük bir kabul komutu: paused durumda bir `VolleySelected` ret sonucu+tam snapshot eşitliği; aynı coroutine adımında iki `VolleySelected` çağrısının ilkAffected1/ikinciAffected0 sonucunu kaydetmek, sonra normal tick'i bekleyip ammo delta1 ölçmek. Bu bir UI çift tıklama kanıtı değil, oyuncu içindeki shared API entegrasyon kanıtıdır. State/clock injection veya doğrudan Simulate çağrısı olmamalıdır.

Bu ek yollar gerekirse AutoShots/native helper sahipliği root'tadır; burada yeni komut uygulanmadı. Mevcut fixture paused dünyanın sabit kalmasını ve **bir gerçek atışın** görsel ömrünü örnekler; reddedilmiş/double-click emirlerin gerçek oyuncuda ayrıca denenmiş olduğunu iddia etmez. Bu görsel baseline'ın kabulü tablodaki gerçek ammo/duman gözlemidir; yeni sentetik duplicate testi veya75ms flaş garantisi şartı eklenmedi.

## Gerçek eski-görsel baseline —2026-09-06 00:44:50UTC

Root koşusu: `output/verify/volley-baseline-20260906-004450-276-c4666896`. Mevcut `accord-layout-final-20260906-002826-992-56dba0b4` oyuncusu yeniden kullanıldı;49s, native0,13assert/8PNG/9JSON, frames8/8 problems0.141 build dosyasının ad/boyut/SHA256 değerleri koşu boyunca değişmedi. MakbuzPARTIAL; EditMode/yeni build/browser atlandı. Runtime SHA256 `EEEC2A00A052CADE6CEEC14823E6B64CE432083F2D3A7FF25F09E9F04FCF430B`; script SHA256 `040C92A53D8913DDD2E47ED0F1464F787C8D059B412B97A52B5B08141F74AC55`. Sonradan değişen yumuşak duman maskesi kaynaklarına kanıt değildir.

Bu ajan aşağıdaki verileri ve dört tam PNG'yi salt okunur inceledi; yeni oyuncu/Unity/derleyici çalıştırmadı, kaynak veya fixture değiştirmedi.

### Id3 / oyuncu slot4 için gerçek olay

| JSON | Paused | Simulation elapsed | Ammo | Reload | Pending | Hazır |
| --- | --- | --- | --- | --- | --- | --- |
|01-before-shot-paused | true |24.599911 |11 |0 |false |false (pause) |
|02-pause-held | true |24.599911 |11 |0 |false |false (pause) |
|03-before-accepted-order | false |24.599911 |11 |0 |false |true |
|04-early-clouds | true |24.749908 |10 |15.226265 |false |false (pause) |
|05-clouds-pause-held | true |24.749908 |10 |15.226265 |false |false (pause) |
|06-developed-clouds | true |25.849892 |10 |14.126261 |false |false (pause) |
|07-clouds-cleared-candidate | true |32.349792 |10 |7.626236 |false |false (pause) |

Seçili, komuta edilebilir topçu bütün örneklerde FireAtWill=false ve Moving=false; konumu14.185584/−6.222700, hedef14/−6. Gerçek kabul sonrası ammo yalnız bir kez11→10 oldu ve bütün sonraki fazlarda10 kaldı.04'te ContactReload0.5,06/07'de0. 01–06 asker206 iken07'de196: normal düşman savaşı durdurulmamıştır. Atış anındaki kısa Pendingtrue burada yakalanmadı;03 sonrası normal tick'te tüketilmesi ve gerçek ammo/reload değişimi gözlendi, aynı-frame queue görüntüsü iddia edilmez.

01/02 **bütün ham JSON** metinleri tam eşit;04/05 de tam eşit. PNG dosyaları da eşit:01/02 SHA256 `6726884f3caa35b2724331617ca840bb4e3de56601f112f55702eb5ef2d259f6`;04/05 `7653d60ba457af97bbd02955b315c886a4eb8fd8e73d24f8e955a3bc93aca624`. Pause sırasında simulation ve render dondurma bu iki gerçek çiftte doğrulandı. Çift tıklama veya reddedilen ikinci emir bu koşuda yapılmadı.

### Ayrı görsel değerlendirme

01/04/06/07 tam1440×900 PNG'leri açıldı; gerçek sahne, HUD ve farklı savaş anları görülüyor. Fakat seçili topçunun bilgi etiketi yaklaşık canvas x800–956/y427–465 alanında, kuzeybatıya dönmüş namlusunun önündeki efekt bölgesini örtüyor.04'te ve06'da **bu topçuya ait** erken/yayılmış bulut, normal tam-kare görünümünde açıkça okunur değil. Merkezdeki daha belirgin beyaz küreler komşu düşman birliklerindedir; onları bizim aimed atışın dumanı diye saymak yanlış olur.

07'de seçili batarya çevresinde belirgin yeni bulut görülmez; başka düşman topçusu yeni duman üretebiliyor. Önceki kendi bulutumuz net ayırt edilemediği ve per-source effect kimliği olmadığı için bu tek kareyi kesin aynı-bulut temizlenme kanıtına yükseltmiyoruz. Snapshot'taki elapsed simulation zamanıdır; exactvisualclock bulunmadığından bulutun tam yaşı hesaplanmış gibi verilmez.

Sonuç: **native/protokol/kare dosyası kontrolleri ve tek gerçek atış + pause sabitliği PASS**. Kendi topçu dumanının erken→yayılmış→silinmiş görsel okunurluğu bu baseline'da açık kabul alamadı; normal etiketin efekt alanını örtmesi somut sunum engelidir. Yeni maske sürümü bu eski koşunun başarısı sayılmaz; root'un ayrı yeni derleme/phase karşılaştırması gerekir. Oyuncu kapanışı gerçek retreat/accept ile93 toplam kaybı kampanyaya bir kez taşıdı ve return assertion geçti; doğal zafer denenmedi.

## Sonraki en küçük varyant: normal seçimle topçu etiketini azaltma

Root isteğiyle ayrı `tools/smoke-uncovered.script` hazırlandı. Dondurulmuş `volley-review.script` değişmedi. Tek davranış farkı, accepted volley +0.12s +normal SetPaused(true) sonrasında,04'ten önce `battle select 3 replace` ile süvari seçilmesidir.04/05/06/07 bundan sonra çekilir. Bu sıradan oyuncu emridir; hideUI, efekt oluşturma/temizleme, clock yazma veya sonuç enjeksiyonu yoktur. Sonraki JSON'da SelectedIds[2] beklenir; ammo/atış takibi yine **Id3 / PlayerSlot4** üzerinden yapılır, o an seçili birliğe taşınmaz.

Eski tam-karede etiketin source rectangle hesabı yaklaşık x800.56–956.56, y427.61–463.61; alt üç piksellik bar y466.61'e kadar iner. Id3'ün baseline konumu14.185584/−6.222700 ve Facing−42.37252°, eski kameranın ortografik izdüşümüyle iki namlu/bulut başlangıç merkezi yaklaşık(845.83,465.06) ve(870.61,447.57) canvas pikselidir. İlk merkez alt bar üzerinde, ikincisi koyu etiket gövdesi içindedir. İncelenecek küçük alan yaklaşık x830–895/y432–484'tür.

Bu merkezler görünmeyen bulutlardan ölçülmüş kesin piksel koordinatları **değildir**: snapshot fiziksel konum/facing taşır; Root dönüşümü görsel olarak yumuşatılır ve TerrainHeight ile yükselir. İzdüşüm, kapatılan bölgeyi tarif eder; yeni build'de gerçek konum ve bulut şekli tekrar görülmelidir. Etiket ile örtüşme tam PNG incelemesinde görüldü, bulutun bütün konturu bu baseline'da ayırt edilemedi.

Kaynakta seçilmemiş birlik etiketi de son4 görsel saniyede vurulmuşsa, hover altındaysa veya morali36'nın altındaysa gösterilir. Bu nedenle süvariye geçmek seçime bağlı etiketi kaldırır; topçu yeni vurulduğunda bütün etiketlerin kaybolmasını garanti etmez. Sonraki karede etiket hâlâ örtüyorsa bunu açık bulgu olarak bırakmak gerekir. Sistematik ürün etiketi yerleşimi ayrı UI işi olup test için özel saklama yoluyla geçilmiş sayılmaz.

Bu varyant henüz oyuncuda çalıştırılmadı. Yeni yumuşak maskenin actual build'i seçildikten sonra root tek yeni player-only koşu yapar; eski baseline verileri ayrı kalır. Salt okunur timestamp gözlemi önerisi henüz uygulanmadı.

Mevcut parser varyantı68komut/13assert/8PNG/9JSON olarak kabul etti. Yorumlar hariç dizi karşılaştırmasında tek ek komutun doğru fired-pause noktasındaki süvari seçimi olduğu doğrulandı. Orijinal fixture SHA256 hâlâ `040C92A53D8913DDD2E47ED0F1464F787C8D059B412B97A52B5B08141F74AC55`; eski koşudaki makbuzla aynı.

## Smoke A: gerçek maske derlemesi, otomasyon geçti / görsel kabul geçmedi

Root'un değişmez çıktısı: `output/verify/smoke-wash-20260906-005638-116-38a1dee3`. Tam gate GREEN: 128/128 EditMode, yeni derleme ve 141 dosyalık manifest, native çıkış denetimi, 68 komut/13 assert/8 PNG/9 JSON, 8 otomatik kare kontrolü ve 10 browser testi. Runtime SHA256 `A06A95FD0824EBDE17DE60A08CEAD7A0E7B8EA39A16CB77E04CD1877E1247B64`. Protokol tamamlanma zamanı 00:57:34.5052479 UTC; frame helper native0. Bu ajan yalnız çıktıları okudu; Unity, player veya test başlatmadı.

Id3 topçunun 01–07 ölçümleri önceki tablonun tamamıyla aynı: elapsed 24.599911 → 24.749908 → 25.849892 → 32.349792; ammo 11 → 10 ve sonrasında10; reload 0 → 15.226265 → 14.126261 → 7.626236. FireAtWill=false, Moving=false, konum14.185584/−6.222700 ve hedef14/−6 sabit.04 sonrası `SelectedIds=[2]`, yani süvari seçimi doğru uygulanmış. Pendingtrue anı yakalanmadı; gerçek ammo/cooldown değişimi ve sonraki pending=false, emrin normal tick tarafından tüketildiğini gösteriyor. Yeni çift tıklama veya paused volley emri uygulanmış değildir.

01/02 bütün ham JSON metinleri aynı; PNG SHA256 `50b4d334f42bdeab2d55fca72135e3b60d337cc684850194d12859c49e613276`.04/05 bütün ham JSON metinleri aynı; PNG SHA256 `908cdd059a18f9391392ce89ef8321d95ea1380aaef2e4788e9dc11104bfbdab`. Her iki gerçek pause çifti simülasyonu ve render'ı sabit tutuyor.

Eski baseline ile 01–07 bütün regiment snapshot alanları tek tek karşılaştırıldı.01–03 hiçbir fark yok;04–07 yalnız Id2/Id3 `Selected` farkı var. Elapsed değerleri de birebir aynı. Bu örneklerde sanat değişikliğinin farkında olmadan asker, moral, mühimmat, hedef, hareket veya reload simülasyonunu değiştirdiğine dair bulgu yok. Bu, bütün olası savaşların eşdeğerliği iddiası değildir. Son retreat/accept yine93 kayıp,1107 asker, Moves0, Île-de-France konumu ve tek resolved ID ile döndü.

04/06/07 tam PNG'leri açıldı. Süvari seçimi kendi iki topun önünü açıyor;06'da yaklaşık x833–898/y432–476 alanındaki iki açık şekil artık kendi bataryamızla ilişkilendirilebiliyor. Ancak bunlar yumuşak duman yerine **opak, açık renkli dörtgenler**. Görsel kabul bu yüzden RED; otomatik GREEN sanat kalitesi onayı değildir.07'de aynı alanda bu iki dörtgen görülmüyor, fakat topçunun yeniden hasar aldığı etiketi alanın bir kısmını örtüyor. Exact visual clock veya per-source effect sayımı olmadığından kesin bulut yaşı ve tamamen temizlenme anı ileri sürülmüyor. Root/artist yeni shader/material düzeltmesini ayrı yeni build ile değerlendirecek; bu tamamlanmış koşu değiştirilmedi.

## Birleşik son askerî gate:2026-09-06 01:27:10 UTC

Root çıktısı `output/verify/military-art-final-20260906-012710-424-48b0deff`: tam GREEN,203s,176/176 EditMode,yeni build,171 komut/38 assert/21 PNG/21 JSON,21 otomatik kare ve10 browser testi. Runtime SHA256 `FC1E21937ACE6213B4F62FD20CD2E7727FE465ADA5BAA3E853F713CE61A4CFF6`. Script'in dondurulmuş smoke-uncovered (son quit hariç, artifact adları smoke- öneki) + güncel104 komut victory-campaign birleşimi olduğu kaynak metninden birebir doğrulandı. Komut veya bekleme süresi eklenip çıkarılmadı.

Yeni smoke01–07 içindeki **bütün Battle snapshot nesneleri** Smoke A ile birebir aynı. Tek atış11→10, sonraki pendingfalse ve diğer simülasyon verileri korunuyor. Ham smoke01/02 JSON'ları ve PNG'leri aynı; PNG SHA256 `E429032D578567B0352D6A6ACC8042BE2B376F418FD51C36566479FB678AED8B`. Ham smoke04/05 JSON'ları ve PNG'leri aynı; SHA256 `E33DE1AD367D7B9F2E0CE62F6B24F99C886BE13A5BADC30D477E7394CE5CE792`. Bu gerçek örneklerde görsel düzeltmenin pause veya savaşı değiştirdiği bulgusu yok.

İkinci yeni kampanyanın bütün12 raw JSON'u ayrı bonus-first run'ıyla eşit; dünya temizliği/yeniden başlangıç sonrası doğal zafer ve mali transfer değişmedi. Ayrıntı `victory-review-plan.md` birleşik gate bölümündedir. Bu ajan smoke06, ikinci dünya02 ve bonus11 tam PNG'lerini açtı. Önceki opak dörtgenler smoke06'da yok; duman çok düşük kontrastlı, buradan bağımsız güçlü sanatsal okunurluk kabulü verilmedi. İkinci dünya02'de önceki birlik veya duman üst üste kalmış görünmüyor. Sonuç PNG'si artık settled756 altın gösteriyor. Otomasyon/kampanya eşdeğerliği PASS, sanat kabulü root/artist görüşünden ayrı tutulur.
