# Bölgesel direnç: gerçek taktik konuşlanma fixture'ı

6 Eylül 2026. Yalnız yeni `tools/resistance-deployments.script` ve bu not yazıldı; Assets, mevcut script'ler veya eski kanıtlar değiştirilmedi. Bu ajan test, derleme veya oyuncu başlatmadı.1114/1234/1106 root'un verdiği yeni Core sözleşmesidir; henüz bu fixture'ın gerçekleşmiş sonucu değildir.

Dört bağımsız `new`:

| Yeni başlangıçtan sonraki public hazırlık | Oyuncu / düşman |
| --- | --- |
| Champagne'ı seç |1200 /1114 |
| Île normal recruit, commission grant, commission recruit; Champagne'ı seç |1600 /1114 |
| Champagne tax |1200 /1234 |
| Champagne tax, bread |1200 /1106 |

Her yol gerçek `march` → active bekle → pause → gerçek `BattleOurTroops` / `BattleEnemyTroops` assertion'ı → battle snapshot ve bir tam PNG üretir. Ayrı hareket, hızlandırma, sağlık/cephane yazma veya sonuç enjeksiyonu yoktur. Kullanılan `retreat` ve `accept`, AutoShots'ın mevcut üst düzey komutlarıdır; ortak TacticalBattle emir API'sine giderler. `battle retreat` adlı yeni, desteklenmeyen alt komut uydurulmadı.

Her vaka üç JSON üretir: deployment (march öncesi campaign klonu dahil), gerçek retreat report (gözlenen kayıp ve dönüş morali), kabul edilmiş campaign return. Toplam4 PNG /12 JSON. Böylece toplam düşman kuvveti için dört alayın `Original` alanları, gerçek kalanlar ve nihai rapor ayrı ayrı okunabilir. İlk ve ikinci deployment aynı Champagne'ın oyuncu sayısı değişince düşmanı büyütmediğini; üçüncü/dördüncü gerçek yerel politika değişiminin taktiğe taşındığını göstermelidir.

Her geri çekilme `Ended=true`, `HasOutcome=true`, `Won=false` gerektirir. Kayıp veya geri dönüş Troops miktarı sabitlenmez. Root'un genişlettiği `battle verify-return`, başlangıç klonundan Core direncini/düşman Original toplamını ve gözlenen sonuçtan Troops, erzak, hareket, teçhizat, moral, tek battle ID ve geçmişi denetler. Böylece kaynakların fiyatı eski kayıp örneklerine göre yeniden yazılmaz. Her dönüşte ordu Île'de, Week0, ResolvedBattleCount1, PendingVictory yoktur. Takviyeli vaka commission/used hakkının da kaybolmadığını denetler.

Bu bir zafer veya taktik denge kabulü değildir; konuşlanma sözleşmesi ve gerçek retreat/accept taşınması incelemesidir. İlk gerçek runtime sonucunu ve karelerin okunurluğunu root ayrı değerlendirecek.

## Tamamlanmış gerçek deployment denetimi

`resistance-deployments-20260906-043417-446-99c65d8d`: PARTIAL/native0,14 saniye,135 komut/79 assertion/4 PNG/12 JSON. Yeniden kullanılan build `regional-resistance-first-20260906-042905-276-5f3716da`; rapor141 dosyanın değişmediğini bildirir. Bu kaynak inceleme görevinde yeniden test/derleme/oyuncu başlatılmadı ve eski artifact değiştirilmedi.

Gerçek deployment JSON'larındaki dört düşman alayının Original değerleri:

| Yol | Oyuncu Original toplamı | Düşman Original listesi = toplam | Gözlenen retreat → dönen asker |
| --- | --- | --- | --- |
| Başlangıç |1200 |356+311+245+202=1114 |42 →1158 |
| Takviye |1600 |356+311+245+202=1114 |56 →1544 |
| Tax |1200 |394+345+271+224=1234 |42 →1158 |
| Tax+Bread |1200 |353+309+243+201=1106 |42 →1158 |

Her toplam snapshot EnemyOriginalTroops ve yeni script expectation ile aynı. İki farklı oyuncu mevcudunda aynı bölgenin dört düşman alayı dahi aynı kalmış. Tax öncesi U69/C60.5/Elite60, tax sonrasında81/60.5/56, bread sonrasında66/62.5/56 değerleri CampaignBefore klonlarında görüldü.

Dört `PASS battle return` satırı enemy1114/1114/1234/1106 ve gözlenen42/56/42/42 kayıpları ayrı kaydeder. Source'daki kontrol gerçek klondan Core direnci ile rapordaki EnemyOriginalTroops ve düşman Original toplamını karşılaştırır; sonra canlı march preview ve kabul edilen sonuçla bütün dönüş alanlarını sınar.

Raw JSON'da dört Troops farkı, Won=false, gözlenen CampaignReturnMorale ile tam aynı dönen moral, Île ordu konumu, tek doğru `battle-0-2-ile-champagne` kimliği ve boş PendingVictory ayrıca doğrulandı. Gold840/600/940/940 aynı kaldı; Food342/298/342/302, Supplies115/85/115/115, Moves0. Bunlar başlangıç stoklarından1200 veya1600 kişinin gerçek difficult yürüyüş18/22 gıda ve5 teçhizat bedelidir; retreat recovered0. Dönüş moralleri sırasıyla60.680008 /56.680000 /60.680008 /60.680008 ve raporla aynıdır.

Kanıt sınırlaması: BattleEvidence kaynakta Arrival alanı taşısa da `MarchPreview` Serializable olmadığı için mevcut JSON'da Arrival yoktur. Bu yüzden JSON.Arrival üzerinden ikinci bir bağımsız preview karşılaştırması yapılamaz; ilk okuma bu olmayan alanı null buldu. Yukarıdaki runtime `verify-return` canlı `battleArrival` ile geçti, ayrıca kaynak maliyetleri ve gerçek before/after stoklar tutarlıdır. Bu durum failed campaign comparison olarak sunulmaz veya eski JSON'a sonradan Arrival eklenmez. Root'a ileride yeni kanıtta düzeltilebilecek dar serialization eksikliği olarak bildirildi.
