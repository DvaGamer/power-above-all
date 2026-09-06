# Power Above All — görsel üretim kuralları

Bu belge kullanıcının 6 Eylül 2026 tarihli yeni talimatını uygular. `ART_DIRECTION.md` içindeki palet ve atlas kimliği korunur; nihai görsel üretimde image generation varsayılanı kaldırılmıştır.

## Üretim sırası

Referans kartı → form analizi → üç farklı kompozisyon → ana kütleler → silüet → palet → ikincil biçimler → kontrollü kusurlar → oyun varlığı → gerçek Windows screenshot → düzeltme.

İzinli yöntemler: özgün SVG/path, elle belirlenen eğriler, Unity mesh, SpriteShape, sade geometrik bileşim, küçük özgün sprite sheet, kontrollü prosedürel yüzey ve shader, lisansı doğrulanmış tarihî belge. AI görsel yalnız geçici konsept olabilir; yeniden çizilmeden final oyuna girmez. Hazır particle ve stock ikon setleri oyunun diline uyarlanmadan kabul edilmez. Ayrıntı kendi başına kalite değildir.

## Sekiz renk ve malzeme

| İşlev | Renk |
|---|---|
| Kâğıt / duvar | `#F3E7CA` |
| Mürekkep / en koyu gölge | `#243B37` |
| Adaçayı / arazi | `#A9BA88` |
| Su / soluk mavi çatı | `#83B0B6` |
| Mercan / tehlike | `#C98270` |
| Mat pirinç / seçili iz | `#CAB36F` |
| Toprak / yol | `#B79D71` |
| Şarap / siyasi gerilim | `#58464D` |

Ara tonlar yalnız bu renklerin karışımlarıdır. İki komşu nesne aynı malzemeyi aynı renk ailesiyle anlatır. Parlak metal, fotoğrafik doku, rastgele gradient ve her yerde vignette yok. Kâğıt tanesi düşük kontrastlıdır; şekil kenarını eritemez. Gölge ayrı sade kütledir; ekranın tamamını sinematik ışıkla örtmeyin.

## Görsel alfabe

İlk dokuz düzenlenebilir kaynak varlık [Art/Canonical](Art/Canonical/README.md) içindedir. Elle yazılmış SVG; küçük ölçekte ve hedef fon üzerinde inceleyin. Kaynak varlık ile runtime mesh/sprite entegrasyonu ayrı kabul adımlarıdır.

- Çizgi: üç sınıf — ana silüet, iç ayrıntı, bağlamsal işaret. Aynı ölçekte oran 3:2:1; en küçük UI çizgisi fiziksel ekranda en az bir piksel. Harita nehirleri kıvrımlı, yollar kesintili toprak izi, siyasi sınırlar sade mürekkep; aynı işaret birbirinin yerine kullanılamaz.
- Form: büyük, orta, küçük en fazla üç kütle grubu. Küçük nesnenin karakteri 32 pikselde okunmalıdır. Şekil karmaşıklığı kamera yaklaştığında açılır.
- Ağaç: 7–15 kontrollü form, iki/üç loblu taç, küçük yana kaymış gövde. Orman 4–8 varyanttan kümelenir; her ağaç farklı rastgele gürültü değildir.
- Mimari: duvar, çatı, baca/kule, kapı. Çatı genişliği duvardan biraz taşar; şehir kimliği çatı ritmi ve ana kuleyle kurulur. Paris, Bordeaux ve kıyı yerleşimleri tek dev resim yerine aynı parçalardan farklı silüet taşır.
- İnsan: baş, şapka, gövde, iki bacak, tüfek. Baş toplam boyun yaklaşık beşte biri; yön silah ve omuz hattından okunur. Idle/march/aim/fire/reload/melee/fall/rout aynı iskeletin pozları olmalıdır.
- Portre: özgün gravür medalyon, geometrik portre veya lisanslı tarihî kolaj tercih edilir. Kurgusal karakter gerçek tarihî kişiye sessizce dönüştürülmez. `PoliticalPortraits-v1` AI sheet final pipeline dışında geçici konsepttir; runtime gravür çizimine dönmelidir.
- UI ikon: tek görüş açısı, açık uçlu sade çizgi, aynı köşe ve çizgi oranı. İkon tek başına renk ayrımına bağımlı değildir; anlamı metinle öğrenilir.
- Bulut: üç geniş düzensiz kütle, tek alt gölge; gökyüzünü süsleyen parlak fotoğraf lekesi yok.
- Müşket dumanı: çok kısa keskin parıltı → küçük açık kütle → 3–5 büyüyen yumuşak lop → seyrelme → soluk artık. Ritim ateş animasyonuna bağlıdır; sürekli aynı patlama şekli ve fazla parlama yok.
- Belge süsü: küçük çift çizgi, kırık köşe ve bir rozet; okunacak metinden yüksek kontrastlı olamaz.
- Asimetri: temel silüeti değiştirmeden %3–8 sapma; bir iki odaklı kusur. Geometriyi ve GIS kıyı doğruluğunu gürültüyle bozmayın. Nehrin yeri biçimsel çeşitlilik bahanesiyle değişmez.

## Kabul kontrolü

Gerçek oyun ekranında dünya, Avrupa, Fransa ve yakın bölge ayrı incelenir. Silüetler okunuyor mu? Aynı elin çizgisi tekrarlanıyor mu? Malzemeler tutarlı mı? Öncelik ilk iki saniyede anlaşılıyor mu? Her ayrıntı bir amaca hizmet ediyor mu? Fuzzy kenar, anlamsız bezeme, her yerde aynı ayrıntı, aşırı ışık, pseudo-painterly noise veya tutarsız perspektif varsa önce sadeleştirin.

Her önemli iş için kayıt: **REFERENCE → INTENDED LESSON → OUR IMPLEMENTATION → RESULT → REMAINING GAP**. Compile/test görsel kabul değildir. Ses dinlenmeden, kamera gerçek girdilerle oynanmadan ve oyun senaryosu tekrarlanmadan o alan doğrulandı denmez.
