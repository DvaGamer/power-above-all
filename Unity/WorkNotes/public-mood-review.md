# Halk desteği: gerçek eşik yolu ve ilk Economy görüntü yöntemi

6 Eylül 2026. `tools/public-mood.script` hazırlandı; bu ajan test, Core probe, derleme veya oyuncu başlatmadı. Root'un tamamlanmış `ActiveCivilPolicyProbe-2026-09-06T05-02-49-038Z-5645c958/probe.stdout.log` içindeki0..8 haftalık aktif yol ve PASS873 kaydı okundu. Yeni Urban helper/UI kabulü henüz bu script'in gerçek sonucu değildir.

Root'un istediği public sıra: legacy new → budget1000 → Champagne accord → Champagne bread → Île subsidy. Probe aynı ilk haftada subsidy'yi önce, sonra accord ve bread verir. Subsidy açmanın anlık stok/Approval etkisi yok; ücret ve destek haftalık hesapta oluşur. Bu nedenle ekonomik ve sosyal değerler aynı; ilk üç journal kaydının sırası farklıdır. Probe JSON'u ile ham eşitlik iddia edilmez.

## Ölçülmüş rota

| Tamamlanmış hafta | UrbanApproval | Gold / Food | Troops / Manpower | O haftanın gerçek Tax / ArmyCost / NetFood |
| --- | --- | --- | --- | --- |
|0, emirlerden önce |35 |840 /360 |1200 /2400 |Henüz hesap yok |
|0, emirlerden sonra |37 |840 /320 |1200 /2400 |İlk hesap197 /136 /−16 |
|1 |40 |901 /304 |1200 /2400 |197 /136 /−16 |
|2 |43 |964 /288 |1000 /2600 |199 /136 /−16 |
|3 |46 |1060 /279 |1000 /2600 |216 /120 /−9 |
|4 |49 |1159 /270 |1000 /2600 |219 /120 /−9 |
|5 |52 |1281 /262 |1000 /2600 |242 /120 /−8 |
|6 |55 |1406 /254 |1000 /2600 |245 /120 /−8 |
|7 |58 |1532 /246 |1000 /2600 |246 /120 /−8 |
|8 |61 |1659 /238 |1000 /2600 |247 /120 /−8 |

Her hafta subsidy20 gıda gerçekten ödenir ve urban+3 olur. Bread başlangıçta40 gıda ve urban+2'dir; dört hesaplık accord tax forgone14+14+16+15=59, haftalık gelirde zaten eksiktir. Week2 eski1200 kişinin136 gideri hesaplandıktan sonra200 asker rezerve aktarılır; Dumas Relationship−4 ayrı mevcut kuraldır. Week4 anlaşma tamamlanır. Probe0..8 arasında sonraki CivilOrders hiçbir yeni bread/accord/subsidy değişimi yapmaz; bundan dolayı script ilave politik karar uydurmaz.

Bu rotada39/40 veya59/60 tam komşu değerleri görülmez. Gerçek geçişler37→40 ve58→61'dir; yeni fixture için stok/destek yazılmaz veya bir sonraki hafta ilerlemiş gibi gösterilmez. Normandy seçili kalır: başlanırkenU30;37→40 hesabındaU30 korunur,58→61 hesabındaU29 olur. Böylece aynı hafta ödenen subsidy sonrası desteğin bölgesel etkisi gözlenebilir. Salt mevcut37 değerine göre yanlış+1 uygulanması veya58'e göre düşüşün bir hafta geciktirilmesi bu gerçek yerel assertion'ları bozacaktır.

## Engellenmiş hafta ve görüntü yöntemi

Week2 gerçek petition anında UrbanApproval43'tür;65 bu public yolun sayısı değildir. Script sonraki `week` çağrısını dener, bütün campaign `same` ile değişmez kalmalıdır; Economy RU/TR bu engellenmiş durumda çekilir. UI bir sonraki hafta uygulanabiliyormuş gibi yeni bölgesel etki göstermemelidir. Gerçek `petition negotiate` sonrasında urban43 kalır ve yol devam eder. Engellenmiş state save/load ile de aynı kalmalıdır.

12 PNG /8 JSON: baseline35 RU/TR, hesap öncesi37→40 RU/TR, gerçek40 sonrası RU/TR, blocked petition43 RU/TR, hesap öncesi58→61 RU/TR, gerçek61 sonrası RU/TR. Her dil çifti `same` kullanır. Son state gerçek save/load sonrası yine aynıdır. Sayılar script'te root'un ekleyeceği readonly UrbanApproval key'iyle denetlenir; UI kategori metni yeni Core helper'dan gelir, script kendi kategori formülü üretmez.

