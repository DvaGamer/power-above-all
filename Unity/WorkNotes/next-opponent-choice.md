# İlk bağımsız siyasi girişim

6 Eylül2026. Root, aşağıdaki tam-karşılama yaklaşımını daha sonra onayladı; kaynak uygulaması ve ayrı doğrulama kaydı belgenin sonundadır. Kişiler ve olaylar oyun kurgusudur; tarihsel olay veya kullanıcının seçtiği nihai rol sayılmaz. Kaynaklar mevcut `CampaignCore`, `CampaignRoles`, `CampaignPatronTrust`, `CampaignVictoryDecisions` ve `Localization/core.json` gündemleridir.

Bugün Morel temsilcilerin karar etkisini, Lefèvre düzenli ekmeği, Valcourt saray yetkilerini, Dumas ikmal ve zafer nüfuzunu istiyor. Kaynak sonuçları ve güven kapıları çalışıyor; fakat bu kişiler çoğunlukla oyuncunun düğmesini bekliyor. İlk girişimin farkı şu olmalı: NPC gerekçesini açıklasın, hazırlığını başlatsın ve oyuncu cevap vermese de ilan ettiği işi yapsın. Girişim yeni zorunlu dilekçe olmamalı.

## Üç farklı küçük konsept

**A — Morel'in tahsilat boykotu.** Vergi bölgeyi65 direnç eşiğine çıkarırsa temsilcileri örgütler; iki hesaplık uyarıdan sonra iki hesap gelir boykotu ilan eder. Mevcut ekmek/uzlaşmayla direnci azaltmak veya siyasi yetki devretmek çıkıştır. Ekonomi ve ilişki etkilenir; fakat yeni vergi tatiline benzer, ikinci ihlal cezası gibi okunabilir.

**B — Dumas'nın yiyecek toplama emri.** Gerçek açlıktan sonra gelecek hesap için yerel toplama ilan eder. Oyuncu ihtiyacı giderebilir veya yasaklayabilir; cevap yoksa NPC kendi işini yapar. Orduyu beslerken bölgesel gerilim ve siyasi yetki bedeli yaratır. **Önerim B**, aşağıdaki tam-karşılama düzeltmesiyle.

**C — Lefèvre'nin bağımsız mahalle iaşesi.** Başarısız Paris yardımından bir hesap sonra yerel üretimi iki hesap mahallelere ayırır: şehir sakinleşir, ulusal gıda azalır. Düzenli yardımı gerçekten ödemek çıkıştır. Sivil dayanışmayı gösterir; mevcut ulusal havuzdan ayrı yerel dağıtım hesabı gerektirdiği için ertelenir.

## Düzeltme: eksik yardım bugün yardım sayılmıyor

Mevcut açlık ikilidir:60 gıda açığına40 eklemek stok0, asker kaybı%8, ikmal−25 ve moral−15 sonucunu değiştirmez. Önceki `min(40, deficit)` önerisi bu durumda yalnız yerel/siyasi zarar üretirdi; **geri çekildi**. İki dar çözüm karşılaştırıldı:

1. **Yalnız tamamen kapatılabilir açığa müdahale.** Yerel zarardan sonraki ihtiyacın tamamı1–40 arasındaysa tam miktar toplanır.40 yetmiyorsa hiçbir toplama veya yan etki uygulanmaz; haber “yerel kaynaklar açığı kapatmaya yetmiyor” der. Eski açlık hesabına dokunmaz. Önerim budur.
2. **Kısmen korunmuş asker grubu.** Toplanan yiyeceğin besleyebildiği askerleri o haftanın açlık kaybından ayrı hesaplamak gerçek kısmi fayda sağlar. Fakat ulusal kıtlık, asker kaybı, ikmal/moral ve sivil tepkiyi ayıran yeni kurallar gerektirir. Bu bağımsız, açık bir tasarım olabilir; mevcut haftalık açlığa gizlice uygulanacak küçük düzeltme değildir.

Tam gıda açığını kapatmak bile ödenmeyen maaşı veya teçhizat eksikliğini çözmez. Önizleme bunları ayrıca göstermeli; “ordu kurtuldu” demek yerine “bu hesapta gıda açığı kapandı” demelidir.

