# Sürekli ordu mevcudu — arayüz tasarımı

6 Eylül 2026. Root'un onayladığı A kuralı için **yalnız tasarım**; Assets/kod değiştirilmedi, Unity/player/derleme başlatılmadı. Son GREEN checkpoint `c329851` bu henüz uygulanmamış arayüzü içermez. Temel: `next-country-decision.md`, mevcut CabinetHud ekonomi/konsey belgeleri, gerçek 1440×900 atlas ve Dumas belgesi incelemesi. Son alan adları ve değerler Core sözleşmesinden alınmalıdır.

## Üç farklı sunum

| Sunum | Atlas içindeki deneyim | Bedeli |
| --- | --- | --- |
| **A — Asker mevcudu belgesi** | Mevcut sağ kâğıt panelde düzenlenebilir hedef ve yukarıdan aşağı **şimdi → ilk ayrılış → hedef**. İnce mürekkep çizgileri, mevcut serif başlıklar; sayı/süre/sonuç aynı okuma akışındadır. Hesaplar içinden açılır, atlas ve hafta düğmesi görünür kalır | 238 px metin alanında üç sütun kullanılmaz. Tam şartlardan sonraki emir için yaklaşık bir ekranlık kaydırma gerekir. **Önerilen** |
| **B — Katlanmış personel cetveli** | Oyuncu isterse atlas üstüne açılan geniş, açık renkli yatay evrak. Satırlarda asker, insan gücü, maaş/teçhizat ve iaşe; sütunlarda şimdi/ilk ayrılış/hedef. Hedef ve tek imza altta; ana akış dışında zorunlu vade ekranı yok | Karşılaştırma güçlüdür ama haritayı örter, yeni açma/kapatma odağı oluşturur ve mevcut mandate geniş evrakı ile benzer bir ikinci kalıp ekler. Bu dar karar için fazla alan tüketir |
| **C — Haritadaki ordu kaydı** | Mevcut ordu işaretine bağlı küçük kâğıt etiketinde hedef ayarlanır; ayrılacak kişi sayısı haritadaki asker kaydından insan gücüne bağlanan tek çizgiyle gösterilir. Gider ayrıntısı mevcut Hesaplar'a yönlendirir | Bölgesel yürüyüş ile ulusal personel politikasını karıştırabilir. Kararın bedelini iki yere böler; kuzey/güney konumunda konsey veya coğrafya etiketlerini örtebilir. Yeni harita input/yerleşim işi gerektirir |

**A seçimi:** Bu bir Dumas teklifi veya anlık terhis satın alımı değil, hükümetin sürmekte olan personel emridir. Bu yüzden yeni portre, uzun birinci şahıs alıntısı, gösterge kartları veya koyu renkli veri panosu eklenmez. Kâğıt arka plan, orman mürekkebi, küçük mat altın etkin-hedef çizgisi; yalnız gerçek askerî/siyasi kayıp için mevcut sıcak kırmızı kullanılır.

## Yer ve okuma sırası

Mevcut dört sekme korunur. **Hesaplar'ın kısa girişinden sonra** tek38–42 px `Военный штат / Ordu mevcudu` düğmesi; etkin politika varsa altında yalnız hedef ve sıradaki tarih. Bu giriş, aşağıdaki uzun maaş defterinin veya Konsey'de zaten bulunan zafer/Dumas/bölgesel tekliflerin arkasına gömülmemeli. Belge açıldığında Hesaplar sekmesi seçili görünür ve ilk satır `← К счетам / ← Hesaplara` olur. Konseyde ikinci uzun özet gerekmez; daha sonra istenirse yalnız aynı belgeye kısa bağlantı eklenebilir.

Mevcut sağ viewport `(1156,201,278,584)`, içerik251 px, metin238 px kullanılır. Aşağıdaki yükseklikler tasarım bütçesidir; gerçek `CalcHeight` ile büyür, sabit kutuda kesilmez:

