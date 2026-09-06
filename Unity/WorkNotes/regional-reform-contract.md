# Bölgesel reform B — önerilen kesin uygulama sözleşmesi

Durum: root B'yi seçti; bu belge API hazırlığıdır. **Assets/Git/derleme/oyuncu değişikliği veya çalıştırması için başlangıç izni değildir.** PublicMood/atlas kaynak dondurması sürer. İlgili tasarım `next-regional-reform.md`; aşağıdaki adlar sonraki bağımsız Core/UI/test işlerinin ortak sözleşmesi olarak önerilir.

## Kurallar ve kalıcı alanlar

Tek proje/etkin düzenleme. `provisioning` sponsoru `morel`; `commerce` sponsoru `valcourt`.120 Gold ve4 Power başlangıçta tam ödenir; para veya güç yetmezse hiçbir değişiklik olmaz. Dört uygun **başarılı hafta hesabı** gerekir. Bölgenin hesabın sonundaki gerçek Unrest<65 ve Control≥55 ise bir adım ilerler; aksi halde yalnız proje bekler. Uygun olmayan bölgede başlanabilir, bekleme nedeni görünür olmalıdır.

Tamamlanma eski bütçe, bölge etkileri ve eski otomatik sözleşme/ordu etkilerinden sonra yapılır. Dördüncü hesabın ekonomisi eskidir; etkin reform ilk kez sonraki bütçede hesaba girer. Yeni week gate yoktur. Hafta2'de o başarılı hesap içinde gelen dilekçe, kazanılmış proje adımını iptal etmez; sonraki haftanın mevcut dilekçe/mandate retleri her şeyden önce çalışır.

Tamamlanırken sponsor Relationship değişimi `Clamp(current+4)−current`; sona erdirirken hazırlık/aktif ayrımı olmadan `Clamp(current−8)−current`. İptal ek kaynak istemez, eski120/4 geri dönmez. Asker, rezerv veya geçmiş üretim geri alınmaz. Sponsor ve bölge seçilen mod/bölgeden sabittir; haritada başka yer seçilmesi projeyi taşımaz. Yeni yere/moda geçmek önce açık iptal, sonra yeni ödeme ve hazırlıktır.

`CampaignState` için üç alan:

| Alan | Tip | Kanonik anlam |
| --- | --- | --- |
| `ReformRegionId` |string |Kapalıysa `""`; aksi halde mevcut tanım kimliği. |
| `ReformModeId` |string |Kapalıysa `""`; aksi halde provisioning/commerce. |
| `ReformStepsRemaining` |int |Kapalı0; hazırlık1..4; çalışan proje0. |

Sponsor, başlangıç tarihi veya etkinleşme geçmişi ayrıca saklanmaz. `remaining=0` tek başına “proje yok” anlamına gelmez. Bekleme için ayrı boolean saklanmaz; mevcut bölge durumu okunur. Hazırlık durakladığı için takvim sonuna yaklaşan eski proje geçerli kalır; doğrulama onu yeni başlangıçmış gibi reddetmez.

Takvim için açık öneri: yeni imza, dört uygun hesap **ve ilk değiştirilmiş bütçe** için yer varsa kabul edilir; son başlangıç haftası `MaximumWeek−5`. Bu, önceki en-erken-tamamlanma cümlesini ilk kullanılabilir bütçe açısından netleştirir; root başlangıç yetkisinde bu sınırı onaylamalıdır. İptal hafta ilerletmediğinden MaximumWeek'te de mevcut dilekçe/mandate önceliğinden sonra mümkün olmalıdır; hazırlıkta kalmış projeden çıkış kapanmaz.

## Public API

```csharp
public const int RegionalReformGoldCost = 120;
public const float RegionalReformPowerCost = 4f;
public const int RegionalReformPreparationWeeks = 4;
public const float RegionalReformMinimumControl = 55f;

public static bool HasRegionalReform(CampaignState state);
public static RegionalReformTerms GetRegionalReformTerms(CampaignState state);
public static RegionalReformTerms GetRegionalReformTerms(
    CampaignState state, string regionId, string modeId);
public static ActionResult CanBeginRegionalReform(
    CampaignState state, string regionId, string modeId);
public static ActionResult BeginRegionalReform(
    CampaignState state, string regionId, string modeId);
public static ActionResult CanEndRegionalReform(CampaignState state);
public static ActionResult EndRegionalReform(CampaignState state);
```

