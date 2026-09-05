# Seçilebilir başlangıç rolleri — küçük oynanabilir kesit önerisi

**Durum:** Öneridir; kesinleştirilmiş rol listesi değildir. Kullanıcı rol seçimini, kişisel iktidar/ülke/savaş ilişkisini ve inandırıcı alternatif tarihi istedi. Çalışma varsayımı: kurgusal kişiler, Fransa'nın 12 bölgesi, haftalık döngü. Tarihsel kişi, coğrafya ve kampanya süresi açıktır.

## Üç farklı uygulama yolu

1. **Yetki sınırları:** Roller farklı emir yetkileri taşır. Bazı kararlar doğrudan alınabilir; diğerleri siyasi destek üzerinden pazarlık gerektirir. Az yeni kayıt alanıyla belirgin oynanış farkı üretir.
2. **Himaye ve borç:** Bütün emirler herkese açıktır; seçilen rol bir hamiden yetki alır. Yardım karşılığında ileride yerine getirilecek talepler oluşur. İlişki hikâyeleri güçlüdür; ancak gerçek yükümlülük kaydı ve takip olayları gerektirir.
3. **Görev sözleşmeleri:** Aynı yetkiler korunur; rol farklı görev ve görevden alınma koşulu taşır. Uygulaması küçüktür fakat emir alışkanlığını yeterince değiştirmeyebilir.

**Öneri: 1.** Her rolün özgün yetkisi ve görünür siyasi sınırı olsun. Ortak harita, ikmal, savaş ve ekonomi korunur. Bedeller mevcut hazine, gıda, kişisel iktidar, dört gücün desteği ve bölgesel kontrol üzerinden hesaplanır.

## Üç geçici rol

| Rol | Gerçek yetki ve karar farkı | Bedel ve sınır |
| --- | --- | --- |
| **Saray yetkilisi** | Ülke çapında olağanüstü vergiyi doğrudan emreder. Paris yardımını sürdürme veya kaynağını kesme kararı kendisindedir. | Zorla tahsilat, meclis/kent desteğini ve hedef bölgedeki elit sadakatini aşındırır. Kraliyet desteği çökerse aynı emir için açık taviz gerekir. |
| **Meclis komiseri** | Olağanüstü vergi için mevcut Meclis desteği gerekir. Destek yoksa sarayın hazinesinden siyasi tavizle kaynak istemek veya daha küçük, gönüllü bölgesel katkıyı kabul etmek arasında seçer. | Hızlı tek taraflı tahsilat yoktur. Saray desteği karşılığında alınan para meclis desteğini ve kişisel iktidarı azaltabilir; düşük gelir ordunun hazırlığını geciktirir. |
| **Ordu temsilcisi** | Saha iaşesi emri yalnız ordunun bulunduğu bölgede verilir: ödeme yaparak tedarik veya yerel zor alım. Merkezden yardım/ülke çapında tahsilat için siyasi destek gerekir. | Zor alım yürüyüşü mümkün kılar ama yerel huzursuzluğu artırıp kontrolü düşürür. Ülke desteği zayıfladıkça iaşe ve maaş sorunu büyür; yalnız savaş kazanmak yeterli değildir. |

Her rol ortak taktik savaşı yönetebilir; fark savaş öncesi imkânları doğurur. İlk kesit emir ayrımları ve açık destek koşullarıyla sınırlıdır; rol değişimi, makam merdiveni ve gizli borç sistemi sonraya kalır. Denge sayıları ayrıca seçilir.

## Ortak sonuç zinciri ve örnek

Sekiz haftalık saray hikâyesi: ilk hafta Paris'e yardım; ikinci hafta ekmek dilekçesi; üçüncü hafta masraf için Champagne'da tahsilat. Kontrol zayıflarken ikmal yolu zorlaşır. Oyuncu yardımı azaltıp siyasi bedeli üstlenebilir, vergiyi bırakıp orduyu bekletebilir veya düşük ikmalle sefere çıkabilir. Altıncı haftadaki pahalı zafer komutanı güçlendirir; sekizinci hafta ülkenin durumu ile oyuncunun kişisel iktidarı birlikte değerlendirilir. Olaylar zorunlu sıra değildir; bu, koşullar oluşursa yaşanabilecek örnektir. Komutan tehdidi yalnız modelde gerçekten hesaplanan sonuçlarla anlatılır.

