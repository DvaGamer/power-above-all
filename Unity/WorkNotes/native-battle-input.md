# Gerçek Windows savaş girdisi incelemesi

Kaynak: `tools/native-battle.script`. Bu dosya hazırlanırken Unity/player/girdi başlatılmadı. Kurulum yalnız yeni legacy sefer, Champagne yürüyüşü ve gerçek karşılaşmanın açılmasıdır. Script, inceleme penceresinde `battle select/move/pause/formation/fire` çağırmaz. Asıl seçim, sağ fare hareket emri, Space ve düzen düğmeleri root tarafından native araçla uygulanır.

Mevcut `Get-ReviewPlan` ile salt okunur ayrıştırma PASS:32 komut,13 assertion,4PNG,5JSON; `git diff --check` PASS. Gerçek native uygulama ve JSON değerlendirmesi henüz yapılmadı.

Başlatma: root, açıkça seçtiği doğrulanmış EXE ile `tools/native-input-review.ps1 -Action Start -PlayerPath <EXE> -ScriptPath <repo>\tools\native-battle.script -VisiblePlayer` kullanır. Dönen `owned-process.json` bütün sonraki çağrılara `-ReceiptPath` olarak verilir. Yardımcı kendi başlangıç makbuzu/ilk1440×900 `00-start.png` karesini bekler. Script toplam90 saniye gerçek girdi zamanı verir (60+30); sahip player180s sınırında kalır. Oyunun penceresi bu sırada odakta tutulur; başka pencereye geçiş oyunu otomatik duraklatır.

## İlk60 saniyede uygulanacak sıra

Koordinatlar OS ekranının değil,1440×900 tasarım tuvalinin koordinatlarıdır. Native araç gerçek client/DPI/letterbox dönüşümünü yapar. Her çağrı zaten kendi ekran görüntüsünü döndürür; bu ara kanıtlar özellikle her rakam tuşunun seçimi için saklanmalıdır.

1. `00-start` görüntüsünde DURAKLATILDI bandı yoksa `-Action Key -Key Space` ile duraklat. Zaten duraklamışsa aynı tuşla yanlışlıkla devam etme. Başlangıç `Battle.Paused` bilgisi `00-before-native-orders.json` içinde de vardır; pencere odak değişimi daha sonra duraklatmış olabilir.
2. Önce `-Action Key -Key Digit4` ile başlangıçtaki1 seçimini değiştir; sonra `-Action Key -Key Digit1` ile1'e dön. Böylece Digit1'in gerçekten seçim değiştirdiği de görünür. Ardından `-Action Click -X 1151 -Y 778` (kol düzeni); `-Action RightMouse -X 520 -Y 600`.
3. `-Action Key -Key Digit2`; `-Action Click -X 1331 -Y 778` (kare düzeni); `-Action RightMouse -X 650 -Y 600`.
4. `-Action Key -Key Digit3`; `-Action Click -X 1151 -Y 778` (kol düzeni); `-Action RightMouse -X 810 -Y 600`.
5. `-Action Key -Key Digit4`; `-Action RightMouse -X 960 -Y 600`. Topçu mevcut hat düzeninde kalır. Son seçili birlik4 olmalıdır.
6. Başka emir vermeden `shots/01-orders-paused.png` oluşmasını bekle. Bu kareden önce script PausedTrue, SelectionArrivedFalse ve HasOutcomeFalse beklentilerini geçirmiştir. Kare görünür görünmez, tercihen5 saniye içinde `-Action Key -Key Space` gönder; duraklama bandının kalktığını aynı komutun görüntüsünde kontrol et. Devam tuşunu yalnız bir kez gönder.
7. İkinci30 saniyelik bölüm bitene kadar yeni pencere/oyun emri açma. Script doğal hareketi, PausedFalse ve SelectionArrivedTrue ile kontrol eder; ardından gerçek geri çekilme/rapor kabulüyle atlas aktarımını sınar ve çıkar.

İlk bölümde13–14 native çağrı vardır; bunları sırayla, uzun ara yorumlar vermeden uygulamak60 saniyelik pencereye yeterli pay bırakır. Bir çağrı ret verirse hatayı sakla; test sonucunu düzeltmek için semantik emir veya JSON değişikliği kullanma. Yeni koşu gerekiyorsa farklı artefakt klasörü açılır.

Bu ek Digit4→Digit1 adımı yalnız gelecek koşu içindir. `native-input-20260906-000936-3ea85fdc` başarılı koşusunda Digit1 zaten seçili1 üzerinde gönderildi; o eski koşu ayrı bir Digit1 geçişini kanıtlamaz. Eski artefaktlar ve bu sınır değiştirilmez.

## Kaynaktan çıkarılan koordinatlar ve gözlemler

