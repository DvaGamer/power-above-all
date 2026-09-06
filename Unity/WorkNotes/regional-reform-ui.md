# Bölgesel yatırım — dar belge düzeni

6 Eylül 2026. Yalnız UI tasarımı; root B ekonomik uzmanlaşmasını seçti. `next-regional-reform.md`, `regional-reform-contract.md`, mevcut `CabinetHud.Province/Economy/Cabinet` ve ordu/anlaşma belgelerinin girişleri okundu. Gameplay ajanı actual karşılaştırmaların **ülke toplamı** olduğunu ayrıca doğruladı. Assets değiştirilmedi; test/derleme/oyuncu/Git çalıştırılmadı. API'nin takvim sonu ayrıntıları root onayındaydı; aşağıdaki düzen tarihi kendisi üretmez.

## Seçilen sunum ve girişler

Üç küçük sunum değerlendirildi: A iki yönü aynı anda iki uzun fiyat sütunuyla göstermek; B seçilen yönün tek kâğıt hesabı; C önce sponsor seçip sonra ayrı şartlara geçmek. 238 px'de A sayıları daraltır, C aynı basit karara gereksiz ikinci gezinme ekler. **B seçilsin:** mevcut sağ belge ve scroll içinde iki kısa yön düğmesi; yalnız seçilen yönün tam şartları. Yeni sekme, modal, portre veya görsel varlık yok. Mevcut krem kâğıt, koyu mürekkep; bekleme/iptal bedelinde mevcut şarap rengi yeterli.

Oyun adı: **«Хозяйственный проект» / «Bölge yatırımı»**. Yönler **«Снабжение» / «İaşe»**, **«Торговля» / «Ticaret»**. Seçim henüz emir değildir; yalnız şartları değiştirir. İlk yön varsayılan olarak iaşe olabilir, fakat seçili durumu açık görünmeli. Sponsor tek satırda mevcut adı ve ilişki değeriyle yer alır; 96 px portre bloğu eklenmez.

- **Economy:** mevcut `ArmyEstablishmentEntry` hemen sonrasında bir 238×39 giriş; başlık «Хозяйственный проект →» / «Bölge yatırımı →». Altında tek kısa durum satırı: kapalıysa seçilen bölge; açıksa asıl bölge + hazırlanıyor/çalışıyor. Mevcut dört ana sekme değişmez, bu belge Economy altında seçili sayılır.
- **Bölge raporu:** mevcut BaseTax/BaseFood satırlarının sonrasında, ordu raporundan önce 195 px genişliğinde giriş. İlk ekmek/vergi/alım/yürüyüş/subsidy konumları değişmez; mevcut tek province scroll uzar. Kapalıysa «Проект этой области →» / «Bu bölgeye yatırım →». Açık proje başka yerdeyse giriş adı gerçek yeri söylemeli: «Проект: {область} →» / «Yatırım: {bölge} →». Yanlışlıkla seçili bölgenin başvurusunu imzalanmış gibi açmamalı.
- İki giriş aynı sağ belgeyi açar. Kapalı projede selected region okunur; pending/blocked/active durumda yalnız `GetTerms(state)` asıl bölge/modu kullanılır. Haritadan farklı bölge seçmek açık projeyi veya modunu değiştirmez. İstenirse yalnız farklı seçiliyken küçük «Показать область» / «Bölgeyi göster» bağlantısı gerçek proje bölgesini seçer.

## Belge sırası: aynı 238 px sütun

Mevcut viewport 584 px yüksekliğinde, content genişliği 251 ve okunur satır genişliği 238 px. Yalnız düğmelerin yükseklikleri sabit; bütün metinler mevcut `Paragraph/CalcHeight` ile ölçülür. Beş basamaklı gelir veya uzun TR bölge adı için dar iki sayısal sütun kullanılmaz. Normal öneri için yaklaşık 600–740 px toplam hedeflenir; bütün şartları okuyup son eyleme ulaşmak küçük bir scroll gerektirebilir. Font küçülterek tek ekrana zorlanmaz. Gerçek yeni PNG olmadan bu yükseklik gerçekleşmiş sayılmaz.

