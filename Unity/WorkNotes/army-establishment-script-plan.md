# Hedef mevcut: gerçek player senaryosunun hazırlığı

Yalnız plan; Assets/tools dosyası değiştirilmedi, hiçbir süreç çalıştırılmadı. Root AutoShots arayüzü: `establishment budget <integer>`, `establishment campaign 0`, `panel establishment`; public expectation alanları ArmyPolicyId/ArmyTargetTroops/ArmyReductionDueWeek, HasArmyEstablishment, ArmyCost, ArmyConsumption. Gerçek sayılar root'un yeni Core probe çıktısından alınacak.

## İlk kısa yol: ertelenmiş maliyet ve yeniden asker alma

`new` → `establishment budget 1000` → belge RU/TR + state/save/load/same → `week` → belge mevcut ordu/aynı due → `week` → yeni dilekçenin arkasındaki gerçek aktarım snapshot'ı. Bu durumda yeni `week` ret/same, sonra `petition relief`. Eski başlangıç ordusunun ilk iki hesabı yeni hedefe göre geriye dönük ucuzlamamalı.

`week` ile ilk daha düşük hesabı göster; sonra mevcut `act recruit` ile normal altın/gıda/teçhizat/Manpower bedeli öde. Politika hâlâ budget1000 iken yeni açık due tarihini ve gelecekteki gerçek partiyi görünür kıl. Save/load gelecekteki tarihi korumalı. `establishment campaign 0` henüz ayrılmayan sonraki partiyi durdurur; tamamlanmış asker aktarımı geri alınmaz. Aynı mevcutta tekrar hedef seçme veya reddedilmiş komut bütün kampanyayı korumalı.

Bu yolun Gold/Food/Manpower ve bütün journal beklentileri yeni probe öncesinde sabitlenmez. Önceki sözleşmenin136→120 ordu maliyeti ve40→34 gıda karşılaştırması, yalnız başlangıçtaki koşullu aritmetiktir; bütün ülke bütçesi gibi sunulmaz.

## İkinci erişilebilir yol: Dumas iptali ve açık80 borç

Önceki gerçek player ve API probe'un ortak hazırlığı: `new` → `role-menu` → `role-start army` → `act subsidy` →0. haftada recruit →2. haftada relief ve ikinci recruit → normal haftalarla12'ye gel. Daha önce bu anda20 Dumas toplaması ve1472 asker ölçüldü; yeni Core'da tekrar ölçülmelidir.

12. haftada `mandate issue` → `establishment budget 1000`; her iki teklifin kendi vadesi ve bedeli görünür olsun.13. hafta mevcut yiyecek yardımı Dumas önerisini sufficient olarak kapatırken hiçbir erken asker aktarımı olmamalı.14. haftanın hesabı başlangıçta henüz vadesi dolmamış borçla kabul edilir; sonunda aktarım ve dueMandate birlikte görünür. Sonraki `week` atomik olarak reddedilmeli. `mandate fulfil` ilk Île borcunu80 altınla öder; asker azaltımının yeni tarihi veya asıl bölge borcu yeniden fiyatlanmamalı. Bu birleşik yol Core ajanına probe adayı olarak iletildi; yeni sonuç sayıları uydurulmadı.

## Sıfır hedef ve gerçek küçük son parti

Sıfır hedef için ayrı yeni başlangıç ve normal haftalar yeterlidir: iki hesapta bir en fazla200 ayrılır. İlk iki haftadaki dilekçeyi olağan yoldan çöz; bütün eski hafta etkileri tamamlandıktan sonra son yaşayan parti0'a indirir. O hesapta garnizonun eski etkisi vardır; sonraki hesapta yoktur. Dumas yeni duyurusu artık olmayan ordu için açılmamalı; recruit ile yeniden büyüme olağan bedellerini korumalı. Gerçek takvim ve sayılar yeni probe'da izlenecek.

Başka anlamlı küçük parti adayı: önce politika1000, sonra **gerçek** mevcut doğal savaş yolu. Eski build'de1200 ordunun196 kayıpla1004 döndüğü gözlenmişti; aynı sonucu yeni koşuda peşinen varsaymayız. Muharebe raporunu gerçek kazanç/kayıpla kabul ettikten sonra due hafta yaşayan hedef fazlasını aktarır; gerçek sonuç1004 olursa parti4'tür. Outcomes veya asker sayısı enjekte edilmez. Bu daha uzun muharebe kanıtı ilk kısa script'in tamamlanması için zorunlu değildir; root sonraki run bütçesine göre seçer.

100000000 rezerv kapasitesi ve açlığın104 kişilik son partisi gibi hassas sayısal sınırlar anlamlı Core fixture'larıdır. Sırf bu değerlere ulaşmak için gerçek player state'ine gizli yazım yapılmaz.

## Ölçülmüş fixture'lar hazır