## B'nin önerilen koşulları

Başarılı haftadaki **gerçek açlık**, yaşayan ordu ve dolmuş girişim aralığı hazırlığı açar. Gıda0 tek başına tetiklemez. İlk açlığın eski sonuçları aynen kalır; Dumas bir sonraki hesabı tarih olarak ilan eder. Hazırlık başlangıcından itibaren dört haftalık aralık önerilir; veto, yetersiz yerel kaynak veya düzelme bu aralığı sıfırlamaz.

Oyuncu gerçek açığı kapatabilir: açık Paris yardımını kapatmak tüketimi20 azaltır, eski siyasi bedellerini korur. Askerî rolün erişilebilir40-gıda yardımı yalnız o role aittir. Vergi tatilinin sakinleştirmesi üretimi artırıp açığı kaldırabilir; aşağıdaki ortak hesap bunu görür. Altın toplamak gıda satın almak sayılmaz.

Oyuncu karışmazsa Dumas yalnız tamamen kapatabileceği açığı toplar. Kamp bölgesinde huzursuzluk+8/elit bağlılığı−6, Dumas hırsı+3; sadakat hırstan düşükse4 kişisel güç, değilse0 bedel vardır. Otomatik kayıp kalan güçle sınırlıdır. Root bu miktarları tam-karşılama koşuluyla onayladı. Açık veto hazırlığı kapatır ve ilişki−4 getirir; normal açlık sürüyorsa eski sonuç uygulanır. Yetkinlik, normal emirler ve güveni onarma yolu değişmez.

Haber kampı izlediğini açıkça söyler; yürüyüşle güncel bölge değişebilir. Toplama eski anlaşmayı otomatik bozmaz. Hiç işlem olmayacaksa aday bölgenin zararları da uygulanmaz. Süreli haber için iki alan yeterli adaydır: `DumasForageDueWeek/NextForageWeek`; eski kayıtlar boş başlar. Son takvim sınırının ötesine hazırlık açılmaz.

## Tek hesap yolu, döngüsüz önizleme

Önerilen yapı `BuildWeekProjection(state, scenario) → CalculateEconomy(view)` şeklindedir. `view`, bölge değerleri ve vergi istisnasının salt okunur görünümüdür; tam seferi JSON ile kopyalamaz. **Yaprak** `CalculateEconomy` yalnız bugünkü üretim/vergi/tüketim formüllerini ve aynı yuvarlamayı içerir; NPC, Forecast veya terms çağırmaz.

1. Scenario varsa önce varsayımsal anlaşmanın gerçek clamp ile−10 huzursuzluk/+3 kontrol ve vergi istisnası görünümünü kur. Bu tabanın ekonomisini yaprakla hesapla.
2. NPC vadesi gelmemişse, ordu yoksa veya bu tabanda `Food+NetFood>=0` ise toplama yok. Özellikle varsayımsal sakinleşme açığı kaldırmışsa NPC zararını sonradan ekleme.
3. Aksi halde ayrı aday görünümde mevcut kampın+8 huzursuzluk/−6 elit değişimini uygula ve yaprağı yeniden çağır. `needed=−(Food+candidate.NetFood)` hesapla. Yalnız `1<=needed<=40` ise aday ve tam `needed` seçilir; aksi halde ilk görünüm korunur.41 ihtiyaca40 verme yoktur.
4. Son ekonomide üretim doğal üretim olarak kalır; ayrı `ForageFood` bileşeni görünür. `NetFood=Production+ForageFood−tüketimler`. Son vergi de seçilen görünümden gelir. Plan, iptal/yetersizlik/uygulama gerekçesini ve gerçek yerel/kişi etkilerini taşır.

`Forecast` planın ekonomisini, `GetForageTerms` aynı planın NPC kısmını döndürür. `GetRegionalAccordTerms`, normal ve varsayımsal scenario planlarını kullanır. Plan bu public yöntemlerin hiçbirini çağırmaz: Forecast↔GetTerms döngüsü yoktur. `TaxForgone`, seçilmiş aynı son görünümde yalnız vergi istisnasını açıp kapatan yaprak farkıdır; sakinleşme veya NPC kararı yanlışlıkla ikinci kez değiştirilmez.

