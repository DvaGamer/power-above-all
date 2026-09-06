# Power Above All

> **Unity 0.2 prototipi geliştiriliyor.** Vizyon [VISION.md](VISION.md), sanat yönü [ART_DIRECTION.md](ART_DIRECTION.md), doğrulanmış sonuçlar ve kalan sınırlar [NIGHT_REPORT.md](NIGHT_REPORT.md) içinde tutulur.

**Fransa. Mayıs 1789. Her şeyin üstünde iktidar.**

Fransız Devrimi'nin başlangıcında geçen bir strateji oyununun ilk oynanabilir prototipi. Bölgeleri yönetin; hazineyi, halkın hoşnutsuzluğunu ve ordunun ihtiyaçlarını dengeleyin; taktik çatışmalarda birliklerinize komuta edin.

> **Güncel proje Unity 6000.3.23f1'de; oyun dilleri Rusça ve Türkçe.** Son tam doğrulama: 496 Unity testi, yeni Windows build, 18 gerçek kare, 72 durum kontrolü, 14 kampanya kaydı ve 10 tarayıcı testi. Tarayıcı 0.1, Rusça referans olarak korunur. Belgeler Türkçedir.

Geçerli aşama **Unity 0.2 oynanabilir prototipi**: kişisel iktidar, ülke ekonomisi ve alay savaşını birbirine bağlayan temel. Geniş diplomasi, kampanya sonu ve kapsamlı alternatif tarih yolları henüz tamamlanmadı. [POLISH_PLAN.md](POLISH_PLAN.md) önceki görsel iyileştirme planını korur; güncel kapsam için [VISION.md](VISION.md) ve [NIGHT_REPORT.md](NIGHT_REPORT.md) esas alınır.

## Unity temelinin durumu

12 bölgeli atlas, kişisel hamiler, hesap defteri, ikinci hafta ekmek dilekçesi, taktik savaş ve kayıt bağlantısı birlikte çalışır:

- Taç, meclis veya ordu rolü; yardımlar, vadeli sözler, temerrüt ve hami güvenini onarma.
- Vergi, ekmek, Paris yardımı, dört haftalık vergi tatili ve Dumas'nın gerçek açlığa bağlı erzak girişimi. Ekonomi tahmini gerçek haftayla aynı hesap yolunu kullanır.
- Ordu bütçe hedefiyle kademeli rezerve dönüş, ücretli ek asker alımı sağlayan subay hakkı ve gerçek zaferden sonraki siyasi kararlar.
- Erzak veya ticaret yönünde bölgesel reform: tam başlangıç bedeli, dört uygun hafta, sonraki bütçede etki, sponsor ilişkisi ve açık iptal.
- Yerel huzursuzluk, denetim ve elit muhalefetinden hesaplanan düşman kuvveti; oyuncunun ordusunu büyütmek düşmanı doğrudan büyütmez.
- Çoklu seçim, hareket, hat/kol/dörtgen düzenleri, ateş izni, elle salvo, duraklatma ve tek sefer uygulanan savaş sonucu.
- Resimli kişiler, guaj atlas, şehir siluetleri ve kaydırılan kâğıt belgeler; en-boy oranını koruyan **1440×900** temel arayüz.

**496/496 Unity EditMode testi** ve 25 yerelleştirme kaynağında **692 RU/TR anahtarı** doğrulandı. Arşiv v8, v1–v7 kayıtlarını eski siyasi ve askerî durumları koruyarak taşır. Ayrı oyuncu senaryoları doğal savaş, kampanyaya dönüş ve kayıt/yükleme eşitliğini; gerçek Windows girdileri seçim, salvo ve çeşitli siyasi belgeleri sınadı. Reformun gerçek fare incelemesi henüz yapılmadı. Otomatik kare kontrolü bütün görsellerin kalite kabulü anlamına gelmez; ayrıntılar [NIGHT_REPORT.md](NIGHT_REPORT.md) içindedir.

On kısa prosedürel foley taslağı bulunur; ses düzeyleri, eşzamanlı ses sınırı ve sessize alma uygulanmıştır. Bunlar profesyonel veya tarihsel ses kayıtları değildir; son dinleme ve bütün ekranların görsel kabulü bekler.

Unity için Rusça/Türkçe gereksinimi; menüler, emirler, araç ipuçları, olaylar, kayıtla geri gelen günlükler ve savaş mesajlarını kapsar. Dil değiştirmek sefer durumunu değiştirmemelidir. Yerelleştirme anahtarları içeriğe, çeviriler kaynak dosyalarına aittir.

## Windows oyununu veya Unity projesini açma