`GetTerms(state)` geçerli kapalı kampanyada da null değildir: StatusId=closed, boş kimlikler,0 adım; gerçek mevcut ekonomi ve onunla eşit karşılaştırmalar. Geçerli açık projede hep **asıl** bölge/modu döndürür. `GetTerms(state,region,mode)` kapalı kampanyada iki öneriyi karşılaştırmak içindir; açık proje varsa null döner, sessizce mevcut projeyi değiştiren hayalî bir geçiş göstermez. Invalid state/unknown region/unknown mode null. Yeterli kaynak olmaması bir önizlemeyi gizlemez; CanBegin ayrı ret nedenini verir. Bütün sorgular salt okunurdur.

## RegionalReformTerms — alan alan

| Alan | Tip | Kesin anlam |
| --- | --- | --- |
| `RegionId`, `ModeId`, `SponsorId` |string |Asıl proje veya seçilen kapalı öneri; closed durumunda boş. |
| `StatusId` |string |`closed`, `proposed`, `pending`, `blocked`, `active`. blocked yalnız hazırlığın mevcut yerel koşulları uygun değil demektir. |
| `RegionReadyNow` |bool |Bugünkü U<65 ve C≥55. **Bir sonraki hesabın kesin ilerlemesi değildir.** |
| `WaitReasonKey` |string |Boş veya `reform.wait.unrest`, `.control`, `.both`; closed/active için boş. |
| `WaitReasonArgs` |string[] |Boş olmayan neden için [mevcut U, mevcut C,65,55]; gereken sınırlar da Core'dan gelir. UI yeni karşılaştırma veya eşik sabiti yazmaz. |
| `RegionUnrest`, `RegionControl` |float |Bugünkü gerçek yerel değerler; closed için0. |
| `GoldCost` |int |Başlangıç fiyatı120; active ekranında yeniden ödenecek tutar olarak sunulmaz. |
| `PowerCost` |float |Başlangıç fiyatı4; düşük güçte gizlice min(clamp) ile ucuzlamaz. |
| `StepsRemaining` |int |Proposed4; mevcut hazırlıktaki gerçek1..4; closed/active0. |
| `EarliestActivationWeek` |int |Proposed/pending için Week+remaining, takvim içindeyse; closed/active veya ulaşılamıyorsa−1. Geçmiş etkinleşme tarihi uydurulmaz. |
| `EarliestFirstReformedBudgetWeek` |int |Yukarıdaki en erken etkinleşme+1, takvim içindeyse; diğer durumda−1. |
| `NextBudgetWeek` |int |Week+1 takvim içindeyse, aksi halde−1. Active UI bu tarihte zaten etkin bileşimin kullanılacağını söyleyebilir. |
| `BaseTax`, `BaseFood` |int |Asıl değişmeyen RegionDefinition tabanları. |
| `ReformedBaseTax`, `ReformedBaseFood` |int |Seçilen mod çalışıyormuş gibi etkin tabanlar. |
| `NominalTaxDelta`, `NominalFoodDelta` |int |Yukarıdaki iki taban farkı; gerçek tahsilat/stok kazancı değildir. |
| `SponsorRelationship` |float |Bugünkü sponsor ilişkisi. |
| `CompletionRelationshipDelta` |float |Bugün tamamlansaydı gerçek clamp ile+4'ün uygulanacak kısmı. Gelecekteki kesin ilişki kazancı vaadi değildir. |
| `EndRelationshipDelta` |float |Bugün iptal edilirse gerçek clamp ile−8'in uygulanacak kısmı. |
| `CurrentTaxIncome`, `CurrentProduction`, `CurrentNetFood` |int |Gerçek bugünkü ortak haftalık Forecast. |
| `WithoutReformTaxIncome`, `WithoutReformProduction` |int |Aynı state/koşullar, yalnız bu reform etkin değil. |
| `WithReformTaxIncome`, `WithReformProduction` |int |Aynı state/koşullar, yalnız bu bölge/mod etkin. |
| `TaxIncomeDelta`, `ProductionDelta` |int |With−Without; ülke toplamları yuvarlandıktan sonraki fark. |
| `WithoutReformForageFood`, `WithReformForageFood` |int |Bu iki gerçek koşullu projeksiyondaki Dumas bileşeni. |
| `WithoutReformNetFood`, `WithReformNetFood`, `NetFoodDelta` |int |Aynı iki projeksiyon ve With−Without. Üretim artışını aynı miktarda stok artışı gibi göstermemek için. |

