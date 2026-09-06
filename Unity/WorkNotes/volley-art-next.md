# Yaylım — boyanmış barut izi

Durum: özel unlit alfa yolu ve güçlendirilmiş tint/.46 gerçek oyuncuda incelendi. Teknik hata giderildi; gelişmiş fazda çok yumuşak açık duman okunuyor, ilk faz hâlâ sönük. Ek kör yoğunluk artışı önerilmedi; daha iddialı yaylım vurgusu ayrı iş olarak kalır. Bu ajan Unity, player, Git, probe veya derleyici çalıştırmadı.

## Gerçekte var olan fazlar

`TacticalBattle.cs`, ortak `TacticalBattleSimulation.cs` ve `Resources/BattleMaterials/DioramaTransparent.mat` okundu. `output/verify/accord-ford-natural-20260906-002505-474-25aa327c/shots/04-artillery-volley.png` ile `05-contested-crossing.png` gerçekten açıldı. Ortadaki beyaz yuvarlak duman parçaları küçük parlak boncuklar gibi okunuyor; daha fazla nesne eklemek yerine bu siluet düzeltilmeli. Dosya adındaki “volley”, karenin namlu parlamasının ilk 75 ms'sini yakaladığını kanıtlamaz.

| Faz | Mevcut uygulama | Bu pakette korunacak sınır |
| --- | --- | --- |
| Hazırlık | `PreparingVolley`: sabit, mühimmatlı, uygun yön/menzildeki atıcı için yeniden doldurma ≤ `.6 s`. `UpdateVisual` silahı kaldırır; atış sonrasında `.30 s` kaldırılmış durumda tutar, yükseltme hızı 5 / indirme 1.7 | Bu yöntem, ateşe izin veya hasar hazırlık evresi değildir. `FireAtWill` denetlemediğinden tutulan ateşte de uygun hedefe nişan duruşu görülebilir; yeni kural diye anlatılmamalı. Değiştirme |
| Gerçek atış | Ortak `.05 s` adım bütün saldırıları hesaplar, sonra gerçek menzilli atışlar için `LastVolley=visualClock` ve `VolleyEffects` çağrılır | Hasar, mühimmat, hedef, shared tick sırası ve RNG aynen |
| Namlu parlaması | Topçuda 2, diğer menzilli alayda 5 küçük küre; ömür `.075 s`, aralık `i × .012 s`. İlk parça tek ses işaretini taşır | Sayı, konum, gecikme, ses ve ömür aynen; uzun alev veya yeni ışık yok |
| Duman | Her namlu parçasına 1 Sphere; topçu `5.2 s`, diğerleri `4.1 s`. Başlangıç gizli, gecikme sonrası görünür. Yaşla büyür | Görünür beyaz boncuk sorununun kaynağı; yalnız sunum şekli/malzeme burada aday |
| Sürüklenme/sönme | `(.48+i×.055, .12, .13)×age` ve yukarı `.12×age²`. Büyüme `.42 + age×.72`. Alfa ilk yaklaşık `.083 s` içinde yükselir, `.34×(1-life)^.85` ile düşer | Drift, büyüme katsayısı, Born/Delay/Lifetime/visualClock aynen; uzun ve opak perde eklenmez |
| Top mermisi | Her top parçası için küçük demir küre, hedefe `.10 s` doğrusal görsel geçiş | Çarpma zamanı değildir; gerçek hasar daha önce ortak adımda hesaplanmıştır. Tracer, yay, gecikmeli hasar ekleme |
| İsabet | Hasar adımında `LastHit`; yaşayan figürlerde `.45 s` kısa eğilme, görünür kayıp figürlerinde `.65 s` yere düşüş. Namlu geri tepmesi `.18 s` | Ayrı zemine çarpma/toz patlaması veya hedef kıvılcımı yok; varmış gibi anlatma ve bu pakete ekleme |

