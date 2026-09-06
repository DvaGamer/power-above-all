# Zafer kararinin gercek Windows girdisi

Hazirlik:2026-09-06. Henuz player, Unity, derleyici veya test calistirilmadi. Root yeni kaynak/build freeze sonrasi yalniz kendi gorunur oyuncusunu baslatir. `native-victory.script` ilk karesi helper'in zorunlu `00-start.png` adini kullanir; savasin emir/shot sirasi mevcut dogal kazanilmis tactical fixture ile aynidir. Muharebe semantic shared emirlerle hazirlanir; popup icin test edilecek Esc/fare kararini script uygulamaz.

## Root baslatma arayuzu

`native-input-review.ps1 -Action Start -PlayerPath <explicit-review-build> -ScriptPath <repo>/tools/native-victory.script -VisiblePlayer -PlayerTimeoutSeconds 240`

Her shell komutu makine kuralina gore disaridan `bash -lc` ile sarilir. Player yolu `output/verify` icindeki bilinen build olmalidir. Sonraki Inspect/Key/Click komutlari baslatmanin verdigi tam `owned-process.json` makbuzunu kullanir; timeout'u tekrar gondermek gerekmez. Default180 korundu, kabul edilen CLI/receipt player araligi180..300 tam saniye. Owner butcesi daima player+60; bu run240/300. Exact owner command line, zamanlar, PID/parent PID, hash/path, focus, pointer/window ve finally release kontrolleri korunur. Rapor en fazla PARTIAL olabilir; native hata, timeout veya eksik kanit RED kalir.

## Onceden planlanmis60 saniyelik girdi sirasi

00-start erken hazirliktir; henuz popup'a tiklanmaz. Gercek savas yaklasik126s surdugunden root `shots/08-native-popup-ready.png` ortaya cikmasini ve kadri bekler. Bu kare `HasPendingVictory True` / base-state snapshot sonrasi cekilir. Ardindan script60s bekler. Bekleme kampanya haftasini veya maliyeti ilerletmez; sonucu doguracak tek siyasi eylem gercek Windows click olmalidir.

1. `Key Escape`: popup kapanmali, kampanya yerinde kalmali. Helper Temp karesini gor; bu pencereyi saklama kanitidir, decline degildir.
2. `Click X1189 Y169`: Council sekmesini sec. Ardindan yeniden acma dugmesini actual kareden dogrula. Onceki ayni canvas/font RU/TR karelerinde dugme merkezi1279/344 idi. Dil veya scroll degismisse y'yi gorerek belirle; tahmini tikla ilerleme.
3. `Click X1279 Y344` (actual rect dogrulaninca): popup yeniden gorunmeli ve ayni Champagne/bedel bulunmali. Bu tekrar acilmasi Esc'nin teklifi tuketmedigini gosterir.
4. `Click X1081 Y162`: popup TR. Temp karesinde Turkce baslik/bedeli gor.
5. `Click X1013 Y162`: popup RU. Temp karesinde Rusca baslik/bedeli gor.
6. `Click X919 Y562`: gercek prim dugmesi. Popup kapanmali. Ek siyasi tiklama gonderme; script60s sonunu kendi siniri icinde tamamlar.

Koordinatlar1440x900 mantiksal canvas'indir; mevcut helper DPI/letterbox donusumunu yapar. Butun native adimlar ayni yasayan owned receipt ile yapilir. Su anki popup'a Enter/ok ile secim testi atfedilmez. Gercek close dugmesi992/712 ve ordinary decline582/712 bu dizide **tiklanmaz**; semantic fixture/Core testleri native fare kaniti yerine gecmez. Decline icin ikinci gercek zafer veya ayri onayli fixture gerekir.

## Kabul ve sinirlar

Script sonunda pendingfalse, resolved1, Language ru beklenir;08-after-native-choice JSON ve09 PNG sonra normal quit olur. Native owner exit0/noTimeout ve protokol/kare sonucunu ayrica yazmalidir. Bu assertion'lar yalniz popup'in kapanmasini bonus diye adlandirmaya yetmez: root07-before/08-after JSON farkini okur. Gold farki ceil(before.Troops/12), Dumas Loyalty+5 capped, gercek zafer bolgesiControl+3 capped ve tek `log.victory.bonus` kaydi bulunmalidir; diger kaynak/terms/history sabit kalir. Inceleme tamamlaninca makbuz/PID/zamanlar ve her gercek Key/Click Temp karesi burada kaydedilir. Su an bu bir plan, input proof degildir.

Yaklasik126s savas + checkpoint masrafi +60s girdi icin240s player yeterli ilk adaydir; natural-ended condition ve180..300 ust siniri sureyi sinirli tutar. root gozlemi gec kalirsa sure doldu diye input veya sonuc uydurulmaz; RED kaniti korunur. Tool testi yalniz timeout/receipt dogrulamasini ve native fixture makbuzunu kapsar; oyuncuda tus/fare basmaz. Gercek sinif/GUI girdi davranisi root'un sonraki bounded run'ini bekler.

