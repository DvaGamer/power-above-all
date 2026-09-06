# Savaş polish çalışma notları — 5 Eylül 2026

- Kullanıcı POLISH_PLAN.md çalışmasını yeniden başlattı. Kapsam yalnızca TacticalBattle.cs, battle.json ve savaş görselleridir; yeni mekanik/denge değişikliği yoktur.
- Üç yön değerlendirildi: canlı gravür levhası (çok düz), sürekli yakın kamera ile sinematik meydan (hat okumasını zayıflatıyor), masa üzerindeki askerî arazi dioraması. Seçim üçüncü yön: gerçek mesafe ve cepheyi ince pirinç emir çizgileriyle gösteren, yumuşak toprak/orman renkleri taşıyan yaşayan minyatürler.
- Mevcut kodda Shoot hasarı hemen uygular; yaylım parlaması 0,24 sn, hedef tepkisi 0,38 sn gecikir. Bu görsel uyumsuzluk mevcut hasar hesabını değiştirmeden düzeltilecek.
- MaterialPropertyBlock alan başlatıcısında oluşturulmuyor; Begin içinde ilk kullanımdan önce oluşturuluyor. Bu yaşam döngüsü düzeltmesi korunacak.
- Unity çalıştırılmayacak; ana ajan editör/kampanya durumunu koruyarak entegrasyon doğrulamasını yönetiyor. Görsel kalite ve ses dinleme henüz doğrulanmış değildir.
- Uygulanan ilk kaynak turu: ayrı ve duraklamada donan görsel saat; mevcut doldurmanın son 0,6 saniyesinde silah hazırlığı; Shoot anında namlu/hasar tepkisi; sürüklenen asimetrik ortak duman; adım/çizme hareketi; arazide seçili cephe, menzil yayı, verilmiş hareket çizgisi ve hedef işareti. Görseller savaşın RNG akışını kullanmaz.
- Yeni rapor gerçek başlangıç, kalan ve kayıp sayısını gösterir. BattleOutcome.EndingMorale değişmedi; yeni isteğe bağlı BattleSetup.CampaignMoraleAfterBattle delegesi yalnızca çekirdeğin sefer dönüşü moralini ayrıca gösterir. AcceptOutcome içindeki delivered koruması ve Stop öncesi callback kopyalama korunuyor.
- Ana ajan genel Runtime + Editor Roslyn derlemesinin geçtiğini bildirdi. Bu ajan tarafındaki `node output/verify-unity-compile.cjs` komutu bash PATH içinde node bulunmadığından çalışmadı; derleme hatası değildir. Ana ajan kullanıcıya Play açarken C# değişiklikleri geçici olarak tutuluyor.
- Kaynak kontrolü: battle.json içindeki 58 anahtar benzersiz; RU/TR değerleri boş değil ve biçim yer tutucuları eşleşiyor. TacticalBattle.cs içindeki 55 statik battle anahtar referansının tamamı tabloda var. `git diff --check` bu iki dosya için geçti.
- Son runtime turu: çoklu hareket emrinin merkezi yalnızca emir alabilen birliklerden hesaplanır; dönüş morali Finish içinde bir kez hesaplanır. Sakin konvoy yolu, tarla izleri ve uzaktaki çiftlikler eklendi; RNG ve arazi kuralları değişmedi. Görünen yardım bunların dekoratif olduğunu söyler.
- Root ViewLayout ortak tuvaline kamera çerçevesi, pointer, seçim yarıçapı ve alay etiketleri bağlandı.
- Bu ajan Windows Node yoluyla gerçek Unity DLL derlemesini iki kez başarıyla çalıştırdı: Runtime 9 dosya, Editor 2 dosya PASS. Son `git diff --check` temiz. Bu derleme test assembly'sini kapsamaz.
- Root izniyle BattlePresentationTests.cs ve .meta eklendi: 5 EditMode NUnit testi; Begin/Stop native yaşam döngüsü, görsel adımlarla/adımsız aynı 1.000 tick savaş, ortak hasar/parlama/tepki zamanı, donmuş görsel saat ve tek sonuç teslimi/geri çekilme kaybı. Testler GameApp veya kayıtları kullanmaz. EditMode temizliği için ReleaseObject yalnızca Play dışında DestroyImmediate kullanır.
- NUnit testleri bu ajan tarafından çalıştırılmadı; ana ajan Runner sonucunu alacak. Savaşın gerçek iki dil ekran ve ses incelemesi hâlâ ayrı kabul adımıdır.
- Ana ajan ilk gerçek EditMode koşusunda toplam 21/21 testin, bu ajanın 5 savaş testi dahil, geçtiğini bildirdi. İlk Windows oyuncu derlemesi de ana ajan tarafından başarılı tamamlandı.
- Son incelemede OnGUI'de yaratılmış fakat henüz Update görmemiş yaylım efektinin, hemen duraklatıldığında ilk sesini oynatabileceği uç durum bulundu. Ana ajanın derleme sonrasındaki izniyle UpdateEffects başına paused koruması ve 6. savaş regresyon testi eklendi. Bu ek test ve son satır ilk 21/21 / ilk oyuncu derlemesi kanıtına dahil değildir; yeni koşu bekler.
- Cabinet ajanının bağımsız terim incelemesi doğrultusunda konvoy ödülü RU «снаряжение» / TR «teçhizat» olarak düzeltildi. Ödül MilitarySupplies stoğuna gider; yüzde Supply/ikmal ile karışmamalıdır. Bu yalnızca çeviri düzeltmesidir.