`UpdateEffects` duraklamada ilerlemez. Savaş bitince kalan görseller mevcut akışı tamamlayabilir; henüz başlamayan ses işareti bitişten sonra çalınmaz. Bunlar da korunur.

## Üç farklı küçük görsel yol

| Kavram | Uygulama ve güçlü yanı | Sınır |
| --- | --- | --- |
| **A — Guaj barut lekesi** | Küre yerine kenarı yumuşak, bir yönde uzayan tek asimetrik duman izi. Ortak küçük alfa maskesi ve ortak düz mesh; aynı 2/5 çıkış noktasında düşük yoğunluklu izler bir yaylım ritmi oluşturur. Sıcak keten/serin adaçayı ailesi | Kamera yönüne bakan yüzeyin düz etiket gibi görünmemesi gerekir; keskin dörtgen kenar ve parlak beyaz merkez olmaz. **Seçilen dar paket** |
| **B — Oyulmuş minyatür dumanı** | Mevcut hacimli parçaları tek ortak az yüzeyli, eğik ve basık 3D duman meshine çevir. Dokudan bağımsız, sahne ışığı ve diorama hacmi korunur | Aydınlatılan sert kabuk yine katı pamuk veya taş gibi okunabilir. Şu anki boncuk sorununu A kadar doğrudan çözmez |
| **C — Gravür barut çizgileri** | Her çıkıştan birkaç parçalı fakat tek meshten oluşan ince eğik mürekkep/keten şeridi; boncuk yerine yırtılan çizgi silueti. En az örtme, güçlü tarihî çizim dili | Ateş yayı, hareket yolu ve hedef halkası da çizgisel. Dumanın yeni emir göstergesi gibi okunması ve savaşın çizgi film çizgilerine dönmesi riski daha yüksek |

## A için somut uygulama sınırı

- Yalnız mevcut duman küreleri değiştirilir; namlu parlaması, mermi, silah hazırlığı/geri tepme ve kayıp tepkisi bu ilk pakette kalır. Aynı beş/iki çıkış noktasını tek büyük sis duvarında birleştirme.
- Bir adet özgün, deterministik **64 × 32 RGBA alfa maskesi** düşünülebilir: üç geniş, önceden seçilmiş yumuşak loptan oluşan uzamış iz; küçük gren, fotoğraf, paket veya rastgele nokta bulutu yok. Ortası süt beyazı disk olmaz; alfa bütün dış kenarlarda sıfıra iner. Üç lop tek resmin dış çizgisidir, üç yeni GameObject değildir.
- Aynı mesh ve maske bütün duman örneklerinde paylaşılır. Mevcut `DioramaTransparent`/Standard yolu korunur; yeni shader veya render pipeline gerekmiyor. Duman malzemesinin rengi keten/adaçayı ailesinde ve sahne ışığında ayrıca kontrol edilir. Sırf kaynak RGB'ye bakıp beyazlık çözüldü denmez.
- Yeni maske tepe alfa değeri en fazla yaklaşık `.66`; mevcut yaş alfasını artırmaz. Tek parçanın teorik tepe alfası böylece yaklaşık `.22` olur. Aynı noktada çoklu dumanın birleşik opaklığı daha büyük olabilir; bu bir bütün-sahne örtme garantisi değildir. Genişlik/yükseklik mevcut puff ölçeğinin çevresinde kalır; daha uzun ömür veya geniş ekranı örten hacim yok.
- `UpdateEffects` duman kolunda billboard yönü ve hafif sabit şekil eğimi kullanılabilir; eski dünya eksenli döndürme düz meshi kenara çevirip yok etmemeli. Yön işlemi yalnız görsel dönüşümdür; mevcut position/drift, scale büyüme, gecikme ve alpha-age zamanlaması değiştirilmez. Tek iz için yeni simulation RNG çağrısı yapılmaz.
- Duman gölge atmaz; dünya üzerindeki yeni koyu kaplama veya koruma alanı gibi davranmaz. GUI etiketlerini veya emir kartlarını taşımak bu görevin parçası değildir.

