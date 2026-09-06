# Dumas subay yetkisi: kaynak incelemesi ve gerçek rota fixture'ı

6 Eylül 2026. Root yeni Core için Runtime27/Editor3 statik PASS ve `OfficerCommissionProbe-2026-09-06T03-25-54-113Z-8f54535b` PASS52 bildirdi. Probe stdout ve kaynak rotası doğrudan okundu; `CampaignOfficerCommission`, Core recruit/NextWeek hooks, Archive v7 ve `CabinetOfficerCommission` salt okunur incelendi. Bu ajan Unity, test, probe, player veya derleyici başlatmadı. Edits yalnız yeni player script'i ve bu belgedir.

## Dar kaynak incelemesi

- Ortak `CheckRecruitment` eski sırayı koruyor: normal kullanılan bölge → mevcut kamp → Gold/Food/Supplies/Manpower yeterliliği → asker kapasitesi. `ApplyRecruitment` aynı120 altın/20 gıda/15 malzeme/200 insan gücünü öder;200 asker, yerel huzursuzluk+2/moral−2/army approval+2 ve eski reduction Refresh vardır. Eski normal recruit kendi başına Dumas sadakati veya extra-used hakkı vermez.
- Extra recruit önce tüm sefer, dilekçe, dueMandate ve takvim guard'larından geçer; aktif yetki, henüz kullanılmamış global hak ve **şimdiki kampın** normal recruit işareti gerekir. Ödeme ve gerçek capped Loyalty+1 ancak kabul edilen grupta uygulanır. Grant yalnız yetkiyi açar, ücretsiz sadakat vermez.
- DumasExtraRecruitUsed bölgeye bağlı ikinci hak değildir. Revoke veya regrant onu temizlemez. Başka kampın normal recruit'i hâlâ kullanılabilir, ama ikinci Dumas grubu aynı haftada açılamaz. Başarılı NextWeek bölgesel kullanılan bayraklarla birlikte bu global flag'i sıfırlar; önceki guard retleri reset yapamaz. Used=true/active=false geri alınmış ama bu hafta harcanmış hakkın geçerli arşiv durumudur.
- Bütçe politikasını kabul etme guard'ı aktif yetkiyi reddeder; Validate da active commission + budget birlikteliğini reddeder. Revoke fiyatı o anda yaşayan Troops için ceil(Troops/12); imza anındaki eski mevcuttan kalıcı fiyat saklanmaz. Revoke askeri geri getirmez veya used'i yenilemez.
- Archive v7 iki typed zorunlu bool alanı ayrı sözleşmeyle okur. V7 öncesi sürüm numarası altında true alanı saklanamaz; eski false/yok alanlar açıkça false'a taşınır. Önceki accord/victory/Dumas/establishment eşikleri bağımsız kalmış. Null/missing bool ve karmaşık eşzamanlı kayıtların gerçek çalışma zamanı kabulü root testlerini bekler; salt kaynak incelemesi bunları çalışmış saymaz.
- UI üç ayrı Can* sonucunu kullanıyor; aynı sayısal teklif kabul yetkisi gibi tek başına kullanılmıyor. Normal kamp grubu gereği, bir haftalık kullanım ve değişen geri alma bedeli aktif bölümde gösteriliyor. İncelenen kaynakta somut gameplay/atomiklik kusuru bulunmadı; bu sonuç bütün UI girdileri veya yeni tam gate için genel kabul değildir.

## Public probe'da ölçülen yollar

| Yol | Asker / rezerv | Gold / Food / Supplies | Hareket / yorgunluk | Konum ve hak |
| --- | --- | --- | --- | --- |
| Normal recruitÎle → yürüyüş → normal recruitNormandy |1600 /2000 |600 /306 /85 |1 /10 | Normandy; yetki yok |
| Grant → normal recruit → tek extra recruit |1600 /2000 |600 /320 /90 |2 /0 | Île; aktif, used=true; Loyalty61 |
|134 öde/regrant → Normandy'ye yürü → normal recruit |1800 /1800 |346 /284 /70 |1 /10 | Aktif, used=true; yeni revoke fiyatı150 |
|150 öde → budget1400 → dört gerçek hesap |1400 /2200 |318 /214 /84 |2 /0 | Normandy; aktif/used=false; Loyalty61 |

İlk iki yol aynı1600 kişiye varır ama aynı coğrafi sonuç değildir: normal yol Normandy'ye gitmiştir. Tek kamp yolu14 gıda/5 malzeme ve bir hareket/10 yorgunluğu harcamaz; karşılığında budget kararı için ücretli geri alınması gereken yetki vardır. Bu rakamlar bir yolun koşulsuz daha iyi olduğu iddiasına dönüştürülmez.

Ücretsiz grant sırasında Loyalty60 kalır. İlk ödenen extra grup61 yapar; normal recruit ve revoke/regrant bunu tekrar artırmaz.134 ve150 iki gerçek ayrı revoke ödemesidir. Son inactive1400 kişilik durumda117 yalnız o mevcut için teklif hesabıdır; üçüncü117 ödeme yapılmış veya açık borç oluşmuş sayılmaz. Dumas Loyalty ve demobilizasyonun etkilediği Relationship ayrı alanlardır.

## Player fixture freeze

`tools/officer-commission.script`:225 komut,86expect+11same=97 assertion,19PNG/13JSON; ilk `new`, son `quit`. Önce normal iki bölge yolu, sonra yeni army kampanyasında ölçülmüş bütün yetki yolu vardır. RU/TR offer üst/alt; aktif hak ve134 geri alma bedeli üst/alt;1400 normal gruptan sonraki ek işe alım olanağı; yeniden imza/yolculuk sonrasında150 bedeli; budget engeli, ikinci ücretli geri alma, haftalık reset, dilekçe önceliği ve dört haftalık azaltım gösterilir.

İkinci grant, normal grup olmadan extra, aktif yetkiyle budget, used hakkın yeniden imza/başka kampla tekrar kullanımı ve dilekçe/policy altındaki yeni grant retleri bütün state `same` ile korunur. Ücretli grup ve global kullanım ile son asker azaltımı save/load tekrarından geçer. Scroll650/1300 yalnız ilk görüntü konumudur; root gerçek PNG incelemesinden sonra ayarlayabilir. Yeni player veya parser sonucu henüz bu ajan tarafından elde edilmedi.
