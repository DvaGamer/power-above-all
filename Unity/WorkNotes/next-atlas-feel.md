# Bir sonraki küçük atlas / diorama işi

6 Eylül 2026. **Yalnız plan; Assets için yetki değildir.** Görsel temel son GREEN `dumas-labels-first-20260906-021758-659-0da55b25`; yeni ordu mevcudu UI'sinin henüz görülmemiş kareleri bu değerlendirmenin kanıtı değildir. `ART_DIRECTION.md` üstteki güneşli guaj atlası, sekiz renk ailesi ve renk dışındaki seçim ayrımı kuralları okundu. Bu ajan yalnız mevcut kaynak ve PNG okudu; bitmap, kod veya Assets değiştirmedi, Unity/player/derleme başlatmadı.

## Gerçek karelerde ne var?

- `shots/00-start.png`: seçili Île-de-France adı kalındır, fakat yaklaşık x686–807 / y326–451 çevresindeki ince altın/adaçayı çift çizgi kara renginden çok az ayrılır. Ordu bayrağı aynı bölgededir; seçili bölgenin bütün şeklini gözle izlemek için yakın bakmak gerekir. Metnin okunmadığı veya seçimin hiç anlaşılmadığı iddia edilmiyor.
- `shots/labels-11-campaign-return-ru.png`: seçili Champagne ve ordunun Île-de-France konumu artık farklıdır. Champagne'nın çevre çizgisi yine yakın renkli yüzeyde hafiftir; kalın ad seçimi taşır, atlasın ana oyuncu odağı olan bölge şekli geri planda kalır. İnce nehirler, küçük şehir gravürleri ve açık/koyu kabine dengesi korunmaya değer.
- `shots/labels-07-infantry-dense-ru.png`: son etiket işi seçili iki piyade ile topçuyu açmış. Bahçe okunur, fakat yaklaşık x405–610 / y326–451'deki düzgün top biçimli tekrarlar, boyanmış zemin ve elden kurulmuş şehir gravürlerine göre daha ilkel görünür. Bunun bir orman/engel belirsizliği veya mekanik hata olduğu söylenmiyor.
- İlk iki atlas karesinde kontrol katmanının mevcut değerleri sarı-adaçayı orta tonlarında yakındır. Bu gerçek değer aralığına uygun olabilir; verinin yanlış çizildiğine dair kanıt yoktur. Yine de gelecekte 40/60/80 gibi yakın ama anlamlı değerlerin ayırt edilebilirliği ele alınabilecek ayrı bir sorudur.

## Üç farklı küçük yön

| Kavram | Gerçek fayda / özgün karakter | Dar kaynak alanı ve risk |
| --- | --- | --- |
| **A — Mevcut seçim çizgisinin kontrastı** | Zaten var olan iki geçişli seçim konturunun alt geçişi, genel sınır adaçayı yerine daha koyu orman mürekkebi kullanır. Altın üst çizgi ve arazi boyası aynı kalır. Bölge şekli, açık bir belge yanındayken daha hızlı izlenir | `CampaignMap.cs`: yalnız mevcut alt geçişin materyalini değiştiren ayrı seçim materyali ve sahipliği. Üçüncü çizgi, yeni katman veya kalınlık değişimi yoktur. Gerçek1440×900 karede fark yetersizse iyileştirme yapılmış sayılmaz. **Seçilen dar aday** |
| **B — Tematik boyanın değer ayrımı** | Aynı mercan→sıcak orta ton→adaçayı ailesinin ara değerleri yeniden dengelenir; kontrol/elit bağlılığı ve terslenen huzursuzluk katmanında orta değerler daha seçilebilir olur. Guaj yüzeylerin çeşitliliği gerçek veriden gelir | `CampaignMap.ModeColor`, gerekirse `CabinetHud` mevcut legend örnekleri. Değer eşikleri/kurallar değişmez. Bütün katmanları ve düşük/yüksek değerleri görmek gerekir; keyfî yerel renk eklemek veya normal durumu kriz rengine boyamak kabul edilmez. A'dan daha geniş renk kabulü gerektirir |
| **C — Bahçenin yaşayan siluetleri** | Mevcut20 ağacın top yığınları yerine 3–4 kontrollü, basık/eksantrik taç ailesi aynı bahçe sıralarını taşır. Ufak açık üst yüzey ve koyu alt kütle, zaten açıkta kalan asker siluetlerini daha iyi ayırır. Yeni taş/çit/çiçek eklemeden diorama karakterini güçlendirir | `TacticalBattle.cs` yalnız mevcut orchard crown üretim bölgesi/özel ortak mesh; aynı gövdeler, konumlar, yükseklik tavanı ve `InOrchard` gerçek x(−27,−10),z(−5,10) maskesi. Silueti büyütüp asker veya oyun alanı dışını örtme riski vardır. Yeni cover, RNG, ışık veya bitmap işi olmaz; doğal savaş ve yoğun merkez karşılaştırması gerekir |

