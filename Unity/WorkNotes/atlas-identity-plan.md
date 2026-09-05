# Atlas kimliği — salt okunur sanat önerisi

## Gerçek gözlem

İncelenen kareler:

- `output/verify/painted-atlas-review-20260905-220606-170-49890db5/shots/01-atlas-ru.png`
- `output/verify/six-week-dx11-20260905-222730-211-0abee00d/shots/11-week-six-journal-ru.png`

Palet ve metin alanları tutarlı;12bölgenin merkezi rahat bulunuyor. Kentler yaklaşık34×20px açık duvar/mavi çatı kümeleri. Paris biraz geniş, kuzeybatı ve sınır için küçük ayrıntılar var, fakat normal bakışta çoğu aynı kuleli köy gibi okunuyor. Ek ayrıntı çok küçük kalmış. Deniz/nehir/dağ çizgileri zaten mevcut; daha çok rastgele tarama eklemek temel sorunu çözmez. Paris/Orléans sancağı yakın ad alanına yaklaşıyor; büyüyen bir simge bu çakışmayı artırabilir.

Kaynak: `CampaignMap.MakeCity` zaten capital/coastal/frontier/grain ailelerine ayrılıyor; `CityEngraving` tek vertex-renk mesh'i üretir. Görsel kimlik ilk kez icat edilecek bir altyapı değil, mevcut ailelerin silüetlerinin ayrıştırılmasıdır. `CabinetHud.Atlas` ad için merkezden y−34, kent altyazısı için y+14 kullanır; Champagne adı ayrıca18px yukarıdadır. `Province` emirleri başlık ve kent adından hemen sonra sabit tutar.

## Üç maddi olarak ayrı yol

1. **Kent silüeti aileleri.** Aynı yerleşim noktalarında12elle düzenlenmiş küçük işaret. Fark pencere sayısında değil; yatay/dikey oran, tek/çift odak, çatı ritmi ve boşluktadır. Tek koyu kontur, krem yüzey, az yumuşak mavi gölge. Mevcut mesh üreticisi yeterli; yeni bitmap veya etkileşim gerekmez.
2. **Bölgenin peyzaj imzası.** Kentleri aynı tutup her hücrenin boş kısmında seyrek, elle yerleştirilmiş bahçe/tarla/kıyı/rölyef grubu. Hücre sınırında kırpılmalı, dinamik harita renklerini örtmemeli. Bölgenin bütün alanı karakter kazanır; fakat12hücrede çizgi yoğunluğu ve asker rotasıyla çakışma riski daha yüksek.
3. **Seçili bölgenin küçük resimli başlığı.** Haritayı koruyup sol üst mevcut başlık alanında seçilen bölgeye ait tek boyanmış silüet/vinyet. Aynı anda yalnız biri görünür; büyük resim okunur ve anlatıcıdır. Ancak mevcut212px başlıkta metne yer bırakmak zor, sabit emirleri aşağı itmeden12resim için iyi bir sunum sınırı gerekir. İlk atlas paketi için uygun maliyet/fayda vermiyor.

## Önerilen sınırlı sonraki görev

**Birinci yol: mevcut12kent gravürünün silüetini ayrıştır.** `MakeCity`, `EngravedHouse` ve yeni küçük çizim yardımcılarıyla sınırlı; veri, sınır, collider, yol, ikmal ve harita kipleri değişmez. Kıyı/nehir ayrıntısını çoğaltma bu pakete alınmaz.

İlk çizim taslağı — bunlar belgelenmiş tarihî yapılar veya yeni üretim/liman mekaniği değildir:

| Bölge | İmza |
| --- | --- |
| Île-de-France | Geniş yatay devlet cephesi, tek alçak merkez; mevcut Paris vurgusu |
| Bretagne | Ayrık, alçak ve asimetrik üç çatı; Rennes işaretine sahte liman eklenmez |
| Normandie | Uzun eğimli çatı ve ince tek kule; Bretagne'den dikey oranla ayrılır |
| Picardie | Geniş basit ambar çatısı, küçük ek yapı; yüksek merkez kule yok |
| Champagne | İki ayrı çatı kümesi arasında açık boşluk; tek ince kapı vurgusu |
| Lorraine | Kompakt iki kare kule ve okunur kapı boşluğu |
| Bourgogne | Tek yüksek kırma çatı, yanında alçak uzun yapı |
| Orléanais | Yatay kemerli geçit ve küçük karşı ağırlık; belirli gerçek köprünün kopyası değil |
| Poitou | Alçak yapı ve ince yel değirmeni silüeti; yeni üretim simgesi gibi etiketlenmez |
| Guyenne | Uzun alçak sıra, bir uçta küçük kule; Lorraine'in simetrisini kullanmaz |
| Languedoc | Geniş sade çatı ve üç kısa kemer; sıcak ışık ailesi |
| Provence | Kademeli alçak çatı sırası, ince bacalı profil; belirli tarihî liman iddiası yok |