| Sıra | İçerik / boyut | Sözleşme ve amaç |
| --- | --- | --- |
| 1 | 238×30 «К счетам» / «Hesaplara»; 8 px boşluk; başlık; bölge adı | Bölge adı her durumda üstte. Pending/active belge açıkken seçili harita başka yer olsa da bu isim değişmez. |
| 2 | Yalnız öneride iki 115×36 düğme, arada 8 px; sponsor satırı | Mevcut seçili düğme stili. İaşe/commerce iki alternatif, iki bağımsız eşzamanlı proje değil. Açık projede aynı alan kısa salt okunur yön/durum olur; başka modun hayalî yeni önerisi gösterilmez. |
| 3 | Kısa koşullu karşılaştırma başlığı ve üç metin satırı | **Ülke genelinde, bugünkü şartlarla: projesiz → projeli.** Vergi `WithoutTax → WithTax (delta)`; üretim `WithoutProduction → WithProduction (delta)`; gıda dengesi `WithoutNetFood → WithNetFood (delta)`. Üçüncü satır üretimin aynı miktarda depoya girdiği yanılgısını engeller. |
| 4 | Hazırlık veya çalışma durumu; koşullu tarihler | Öneride dört uygun hesap; pending'de gerçek kalan sayı; blocked'da gerçek WaitReason; active'de bileşim zaten etkin ve yalnız takvim içindeyse sonraki bütçe tarihi. |
| 5 | Başlangıç bedeli ve sponsor sonuçları | Öneride Gold−120 / Power−4, tamamlanmanın bugünkü ilişki tahmini, ileride geri alma bedeli. Pending/active'de 120/4 yeniden ücret gibi gösterilmez. |
| 6 | Gerçek ret nedeni; 238×42 tek ana eylem | Öneride «Начать проект» / «Yatırımı başlat»; hazırlıkta «Отменить подготовку» / «Hazırlığı iptal et»; active'de «Отозвать право» / «Yetkiyi geri al». İptal sonucu ilgili düğmeden **önce**, aynı son blokta. |

Ekonomi başlığı için RU: `При нынешних условиях, за неделю: без проекта → с проектом.` TR: `Bugünkü koşullarla, haftalık: yatırımsız → yatırımlı.` Bu sayı gelecekteki kesin kazanç veya bölgenin kendi kasası diye sunulmaz. `CurrentTaxIncome→WithTax` kullanılmaz: active'de aynı sayı olur ve gerçek katkıyı sıfırlar. Nominal tabandaki ±25%/örnek Normandy−8/+5 ana karşılaştırmaya yazılmaz; actual değerlerin yerine geçemez.

With/WithoutForageFood farklıysa yalnız o durumda kısa açıklama: RU `Сбор Дюма: {без} → {с}; это учтено в балансе хлеба.` / TR `Dumas'nın topladığı erzak: {yok} → {var}; gıda dengesine dahildir.` Sayısal değişim sıfırsa değişmiş gibi davranılmaz. Mevcut vergi tatilinin ticari actual farkı sıfırlaması dürüstçe 0 görünür; dört hafta sonraki gelir tahmin edilip doldurulmaz.

## Zaman, koşul ve siyasi bedelin kesin anlamı

Hazırlık açıklaması bir kez verilir:

- RU `Нужно {n} подходящих недель: в конце каждой волнения <{u}, контроль ≥{c}.`
- TR `{n} uygun hafta gerekir: hafta sonunda huzursuzluk <{u}, denetim ≥{c}.`

