# Sonraki siyasi baskı — karar için konsept

6 Eylül2026. Yalnız tasarım; uygulama izni değildir. Başlangıç rolleri, denge ve kurgu kişiler çalışma varsayımıdır. Yeni tarihsel iddia yoktur. Kaynak incelemesi: `CampaignCore.CalculateEconomy/NextWeek`, `CampaignRoles`, `CampaignPatronTrust`, `CampaignRegionalAccords`, `CampaignVictoryDecisions`, `CampaignDumasInitiative`, `CampaignArmyEstablishment`. Bu not hazırlanırken root ordu sınırının ortak kapısını çalıştırıyor; Assets değiştirilmedi, derleme/probe/player açılmadı.

## Somut boşluk

Oyuncu bugün vaat bozabilir; gerçek yiyecek açlığından sonra Dumas kendisi davranır. Buna karşılık devletin taşıdığı sürekli ordunun siyasi meşruiyeti sorgulanmıyor. Sağlıklı her hafta oyuncuya +0,5 Power verir; bütçe açık olsa bile hazinedeki eski para maaşı ödüyorsa bu artış sürer. Assembly Approval düzenli vergiyi gerçekten etkiler; Morel'in kendi kurumsal talebi buna rağmen çoğunlukla oyuncunun yardım istemesini bekler. Yeni ordu sınırı ekonomik daralmayı uygulanabilir kıldı. Eksik bağlantı, bu kararı oyuncudan isteyen ve uymamanın kişisel yetki üzerindeki sonucunu açıkça üstlenen bir siyasi aktör olabilir.

## Üç farklı yön

**A — Morel'in askerî bütçe itirazı.** Düzenli vergi, ordunun haftalık giderini karşılamadığında Morel bunu konseyde açıklayıp iki hesaplık düzeltme süresi ilan eder. Kasa dolu olması siyasi itirazı kaldırmaz: mesele o anda iflas değil, sürekli orduyu hangi gelirle taşıdığımızdır. Oyuncu mevcut ordu sınırıyla gideri azaltabilir, ülkeyi sakinleştirip düzenli tahsilatı artırabilir veya ordusunu tutarak meclis desteği ve kişisel yetki birikimini riske atabilir. Yeni borç, satın alınan tahsilat bonusu veya özel vergi istisnası gerekmez.

**B — Valcourt'un bölgesel arabuluculuğu.** Sürekli dirençli bir bölgede oyuncu uzun süre çözüm üretmezken Valcourt yerel eşrafla kendisi görüşme ilan eder. Oyuncu önce mevcut ekmek/uzlaşma/asker yoluyla işi çözebilir; görüşme gerçekleşirse geçiş sakinleşir, fakat sarayın desteklediği eşraf idarede söz kazanır. Sonraki zorlayıcı emir, yalnız kaynak fiyatı değil saraya karşı kişisel yetki bedeli taşır. Bu bölgeyi yönetme hakkının sahibini görünür yapar. Ancak bölgesel yetki alanı, hangi emrin kime karşı kullanıldığı ve meşru geri alma yolu için yeni kurallar gerekir. Sadece huzursuzluk indirimi ve sonraki vergi cezasına indirgenirse mevcut anlaşmayı tekrarlar; bu yüzden ilk tercih değil.

**C — Lefèvre'nin belediye muhafızları.** Başkentte ordu azaltıldıktan sonra şehir temsilcisi, dönen insanların bir bölümünü mahalle güvenliğine çağırır. Bu kişiler ölmez veya görünmez havuza kaybolmaz: Manpower içinde açıkça işaretlenmiş sivil taahhüt olur. Yerel güvenlik kazanılır; aynı kişileri yeniden düzenli orduya almak şehir desteği ve kişisel yetki pahasına mümkündür. Oyuncu küçük orduyla güvenliği temsilcilere bırakmak ile tek askerî merkeze sahip olmak arasında kalır. Bu gerçek bir toplumsal bağımlılık yaratır, fakat ikinci garnizon kaynağı ve iki amaçlı rezerv muhasebesi gerektirir. Sıfır orduda yeniden toparlanmayı kapatmamak özel tasarım ister.

