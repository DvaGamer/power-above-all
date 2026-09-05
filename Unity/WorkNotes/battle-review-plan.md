# Gerçek taktik emirlerle sınırlı oyuncu incelemesi

Bu plan salt okunur kaynak incelemesidir. Yeni API, script veya savaş davranışı henüz uygulanmadı; bu ajan Unity/player başlatmadı. Hedef normal simülasyondan doğan sonucu görmek ve seferde bir kez uygulandığını kanıtlamaktır. Zafer henüz kanıtlanmış değildir.

## Mevcut kaynakta emir yolları

- `TacticalBattle.HandleInput`186: Space doğrudan paused değiştirir. Sayı tuşları `SelectIndex`241'e gider. Harita sol tık seçimi ve sağ tık emri aynı yöntemde ayrıca yazılmıştır. Sağ tık yer düzlemiyle ışın kesişimini bulur; seçili komuta edilebilir birliklerin merkezine göre mevcut aralıklarını korur ve `Bound` ile x[-36,36], z[-28,30] alanına sınırlar.
- Shift+harita tıklaması zaten seçili birliği seçimden çıkarır; Shift+sayı/kart seçimi yalnız ekler. Tek bir shared API bu iki davranışı bilerek korumalıdır. Slotlar oyuncunun gördüğü1–4'tür; komutlar düşman liste indislerine erişmemelidir.
- `OrderFormation`500 ve `SetFireOrder`1156 seçim üzerinde çalışır. Kare düzeni seçili piyade/milise uygulanır; süvari/topçu atlanır. Karma seçimde geçerli birliklerin emri korunur. Oyuncu duraklatılmışken seçim, hareket planı, düzen ve ateş politikası verebilir.
- `OrderVolley`514 özel yönteminin kendisi paused/Active/ended koruması yapmaz; duraklamada ateşi yalnız `DrawHud` düğmesini devre dışı bırakarak engeller. Bu yöntemi reflection ile doğrudan çağırmak insan oyuncunun kuralını aşar. Yeni inceleme böyle bağlanmamalıdır.
- `Finish`466 sonuç üretir; `AcceptOutcome`490 yalnız bitmiş/henüz aktarılmamış raporu bir kez gönderir. `GameApp.CompleteBattle`304 hedef, battleId ve gerçek raporu Core'a geçirip atlası geri açar. Mevcut AutoShots `retreat` özel Finish(false,true), `accept` özel AcceptOutcome reflection çağrısı yapar; doğal zafer incelemesi bunlara yeni bir Finish(true) yolu eklememelidir.

## Küçük ortak emir API'si

Klavye, harita tıklaması, HUD düğmeleri ve AutoShots aynı public yöntemleri kullanmalı. Input yalnız fareyi dünya X/Z noktasına ve tuşu slot/seçim kipine çevirir. Private Regiment, liste, RNG veya saat dışarı açılmaz.

| Önerilen API | Ortak davranış |
| --- | --- |
| `SelectPlayerRegiment(slot, Replace/Add/Toggle)` |1–4 oyuncu slotu, komuta edilebilirlik; yanlış indis atomik ret. |
| `MoveSelected(Vector2 worldXZ)` | Sonlu koordinat, mevcut grup aralıkları ve Bound; konumu değil hedef/emri değiştirir. |
| `SetSelectedFormation(Line/Column/Square)` | Mevcut uygun tür süzmesi, aynı düzene tekrar emir mevcut maliyeti tekrar ödemez. |
| `SetSelectedFireAtWill(bool)` | Seçili komuta edilebilir birliklere mevcut ateş politikası. |
| `VolleySelected()` | Aktif/bitmemiş/duraklamamış savaş, komuta edilebilir ve mevcut CanAttack koşullarını karşılayan birlik; Shoot yalnız buradan mevcut kuralla çağrılır. |
| `SetPaused(bool)` | Space/HUD aynı API ile tersine çevirir; script istenen değeri açık söyler. |
| `Retreat()` / `AcceptReport()` | Var olan gerçek geri çekilme/tek seferlik devam düğmeleri; sonuç parametresi yok. |