1. **Giriş ve hedef, yaklaşık200 px.** Geri dönüş30 px; tek serif başlık; “En çok200 kişi, iki haftada bir insan gücüne döner” kuralı bir kısa paragraf. Hedefin düzenleme satırı `[−200] [800] [+200]`, toplam238 px; örnek genişlikler62/102/62 ve aralarda6 px. Altında dört eşit küçük öneri `0 · 400 · 800 · 1200`; mevcut hedef bunlardan farklıysa ortadaki tam sayı aynen gösterilir. Hedefin canlı asker sayısından büyük olması otomatik takviye anlamına gelmez. Core'un kabul sınırları ve kaynak nedenleri UI tarafından yeniden uydurulmaz.
2. **Üç ardışık kayıt, yaklaşık210–250 px.** Kart yerine ince ayırıcı ve açık başlık: “Şimdi”, gerçek tarihli “İlk ayrılış”, “Hedef”. Her kayıtta asker sayısı; ilk ayrılışta gerçek üst sınırdaki kişi→insan gücü; şimdi/ilk ayrılış için maaş+teçhizat ve gıda haftalık giderleri. Hedef satırı yalnız hedef asker sayısını ve mevcut fazlayı gösterir; son Core sözleşmesi tam hedefteki gider veya bitiş tarihi vermediği için UI bu iki değeri hesaplamaz. Sayılar iki kısa metin satırına bölünür;238 px içine üç kolon sıkıştırılmaz. İlk ayrılışın hemen altında **eski mevcutla hesap tamamlanır, ilk düşük gider sonraki tarihte** bilgisi verilir.
3. **Kararın bedeli ve tek emir, yaklaşık150–220 px.** Gerçek ilk grubun Dumas ilişki etkisi, daha küçük ordunun muharebeye gireceği ve hedef0 ise garnizon kaybı. Ardından `Ввести сокращение / Azaltmayı başlat` düğmesi; altında tek kısa devam kuralı: durdurmak ayrılanları bedelsiz geri getirmez. Başlatma düğmesi bütün ilk sonuçların arkasındadır; ayrı bir tekrarlı onay modalı açılmaz. Toplam normal içerik hedefi yaklaşık600–720 px,0 hedefinde ek açıklamayla biraz daha uzun; üç ekranlık metin kabul edilmez.

Hedef seçicileri **yerel taslak** değiştirir; bu düğmelere dokunmak kampanya emri vermez. Başlatmak/güncellemek ayrı açık eylemdir. Dil değişiminde, aynı belge içi kaydırmada ve kaynak yenilemesinde taslak korunur; belge kapatılınca sessizce emir uygulanmaz. Aktif hedefi göstermek için küçük `Действует / Yürürlükte`, farklı taslak için `Новый предел / Yeni hedef` etiketi yeterlidir; iki tam belge üst üste çizilmez.

## Kesinlik ve örnek karşılaştırma

Başlangıç5 Mayıs1789 /1200 asker, hedef800 örneği, başka hiçbir koşul değişmezse:

| Kayıt | Asker / insan gücüne dönüş | Ordu gideri / iaşe |
| --- | --- | --- |
| Şimdi |1200 |136 livre /40 gıda haftalık |
| İlk ayrılış:19 Mayıs hesabının sonunda |1000 /+200 |19 Mayıs hesabı eski mevcutla; **26 Mayıs hesabından**120 livre /34 gıda, fark16/6 |
| Hedef:800, bu varsayımla2 Haziran sonunda |Toplam+400 |**9 Haziran hesabından**103 livre /27 gıda, fark33/13 |

Bu tablo yalnız tasarım aritmetiğidir, yeni Core/player sonucu veya bu üç sütunun hepsinin UI'ye aktarılma şartı değildir. Son Core sözleşmesi yalnız gerçek **şimdi/ilk grup** giderlerini verir; tablodaki800 asker gideri ve hedef tarihi mevcut UI kapsamına alınmaz. UI aynı rakamları sabitlemez veya bütçe formülünü yeniden üretmez. İlk grup farkı yalnız ordu bileşeninin karşılaştırmasıdır: **“haftalık tasarruf” doğrudan hazine/gıda stoku artışı veya ülkenin açlıktan çıkacağı garantisi değildir.** Gerçek bir sonraki toplam ülke hesabı mevcut Hesaplar'da kalır; belge altındaki geri bağlantı yeterlidir. Aynı bütçe defteri tekrar basılmaz.