Sayı/eşik Core'dan gelir. Bugünkü uygunluk gelecek hesabın kesin ilerlemesi değildir; `WaitReason` mevcut koşul olarak adlandırılır. Pending satırı RU `Подходящих недель осталось: {n}.` / TR `Kalan uygun hafta: {n}.` Ayrı sıfırdan başlayan animasyon/ilerleme grafiği gerekmez. Başarısız week komutu ilerleme gibi gösterilmez; gerçek hafta kapısı varsa o neden de eylem öncesinde görünür.

İki en erken tarih tek kısa paragrafta: RU `Если условия сохранятся: готово не раньше {activation}; первый новый бюджет — {budget}.` / TR `Koşullar korunursa: en erken tamamlanma {activation}; ilk yeni bütçe {budget}.` Bu, dördüncü hesabın eski bütçe olduğunu açıklayan ikinci uzun paragrafın yerini alır. Tarih−1 ise sahte tarih yok; Core'un takvim/ret bağlamı gösterilir. Active durumda geçmiş tamamlanma tarihi saklanmadığı için uydurulmaz.

**Öneri, düğmeden önce:** `Казна −{GoldCost} · власть −{PowerCost}` / `Hazine −{GoldCost} · iktidar −{PowerCost}`. Tamamlama ilişkisi bugünkü koşula bağlanır: RU `По завершении: {sponsor} {actualDelta} при нынешних отношениях.` / TR `Tamamlanınca: bugünkü ilişki düzeyinde {sponsor} {actualDelta}.` İptal daha sonra yapılacağı için onun bugünkü clamp değeri gelecek kesin bedel diye sunulmaz: RU `Отмена: до 8 пунктов отношений; сейчас это {actualEndDelta}. Плата за запуск не возвращается.` / TR `İptal: en çok 8 ilişki puanı; bugünkü düzeyde {actualEndDelta}. Başlangıç bedeli iade edilmez.` 8 veya diğer maliyetler UI'nin ayrı denge tablosuna kopyalanmamalı; Core sabiti/terms sözleşmesi kullanılmalı.

**Pending/blocked:** tamamlanma ilişki tahmini hâlâ koşulludur. Son blok actual iptal ilişkisini gösterir; yeniden 120/4 istenmez. **Active:** bitmiş ilişkinin geçmişte gerçekten kaç arttığı DTO'da yok, bunu tekrar +4 ödül gibi göstermeyiz. Mevcut sponsor ilişkisi ve bugünkü geri alma sonucu yeterli. İki açık durumda son metin: RU `Отмена сейчас: {sponsor} {actualEndDelta}. Доплаты нет; уплаченные 120 и 4 власти не возвращаются.` / TR `Şimdi iptal: {sponsor} {actualEndDelta}. Ek ödeme yok; ödenen 120 ve 4 iktidar iade edilmez.` Sayılar terms'ten. Active ekonomik sonuç ayrıca bir kısa satırla: `Следующий расчёт вернётся к обычному производству; прошлые запасы сохранятся.` / `Sonraki hesap normal üretime döner; önceki stoklar korunur.`

Aktif/pending projeyi başka bölgeye/moda dönüştüren düğme yok. İptalden sonra gerçek boş durum oluşur; yeni imza yeniden120/4 ve dört uygun hesap ister. Sıfır kaynak nedeniyle iptal düğmesi kapanmaz; mevcut petition/due mandate gate retleri varsa dürüstçe gösterilir.

## Gerçek RU/TR kabul kareleri

