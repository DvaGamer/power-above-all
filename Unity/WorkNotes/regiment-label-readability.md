# Alay etiketi — sahneyi örten bilgi

Durum: kabul edilen smoke checkpoint `ab378c0` sonrasında root A'yı onayladı; dar yerleşim uygulandı ve aşağıdaki gerçek player karelerinde incelendi. Bu ajan test, Unity, player, derleme veya Git başlatmadı; yalnız root artefaktlarını okudu.

## Gözlenen sorun ve gerçek davranış

`DrawRegimentLabels` her yaşayan/görünür alay için şu koşullardan biri varsa etiketi gösteriyor: seçim, dost alay üzerine hover, son dört görsel saniyede isabet veya moral <36. Çekilmiş/askeri kalmamış alaylar atlanır. Routed alay, bu yöntemde ayrıca saklanmaz. `hovered` ise `HandleInput` içinde yalnız komuta edilebilir dost alaylar arasında, root+2 yüksekliğinin 46 ekran pikseli çevresinden seçilir; düşmana hover ile şu anda ek bilgi açılamaz.

Etiket root+4.8 dünya yüksekliğine sabitlenir: 156×36 koyu %90 opak panel, sayı+moral, durum metni ve 3 px mevcut asker oranı. Yalnız merkez için y148–710 sınırı var; etiketlerin birbirleriyle, alay siluetleriyle, namlu önüyle veya üst arayüzle çakışması hesaplanmaz. Etiketin ayrı bir tıklama alanı yoktur. Alt komut kartları dost alayın sayı/moral/durum bilgilerini zaten verir; düşman için aynı kalıcı kart bulunmaz.

`volley-baseline-20260906-004450-276-c4666896` gerçek 04/06 karelerinde seçili topçunun x800–956 / y427–465 etiketi kendi namlu/duman alanını örter. Sadece root'tan yukarı almak, eğik kamerada ateşin önünü açmamış. Orta düşman dumanı görünürken kendi komutunun görsel cevabı etiket altında kalır. 06'da süvari ve düşman etiketleri de birbirine dayanır; mevcut sistem bunları ayırmıyor. Bu, test amacıyla arayüz gizlenmesiyle çözülecek bir sorun değildir.

## Üç farklı yaklaşım

| Yaklaşım | Oyuncu deneyimi ve uygulama | Bedel / risk |
| --- | --- | --- |
| **A — Kısa saha açıklaması** | Mevcut iki satır ve görünme koşulları korunur. Etiket, alayın yanındaki boşluğa taşınır; küçük bir bağlantı çizgisi/ucu hangi alaya ait olduğunu gösterir. Öncelik seçili alay, ardından kritik durumlar. Alay ve namlu önü için ayrılan ekran alanı üzerine panel konmaz | Boşluk seçimi kararlı olmalı; ateş çıktıkça sağa sola sıçramamalı. Kalabalık merkezde her panelin ideal yeri olmayabilir. Çizgi emir yolu gibi görünmemeli; kısa, ince, o alayın mevcut renk ailesinde olmalı. **Önerilen dar sonraki paket** |
| **B — Bilgi yoğunluğu katmanları** | Dost alayın dünyadaki işareti yalnız mevcut 1–4 numarası ve küçük durum şeridi olur; tam değerler alttaki kartta kalır. Düşmanda kısa sayı/moral satırı ve tehlike rengi; tam durum cümlesi üzerine gelince açılır. Böylece daha az alan örter ve arayüz her bilgiyi sürekli tekrar etmez | Düşman inceleme hover'ı bugün yok; yeni input/bilgi erişimi gerektirir. Sayı/moral/durum dilini yeniden öğretmek ve RU/TR kısa biçimleri doğrulamak gerekir. Mevcut bilgiyi yalnız saklamak kabul edilmez; erişim yolu gerçek oyuncu için açık olmalı |
| **C — Atlas kenarında alay fişleri** | Şu an boş olan sol/sağ kâğıt kenarlarına kısa rapor fişleri yerleşir. Sahada yalnız bağlantılı ufak alay kimliği kalır; büyük metin savaş zemininin dışındadır. Dostlar ve düşmanlar renk/kimlik bağıyla ayırt edilir | Oyuncu gözünü sürekli kenara taşır; uzun bağlantılar ve sekiz olası alay fişi başlık/alt emir alanıyla rekabet eder. Harita ölçeği ve genel HUD düzenini yeniden düşünmek gerekir. Bir smoke polish yamasına sığmaz |

## A için sınır ve kabul önerisi