Savaş/açlık kaybı, asker alma, teçhizat stoku ve emirler sonraki miktarları değiştirebilir. Hedefe varış için “en erken2 Haziran” denmez: savaş kayıpları hedefi daha önce karşılayabilir. Mevcut sözleşmeyle yalnız kalan fazla asker ve iki haftalık aralık gösterilir. İki haftalık hazırlık sırasında askerler gerçek maaş/iaşe tüketir ve savaş kaybı alır. Dumas girişimi vade hesabında hâlâ eski gerçek ihtiyacı görür; azaltmanın aynı gün geriye dönük kıtlık veya ödeme düzeltmesi yaptığı ima edilmez.

Core ajanının bu plan sırasında teyit ettiği sözleşme: `GetArmyEstablishmentTerms(state)` ve `(state,policyId,targetTroops)` taslak önizlemesi; `CanSet/Set`. Terms `PolicyId/Disposition/ReasonKey/ReasonArgs`, `CurrentTroops`, `TargetTroops`, `DueWeek`, `FirstReducedBudgetWeek`, `WeeksRemaining`, `ExcessTroops`, `NextBatchTroops`, `TroopsAfterBatch`, `ManpowerAfterBatch`, `CurrentArmyCost/ArmyCostAfterBatch`, `CurrentArmyConsumption/ArmyConsumptionAfterBatch`, `DumasRelationshipDelta`, `WillRemoveGarrison`. `campaign/scheduled/at_target/calendar` dalları; hedef≥mevcut otomatik asker almaz ve due oluşturmaz. Tarih0 ise takvim sonu veya vadesiz durum için hayalî tarih basılmaz; gerçek ReasonKey gösterilir. Giderler bugünkü koşullar altında ilk grup karşılaştırmasıdır, iki haftalık bütün devlet simülasyonu değildir.

## Etkin emir ve sınır durumları

- **Etkin plan:** girişte mevcut hedef ve sıradaki kesin tarih korunur; ilk-kayıt bloğu kalan gerçek sayıya göre güncellenir. `Изменить предел / Hedefi değiştir` yalnız değişen geçerli taslakta etkinleşir. Core'un verdiği gerçek tarih kullanılır; UI değiştirme işleminin iki haftayı yeniden başlattığını/öne aldığını kendiliğinden söylemez.
- **Durdurma:** son bölümde ikincil `Остановить сокращение / Azaltmayı durdur` ve tek cümle: “Yeni ayrılışlar durur; ayrılanları geri almak normal asker toplama bedeli ister.” Anlık Troops artışı, bedava geri alma veya stok iadesi görseli yoktur. İptal tuşunun adı `Отменить` gibi taslağı kapatmakla karışan tek kelime olmaz.
- **Hedefe ulaşılmış veya Troops≤hedef:** “Hedef sağlandı; şimdi kimse ayrılmıyor.” Negatif çıkış, Dumas−4 veya var olmayan tarih gösterilmez. Politika sürüyorsa daha sonraki normal asker toplamanın hedef üstü yeni ayrılış planlayabileceği bu dalda tek cümleyle söylenir. Bu cümle bütün normal belgelerde yinelenmez.
- **Hedef0:** öneri düğmesinde yalnız0 sayısı, seçilince açık `Без гарнизона / Garnizon yok` satırı. Son asker ayrıldığında mevcut bölge ordunun haftalık huzursuzluk−3 / kontrol+2 katkısını kaybeder; bölge adı o anki ordu bölgesinden okunur, imzayla sabitlenmiş bölge gibi sunulmaz.0 asker gıdası doğru olabilir, fakat teçhizat gideri stok koşuluna bağlı olduğundan bütün askerî masrafların0 olduğu söylenmez. Recruit normal yolu ve bedeliyle geri dönülebilir; gizli oyun sonu iddiası yoktur.
- **Az miktar / kapasite / eski engeller:** kalan50 kişinin çıkışı gerçek50→insan gücü olarak görünür; Dumas tepkisi yalnız gerçekten ayrılan grupta, clamped gerçek delta ile. İnsan gücü kapasitesi veya petition/mandate gibi aktif bir engel varsa ilgili API'nin gerçek nedeni düğmenin hemen üstündedir. Neden giderilmeden görsel seçici geri bildirimi “emir verildi” demez.