**Önerim A.** Yeni ordu politikasını anlamlı siyasi baskıya bağlar; var olan asker, tahsilat ve kişisel Power yeterlidir. Morel oyuncuya kızdığı için rastgele zarar vermiyor: meclisin onaylayabileceği bütçe ve ordunun sınırı için kendi çıkarını savunuyor. Yüksek kişisel ilişki bu kurumsal çıkarı otomatik silmemeli.

## A için küçük, okunabilir sözleşme adayı

Tetikleyici, başarılı haftanın gerçek hesabında `ArmyCost > TaxIncome` ve yaşayan ordu bulunmasıdır. Tek seferlik vergi düğmesinin100 altını veya saray avansı düzenli gelir sayılmaz. Henüz açık itiraz veya devam eden güvensizlik yoksa Morel iki sonraki başarılı hesap sonunda görüş bildireceğini duyurur. Son iki takvim haftasında yeni süre açılmaz. Duyuru para harcatmaz; o haftanın eski sonuçları değişmez.

Talep, iki hafta sonra **sıradaki düzenli bütçede** ordunun kendi haftalık giderinin vergiyi aşmamasıdır. Vade kontrolü ordu azaltmasının gerçek grubundan sonra yapılmalıdır. Böylece ilan edildiği hafta hedefi küçülten oyuncu, tam vade sonunda ayrılan200 kişinin katkısını gerçekten kullanabilir. Yalnız hedef yazmak yeterli değildir; henüz ayrılmamış asker giderden düşülmez. Savaşta verilen gerçek kayıp da gideri azaltır; itirazın konusu gider olduğu için bunu ayrıca cezalandırmayız.

Gider artık karşılanıyorsa Morel itirazını kapatır. Ekstra Power veya hediye kaynak verilmez; oyuncu ordu gücünden, sakinleştirme kaynaklarından veya zamandan zaten vazgeçmiştir. Kasa açığını avansla kapatıp aynı büyüklükte orduyu tutmak askerî açıdan mümkün kalır; siyasi talep yine karşılanmamış olabilir.

Karşılanmazsa tek bir açık güvensizlik kaydı oluşur. **Çalışma fiyatı:** Assembly Approval bir defa−5; güvensizlik sürerken sağlıklı haftanın mevcut +0,5 idari Power artışı verilmez. Savaşın +4 Power'ı, tanınma, diğer kayıplar ve eski patron koşulları aynen kalır. Bu sayılar onaylanmış denge değildir;−5, mevcut anlaşmanın +5 ölçeğiyle ilk karşılaştırma adayıdır. Sabit her hafta yeni ceza veya tekrar tekrar−5 yoktur. Approval değişikliği sonraki vergi hesabını mevcut formülle etkiler; ayrı gizli vergi kesintisi eklenmez.

İyileşme yolu görünürdür: herhangi bir sonraki başarılı hafta sonunda gerçek kalan ordu, sıradaki düzenli vergiyle taşınabilir olduğunda güvensizlik kapanır. Sırf düğme değiştirerek aynı haftada kapatıp yeniden açmak olmaz. Meclisin kaybettiği desteği mevcut anlaşma, dilekçe veya uygun rolün tuttuğu sözlerle yeniden kazanmak mümkündür; kapanış Approval'ı ücretsiz geri yazmaz. İtirazın sonucundan itibaren dört haftalık yeniden açılma aralığı önerilir. Power0, ilişki0 veya ordu0 çıkışı kapatmaz.

İdari Power hesabı haftanın başındaki güvensizlik durumunu kullanır. Vade sonunda yeni güvensizlik oluşması az önce kazanılmış +0,5'i geri almaz; engel sonraki hesapta başlar. Kapanış da ödenmiş haftayı yeniden yazmaz, normal artış sonraki hesapta döner. Bu tarih, terms ve günlükte açıkça görünmelidir.

## En küçük API ve bütünleşme

Kalıcı aday üç alan: `MorelBudgetDueWeek`, `MorelNextBudgetWeek`, `MorelBudgetCensure`. Biri hazırlığı, biri yeni duyuru aralığını, biri yürürlükteki siyasi sonucu taşır. Önceki bütçenin tamamını, asker grubunu veya Journal'dan türetilen sayaçları saklamaya gerek yoktur. Yeni sürüm ancak uygulama seçilirse açılır; mevcut3/4/5/6 göç eşikleri bağımsız kalır.