Root'un `ArmyEstablishmentProbe-2026-09-06T02-51-25-163Z-ad948d18/probe.stdout.log` ve kaynak rotası doğrudan okundu: PASS148, yalnız public API. İlk yol **negotiate** dilekçe cevabını kullanır; önceki taslakta geçen relief burada kullanılmadı.1. hafta1200/2400 kişi,Gold911/Food362;2. hafta1000/2600,Gold979/Food362 ve yeni dilekçe. Her iki hesap da eski136 ordu gideri/40 asker gıdasını öder. Sonra negotiate+normal recruit, due4 korunarak hedef900, campaign iptali;3. hafta1200/2400,Gold939/Food341. Probe sınıra kadar gidip herkesi tekrar ücretsiz askere çevirmez.

Yeni `tools/army-establishment.script` bu aynı rotayı izler:13 PNG/10 JSON,64expect+7same. Uygulama öncesi mevcut UI'nın varsayılan1000 taslağı RU/TR üst/alt çekilir; belge ve dil gezintisinden sonra kampanyanın tam aynı kaldığı doğrulanır. Taslağa reflection veya yeni semantik yazım yapılmaz. Açık vade, gerçekleşmiş parti, yeni recruit vadesi, hedef değişikliği ve iptal archive/load eşitliğiyle korunur. Kaydırma650 başlangıç değeridir; gerçek görüntü kabulü bekleniyor.

Sıfır yolu ayrı `tools/army-establishment-zero.script`:10 PNG/12 JSON,85expect+4same. Hedef0 önce public API ile uygulanır, sonra RU/TR garnizon uyarısı çekilir; bu bir native taslak seçim kanıtı değildir. Probe ölçümü:12. hafta Troops0/Manpower3600,Gold2144/Food467; son hesap53 ordu gideri/7 asker gıdası öder.13. hafta ilk ordusuz hesap Gold2328/Food488 ve0 gider; ardından campaign+normal recruit200/Manpower3400,120altın/20gıda bedeli.12 ve13 JSON'ları garnizonun bölgesel farkının okunmasını sağlar. Kaydırma650/800 görsel başlangıç konumudur; bu ajan player/parser çalıştırmadı.

Ek `WorkNotes/ArmyMandateEstablishmentProbe.cs` birleşik army/Dumas/borç yolunun gerçek public API aracı olarak hazırlandı, fakat henüz çalıştırılmadı.12. hafta40gıda yardımı ve1000 hedefi,13. hafta NPC iptali,14. hafta gerçekleşen parti ve ayrı vadesi gelmiş80 borç, sonraki hafta atomik ret, doğru eski borç ödemesi, ardından16'ya normal hesaplar. Her hesabın eski ordu Forecast'i ve stok/günlük sonucu karşılaştırılır; tarihler ve bütün sayılar stdout'a yazılır. Root merkezi runner'ı başlatmadan sonuç veya asker kaybı miktarı iddia edilmiyor.

## Birleşik yol ölçüldü ve player fixture hazırlandı

Root `ArmyMandateEstablishmentProbe-2026-09-06T02-56-43-265Z-1541e9d6` çalıştırdı; stdout doğrudan okundu: PASS119. Bu ayrı hazırlık **relief** cevabını kullanır.12. haftada1472Troops/2000Manpower ve40 rol yardımı; iki hedefli takvim NPC13, asker/dueMandate14.13. hafta NPC sufficient;14. hafta eski ordu hesabının ardından200 gerçek aktarım:1272Troops/2200Manpower,Gold1590/Food1, sonraki azaltım16. İlk Île borcu80, due14 ve nextMandate16 korunur. Sonraki hafta ret tam state'i korur. Ödeme Gold1590→1510, Dumas46→50; azaltım tarihi16 değişmez.

15. hafta eski tüketimden102 açlık kaybı:1170Troops/2200Manpower,Gold1615/Food0, Dumas cooldown16 nedeniyle henüz yeni duyuru yok.16. hafta94 gerçek açlık kaybı daha olur;1076 yaşayanın yalnız76 kişisi rezervlere geçer:1000Troops/2276Manpower,Gold1713/Food0/Power46.5/Supply45/Morale61/Dumas46. Hedef1000'e ulaşılmış ve reductionDue0 olmuştur. Yeni Dumas duyurusu kalan1000 kişiyi kullanır: **due17/next20 ve ForageFood16 gelecekteki17. hafta önerisidir**.16. haftada16 gıda toplanmış sayılmaz.

`tools/army-establishment-linked.script` aynı gerçek emirlerle bu yolu kurar;12PNG/11JSON. İki takvim, hâlâ açık80 borç, ödeme ve son76 parti archive/load/same ile saklanır. RU/TR borç, sonraki azaltım ve günlük; finalde gelecekteki17. hafta toplama önerisi çekilir. Outcomes/asker/stok enjeksiyonu yok. Dosya root'a freeze teslim edildi; bu ajan parser/player çalıştırmadı. İlk pending/13→14 ve açlık sonrası16→17 karşılaştırmaları gerçek player koşusunun sonraki JSON kabulünde ayrıca okunacak.
