# Sonraki görsel adım — atlasın deniz kompozisyonu

## Durum ve dayanak

6 Eylül 2026. Bu belge yalnız tasarım önerisidir; Assets değişikliği veya uygulama yetkisi değildir. Unity, oyuncu, derleme ve test çalıştırılmadı.

İncelenen gerçek, kabul edilmiş kare: `output/verify/officer-commission-import-fixed-20260906-034710-694-4b17a317/shots/02-offer-upper-ru.png`, 1440 × 900. Sanat dayanağı: `ART_DIRECTION.md`, “güneşli guaj atlas ve yaşayan tarihî minyatürler”. İncelenen kaynak: `CampaignMap.Build`, `AddSurroundings`, `AddCoastalEngraving`, `AddEngraving`, `MakeAtlasMaterial`, `NewMesh`, `MakePaperGrain`.

Karede sıcak belge yüzeyleri, koyu kabine çerçevesi ve küçük şehir gravürleri ortak bir dil kuruyor. Ancak açık deniz, özellikle ülkenin batısında, geniş ve neredeyse tek renkli bir mavi yüzey. İnce kıyı çizgileri bu alanın genel kompozisyonunu değiştirmiyor. Önceki seçili sınır mürekkebi değişikliğinin yararı küçüktü; burada amaç tam karede görülen, fakat ülke verisinin önüne geçmeyen bir değişikliktir.

## Üç farklı kavram

| Kavram | Gerçek sorun ve görünür karşılığı | Sınır ve bedel |
| --- | --- | --- |
| **A — Üç havzayı birleştiren boyalı deniz** | Büyük düz mavi alan, ülkenin sıcak siluetini çevreleyen bilinçli bir soğuk/açık renk kompozisyonuna dönüşür. Batıda serin bir Atlantik alanı, kuzeyde ve güneyde daha sütlü mavi yüzeyler; aralarında geniş, sakin ve asimetrik geçişler. | Yalnız mevcut deniz yüzeyinin rengi. Kara, veri katmanları, kıyı çizgisi, etiketler ve kamera aynı kalır. Yanlış uygulanırsa sıradan radyal renk lekeleri veya su derinliği haritası gibi görünür. |
| **B — Tekrarlanan üçgenlerden yönlü dağ sırtlarına** | Karede güney ve doğudaki aynı küçük üçgenler, bölgenin şeklini anlatmaktan çok tekrar eden işaretler gibi okunuyor. Mevcut 18 işaret yerine aynı ayak izinde iki farklı, kesintili sırt ailesi kullanılır; doğuda daha dik, güneyde daha yatay çizgi ritmi. | Yalnız mevcut dekoratif dağ gravürleri değiştirilir; yeni yükseklik veya kural eklenmez. Küçük ölçekte şehir ve bölge yazılarıyla rekabet etme riski vardır. Tam karede A kadar büyük etki üretmez. |
| **C — Kabine belgesinde belirgin yazar başlığı** | Memur yetkisi belgesinde portre, ad, iki değer ve giriş paragrafı ayrı dikey parçalar olarak duruyor. Aynı portreyi sınırlı koyu mürekkep alanına oturtup ad ve belge başlığını yanındaki sıcak kâğıtta tek bir başlık grubu yapmak, sözün kimden geldiğini daha güçlü anlatır. | İlk deneme yalnız `CabinetOfficerCommission` başlığında olur; metin, düğmeler ve aşağıdaki koşullar korunur. Yeni pencere veya gösterge kartları yoktur. Mevcut belge zaten okunur ve kabul edilmiştir; dar panelde yükseklik ve Türkçe başlık riski nedeniyle şimdi seçilmez. |

## Seçim: A

A, mevcut kaynaklarla en görünür ve en dar kapsamlı adımı verir. Ekranın geniş bir bölümünü değiştirirken oynanabilir alanların renk anlamını ve kabul edilmiş kabine akışını korur. Amaç denize daha fazla nesne koymak değil, ülke silueti ile çevresinin sıcak/soğuk ilişkisini düzenlemektir.

Önerilen ilk aday paleti mevcut aileden türetilir:

- Ana su: mevcut `#83B0B6`.
- Açık Atlantik'in batı tarafındaki en serin alan: `Lerp(#83B0B6, #5F8DA5, 0.65)`, yaklaşık `#6C99AB`. Koyu orman mürekkebine yaklaşmaz; deniz yazıları açık yüzeyde kalır.
- Kuzeydeki sütlü ışık: `Lerp(#83B0B6, #F3E7CA, 0.22)`, yaklaşık `#9CBCBA`.
- Güneyde daha küçük sıcak açıklık: aynı kâğıt karışımı, en çok `0.14`; kuzey alanının kopyası olmaz.

Bunlar başlangıç malzeme değerleridir, görüntü pikseli veya kabul edilmiş son renk değildir. Kara üzerindeki kontrol/huzursuzluk renklerine bu karışım uygulanmaz.

Biçim tarifi: batıdaki soğuk alan uzun ve eğik bir fırça alanı gibi davranır; bir merkezden yayılan yuvarlak leke olmaz. Kuzey açıklığı Kanal'ın uzun yatay boşluğuna, güney açıklığı ülkenin altındaki ayrı su boşluğuna uyar. Geçişlerin genişliği tam karede yaklaşık 60–140 piksel hissi verir; küçük kıvrım, köpük, dalga, gemi veya yeni kıyı halkası eklenmez. Üç alan tek sakin yüzey halinde birleşir; ayrı renk adacıkları görülmez. Kıyıya eş uzaklıklı renk şeritleri kullanılmaz: bu bir derinlik veya hareket maliyeti haritası değildir.

## Gelecekteki uygulama sınırı

Tek kaynak dosya: `Unity/Assets/Scripts/Presentation/CampaignMap.cs`.

Mevcut `MakeFlat("Atlas sea", ..., #83B0B6, -.22f)` çağrısının yerine, aynı dört dış köşeyi ve yüksekliği koruyan tek statik deniz mesh'i önerilir. Büyük renk alanları az sayıda elle belirlenmiş kontrol eğrisiyle veya seyrek bir ızgaranın vertex renkleriyle tarif edilir. Bir mesh, bir materyal, mevcut `owned` ömrü; her karede hesaplama yoktur. İlk aday için 1500 vertex'i aşan geometriye ihtiyaç yoktur.

Mevcut `MakeAtlasMaterial(Color.white)` yolu, `Sprites/Default` ve mevcut `paperGrain` yeniden kullanılabilir. Şehir gravürlerinde vertex renkli mesh yolu zaten vardır. `MakePaperGrain` değiştirilmez; ortak dokuyu değiştirmek kara ve belgelerde istenmeyen sonuç doğurur. Yeni shader, paket, raster doku, rastgele sayı akışı, kayıt alanı veya animasyon gerekmez.

Değişmeyecek alanlar: `MainlandCoast`, çevre kara parçaları, Corsica şekli, hücreler ve tohumları, `ModeColor`, normal/seçili sınırlar, kıyı gravürü, nehirler, dağlar, şehirler, ordu görünürlüğü, harita yazıları, GUI ve kamera. Yalnız deniz oluşturma çağrısı ile ona özel küçük private oluşturma/renk yardımcıları uygulama kapsamı olabilir.

## Görsel kabul

Root'un aynı kayıt ve kamera ile gerçek oyuncudan alacağı önce/sonra kareleri gerekir. Asgari karşılaştırma: normal kontrol görünümü RU, aynı görünüm TR ve huzursuzluk/ordu katmanlarından birer tam kare. Aynı dil/durum çiftlerinin JSON'ları eşit olmalı; bu, estetik kabulün yerine geçmez.

1. Fark 1440 × 900 tam karede, kırpma veya yakınlaştırma istemeden görülür. Ülke sıcak ve tek bir siluet olarak kalır; su dikkati şehir veya seçili bölgeden çekmez.
2. Atlantik, Kanal ve Akdeniz yazıları her iki dilde açıkça okunur. Yazı altına yeni plaka veya halo ekleyerek renk sorunu gizlenmez.
3. Batı suyu ile kuzey/güney açıklıkları aynı paletin parçası görünür. Yuvarlak ışık lekesi, neon turkuaz, koyu delik, üç ayrı blok veya görünür mesh üçgenleri yoktur.
4. Kıyıda kesik, renk dikişi, yeni halka veya piksel sızıntısı görülmez. Küçük şehirler, seçili sınır ve ordu işareti önceki konum ve önceliğini korur.
5. Sonuç yalnız başka bir mikro kontrast düzeltmesi olarak kalırsa veya süslü bir arka plan gibi ülkenin önüne geçerse aday reddedilir; sonraki katmanlarla örtülmez.