## Kısa RU/TR dili ve gerçek kabul

| Amaç | RU | TR |
| --- | --- | --- |
| Başlık |Военный штат |Ordu mevcudu |
| Seçici |Предел постоянной армии |Sürekli ordunun hedefi |
| İlk olay |Первый выход — {date} |İlk ayrılış — {date} |
| Kaynak değişimi |В строю остаётся {troops}; в запас +{released} |Orduda {troops}; insan gücüne +{released} |
| Gecikmiş ekonomi |Этот расчёт — по старому штату. Экономия с {date}. |Bu hesap eski mevcutla. Tasarruf {date} hesabından. |
| Gelecekte kalan bedel |В следующий бой пойдёт меньше солдат. |Sonraki savaşa daha az asker gider. |
| İlişki |За эту группу: отношения с Дюма {delta}. |Bu grup için Dumas ile ilişki {delta}. |

“Запас” metni mevcut üst Manpower adıyla eşleştirilmelidir; yeni ayrı asker deposu icat edilmez. Yer tutucular son localization biçimine uyarlanır; çevrilmiş metinde fiyatlar/kurallar ikinci kez kodlanmaz.

Root'un uygulamasından sonra gerekli görsel/native inceleme: RU/TR ilk açılışta hedef ve ilk tarih okunur; alt kaydırmada bütün gerçek bedeller ve tek başlatma düğmesi görünür. Normal1200→800, hedef0, son kısmi grup ve kapasite reddi özellikle görülür. Gerçek fareyle önce öneri/±200 seçip emri vermeden kapatma kampanyayı değiştirmemeli; tekrar açma, başlatma, dil değiştirme, kaydet/yükle, ilk çıkış ve aktif planı durdurma incelenir.19 Mayıs örneğinde çıkıştan önce eski gider, sonraki hesap için azalan gider ayrımı görülür;0 hedefinde garnizon metni atlanmaz. Native ölçümde iki ana karar düğmesi ayrı ve tam görünür; sağ kaydırma atlas veya haftayı yanlışlıkla tetiklemez. Mevcut forecast ve Core kontrolleri ayrıca root/verification sorumluluğudur; bu tasarım dosyası bunların geçtiği iddiası taşımaz.

## İlk kaynak UX incelemesi — gerçek kare öncesi

Root'un `CabinetArmyEstablishment.cs`, `establishment-ui.json`, `establishment-core.json` ve çağrı/çekirdek sözleşmesi okundu. Root Runtime25 statik derlemesini PASS bildirdi; bu ajan çalıştırma yapmadı, Assets değiştirmedi. Yeni player karesi henüz görülmedi.

**Doğru kurulanlar:** Düğme Hesaplar girişinin hemen ardından; etkin alt belge Hesaplar sekmesini koruyor. ±200 ve öneriler yalnız `establishmentDraft` ile preview/check'i değiştiriyor; gerçek Set çağrısı ayrı. State değişimi/politika değişimi gözlemi var, salt dil değişimi taslağı sıfırlamıyor. Gerçek `NextBatchTroops`, `DueWeek`, `FirstReducedBudgetWeek`, giderler ve clamped Dumas deltası ortak terms'ten; sahte iki haftalık ülke neti veya hedef bitiş tarihi yok. Vade hesabının eski mevcutla ödendiği açık.0 uyarısı, son askerlerin ayrılışı ve bir sonraki hesapta garnizon etkisinin kalkışı sırasını doğru kuruyor; hareket eden mevcut ordu bölgesini adlandırıyor. Sayı ve buton genişliklerinde kaynak üzerinden bariz taşma yok; Paragraph yüksekliği metne göre büyüyor.