## Yerleşim ve kabul

- Mevcut kent merkezleri kullanılır. Tercihen mevcut34×20px ayak izi; en fazla40×22px. Önce fark silüette sağlanır, sadece büyütmek çözüm sayılmaz.
- Yeni işaret mevcut ad veya kent altyazısına4px'den fazla yaklaşırsa yalnız ilgili ad ofseti düzenlenir. Yeni genel satır/bant eklenmez, emirler aşağı itilmez. Paris ve Orléans'ta sancak ayrıca görülür.
- Ortak renk ailesi korunur. Bölge başına ayrı renk kodu eklenmez; denetim/tehlike/vergi renkleri mekanik anlamını korur.
- RU/TR genel atlas, ordukipi ve seçili Paris/Orléans/Champagne kareleri incelenir. Normal1440×900 ölçekte en az Paris, Lorraine, Bretagne ve Poitou, adı kapatılsa da farklı şekiller olarak ayırt edilir.
- Gösterge/rota/seçim/sınır okunurluğu önceki karelerden kötüleşmez. Pencere düzeyindeki süs görünmüyorsa çıkarılır. Gerçek anıt, arma, sınır veya tarih iddiası eklenmez.
- Bu not yalnız öneridir. Kaynak düzenlenmedi; Unity/build/player çalıştırılmadı. Rol arayüzünün mevcut kaynak dondurması korunur.

## Uygulamaya hazır çizim sözleşmesi — 23:03 sonrası hazırlık

Root rol checkpoint'ini bitirene kadar bu bölüm yalnız çizim tarifidir; Assets değişmez. Yalnız `CampaignMap.MakeCity` ve şehir çizim yardımcıları değiştirilecek. Paneller, kent merkezleri, ad yerleşimleri, ordu sancağı, nehir ve kıyı çizgileri bu pakette düzenlenmez.

### Koordinat ve malzeme

- Koordinatlar mevcut `CityEngraving` yerel `(x,z)` düzleminde; pozitif z ekranda yukarıdır. Yükseklik alanı `.12` ve `city.localPosition = World(seed.Point,.14)` korunur.
- Çizimin bütün noktaları `x ∈ [−1.55,1.55]`, `z ∈ [−.84,.84]` içinde kalır: mevcut kamera ölçeğinde yaklaşık36×20px. Bu sınır dış kontur kalınlığını da kapsar. Daha büyük kent veya yeni ad ofseti gerektiren tarif uygulanmaz.
- `EngravedHouse` derinlik payı `.15`, sağ çatı çıkması `.06`, sırt yüksekliği ek payı `.10` kullanıyor. Aşağıdaki H değerleri bu payları hesaba katar. Yeni yardımcılar da sınır dışına taşmaz.
- Var olan `ink #3E5A4E`, `wall #F3E7CA`, `shade #B79D71`, `roof #60868B`, `roofLight #A0BEC0` kullanılır. Büyük düzlem ve net koyu kontur, görünmeyen pencere sayısından önemlidir. Yeni renk tablosu gerekmez.
- Ana dış çizgi `.065` (yaklaşık.75px), büyük tanıtıcı yelken/kemer çizgisi `.09`, ikincil çizgi `.04–.045`. Sırf süs için çok ince yeni tarama yoktur.

### Kullanılacak küçük yardımcılar