`HasBudgetPressure(state)` ve salt okunur `GetBudgetPressureTerms(state)` yeterli ilk API'dir. Terms: durum/aktör/gerekçe, süre, gerçek güncel `ArmyCost/TaxIncome`, karşılanmayan fark, idari Power artışının engellenip engellenmediği ve gerçek clamp ile tek seferlik destek bedeli. Ayrı bir “itirazı öde” komutu yoktur; oyuncu zaten `SetArmyEstablishment`, `Act`, `GrantRegionalAccord`, `Issue/ResolveMandate`, `March/ResolveBattle` kullanır. Kayıt, gelecekteki tüm giderleri garanti eden sözleşme diye sunulmaz; karşılaştırma sonraki tek hesabın bugünkü görünümüdür.

Önce eski petition/mandate/calendar guard'ları çalışır. Bütçe ve Dumas toplaması aynı ortak projection üzerinden bir kez uygulanır; dört vergi hesabının bitişi ve gerçek demobilizasyon tamamlanır. Siyasi değerlendirme bu sonuçtaki bir sonraki hesap görünümünü okur. `Forecast` siyasi terms'i çağırmaz; dış değerlendirme `Forecast` sonucunu okur, böylece yeni karşılıklı özyineleme oluşmaz. Güvensizlik cezası mevcut haftanın tahsilatını geri dönük değiştirmez. Yeni başlangıç, vade sonucu ve kapanış jurnal/satır haberi olur. Hafta2 dilekçesi ve dueMandate yalnız mevcut öncelikleriyle kalır; yeni itiraz dördüncü kilitleyen modal değildir. PendingVictory eski başarılı hafta kuralıyla kapanır.

## Oyuncunun anlatabileceği on dakika

“Sefer için orduyu büyüttüm. Kasada param vardı ama Morel vergilerin bu orduyu taşımadığını gösterdi. İki hafta sonra askerlerimin bir kısmını eve gönderebilir ya da siyasi desteğimi riske atabilirdim. Önce kazandığım savaşı tutmak istedim; meclisin desteği düştü ve makamımın kendiliğinden güçlenmesi durdu. Sonra gerçek bir grubu terhis ettim, taşradaki düzeni düzelttim. Bütçe düzeldi; Morel'in itirazı kalktı ama kaybettiğim desteği ayrıca toparladım.” Bu anlatı henüz koşulmuş fixture değildir. Hazırlık için API-only bir alım rotasında ilk gerçek ArmyCost>TaxIncome hesabı bulunmalı; tutarı veya haftayı kaynak tahminiyle gerçek player sonucu diye yazmamalıyız.

## Üç risk ve kabul kontrolü

1. **Yeni kısır döngü:** destek düşüşü vergiyi düşürür. Ceza tek sefer kalmalı;0 orduyla toparlanma, patrona erişim kaybı ve Power0 ayrı kontrol edilmeli. Takvim dışında imkânsız süre kurulmaz.
2. **Vade ve önizleme dürüstlüğü:** demobilizasyon aynı gün gerçekten yetişmeli; kayıplar/forage/vergi tatilinin son hesabı iki kez uygulanmamalı. Duyurudan önce/sonra ve vade önce/sonra actual Forecast ile günlük kayıt karşılaştırılmalı; eski hafta gelirinin yeniden hesaplanması yasak.
3. **Otomatik doğru seçim:** küçülme her senaryoda üstün olmamalı. Aynı kayıttan büyük orduyu koruyup siyasi bedeli taşımak ile iki hafta sonunda küçük orduya geçmek karşılaştırılmalı. Sadece bir düğmeyle ücretsiz kapatma, tekrarlı bildirim çiftliği ve başarıda bedelsiz Approval geri kazanımı bulunmamalı.

## Revizyon: oyuncunun verdiği ve geri alabildiği gerçek yetki

Root A'yı seçmedi; yukarıdaki A sözleşmesi **uygulama adayı olarak geri çekildi**. B/C daha geniş bölge ve rezerv kuralları istiyor. Onların yerine **Dumas'nın subay tayin beratını** öneriyorum. Bu yalnız tasarımdır; fiyat ve sadakat deltası henüz çalışma sayılarıdır.

