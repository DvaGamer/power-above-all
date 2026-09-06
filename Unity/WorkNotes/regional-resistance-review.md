# Bölgesel direnç B: ilk public API ve UI fixture

6 Eylül 2026. Yalnız `tools/regional-resistance.script` ve bu not yazıldı. Root'un verdiği1114→1234→1106 hesabı kabul edilmiş yeni sözleşmedir; bu ajan henüz Core probe, Unity testi, derleme veya oyuncu çalıştırmadı. Eski Act/Recruit/Accord/Travel maliyetleri kaynak üzerinden okundu. İlk gerçek koşu root tarafından yeni kaynak freeze sonrasında yapılacak.

Script10 PNG ve8 state JSON hedefler. Beş görünümün her biri RU/TR: başlangıç Champagne, ordunun1600'e hazırlanmasından sonraki aynı hedef, gerçek vergi, gerçek ekmek ve ayrı başlangıçta anlaşma sonrası barış. Her dil değişiminde tam campaign `same` korunur. Henüz denenmemiş bir Province scroll konumu komuta eklenmedi; bu fotoğrafların yeni UI metnini gerçekten gösterdiği ilk PNG incelemesinde değerlendirilmelidir.

| Faz | Troops / Manpower | Gold / Food / Supplies | Champagne Unrest / Control | Resistance |
| --- | --- | --- | --- | --- |
| Yeni legacy |1200 /2400 |840 /360 /120 |69 /60.5 |1114, active |
| Île normal recruit + commission grant + extra recruit |1600 /2000 |600 /320 /90 |69 /60.5 |1114, active |
| Aynı Champagne'a tax |1600 /2000 |700 /320 /90 |81 /60.5 |1234, active |
| Aynı Champagne'a bread |1600 /2000 |700 /280 /90 |66 /62.5 |1106, active |
| Ayrı new + Champagne accord |1200 /2400 |840 /360 /120 |59 /63.5 |0, peaceful |

Tax yalnız sayı değiştiren hazırlık değildir:100 gerçek Gold gelir; yerel EliteLoyalty60→56, urban Approval−3 ve crown Approval+1 mevcut Core etkileridir. Bread40 Food öder; urban Approval+2, Lefevre Relationship+2 ve kontrol+2 uygular. Bu nested alanlar state JSON'larında karşılaştırılabilir; yeni expectation API'si uydurulmadı. İki alımÎle'de kendi huzursuzluk/moral ve faction maliyetlerini öder; Champagne'a gerçek bir eylem yapılana kadar aynı1114 kalmalıdır.

Ekmek sonrası state `06-bread-resistance.json` ile gerçek save/load sonrası `07-bread-loaded.json` aynı olmalıdır. Save/load öncesi farklı bir bölge seçilir; seçili Champagne'ın ve türetilen1106'nın dönmesi açıkça denetlenir.

Peace RU/TR'den sonra gerçek `march` vardır, fakat11. PNG eklenmez; sonuç `10-peaceful-march.json` ve gerçek yüklemeden sonra `11-peaceful-march-loaded.json` ile kanıtlanır. HasAccord/Until4 korunur, BattleActive=false, ResolvedBattleCount0, PendingVictory yoktur.

Barış aynı zamanda kolay yol demek değildir. Accord sonrasıU59, mevcut `difficult` eşiği50'nin üzerindedir:1200 kişiye18 Food,5 Supplies,2 Moves,20 Fatigue ve12 Supply maliyeti; hedefteU+2/Control−2. Beklenen gerçek varış: Food342, Supplies115, Moves0, Fatigue20, Supply88, Morale78; Champagne U61/Control61.5. Dolayısıyla hâlâ Resistance0 ve1200/2400 kişi, Week0; yeni savaş veya kayıp olmamalıdır. Son save/load tam state eşitliğini korur.

Bu fixture'da farther, sıfır ordu, taktik BattleEnemyTroops veya BattleOurTroops yoktur; bir savaş snapshot'ı olmadan bu alanlar kullanılamaz. Genel UI veya taktik kabul iddiası bu script'in yazılmasıyla oluşmaz.
