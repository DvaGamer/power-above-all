# İlk rol kesiti — ayrıcalık ve verilen söz

Kullanıcı parti öncesi farklı roller; kişisel iktidar, ülke yönetimi ve savaşın birlikte işlemesini istedi. Buradaki üç rol, sayılar ve kurgusal görevler gece çalışmasının tasarım varsayımıdır. Fransa/haftalık mevcut harita bu kesitin zemini; nihai dünya, tarihsel kahraman veya kariyer sistemi kararı değildir.

Üç seçenek değerlendirildi: emirleri makama göre yasaklamak; aynı emirleri koruyup ayrıcalığı görünür yükümlülüğe bağlamak; yalnız başlangıç görevi/hedefini değiştirmek. İkinci yaklaşım seçildi: oyuncu yeni bir kriz çözme yolu kazanır ve aldığı yardım sonraki kararında karşısına çıkar. Eski bütün emirler, askerî toparlanma ve uzun kampanya devam eder.

## İlk deneme değerleri

| Rol | Şimdi | İki hafta sonra: tut / boz |
| --- | --- | --- |
| Saray yetkilisi `crown` | Kraliyet avansı: +120 hazine, kraliyet desteği −3. | −150 hazine, kraliyet desteği +5, Valcourt ilişkisi +4 / kraliyet desteği −12, ilişki −10, kişisel iktidar −6. |
| Meclis komiseri `assembly` | Tahıl sözüyle uzlaşma: seçili bölgede huzursuzluk −18, kontrol +6; Meclis desteği −3. | −40 gıda, Meclis desteği +5, Morel ilişkisi +4 / aynı bölgede huzursuzluk +18, kontrol −6; Meclis desteği −10, ilişki −10, kişisel iktidar −4. |
| Ordu temsilcisi `army` | Yalnız ordunun bulunduğu bölgede zor alım: +40 gıda, +15 malzeme, yerel huzursuzluk +8, elit sadakati −6. | −80 hazine, aynı bölgede huzursuzluk −5, elit sadakati +4, Dumas ilişkisi +4 / huzursuzluk +12, elit sadakati −8, Dumas ilişkisi −6, kişisel iktidar −5. |

Bir açık yükümlülük sınırı; aynı rol ayrıcalığı dört haftada bir. Başlatmak için kişisel iktidar en az10. Ordu yokken zor alım yapılamaz; normal asker toplama devam eder. Tutma/bozma bütün sonuçları ilk emirden önce açıkça görünür. Yerel etkiler güncel duruma eklenir, bölgenin geçmiş görüntüsü geri yüklenmez. Verilen söz ordu veya seçili bölge değişse de ilk bölgeye aittir.

## Akış ve kayıt sözleşmesi

- Yeni oyuncu başlangıç masasında üç rolü karşılaştırır; başlatmadan mevcut sefer/saklı kayıt değişmez. Eski kayıt mevcut yetkileri koruyan `legacy` olarak açılır. Yeni butonu rol seçimine gider; iptal eski seferi sürdürür.
- `CampaignCore.Create()` eski testler ve eski seferler için legacy davranışını korur. `Create(roleId)` yalnız tanımlı kimlikleri kabul eder.
- `CampaignState`: `RoleId`, `NextMandateWeek`, arşivde 0–1 öğeli `Mandates` listesi; çekirdekte `Obligation` özelliği. Yükümlülük tür, bölge, veriliş/vade haftası, GoldDue/FoodDue içerir. Kararlı işlem kimliği tür+hafta+bölgedir. Standart DataContractJsonSerializer eski eksik alanları taşırken bozuk yeni arşivi reddeder; Unity JsonUtility'nin null alanı boş nesneye dönüştürme davranışına güvenilmez.
- `CanIssueMandate`/`IssueMandate`, `GetMandateTerms`; `CanResolveMandate`/`ResolveMandate(expectedId, fulfil|break)` aynı kuralları kullanır. Reddedilen eylem hiçbir durumu veya günlüğü değiştirmez.
- Vade gelmeden erken ödeme mümkündür. Vade geldiğinde oyuncu ödeme veya açıkça sözünden dönme kararını verir; bu karar çözülmeden sonraki hafta ilerlemez. Aynı anda ekmek dilekçesi varsa önce dilekçe görünür. Haftalık ekonomi sözde ödemeyi otomatik yazmaz; tutar/vade ayrıca gösterilir.
- Eski v1 arşiv doğrulanarak legacy/boş yükümlülük olarak açılır. Yeni yazı v2; bozuk v2 sessizce eskiye çevrilmez. İnsan kaydı test amacıyla kullanılmaz.
- Kampanya sekizinci haftada kapanmaz. Sonuçlar politik destek, ilişki, kişisel iktidar, bölgesel denetim ve gerçek ikmal üzerinden yaşar.

## Kabul

Üç rolün seçimi RU/TR okunur; ayrıcalık önizlemesi gerçek uygulamayla aynı; yetersiz kaynak/konum/tekrar atomik ret; vade ve ekmek dilekçesi sırası; kayıt/yükleme; Meclis sözüyle önce savaşsız sonra bozunca çatışmalı Champagne; ordu başka bölgeye gitse bile ilk bölgeye tazminat;200hafta ve sıfırordudan toparlanma eski testleri. Özgün sonuç hikâyesi bir rol seçmenin gerçek oyun farkını göstermelidir.

**Doğrulandı, 5 Eylül 23:03 UTC:** `roles-visible-20260905-230302-558-1717bcb0` tam GREEN, 56 Unity testi ve 22 gerçek kare / 29 durum kontrolü / 3 JSON. Eski atlas-savaş rotası da `roles-base-regression-20260905-230755-557-e852bd7c` ile GREEN. Bu kabul üç rolün ilk söz kesitidir; yeni avans için güven koşulu henüz yoktur ve denge incelemesinde açık olarak kaydedilmiştir.
