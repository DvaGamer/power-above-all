# Sonraki küçük siyasi seçim — bölgesel vergi uzlaşması

**Root'un onayladığı uygulama sözleşmesi; henüz tamamlanmış özellik değildir.** İncelenen checkpoint `0b108de`. Aşağıdaki−10/+3 profili, vergi tatili ve v3 arşiv kararı önceki−18/+6/isteğe bağlı v2 uzatma önerisini değiştirir. Kaynaklar: `CampaignCore.Create/Forecast/Act/CanMarch/TravelProjection/NextWeek/ResolveBattle`, `GameApp.March`, `CampaignPatronTrust`, mevcut konsey görünümü ve `history-night.md`. Aşağıdaki miktarlar oyun tasarımıdır; tarihî olay veya tarihî bütçe iddiası eklenmedi.

## Bugünkü kişiler gerçekte ne yapıyor?

- `Competence`: yalnız Dumas'nın değeri78, `GameApp.March` üzerinden `BattleSetup.CommanderCompetence` olur; gerçek savaşta moral toparlanması ve gelen moral şoku buna bağlıdır. Diğer üç kişinin65 değeri bu işte kullanılmaz.
- `Loyalty`: başlangıçta kişinin kurum desteğinden ayrı bir kez atanır. Sıkışan haftalarda Dumas'nın sadakati5 azalır; şimdilik emir, üretim veya savaşa etkisi yoktur.
- `Ambition`: Dumas80, diğerleri55 başlar. Zafer Dumas'ya +3 verir; şu anda kararları veya Güç kaybını tetiklemez.
- `Relationship` yeni hami yardımı/telafisi ve mevcut ekmek, sübvansiyon, savaş sonuçlarıyla çalışır. Konsey kartlarında görünür. Yeni seçimi anlamlı göstermek için kullanılmayan sadakat/hırs değerlerine gizli rastgele isyan yüzdeleri eklememeliyiz.

## Üç ayrı kavram

| Kavram | Oyuncunun gerçek seçimi | Mevcut koda bağ | Bu dilimde bedeli |
| --- | --- | --- | --- |
| Subay atamaları | Daha iyi saha komutası karşılığında kişisel yetkiyi bir subaya bırakmak; daha bağımlı bir adayı seçmek | Yetkinlik zaten savaşta çalışır; sadakat/hırs ileride açık yetki pazarlığı olabilir | Mevcut dört kişi dört siyasi kurumun lideridir, dört hazır subay adayı değildir. Diğerlerinin65 değerini kullanıp Lefèvre'yi general yapmak sığ bir atama olur. Dürüst küçük kadro, komutan kimliği ve ekonomik yetki sözleşmesi ister. Sonraki karakter dilimi için güçlü, şu an ek kapsam. |
| Kent–ordu tahıl konvoyu | Aynı gıdayı halk dağıtımı veya korunan sefer rezervi yapmak | Gıda, ekmek emri, Supply, yürüyüş maliyeti, kent desteği hazır | Şimdiki taktik konvoy ödülü **24 askerî malzeme**, tahıl değil. Gıda diye yeniden adlandırılamaz. Ayrı ordu gıda rezervi eklenirse `NextWeek` içindeki tek açlık bayrağını da ayırmak gerekir; aksi hâlde korunmuş erzak varken asker yine sivil açlığından kaybolur. İyi devam sistemi, fakat bu küçük seçimden daha geniş ikmal değişikliği. |
| **Bölgesel vergi uzlaşması** | Geliri bir süre bırakıp direnci düşürmek veya bugünkü vergiyi isteyerek yeniden direnişi göze almak | Vergi zaten huzursuzluk/kontrolden gelir; huzursuzluk65'te yürüyüş muharebeye dönüşür; mevcut vergi emri söz bozabilir | En küçük yeni kalıcı durum; yeni savaş türü, kaynak havuzu, rol veya zorunlu vade ekranı gerekmez. **Öneri.** |

## Önerilen oynanabilir sözleşme

Konseyde Morel seçili bölge için **“Dört haftalık vergi tatili” / «Налоговые каникулы»** önerir. Vazgeçilen gelir sonradan tahsil edilmez; borç veya erteleme yoktur. Valcourt gelir kaybını, Dumas ordunun geçiş imkânını aynı kısa metinde açıklar. Oyuncu anlaşmayı imzalar veya mevcut vergi/ordu emirleriyle devam eder. Yeni panel, otomatik açılan modal veya zorunlu cevap yoktur.