## Mevcut kaynaklarla yeterli mi?

Evet, bu önerinin görünür yararı için yeni bitmap gerekli değildir. Sorun resim ayrıntısı eksikliği değil, büyük yüzeyin renk kompozisyonudur. Mevcut mavi/kâğıt/kumaş renk ailesi, çok hafif gren, vertex renkli mesh ve mevcut shader yeterli araçları sağlar. Bu, önerinin çalışma zamanında güzel görüneceği anlamına gelmez; nihai karar gerçek tam kare A/B ile verilmelidir.

## Onay sonrası kod hazırlığı — kaynak henüz donuk

Root A kavramını onayladı; Assets'e başlamak için ayrı başlangıç mesajı bekleniyor. Aşağıdaki somut tarif yalnız bu notta hazırlanmıştır.

### Tek mesh geometrisi

Kare dış sınırları aynen `x = -700..1500`, `y = -600..1600`; dünya yüksekliği `-.22f`. Izgara görünür atlas çevresinde daha sık, ekran dışındaki uzak kenarlarda seyrek olur:

- X düğümleri: `-700, -300, -100`; sonra `-60..930` dahil, adım `30`; sonra `1200, 1500`. Toplam **39 sütun**.
- Y düğümleri: `-600, -300, -180, -90`; sonra `-30..900` dahil, adım `30`; sonra `1200, 1600`. Toplam **38 satır**.
- Toplam **1482 vertex**, **2812 üçgen**, bir `MeshRenderer`, bir materyal. Görünür yerde yaklaşık 30 atlas birimlik örnek aralığı, büyük geçişlerde iri üçgen lekesi riskini azaltır. Dışarıdaki seyrek yüzler yalnız değişmeyen sınırı tamamlar.
- Vertex: mevcut `World(new Vector2(x, y), -.22f)`. UV ve sahiplik mevcut `NewMesh` ile oluşturulur; sonrasında `mesh.SetColors` çağrılır. Ek `owned.Add(mesh)` yapılmaz; `NewMesh` zaten ekler.
- Her hücrede üst sol indeks `i`, sütun sayısı `39`: üçgenler `(i, i+1, i+39)` ve `(i+1, i+40, i+39)`. Dünya Y yönündeki normal yukarı bakar. Kareler arasında ortak vertex kullanılır.
- Materyal bir kez `MakeAtlasMaterial(Color.white)` ile oluşturulur; bu yardımcı da sahipliği zaten kaydeder. Mevcut gren, opak vertex alpha `1`, gölge üretimi kapalı. GameObject adı `Atlas sea` kalır; collider eklenmez.

### Deterministik renk alanları

`S(v,a,b) = t²(3−2t)`, `t = Clamp01((v−a)/(b−a))`. Her vertex için üç geniş yönlü alan; sinüs, gürültü, rastgele sayı veya dairesel uzaklık yoktur. Aşağıdaki tüm konumlar mevcut atlas koordinatıdır.

```text
atlanticEdge = 230 + .34 * (y - 280) + 24 * S(y, 360, 620)
atlanticWeight = (1 - S(x, atlanticEdge - 210, atlanticEdge + 120))
                 * S(y, 150, 320)
colour = Lerp(baseWater, coolAtlantic, atlanticWeight)

channelEdge = 212 + .10 * x - 30 * S(x, 90, 370)
channelWeight = 1 - S(y, channelEdge - 90, channelEdge + 70)
colour = Lerp(colour, lightChannel, channelWeight)

southEdge = 560 - .12 * (x - 470)
southWeight = S(y, southEdge - 45, southEdge + 105) * S(x, 350, 530)
colour = Lerp(colour, lightSouth, southWeight)
```

Paletin dört `Color` değeri mesh oluşturulurken bir kez hazırlanır ve renk yardımcısına aktarılır. Böylece her vertex'te hex ayrıştırılmaz. Önceki bölümdeki yaklaşık 60–140 piksel geçiş hedefi kuzey/güney içindir; Atlantik'teki eğik geçiş özellikle daha geniştir. Kıyı mesafesi ölçülmez ve hücre verisi okunmaz. Alanların kara altında kalan bölümleri mevcut opak kara mesh'leriyle doğal olarak örtülür.