- İlk kapsam yalnız `DrawRegimentLabels` ve özel yerleştirme yardımcıları/kararlı görsel konum önbelleği. Aynı iki satır, aynı font, aynı bilgi ve görünme koşulları. `DrawHud` ana panelleri, komutlar ve `HandleInput` ilk pakete dahil edilmez. Kaynağı değiştirmek için ayrıca root kararı gerekir.
- Etiket adayları alay çevresindeki birkaç sabit sağ/sol konumdan seçilmeli; salt daha yükseğe taşımak yeterli değil. Seçim sırasında o alayın projekte edilmiş gövdesi ve namlu önü ile diğer yaşayan alayların görünür gövdeleri hesaba katılmalı. Mevcut görünen etiketlerin alanı ve üst/alt HUD da korunmalı. Her yeni smoke nesnesinden alan üretmek gereksiz maliyet ve yer sıçraması yaratır; sabit ateş önü zarfı daha kararlı.
- Mümkünse mevcut taraf korunur, yalnız gerçek çakışmada diğer adaya geçilir. Hiç aday tam temiz değilse kısa ve deterministik bir en düşük örtme tercihi gerekir; "her durumda çakışmasız" diye söz verilmez. Yeni uzun animasyon veya sürekli panel kayması eklenmez.
- Kenara taşınan panel hâlâ düğme değildir. Görünümünü düğme gibi yapmamak gerekir; ayrı tıklama işlevi eklemek bu dar düzeltmenin dışında değerlendirilir.
- Kabul kareleri: seçili topçuyla erken/gelişmiş kendi dumanı görünür; süvari+iki piyadenin merkezde toplandığı doğal karede bilgi alayından kopmaz; düşmanın yeni isabet ve düşük moral bilgisi kaybolmaz; duraklamada etiket durur; üst başlık/pausa/alt kart alanlarına taşmaz. RU/TR uzun durum satırları ve çoklu seçim ayrıca görülmeli. Smoke A kaynakları ile aynı pakette uygulanmaz.

## Smoke A karşılaştırması bekleniyor

Root'un yeni dizini `output/verify/smoke-wash-20260906-005638-116-38a1dee3`; şu an yalnız ilk 00 başlangıç karesi vardı. 04/06/07 çıktıktan ve root build/run durumunu bildirdikten sonra kendi topçu dumanı ile düşman dumanı ayrı değerlendirilir. Eski 04/06 seçili-topçu örtülmesi, yeni normal süvari seçimiyle açılmış görüntünün doğrudan şekil karşılaştırmasına eşit değildir; bu kadraj farkı raporda belirtilmelidir.

## Uygulanan A — sınırlar ve kaynak dondurma

Yalnız `TacticalBattle.cs` içinde `DrawRegimentLabels`, özel `RegimentLabelLayout`/yerleşim yardımcıları/önbelleği ve `Stop` temizliği değişti. Simulation, input, seçme API'si, smoke materyali/meshi/zamanlaması, kamera ve ana `DrawHud` düzeni değişmedi.