### Seçim ve somut çıkar

Oyuncu Dumas'ya, kendi subaylarıyla mevcut kampta ilave200 kişilik grup kurma yetkisi verir. Her grup oyuncunun açık ve ücretli emriyle oluşur. Dumas'nın gerçek hakkı: bu kadro sürerken orduyu bütçe sınırına indirmek için önce subay düzenini devralmak gerekir. Karşılığında kampı terk etmeden aynı hafta ikinci alım yapabiliriz.

**Kendin yap:** mevcut `Act(recruit)` bir bölgede haftada bir200 kişi verir. Mevcut kodda ordu başka bölgeye yürürse orada da alım mümkündür; normal yöntem küresel olarak haftada200 ile sınırlı değildir. Dolayısıyla Dumas beratı olmadan beklemek veya gerçekten yürüyüp başka bölgede alım yapmak geçerli alternatiftir. Yürüyüş mevcut yiyecek, teçhizat, yorgunluk ve hareket bedellerini taşır. Oyuncu buna karşılık ordu sınırını serbestçe değiştirmeye devam eder.

**Dumas'ya yaptır:** aynı kampta bir ilave grup kur; başka bölgeye yürüyüş ve onun hareket bedelini harcama. Eski120 altın/20 yiyecek/15 teçhizat/200 Manpower bedeli tam ödenir; savaş bekleyen askerlerin tümünü kullanır. Özel grup için mevcut alımın yerel huzursuzluk+2, moral−2 ve ordu desteği+2 sonuçları korunur. Dumas'nın sadakati yalnız gerçekten kurulmuş bu grup için **+1** artar; imza, boş bekleme veya tekrar açma sadakat üretmez. Böylece oyuncunun verdiği iş ve kaynak kişisel bağlılık yaratır. Bu sadakat mevcut zafer tanınması ve yiyecek toplamasındaki Ambition>Loyalty karşılaştırmasında gerçekten çalışır. Yeni gizli ihanet olasılığı, genel yetkinliği yükseltme veya doğrudan Power ödülü eklenmez. +1, asker toplama hızının asıl faydasını gölgelememek için küçük bir ilk adaydır.

### Dar, kesin sözleşme adayı

- Tek berat, yalnız Dumas. Dört siyasetçiyi aynı mesleğe çevirmiyoruz. Bütün başlangıç rolleri erişebilir; yeni rol veya yeni karakter yoktur.
- Beratı imzalamak için yaşayan ordu ve mevcut `campaign` politikası gerekir. Aktif `budget` veya bekleyen azaltma kendiliğinden iptal edilmez; oyuncu önce o politikasını açıkça değiştirir. İmzanın anlık kaynak/Power/sadakat etkisi yoktur. Dumas'nın kendisine yetki veren teklifi kabul etmesi düşük kişisel ilişkide de makuldür; role özgü güven onarımı yeni bir erişim kilidine dönüşmez.
- Berat açıkken, mevcut kampın normal alımı zaten kullanılmışsa başarılı hafta başına **bir** ilave grup alınabilir. Limit bölge başına değil bütün kampanya için birdir. Aynı hafta berat iptali, yeniden imza veya kamp değiştirme bu hakkın kullanıldığını unutmaz. Kaynak/kapasite/konum kontrolleri normal alımla aynı helper'dan gelir.
- Berat açıkken yeni `budget` politikasına geçiş, “önce subay tayin hakkını devral” gerekçesiyle reddedilir. Olağan alım, hareket, savaş, ekmek, vaatler ve haftanın ilerlemesi sürer. Mevcut askerler tutulur; demobilizasyona gizli istisna veya dokunulmaz subay birliği yaratılmaz.
- **Yetkiyi geri al:** mevcut canlı ordunun bir haftalık yalnız asker ücretini, `ceil(Troops/12)` altın, geçiş ödemesi olarak ver; berat kapanır.36 altın teçhizat ikmali bu fiyata eklenmez. Ödeme mevcut subay düzeninin devridir; asker veya Manpower azalmaz, Dumas'nın kazanılmış sadakati geri silinmez. Böylece oyuncu önce onun örgütleme kapasitesini kullanıp sonra para ödeyerek yönetim hakkını geri alabilir. Aynı gün artık yeni bütçe sınırı kurabilir; eski iki haftalık demobilizasyon kuralı başlar.
- Geri alma fiyatı canlı orduyla değişir; yeni alım artırır, gerçek kayıp azaltır. Önizleme sabit geçmiş söz diye yazılmaz. Miktar yetmiyorsa bütün işlem atomik ret olur. Yaşayan ordu0 ise fiyat0 ve geri alma mümkündür; Power asgari şartı yoktur. Alınmamış gelecekteki asker üzerinden bedel çıkarılmaz.