## A için tek gözle görülür değişiklik

Mevcut kaynak seçimi `borderMat #677960` genişlik .20 / yükseklik .19 ve `goldMat #CAB36F` genişlik .09 / yükseklik .21 ile çizer. Aynı `borderMat`, normal sınırlar/kıyılarda da kullanıldığı için global rengini değiştirmek kapsamı büyütür. Mevcut `goldMat` ordu standardı ve eylem geri bildirimiyle paylaşılır; o da değiştirilmez.

**Root'un kaynak uyarısından sonra kapsam kesinleştirildi:** alt çizgi eklenmeyecek; o çizgi zaten vardır. İlk geçişin `borderMat #677960` materyali, mevcut şehir mürekkebi ailesinden **#3E5A4E** kullanan seçime özgü materyalle **değiştirilir**. İlk geçiş genişlik **.20**, yükseklik **.19**; ikinci mevcut `goldMat #CAB36F` geçişi genişlik **.09**, yükseklik **.21** olarak aynen kalır. Önceki .22/.08 genişlik önerisi geri çekildi. Kaynaktaki tek görsel değişken alt çizginin rengi olur; genel `borderMat` rengi ve bütün diğer kullanımları korunur.

Kaynak renk farkı somuttur: alt geçiş RGB103/121/96'dan62/90/78'e iner; mevcut mat altının iki yanındaki dar kenar daha koyu olur. Bu kaynak renk farkıdır, gerçek ekran kontrast ölçümü veya kabul edilmiş sonuç değildir. Mevcut iki çizginin toplam kalınlığı, geometri sayısı ve merkez hattı aynı kaldığından şehir/komşu bölge alanına yeni bant eklenmez. Fark gerçek önce/sonra karede ancak yakınlaştırınca seçilebiliyorsa bu aday yeterli sayılmaz; otomatik olarak ek kontur/kalınlık katmanıyla büyütülmez.

Yeni tekrarlanan nokta, arma, köşe süsü, parlama, animasyon, gürültü veya rastgele dalgalı sınır eklenmez. “Elle boyanmış” hissi gerçek sınırın merkezini kaydırmakla elde edilmez. Aynı geometri üzerine iki bilinçli ton, mevcut guaj zemin ve gravürlerle birlikte yeterlidir. Seçili yüzeyin şu anki %10 kâğıt vurgusu, hover, pulse süresi, bütün `ModeColor` değerleri, rota ve ordu işareti aynı kalır. Bu pakette B'nin renk ayarı veya C'nin ağaçları birlikte uygulanmaz.

Muhtemel dosya: **yalnız `Unity/Assets/Scripts/Presentation/CampaignMap.cs`**, seçime özgü materyal alanı + `Build` içinde mevcut `owned` listesine eklenen bir materyal + `Refresh` içinde **birinci mevcut** `BorderOfCell` çağrısına bu materyalin verilmesi. Çağrı sayısı yine2'dir. Genel `BorderOfCell`, hücreler, coastline, Collider, `Pick`, durum verisi ve dil etiketleri değişmez. Aynı shader/material yordamı kullanılır; yeni shader, Resource, texture veya paket gerekmez. Var olan `OnDestroy` sahiplik listesi temizler; her seçimde materyal üretimi yapılmaz.

## Gerçek kabul ve geri alma eşiği

Root ayrı kaynak yetkisi verdikten ve uygulama bittikten sonra en küçük inceleme:

1. Aynı build rotasında seçili Île-de-France ve Champagne, önce/sonra1440×900. İkincide ordu başka bölgede kalır: kontur seçimi, bayrak orduyu anlatmalı. Bir bakışta seçili bölgenin şekli ayırt edilmeli; yalnız adın kalınlığına ihtiyaç duymamalı.
2. Kıyıya değen bir seçili bölge ve merkezdeki küçük Île-de-France. Kıyı ve komşu sınırları aynı yerde, parçalanma/çift hayalet çizgi yok; koyu iz normal sınırlarla birleşip yanlış büyük bölge üretmiyor.
3. RU/TR aynı seçili sahne; kontrol, huzursuzluk ve mavi ordu katmanında çizgi görünür. Harita metni, şehir gravürü ve gerçek güzergâh kapanmaz. Değer rengi değişmediği için eski legend anlamı korunur.
4. Gerçek tıklamada eski doğru bölge seçilir; hover ve mevcut katmanlar ayrıdır. Birden fazla seçim sonrası materyal/nesne birikmesi veya çıkışta hata görülmez. Gerekli teknik kontrolleri root belirler; koordinatları tekrarlayan yeni ayna testleri önerilmez.

