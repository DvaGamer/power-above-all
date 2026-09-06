# Atlas ve kabine — 6 Eylül gece çalışması

- Kapsam: `CampaignMap.cs`, `CabinetHud.cs`, `PetitionDocument.cs`, kabine/dilekçe çevirileri. Geometri, kamera, ekonomi, savaş ve oyuncu kayıtları kapsam dışıdır.
- Başlangıç kapısı ana ajan tarafından `7aad17e` ile doğrulandı. Bu ajan Unity başlatmaz, commit/push yapmaz.
- Çalışma yönü: güneşli guaj atlas; sıcak kâğıt `#F3E7CA`, mürekkep `#243B37`, adaçayı `#A9BA88`, su `#83B0B6`, mercan `#C98270`, pirinç `#CAB36F`, toprak `#B79D71`, şarap `#58464D`.
- Gözlenen sorunlar: Lorraine başlığı +12 piksel aşağı kayıp kuleye değiyor; devre dışı düğmeler iki kez solduruluyor; üst bilgi 11 piksel/düşük kontrast; dilekçe dil düğmeleri varsayılan Unity görünümünde.
- Portre bağlantısı: isteğe bağlı `Resources/Art/PoliticalPortraits-v1`; 2×2 sayfada sol üst taç, sağ üst meclis, sol alt kentliler, sağ alt ordu. Eksikse mevcut gravür portreleri kullanılır. Yeni görsel dosyasını ana ajan üretir.
- Beklenen kapı: kaynak farkları + ana ajanın derleme ve 1440×900 RU/TR ekran incelemesi; görüntü kabulü henüz yapılmadı.

## İlk inceleme paketi

- Sıcak kâğıt/koyu mürekkep kabine, 12 piksel üst açıklamalar, daha açık kaynak etiketleri; kısa ülke/yıl alt başlığı ve geniş sayılar için ölçülen yazı küçültme eklendi.
- Alt haftalık etiket, 12 pikselde iki dilde de sığması için yalnız hafta numarasını gösterir; taşan tekrar cümlesi kaldırıldı.
- `Press` eylemi devre dışı tutarken metni açık mürekkeple çizer; alfa ve Unity devre dışı soldurmasının üst üste binmesi kaldırıldı. Ana ajanın eklediği `!L.IsReviewSession` ses tercihi koruması korunur.
- Lorraine başlığının +12 piksel kayması kaldırıldı; şehir adları/coğrafya koyulaştırıldı. Bölge geometrisi, kamera ölçüleri, yürüyüş ve kaynak formülleri değişmedi.
- Deniz yumuşak mavi, çevre kıyılar sıcak keten, şehir duvarları krem ve çatıları mavi/yeşil gölgedir. Kâğıtta düşük genlikli yerel boya dağılımı kullanılır; oynanış rastgelelik akışı tüketilmez.
- Harita ve lejanttaki renk örnekleri aynı `CampaignMap.ModeColor` işlevini kullanır. Denetim/huzursuzluk/elit sadakati mercan–adaçayı; ordu mavi; gıda çayır; vergi toprak/pirinç ailesidir. Veri normalleştirme ve eşikler aynıdır.
- Portre sayfası dosyası ana ajan tarafından getirildi ve bu ajan da görsel olarak gördü. Dört çeyrek UV kullanılır; en/boy oranı korunur, eksik kaynak eski gravüre döner. Kartta 78×88 alan ayrıldı.
- `PetitionDocument.DrawLanguageControls(app)` eklendi; GameApp entegrasyonunu ana ajan yapar. Dilekçe sıcak kâğıt, mat dil seçimi, daha belirgin seçenek oku ve okunur etkilerle aynı aileye bağlandı.
- `git diff --check` bu paketin ilk kaynak değişikliklerinde temizdi. Unity çalıştırma/derleme/görüntü kapısı ana ajandadır; bu not görsel kabul iddiası değildir.
