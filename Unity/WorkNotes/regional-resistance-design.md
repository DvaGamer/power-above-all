# Bölgesel direniş: oyuncunun asker sayısından bağımsız konuşlanma

İlk bölüm tasarım gerekçesidir; root daha sonra B modelinin dar uygulamasını onayladı. Kaynak teslim durumu sondadır. Sayılar oyun ölçeğidir, tarihsel asker istatistiği değildir. Aşağıdaki hesaplar kaynak formüllerinden yapılan aritmetiktir; yeni oyuncu koşusu veya savaş sonucu kanıtı değildir.

## Mevcut sorun ve korunacak sınır

`TacticalBattle.Begin`, düşmanı `Max(200, RoundToInt(originalTroops * .9f))` ile kuruyor. Haritada 1200→1600 takviye, aynı Champagne için düşmanı1080→1440 yapıyor. Gerçek subay rotasında ayrıca iki normal fiyatlı grubun moral bedeli vardır; fazla asker tek başına hızlı zafer garantisi olmamalı, fakat rakibi otomatik büyütmemeli.

`RegionDefinition` zaten Population, BaseTax, BaseFood ve Neighbours; `RegionState` Unrest, Control, EliteLoyalty içeriyor. Mevcut düşmanlık sınırı Unrest≥65. Bu sınırı, yürüyüş maliyetlerini, dört birlik türünü, mevzi geometrisini ve savaş sonrası sonuçları bu dilimde değiştirmeyelim.

## Üç farklı model

**A — Bölgeye ait hazırlanmış birlik listesi.** Her tanım için sabit bir `ResistanceBaseTroops`; Champagne örneğin yaklaşık1100. Siyaset, mevcut65 sınırı üzerinden çatışmanın çıkıp çıkmayacağını belirler. Haritadaki yatırım rakibi değiştirmez; içerik yazarı zorluk dağılımını kolayca denetler. Fakat huzursuzluk81→66 iyileşmesi, savaş sürdüğü müddetçe kuvveti hiç azaltmaz. On iki yeni içerik kararı gerekir; mevcut Control ve EliteLoyalty hâlâ zayıf bağlantılar olur. Kalıcı savaş kaybı takibi eklenirse kapsam ayrıca büyür.

**B — Yerel örgütlenme ve üç siyasi baskı.** Sabit ekonomik taban, halkın huzursuzluğu, devlet denetimindeki açıklık ve karşı duran elitlerin birleşimi mevcut sefer kuvvetini verir. Vergi veya iaşe kararının kısmî siyasi etkisi,65 eşiği aşılmasa bile sonraki çatışmada görülür. Yeni kalıcı veri gerekmez. Katsayılar açık bir oyun modeli olarak kalır; üretim ve gerçek hazineyle karıştırılmaz.

**C — Komşu bölgelerin desteklediği koalisyon.** Yerel sabit kuvvete, açık koşulları sağlayan komşu düşman bölgeler katkı verir. Oyuncu hedefe girmeden destekçilerini yatıştırabilir; Neighbours gerçek stratejik önem kazanır. Fakat komşuların aynı birliği sınırsız göndermesi, kayıpları, birden fazla cephe ve yardımın ne zaman kesileceği açıklanmalıdır. Kalıcı kayıt eklemeden yapılabilir, ancak ilk dar düzeltme için daha fazla eşik ve harita açıklaması üretir.

**Öneri: B.** Eksik bağlantı, zaten ödenen ekonomik ve siyasi kararların savaş büyüklüğüne ulaşmasıdır. B bunu yeni bir ayrı kaynak, kredi veya zorunlu cevap açmadan kurar.

## Önerilen tek hesap

```text
Unrest < 65 ise: RequiresBattle=false; EnemyTroops=0
Diğer durumda:
  örgütlenmeTabanı = 30 × BaseTax
  baskı = Unrest/100 + (100−Control)/100 + (100−EliteLoyalty)/100
  EnemyTroops = RoundAwayFromZero(örgütlenmeTabanı × baskı)
```

BaseTax, bölgenin sabit ekonomik ve örgütsel ağı için mevcut vekil veridir; o hafta tahsil edilen vergi değildir. Vergi tatili, düşük kasa veya oyuncunun fakirleşmesi düşmanı doğrudan aç bırakmaz. BaseFood tek başına seçilmedi: Paris'in düşük tarımsal üretimi, şehrin örgütlenme gücünü yapay biçimde en alta indirmemeli. Population da değerlidir; aynı küçük hesapta iki kapasite göstergesini keyfî ağırlıklarla karıştırmaya gerek yok.