Değişen renk siyah bir çerçeve gibi baskınlaşır veya açık guaj ülkeyi kutulara bölüyormuş gibi hissettirirse materyal adayı geri alınır. Diğer katmanların rengini veya iki mevcut genişliği eş zamanlı telafi etmek bu ilk karşılaştırmanın dışındadır. Bu plan hazırlanmış olması, A'nın uygulanmış veya yeni GREEN kapsamında kabul edilmiş olduğu anlamına gelmez.

03:06 UTC sonrası durum: root ayrı olarak sıfır ordunun hayalet bayrağı/ordu katmanı/yerel düğme metnini düzeltti ve `army-establishment-final-20260906-030602-688-f7c2fdcb` gate'ini başlattı. Bu iş gerçek durumun doğru gösterilmesidir ve bu küçük kontrast adayından önceliklidir. Kaynak okumasında mevcut seçili iki geçişin hâlâ .20/.09 olduğu teyit edildi; bu ajan root'un değişen Assets alanına dokunmadı. Yeni gate kabulü ve A'nın ek kaynak yetkisi ayrı adımlardır.

## Yetkili dar uygulama — kaynak dondurma

Root `777dfec` ordu checkpoint'i sonrasında bu tek materyal adayına açık uygulama yetkisi verdi. `CampaignMap.cs` içinde yalnız üç dar satır değişti: `selectionInkMat` alanı, `Build` içinde `MakeMaterial(Hex("#3E5A4E"))`, birinci mevcut seçili `BorderOfCell` çağrısında bu materyalin kullanımı. İlk geçiş .20/.19, altın geçiş .09/.21, normal sınırlar ve kıyı aynı kaldı. Üçüncü geçiş, yeni texture/shader, harita geometrisi/konum veya durum değişikliği yoktur.

`MakeMaterial` mevcut `owned` listesine otomatik ekler; `OnDestroy` aynı listeden temizler. `Build` koruması nedeniyle materyal seçim başına üretilmez. Kaynak donduruldu; bu ajan compile/Unity/player/Git başlatmadı. Yeni gerçek A/B görüntüsü henüz yok, materyalin daha okunur olduğu henüz kabul edilmedi. Root aynı kayıt/baseline ile karşılaştıracak; tam karede fayda yoksa bu aday geri alınabilir.

## Gerçek A/B — dar kontrast kabulü

Root koşuları: önce `atlas-contour-before-20260906-031343-374-61945cf5` PARTIAL/native0; sonra `atlas-contour-after-20260906-031416-001-dcf9ac99` GREEN304 /7 PNG /9 assertion /7 state. Bu ajan `00-ile-control`, `01-normandy-control`, `04-provence-unrest`, `06-ile-army-tr` çiftlerini tam1440×900 olarak gördü; root02Champagne çiftini ayrıca gördü. Dört çiftte yalnız seçilen hattın koyuluğu değişiyor; şehirler, kıyı/hücre şekli, nehir, rota ve sayfa yerleşiminde fark görülmedi.

Fayda **küçük ama seçilebilir**:00/01/04'te özellikle Normandiya'nın kara sınırı ve Provence'ın batı kenarı biraz daha kolay izlenir.06'nın mavi ordu dolgusundaysa değişiklik neredeyse fark edilmez; bölgeyi zaten dolgu rengi ayırır. Bu bir yeni sanat kimliği veya bütün odağı çözen büyük değişiklik değildir. Dar bir kontrast düzeltmesi olarak tutulabilir; ek çizgi/kalınlık/renk işiyle genişletilmesi önerilmedi. Koyu kutu etkisi veya okunurluk kaybı görülmedi.

Bütün7 aynı adlı JSON dosyasının önce/sonra SHA-256 değerleri birebir eşittir. Kontrol/huzursuzluk/dil değişimi aynı kampanya durumlarında yapılmış; görsel aday simülasyonu değiştirmemiştir. PNG'lerin aynı olması beklenmez ve söylenmez. Bu incelemede Assets tekrar düzenlenmedi, yeni çalıştırma yapılmadı.