1. Aynı bölgenin iki önerisi: üstte seçili yön, ülke toplamı/actual fark, aşağıda bütün fiyat ve iptal şartları; düğme bunlardan sonra. TR uzun metinlerde üst/alt iki kare. Eksik Gold/Power fiyatı veya karşılaştırmayı saklamamalı.
2. Hazırlık sırasında başka bölge seçimi ve wait durumu: asıl isim/mod/kalan sayı değişmez; U/C sınırı ve koşullu tarihler okunur. UI uygun bugünkü değerleri kesin hafta kazanımı diye söylemez.
3. İlk active bütçe, vergi tatili veya Dumas farkı: katkı actual Without→With; gıda üretimi ve stok dengesi farklıysa ikisi de görünür. Önceden tamamlanmış ilişki artışı yeniden kazanılacak ödül değildir.
4. Pending/active iptal, sponsor ilişkisi0 ve100, sıfır Gold/Power: actual clamp sonucu düğmeden önce; harcanmış120/4 iade edilmez; yeni modal çıkmaz. İptal sonrası yeni bölge/mode ancak normal yeni ödeme ile.
5. Paris'in en uzun emir listesi: yeni bölge girişi alt raporda; ilk mevcut emirlerin koordinatları/bedelleri değişmez. Economy yeni girişleri viewport dışındaki tek ortak scroll içinde; karanlık dashboard veya beşinci ana sekme oluşmaz.

Bu belge yalnız uygulanacak yerleşim önerisidir; source veya native görüntü kabulü değildir. Nihai metin/API değerleri root'un uygulamasından sonra gerçek RU/TR karelerle doğrulanmalıdır.

## İlk kaynak incelemesi — root uygulaması

`CabinetRegionalReform.cs`, CabinetHud giriş/cache/dispatch bağlantıları, GameApp Begin/End çağrıları ve `regional-reform-ui.json` okundu. Bütün inceleme salt okunur; Assets ve süreçler yalnız root'un sahipliğinde.

Doğru uygulananlar: proposal sorgusu yalnız closed durumda selected region/mode için kuruluyor; pending/blocked/active `reformCurrent` asıl bölgeyi tutuyor. Açık projede iki mod seçimi çizilmiyor. Actual ekonomi her durumda Without→With; ayrı üretim, gıda dengesi ve değişmiş Dumas toplaması var. Süre koşulları <65/≥55 ve iki en erken tarih Core'dan; tamamlama ilişkisinin gelecekteki clamp'ı koşullu anlatılıyor. Active durumda tamamlanma ödülü yeniden çizilmiyor. Gold/Power yetersizliği şartları saklamıyor; eylem en sonda gerçek Can sonucu ile açılıyor. Mevcut dört sekme ve ilk province emir konumları korunmuş.

Root'a hemen üç somut kusur bildirildi ve root dar patch ile düzeltti; değişen satırlar yeniden okundu:

1. Ortak `conditional` active durumda “Hazırlık bitene dek etki uygulanmaz, o zamana kadar…” diyordu. Artık genel karşılaştırma/koşul cümlesi, geçmişte hazırlık gerektiği bilgisiyle uyumlu.
2. `no_date` henüz başlanmamış öneriye “Proje sona erdirilebilir” diyordu. İkinci cümle kaldırıldı.
3. `exit_proposed` gömülü `−8` kullanıyordu. Root üçüncü argümanda `RegionalReformEndRelationshipLoss` geçiriyor, metin en çok pozitif8 ilişki puanı kaybı ve bugünkü actual delta'yı ayırıyor. Core ayrıca CompletionRelationshipGain=4, UnrestLimit=65 ve MinimumControl=55 public sabitlerini verdi. Seçili yönün düğmesi de artık disabled görünümüne zorlanmıyor.

Kalan dar copy önerisi: `ui.reform.direction.commerce` kesin “daha çok vergi geliri” diyor. Mevcut gerçek vergi tatilinde actual vergi farkı0 olabilir. Sayısal karşılaştırma doğru olsa da sloganı doğrudan değişen tabana bağlamak daha dürüst: RU `Выше налоговая база, меньше продовольствия.` / TR `Daha yüksek vergi tabanı, daha az erzak.` Bu ekonominin bütününde kesin tahsilat artışı vaat etmez.

İlk gerçek karelerde kontrol edilecek iki okunurluk riski:

- Tasarımdaki kısa metin yerine karşılaştırma açıklaması, nominal+direniş paragrafı, koşul/mevcut bölge/bekleme ve iki cümlelik completion art arda geliyor. Normal önerinin hedeflenen600–740 px'den uzun olması beklenir; kesin yüksekliği ölçmedim, clipping iddia etmiyorum. Önce gerçek üst/alt kare alınmalı. Gerekirse `completion` tek koşullu cümleye, nominal açıklama `Хозяйственная база: налоги…; хлеб… Военная база прежняя.` / `Ekonomik taban: vergi…; erzak… Direniş tabanı değişmez.` biçimine kısaltılabilir; sayılar ve askeri ayrım korunur. `WaitReason` iki bugünkü değeri zaten verdiği durumda `region_now` tekrarını kaldırmak da aynı bilgiyi iki kere söylemeyi önler.
- Giriş düğmeleri şu anda hep genel başlık; aktif asıl bölge girişte görünmüyor. Açılmış belgenin üstü doğru ismi veriyor, fakat uzun alt scroll'da iptal düğmesinin yakınında yalnız sponsor var. Başka bölge seçili native karede bağlam kaybı varsa önce son eylem üstüne kısa `Проект: {region}` / `Proje: {region}` satırı eklemek yeterli; başka moda sessiz geçiş veya yeni popup gerekmez. Bu kaynakta yanlış bölge mutasyonu bulunduğu anlamına gelmez.

Kaynak koşullu sayılar/tarihler açısından yukarıdaki üç düzeltmeden sonra ilk build incelemesine hazır. Mevcut sponsorun sayısal ilişki düzeyi öneride yalnız actual delta bağlamında, portrait/dış Council'da ayrıca incelenebilir; onu eklemek zorunlu yeni bir blok olarak önerilmedi. Gerçek yerleşim/kontrast/scroll erişimi henüz kabul edilmedi.

## İlk gerçek oyuncu kareleri — görsel kabul önerisi

`output/verify/regional-reform-first-20260906-060159-696-0035bd7a/shots` içindeki 12 tam1440×900 PNG açıldı:02/03,04/05,06/07,08/09,12/13,16/17. Root aynı paketin diğer RU karelerini inceliyor. Root ilk build496 test,18PNG/72assert/14state bildirdi; son frame/browser kapılarının bu ajan tarafından tamamlandığı veya state JSON'larının bağımsız karşılaştırıldığı ileri sürülmez.

- **02/03 iaşe TR:** seçili İaşe düğmesi koyu zeminde açıkça ayrılıyor. Vergi207→202(−5), üretim152→156(+4), gıda dengesi+2→+6(+4) ayrı ve okunur. Altta dört uygun hafta, U<65/C≥55, bugünkü30/80, en erken2Haziran ve ilk yeni hesap9Haziran, koşullu+4 ilişki,120/4 hemen ödeme ve en çok8/bügünkü−8 iptal bedeli birlikte görülebiliyor. Eylem43px yükseklikte tamamen viewport içinde.
- **04/05 ve06/07 commerce RU/TR:** iki düğme, sponsor ve artık kesin gelir artışı vaat etmeyen vergi tabanı açıklaması okunuyor. Actual vergi207→213(+6), üretim152→148(−4), netgıda+2→−2(−4); nominal32→40/20→15 ayrı açıklama. Alt karede tam fiyat, iadesizlik, yeni bölge/yön için yeni proje bilgisi ve ödeme düğmesi kesilmeden okunuyor. RU uzun düğme metni de tek satırda sığıyor.
- **08/09 özgün bölge:** haritada ve solda Bretagne/Бретань seçili, sağ belge başlığı yine Normandy/Нормандия. Pending3 uygun hafta, doğru özgün bölge U32/C80, önceki en erken tarihler korunmuş. Root'un eklediği `Normandiya · İaşe` satırı alt eylem önünde de görünüyor; kaynak incelemesindeki bağlam kaybı riski bu karede çözülmüş. Hazırlığın ödendiği, tamamlamada ikinci ödeme olmadığı ve bugünkü iptal−8 aynı alt blokta.
- **12/13 active TR:** üstte yeni düzenin etkin olduğu açık. Actual vergi211→205(−6), üretim145→149(+4), netgıda−5→−1(+4); katkı yanlış0'a düşmemiş.9Haziran sonraki hesap, Normandiya·İaşe, iadesiz iptal−8 ve normal tabana dönüş açıklaması altta beraber okunuyor. Eski tamamlanma+4 yeni ödül olarak çizilmiyor.
- **16/17 yeniden commerce:** eski Normandiya yerine gerçek yeni Bretagne/Бретань·Ticaret/Торговля, yeniden4 uygun hafta ve koşullu7Temmuz/14Temmuz tarihleri var. Yeni hazırlığın zaten ödendiği anlatılıyor; ikinci ödeme düğmesi yok, gerçek iptal düğmesi var. Alttaki bölge/mod tekrarı iki dilde de kararın hangi projeye ait olduğunu koruyor.

