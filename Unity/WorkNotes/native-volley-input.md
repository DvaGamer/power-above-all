# Gercek Shift secimi ve manuel aimed volley

2026-09-06 hazirlik. Henuz bu ajan test/derleme/Unity/player/input calistirmadi. Root iki40s fazi ve son gercek Space ile reload'u dondurma sirasini onayladi. Yeni `ShiftClick` yalniz sol Shift+sol mouse bileşimidir; serbest modifier veya event injection arayuzu eklenmedi.

## Arac ve zaman siniri

Root `native-input-review.ps1 -Action Start -PlayerPath <frozen-review-build> -ScriptPath <repo>/tools/native-volley.script -VisiblePlayer` kullanir; default180s player/240s owner korunur. Sonraki komutlar ayni tam receipt'i kullanir. `ShiftClick -X337 -Y769` ikinci alt HUD kartinin basligina basar. Koordinatlar1440x900 canvas'idir; helper DPI/letterbox donusumunu uygular.

Arac fiziksel LeftShift VK0xA0/scan0x2A eslemesini kullanir; herhangi Shift zaten basiliysa reddeder ve onu birakmaz. Kendi Shift down olayi,70ms, tekrar owned-window/focus/pointer denetimi, mouse down70ms, mouse up70ms, sonra Shift up sirasi vardir. Mouse up ile Shift up arasindaki bekleme Unity'nin kart tiklamasini modifier hala basiliyken gormesi icindir. Kayip focus veya mouse hatasinda nested finally kendi bastigi tuslari birakir; guard reddi basari sayilmaz. Pure testler ayni dar sirayi sahte olay callback'leriyle calistirir, native event gondermez. PID/hash/parent/handle/sure/kayit denetimleri degismedi.

## Iki gercek faz

`00-start` normal semantic deployment sonunda, erken gelir; helper12s hazirlik siniri icindir. Bütün ranged player birlikleri hold fire. Daha onceki artillery pozisyonu14/−6, piyade merkez ve suvari yan pozisyonlari ayni shared player emirleriyle kurulur. Normal unpause ve bounded arrived/volley-ready beklemelerinden sonra pause gelir. Yaklasik25s'de `01-artillery-ready` PNG/JSON hazir: yalniz Id3 secili, Ammo11,Reload0,FireAtWill=false. Root bu yeni kareyi gordukten sonra40s penceresinde:

1. Gercek `Key Digit1` ile Id0'i sec.
2. Gercek `ShiftClick X337 Y769` ile ikinci kart **basligini** sec. Kartin ayrinti alani button degildir. Iki kartin vurgusunu Temp karesinde gor.
3. `02-native-group` olusana kadar baska selection/Space gonderme. JSON SelectedIds tam[0,1] olmali; diger6 regiment secili olmamali. Bu ilk pencere paused kalir, bu nedenle01/02 arasindaki butun simulation alanlari degismemeli; yalniz selection/selection-dependent observation farklari kabul edilir.

Ikinci40s pencere `02-native-group` sonrasinda baslar:

1. Gercek `Key Digit4`: artillery Id3 tek secili olmali. Bu ayni zamanda Shift'in birakildigini sinar; Shift hala basili kalmis olsaydi Add ile[0,1,3] olusurdu.
2. Gercek `Key Space`: unpause. Volley dugmesi pause'da bilerek devre disidir; once atis icin etkin gorunmelidir.
3. **Bir** gercek `Click X1332 Y817` ile aimed volley. Fire-at-will dugmesine basma; topcu hold fire kalmali.
4. Tool tamamlaninca gecikmeden gercek `Key Space` ile yeniden pause. Yalniz bu faz sonunda degil, reload bitmeden durdur:40s bekleyip sonra pause etmek15s cooldown'u kaybettirir.
5. `03-native-volley` ve0.75s sonraki `04-native-pause-held` ciksin; baska girdi gonderme. Script bundan sonra gercek retreat/accept ile normal sinirli kapanir, dogal zafer iddia etmez.

Hazirlik~25s +80s native pencereler + checkpoint masrafi ile normal toplam~115s beklenir, default180s yeterli ilk adaydir. Bekleme kosullari basarmazsa veya root gec kalirsa sonuc RED/kapsanmamis kalir; fake snapshot veya semantic emirle eksik native adim tamamlanmaz.

## Kabul kaniti

01->02 JSON: exact SelectedIds[0,1] ve secilmeyen diger birlikler; paused/elapsed/ammo/reload/konum/hedef/asker/moral sabit. Root native Digit1 ve ShiftClick Temp karelerini gorup kaydeder. Pause'i andiran goruntu tek basina group kaniti sayilmaz.

02->03: exact SelectedIds[3]; ayni oyuncu artillery Id3 Ammo11 ->10 **bir kez**, Reload>0, AimedVolleyPending=false ve FireAtWill=false.03/04 butun raw JSON ve PNG byte-esit olmalidir; bu yeni atis veya cooldown akisinin pause'da surmedigini gosterir. Gercek native volley dugmesi ve son Space Temp kareleriyle birlikte okunur. Pendingtrue gecici ani veya mouse olaylarinin ayni simulation tick'te oldugu iddia edilmez. Normal dusman atesi/movement unpaused bolumde ilerleyebilir; butun savas state'inin02/03 esitligi beklenmez.

Player protokolu yalniz mevcut pause/outcome/return/assert komutlarini kullanir; kesin group/ammo/kalan cooldown kabulunu root JSON audit'i yapar. Bu alanlar yanlisken arac genel native0/receiptPASS verse bile group/volley proof verilmez. Final owner native exit/noTimeout ve otomatik kare kontrolu ayri kaydedilir; rapor en fazlaPARTIAL. Gercek run kaniti sonra bu dosyaya eklenecek.