Başlangıç için küçük `BattleOrderResult { Ok, ReasonKey, AffectedCount }` yeterlidir. Karşılanmayan emir scripti açık nedenle başarısız kılar. Aynı zaten-geçerli emrin tekrarında Ok=true/AffectedCount0 olabilir. Bütün yöntemler Active/!Ended ve uygun seçim kontrollerini kendi içinde yapar; GUI.enabled tek yetki sınırı olmaz. AcceptReport'un özel kuralı ended/undelivered'dır. Normal düzen emrinin mevcut cohesion/reload maliyeti korunur; inceleme bu alanları doğrudan yazmaz.

## Önerilen en küçük script dili

Tek `battle` komutu altında aşağıdaki alt komutlar yeterli. Kampanya `select`, `state`, `expect`, `remember/same`, `shot`, `save/load`, `accept` anlamları korunur.

```text
battle select 1 replace
battle select 2 add
battle select 2 toggle
battle move -20 6
battle formation line
battle formation column
battle formation square
battle fire hold
battle fire free
battle volley
battle pause on
battle pause off
battle state tactical-orders
battle wait active 5
battle wait arrived 30
battle wait volley-ready 45
battle wait ended 120
expect BattlePaused True
expect BattleEnded True
expect BattleWon True
```

Sayısal argümanlar invariant culture ve finite olmalı; hareket x/z negatif olabilir, mevcut yalnız pozitif süre ayrıştırıcısı kullanılmamalıdır. Slot ve enum kapalı seçeneklerdir. `battle state` için mevcut güvenli dosya adı/benzersizlik kuralı ve shots klasörü kullanılır; `Get-ReviewPlan` bunu durum artefaktı saymalı, receipt.states tam listeyi içermelidir. `expect` ek alanları salt okunur snapshot sorgularıdır; BattleWon yalnız bitmiş raporda okunabilir. Gerçek zafer hedefli test Won=true bekleyebilir ama false sonucu gizleyemez.

İlk inceleme yenilgi veya zaferi dürüstçe raporlayan keşif olabilir: yalnız BattleEnded/rapor aktarımı zorunlu. Başarılı bir taktik plan gözlendikten sonra ayrı zafer regresyonu Won=true koşulu ekler. Bir sonuç enjeksiyonu veya kayıpları sabitleme yapılmaz.

## Kopuk, salt okunur savaş snapshot'ı

`CaptureSnapshot()` yeni DTO/array kopyaları döndürmeli; canlı Regiment, selected veya outcome nesnesi döndürmemeli. Önerilen alanlar:

- Savaş: schemaVersion, Active, Paused, Ended, Delivered, ElapsedSeconds, OriginalTroops; PlayerHold/EnemyHold ve konvoy X/Z; SelectedIds. Kampanya battleId, başlangıç bölgesi/hedefi ve hazırlık önizlemesi GameApp'ten ayrı salt okunur bağlam olarak eklenebilir.
- Birlik: Id, PlayerSlot/Player, Kind, Original/Men, Morale, Cohesion, Fatigue, Ammo, Reload, Formation, FireAtWill, Moving, Routed, Withdrawn, Commandable, PositionX/Z, DestinationX/Z, Facing; CanVolley ve VolleyReasonKey. Bunlar mevcut kullanıcıya görünen konum/kuvvet/durumların ölçülebilir kanıtıdır.
- Sonuç: HasOutcome, Won, Casualties, EndingMorale, MilitarySuppliesRecovered, CampaignReturnMorale. HasOutcome açık olmalı; bitmemiş savaşın varsayılan false'u yenilgi diye okunmamalıdır. İç normal bitiş noktası salt okunur bir neden kaydedebilir: player/enemy objective, opposing force gone, player force threshold veya explicit retreat. Bu neden reviewer tarafından atanamaz.