- Aynı 156×36 panel +3 px oran şeridi, aynı 145 px iki metin satırı ve `smallStyle`, aynı renkler/sayılar korunur. Çekilmiş/askersiz filtre, seçili/hover/son4görsel-saniye-isabet/moral<36 görünme şartı ve eski root+4.8 projeksiyonunun y148–710 görünürlük eşiği aynen kaldı. Düşmana yeni hover veya etikete tıklama eklenmedi.
- `Time.frameCount` başına bir hesap yapılır; aynı karedeki Layout/Repaint çağrıları listeyi yeniden kurmaz. Sekiz alayın mevcut `Miniature.Root` konumları, yaşayan figürün muhafazakâr gövde/yükseklik payı, bayrak ve top arabası için yerel sekiz köşe projekte edilir. Figür renderer ağacı taranmaz; `GetComponentsInChildren`, yeni texture/material veya her karede yeni alay-yerleşim nesnesi yoktur. Fallen figürler gövde engeline katılmaz; savaş alanındaki tüm cesetleri kapsadığı iddia edilmez.
- Süvari dışında, gerçek ilk/son namlu çıkışlarının yerleri ve sabit erken yayılma payı da korunur. Sabit world drift zarfı `(1.4,.75,.26)`, yatay pay topçuda2.2 / piyadede1.7 world, dikey1.1 world'dür. Bu, kuvvetli erken duman için ihtiyatlı bir alan; bütün5.2s boyunca en soluk son pikselin hiç örtülmeyeceği garantisi değildir. Canlı `effects` listesi veya ateş sayısı okunmaz, dolayısıyla her puff çıkışında etiket yanı değişmez.
- Her etiketin iki yanında, birleşik kendi-gövde/namlu sınırından10 px boşlukta toplam10 aday vardır: dikey0/−45/+45/−90/+90 px. Yan indeksi sabittir; alay ekran ortasından geçince "sağ" ile "sol" anlamı değişmez. İlk eşitlikte dış taraf tercih edilir, sonrasında önceki adaydan ayrılmaya120 puan eklenir. Yerleşim sırası seçili→moral<36/routed→diğer, eşitlikte kalıcı alayId'dir. Metinler ters sırayla çizilip önemli olan öne çıkar.
- Alan maliyeti canlı gövde×6, sabit namlu alanı×2, önceden yerleştirilmiş etiket ve3 px çevresi×10; dikey uzaklık küçük ek maliyettir. Böylece yoğun kümelerde en düşük toplam maliyetli aday kullanılır. Mutlak sıfır örtüşme sözü verilmez; seçilen birikimli yerleşim bazı fallback karelerinde kısmi çakışma bırakabilir.
- Panel tamamı canvas x20–1420 / y148–720 sınırında kalır. Pausa bölgesi `(502,140,436,48)` ve emir açıklaması `(941,683,486,55)` sürekli ayrılır; panel bu bölgelerle çakışırsa sırasıyla y188 veya y644'e alınır. Bu alanlar pause/hover durumu açılıp kapanırken değişmez. Ana başlıklar ve alt kartlar yeniden düzenlenmedi.
- Bağlantı mevcut gövde sınırından panelin en yakın kenarına1 canvas px kalınlığında, düşük opaklıktaki mevcut dost/düşman renk ailesiyle çizilir; ok/işaret veya yeni emir anlamı yoktur. Bütün bağlantılar metin panellerinin altında çizilir. `GUI.matrix` ve renk çizim sonunda geri yüklenir.
- `Stop` sözlük, iki yeniden kullanılan liste ve frame numarasını temizler. Değişen kaynak parçaları statik olarak tekrar okundu; ayna-koordinat testi veya uygulama çalıştırılmadı. Root'un gerçek seçili-topçu, yoğun merkez, RU/TR ve duraklama kareleri hâlâ gereklidir.

## Gerçek kare kabulü — 6 Eylül 2026, 02:20 UTC sonrası

Artefakt: `output/verify/dumas-labels-first-20260906-021758-659-0da55b25`. Root raporu GREEN: 230/230 EditMode, fresh build, 24 PNG / 66 assertion / 21 state / 10 browser kontrolü, bütün birleşik gate 99 saniye. Görsel inceleme ayrıca aşağıdaki karelerde yapıldı; Assets değiştirilmedi.

- `labels-01`, `03`, `05`, `06`: seçili topçu paneli namlu/asker alanının sağına, yaklaşık x921–1078 / y447–487'ye yerleşir. Gerçek toplar yaklaşık x838–898 / y446–495'te açık kalır; iki satır ve bağlantı okunur. 01'de mühimmat11, atıştan sonraki03'te10 görünür. Etiket kendi topçusunun önüne dönmez. Duman hâlâ yumuşak ve çok hafiftir; bu inceleme onu güçlü/çarpıcı bir efekt olarak yeniden onaylamaz.
- `labels-05` ve `06` dar fallback'i gerçekten gösterir: x658–664 / y262–265 civarında dost süvari panelinin sol üst köşesi ile düşman panelinin sağ alt köşesi yaklaşık6×3 px alan paylaşır. Metin kaybı yoktur. Dolayısıyla "bütün etiketler hiç çakışmıyor" sonucu çıkarılmaz; önceden tanımlı düşük örtme kompromisi burada kabul edilebilir.
- `labels-07` RU ve `09` TR: iki seçili piyade gerçek yoğun merkezde görünür. Hat piyadesinin paneli solda, milisinki sağ üsttedir; askerler, topçular ve geçit açık kalır. İki dilde aynı yerleşim, tam sayı/moral/durum satırları ve ince oksuz bağlantılar vardır. Başlık, pausa bandı veya alt komut alanına taşma görülmedi.
- SHA-256 ile PNG çiftleri `03=04`, `07=08`, `09=10` **pobayt aynıdır**. Aynı üç JSON çifti de aynıdır; ayrıca bütün `07/08/09/10` JSON dosyaları aynı `ae31ffbe98ac868f585e231400fcdf08da7e6c640f5ddb045a7fc717e9ccc348` özetine sahiptir. RU/TR değişimi duraklatılmış dünya/seçim durumunu değiştirmemiştir. Bu, gözlenen çiftlerde etikette veya efektte pause kayması olmadığını doğrular; tüm olası kalabalık savaşların garantisi değildir.

Bu kapsam için ilave görsel kaynak düzeltmesi önerilmedi. Uzun/fazla bağlantıların ve başka yoğun kümelerin maliyeti sonraki gerçek oynanışta değerlendirilebilir; mevcut kabul yeni mekanik veya input işi açmaz.
