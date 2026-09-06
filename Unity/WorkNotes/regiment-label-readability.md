# Alay etiketi — sahneyi örten bilgi

Durum: yalnız kaynak ve mevcut gerçek kare incelemesi; Assets değişikliği veya uygulama çalıştırma yok. Smoke A'nın runtime incelemesi tamamlanmadan bu öneri uygulanmaz.

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
