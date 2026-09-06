# Ordu politikaları:24 hesaplık karşılaştırma hazırlığı

Checkpoint777dfec sonrası root'un verdiği dar bağımsız görev. `WorkNotes/ArmyPolicyComparisonProbe.cs` hazırlandı; bu ajan hiçbir test, probe, Unity, player veya derleme başlatmadı. Yeni sayısal sonuç yoktur. Kaynak ve bu belge dışında dosya değiştirilmedi.

## Eşit koşullar

Altı senaryo tam aynı `CampaignCore.Create()` legacy state ve günlüğünden başlar; tam serialize eşitliği bunu kontrol eder.24 **başarılı** hafta tamamlanır. Mevcut dilekçeye her seferde negotiate cevabı verilir. Paris yardımı başlangıçtaki kapalı hâlinde kalır; yeni vergi/emir/borç/anlaşma, yürüyüş veya yapay savaş eklenmez.

| Senaryo | Politika | Diğer planlı emir |
| --- | --- | --- |
|campaign | Eski sefer serbestisi | Yok |
|budget1000 |1000 hedef | Yok |
|budget600 |600 hedef | Yok |
|budget0 |0 hedef | Yok |
|campaign-recruit024 | Sefer serbestisi |0/2/4. haftalarda olağan recruit denemesi |
|budget1000-recruit024 |1000 hedef |0/2/4. haftalarda olağan recruit denemesi |

Core'da ayrı bir CanAct yoktur. Normal `Act(recruit, ArmyRegionId)` kullanılır; ret varsa kampanyanın tamamı aynı kalmalı, açık ret nedeni ve sıfır ödenen kaynak stdout'a yazılmalıdır. Ret diğer haftaları gizlice atlatmaz veya kaynak eklemez. Kabul edilen recruit'lerin gerçek Gold/Food/MilitarySupplies farkı toplam masraf olarak kaydedilir.

## Kayıtlar ve kabul sınırı

`ROW`0/6/12/24: Gold,Food,Troops,Manpower,ortalama huzursuzluk,Power,Dumas ilişkisi; ayrıca malzeme/ikmal/moral, geçerli hedef ve vade, gelecek hesap vergi/ordu/gıda dengesi, Dumas gelecekteki planı.0. hafta satırı politika ve varsa ilk recruit emrinden **sonra**, ilk hesap öncesidir. Ortak ham başlangıç ayrıca bütün senaryolarda tam eşitlikle doğrulanır;0. hafta farklı sayılar eşit olmayan başlangıç sanılmamalıdır.

`ACTION` gerçek recruit/negotiate sonucudur. `EVENT` yalnız gerçekleşmiş haftanın shortage veya gathered kaydını yazar. `nextForage`, gelecek hesap önerisidir; `actualGatheredFood` ve `actualForageWeeks` gerçekten uygulanmış kayıtların toplamıdır. Gıda0 tek başına açlık kabul edilmez: mevcut stok+ortak NetFood sonucunun negatif olması ve gerçek shortage kaydı karşılaştırılır. Sivil açlığın0 askerle de mümkün olduğu durumda asker kaybı0 olarak doğru kalır.

Her haftada salt okunur Forecast'in state'i değiştirmediği, gerçek Gold/Food stokları ve `log.week` değerleri ile eşitliği doğrulanır. Mevcut hafta günlüklerinden bildirilen kayıplar bağımsız olarak `Troops+Manpower` toplamı ile karşılaştırılır; recruit/azaltma insan üretip silemez. Policy'nin yalnız gerçek reduced kayıtları sayılır. Sonuçta24 başarılı hesap, başlangıç3600 kişi eksi gerçek raporlanan kayıplar ve public Archive serialize→deserialize→serialize tam eşitliği kontrol edilir.

`SUMMARY`, kabul/ret sayıları, gerçek recruit maliyetleri, açlık/diğer shortage haftaları, kayıplar, gerçek toplama, NPC duyuruları, gerçekleşen azaltım partileri ve rezerv dönüşünü toplar. Altı `FINAL-ARCHIVE` satırı tam son durumları içerir; bunlar uzun olabilir. İlk incelemede yalnız ROW/ACTION/EVENT/SUMMARY/PASS satırları seçilmeli; tüm JSON metni bağlama dökülmemeli. Probe insan kaydına veya dosyalara yazmaz; root merkezi `run-core-probe` aracının ayrı çıktısı stdout'u ve kaynak hash'lerini saklar.

Bu karşılaştırma bir politikayı “en iyi” ilan etmez. Küçük ordu kasayı rahatlatırken sahra gücünü ve garnizonu azaltır; sonraki savaş, tekrar büyüme maliyeti, açık yeni NPC planı ve mevcut siyasi baskı ayrı değerlendirilmelidir. Yapay sabit muharebe kayıpları veya kazanma olasılıkları bu ekonomik kanıta eklenmez. Gelecek siyasi mekanizma seçimi ancak root'un gerçek çıktısını incelemesinden sonra tartışılacak.