- **H(x,b,w,h,r)**: mevcut `EngravedHouse(d,x,b,w,h,r)` aynen. Gövde yüksekliği h, çatı yüksekliği r. Bazı tariflerde mevcut kapı/pencereler fazla görünürse kaldırılabilir; silüeti oluşturmak için yeni pencere eklenmez.
- **R(x,b,w,h,color)**: `Shape(color,x,b,x+w,b,x+w,b+h,x,b+h)`; sadece dış kenarlar gerekiyorsa `Line`. Gölge yan yüzü için `.12`x/`.08`z derinlik yeterli.
- **T(x,b,w,h)**: düz kare kule. R gövde; sağ yan yüz R yerine dörtgen `(x+w,b),(x+w+.12,b+.08),(x+w+.12,b+h+.08),(x+w,b+h)`. Üst kapak `z=b+h..b+h+.12` ve koyu dış çizgi. İki büyük üst köşe yükseltisi en çok `.08`; çok küçük mazgal sıraları çizilmez.
- **A(cx,b,w,h,filled=false)**: belirgin kemer. İki `.12` kalın ayak, yüksekliği `h*.45`; üstte6dörtgenlik yarım elips halkası. Dış yarıçaplar `(w/2,h*.55)`, iç yarıçaplar `(w/2−.12,h*.55−.12)`, merkez `(cx,b+h*.45)`. `filled=false` boşlukta mesh olmaz. İç boşluk gerekiyorsa yalnız `filled=true` koyu düz kemer şekli koyulur; yeni geçilebilirlik mekaniği değildir.
- **B(x,b,w,h,n)**: n adet açık A kemeri; merkezleri `x+(i+.5)*w/n`, kemer genişliği `.94*w/n`. Üstte `R(x−.04,b+h,w+.08,.12,wall)` güverte. Kemeri tek içbükey fan poligonu yapmak yerine yukarıdaki dörtgenlere ayır.
- **M(cx,b)**: değirmen silüeti. Daralan gövde `(cx−.30,b),(cx+.30,b),(cx+.18,b+.95),(cx−.18,b+.95)`; küçük çatı tepesi `b+1.12`. Kanat merkezi `(cx,b+.96)`; iki çaprazın uçları merkezden `(±.43,±.43)`, `.09` geniş koyu çizgi üzerinde daha dar açık iç çizgi. En küçük ayrıntı kanat; simgenin yanına buğday veya yeni kaynak işareti eklenmez.

### On iki açık tarif

İşaretler aynı tabana oturur. H çağrıları arkadan öne verilmiştir; daha alçak ön yüz son çizilir. Silüetleri sınır içinde tutmak için bu ilk değerlerle başlanır.

| Kimlik | Çağrılar / belirgin çizgi |
| --- | --- |
| `ile` | H(−1.40,−.72,.68,.58,.18); H(.64,−.72,.64,.58,.18); H(−.42,−.73,.84,.98,.20). Son olarak yatay korniş `Line(ink,.065,−1.38,−.24,1.30,−.24)`. Üç parçalı geniş cephe ve tek merkez. |
| `brittany` | H(−1.44,−.73,.62,.44,.22); H(−.52,−.56,.66,.55,.20); H(.58,−.76,.70,.35,.30). Boşlukları doldurma; tek kule ve eski yelken kaldırılır. Üç ayrı düşük kütle. |
| `normandy` | H(−1.44,−.75,1.65,.40,.40); T(.72,−.74,.40,1.22). Uzun tek çatı ve sağda ince düz kule; kulenin dış tepe sınırı.68. |
| `picardy` | H(−1.43,−.73,2.02,.55,.44); H(.90,−.75,.38,.30,.14). Büyük tek ambar üçgeni, çok küçük sağ ek. Ana gövdede kapı iki kısa çizgiyle geniş yapılabilir; tarla çizgisi eklenmez. |
| `champagne` | H(−.80,−.55,.50,.60,.22); H(−1.44,−.76,.70,.40,.24); H(.60,−.75,.62,.55,.26). Arada `R(−.04,−.76,.36,.38,wall)` ve tek koyu kapı. Üst orta boşluk açık kalır; Lorraine gibi birleşik duvar yapılmaz. |
| `lorraine` | T(−1.24,−.73,.55,1.12); T(.55,−.73,.55,1.12); R(−.69,−.73,1.24,.49,wall); A(−.05,−.73,.40,.40,true). İki eş kule, ortada alçak duvar ve okunur kapı. |
| `burgundy` | H(−.84,−.74,.90,.88,.45); H(.15,−.76,1.15,.33,.17); H(−1.43,−.78,.44,.30,.18). Solda yüksek geniş çatı, sağda uzun alçak kanat; ikinci yüksek kule yok. |
| `orleans` | H(−.43,−.12,.65,.44,.20); B(−1.30,−.76,2.50,.58,3). Alt kenarda üç gerçek görsel boşluk, üstte küçük yapı. Mevcut nehir hattına yeni bağlantı veya hareket kuralı eklenmez. |
| `poitou` | H(−1.43,−.76,.93,.40,.24); M(.65,−.76). Değirmen kanatlarının üst noktası.63, sağ noktası1.08; bütün grup mevcut ayak izinde. Kanatları animasyonla veya kaynak durumuyla bağlama. |
| `guyenne` | H(−1.43,−.75,.65,.41,.18); H(−.76,−.75,.66,.44,.16); H(−.08,−.75,.65,.40,.20); T(.85,−.74,.34,1.02). Üç alçak testere dişi çatı, tek uç kule. Normandie'nin tek uzun çatısı burada kullanılmaz. |
| `languedoc` | Ön cephe R(−1.30,−.75,2.42,.59,wall). Çatı dörtgeni `(−1.40,−.16),(−.96,.25),(.84,.25),(1.30,−.16)` ve sağ gölge payı. Koyu A boşlukları merkezler−.78,−.10,.58; b−.75,w.40,h.43,filled=true. Yatay çatı sırtı ve üç kemer, yüksek kule yok. |
| `provence` | R(−1.42,−.76,.74,.38,wall); R(−.67,−.76,.79,.60,wall); R(.13,−.76,.93,.82,wall). Her çatıda düz `.09` mavi kapak; son yapının sağında R(1.10,−.35,.14,.67,shade) ince baca. Basamaklı yatay teraslar; Normandie/Guyenne'deki sivri çatı dizisi yok. |