1. Bir seferde en fazla bir bölge. Bütün başlangıç rolleri kullanabilir. Gold/Power/yaşayan ordu eşiği gerekmez; yoksul yönetim de gelecekteki gelirinden vazgeçebilir. Mevcut dilekçe/vadesi gelmiş söz önceliği korunur; uzlaşmanın kendisi haftayı durdurmaz.
2. İmzada bölge huzursuzluğu−10, kontrol+3; stok veya kurum desteği anında değişmez. Karşılığında bu bölgenin **sonraki dört haftalık olağan vergi katkısı0** olur. Forecast'te bölge katkısı toplamdan önce çıkarılır; toplam gelirdeki gerçek yuvarlama önizlemede kullanılır. Fiyat `BaseTax` veya dört kez bugünkü tahmin diye sabitlenmez: diğer siyasi/yerel değişiklikler ekonomide yaşamaya devam eder.
3. Dördüncü haftalık hesap kapandığında vergi tatili kendiliğinden biter. Normal vergi katkısı sonraki haftaya döner; Morel ilişkisi+4 ve meclis desteği+5 ile tutulan söz kayda geçer. Sonuç tek kez uygulanır; oyuncudan ikinci ödeme/seçim istenmez. Örneğin0. haftada imza:0→1,1→2,2→3,3→4 hesaplarında istisna;4. haftada aktif kimlik temizlenir. Hiçbir eski gelir topluca geri alınmaz.
4. Oyuncu tatildeki bölgeye olağanüstü `tax` emrini **yine verebilir**. Düğmenin önizlemesi anlaşmayı bozduğunu gösterir. Başarılı vergi, nominal karşı tepkiyi (+10 huzursuzluk/−3 kontrol) ve Morel−10/meclis−10/Güç−4 bedelini uygular, ardından mevcut verginin +100 altın/+12 huzursuzluk/−4 elit sadakati ve siyasi etkileri gelir. İmza tekrarına izin verilen tarih değişmez; o hafta yeni bölgeye anlaşma açıp bedelsiz sakinleştirme yoktur. Yerel etkiler mevcut0–100 sınırlarına göre uygulanır; geçmiş bir bölge görüntüsünü geri yüklemez.
5. Vergi tatili bir askerî dokunulmazlık değildir. Asker alma, ekmek, hareket ve savaş mevcut kurallarıyla sürer. Kıtlık veya kötü yönetim huzursuzluğu yeniden65'e çıkarırsa bölge söz devam ederken de direnir. Harita ve yürüyüş önizlemesi hep **gerçek güncel** durumu gösterir.

Meclis rolüyle fark: onun−18/+6 müdahalesi daha güçlüdür, olağan vergi gelirini korur ve40 gıdayla hemen veya iki haftada yerine getirilebilir; başlangıç meclis−3/yerine getirme+5 neti+2'dir. Genel uzlaşmanın−10/+3 etkisi daha kırılgandır; +5 desteği ancak dört vergi hesabı gerçekten geçince kazanır. Champagne için59'a karşı51, huzursuzluğun yeniden65'e ulaşmasına kalan farklı güvenlik payıdır. Ortak siyasi ödül oyuncuya Morel'in aynı tutulmuş sözü tanıdığını anlatır; iki yolun zaman, kaynak ve dayanıklılık profili aynı değildir.

Bu paket sadakat/hırsın henüz çalışan bir davranış olmadığını değiştirmez. Onları şimdi yeni dolaylı çarpanlara dönüştürmek yerine oyuncunun gerçek emriyle Morel'in güveni, meclisin vergi etkisi ve ordunun karşılaşacağı direnç arasında açık zincir kurar. Subay ataması bu alanlara ayrı, görünür bir yetki kararı verdiğinde eklenmelidir.

## Somut bir ilk hikâye

Başlangıçta Champagne huzursuzluğu69, kontrolü60,5 ve Paris'e komşudur. Vergi tatili59/63,5 yapar: `CanMarch` muharebe yerine barışçıl yürüyüş verir. Yol hâlâ huzursuzluk50 eşiğinin üzerinde olduğu için zordur:1200 askerle18 gıda, iki hareket ve mevcut zor yol ikmal/yorgunluk bedeli kalır. Oyuncu askerlerini korur; o bölgede dört gelir hesabını bırakır, muharebe/konvoy ödülü kazanmaz.

Üçüncü haftada hazine sıkışırsa oyuncu vergiyi seçebilir. Bölgenin o anki huzursuzluğu yeniden artar; asker ileride tekrar buraya dönerse savaş çıkabilir. Sözü sonuna kadar tutarsa Morel ve meclis güçlenir; bu destek mevcut ulusal vergi hesabına da sonraki haftadan yansır. Aynı anlaşmanın imzası, bütçesi, ordu yolu ve daha sonraki siyasi sonucu görülebilir.

## En küçük veri/API sınırı