Snapshot yalnız gözlemdir. `elapsed`, accumulator, visualClock, playerHold/enemyHold, Men/Morale/Reload/Ammo, RNG veya callback'e doğrudan yazı yoktur. Snapshot alma Update/Simulate çağırmaz.

## Koşullu bekleme ve başarısızlık kanıtı

`battle wait <condition> <1..120 seconds>` her normal karede koşulu okur; süre `Time.realtimeSinceStartup` ile ölçülür. Simülasyonu hızlandırmaz, sabit adım çalıştırmaz, hedef sayacını ilerletmez ve otomatik duraklama kaldırmaz. Mevcut player dış timeout300s ayrıca kalır.

- active: GameApp'ın kurye aşamasından gerçekten BattleActive'a geçmesi.
- arrived: boş olmayan seçimin hâlâ komuta edilebilir bütün üyeleri hareketi bitirmiş; birliğin bozulması/çekilmesi varış sayılamaz.
- volley-ready: en az bir seçili komuta edilebilir birlik için gerçek VolleySelected uygunluğu, duraklama dahil. Bu koşuldan önce hold fire gerekir; free fire kendi fırsatını normal Update'te tüketebilir.
- ended: gerçek bitiş bayrağı ve üretilmiş rapor birlikte. Bitiş beklenirken Active beklenmedik biçimde kapanırsa başarısızlık; yeni atlas durumu sonuç yerine kabul edilmez.

Süre dolarsa son snapshot, seçili birliklerin readiness nedenleri ve pause durumu benzersiz timeout artefaktına yazılır; script/receipt/native çıkışı başarısız olur. Fokus kaybının normal OnApplicationFocus duraklaması teşhiste görünür kalır. Bekleyen koşul adına otomatik zafer, geri çekilme veya sessiz başarı yoktur.

## İlk gerçek denemenin kanıt sırası

1. `new` ile izole kampanya; Champagne seçimi, başlangıç kampanya state/remember, normal march, `battle wait active 5`. Gerçek savaşın seçimi/haritası görünür.
2. Normal pause emri; birlik seçimleri, düzen, hold fire ve sağ-tıkla aynı dünya hedefi API'si. Emir öncesi/sonrası snapshot ve görüntü. Başlangıç piyade/topçu korunurken süvari/rezervin konvoy çevresine manevrası bir taktik varsayımıdır; otomatik başarı vaat edilmez.
3. Resume; koşullu varış/hazır oluş bekleme; normal volley; uygun aralıkta yeni oyuncu emirleri. En az bir gerçek yaylımın mühimmat/reload/hasar etkisi ve karşı hareketi gözlenir. Kayıp sayısı duvar saatine göre sabitlenmez.
4. Natural ended bekle; RU/TR rapor görüntüsü ve tam sonuç snapshot'ı. İlk koşuda sonuç neyse saklanır. Zafer özel koşusu ayrıca Won=true ister ve yenilirse RED olur.
5. Gerçek `AcceptReport` üzerinden mevcut `accept`; atlas dönüşü. Önceki askerden gerçek Casualties çıkarıldığı, rapora uygun moral/ikmal, doğru ordu bölgesi ve battleId'nin yalnız bir kez eklendiği karşılaştırılır. Sonra save/load tam kampanya eşitliği ve atlas görüntüsü; son komut quit.

Bu ortak API'yi kabul ederken küçük NUnit sınırları yeterlidir: duraklamada volley/inaktif/bitmiş emirlerin atomik reddi, seçim kipleri ve karma kare düzeninin gerçek uygunluğu, grup hareketinin aralık/sınır koruması, raporun bir kez aktarılması. Bu testler public emir yolu üzerinden çalışmalı; gerçek oyuncu smoke simülasyon saatini veya sonucu reflection ile sürmemelidir.
