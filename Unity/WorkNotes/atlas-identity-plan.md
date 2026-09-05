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
