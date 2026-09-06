# Bağlı reform rotası: ölçüm ve player senaryosu

Kaynak: `output/core-probes/RegionalReformProbe-2026-09-06T06-03-24-264Z-d67c9e3f/probe.stdout.log`. Root çalıştırması **PASS227**, saf Core; player kanıtı olarak sunulmaz. İlk ayrı UI gate'i root **496 test /18 PNG/72 assertion/14 state/native0** olarak bildirdi; aşağıdaki bağlı script henüz çalıştırılmadı.

`tools/regional-reform-linked.script` iki yeni gerçek legacy kampanyasını tekrarlar; state veya savaş sonucu enjeksiyonu yoktur. Tam16 PNG sınırı: Champagne blocked belgesi RU/TR üst-alt, toparlanma, aktif muafiyet ve7. hafta; Dumas'ın31/35 karşılaştırması RU/TR üst-alt, gerçek toplama koşulları,9. hafta sonuçları ve iptal sonrası belge. Her dil değişiminde scroll yeniden ayarlanır. Hatırlanan kayıtlar UI gezinmesinin ekonomik state'i değiştirmediğini ve save/load eşitliğini sınar.

## Çalışmış Core sonuçları

**Champagne:** 120 Gold/4 Power başlangıcı, iki başarılı ama uygun olmayan hafta: U69→73, C60.5→54.5, dört adım hâlâ durur. Negotiate ardından40 Food bread U58/C56.5 yapar;3. hafta bir adım kazanılır. O hafta verilen accord U50/C59.5, until7; kalan adımlar4/5/6. hafta sonunda tamamlanır. Week6 Gold1138/Food313, proje aktif ama ilk yeni bütçe henüz uygulanmamıştır. Week7 gerçekleşen tax196/production147/netFood−3; Gold1198/Food310. Bu aynı zamanda dördüncü muaf hesaptır; accord sonra kapanır. Bundan sonraki gerçek Forecast tax208; reformun kendi katkısı muafiyetsiz211→208, production142→145 olarak görünür.

**Dumas:** Paris desteği, normal alım0/2/4/6, week2 relief; officer commission week0'dan açık, ek grup kullanılmaz. Normandy projesiweek4'te ödenir; dört eski bütçe sürer. Week8'de önceki gerçek açlık2000 kişiden160 kaybettirir; proje aynı haftanın sonunda etkinleşir. Orleans accord verilince aynı kayıt: Gold645/Food0/1840 asker/Power49.5, reformaktif, Dumasdue9/cooldown12, accorduntil12, commissionaçık. Proje karşılaştırması tax211→205, production157→161, forage35→31, **NetFood0→0**. Fazladan üretim bu koşulda depoya4 birim eklemez, gereken toplamayı4 azaltır.

Gerçek week9 hesapta tax205/armyCost190/production161/forage31/NetFood0: Gold660/Food0/1840 asker/Power46. Dumasdue kapanır, cooldown12 ve accord/commission kalır. Player script bu hesabı gerçekleştirdikten sonra projeyi iptal eder; Gold660/Power46 değişmemeli, eski bağımsız haklar ve tarihler save/load içinde kalmalıdır. İptalin gerçek sponsor ilişki farkı state/journal üzerinden okunur.

AutoShots mevcut `expect` alanları TaxIncome ve ForageFood'u destekler. Production ve NetFood için yeni protocol alanı eklenmedi: bunlar state JSON'daki Economy/Reform terms ve gerçek `log.week` kaydı üzerinden root tarafından karşılaştırılacaktır. Özellikle week7 sonrası Forecast208, gerçekleşmiş week7 tax196 ile aynı tarih sanılmamalıdır. Bu rotalar uzun dönem dengeyi veya savaş kazanma olasılığını kanıtlamaz.