`NextWeek` eski dilekçe→dueMandate→takvim guard'larından **sonra bir kez** normal planı alır. PendingVictory kapanır; planın yerel/kişi etkileri bir kez uygulanır. Sonra `Food=Stock(oldFood+plan.NetFood)` kullanılır: yiyecek ayrıca stoka eklenip ikinci kez sayılmaz. Yeni Forecast çağrısıyla uygulanmış NPC zararı tekrar hesaplanmaz. `log.week` tam aynı `plan.NetFood` değerini yazar. Dört vergi hesabının sonuncusu hâlâ istisnalı hesaplanır, anlaşma eski yerinde sonra tamamlanır. Reddedilen hafta hiçbir süreyi tüketmez.

## Gerçek yollarla sekiz haftalık hazırlık fixture'ı

Kaynak `CampaignBalanceProbe.cs` içindeki ölçülmüş `paris_subsidy_recruit` politikasıdır: legacy başlangıç;0. haftada Paris yardımı,0/2/4/6. haftalarda mevcut yerde birer gerçek recruit;2. hafta dilekçesine önce relief. Başka emir veya doğrudan state yazımı yoktur. Her hafta normal NextWeek; her eylemin Ok sonucu kaydedilir. Önceki koşu ilk açlığı8. hesapta göstermiştir. Bu mevcut yolla hazırlanan **sekizinci hafta sonu** fixture'ıdır; yeni uyarının uygulaması ilan edilen **dokuzuncu** hesaptadır, sekizinci diye sunulmamalıdır.

Root güncel kaynakla bu yolu yeniden doğrulamalı.8. haftanın Food/Troops, doğal ve yerel-zararlı Forecast, needed değeri kaydedilir.1–40 ise dokuzuncu hesapta yiyecek açığının tamamen kapanması ve preview/log eşitliği karşılaştırılır.40'tan büyükse yeni kuralın sıfır yan etkili iptali beklenir; sayıya ulaşmak için gizli state ayarı yapılmaz. Bu düzeltmede yeni koşu yapılmadı.

On dakikalık karşılaştırma bu son kayıttan yürür: NPC'yi bırak, veto et veya mevcut yardım politikasını değiştir; aynı sonraki hesabın asker, ikmal, kasa ve bölge farklarını gör. Ayrı sınır kontrolleri40/41, varsayımsal anlaşmayla ihtiyacın0 olması ve her iki sayısal bileşenin tek uygulanmasıdır. Haber konsey satırıdır; eski dilekçe ve patron borcunun yanına yeni zorunlu modal eklenmez.

## Uygulanan kaynak ve doğrulama kaydı

Sahip olunan kaynaklar: `Core/CampaignCore.cs`, yeni `CampaignDumasInitiative.cs`, `CampaignRegionalAccords.cs`, `CampaignArchive.cs`; `Tests/Editor/DumasInitiativeTests.cs` ve gerekli Role/RegionalAccord/Victory arşiv sürüm uyarlamaları. Yeni runtime/test `.meta` dosyaları sabit GUID taşır. UI, AutoShots ve RU/TR metinleri root'a aittir. Bu ajan derleyici, probe, NUnit, Unity veya player çalıştırmadı; yalnız kaynak yazdı ve root'un çıktısını okudu.

Public API: `HasDumasInitiative(state)`, `GetDumasInitiativeTerms(state)` ve stale tarihe karşı `CanVetoDumasInitiative(state, expectedDueWeek)` / `VetoDumasInitiative(state, expectedDueWeek)`. Terms: `RegionId/Disposition/ReasonKey/ReasonArgs`, `DueWeek/NextForageWeek/FoodGathered/FoodShortfall`, gerçek `UnrestDelta/EliteLoyaltyDelta/AmbitionDelta/PowerCost/VetoRelationshipDelta`. Disposition `gather/sufficient/too_large/no_army`. Too-large ihtiyacı aday yerel üretim kaybını içerir; uygulanan ekonomik açık ilk görünümde kalır. Bu iki değer UI'da aynı ölçü diye sunulmamalıdır.