Görülen document uzunluğu yaklaşık iki ekranlık; önceki600–740px tasarım hedefinden uzun olsa da bu gerçek çiftlerde şart kaybı veya erişilemeyen alt eylem yok. Üst/alt viewport kenarında paragrafın doğal devamının kesilmesi normal scroll davranışı; kesilmiş tek sabit satır veya başka öğenin üzerine taşmış metin bulunmadı. Mevcut font/kontrast okunuyor; bu aday için yeniden layout düzenleme veya küçültme gerekmiyor.

Tek düşük öncelikli dil cilası: RU dinamik kişi adı önündeki `с` çekimsiz tam adla birleşiyor (`с Этьен де Валькур`). Sonraki doğal copy turunda `Отмена сейчас: {sponsor}, отношения {delta}.` gibi adın yalın kalabildiği liste dili kullanılabilir. Bu görsel kabulü durduran bir işlev veya yerleşim kusuru değil.

**Öneri: incelenen12 kare bakımından kabul.** Henüz bu incelemeye dahil olmayan blocked/yetersiz kaynak/son takvim/Dumas özel koşulları için görsel kabul çıkarılmadı. Native tıklama kanıtı da bu PNG incelemesinden türetilmez. Assets değişmedi; Unity/player/derleme/Git başlatılmadı.

## Kısaltılmış son metin — beş gerçek TR kare kabulü

Root'un `regional-reform-final-20260906-060546-199-b6af33b0` paketi GREEN496/18PNG/72assert/14JSON/10browser/141/native0 olarak bildirildi. Bu ajanın son görsel incelemesi yalnız03/07/09/13/17 bottom TR tam kareleridir; hepsi gerçekten açıldı.

Kısaltma yararlı:03/07/09/17'nin üst kenarında artık üretim ve gıda dengesi karşılaştırmasının daha fazlası da görünüyor. Buna rağmen dört/üç uygun hafta, <65/≥55 sınırları, mevcut bölge değerleri, iki koşullu tarih, ilişki kazancının bugünkü düzeye bağlı olması ve son ödeme/iptal bloğu kaybolmadı. Öneride120/4 ile azami8/actual−8 ve iadesizlik; açık projede ek ödeme ve iade olmadığı açık.09/13'te Normandiya·İaşe,17'de yeni Bretagne·Ticaret eylemin önünde kaldı.13 active bütün gerçek ekonomik katkıyı,9Haziran hesabını ve iptal sonucunu tek alt görünümde gösteriyor. Yeni clipping, düğme metni taşması veya okunmaz karşıtlık yok.

**Bu beş son TR kare kabul edilir.** Önceki ilk build'in geçerli görsel bulgularının yerini yanlış state eşitliği iddiası almıyor; bu ajan JSON veya native tıklama testi yapmadı. Bundan sonra sırf metni biraz daha kısaltmak için ek UI değişikliği gerekmiyor.