1. **Oyunu açmak için `PLAY_GAME.cmd`** dosyasını çalıştırın. Başlatıcı önce yerel `output/verify/` içinde bütün kontrolleri geçen son GREEN build'i arar; manifest varsa dosyaların boyut ve SHA256 değerlerini denetler. Eksik veya değiştirilmiş aday yerine önceki sağlam build'i dener. Böyle bir sonuç yoksa `Unity/Builds/WindowsPolish/` veya `Unity/Builds/Windows/` içindeki normal yerel derlemeyi doğrulanmamış olarak açabilir. Pencere 1440×900'dür; hiçbir derleme yoksa bunu bildirir.
2. **Editörü açmak için `OPEN_UNITY.cmd`** dosyasını çalıştırın. Başlatıcı `Unity/` projesini hedefler. Her iki başlatıcı Node.js kullanır.
3. Başka bir bilgisayarda Unity Hub'a deponun **`Unity/` alt klasörünü** ekleyin ve Unity **6000.3.23f1** ile açın. Başlatıcı Editor'ü bulamazsa `UNITY_EDITOR` ortam değişkeniyle yürütülebilir dosyanın yolunu belirtin. Başlangıç sahnesi `Assets/Scenes/Main.unity`; Editor'de Play'e basın.

Derlenmiş Windows dosyaları ve yerel doğrulama çıktıları Git deposuna dahil değildir. Yeni klonda Unity menüsündeki **Power Above All → Build Windows** ile oyun oluşturulabilir. `node play-game.cjs --check`, oyunu açmadan seçilecek yolu ve doğrulama düzeyini gösterir.

## Hızlı başlangıç — doğrulanmış tarayıcı 0.1

**Gereksinimler:** Node.js 20 veya üzeri, güncel bir masaüstü tarayıcısı ve Git. CI ortamında Node.js 24 kullanılır. Çalışma zamanı paket bağımlılığı yoktur; `npm install` gerekmez.

Depo, proje sahibinin onayıyla **herkese açıktır (Public)**; klonlamak için özel depo erişimi gerekmez.

```sh
git clone https://github.com/DvaGamer/power-above-all.git
cd power-above-all
npm start
```

Tarayıcıda **http://127.0.0.1:1789** adresini açın. Sunucu yalnızca yerel bilgisayardan erişilebilir. Terminalden çalıştırdıysanız durdurmak için `Ctrl+C` kullanın.

**Windows alternatifi:** `START.cmd` dosyasına çift tıklayın. Başlatıcı sunucuyu gizli bir arka plan işlemi olarak çalıştırır ve tarayıcıyı açar. Aynı oyun sunucusu zaten çalışıyorsa yeniden kullanılır. Arka plan sunucusu ilgili Node işlemi sonlandırılana veya bilgisayar kapanana kadar çalışır.

## İlk deneme — tarayıcı 0.1

1. Haritada bir bölgeye tıklayın. Sol panelde temel üretim, vergi ve hoşnutsuzluk bilgileri görünür.
2. Ekmek dağıtın, olağanüstü vergi toplayın veya ordunuzun bulunduğu bölgede asker alın. Her emir, her bölgede haftada bir kez kullanılabilir.
3. Savaşı hemen denemek için **Champagne / Şampanya** bölgesini seçip çatışmaya girin. Bu, mekaniği göstermek için kurgulanmış bir karşılaşmadır.
4. Savaş alanına tıklayarak mavi askerlerinize ortak bir mevzi belirleyin. Menzile girince otomatik ateş ederler. Nişanlı salvo, duraklatma ve geri çekilme düğmelerini kullanabilirsiniz. Savaş alanı odaktayken yön tuşları mevziyi değiştirir, boşluk tuşu duraklatır.
5. Sonucu onaylayarak kayıpları ve ordunun konumunu sefer haritasına aktarın.
6. Sonraki haftaya geçin. İkinci haftadaki konsey olayını çözmeden zaman ilerlemez.

Mevcut Rusça arayüzdeki temel düğmeler:

| Türkçe karşılığı | Ekrandaki metin |
| --- | --- |
| Şampanya | Шампань |
| Ekmek dağıt | Раздать хлеб |
| Olağanüstü vergi | Чрезвычайный сбор |
| Asker al | Набрать солдат |
| Orduyu gönder | Отправить армию |
| Çatışmaya gir | Вступить в сражение |
| Sonraki hafta | Следующая неделя |
| Kaydet / Yükle | Сохранить / Загрузить |

Öğretici senaryoda hedef, sekiz hafta sonunda pozitif hazineye ve en az bir askere sahip olmak, ortalama hoşnutsuzluğu 55'in altında tutmaktır. Ardından serbest oyuna devam edilebilir. Zorluk şimdilik düşüktür; amaç sistemlerin birlikte çalışmasını sınamaktır.

## Doğrulanmış tarayıcı 0.1 özellikleri

- Seçilebilir 12 şematik bölge; bölge ve hoşnutsuzluk harita katmanları.
- Haftalık vergiler, üretim, tüketim, asker maaşları ve kaynak yetersizliğinin sonuçları.
- Haftada iki ordu hareketi, komşuluk denetimi ve asker alımı.
- Taht, zümre temsilcileri ve halk için üç destek göstergesi; üç seçenekli bir konsey olayı.
- Tüfekli birlikler, ortak hareket emri ve sefer haritasına dönen sonuçlarıyla taktik çatışma.
- Veri yapısı doğrulanan otomatik ve elle kayıt.
- Farklı ekran boyutlarına uyum sağlayan arayüz ve oyun içi yardım.