**Öncelikli üç dar düzeltme:**

1. **Koşul ve tekrar eden bedel dili.** `ui.establishment.conditions` RU “Потери уменьшат уходящую группу” / TR “Kayıplar ayrılan grubu küçültür” kesinliği yanlış:1200 asker/hedef800 iken100 kayıp olsa da ilk grup200 kalır. “Могут изменить / değiştirebilir” denmeli. “В следующий бой…” da vade öncesindeki savaşı kapsayarak erken küçülme sözü verir; “После выхода… / Ayrılıştan sonra…” ile bağlanmalı. Öneri RU: “Расчёт при нынешнем составе и запасах. Потери могут изменить группу. После выхода в строю останется меньше солдат.” TR: “Hesap bugünkü mevcut ve stoklarla. Kayıplar grubu değiştirebilir. Ayrılıştan sonra orduda daha az asker kalır.” Dumas metni yalnız ilk grubun bedelini veriyor; her gerçek grupta yeniden ilişki kaybı uygulandığı da açık olmalı. Örneğin gerçek ilk delta yanında “За каждую фактически ушедшую группу — до4 отношений” / “Fiilen ayrılan her grup için ilişki en çok4 azalır”; Core sabiti/gerçek delta kullanımı root tarafından bağlanmalı. İlk deltayı bütün gelecek gruplara aynen yaymak, ilişki tabanı0 yüzünden doğru değildir.
2. **Etkin emir ile farklı taslak birlikte görünmeli.** Satır54–56 aktif hedefe eşitken “ДЕЙСТВУЮЩИЙ ПРЕДЕЛ”, değişince yalnız “ЧЕРНОВИК” yazıyor; mevcut800 emri varken1000 taslağına geçildiğinde gerçek800 ve sürmekte olan tarih belgeden kayboluyor. Seçicinin üzerinde, yalnız active+draftDifferent dalında, `establishmentCurrent` değerlerinden tek kompakt satır yeterli: “Действует800 · ближайший выход19мая” / “Yürürlükte800 · sıradaki ayrılış19Mayıs”. Due0 ise yalnız etkin değer. Aynı tam şartları ikinci kez basmak gerekmez.
3. **Geçersiz taslak mevcut emrin durdurulmasını saklamamalı.** Satır75–79 `terms==null` olduğunda erken return ediyor; active planın durdurma düğmesi114–119'a ulaşılamıyor. Kapasiteye sığan etkin hedef varken daha düşük, kapasiteye sığmayan taslak bunu gerçekçi biçimde tetikleyebilir. Hata/selektörler kalabilir, fakat mevcut planın stop+bedelsiz geri dönüş yok açıklaması preview geçerliliğinden bağımsız çizilmeli. Mevcut Set API'sinin yetkisini değiştirmek gerekmiyor; yalnız mevcut izinli kontrol kaybolmamalı.

İki küçük anlam ayrıntısı da root'a iletilebilir: aktif ikinci/üçüncü grupta “ilk çıkış” yerine **“ближайший выход / sıradaki ayrılış”** daha doğru; bu niteleme giriş/ilk satır/Dumas ve ilgili Core açıklamalarında aynı olmalı. Troops zaten0 ve draft0 iken gelecek zamanlı “son asker ayrılınca” uyarısı, mevcut garnizon yok durumuna kısaltılabilir; bu dal bugün yeni bir sıfıra düşüş yaşanıyormuş gibi sunulmamalı.

**Yalnız kareyle doğrulanacaklar:** kaynak yüksekliği hesabı normal aktif belgenin yaklaşık1.3–1.5 viewport, hedef0'ın biraz daha uzun olabileceğini gösteriyor; kesin ölçüm değildir. Önce hedef/tarih, kaydırdıktan sonra tam bedeller ve eylem görünmeli. Şu aşamada “kesiliyor”, “buton erişilemiyor” veya bütün RU/TR sığıyor sonucu çıkarılmadı. Farklı taslak/etkin değer,0 hedefi ve kapasite reddinde stop yolu gerçek native incelemenin somut ek durumlarıdır.