Geri alma, zafer priminin miktarıyla hesaplanır; karşılığında yeni sadakat/kontrol değil verilmiş emir hakkı geri gelir. İlave gruba ihtiyacı olmayan oyuncu imzalamayarak parasını ve doğrudan yönetimini korur.

### Asgari veri, API ve mevcut yükümlülükler

İki kalıcı bool yeterlidir: `DumasOfficerCommission`, `DumasExtraRecruitUsed`. İkinci alan berat kapalıyken true kalabilir; başarılı NextWeek'de normal alım işaretleriyle birlikte temizlenir. Kayıp/yeniden yükleme yeni hak üretmez. Ayrı asker soy kütüğü, yeni savaş birimi, anlık hazine kredisi veya yerel vergi hakkı gerekmez.

`GetOfficerCommissionTerms(state)` mevcut hak, ilave alımın kullanımı, yeni grubun bütün eski fiyatları, gerçek clamp sadakat deltası, canlı asker ve güncel geri alma fiyatını döndürür. `CanGrantOfficerCommission/GrantOfficerCommission`, `CanRecruitThroughDumas/RecruitThroughDumas`, `CanRevokeOfficerCommission/RevokeOfficerCommission` mevcut `ActionResult` düzenini izler. Önizleme bir sonraki ordu ücretini ortak gider helper'ından da gösterebilir; bütün ülkenin geleceğini tahmin etmez. Çift tıklama ve aynı imza atomik reddedilir.

Mevcut petition ve dueMandate önceliği yeni komutların tümünden önce kalır. Aktif dört vergi hesabının şartı, patron borcunun bölgesi/tutarı, Dumas toplamasının ortak Forecast'i ve PendingVictory'nin yaşam süresi değişmez. İlave alım gerçek Troops eklediği için bir sonraki ücret/gıda ihtiyacı, zafer priminin güncel fiyatı ve Dumas'nın açlık değerlendirmesi kendiliğinden mevcut formüllere girer. Yeni zorunlu pencere ve haftalık NPC oylaması yoktur. Olası arşiv artışında bütün eski bağımsız eşikler korunur.

**0 Troops +0 Power toparlanması:** açık berat ücretsiz kapatılır; boş orduyla hiçbir komut hakkı gasp edilmez. Mevcut kaynakları yeten oyuncu normal recruit ile200 kişilik orduyu kurabilir; bu zaten oyundaki aynı120/20/15/200 bedeldir. Kaynaklar yoksa yeni sistem ücretsiz asker icat etmez, mevcut sıfır-ordu ekonomi/toparlanma yolunu açık bırakır. Yaşayan orduda0 Power da ne eski alımı ne ücretli geri almayı engeller. Geri alma için altın yetmiyorsa beklemek veya mevcut vergi eylemi kullanılabilir; haftayı durduran yeni kilit yoktur.

### Oyuncu anlatısı ve üç kabul kontrolü

“Cepheyi terk etmek istemedim; Dumas'ya subaylarını seçme yetkisi verip aynı kampta ikinci grubu kurdurdum. Parasını ben ödedim, adam bana daha bağlı hale geldi. Zaferden sonra ordunun pahalı olduğunu görünce onun tayin hakkını devraldım ve küçülmeye kendim karar verdim. Daha yavaş normal alımı seçseydim bu siyasi düzeni kurmam gerekmeyecekti.” Bu, henüz denenmiş player rotası değildir.