## Gerçek Windows oyuncusu shader düzeltmesi

- Root yeni görev açtı: ilk Windows oyuncusunda savaşa geçiş `ArgumentNullException(shader)` ile duruyor. `output/player.log` satır 46 ve 59 bu hatayı, MakeMaterial satır 556'yı gösteriyor. Editör testleri bu player stripping hatasını yakalamamıştır.
- MakeMaterial yalnızca Shader.Find(Standard), sonra projede kullanılmayan URP/Lit arar. GraphicsSettings.asset dosyasında Standard açıkça listelenmiyor; eski BuildTools SerializedObject değişikliği kalıcı sonuç için tek dayanak olamaz.
- Seçilen çözüm: opak, saydam ve emissive Standard varyantlarına açık referans veren Resources malzeme şablonları; runtime bu şablonları kopyalar. BuildTools şablonları/başvuruları doğrular ve gerçekten kullanılan shader'ların dahil edilmesini kalıcı olarak doğrular. Yeni render pipeline kurulmaz.
- Kurulu Unity 6000.3.23f1 yerel API XML'lerinde AssetDatabase.SaveAssetIfDirty, EditorUtility.SetDirty, GraphicsSettings.GetGraphicsSettings ve SerializedObject.ApplyModifiedProperties belgeleri bulundu. Root editör/build çalıştırmayı üstlenir.
- Kaynak düzeltmesi hazır: Resources/BattleMaterials içinde üç açık Standard malzemesi (opak, _ALPHABLEND_ON ile fade, _EMISSION); TacticalBattle ilgili şablonu kopyalar. URP bağımlılığı kaldırıldı; şablon eksikse Standard → Unlit/Color → Sprites/Default denenir, hiçbir shader yoksa açık kaynak adıyla hata verir.
- BuildTools artık gerçek GraphicsSettings.GetGraphicsSettings nesnesini kullanır; kullanılan Standard/Unlit/Color/Sprites/Default shader'larını dahil eder, Unity yerel serileştiricisiyle ProjectSettings'i kaydeder ve shader GUID/fileID başvurularının diske yazıldığını kontrol eder. Resources şablonlarının shader/keyword başvuruları build öncesinde doğrulanır. IPreprocessBuildWithReport aynı kontrolü menü dışı build yollarına uygular.
- Bu turun diff --check kontrolü temizdir. Root Unity editörünün dışarıdan kapandığını ve ortak dizinde başka bir aracın untracked dosyalarını gördüğünü bildirdi; bu nedenle hiçbir editör, derleme veya test başlatılmadı. AutoShots.cs, tools/verify.ps1 ve tools/shots.script değiştirilmedi. Son shader yamasının derlenmesi, materyal ithali, Windows build ve gerçek savaşa giriş doğrulaması root koordinasyonu sonrası bekliyor.

## Editörde bağımsız savaş doğrulaması için sınırlı senaryo

- Kampanyayı kullanmadan geçici kök + kamera + devre dışı TacticalBattle oluştur; Begin çağır. MaterialPropertyBlock örneği ve dünya nesneleri kurulmalı, alan başlatıcısı hatası olmamalı.
- Aynı seed ve aynı setup ile iki ayrı 1.200 tick koşusu: yalnızca Simulate(.05) çağır; bütün alayların Men, Morale, Ammo, Reload, Position, Routed değerlerini ve outcome alanlarını karşılaştır. Gerçek oyuncu kampanyasının RNG/durumunu değiştirme.
- Bir hazırlık/atış pozisyonunda Shoot çağrısından hemen sonra hedef Men azalmış, LastHit ve LastVolley görsel saate eşit olmalı; ilk efektin Born alanı aynı saate eşit olmalı.
- Efektleri ve UpdateVisual(regiment,0) çağrılarını görsel saat değişmeden iki kez çalıştır; duman pozisyonu/ölçeği, figür pozu ve bayrak pozu aynı kalmalı. Saat ilerletilip pozitif dt verildiğinde duman ve figür hareket etmeli.
- Finish ardından AcceptOutcome iki kez çağır; callback sayısı 1 olmalı, sonuç kayıpları 0..Troops aralığında olmalı. Stop ve DestroyImmediate ile yalnızca geçici doğrulama nesnelerini temizle.
- Tam Play/iki dil ekran incelemesi, ses dinleme ve oyuncu derlemesi bu kaynak kontrolüne dahil değildir.