Üç baskıya eşit ağırlık verilmesi ilk okunabilir çalışma varsayımıdır: halkın katılım isteği, devletin müdahale açıklığı ve yerel örgütleyicilerin muhalefeti. Control başlangıçta Unrest'ten türetilse de sonraki gerçek eylemler alanları ayrı değiştirir. Başlangıç korelasyonu gizli yeni çarpan değildir. `30` tek genel karşılaşma ölçeğidir; kişi başına gerçek vergi veya tarihsel seferberlik oranı iddiası taşımaz. Mevcut yaklaşık bin kişilik ilk muharebe ölçeğini korur; her bölgeye veya oyuncu ordusuna ayrı uyum katsayısı konmaz.

| Aynı Champagne için durum | U / C / E | Düşman |
| --- | --- | ---: |
| Başlangıç |69 /60,5 /60 |1114 |
| Yalnız bir olağanüstü vergi sonrası |81 /60,5 /56 |1234 |
| Bu vergiden sonra ekmek yardımı |66 /62,5 /56 |1106 |
| Bu vergiden sonra vergi tatili |71 /63,5 /56 |1136 |
| Başlangıçtan doğrudan ekmek veya tatil |54 veya59 / ilgili yeni değerler |0; barışçıl yürüyüş |

Tablo araya hafta, garnizon veya başka eylem girmeyen tekil etkileri karşılaştırır. Tatilin dört gerçek vergi hesabı bedeli ve mevcut sözleşmeleri aynen sürer. Başlangıç1114, eski1080'den yaklaşık%3,1 fazla; tam1080'e uydurmak için ek sabit yok. Aynı hedefte oyuncu1200,1600 veya2000 olduğunda1114 değişmez. Başka bölgede asker toplamak hedefi değiştirmez; hedefte gerçekten uygulanan yerel Unrest artışı ise haklı bir nedendir.

Karşı örnekler: Champagne'da Control veya EliteLoyalty+10, düşmanı yaklaşık75 azaltır; huzursuzluk+10 yaklaşık75 artırır. U65/C100/E100 hâlâ488 kişilik halk direnişi üretir; sadık elitler tek başına isyanı sıfırlamaz. Üç baskının azamisinde Champagne2250 olur. Mevcut en yüksek BaseTax48 için4320, en düşük19 için düşmanlık eşiğinde en az371 çıkar. Bu üst örnekler bir başlangıç ordusunun hepsini yenebilmesi vaadi değildir; önce yönetim, sonra sefer gerçek alternatif olmalıdır. Yeni oyuncu-tabanlı alt/üst sınır kullanılmaz.

Garnizonun sayısı doğrudan çarpan değildir: mevcut başarılı haftanın Unrest−3/Control+2 etkisi yeni bölge durumunu oluşturur, sonraki hesap onu okur. Zaferin mevcut Unrest−22/Control+12 sonucu da gelecekteki direnişi gerçekten azaltır. Model kalıcı düşman tabur kayıplarını saklamaz; ileride tekrar yükselen isyan yeni yerel seferberlik sayılır. Bu sınırlama açık tutulmalı.

## Mıntıka ve okunabilir tahmin

RegionDefinition'da bölgesel terrain alanı yok. Bugünkü muharebeler aynı geçit, bahçe ve tepe düzenini kullanıyor; X/Y harita konumundan yeni arazi cezası çıkarılmamalı. Gerçek tepe, görüş ve hareket etkileri taktik oyunda kalır; insan sayısında tekrar sayılmaz. İleride yazılmış bölgesel arazi düzenleri ayrı bir içerik işi olabilir.

Mevcut yürüyüş satırı, bizim canlı askerimizi ve hedefin hesaplanan kuvvetini birlikte gösterir; ayrıntıda bölgenin üç siyasi nedeni görünür. Bunlar başarı yüzdesi değildir. Moralin, ikmalin, formasyonun ve konvoyu tutmanın sonucu değiştirdiği korunur. Sayı tam gözlenen güncel bölge durumundan deterministik olduğundan sahte bir belirsizlik aralığı veya gizli istihbarat RNG'si eklenmez.

## En küçük teknik sözleşme

Önerilen salt okunur `GetRegionalResistance(state, regionId)` → `RegionalResistanceTerms { RegionId, RequiresBattle, EnemyTroops, BaseTax, MobilizationBase, UnrestPressure, ControlGap, EliteOpposition }`. CanMarch'tan bağımsızdır: sıfır askerle de hedef incelenebilir. Bileşenler kesirli tutulur; yalnız nihai toplam yuvarlanır, UI ayrı yuvarlanmış alt toplamları yanlış eşitlik gibi sunmaz.