`TacticalBattle.HandleInput`, üst sıra Alpha1–4'ü oyuncu slotlarına, Space'i ortak pause API'sine ve sağ fareyi kamera rayının y=0 düzlemi kesişimine yollar. Oyun alanı tıklaması canvas y142..729 içinde olmalıdır. HUD Column düğmesi(1068,763,166,31), Square(1244,763,175,31); seçilen merkezler güvenle bu sınırlar içindedir. `ViewLayout` tuvali1440×900, kamera viewport'u(0,.19,1,.77), ortografik boyut31, kamera(0,55,-40) hedef(0,0,5) değerlerindedir.

Yukarıdaki dört sağ tıklamanın beklenen dünya hedefleri, yuvarlama toleransı yaklaşık0.3 olmak üzere aşağıdadır. Bunlar gameplay'e enjekte edilen sayılar değil, gerçek ekran rayının sonradan doğrulanacak izdüşümüdür. Yön/ölçek değişirse bu koordinatlar yeniden gözden geçirilir.

| Oyuncu slotu / Id | Son düzen | Beklenen DestinationX | Beklenen DestinationZ | Son seçili |
| --- | --- | --- | --- | --- |
|1 /0, Infantry | Column | -17.89 | -20.14 | false |
|2 /1, Militia | Square | -6.26 | -20.14 | false |
|3 /2, Cavalry | Column | 8.05 | -20.14 | false |
|4 /3, Artillery | Line | 21.47 | -20.14 | true |

Başlangıç dört dünya konumu X=-24,-8,8,24; Z=-18'dir. Hedefler yakındaki güvenli geri mevzilerdir; en yavaş topçunun hareketi de ikinci30 saniyeye sığacak birkaç dünya birimidir. Sabit kayıp veya RNG'ye bağlı sonuç beklenmez.

## Kanıtın değerlendirilmesi

- İlk ve duraklatılmış JSON'da dört hedefin başlangıçtan değişmesi, beklenen düzenler ve `SelectedIds=[3]` / yalnız slot4.Selected=true birlikte kontrol edilir. Duraklatılmış bölümde `Moving=true` emir planını gösterir; bu aşamada konumun hedefe ışınlanmaması gerekir. Her Digit çağrısından sonra native ekran görüntüsünde ilgili kartın seçildiği de görülmelidir; yalnız son durum ilk üç rakam tuşuna tek başına kanıt değildir.
- Çalışan JSON'da `ElapsedSeconds` artmış olmalı; seçili topçu `Moving=false` olmalı ve konumu hedefe0.35 civarında yaklaşmalıdır. PausedTrue ara durumundan PausedFalse son durumuna geçiş, gerçek Space devam yolunun kanıtıdır. Otomatik odak duraklaması tek başına bu kanıtı sağlayamaz.
- AutoShots mevcut beklenti dili, ayrı birliğin Id/düzen/hedef alanları için doğrudan `expect` sunmaz. Bu ayrıntılar burada açıkça root/ajan JSON incelemesi olarak bırakılır; scriptin toplam PASS sonucu tüm seçim/düzen ayrıntılarının otomatik kontrol edildiği şeklinde sunulmaz.
- `03-after-return.json` gerçek retreat sonrasında kampanya durumudur. `battle verify-return`, gözlenen rapordaki kayıp/moral/malzeme ve çözülmüş battleId aktarımını sınar. Bu kapanış taktik zafer denemesi değildir.
- Yeni native sahibi gerçek native çıkış0, scriptin tüm assertion/artifact sonuçları ve gerçek kare kontrolleri birlikte geçince yalnız PARTIAL yazar. Unity test/build/browser bu çalışmada atlanır. Önceki veya başarısız çıktılar değiştirilmez.

## İkinci gerçek koşuda Digit4→Digit1 kanıtı

`native-input-20260906-003635-cf3cced5`, `accord-layout-final-20260906-002826-992-56dba0b4` oyuncusunda tamamlandı: native0, timeoutfalse,100.4s,13assert/4PNG/5JSON ve gerçek kare kontrolü PASS; raporPARTIAL. Root başlangıçta açıkça Digit4 sonra Digit1 gönderdi ve `Temp03-37-04.png` topçu seçimini, `Temp03-37-05.png` piyadeye dönüşü gösteren iki ara görüntüyü açıp doğruladı. Bu, ikinci koşunun kanıtıdır; ilk koşunun idempotent Digit1 sınırı geriye dönük silinmez.

Bu ajan01/02 JSON ve native/result makbuzlarını salt okunur kontrol etti.01'de pausedtrue/elapsed20.59997, yalnız Id3 seçili; dört oyuncu birliği hâlâ ilk konumunda ve Movingtrue. Beklenen Column/Square/Column/Line düzenleri ve X hedefleri−17.89322/−6.26263/8.05195/21.47186, ortak Z−20.25765 doğrulandı.02'de pausedfalse/elapsed31.19981, aynı hedefler, bütün Movingfalse ve SelectedIds[3]; tüm birlikler hedeflerine0.3 dünya biriminden yakın. Gerçek Space devamı yaklaşık10.59984 simülasyon saniyesi bırakmış ve hareket tamamlanmıştır. Shift ve manuel volley bu koşunun kapsamına eklenmez.