## Kayıt ve doğrulama

Yeni kayıtta kararlı `RoleId` ve şema sürümü tutulur; çeviri rol kimliği olarak kullanılmaz. Eski kayıtta rol eksikliği mevcut emirleri koruyan açık bir eski-sefer modu sayılır; sessizce yetki kaybedilmez. İlk yeni kampanyada rol seçilir. Önizleme ve uygulama aynı yetki kontrolünü kullanır; reddedilen emir kaynak harcamaz. Üç rol için aynı kriz, düşük destek, kayıt/yükleme ve ikinci kez emir verme kontrolleri gerekir.

## Çekirdek incelemesi sonrası ikinci öneri — güncel olan

**Önceki sert yetki kapılarının yerine ayrıcalık + görünür yükümlülük öneriyorum.** Mevcut `Act`, `March`, savaş ve dilekçe seçenekleri bütün rollerde aynen kalır. Sert kapılar mevcut döngüyü ve eski kayıtların yetkilerini değiştirir; küçük kesitte oyuncuya daha çok yasak gösterebilir. Rol ayrıcalığı ise oyuncuya kısa vadeli bir çıkış yolu verir, bunun siyasi bedelini sonra yaşatır. Bu, kişisel iktidar hedefini doğrudan gösterir. Aşağıdaki sayılar önerilen ilk deneme değerleridir, kullanıcı kararı veya tarihsel iddia değildir.

### Üç ayrıcalık ve somut sonraki karar

| Rol/kararlı kimlik | Şimdiki karar | İki hafta sonraki yükümlülük |
| --- | --- | --- |
| Saray yetkilisi / `crown` | **Kraliyet avansı:** hazine +120; kraliyetin desteği −3. Ordunun ücretini veya bir sonraki seferi zamanında karşılar. | **Öde:** hazine −150, kraliyet desteği +5. **Sözünü boz:** kraliyet desteği −12, kişisel iktidar −6. |
| Meclis komiseri / `assembly` | **Tahıl sözüyle uzlaşma:** seçili bölge huzursuzluğu −18, kontrol +6; Meclis desteği −3. Bölge gerçekten 65 eşiğinin altına inerse savaşsız yürüyüş mümkün olur. | **Sözü tut:** gıda −40, Meclis desteği +5. **Vazgeç:** hedef bölgede huzursuzluk +18, kontrol −6; Meclis desteği −10, kişisel iktidar −4. Değerler mevcut duruma eklenir; eski durum geri yüklenmez. |
| Ordu temsilcisi / `army` | **Saha zor alımı:** yalnız mevcut ordu bölgesinde; gıda +40, askerî malzeme +15, yerel huzursuzluk +8, elit sadakati −6. Ordu yoksa ayrıcalık kullanılamaz; normal yeniden asker toplama açıktır. | **Tazmin et:** hazine −80, hedef bölgede huzursuzluk −5, elit sadakati +4. **Ödemeyi reddet:** huzursuzluk +12, elit sadakati −8, kişisel iktidar −5. Ordu başka yere gitse de alacak eski bölgede kalır. |

Yeni ayrıcalık için kişisel iktidar en az 10, açık yükümlülük bulunmaması ve dört haftalık bekleme süresinin bitmesi gerekir. Böylece aynı hafta sınırsız avans/zor alım döngüsü oluşmaz. Bedeli ödeyemeyen oyuncu sözünden dönebilir; kaynaklar eksiye inmez. Yükümlülükler gizli sürpriz değildir: ilk tıklamadan önce tutar, hedef, vade ve reddetme bedeli gösterilir.

### Asgari kalıcı veri

`CampaignState` içine yalnız:

- `string RoleId`: `legacy`, `crown`, `assembly`, `army`.
- `int NextMandateWeek`: sonraki ayrıcalığın açılacağı hafta.
- `MandateObligation Obligation`: yoksa `null`; tek açık yükümlülük yeterlidir.