GameApp yürüyüş kabulünde aynı hedef durumundan bir kez değer alır ve mevcut BattleSetup'a `EnemyTroops` ekleyerek taşır. Taktik taraf `DeployArmy(false, setup.EnemyTroops)` kullanır. Savaş sırasında yeni asker alımına veya oyuncu kaybına göre düşman tekrar hesaplanmaz. Eksik değer için eski*.9 formülüne sessiz dönüş yoktur; bağımsız taktik kurulum/testler açık düşman sayısı verir. Normal kampanya önizlemesi ve oluşturulan dört düşman alayının toplamı eşit olmalıdır.

CampaignState ve archive değişmez: sayı zaten kayıtlı hedef durumundan türetilir. Kayıttan yüklenen aynı bölge aynı kuvveti verir. Yeni tohum veya kalıcı stok yok; mevcut savaş içi rastlantı/seed davranışı bu işin parçası değildir. UI son çiziminden sonra gerçek durum değişirse kabul anında güncel sonuç kullanılır ve yeniden gösterilir.

## Kabul kontrolleri ve ilk gerçek rota

1. Aynı bölge için farklı Troops, Gold, rol, subay hakkı ve kadro politikası kuvveti değiştirmez. Yerel vergi/ekmek/tatil ve haftalık garnizon sonucu değiştirir;64,99/65 sınırı korunur. Önizleme salt okunur ve save/load sonrası aynıdır.
2. Taktiğe geçirilen sayı, dört düşman grubunun toplamına tam eşittir; savaşta yeni kuvvet doğmaz. Eski birlik, ammo, eşzamanlı saldırı ve dönüş testleri davranışlarını korur; yalnız artık açık olan başlangıç kuvveti girdisi uyarlanır.
3. Gerçek iki kampanya rotasında Paris'te1200 ve ücretli1600 askerle aynı başlangıç Champagne'a gidilir. İkisinde düşman1114 olmalı; gerçek emirler ve konvoyu ele geçirme gerekir. Sonuç veya kayıp önceden yazılmaz. Üçüncü kısa rotada vergi→ekmek karşılaştırması,1106 kişilik hâlâ düşman hedef ile politik bedeli gösterir. Bunlar önerilen gelecek doğrulamalardır, henüz çalıştırılmış kanıtlar değildir.

## Onaylı kaynak teslimi — 2026-09-06

`Core/CampaignRegionalResistance.cs` yukarıdaki DTO'yu uygular: RegionId string; RequiresBattle bool; EnemyTroops/BaseTax int; diğer dört bileşen double. Invalid campaign/unknown region null verir. Public sorgu Validate yapar; CanMarch yalnız ortak `IsHostileRegion` yaprağını kullanır, eski ret sırası ve sıcak yoluna yeni tam doğrulama eklenmedi.

BattleSetup açık ve pozitif EnemyTroops ister. Null setup, pozitif olmayan düşman veya null camera, Stop ve mevcut dünyanın herhangi bir değişiminden önce reddedilir. Kabulde ayrı `enemyOriginalTroops` alanı sabitlenir; BattleSnapshot.EnemyOriginalTroops bunu taşır. Deployment paylaşımı ve son alayın kalan kişi hesabı değişmedi. Mevcut üç taktik test kurulumuna açık1080 yazıldı; scripted ResolveBattle196 sonuçları değiştirilmedi. CampaignState ve archive sürümü değişmedi.

Yeni `RegionalResistanceTests.cs`:12 metot, eşik çiftinden dolayı13 NUnit vakası. Mevcut BattleCommandTests'e3 vaka eklendi:1200/1600 için gerçek dört alay toplamı; sonradan setup değişimi ve gerçek retreat sonrasında sabit düşman; hatalı yeni Begin'in eski dünya, snapshot ve completion callback'ini koruması. Bu sayı16 yeni vaka kaynağıdır, geçmiş test sonucu iddiası değildir.

`Unity/WorkNotes/RegionalResistanceProbe.cs`, public Main sınıfı RegionalResistanceProbe: yalnız gerçek Core API'leriyle1200/ücretli1600, vergi→ekmek, vergi→dört haftalık tatil ve barışçıl tatil→yürüyüş→garnizon rotaları. Salt okunurluk ve archive roundtrip de kontrol edilir. Bu ajan derleme, probe, NUnit, Unity veya player çalıştırmadı; kaynaklar root'un merkezi doğrulamasına teslim edildi. Sonuçlar elde edilmeden PASS veya yeni muharebe kaybı yazılmadı.