Takvim sonu root tarafından daraltıldı: yeni duyuru yalnız `Week <= MaximumWeek−4`; `NextForageWeek <= MaximumWeek`. Aktifte `DueWeek==Week+1`, `NextForageWeek==DueWeek+3`. İlk duyuru ancak ilk başarılı hesabın sonunda olabileceğinden sıfır olmayan Next en az5'tir. Veto/iptal/uygulama due'yu temizler, Next'i tutar. Veriler bütçe hesabından önce, `RecordDumasInitiative` ise **Week++ sonrasında ilan edilmiş DueWeek tarihiyle** bir kez uygulanır/kaydedilir. `Forecast` ve tüm teklifler tek projection yolunu paylaşır; NPC yiyeceği yalnız `ForageFood` üzerinden `NetFood`'a bir kez girer.

Archive v5 bağımsız eşikleri korur: anlaşma gerekli alanları>=3/göç<3; zafer alanı>=4/göç<4; yeni iki NPC tarihi>=5/göç<5. v3 aktif/bozulmuş anlaşması, v4 açık zaferi ve eski rol sözleri yeni göçte silinmez. Eksik/null/yanlış tür NPC alanı v5'te reddedilir; eski sürüm numarasıyla gizlenen sıfır olmayan NPC tarihi de reddedilir.

Root'un gerçek saf Core çalıştırması: `output/core-probes/DumasInitiativeProbe-2026-09-06T02-13-33-458Z-9d6ace95/`. `probe.stdout.log` bu ajan tarafından okundu; **141 kontrol PASS**, fixture yalnız public API ile kuruldu. Çalıştırılan kaynak `Unity/WorkNotes/DumasInitiativeProbe.cs`; root helper adı `DumasInitiativeProbe`. Kaynak özeti ve derleme sonucu aynı klasördeki receipt'tedir. Bu Unity/player kanıtı değildir.

| Rota |8. hafta toplama önizlemesi |9. hafta asker |9. hafta ikmal/moral |9. hafta kasa/Güç |
| --- | --- | --- | --- | --- |
| Müdahaleye izin |36 gıda |1840 |85 /79 |804 /50 |
| Veto |0 gıda |1692 |50 /61 |806 /48,5 |
| Paris yardımını kapat |16 gıda |1840 |85 /79 |801 /48 |
| Orléans vergi tatili |35 gıda |1840 |85 /79 |786 /50 |
| Normandiya'ya gerçek yürüyüş |35 gıda |1803 |65 /71 |807 /50 |

Ortak8. hafta durumu765 altın/0 gıda/1840 asker/75 ikmal/76 moral/53,5 güç, Due9/Next12'dir. İzin rotasının gelecek hesap önizlemesi Production156+Forage36−tüketim192=NetFood0; dokuzuncu hesap yiyeceği0'da tutar, eski açlık kaybını engeller. Dokuzuncu hafta sonrasında yeniden Forecast almak **sonraki** haftayı gösterir: izin rotasında yeni NetFood−35, planlanan toplama0'dır. Bu, uygulanan dokuzuncu hesapla karıştırılmamalıdır. Yardımı kapatma veya bu Orléans anlaşması örnekte ihtiyacı azaltmış, tamamen kaldırmamıştır; sufficient iptali için ayrı gerçek rota root tarafından hazırlanır.

Test kaynakları ve bütün owned Assets SOURCE FREEZE olarak root'a teslim edildi. Sayısal NUnit sınır fixture'ı, gerçek ilk açlıktan sonra kontrollü koşullar kurar: bütün bölgeler60 huzursuzlukta Production134; kamptaki zarar sonrası133.1890/1920 asker için aday ihtiyaç40/41 olur. Bu doğrudan state ayarlı sınır testi, yukarıdaki API-only rota veya gerçek player kanıtı diye sunulmaz. Tam Unity ve görsel oyuncu kapısı root'tan ayrıca beklenecek.
