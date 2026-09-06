# Sonraki küçük bölgesel reform: üç seçenek

Durum: yalnız tasarım, uygulama izni değildir. Bölge, kişi ve sayılar mevcut kurmaca oyun modelidir; tarihsel kurum veya istatistik iddiası yoktur. Core/Assets/test değiştirilmedi, süreç çalıştırılmadı. Mevcut archive v7 ve root'un gerçek24 haftalık civic çıktısı esas alındı.

## Başlangıç problemi

Aktif legacy+budget1000 örneği bir bread, bir tatil ve24 Paris ödemesiyle0/12 düşman, Gold3981/Food234 üretti; aynı pasif bütçe11/12,2292/61. Bu sonucu sessizce geri almak için yeni genel huzursuzluk veya bakım cezası eklemeyelim. Oyuncunun kurduğu düzen gerçek kazanımı olarak kalmalı. Sonraki reform, bu düzenin **nasıl kullanılacağına** dair gönüllü bir fırsat maliyeti açmalı: daha çok para, asker besleme kapasitesi veya başkentten bağımsız yerel yönetim aynı şey değildir.

Tek birleşik civic deneyinden her durum için en iyi politika çıkarılmadı. Reform fikri de onu zorunlu olarak bozma veya bütün sakin bölgeleri tekrar isyan ettirme aracı değildir.

## A — Valcourt ile doğrudan bölge idaresi

Oyuncu yerel aracılar yerine kraliyete bağlı bir tahsilat/idare heyeti kurar. Bir kere kayıt ve personel bedeli öder; birkaç başarılı hesapta bölge Control'ü gelişir, böylece gerçek vergi tahsilatı ve askerî direniş hesabı değişir. Valcourt bir yetki kazanır; yerel elitler bunu çıkar kaybı olarak görür. Alternatif, aynı bölgeyi garnizonla tutup yeni bağımlılık yaratmamaktır.

Bedeli yalnız+Control karşılığı Gold olmamalı: Valcourt'un yetkisi sürerken yerel zorlayıcı kararları onun adına almak veya yetkiyi görünür bir bedelle geri almak gerekir. Burada yeni hakların hangi gerçek komutu değiştireceğini ve düşük EliteLoyalty'nin geri kazanılmasını ayrıca çözmek şart. Tek başına kalıcı elit cezası mevcut `<35 → Control−2` kuralıyla uzun çıkmaz yaratabilir. **Yararı büyük, ilk dar sürüm için siyasi hak ve recovery kapsamı fazla.**

## B — Morel veya Valcourt ile bir deney bölgesinin ekonomik uzmanlaşması

Oyuncu aynı bölgesel kapasitenin bir bölümünü iaşeye ya da ticarete yönlendirir. Morel, tahıl tedariki düzenlemesini; Valcourt, parasal gelir önceliğini temsil eder. İlki savaş ordusunu ve devamlı yardımı beslemeyi kolaylaştırırken vergiyi azaltır; ikincisi parayı artırırken gıda üretimini azaltır. Eski bread doğrudan mevcut40 Food harcar; eski tatil dört tahsilatı geçici kaldırır. Bu reform ise hazırlık sonrası **gelecek üretim/gelir bileşimini** kalıcı fakat geri alınabilir biçimde değiştirir.

Kontrolün önemi, hazırlığı gerçekten yürütebilmek ve sonrasındaki mevcut vergi verimidir. Bu sürüm ayrıca bedava kalıcı+Control vermemeli; aksi halde imzala/iptal et döngüsü ayrı bir gösterge toplama işine dönüşür. Halk desteği yüksek ülke projeyi güvenle kurabilir; hangi çıktıyı feda edeceğine yine karar vermek zorundadır.

**En kompakt önerim B.** Mevcut EconomyView/CalculateEconomy içinde tek yerel taban dönüşümü yeterlidir; gıda, vergi, ordu ve siyasi yetki arasında yeni seçim açar. Yeni bölgesel stok deposu, aktör AI'si veya ülke çapında yaptırım gerektirmez.

## C — Morel'in yerel meclisine sivil denetim yetkisi