DTO alanları sayısal fiyat/karşılaştırma verir; UI lokalizasyon anahtarına gömülü ikinci bir denge tablosu tutmaz. Proposed yerel koşulları kötü olsa bile StatusId=proposed kalır; RegionReadyNow/WaitReason uyarıyı taşır. Mevcut projenin blocked/pending ayrımı bugünkü durumu açıklar. Gerçek bir sonraki adımı göstermek istenirse root'un mevcut başarılı `nextState` kopyasındaki remaining farkı kullanılabilir; yalnız RegionReadyNow'dan “bu hafta kesin ilerler” çıkarılmaz. `weekCheck.Ok=false` iken bu kopya kabul edilmiş hafta gibi sunulmaz.

## Tek ekonomik yol

Asıl BaseTax/BaseFood dörtte biri ayrı ayrı double ile AwayFromZero yuvarlanır. Provisioning: tax−pay, food+pay; commerce tersidir. Kalıcı tanımlar değiştirilmez. `EconomyView` reform override'ını taşıyan bir lens alır; CalculateEconomy etkin tabanları bu lensten okur. WithForage ve WithExemption gibi türetilen görünümler aynı reform lensini korur.

GetTerms üç readonly senaryo kurar: actual, reform kapalı, seçilen reform etkin. Her biri **BuildWeekProjection** yolundan geçer; Forecast→GetTerms veya GetTerms→NextWeek döngüsü yoktur. Dumas'ın aday yerel hasarı ve toplaması her senaryoda kendi gerçek gıda ihtiyacına göre hesaplanır. Reform yeterli üretim sağlarsa Dumas ihtiyacının kalkması, ulusal Production/TaxIncome farkına da yansıyabilir; yalnız yerel BaseFood farkını sonuç diye vermeyiz.

Aynı bölgedeki gerçek tatil o bugünkü senaryoda hâlâ muafiyet uygular. Bu karşılaştırma **bugünkü koşullarda koşullu tek hesap**tır; hazırlık bittiğinde tatilin biteceğini varsayıp geleceğin vergisini eklemez. “Dört hafta boyunca şu kadar kazanırsın” gibi çarpım yapılmaz. Active durumda Current=With, pending/proposed durumda Current=Without. Delta her durumda With−Without olduğundan etkin projenin mevcut katkısı da sıfırmış gibi görünmez.

Direnişin30×BaseTax örgütlenmesi asıl tanımı okumaya devam eder. Reformu aç/kapatmak düşmanı doğrudan yeniden ölçeklendirmez. U/C/E, savaş, ordu giderleri, mevcut açlık kuralları ve radikalizm değiştirilmez.

## Komut retleri ve kayıtlar

Begin sırası: invalid state → unknown region → unknown mode → mevcut petition → due mandate → açık proje → takvim → Gold → Power. Yerel hazırlık koşulu ret değildir. End: invalid state → petition → due mandate → proje yok; ilave kaynak/takvim ilerletme maliyeti yok. Tüm retler state/journal'ı aynen korur.

Yeni error anahtarları: `error.reform.state`, `.mode`, `.open`, `.none`, `.calendar`; `.gold` args=[120], `.power` args=[4]. Bölge/petition/mandate için mevcut uygun anahtarlar kullanılır. `log.reform.ready` yalnız Can dönüşü, journal değil. Gerçek kayıtlar:

- `log.reform.started`: [regionKey, modeKey, sponsorNameKey,120,4,4steps].
- `log.reform.progress`: [regionKey, remainingSteps], yalnız gerçekten ilerleyen ve henüz tamamlanmayan hesapta.
- `log.reform.completed`: [regionKey, modeKey, sponsorNameKey,actualRelationshipDelta].
- `log.reform.ended`: [regionKey, modeKey, sponsorNameKey,actualRelationshipDelta,previousStatusKey].

Bekleme nedeni belge üzerinde canlı görünür; her hafta zorunlu modal veya aynı bekleme journal spam'i yoktur. Başlangıç fiyatı ve tarih kayıtları gerçek Begin journal'ında; DTO mevcut koşullu değerleri geçmişte gerçekten uygulanmış miktar olarak sunmaz. Özellikle aynı hafta eski accord Morel'i100'e getirmişse reform completion etkisi ayrıca0 olabilir.

## Arşiv ve iş sahipliği

v8 typed required projection üç alanın varlığını/null/uygunsuz tiplerini denetler; oyun-state doğrulaması mode/region/remaining birleşimlerini denetler. DCS'nin mevcut kabul ettiği coercion sınırı korunur; yeni el yapımı JSON parser yoktur. v1–7 boş defaults'a açık göç eder, gizlenmiş nondefault yeni alanlar reddedilir. Önceki accord≥3, victory≥4, Dumas≥5, establishment≥6 ve commission≥7 şartları bağımsız korunur.

- **Domain agent:** yeni `Core/CampaignRegionalReforms.cs` ve meta; CampaignCore'da üç state alanı/Validate/tek hafta-sonu hook ve etkin ekonomik taban okuması; EconomyView mevcut dosyası `CampaignDumasInitiative.cs` üzerinde yalnız lensin korunması; CampaignArchive v8. Localization dosyalarını yazmaz, yukarıdaki key/args sözleşmesini root'a verir.
- **Ayrı test agent, root tarafından atanır:** yeni `Tests/Editor/RegionalReformTests.cs` ve meta; gereken eski archive beklentilerini dar kapsamla uyarlama. Gold/Power/ret atomikliği, dört uygun adım ve duraklama, gerçek ilk bütçe, cap'li ilişki, iptal/tekrar, takvim, v1–7 göç ve v8 corruption, due/petition/accord/Dumas/commission/establishment birlikteliği. Eski testler gevşetilmez.
- **Root:** GameApp, mevcut belgeler içindeki Presentation, bütün RU/TR localization, AutoShots/protocol, bağımsız statik compile/pure probe/Unity/player/Git/checkpoint. Önce API ve Core/probe freeze, sonra tüm test/source freeze ve tek sahipli çalıştırmalar.

Normandy'nin+5 nominal Food'u200 yeni askerin bütün haftalık gıda ihtiyacını karşılamaz. Bu fiyatların veya dörtte bir dönüşümün hazır dengeli olduğu iddia edilmez; root'un doğal rota karşılaştırmasında gerçek ulusal fark ve sponsor bedeli açık ölçülür. Uygulama, ayrı başlangıç mesajından önce başlamaz.

## Uygulama teslimi

Root'un ayrı uygulama izninden sonra Core ve Archive v8 kaynakları tamamlandı ve SOURCE FREEZE bildirildi. Yeni partial/meta, CampaignCore üç alanı ve hafta-sonu hook'u, EconomyView reform lensi ve v8 required projection dışında kaynak değiştirilmedi. Hazırlık koşulu için U65/C55, ilişki için +4/−8 public sabitleri UI ile ortak. Geçmiş başarılı haftalardan fazla hazırlık adımı taşıyan state reddedilir; bekleyen projeye yeni bir bitiş tarihi uydurulmaz.

Kaynak incelemesi: WithForage/WithExemption aynı reform lensini koruyor; eski hesap lensi haftanın sonunda etkinleşen projeyi geriye dönük kullanmıyor. Accord tamamlanması ilişkiyi önce değiştiriyor, reform gerçek clamp farkını sonra kaydediyor. Bağımsız 3/4/5/6/7 arşiv eşikleri korunuyor. Bu teslim için domain agent derleme, probe, Unity, player veya test çalıştırmadı; gerçek doğrulama ve bağımsız test dosyaları root/test agent sorumluluğunda.
