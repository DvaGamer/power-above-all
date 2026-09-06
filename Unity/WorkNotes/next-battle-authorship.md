# Sonraki savaş görseli — bahçeye çizilmiş bir siluet

6 Eylül 2026. Yalnız tasarım ve kapsam önerisi; Assets değişmedi, Unity/oyuncu/derleme başlatılmadı. Plan uygulama yetkisi değildir. Dayanak `ART_DIRECTION.md`: güneşli guaj atlas, yaşayan minyatürler, hoş aydınlık/koyu dengesi ve bilinçli büyük şekiller.

## Gerçek görüntü dayanağı

İki yeni tam 1440 × 900 kare açıldı:

- `output/verify/resistance-deployments-20260906-043417-446-99c65d8d/shots/03-reinforced-deployment.png`
- `output/verify/resistance-natural-victory-20260906-045323-372-ede571f9/shots/05-contested-crossing.png`

Mevcut tepe yıkaması, kuru geçit ve yandan alay etiketleri korunmaya değer. Açık çayırda birimlerin okunacağı boşluk var; bütün alanı ayrıntıyla doldurmak ters etki yapar. Bahçe açıkça düzenli bir dikim olarak okunuyor, fakat her ağaç aynı üç yuvarlak parçanın kopyası gibi. Özellikle ilk karede 4 × 5 tekrar güçlü. İkinci karede piyade ve süvari taçların arasına giriyor; yeni görselin daha geniş veya yoğun taçlarla bu birimleri saklamaması gerekir.

Kaynakta `BuildLandscape` bahçeyi 20 gövde ve ağaç başına üç `Sphere` ile kuruyor. `BuildCreekReach` iki dar mavi şerit, hedef sınırı ise 40 küçük pirinç küp. Aşağıdaki üç seçenek mevcut öğeyi değiştirir; yeni süs nesnesi eklemez.

## Üç küçük ve farklı yön

| Yön | Görüntüdeki somut sorun / değişiklik | Okunabilirlik riski |
| --- | --- | --- |
| **A — Üç çizilmiş meyve ağacı silueti** | Aynı üç kürenin 20 kez tekrarı yerine üç sakin, asimetrik taç profili. Aynı koyu adaçayı kütlesi içinde yuvarlak-geniş, yukarı daralan ve yana omuz veren biçimler; tek bahçe ailesi gibi. | Yeni tek taç, sınırı büyümese bile mevcut parçalar arasındaki boşluğu doldurabilir. Yoğun çatışmada askerleri daha fazla örtmemesi gerçek kareyle kontrol edilmeli. |
| **B — Dere yüzeyine yönlü renk** | Derenin uzun, hemen hemen tek mavi şerit görünümünü aynı dar geometri içinde serin merkez / daha açık kısa yüzey yönüyle yumuşatmak. Köpük, taş, su sıçraması veya kıyı büyütme yok. | Yaklaşık birkaç düzine piksel genişlikte güçlü açık çizgi, sudan geçen başka bir yol veya hedef oku gibi görünebilir. Küçük fark yetmezse daha çok efekt eklemek kapsamı bozar; seçilmez. |
| **C — Pirinç hedef işaretinin daha yumuşak çizgisi** | Kuru geçit etrafındaki 40 kare taneciği, aynı yarıçap ve aynı açıklıklarda, tek düz kesintili fırça çizgisine dönüştürmek. Hedef okunurluğunu koruyan küçük biçim değişikliği. | Burası işlevsel bir yakalama sınırı. Daha zarif fakat daha zayıf çizgi, çarpışma sırasında amacı gizleyebilir; yeni halka biçimi daha büyük oyun algısı riski taşır. Şimdiki sınır zaten okunuyor, seçilmez. |

**Öneri: yalnız A.** En somut tekrar sorununa cevap veriyor; tek tek yaprak, çimen, çit veya bina eklemeye ihtiyaç duymuyor. Su ve hedef, anlaşılmış kuralları taşıyan basit şekiller olarak kalıyor. Üç varyant, rastgele çeşitlilik değil elle belirlenmiş aynı ailenin küçük farklılıkları olmalı.

## A'nın kesin sınırı

Tek gelecekteki kaynak: `Unity/Assets/Scripts/Battle/TacticalBattle.cs`. Yalnız `BuildLandscape` içindeki üç taç oluşturma çağrısı ve onlara özel private mesh/oluşturma yardımcıları önerilir. Gövdeler, zemin, tepe, su/geçit, yol/tarla, çit, konvoy, hedef sınırı, askerler, etiketler, kamera/ışık, oyun zamanları ve mekanik API'ler aynı kalır.

Mevcut dikim merkezleri aynen:

```text
x = -25.8 + (i % 5) * 3.55
z = -3.4 + (i / 5) * 3.7
height = 2.05 + (i % 3) * .16
y = TerrainHeight(x,z)
```

Bahçenin gerçek kural bölgesi `−27 < x < −10`, `−5 < z < 10`; yeni taç bu alanın dışına taşmamalı. Ağaç yerleri, gövde sayısı ve var olan yükseklik hesabı değişmez. Rastgele sayıya, state'e, haftaya veya kameraya göre şekil seçilmez.

Taç önerisi: en çok **üç ortak mesh**, her biri yaklaşık 12 çevre örneği × 4 yükseklik halkası + iki uç, yaklaşık 50 vertex / 96 üçgen. Geniş, az sayıda kontrollü yüzey; sık zikzak, diken, yaprak kümeleri veya plastik parlama yok. Normal geçişleri yumuşak olabilir; göze batan kristal üçgenler istenmiyor.

- Aile 0: geniş, hafif yassı üst; iki yana eşit olmayan omuz.
- Aile 1: üst yarıda daralan fakat sivri olmayan taç; alt omuz daha dolu.
- Aile 2: bir tarafta alçak omuz, öbür tarafta yüksek yumuşak kütle; gövdeden yana taşan yeni genişlik yok.

Her ailede yerel X `−1.12..1.16`, Z `−.90...95`; Y, mevcut `height` merkezine göre yaklaşık `−1.05..1.10` içinde tutulur. Bunlar eski üç kürenin birleşik yatay/yüksek sınırını büyütmeyen üst sınırlar. İlk denemede tüm şekiller bu ortak kutuda kalmalı; rastgele dönüşle sınır aşılmamalı. Her sırada farklı ama sabit bir aile dizisi seçilebilir; 4 × 5 düzenli meyve bahçesi ormana dönüştürülmez.

Renkler mevcut `leaf #4F7361` ve `leafLight #71936B`. Yeni renk ailesi veya materyal yok. Mevcut Standard materyal yoluna ham vertex renklerinin desteklendiği varsayımı yapılmaz: gerekirse iki submesh, bu iki mevcut shared materyali kullanır. Açık yüzey tek sakin üst/yan bölge; dama, benek veya keskin rastgele renk yüzleri olmaz. Bir ağaçta bir renderer, toplam en çok 20 taç renderer; bugünkü 60 küre renderer'ından fazlası gerekmez.

Üç mesh mevcut `meshes` sahiplik listesine birer kez eklenir; ağaçlar `world` altında aynı `Stop` temizliğiyle kalkar. Yeni collider, özel shader, paket, Resource veya bitmap gerekmiyor. Kaynak PNG'ler ve duman shader'ı bu kapsamda yok.

## Gerçek kabul ve ret ölçütleri

1. Aynı yerleşimli önce/sonra 1440 × 900 karede bahçe artık aynı üç balonun kopyaları gibi görünmemeli; üç küçük biçim ritmi yakınlaştırmadan sezilmeli. Sadece başka bir geometrik tekrar veya sert düşük poligon ağaç paketi gibi görünürse aday reddedilir.
2. Geniş bahçe kütlesi koyu, çayır açık kalır; yeni taçlar askerlerin mavi/mercan ve bayrak ayrımından daha güçlü vurgu olmaz. Ek gölge/karanlık tabanla yoğunluk artırılmaz.
3. Mevcut `03-reinforced-deployment` ile eşdeğer boş yerleşim ve `05-contested-crossing` ile eşdeğer yoğun temas karesi gerekir. Özellikle bahçenin üst/sağ kenarındaki süvari ve alt sıradaki piyade, eski görüntüye göre daha çok saklanmamalı. Topçu, hedef halkası ve geçit aynen okunmalı.
4. Görsel birim sayısı, dünya konumu, label yerleşimi ve zamanlar değişmez. Aynı deterministik fixture'ın durum çıktıları eşit olmalı; teknik eşitlik, güzel veya okunur görünümün yerine geçmez.
5. Kamera dönüşü, ağacı tıklayınca saydamlaştırma, yeni örtü sistemi veya ek parçacıklarla bir sorun telafi edilmez. A tek başına küçük ve kabul edilebilir sonuç üretmezse geri alınır.

## Mevcut kaynak yeterliliği

Evet. Buradaki eksik, doku ayrıntısı değil siluet tekrarının fazla görünür olması. Mevcut iki yaprak tonu, sahne ışığı ve üç küçük özgün mesh yeterli araçları sağlar. Ancak küçük ölçekte tek taç hacminin kapladığı ekran alanı başlıca risktir; bu planın uygulanması veya kabulü varsayılmamalı.