Oyuncu garnizon yerine yerel temsilcilerle idare kurar. Hazırlıktan sonra ordu yokken Control'ün korunması/gelişmesi mümkün olur; karşılığında yerel asker alma veya olağanüstü emirler için temsilcilerin hakkını tanır. Sefer ordusunu serbest bırakır fakat oyuncunun aynı bölgede emir esnekliğini azaltır. Yetki geri alınabilir; askerler veya mevcut rezerv ücretsiz çoğalmaz.

Bu, bölgeyi elde tutmakla insanları kendine bağlamak arasındaki en güçlü siyasi seçenek olabilir. Fakat Core'un normal recruit sırasına, güncel subay hakkına ve ekonomik anlaşmaya yeni gönüllü yetki sınırları getirmek gerekir. Çok sayıda kilitlenmiş buton üretme riski var. **B'den sonraki sürüm için, açık komut/recovery sözleşmesi gerektirir.**

## B için önerilen küçük çalışma sözleşmesi

Rakamlar ilk tasarım varsayımıdır; burada probe yapılmadı veya dengeli oldukları ileri sürülmedi.

- Aynı anda bir deney bölgesi. Modlar `provisioning` / `commerce`; Morel / Valcourt. İmza120 Gold ve4 Power gerektirir; önceden ödenir, tamamlanınca tekrar alınmaz. Eski ihale, rol, ordu politikası ve sözleşmeler kendiliğinden kapanmaz.
- Hazırlık dört **uygun başarılı hafta hesabı** ister. İlgili bölgenin son durumunda Unrest<65 ve Control≥55 ise bir adım tamamlanır; değilse yalnız proje bekler. Bu, dört takvim haftası garantisi değildir. Başlangıçta bile koşul uygun değilse açık uyarıyla başlanabilir; kalıcı bir sonraki-hafta modalı yoktur. Başarısız NextWeek hiçbir adım tüketmez.
- Başlangıçtaki en erken sonuç hafta4 sonudur; yeni ekonomik bileşim ilk kez **beşinci bütçede** görünür. O dört hesabın eski üretim/vergi kuralları korunur. Erken bir çatışma, olağanüstü vergi veya levazım toplaması Unrest'i yükseltip hazırlığı durdurabilir; savaş veya bread/accord ile gerçek durum düzelince devam eder. Gizli rastgele sabote veya otomatik kayıp yoktur.
- Tanımdaki BaseTax ve BaseFood'un dörtte biri ayrı ayrı, mevcut AwayFromZero yöntemiyle yuvarlanır. Provisioning: etkin BaseTax bu vergi payı kadar azalır, etkin BaseFood gıda payı kadar artar. Commerce tersidir. Örneğin Normandy için değişim `−8 BaseTax/+5 BaseFood` veya tersi. Bunlar doğrudan cüzdan deltalari değildir; gerçek Unrest/Control/Assembly çarpanları ve ortak toplam yuvarlaması uygulanır.
- Değişmeyen `RegionDefinition` yerinde kalır. Dönüşüm yalnız bu bölgenin ekonomi görünümünde uygulanır. Direnişin örgütlenme tabanı hâlâ asıl BaseTax'tır: ticaret ruhsatını kapatarak düşman askerini anında küçültmek gibi yeni bir kestirme yoktur. Direniş yalnız gerçek U/C/E değişirse değişir.
- Tamamlandığında ilgili kişinin Relationship'i gerçek clamp ile+4; bu sponsor hakkının kurulmasını temsil eder ve mevcut crown/assembly patron erişimiyle bağlantılıdır. Yeni etkisiz Ambition sayacı eklenmez. Hak açıkça geri alınmadıkça ekonomik mod sürer; sponsor otomatik sadakat veya darbe hakkı kazanmaz.
- İptal/geri alma her aşamada mümkün: harcanmış120 Gold ve4 Power geri verilmez; ilgili kişinin Relationship'i gerçek clamp ile−8 düşer. İptal ek Gold/Power istemez, dolayısıyla0 asker/0 Power durumunda çıkış kapanmaz. Hazırlık/aktif mod temizlenir, sonraki hesap normal tabana döner; eski askerler, yiyecek ve kazanılmış diğer haklar değişmez. İptal geçmişteki üretimi geri toplamaz.
- Yeniden kurmak veya diğer moda geçmek önce açık geri alma, sonra yeni120/4 ve dört uygun hesap ister. İmza/iptal döngüsü bedava kaynak, Control veya ilişki üretmez. Proje tamamlandıktan sonra düşük huzursuzluğa otomatik küresel bonus eklenmez.