Planlanan private yardımcılar: `MakeAtlasSea`, `SeaWashColor`, `SeaWashTransition`. `Build` içinde yalnız mevcut deniz `MakeFlat` çağrısı değiştirilir. Alan, kamera ve yaşam döngüsü için yeni field gerekmez. `MakeFlat`, `MakeAtlasMaterial`, `NewMesh`, `MakePaperGrain` ve genel `OnDestroy` gövdelerine dokunulmaz.

Bu sayılar hazırlık tarifidir; kaynak değişikliği, derleme veya oyuncu kabulü yapıldığı anlamına gelmez.

## Uygulama adayı — SOURCE FREEZE

Root'un ayrı başlangıç mesajından sonra tarif uygulandı. `CampaignMap.cs` içindeki tek deniz çağrısı `MakeAtlasSea()` oldu; yalnız `MakeAtlasSea`, `SeaWashColor`, `SeaWashTransition` eklendi. Kaynak okuması 39 × 38 vertex, eski dış sınırlar/yükseklik, ortak UV/gren yolu ve tek materyal/mesh sahipliğini doğruladı. Diğer harita oluşturucuları ve genel yardımcılar değişmedi.

Derleme, Unity, oyuncu veya Git komutu çalıştırılmadı; index'e dokunulmadı. Kaynak root'un gerçek dört karelik A/B ve derlemesi için donduruldu. Görsel kabul henüz yapılmadı.

## İlk gerçek A/B görsel incelemesi — açık aday, nihai kabul yok

Önce: `atlas-sea-before-20260906-040412-294-5d5eb232`. Sonra: `atlas-sea-after-20260906-040633-348-bc3a6a43`. Dört çiftin sekiz PNG'si ayrı ayrı tam 1440 × 900 boyutta açıldı: `00-provence-control-ru`, `01-provence-control-tr`, `02-provence-unrest-ru`, `03-provence-army-ru`.

**İlk görsel öneri, görülen pastel kompozisyonu kabul etmekti; root incelemesi sonrasında bu öneri nihai kabul olarak kullanılmıyor.** Fark yalnız yakınlaştırmada görülen bir mikro kontrast değil. Deniz belirgin biçimde daha açık ve sütlü; Atlantik kuzey/güney açıklıklarından daha serin ve koyu kalıyor. Gerçek görüntüde bütün su, eski tek renk materyalden daha açık görünüyor. Bu nedenle sonuç “eskiye göre daha koyu Atlantik” olarak anlatılamaz; görünümün hoş bulunması, seçilen kaynak paletin doğru aktarıldığını kanıtlamaz. Aşağıdaki renk uzayı düzeltmesi ayrı aday olarak karşılaştırılacak.

- **00 / 01:** Sıcak ülke silueti açık maviyle temiz ayrılıyor. RU ve TR deniz adları, kuzey işareti, harita başlığı ve şehir yazıları okunur. Kuzeydeki sütlü alanın serin Atlantik'e geçişi yumuşak; yuvarlak renk lekesi veya yeni bir derinlik haritası hissi yok.
- **02:** Champagne'ın mercan huzursuzluk rengi aynı görsel önceliği koruyor. Daha açık su, bu anlamlı kara renginin karşısında sakin kalıyor; seçili Provence konturu kaybolmamış.
- **03:** Orduyu gösteren mevcut mavi Île-de-France dolgusu artık denizden daha koyu okunuyor. Bu ayrım yararlı; kara rengi veya bayrak değişmedi. Seçili Provence ile gerçek ordu konumu hâlâ farklı işaretler.
- Tam karelerin hiçbirinde belirgin ızgara, üçgen izi, basamaklı bant, kıyı dikişi veya boş yüzey görülmedi. Kamera ve panel yerleşimi aynı.
- Yan etki: mevcut soldaki kısa yatay deniz gravürleri ve kıyıdaki iki ince çizgi, açık yüzeyde öncekinden daha görünür. Özellikle soldaki düzenli ritim hâlâ biraz mekanik; metni kapatmıyor veya su kompozisyonunu bastırmıyor. Bu adayın sınırını genişletmek için gerekçe sayılmadı, mevcut çizgilere dokunulmadı.

Bu değişiklik atlasın geniş renk dağılımını iyileştiriyor; tek başına bütün harita biçimlerinin artık yeterince karakterli olduğu iddiası değildir.