## Yeniden kullanım ve maliyet

- Şimdiki `Primitive` her efekt için GameObject/Renderer ve ardından silinen Collider üretir; havuz veya açık adet tavanı yoktur. Ortak smoke/flash/iron materyalleri ve bir `MaterialPropertyBlock` zaten var; her puff için malzeme kopyalama yapılmamalı.
- Tek topçu yaylımı 2 duman + 2 flash + 2 mermi = 6 nesne, diğer menzilli yaylım 5 + 5 = 10 nesne. Başlangıçtaki iki tarafın altı atıcı alayı aynı kez ateş ederse toplam 24 duman, 24 flash ve 4 mermi çıkar. Bu sayı bir ömür boyu sahne üst sınırı değildir; yaş/simülasyon saatlerinin düşük FPS'deki farklı ilerlemesi ardışık nesilleri kısa süre üst üste getirebilir.
- A nesne sayısını artırmaz; duman mesh objesini Collider oluşturmadan kurabilir. Ortak mesh `meshes` listesinde, yeni maskenin tek sahibi ayrı özel alanda; `Stop` bunu açıkça kaldırır. Duman nesneleri mevcut yaş sonu silinmesine ve world temizliğine bağlı kalır. Yeni genel havuz sistemi bu küçük pakete eklenmez.

## Onay sonrası kesin sahiplik

- `TacticalBattle.cs`: yeni özel `CreatePowderMask`, `CreatePowderMesh` / `CreatePowderCloud` yardımcıları ve gerekli iki özel kaynak alanı.
- `CreateMaterials` içinde yalnız mevcut smoke kaynağının maske bağlanması; `VolleyEffects` içinde yalnız cloud yaratımı/render ayarı; `UpdateEffects` içinde yalnız duman yönü ve görsel renk/maskeyle ilgili kol. `Puff` zaman alanları, Flash/Projectile dalları ve feedback dokunulmaz.
- `Stop` içinde yalnız yeni tek maskenin temizliği; ortak mesh mevcut listeye eklenir. `PreparingVolley`, `UpdateVisual`, `TacticalBattleSimulation.cs`, komut API, UI ve yerelleştirme kapsam dışı.
- Root/verification gerekli kare betiğinin sahibidir; bu ajan oyuncu veya Unity açmaz.

## Ölçülebilir kabul

1. Aynı doğal rota ve benzer `04-artillery-volley / 05-contested-crossing` kareleri: beyaz boncuk zinciri yerine yumuşak düzensiz iz, okunur alay siluetleri, görünen dere/geçit/hedef halkası. Yeni izler emir çizgileriyle karışmamalı.
2. Phase capture eklenirse gerçek `LastVolley` değişimine göre yaklaşık `+.05 s`, `+.20 s`, `+1 s`, `+3.5 s` ve `+5.3 s` kareleri alınabilir. İlk kısa parıltı, yayılma ve sonraki sönme ayrılır; örnekleri almak için oyun zamanları veya hasar gecikmesi değiştirilmez. Mevcut genel 04 dosya adı tek başına bu zaman doğrulaması değildir.
3. Aynı yaşta duraklatılmış duman yeni drift/animasyon üretmemeli; savaş sonrası yeni volley sesi olmamalı. Yeni materyal/maske sayısı yaylım adediyle artmamalı, çıkış nesne sayısı eski 2/5 düzenini aşmamalı.
4. Root'un doğal zafer rotası ve mevcut ortak-tick/komut testleri aynı sayısal sonucu korumalı. Daha iyi görünen duman bu kanıtın yerine geçmez. İlk kareler A'yı kabul ederse B/C aynı pakete eklenmez.

## Uygulanan A ve kaynak dondurma