## Tarayıcı 0.1 kayıtları

Otomatik kayıt emirlerden, savaş sonuçlarından ve haftalık hesaplamalardan sonra güncellenir. **Kaydet** ayrıca bağımsız bir elle kayıt noktası oluşturur; **Yükle** bu noktayı geri getirir.

Kayıtlar tarayıcının `localStorage` alanındadır; GitHub'a veya ekip arkadaşlarına aktarılmaz. Farklı tarayıcılar ve farklı adresler ayrı kayıt alanları kullanır. Tutarlı bir deneyim için her zaman `http://127.0.0.1:1789` adresinden oynayın.

Tamamlanmamış savaş kaydedilmez; sayfa yenilenirse çatışma öncesindeki sefer durumu geri gelir. Yeni sefer başlatma onayından sonra mevcut ilerleme ve elle kayıt değiştirilir.

## Tarayıcı 0.1 kapsamı ve bilinen sınırlamalar

Harita tarihî bir atlas değildir: bölge sınırları şematik, nüfus ve ekonomi değerleri kurgusaldır. Korsika gösterilir, ancak seçilebilir bir bölge değildir. Mayıs 1789'da bu çatışmaların gerçekten yaşandığı iddia edilmez. Oyuncunun bir kraliyet konseyini yönetmesi de oyun tasarımı varsayımıdır.

Henüz kapsamlı karakter ve hanedan sistemi, devletler arası diplomasi, inşaat, teknoloji ağacı, deniz savaşları, birden çok özerk devlet veya çok oyunculu mod yoktur. Savaşta her tarafta tek tür asker ve ortak kumanda edilen bir grup vardır; arazi dekoratiftir.

## Ekip ve ilerleme

Başlamadan önce [katkı rehberini](CONTRIBUTING.md) okuyun. Bir görevi üstlenin, kendi dalınızda çalışın ve değişikliği pull request ile paylaşın.

- [STATUS.md](STATUS.md): tamamlanan işler, doğrulamalar ve sıradaki görevler.
- [ROADMAP.md](ROADMAP.md): önceki aşama planı ve geleceğe bırakılan mekanikler.
- [DESIGN_V0.2.md](DESIGN_V0.2.md): gelecekteki bağlantılı siyaset, ekonomi, bölge, ikmal ve alay savaşı tasarımı.
- [REFERENCES.md](REFERENCES.md): dört kalıcı oyun tasarımı referansı.
- [ART_DIRECTION.md](ART_DIRECTION.md): beş görsel referans ve tarihî çalışma atlası yönü.
- [CHANGELOG.md](CHANGELOG.md): sürümlere göre değişiklikler.
- [NOTES.md](NOTES.md): teknik kararlar ve geliştirme notları.
- [Issues](https://github.com/DvaGamer/power-above-all/issues): görevler ve öneriler.
- [Actions](https://github.com/DvaGamer/power-above-all/actions): otomatik denetimler.
- Git geçmişi: tamamlanıp kaydedilen geliştirme adımları.

## Dosya yapısı

| Dosya | Sorumluluk |
| --- | --- |
| `Unity/` | Güncel Rusça/Türkçe prototip, sefer kuralları, alay savaşı ve Unity testleri |
| `PLAY_GAME.cmd`, `play-game.cjs` | Önce doğrulanmış yerel Windows build'ini seçen oyun başlatıcısı |
| `OPEN_UNITY.cmd`, `open-unity.cjs` | Unity proje başlatıcısı |
| `index.html` | Arayüz iskeleti ve şematik harita |
| `styles.css` | Görsel tasarım ve ekran boyutlarına uyum |
| `simulation.js` | Arayüzden bağımsız sefer simülasyonu |
| `app.js` | Harita etkileşimi, arayüz ve simülasyon bağlantısı |
| `battle.js` | Taktik savaş simülasyonu ve Canvas çizimi |
| `server.cjs`, `launch.cjs` | Yerel sunucu ve başlatıcı |
| `tests/simulation.test.cjs` | Sefer kurallarının otomatik testleri |
| `tests/browser-smoke.js` | Playwright CLI ile tarayıcı denemesi |

## Tarayıcı 0.1 doğrulaması

```sh
npm test
node --check app.js
node --check battle.js
```

GitHub Actions diğer JavaScript giriş dosyalarının sözdizimini de denetler. Tarayıcı testi ayrı bir test oturumunda çalıştırılmalıdır; seferi sıfırlar ve o oturumun kayıtlarını değiştirir. Ayrıntılar [katkı rehberindedir](CONTRIBUTING.md#tarayıcı-kontrolü).

## Tarihî dayanak

Genel Meclis'in (États généraux) 5 Mayıs 1789'da Versailles'da açılması, [Versailles Sarayı'nın resmî tarih sayfasıyla](https://en.chateauversailles.fr/discover/history/key-dates/summoning-estates-general-1789) doğrulanmıştır. Oyuncu kararlarının sonuçları alternatif tarih kapsamında değerlendirilir.
