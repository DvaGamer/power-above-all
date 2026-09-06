# Power Above All

> **Unity'de tek haritalı gerçek-zaman stratejisi geliştiriliyor.** Vizyon [VISION.md](VISION.md), sanat yönü [ART_PRODUCTION_RULES.md](ART_PRODUCTION_RULES.md), güncel doğrulama ve kalan sınırlar [STATUS.md](STATUS.md) içindedir.

**Fransa. Mayıs 1789. Her şeyin üstünde iktidar.**

Fransız Devrimi'nin başlangıcında geçen bir strateji oyununun ilk oynanabilir prototipi. Bölgeleri yönetin; hazineyi, halkın hoşnutsuzluğunu ve ordunun ihtiyaçlarını dengeleyin; taktik çatışmalarda birliklerinize komuta edin.

> **Unity 6000.3.23f1; Rusça ve Türkçe.** Güncel uygulama ve test kayıtları [STATUS.md](STATUS.md). Tarayıcı 0.1 eski Rusça referans prototipidir; aşağıdaki tarayıcı yönergeleri Unity kuralları değildir. Belgeler Türkçedir.

Geçerli aşama **oynanabilir kesitlerin birleştirilmesi**: kişisel iktidar, ülke ekonomisi, fiziksel yürüyüş, alay savaşı ve sonlu ikmal. Geniş diplomasi, inşaat/üretim ağı, kampanya sonu ve kapsamlı alternatif tarih yolları tamamlanmadı. Eski haftalı tasarımın yerine [sürekli dünya mimarisi](REALTIME_ARCHITECTURE.md) geçti.

## Unity temelinin durumu

Gerçek küresel GIS atlası üstünde Fransa odaklı 12 bölge, kişisel hamiler, hesap defteri ve aynı dünya orduları:

- Taç, meclis veya ordu rolü; yardımlar, vadeli sözler, temerrüt ve hami güvenini onarma.
- Vergi, ekmek, Paris yardımı, dört haftalık vergi tatili ve Dumas'nın gerçek açlığa bağlı erzak girişimi. Ekonomi tahmini gerçek haftayla aynı hesap yolunu kullanır.
- Ordu bütçe hedefiyle kademeli rezerve dönüş, ücretli ek asker alımı sağlayan subay hakkı ve gerçek zaferden sonraki siyasi kararlar.
- Erzak veya ticaret yönünde bölgesel reform: tam başlangıç bedeli, dört uygun hafta, sonraki bütçede etki, sponsor ilişkisi ve açık iptal.
- Yerel huzursuzluk, denetim ve elit muhalefetinden hesaplanan düşman kuvveti; oyuncunun ordusunu büyütmek düşmanı doğrudan büyütmez.
- Bir dünya saati, fiziksel yürüyüş ve temas; aynı ordunun merkez/kanat/yedek/topçu/süvari görevleri. HQ, geciken alay emirleri, düzenler, görüş ve ateş koridoru; moral kaybı ve fiziksel geri çekilme. Eski arena sınıfları regresyon için durur, yeni oyunda başlatılmaz.
- Sonlu depo ve konvoy yükü, yol kesilmesi, teslim, yerel erzak tüketimi ve dinlenme. Konvoy yola çıktığında stok düşer; ordu ancak teslim alırsa kullanır.
- Resimli kişiler, guaj atlas, şehir siluetleri ve kaydırılan kâğıt belgeler; en-boy oranını koruyan **1440×900** temel arayüz.

Arşiv **v13 / dünya schema3**: saat, yollar, aynı alaylar, muharebeler, emirler, yerel stok, yoldaki konvoy ve ilk görev raporu korunur. **v12 kayıtları açılır; geçmiş kampanyaya geriye dönük bir görev eklenmez.** Fiziksel ikmalden önceki dünya şemaları açık yeni-kampanya mesajıyla reddedilir; kaynak dosya değiştirilmez. Eski siyasi arşivlerin okunabilmesi onların yeni dünya gibi oynanabildiği anlamına gelmez. Otomatik kare kontrolü görsel kalite kabulü değildir.

## Unity'de ilk deneme

1. Yeni kampanya ve rol seçin. **İlk görev** dosyası, haminizin 28 gün sonraki raporda ne beklediğini gösterir. Üst sağdan tekrar açın; yardım ve gerekli bölge/ikmal/hesap dosyasına doğrudan geçin. Alt sağdaki `|| I II III`, Space ve 1/2/3 zamanı yönetir. I gerçek saniye, II saat, III gün hızıdır.
2. Fransa üstünde bir bölge seçip orduyu gönderin. Rota zaman içinde yürünür. G orduya odaklanır; WASD/MMB kaydırır, tekerlek yakınlaşır, Q/E veya sağ sürükleme döndürür.
3. Sakin deneme için Normandiya'ya gidin; ordu raporundaki **Снабжение / İkmal** dosyasını açıp Paris'ten yük gönderin. Dosyadan kolonu bulun; teslim zamanı ve kalan günleri izleyin. Ordu uzaklaşırsa konvoy gönderildiği buluşma yerinde bekler.
4. Muharebe için Champagne yönüne yürüyün. Temasta hız düşer; aynı haritada yakınlaşın. Soldan alay seçin, Shift+tıklamayla konum emri veya alttan niyet/düzen verin. Emir kuyruğu ve yeniden düzenlenme süresi görünür.
5. Yedeği koruyun; boş mühimmatlı alay kendi arabasına dönebilir. Bütün ordunun geri çekilmesi onu silmez: dinlenme, erzak ve yeni rota ile hazırlanın. Sonuçların tümü kayıt/yüklemeye dahildir.

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