Bütün Economy karelerinde root'un sonraki yerleşim kararıyla mevcut `scroll document5000` kullanılır: yeni açıklama Economy'nin altındadır, Unity scroll konumunu gerçek document sonuna clamp eder. Bu rastgele bir ekran koordinatı değil, root'un istediği mevcut bottom-scroll yoludur; full frame okunurluğu yine ilk gerçek PNG'den kabul edilmelidir. Hafta sonrasındaki40 ve61 görünümleri için Economy ve bottom-scroll açıkça yeniden seçilir. Aynı görüntüde gerçek bütçe1000 ve hafta2 azaltımı vardır. Hedef0 için yeni uzun yol eklenmedi: bu bağımsız eşik incelemesinde stok veya asker enjekte etmek gerekmez; önceki gerçek `army-establishment-zero.script` yolu ayrı kanıttır.

## İlk gerçek oyuncu sonucu: 05:29 UTC

`output/verify/public-mood-first-20260906-052918-104-2aaa7720` tamamlandı: result `GREEN`, 407/407 Unity testi, yeni build, 12 PNG, 106 assertion, 8 JSON, 12 frame kontrolü ve 10 browser kontrolü geçti. Protokol 199 komutla `success=true`; bitiş zamanı `2026-09-06T05:29:51.3620436Z`. Bu bölüm mevcut sonucu salt okunur inceleyerek yazıldı; ajan yeni test veya oyuncu başlatmadı.

Sekiz gerçek JSON yukarıdaki probe rotasıyla aynı ekonomik ve sosyal değerleri verdi. İlk kampanya Urban 35 / Gold 840 / Food 360; gerçek başlangıç emirlerinden sonra 37 / 840 / 320; hafta 1 sonunda 40 / 901 / 304. Hafta 2 sonunda 43 / 964 / 288 ve Troops 1000 / Manpower 2600 ölçüldü. Hafta 7 sonunda 58 / 1532 / 246, hafta 8 sonunda 61 / 1659 / 238 görüldü. Son Power 59, Dumas Relationship 46 ve Normandy Unrest 29; ilk Normandy Unrest 30 idi.

Gerçek journal içindeki hafta 1–8 `Tax / ArmyCost / NetFood` dizileri sırasıyla `197/136/-16`, `199/136/-16`, `216/120/-9`, `219/120/-9`, `242/120/-8`, `245/120/-8`, `246/120/-8`, `247/120/-8`. Böylece hafta 2 eski 1200 askerin gideri gerçekten ödendi; sonra `log.establishment.reduced [200,1000,4,1000]` kaydı oluştu. Aynı hafta petition negotiate kaydı mevcut. Accord bitiş kaydı hafta 4'te Champagne için oluştu; hafta 7–8 state'lerinde aktif bölge boş, eski UntilWeek 4 korunmuş.

`shots.log`, bütün RU/TR karşılaştırmalarının tam campaign `same` kontrolünü geçtiğini gösteriyor. Özellikle gerçek petition açıkken sonraki `week` denemesi Week 2 / Urban 43 dahil bütün campaign'i değiştirmedi. `06-petition-blocked.json` ve `07-petition-loaded.json` ham SHA256 değeri ortak: `D4410B5E151E72BF9CC114F4A5E837EEEFBFD50B7E21B85871844271F1B35D5D`. Son `10-after-61.json` ile `11-final-loaded.json` da ham olarak aynı: `36B9E745F7E2961B98683672E6D5D009FBD209784D8927177B65464184A4EC44`.

Son script'te her Economy PNG öncesinde `lang` ardından `scroll document 5000` gelir; dil değişiminin scroll'u sıfırlaması giderilmiştir. Petition gerçek modal olarak kalır: Economy'yi örtebilir ve bunun okunurluğunu root gerçek karelerden değerlendirmelidir. JSON/assertion başarısı tek başına bu örtüşmenin görsel kabulü değildir; görüntü için state veya modal enjekte edilmedi.

Root'un başlattığı `public-mood-final-20260906-053711-706-250a5a71` ayrı bir sonuçtur. Root, değişikliklerin kısa UI metni ve kesin average gösterimiyle sınırlı, Core'un aynı olduğunu bildirdi. Bu ilk koşu incelemesi final koşusunun tamamlandığı veya kabul edildiği iddiası taşımaz.