- Arşiv **version3**; iki zorunlu düz wire alanı: `AccordRegionId` ve `AccordUntilWeek`. Canonical yokluk boş string/0, null string geçersizdir. Aktif kimlik iptal/bitimde boşaltılır; tarih erken bozma sonrasında da aynı dört haftalık tekrar sınırını korur. Bu nedenle boş string/gelecek tarih geçerli beklemedir. Aktif kimlik geçerli bölge olmalı; `until−4 <= Week < until` ve `until <= MaximumWeek` gerekir.
- Saf domain'de iki `[OptionalField]` eski sürümleri okumayı sağlar. Archive v3'te aynı DCS serileştiricisinin küçük, türü belirli `Envelope.State` projeksiyonu iki `[DataMember(IsRequired=true)]` alanının varlığını doğrular; eksik alan/null/yanlış tür geçmez. Tam mevcut state doğrulaması ayrıca çalışır. Böylece JsonUtility yokluk varsayımı, regex veya bütün seferi çoğaltan DTO gerekmez.
- v1→mevcut legacy geçişi, v2→mevcut rol doğrulaması aynen korunur; sonra yeni alanlar açıkça boş/0 olur. v1/v2 içinde boş olmayan yeni bölge veya sıfır olmayan yeni tarih reddedilir; sürüm düşürülmüş aktif anlaşma sessizce silinmez. Eski gerçek kayıtta iki alanın yokluğu normaldir. Yeni v3 kayıtta yokluk normal değildir.
- API: `GetRegionalAccordTerms(state, regionId)` yeni teklif; `GetActiveRegionalAccordTerms(state)` asıl imzalı bölge; `CanGrantRegionalAccord/GrantRegionalAccord`, `HasRegionalAccord`, `TaxBreaksRegionalAccord(state, regionId)`. `RegionalAccordTerms`: `RegionId`, `UntilWeek`, `RemainingWeeks`, `IsActive`, `CurrentTaxIncome`, `ProjectedTaxIncome`, `TaxForgone`, `Immediate/Fulfil/Break` effect nesneleri. Aktif anlaşma seçili bölgeyi izlemez.
- `CurrentTaxIncome` bugünkü Forecast, `ProjectedTaxIncome` imzadan sonraki Forecast (aktifte bugünküyle aynı). `TaxForgone`, aynı projeksiyon/güncel yerel durum ve aynı meclis desteğinde istisnalı/istisnasız **iki gerçek toplam vergi** arasındaki farktır; ilk sakinleşmenin gelir değişimiyle karıştırılmaz. Kalan vergi hesabı `until−Week`; dört haftalık sabit bedel gösterilmez.
- `Forecast`, `Act(tax)`, `NextWeek` üç bağlantı noktasıdır. Verginin bütün mevcut ret koşulları **önce** kontrol edilir; kullanılmış emir veya dolu hazinede reddedilen vergi anlaşmayı bozamaz. Hafta sonu sürenin bitişi, o haftanın eski anlaşmalı vergi hesabından **sonra** uygulanır.
- Bu belge yeni bir kullanıcının tarih/rol seçimi sayılmaz. Miktarlar dört haftalık gerçek sefer karşılaştırmasında ölçülmeden kesin denge sayılmaz.

## Kabul akışları

1. Varsayılan69 huzursuzluklu Champagne: önce muharebe gerekir; imzadan sonra barışçıl ama zor yürüyüş, doğru18 gıda/iki hareket. Sadece seçili bölgenin vergisi dört hesap boyunca çıkar; sürenin sonunda geri gelir ve teşekkür yalnız bir kez yazılır.
2. İmzadan sonra archive turu ve üç hafta ilerleme; dördüncü hesabın gelir kaybı korunur. Süre sonunda veya erken bozma sonrasında save/load ücretsiz ek telafi/ikinci anlaşma üretmez.
3. Bozacak vergi önizlemesi gerçek sonuçla eşittir. Aynı haftada ikinci vergi/yeniden anlaşma reddi bütçe, kişi, tarih ve günlük dahil atomiktir. Gold kapasitesi yüzünden reddedilen ilk vergi de anlaşmaya dokunmaz.
4. Açık rol sözünün bölgesi/miktarı/vadesi değişmez; aynı bölgede iki farklı taahhüdün önizlemeleri karışmaz. Gıda0/Güç0/ordu0 durumunda uzlaşma teklif edilebilir, hafta ilerleme veya normal toparlanma yeni bir konsey kilidine girmez.
5. On iki haftalık karşılaştırma: mevcut savaş yolu, dört haftalık vergi tatili ve erken bozma; hazine, gıda, asker kaybı, gerçek hareket imkânı, Morel/meclis ve huzursuzluk izlenir. Gelir kaybının sonradan artan meclis desteğiyle telafisi stratejik kazanç olabilir; aynı haftada sınırsız döngü olmamalıdır.