- Yalnız `TacticalBattle.cs` düzenlendi: iki özel kaynak alanı; `CreatePowderMask`, `CreatePowderMesh`, `CreatePowderCloud`; smoke materyalinin maske bağlaması; `VolleyEffects` içindeki cloud yaratımı; `UpdateEffects` içinde duman rengi/yönü; `Stop` maskenin açık temizliği ve mesh alanının sıfırlanması. Yeni mesh mevcut `meshes` listesiyle kaldırılır.
- Bir ortak 64×32 RGBA32 maske, mipmap ve bilinear süzme; üç sabit elips merkezi/yarıçapı `(-.30,-.10,.59,.55)`, `(.18,.13,.56,.48)`, `(.64,.22,.28,.28)`. Her leke yumuşak kenarlı `SmoothStep(0,1,Clamp01(1-dx²-dy²))×.9`; birleşim `1−(1−a)(1−b)`. Alfa çarpanı `.66`; kaynak maske kenarları sıfır. Gren veya RNG yok. Alt kısım hafif adaçayı grisi, üst kısım keten tonunda.
- Ortak mesh dört XY köşesinden oluşur, sınır `[-.55,.55]²`, iki üçgen; ön normal −Z, UV 0–1. Kamera dönüşüyle yüze bakar; yalnız roll, başlangıç konumundan türeyen ±9° eğim ve önceki `age×4` hızındadır. Duman için Collider, gölge veya yeni ışık yok.
- Dumanın başlangıç/gizleme sırası, `Born`, `Delay`, `Lifetime`, `Drift`, `Scale`, büyüme/yer değiştirme ve yaş alfa formülü aynen kaldı. Her yaylımda önceki 2/5 duman nesnesi; bir ortak materyal ve mevcut tek PropertyBlock kullanılır. Flash, Projectile, Cue, hazırlık/geri tepme, UI ve shared simulation dokunulmadı. Teorik tek-parça alfa üst sınırı eski `.34` yerine `.34×.66≈.2244` olur; çoklu örtüşme için üst sınır değildir.
- Değişen kaynak parçaları statik olarak tekrar okundu; kaynak dışı test veya derleme yapılmadı. Gerçek shader ışığı altında görünürlük, yumuşak sınır ve küçük ölçekte okunma root'un yeni player kareleriyle henüz doğrulanmalıdır.

## Eski build ile yeni fixture gözlemi

`volley-baseline-20260906-004450-276-c4666896/shots/04-early-clouds.png`, `06-developed-clouds.png`, `07-clouds-cleared-candidate.png` açıldı. 04/06'da parlak yuvarlaklar orta düşman alayının önündedir; seçili dost topçunun `x800–956, y427–465` etiketi kendi namlu/duman bölgesini örtüyor. Bunları dost nişanlı topçu atışının doğrudan görünür kanıtı diye yorumlamak hatalıdır. 07'de başka düşman alayının yeni dumanı vardır; bütün savaşta duman kalmadı anlamına gelmez.

Root/verification aynı gerçek atıştan sonra süvariyi normal yolla seçerek topçuyu açan fixture hazırlıyor. Yeni maskeyi bu görünür kaynakta ve doğal savaş karesinde değerlendirmek gerekir. Etiket kaydırma veya UI gizleme kaynak değişikliği bu pakete eklenmedi.

## İlk runtime reddi ve dar tanı