### Statik ve görsel doğrulama

- Her şehir tek `MeshRenderer`, aynı `cityInkMat`, gölge kapalı ve collider yok. Yeni Shape/Line örnekleri mevcut `owned` mesh ömrünü kullanır.
- Nokta aralığı ve finite kontrolünü geçici statik/Editor gözlemle doğrula; beklenen12renderer kalır. Yeni eklenen kemerler içbükey üçgen fanıyla yanlış kapatılmaz.
- İlk ekran hedefi tüm12seçenekte aynı ölçek; özellikle `ile`–`brittany`–`lorraine`–`poitou` ayrımı normal çözünürlükte görülür. Kalan8'i ayırmak için mikroskopik ayrıntı eklemek yerine çatı oranı değiştirilir.
- Önce RU/TR tam atlas; sonra Champagne seçimi ve Paris/Orléans ordusu. Kontrol, ordu, huzursuzluk kiplerinde okunurluk aynı kalmalı. Başka UI zaten kabul edilmiş olduğundan yeni sembol sığmazsa sembol küçültülür, panel veya ad düzeni bozulmaz.

## İlk uygulama kaynağı hazır

- `b90c7ae` checkpoint'i ardından root kaynak düzenlemesine izin verdi. Yalnız `CampaignMap.cs` şehir bölümü değişti; yukarıdaki12tarif uygulandı. Kıyı/harita boyutu/renk skalası/sınır/etiket/UI/Core değiştirilmedi.
- Yeni yardımcılar: EngravedBlock, EngravedTower, EngravedArch, EngravedBridge, EngravedMill. Ev yardımcısının dış çizgisi.055→.065 oldu. Genel denizci işaretleri kaldırıldı; Rennes kentinde artık ilişkisiz yelken bulunmaz.
- Bir şehir=bir mesh/renderermimarisi, aynı malzeme ve mevcut owned ömrü korundu. Kemer halkası6dışbükey dörtgene bölünür. Açık Orléans kemerlerinde fan dolgusu yok; Lorraine/Languedoc koyu kapı açıklıkları ayrı dolu dışbükey şekillerdir.
- Elle koordinat kontrolünde en geniş H sağ ucu1.51; çizgi payıyla yaklaşık1.543. En sol çatı çizgisi yaklaşık−1.533. Yeni figürlerin yüksekliği önceki tek kuleyi aşmıyor. Bu matematiksel kaynak incelemesidir; gerçek piksel/etiket kabulü henüz yapılmadı.
- `git diff --check` temiz. Unity/player/build/commit başlatılmadı; kaynak root'un toplu derlemesi ve gerçek kareler için dondurulacak.
