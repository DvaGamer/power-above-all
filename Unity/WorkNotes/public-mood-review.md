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