- Root `smoke-wash-20260906-005638-116-38a1dee3` için 128 Unity, 8 PNG, 13 assertion, 9 JSON ve 10 browser GREEN bildirdi. Buna rağmen 06-developed-clouds karesi gerçekten açıldığında dost topçunun iki izi ve orta düşmanın izleri opak krem dikdörtgenlerdir. Teknik gate görsel kabul anlamına gelmez; bu sürüm görsel olarak reddedildi.
- Yeni süvari seçimi dost topçuyu açıyor; bu kez kendi iki cloud'u görülebilir. Eski küreler de benzer biçimde süt beyazıydı. Sorun yalnız yeni maskenin estetiği diye yorumlanamaz; eski malzemenin şeffaflığı da kuşkulu.
- Statik kaynakta `DioramaTransparent` shader id46; `BuildTools.EnsurePlayerRenderResources` shader adının Standard, fade keyword'ünün açık ve Standard'ın always-included olduğunu zorunlu doğruluyor. Materyalde _Mode2, SrcAlpha5 / OneMinusSrcAlpha10, ZWrite0 ve _ALPHABLEND_ON var. `_MainTex` maskeye atanıyor, `_Color` MPB ile yaş alfa değerini alıyor. Bunlar GPU'da doğru varyantın gerçekten çizildiğini kanıtlamaz. `strictShaderVariantMatching=0` var; shader fallback yalnız olasılık, doğrulanmış neden değil.
- Root izniyle `LogPowderRenderer` eklendi: yalnız `L.IsReviewSession` ve ilk duman güncellemesinde; gerçek renderer'ın shader/support, texture adı/boyutu, keyword, blend/depth/queue değerleri ve MPB roundtrip rengini yazıyor. Normal oyuncu logu değişmez. Bir bool Stop'ta sıfırlanır. Tanı sürümü render davranışını değiştirmedi; hata düzeltildi diye sunulmaz.
- Root ayrıca gerekirse yalnız duman için küçük alfa shader'ı ve açık Resources materyaline izin verdi; henüz yaratılmadı. Ortak diğer materyaller/BuildTools değiştirilmedi. Etiket audit'i `regiment-label-readability.md` içinde ayrı tutulur.
- Yeni 04 ve 07 de açıldı: 04'te kendi iki küçük köşeli iz zaten görünür; 06'da büyüyerek açık dikdörtgen olur. 07'de o dost topçu nesilleri görünmez, sağ üst düşmandaki yeni nesil yine küçük beyaz dörtgendir. Zaman çizelgesinin ilerlemesi ile şeffaf biçimde sönme ayrı konulardır; son kare doğru alpha fading kanıtı değildir.

## Gerçek tanı sonucu ve özel alfa yolu

`smoke-diagnostic-20260906-011014-153-05cbab94/REPORT.md` okundu: fresh build, 175/175 Unity, 8 kare, 13 assertion, 9 state, 10 browser GREEN. `player.log:28` gerçekten okundu: shader Standard / supported True, doğru 64×32 texture, yalnız _ALPHABLEND_ON, queue3000, mode2, src5, dst10, zwrite0; materialColor(.86,.85,.78,.34), ilk MPB rengi(.86,.85,.78,0), texture inherited. 06 karesi yeniden açıldı ve opak dörtgenlerin sürdüğü görüldü. Bu, metadata'nın nominal olduğuna rağmen o runtime çizim yolunun başarısız olduğunu gösterir; variant stripping kesin neden olarak kanıtlanmadı.

Root'un açık izniyle yapılan düzeltme:

- Yeni `Assets/Resources/BattleMaterials/PowderWashAlpha.shader`, shader adı **PowerAboveAll/PowderWashAlpha**. Tek `POWDER_WASH` pass; explicit `Blend SrcAlpha OneMinusSrcAlpha`, `ZWrite Off`, `Cull Off`, Transparent queue/tag. Fragment doğrudan `tex2D(_MainTex, uv) × _Color`; yeni ışık veya yüzey aydınlatması, keyword varyantı, gölge, paket ve fallback yok. Aynı maskenin alfası ve aynı yaş alfası doğrudan çarpılır.
- Yeni `Assets/Resources/BattleMaterials/PowderWash.mat` shader GUID'sine açıkça bağlıdır; iki yeni `.meta` da eklendi. Runtime `Resources.Load<Material>("BattleMaterials/PowderWash")` üzerinden tek ortak materyal kopyası üretir; shader adı ve destek durumu doğrulanır. Eksik/yanlış kaynak opak başka shadera sessizce düşmez. Diğer `MakeMaterial` kullanımları ve eski üç Diorama materyali değişmedi.
- 64×32 maskenin içeriği, mesh, sprite adedi, konum/sürüklenme/büyüme, `Born/Delay/Lifetime`, yaş alfa formülü, Flash/Projectile/Cue, simülasyon ve RNG aynı. `_BaseColor` gereksiz MPB girdisi kaldırıldı; gerçek `_Color` kaldı. Review tanısı renderType/pass adını da yazar. Yeni shader'da Mode/Src/Dst/ZWrite property'lerinin "absent" olması beklenir: bu değerler artık shader kodunda sabittir.
- Değişen C#/shader/mat/meta parçaları statik olarak tekrar okundu. Root'a BuildTools doğrulaması için tam yollar/isim iletildi; bu ajan BuildTools'u değiştirmedi. Yeni yolun yumuşaklığı ve görünür yoğunluğu henüz player görüntüsüyle kabul edilmedi.