`MandateObligation` alanları: `string Kind`, `string RegionId`, `int IssuedWeek`, `int DueWeek`, `int GoldDue`, `int FoodDue`. Türler `royal_advance`, `civic_pledge`, `field_levy`; saray avansının hedefi `ile`, diğerlerinin hedefi verildiği bölgedir. Vade ve ödeme tutarları veriliş anında dondurulur. Çeviri metni, portre yolu, hesaplanabilir `Pending` bayrağı veya tüm dünya anlık görüntüsü kaydedilmez. Farklı yükümlülük listesi, genel olay yöneticisi ve sekizinci hafta bitiş alanı gerekmez.

### Önerilen API ve işlem sırası

- `Create(string roleId)` yeni rolü kurar; mevcut `Create()` eski davranışı koruyan `legacy` oluşturur.
- `CanIssueMandate(state, regionId)` ve `IssueMandate(state, regionId)`: aynı rol, vade, konum, kapasite ve iktidar kontrolleri. Emir adı rolden gelir; yabancı rol yetkisi çağrılamaz.
- `GetMandateTerms(state, regionId)`: yukarıdaki sayıların tek kaynağı; önizleme ve uygulama buradan okur.
- `CanResolveMandate(state, expectedId, choice)` ve `ResolveMandate(...)`: `choice` yalnız `fulfil`/`break`. `expectedId`, tür + veriliş haftası + bölgeden türetilir; eski pencere yeni yükümlülüğü yanlışlıkla çözemez.

`NextWeek` önce mevcut dilekçeyi, sonra **vadesi gelmiş** yükümlülüğü kontrol eder; çözülmeyen karar varsa dünyayı değiştirmeden reddeder. 1→2 hafta geçişi tamamlanabilir; iki karar aynı anda uygunsa ekmek dilekçesi önce görünür. Gelecekteki yükümlülük haftayı durdurmaz. Tamamlama bütün bedelleri doğrular, etkileri bir kez uygular, yükümlülüğü temizler ve günlük kaydı oluşturur. `Forecast` oyuncunun henüz seçmediği ödemeyi gerçekleşmiş gider gibi yazmaz; vadeli tutar ayrı gösterilir.

### Kayıt geçişi ve üç kabul akışı

`GameApp.SaveFile.Version` şu an 1; yükleyici yalnız 1 kabul ediyor. Yazıcı açıkça 2 yazmalı. Sürüm 1 önce doğrulanır, ardından `legacy`, boş yükümlülük ve sıfır bekleme ile dönüştürülür. Sürüm 2 alanları sıkı doğrulanır: bilinen rol/tür eşleşmesi, geçerli bölge, tutarlı hafta aralığı, negatif olmayan sınırlı tutarlar. Bozuk yeni kaydı sessizce eski moda çevirmeyin. Önceki kayıt dosyası başarılı yeni yazıma kadar korunur.

1. **Saray:** avans → kayıt/yükleme → vade → para yetersizken ödeme atomik ret → sözünden dönme → tekrar çözüm atomik ret; sonraki hafta sürer.
2. **Meclis:** huzursuzluğu 69 olan Champagne'da söz → savaşsız geçiş → sonraki hafta başkente dönüş ve kayıt/yükleme → ikinci haftada ekmek dilekçesine `negotiate` → vadesi gelen sözden dönme → Champagne'a sonraki yürüyüş tekrar savaş gerektirir. Yükümlülüğün bölgesel sonucu yalnız Champagne'a uygulanır.
3. **Ordu:** yerel zor alım → başka bölgeye yürüyüş/savaş → vade → tazminat yalnız ilk bölgeye uygulanır; aynı emir/sonuç tekrar uygulanmaz. Ordu sıfıra düştüğünde mevcut yeniden toplama testi ve 200 haftalık sefer testi geçmeye devam eder.

İsteğe bağlı bölüm raporu kampanyayı bitirmez; yeni terminal durum veya zorunlu sekizinci hafta zaferi önerilmez.