Bu fiyat kişisel bir tercih yaratır: yüksek sivil gelir her maliyeti önemsiz yapmaz;4 Power mevcut diğer yetkilerde kullanılabilir, sponsorla ilişkiyi geri çekmek sonraki patron erişimini etkileyebilir. İlişki0'a inse bile bugünkü trust-repair yolu korunur. Başlangıç parası/gücü olmayan oyuncu eski bread, tatil, maaş ve asker kararlarına devam edebilir; reform kampanya ilerlemesinin yeni şartı değildir.

## Minimal veri ve API

Öneri: global üç alan `ReformRegionId`, `ReformModeId`, `ReformStepsRemaining`. Boş/boş/0 = kapalı; geçerli bölge/mod ve1..4 = hazırlık; aynı bölge/mod ve0 = çalışıyor. Eski arşivlerin yorumunu değiştirmemek için sonraki açık sürüm v8'de bu üç alan zorunlu, v1–7 açıkça boş başlangıca göçer; eski sürüm içinde saklanmış etkin yeni alan reddedilir. Var olan3/4/5/6/7 sürüm eşikleri bağımsız korunur. Son takvim sınırında yeni proje en erken tamamlanma için yeterli hafta yoksa reddedilir; bekleyen projeden çıkış ayrıca mümkün kalır.

`GetRegionalReformTerms(state,region,mode)` salt okunur ücretleri, mevcut/önerilen nominal tabanları, gerçek bugün koşullu TaxIncome/Production farkını, sponsor ve gerçek siyasi deltalari, kalan uygun adım ve bekleme nedenini verir. `CanBegin/BeginRegionalReform` ve `CanEnd/EndRegionalReform`; başka haftalık cevap yok. Hâlihazırdaki due mandate/petition ve takvim retleri mutasyondan önce; yeni proje diğer borçların sözlerini yeniden yazmaz.

Yeni ekonomik taban tek leaf view'dan hesaplanmalı. Forecast, Dumas'ın aday yerel zararı/toplaması ve gerçek NextWeek aynı yolu kullanmalı; tarım reformunun gıda açığını kaldırması gerçek NPC planını da değiştirebilir. Tatil ile aynı bölge seçilirse dört eski tahsilat boyunca vergi muafiyeti sürer; projenin ticari getirisi ayrıca gizlice tahsil edilmez. Bugünkü farkı dört haftanın kesin getirisi diye çarpmayız.

## Sonraki sürüm için sınırlar ve üç kontrol

1. **Zaman/ortak hesap:** hazırlığın son haftası hâlâ eski bütçe; ilk etkin Forecast ve gerçek sonraki hesap eşit. Mandate/petition yüzünden reddedilen hafta hiçbir şey tüketmez. Bölge koşulu uygun değilken başarılı normal hafta kendi eski bütçesini tüketir; proje ek kaynak veya adım tüketmez. Tatil ve Dumas ile çift uygulama olmaz.
2. **Gerçek alternatif:** iaşe modu orduyu beslerken actual vergi kaybı; ticaret modu geliri artırırken actual gıda kaybı üretmeli. Mevcut güçlü civic strateji devam edebilir, ancak yeni ordunun gıda ihtiyacını bu uzmanlaşmayla karşılarsa parasal kapasiteden vazgeçer. Bu eski başarının sessiz nerf'i değildir.
3. **Tekrar/çıkış:** cancel/restart, sponsor ilişkisi cap0/100, aynı/different bölge ve save/load durumları bedava çıktı üretmez. Normal tabana dönüş0 asker/0 Power'da mümkün; geçmiş vergi tatili veya rol borcu silinmez. UI dört uygun adımı koşullu takvimden ayırır.

Bu tasarımın kalan temel sorusu, tek deney bölgesinin dörtte birlik dönüşümünün gerçek sefer maliyetlerine yeterince değerli olup olmadığıdır. Örneğin+5 nominal Food, mevcut200 kişilik alımın bütün haftalık gıda ihtiyacını tek başına karşılamaz. Önce tek doğal kampanya karşılaştırmasıyla fiyat ve anlam değerlendirilir; daha geniş reform sistemi veya yeni harita bu işin parçası değildir.