### Root düzeltmesinden sonra ikinci kaynak bakışı

Üç ana değişiklik gerçek kaynakta tekrar okundu: koşullar ilk grubun önünde ve olasılıklı; her gerçek grup için en çok4 ilişki kaybı ile yakın grubun exact deltası ayrı; farklı taslak üstünde current hedef/due korunuyor. `ArmyEstablishmentStop` hem normal footer'dan hem null-preview erken dönüşünden önce çağrılıyor; ayrı `establishmentStopCheck` geçersiz taslak yüzünden saklanmıyor. Kontrol yüksekliği +50 ve son açıklama `documentContentHeight` hesabına katılıyor. Salt kaynak incelemesinde kalan bir durdurma engeli görülmedi.

Başka Assets değişikliği önerilmedi. İki küçük zaman kipi/niteleme ayrıntısı root'a gönderildi: giriş/başlıkta “ilk” yerine “sıradaki”, Troops zaten0 olduğunda mevcut garnizon-yok ifadesi. Active+farklı draft+0 en uzun görsel durum; tam butonlar ve son açıklama için alt scroll karesi hâlâ gerekli. Bu ikinci okuma da görsel/native kabul değildir.

## İlk gerçek player incelemesi — 03:00 UTC civarı

Root bu iki küçük kopya düzeltmesini de uyguladı. `output/verify/army-establishment-first-20260906-025643-395-97035b66/REPORT.md` gerçek GREEN:304/304 Unity, fresh build,13 PNG /71 assertion /10 state /10 browser,41 saniye. Bu ajan ayrıca gerçek `04-scheduled-ru`, `07-target-reached-ru`, `08-recruit-new-deadline-tr`, `09-retarget-same-deadline-tr`, `10-stopped-policy-ru` karelerini gördü; root'un00/01/03 incelemesi ayrı kanıttır. Assets değiştirilmedi.

- 04'te etkin1000 hedefi, mevcut1200 /136 livre /40 gıda,19 Mayıs çıkışı ve26 Mayıs düşük gideri açıkça ayrılır. Dumas paragrafının altı doğal viewport sonunda devam ediyor; yatay metin kaybı yoktur.
- 07'de hedefe ulaşılmış1000, gider120/34, şimdi kimsenin ayrılmadığı ve yeni işe alımın hazırlığı yeniden başlatacağı tam okunur. Güncelleme pasif, durdurma görünür; dönüşün normal bedeli hemen altında. Üst kaynaklar bu tek karede geçiş animasyonundadır: ordu1006, belge1000 gösterir. Kaynakta Top `Animated` kullanıyor ve fixture burada yalnız0.3s bekliyor; bu Core tutarsızlığı kanıtı değildir. Son statik karşılaştırmada yaklaşık0.6s beklemek önerildi.
- 08 TR: yeniden alınan1200, sıradaki2 Haziran ve9 Haziran gider ayrımı, yakın grupta+200/ilişki−4 okunur.09'un alt görünümünde iki eylem düğmesi ve stop açıklaması bütünüyle görünür; yeni hedef900, aynı2 Haziran tarihi ve mevcut fazlalık300 doğru bağlamdadır. Belge yaklaşık bir kaydırmayla okunur; yeni büyük panel veya kısaltma gerektiren taşma saptanmadı.
- 10 **küçük gerçek bağlam sorunu** gösterir: politika durdurulduktan sonra belge yeni1000 taslağını sunar. Üstteki `ЧЕРНОВИК` taslak etiketi scroll dışında kalınca “Ближайший выход — 2 июня” satırı yürürlükteki bir emir gibi okunabilir. `Ввести предел` düğmesi bunun öneri olduğunu sonradan açıklar, fakat tarih kendi başına koşullu görünmelidir. Inactive veya farklı draft dalında aynı satırı “По черновику — выход {date}” / “Taslak yürürlüğe girerse — {date}” yapmak önerildi; yeni paragraf/ekran gerekmez. Etkin ve değişmemiş hedefte normal “Ближайший / Sıradaki” kalır.