Kaynak freeze envanteri70 komut/9 PNG/8 JSON/18 assert; Accept dahil prefix sadece ilk PNG adi haric mevcut tactical fixture ile aynidir. Root40/40 native pure checks PASS bildirdi; testler player/pencere/input baslatmadi. Timeout default/range/receipt/live-command uyumu denetlendi. Bu haber gercek native popup input provasinin yerine gecmez; o sonraki run'da kaydedilecek.

## Gercek native kabul:2026-09-06 01:32:05 UTC

Root run `output/verify/native-input-20260906-013205-6d0f5541`, explicit240s player/300s owner. Owner PID4636, baslangic01:32:05.2713849 UTC; player PID10212,01:32:05.7681312 UTC. Military-art-final build/runtime `FC1E21937ACE6213B4F62FD20CD2E7727FE465ADA5BAA3E853F713CE61A4CFF6`. Owner/script/hash ve process/parent/time/path/native argv eslesmesi bu ajan tarafindan dar salt okunur CIM sorgusuyla gercek yasayan sureclerde goruldu; owner gizli, player gorunurdu. Helper/owner yasarken degistirilmedi.

Root ayni makbuzla asagidaki gercek input'lari gonderdi; her tool exit0, PID10212 ve client1440x900 verdi. Root her Temp karesini actigini bildirdi. Dosya kokleri `C:/Users/USER/AppData/Local/Temp/codex-shot-2026-09-06_`, saatler Istanbul:

| Gercek eylem | Temp PNG saati | Gozlenen davranis |
| --- | --- | --- |
| Escape |04-34-39 | Popup saklandi; decline uygulanmadi |
| Council click |04-34-40 | Konsey belgesi acildi |
| Reopen click |04-34-56 | Ayni Champagne zafer teklifi gorundu |
| TR click |04-34-58 | Popup Turkce oldu |
| RU click |04-34-59 | Popup Rusca oldu |
| Bonus click |04-35-01 | Popup kapandi; Gold756 ve Champagne Control74 yuvarlanmis gorundu |

Bu ajan son04-35-01 tam PNG'yi ayrica acti: kazada756, Champagne secili, kampanya mesaji84 maliyeti/Loyalty+5/Control+3 ile dogru. Onceki adimlarin native girdi ve Temp goruntu gozlemi root tarafindan saglandi; hepsini bu ajanin tekrar actigi iddia edilmiyor. Native girdi diliminde semantic victory/panel/lang komutu yoktu; final Language ru ve pendingfalse beklentisi gecerken gercek root click'i sonucu olusturdu.

07-before-native-choice ->08-after-native-choice tam alan incelemesi: Gold840 ->756,1004 askerin ceil(1004/12)=84 maasi; Dumas Loyalty60 ->65, Champagne Control70.5 ->73.5, Pending ID bos. Yeni tek `log.victory.bonus` kaydi disinda eski journal kuyrugu korundu. Degisen tek top-level alanlar Gold/Regions/Characters/Journal/PendingVictoryId; Regions icinde sadece Champagne Control, Characters icinde sadece Dumas Loyalty. Ordu ve harita Champagne'da, ayni tek resolved battle. Gold disindaki kaynaklar, Power/Fatigue ve eski soz/anlasma alanlari degismedi. Isolated archive'de pending bos; bu native script Load yapmadigi icin burada yeni full save/load kaniti ileri surulmez. Gercek outcome Won=true,125.8030777s/196 kayip.

Player native0/noTimeout ile01:35:18.3834128 UTC'de kapandi. Owner makbuzu PARTIAL,200.47s,70 komut/18 assert/9 PNG/8 JSON;9 otomatik kare kontrolu PASS. EditMode/build/browser bu native run'da atlandi. Basariyi GREEN'e cevirmedik. Bu sirada gercek ordinary decline veya ikinci recognize click'i denenmedi.

## Korunan baslatma siniri ve dar hata gorunurlugu

Ilk Start istemcisi3.6s sonra exit1 ve bos stdout/stderr verdi; buna karsin dogru owned player/receipt/00-start olusmus ve battle calisiyordu. Sonraki root Inspect01:33:46 UTC'de ayni makbuzla exit0 verdi; owner/helper degismemisti. Bu nedenle butun Start akisinin temiz oldugu soylenemez. Ilk terminating error yakalanmadigindan neden kesin bilinmiyor; readiness geciciligi veya dis runner hata akisi yalniz olasi aciklamalar, kanitlanmis neden degildir. Timeout240/300 numeric Int32, owner hash ve gercek argv dogru bulundu.

Iki surecin de bittigi ve final makbuzlarin varligi dar CIM sorgusuyla dogrulandiktan sonra root istegiyle yalniz `native-input-review.ps1` basina top-level trap eklendi: Exception.Message ve ScriptStackTrace stdout'a yazilir, exit1 korunur. Owner dosyasi, hash/path/PID/window guard'lari veya red kararinin anlami degismedi. Root bu yeni trap'i bitmis makbuzda gercek Inspect ile dogruladi: beklenen exit1, acik `Native review has ended; no further input is allowed.` mesaji ve Get-NativeOwnedPlayer/review stack'i gorundu; girdi veya yeni player yoktu. Bu gorunurluk duzeltmesi ilk Start nedenini giderdik iddiasi tasimaz. Tamamlanmis ciktilar oldugu gibi korundu; dosyalar tekrar freeze teslim edildi.