## Alfa yolunun gerçek kabulü ve görünürlük ayarı

Root `powder-alpha-20260906-011819-635-7d706a18` için 176 Unity / 8 PNG / 13 assertion / 9 state / 10 browser GREEN bildirdi. 04,06,07 gerçekten açıldı. Artık opak dörtgen yok; dost topçunun açıkta kalan iki duman izi 1440×900 ölçekte zeminle fazla karışıyor. Saydamlığın çalışması oyuncunun atışı rahat görmesi anlamına gelmedi. Özellikle 06'da eski geometrinin yaklaşık30–35 px boyutu yeterli; şekli veya sahne hacmini büyütmek gerekli görünmüyor.

Root'un onayladığı tek dar ayar uygulandı: runtime başlangıç rengi ve MPB tint'i, template materyali ve shader varsayılanı **(.96,.95,.88,.46)** olacak şekilde tutarlı değiştirildi. Yaş alfa çarpanı `.34→.46`; ilk yükselme `Min(1,age×12)` ve sönme `Pow(1-life,.85)` aynen. Maske çarpanı `.66`, bütün maskenin/meshin şekli ve boyutu, 2/5 nesne, clock, drift, büyüme ve lifetime değişmedi. Teorik alfa üst sınırı `.46×.66=.3036`; `.34` ile kıyas eski kaynak katsayısıyla yapılır, hatalı Standard runtime görüntüsünün gerçek saydamlığıyla değil.

Bu, yeni görünürlük adayıdır; yeterince görünür olduğu henüz player kareleriyle doğrulanmadı. Eşzamanlı onaylı Dumas crop ayrı `dumas-crop-review.md` içinde kayıtlı; smoke görevi etiket/HUD yerleşimine genişletilmedi.

## Son birleşik koşunun gerçek incelemesi

`military-art-final-20260906-012710-424-48b0deff/REPORT.md` okundu: tamamlanmış GREEN, 176/176 Unity, fresh build, 21 PNG / 38 assertion / 21 state, 10 browser, süre203s. Bu ajan koşuyu başlatmadı. `player.log:28–29` iki dünya kurulumunda doğru `PowerAboveAll/PowderWashAlpha`, `POWDER_WASH`, Transparent ve (.96,.95,.88,.46) materyal değerlerini gösteriyor; bu tek başına genel bellek sızıntısı olmadığı kanıtı değildir.

`smoke-04-early-clouds`, `smoke-06-developed-clouds`, `smoke-07-clouds-cleared-candidate` gerçek tam kareleri açıldı. Dost topçunun iki izi 06'da namlular üzerinde çok hafif açık yayılma olarak seçilebiliyor; 04'te halen zor fark ediliyor. Beyaz dörtgen, opak boncuk zinciri veya birlikleri örten duvar yok. Sonucun gücü abartılmamalı: saydamlık ve örtmeme düzeltmesi kabul edilir, dramatik/çok belirgin bir yaylım vurgusu tamamlandı sayılmaz. İlk adaydan daha açık olmasına rağmen etki bilinçli olarak sakin kalıyor; şimdilik yeni bir kör alfa/ölçek artışı önerilmedi. 07'de düşmanın yeni nesli ile dostun eski neslinin temizlenmesi birbirine karıştırılmadı.