Root ayrıca RU “4 отношений” yerine “4 пунктов отношений” ve TR yeni “insan gücü” teriminin üst kaynak adı “Yedek” ile birleştirilmesini fark etti. Bu terminoloji değişikliği henüz bu karelerde yoktur; kullanıcıya ikinci bir kaynak havuzu varmış hissi verilmemelidir.0 hedefinin gerçek kare incelemesini root ayrı koşuda yürütüyor; bu beş görüntü onun yerine geçmez.

## Son birleşik gate — gerçek kare kabulü

`output/verify/army-establishment-final-20260906-030602-688-f7c2fdcb` raporu: **GREEN304 Unity /37 PNG /259 assertion /33 state /10 browser**, fresh build,75s. Bu ajanın ayrıca gördüğü tam1440×900 kareler: `first-01/02/03`, `linked-02/04/07/08/09`, `zero-02/03`. Root `zero-10/11` ordu katmanı ve `first-10` durdurulmuş taslak düzeltmesini ayrıca gördü. Tüm37 karenin bu ajan tarafından incelendiği veya ses dinlendiği iddia edilmez.

- **Yeni taslak kopyası kabul edildi.** First01 RU alt görünümde “По черновику — выход19мая” doğrudan tarihin yanındadır; üst etiket scroll dışında olsa da geçerli emir gibi sunulmaz. First02 TR üst görünümde “YENİ HEDEF · TASLAK”, first03 altında “Taslak yürürlüğe girerse” korunur. Başlatma düğmesi ve dönüşün normal bedeli iki dilde de tam görünür. Yeni transferde `yedeğe`, üstte `Yedek` aynı kaynak anlamını taşır. RU ilişki cümlesi doğal “Отношения … падают на4…” biçimine dönmüş; gerçek yakın delta ayrı kalır.
- **Borç küçülen ordudan bağımsız anlaşılır.** Linked02 RU'da ilk söz80livre, Île-de-France ve11Ağustos1789 şartını gösterir. Linked04 TR vade mektubunda yine Hazine−80, aynı asıl bölge ve tarih vardır; asker sayısı1272'ye düşmüşken borç yeniden fiyatlanmış gibi görünmez. İki çözümün bedelleri, stok satırı ve düğmeleri metin kesilmeden görünür; Dumas portresi/başlıkla çakışma yoktur.
- **Koşullu grup gerçek kayıpla tutarlı okunur.** Linked07,18Ağustos'ta1170 asker, hedef1000, o andaki170 fazla ve25Ağustos yakın çıkışını gösterir; gider134/39→120/34, düşük giderin1Eylül'de başlayacağı yazılıdır. Koşullu kayıp açıklaması aynı bölümde kalır. Linked08/09 RU/TR günlükleri25Ağustos'taki94 firarı ve ayrı76 kişilik gerçek rezerv dönüşünü birlikte gösterir:1170−94−76=1000. Geçen hesabın eski mevcutla ödendiği, Dumas−4 ve yeni erzak girişimi birbirine karışmayan ayrı kayıtlardır. Kayıtlar normal scroll içinde devam eder; beklenmeyen yatay kesilme yoktur.
- **Tam sıfır uyarısı kabul edildi.** Zero02 RU ve03 TR'de hedef0, henüz1200 mevcut, ilk+200 grup ve her gerçek grubun siyasi bedeli görünür. Son askerler ayrıldıktan sonraki garnizon kaybı, haftalık huzursuzluk−3/kontrol+2 katkısının kalkışı ve mevcut konaklama bölgesi bütün olarak okunur. İki düğme ile iptalin ücretsiz asker döndürmediği son açıklama aynı alt görünümde tamdır. Kırmızı uyarı yoğunluğu belgeyi koyu bir panele dönüştürmüyor.

Bu görülen son karelerde ek UI/Assets düzeltmesi önerilmedi. Kapasite-null taslağın gerçek native stop yolu gibi önceki kaynak kabulü sınırları ayrı kalır; bu görüntüler çalıştırılmamış bütün olası durumları otomatik onaylamaz. Yeni kontur planı yalnız aday olarak kabul edilmiştir, burada uygulanmadı.