1. **Anlamlı alternatif:** aynı başlangıçtan normal alım+gerçek yürüyüş ile sabit kampta Dumas alımı karşılaştırılmalı. Beklemek zaten yeterliyse veya hareket için iyi bir neden varsa berat otomatik üstün olmamalı. Sonradan devralma fiyatı şartlarda açık okunmalı.
2. **Hak çiftliği ve gerçek etki:** grant/revoke/regrant, yürüyüş ve save/load ilave alımı çoğaltmamalı. Sadakat sadece ücretli yaşayan grupta bir kez artmalı; kapasite/maliyet retleri kaynak, ilişki, hak veya jurnal değiştirmemeli. Tekrarlı alımın borç, gıda ve demobilizasyon üzerinde gerçek bedeli bulunmalı.
3. **Toparlanma ve sözlerin korunması:**0 asker/0 Power,0 altınla yaşayan ordu, eski açık mandate/accord/forage ve vade haftası ayrı denenmeli. Devralma askeri silmemeli, demobilizasyonu erkene çekmemeli ve mevcut sözün fiyatını değiştirmemeli.

### Seçilmiş uygulama ve teslim kaydı

Root son revizyonu onayladı. `CampaignOfficerCommission.cs`, ortak normal alım helper'ı, iki state bool'u, ArmyEstablishment'in açık hak guard/doğrulaması ve Archive v7 uygulandı. İmza yalnız yaşayan ordu/campaign; ek grup mevcut kamptaki normal alımdan sonra haftada bir; eski120/20/15/200 ve yerel sonuçlar aynı helper'dan gelir. Ek grup gerçek clamp ile sadakat+1 verir. Geri alma `ceil(canlı asker/12)` altın; kazanılmış sadakat, askerler, açık zafer ve eski vaatler korunur. Used ancak kabul edilmiş NextWeek'de temizlenir; kapalı hak+Used=true geçerlidir. Used=true için o haftadan en az bir RecruitUsed bölgesi bulunması doğrulanır. Açık hak+budget geçersizdir.

Arşiv v7 iki bool'un varlığını/null/geçersiz değerlerini DCS typed projection ile doğrular. Eski1–6 alanları false'a taşınır; gizlenmiş true değerleri reddedilir.3/4/5/6 eşikleri bağımsız kaldı. Bu yaklaşım DCS'nin kabul ettiği tür dönüşümlerini tamamen yasaklayan bir JSON Schema iddiası taşımaz; yeni elle yazılmış JSON parser eklenmedi. Terms kaynak/kapasite yetmediğinde AfterRecruit stoklarını mevcutta tutar; CanRecruit kesin ret sebebini verir. UI bu durumda uygulanabilir artış varmış gibi göstermez.

Yeni `OfficerCommissionTests.cs` ve meta; eski Role/Accord/Victory/Dumas/Army archive testlerinin current7 uyarlaması ve ayrı eski-version case'leri teslim edildi. Sınırlar: normal ret sırası, gerçek iki-bölge alternatifi, globalUsed/regrant/movement/save, vade guards, fiyatın gerçek kayıpla değişmesi, clamp,0 asker/0 Power'dan eski vergi+normal alım yoluyla toparlanma ve birleşik v7 commission/NPC/victory/mandate/accord. Doğrudan state kurulan sınır testleri player kanıtı değildir.

API-only kaynak `Unity/WorkNotes/OfficerCommissionProbe.cs`; root'un çalıştırdığı çıktı `output/core-probes/OfficerCommissionProbe-2026-09-06T03-25-54-113Z-8f54535b/probe.stdout.log` salt okunur incelendi: **PASS checks=52**. Her iki ilk rota1600 asker/600 altın/2000 Manpower'a ulaşır. Normal yürüyüş rotası306 yiyecek/85 teçhizat/1 hareket/10 yorgunluk ve Normandiya kampı; Dumas rotası320/90/2/0 ve Île-de-France kampı, sadakat61 yerine normal60 verir. Açık yetkinin o andaki geri alma fiyatı134'tür. Son rota açıkça geri alıp4. haftada1400 asker/2200 Manpower'a iner. Bunlar pure Core sonuçlarıdır. Ortak Unity/player kapısı root tarafından ayrıca yürütülür. Bu agent derleme, test, probe veya player açmadı.