Kaynak okuma envanteri50 komut/6 PNG/6 JSON/11 assertion.01-artillery-ready ile retreat arasinda semantic select/pause/volley/fire/move/formation komutu bulunmadigi dogrulandi. Beklenen snapshot/PNG isimleri iki fazi ve pause ciftini ayri tutar. Modifier mapping, onceden basilmis Shift reddi, focus kaybi sonrasi yalniz Shift release, mouse-down hatasinda iki release ve mouse-up hatasinda bile nested Shift release icin pure kontroller hazirlandi. Root testleri kendisi calistiracak; bu ajan parser/test/native event/derleme calistirmadi. Kaynaklar freeze teslim edilir.

## Gercek native run: 2026-09-06 01:58:59 UTC

Root once 51 / 51 native pure checks PASS bildirdi; bu kontroller player veya input baslatmadi. Sonra `output/verify/native-input-20260906-015859-f35ec6e1` run'ini baslatti. Bu kez ilk Start exit 0 verdi. Owner PID 17704, baslangic 01:59:00.0864902 UTC; player PID 14192, baslangic 01:59:00.5512550 UTC. Build `military-art-final-20260906-012710-424-48b0deff`, runtime SHA256 `FC1E21937ACE6213B4F62FD20CD2E7727FE465ADA5BAA3E853F713CE61A4CFF6`. Default player 180 saniye / owner 240 saniye butceleri kullanildi.

Root her gercek toolcall'in exit 0 ve dogru owned PID/client verdigini, ilgili PNG'lerin hepsini actigini bildirdi. Temp kok yolu `C:/Users/USER/AppData/Local/Temp/codex-shot-2026-09-06_`; saatler Istanbul:

| Gercek olay | Temp saati / gozlem |
| --- | --- |
| Ilk hazirlik karesi | 04:59:06, actual 00-start |
| Digit1 | 04-59-42 |
| ShiftClick, ikinci kart basligi | 04-59-43, iki piyade secili |
| Digit4, actual 02-group sonrasinda | 05-00-39, tek artillery |
| Space, unpause | 05-00-40 |
| Tek aimed volley Click (1332, 817) | 05-00-42 |
| Space, yeniden pause | 05-00-44, Ammo 10 ve HUD reload 14 gorundu |

Bu ajan 01 / 02 / 03 / 04 JSON'larini, butun alan farklarini ve actual 02 / 03 tam PNG'lerini bagimsiz okudu. Onceki Temp input goruntulerinin tamami bu ajan tarafindan yeniden acilmis gibi sunulmuyor; onlarin girdi ve gorsel gozlemi root'a aittir.

### Exact group ve simülasyon

01-artillery-ready SelectedIds [3]; 02-native-group tam [0, 1]. Regiment Selected flag'leri de ayni ID listeleriyle eslesiyor; gizli ek selection yok. 01 -> 02 arasinda butun Battle top-level alanlardan yalniz SelectedIds / Regiments degisti; Regiments icinde yalniz Id0 Selected false -> true, Id1 false -> true, Id3 true -> false. Kalan butun regiment alanlari, battle elapsed 24.64990997314453 ve campaign context ayni. Bu gercek Shift group secimidir; paused simülasyon ilerlememiştir.

03-native-volley'de exact SelectedIds [3], yani Digit4 tek bataryaya geri donmus ve modifier secime takili kalmamis. Id3 Ammo 11 -> 10, Reload 0 -> 13.689413070678711; pending false, FireAtWill false, Moving false. Elapsed 28.04985809326172; root'un unpaused araliginda normal dusman savasi ilerlemis. Bir sonraki 04-native-pause-held de ayni degerleri tasiyor. 03 / 04 butun raw JSON metinleri ve PNG dosyalari byte-esit; PNG SHA256 `5CEA2B4C79746E8706BCBE6D5AD9ED693F24A64B8D04FE21E6D7861C973FFCB6`. Tek cephane tuketimi ve pozitif reload'un pause'da sabit kalmasi PASS. Gecici pending true ani veya ayni tick double-click kaniti yoktur.

02 PNG'de iki piyade karti vurgulu; 03'te yalniz artillery vurgulu ve 10 cephane / yuvarlanmis 14 saniye reload gorunuyor. Paused volley dugmesi devre disi ve hover mesaji devam etmeyi soyluyor; bu son pause icin beklenen davranis. Dumanin sanatsal kabulunu bu input kanitina eklemiyoruz.

### Sonuc ve build butunlugu

Player native exit 0, timedOut false, tamamlanma 02:00:52.6454842 UTC. Owner PARTIAL, toplam 117.48 saniye; 50 komut / 11 assertion / 6 PNG / 6 JSON ve 6 otomatik kare kontrolu PASS. EditMode / yeni build / browser bu native run'da atlandi. Normal retreat / accept return assertion'i gecti; dogal zafer iddia edilmez.

Native owner makbuzu executable / runtime / script parmak izlerini tasir. Buna ek olarak bu ajan run sonrasinda full-gate `build-result.json` manifestindeki 141 dosyanin tamamini salt okunur tekrar hashledi: gercek dosya sayisi 141, her beklenen path / size / SHA256 ayni, fark 0. Bu sonradan yapilan manifest kontrolunu native owner'in kendi gate'iymis gibi adlandirmiyoruz. Artefaktlar, helper'lar ve build dosyalari degistirilmedi; yeni player / test / compile / input baslatilmadi.

Bu run'in Start'i temiz exit 0'dır. Onceki native-victory run'indaki ilk sessiz Start exit 1'in nedenini bu sonuc aciklamaz; onceki sinir ve eklenen hata gorunurlugu kaydi oldugu gibi korunur.