Salt okunur dosya kontrolü: dört önce/sonra JSON çifti byte düzeyinde aynı SHA256 değerine sahip (`d994d257aa0de0f8f227cfdbbdfe1fb3bc2f307d18974d49f3a02c9ed1da8854`). Root'un yeni `REPORT.md` sonucu GREEN: 380/380 Unity testi, yeni build, 4 PNG, 8 assertion, 4 state, 10 browser testi. Bunlar ayrı teknik kanıttır; yukarıdaki görsel karar gerçek PNG incelemesine dayanır. Bu incelemede Assets değiştirilmedi veya yeni süreç başlatılmadı.

## Renk uzayı düzeltmesi — ikinci aday SOURCE FREEZE

Root, planlanan koyu Atlantik yerine bütün suyun açılmasına itiraz etti. Yerel kaynak kontrolü: `ProjectSettings.asset:50` içinde `m_ActiveColorSpace: 1` (Linear); eski yol `MakeAtlasMaterial` üzerinden `material.color`, yeni yol ise doğrudan `mesh.SetColors` kullanıyor. Ham RGB vertex değerlerinin Linear projeye aktarımı, materyal rengiyle aynı yol değildir.

Root'un dar düzeltme izniyle yalnız deniz döngüsündeki renk ekleme değiştirildi: `SeaWashColor` sonucu `QualitySettings.activeColorSpace == ColorSpace.Linear` olduğunda `.linear`, Gamma durumunda ham renk olarak eklenir. Palet, ağırlıklar, geometri, alpha ve ortak yardımcılar aynı. Şehir veya başka mesh renklerine dokunulmadı.

Bu ikinci aday kaynak bakımından donuktur. Yeni derleme/gerçek A/B root tarafından yapılacak; ilk açık adayın teknik GREEN sonucu veya geçici görsel önerisi bu adaya taşınmaz. Bu düzeltmede de Unity, derleme, oyuncu ve Git çalıştırılmadı.

## Linear adayın gerçek son incelemesi — kabul

Seçilen sonuç: `output/verify/atlas-sea-linear-20260906-041054-091-2db68a88`, runtime `fdb980581940a214c92dff8de071f23e0c2fca7e1d319478dbf93ab0941fcefc`.

Root yeni `00` kontrol ve `02` huzursuzluk tam karelerini kabul etti. Bağımsız olarak yeni `01-provence-control-tr.png` ve `03-provence-army-ru.png`, eski aynı isimli before kareleriyle tam 1440 × 900 boyutta açılıp karşılaştırıldı.

**Son öneri: Linear düzeltmeli A kabul edilsin.** Atlantik artık eski yüzeyden gerçekten daha derin ve soğuk; kuzey ve güney daha açık fakat önceki ham vertex adayındaki kadar sütlü değil. Etki tam karede açıkça görülüyor. Sıcak ülke, yumuşak mavi su ve koyu kabine çerçevesi aynı palet içinde kalıyor.

- TR `Manş Denizi`, `Atlas Okyanusu`, `Akdeniz` ve kuzey yazıları okunuyor. Atlantik yazısının altında koyu renk metni boğmuyor. Çizgi veya yazı üzerine yeni arka plan eklemek gerekmiyor.
- Ordu katmanında Île-de-France'ın mavi dolgusu ve bayrağı okunur. Seçili Provence konturu ayrı kalıyor; kara ve deniz arasında yeni bir yanlış oyun anlamı oluşmuyor.
- Görünür grid, üçgen, keskin bant veya kıyı açıklığı yok. Büyük geçiş, kuzeyden batıya sakin biçimde ilerliyor; eski ilk adayın gereğinden fazla belirginleştirdiği küçük deniz çizgileri de tekrar geri planda.

Root'un son teknik bildirimi: GREEN 380/380 Unity, 4 PNG, 8 assertion, 4 JSON, 10 browser testi, 141 shipped dosya; dört JSON yine before SHA256 `d994d257…8854` ile aynı. Bu inceleme yalnız dosya ve görüntü okudu; Assets değiştirilmedi ve süreç başlatılmadı. İlk `atlas-sea-after` açık aday, incelenmiş fakat **seçilmemiş ön aday** olarak kalır; onun geçici kabul önerisi geçersizdir.
