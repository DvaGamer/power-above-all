# Power Above All — Proje durumu

Son güncelleme: **6 Eylül 2026**. Kullanıcının on saatlik özerk geliştirme talimatıyla çalışma yeniden başladı. Onaylı yeni yön [VISION.md](VISION.md), görsel hedef [ART_DIRECTION.md](ART_DIRECTION.md), gece işleri [NIGHT_QUEUE.md](NIGHT_QUEUE.md) içindedir.

Son tamamlanmış kapı **GREEN**: `output/verify/roles-visible-20260905-230302-558-1717bcb0/REPORT.md`. 56/56 Unity testi, taze Direct3D11 Windows player, 22 gerçek kare, 29 durum kontrolü, 3 durum dökümü ve 10/10 tarayıcı çekirdek testi geçti. Üç başlangıç rolü ve iki haftalık yükümlülükler çalışıyor; root RU/TR belgeleri gördü. Sanat commit'i `19e5fa8`; rol kesiti ayrı commit hazırlığındadır. Hızlı aktif kayıt [SESSION_PROGRESS.md](SESSION_PROGRESS.md).

**Güncel durum:** Unity 6000.3.23f1; harita → emir → hafta → mevcut dilekçe → yürüyüş → savaş → rapor → dönüş → kayıt/yükleme yeni player'da doğrulandı. Normal inceleme derlemesindeki Development Build filigranı kaldırıldı; geliştirme derlemesi ayrı menüde. `PLAY_GAME.cmd` tamamı geçen en yeni derlemeyi seçer; `node play-game.cjs --check` seçimi oyunu açmadan gösterir.

Geçerli aşama: rol kesitinin eski rotalarla kontrolü, ardından **taktik doğruluk ve bağlı siyasi sonuçlar**. [ROLE_SLICE.md](ROLE_SLICE.md) çalışma tercihlerini, [NIGHT_REPORT.md](NIGHT_REPORT.md) son kanıt ve sınırları kaydeder. Uzun testte DX12 çıkış hatası D3D12Core.dll'e kadar izlendi; aynı build görünür DX11 ile 6 hafta/2 geri çekilme/40 kontrolü exit0 tamamladı. Yeni rol build'i açık DX11 varsayılanıyla doğrulandı. Unity oyununun dilleri **Rusça ve Türkçe**; belgeler Türkçedir.

## Tamamlanan tarayıcı 0.1 işleri

- [x] Oyun adı Power Above All, ortam Fransa, 1789 olarak belirlendi.
- [x] 12 şematik bölge ve iki harita katmanı oluşturuldu.
- [x] Ekonomi, ordu hareketi, emirler, siyasi destek, olaylar ve kayıt sistemi uygulandı.
- [x] Tüfekli birliklerle taktik çatışma ve sonuçların sefer haritasına aktarılması eklendi.
- [x] Masaüstü ve dar ekranlara uyum sağlayan arayüz tamamlandı.
- [x] START.cmd ile yerel sunucu ve tarayıcı başlatma doğrulandı.
- [x] [GitHub deposu](https://github.com/DvaGamer/power-above-all) oluşturuldu; kullanıcının açık onayıyla şimdi **Public** olarak kalıyor.
- [x] İlk uygulama komiti `7e4723d`, main dalına yüklendi.
- [x] Dört oyun referansı kalıcı tasarım rehberi olarak kaydedildi.

## Tarayıcı 0.1 doğrulama kaydı

- [x] Uzun ve tekrar edilebilir seferler dahil tarayıcı çekirdeği testleri geçti.
- [x] Tarayıcıda harita, emirler, elle/otomatik kayıt, ekonomi, siyaset ve konsey olayı doğrulandı.
- [x] Geri çekilme ve tamamlanan savaşın kayıpları doğru şekilde uygulandı.
- [x] 1440×960 ve 390×844 ekranlar denendi; yatay taşma görülmedi.
- [x] İlk sürümün [GitHub Actions denetimi başarılı](https://github.com/DvaGamer/power-above-all/actions/runs/33987434163).

Ekran görüntüleri test bilgisayarındaki `output/playwright/` klasöründedir; Git'e dahil edilmez.

## Ekip hazırlığı

- [x] Arkadaşların da geliştirmeye katılacağı kararı kaydedildi.
- [x] Türkçe belgeler, katkı rehberi, Issue/PR şablonları ve yerel belge bağlantıları kontrol edildi.
- [x] Beş görsel referans ART_DIRECTION.md içine işlendi; bağlantılı v0.2 planı ROADMAP.md içinde taslak olarak kaydedildi.
- [x] `cb200f1` çalışma anlık görüntüsü GitHub'a yüklendi.
- [ ] Son yerel değişikliklerin ikinci anlık görüntüsü; yükleme tamamlanmadan yapılmış sayılmaz.

## Unity doğrulama kaydı — 5–6 Eylül

Unity 6000.3.23f1 ve lisans hazırdır. Proje `Unity/`, başlangıç sahnesi `Assets/Scenes/Main.unity` içindedir. Bu bilgisayardaki Editor yolu `C:/Users/USER/Tools/Unity/6000.3.23f1/Editor/Unity.exe`.

| Denetim | Doğrulanan sonuç | Kapsam sınırı |
| --- | --- | --- |
| Unity Test Runner / EditMode | **23/23 geçti:** 17 sefer çekirdeği + 6 savaş testi | Bütün ekranların veya seslerin kabulü değildir |
| Gerçek Unity DLL'leriyle bağımsız Roslyn | **10 Runtime + 2 Editor dosyası** derlendi | Oyuncu çalışma zamanını tek başına doğrulamaz |
| Tarayıcı referansı testleri | **10/10 geçti** | Rusça tarayıcı 0.1 içindir |
| RU/TR statik dil denetimi | **310 anahtar, 620 metin; 0 hata** | Eksik/yinelenen anahtar ve biçim alanlarını denetler; görsel taşmayı ölçmez |
| Atlas geometri/yerleşim denetimi | **44 rota, 7 ekran oranı** kontrolü geçti | Görsel kalite ve oyuncu deneyimi onayı değildir |
| Doğal atlas görüntüsü | **1440×900, 4× MSAA** görüntüsü alındı | Son madalyon/kaydırıcı değişiklikleri dahil bütün ekranlar yeniden incelenecek |
| İlk Windows derlemesi | Derleme ve oyun açılışı başarılı | Savaşa geçişte eksik `Standard` shader hatası görüldü |
| Son malzeme düzeltmesi | Kaynak düzeltmesi uygulandı | Yeni Windows derlemesi ve savaş → rapor → atlas çalışma zamanı kontrolü bekliyor |

Roslyn kontrol kümesinde dış süreç tarafından eklenen `AutoShots.cs` de bulunur. Bu dosya ve dış sürecin `tools/` araçları mevcut polish ekibinin uygulaması olarak sahiplenilmez.

Son derleme ve çalışma zamanı kanıtları `output/polish-review-20260906` altında ayrı test profiliyle toplanıyor; henüz sonuçlanmış olarak işaretlenmedi. Önceki doğal atlas görüntüsü `output/atlas-ru.png` içindedir. Bu yerel çıktılar kaynak dosyalarının veya başarılı testlerin yerine görsel kabul iddiası oluşturmaz.

## Uygulanan kaynak polish'i

- **Kabine ve emirler:** sol sütundaki emirler sabittir; sadece alttaki durum raporu kaydırılır. Kullanılamayan emir gerçek nedenini ve eksik kaynak miktarını gösterir. Yürüyüş maliyeti ortak çekirdek tahmininden gelir; aynı OnGUI içindeki durum değişimi nedeniyle oluşan null başvuru düzeltildi.
- **Hesap defteri:** şimdi/gelecek hafta stokları, açık, maaş, teçhizat, üretim, sivil/ordu tüketimi, Paris yardımı ve huzursuzluk tahmini görünür. Sonraki hafta hesaplaması gerçek durumu değiştirmeden derin kopya üzerinde çalışır; ekonomi dengesi değişmez.
- **Belge dili:** özel mat kaydırma rayları, metne göre yükseklik, okuma konumunu koruyan günlük ve önemli haber ayrımı; kısa tek satırlı başlık altı metni ve özgün prosedürel gravür madalyonları eklendi. Büyük tam sayı stokları son değerde hassasiyet kaybetmeden gösterilir.
- **Mevcut dilekçe:** ikinci haftadaki aynı ekmek talebi, kişi ve çıkarı belli bir belge olarak sunulur. Stok, doğrudan sonuçlar ve kullanılamama nedeni görünür; yeni olay kataloğu yoktur.
- **Atlas:** kıyı/ana kara çerçevesi, kent işaretleri, doku, rota, komşu yer adları ve bölge vurguları elden geçirildi. Seçim, hover, yerel emir ve ordu yürüyüşü mevcut verilere bağlandı.
- **Savaş:** mevcut alayların hareketi, yaylım hazırlığı, duman, isabet, çekilme ve komut tepkileri iyileştirildi. Sunum duraklamaya uyar; vuruş hesabı görsel efektler tarafından yeniden uygulanmaz. Sonuç raporunda meydan morali ile sefer dönüş morali ayrılır ve atlas dönüşü kısalır.
- **Görüntü:** ortak 1440×900 yerleşimi kamera, arayüz ve işaretçi koordinatlarında en-boy oranını korur. Editor'de doğal ölçek ve 4× MSAA kullanılır; geniş pencerede görüntü yatay/dikey farklı oranlarda gerilmez.
- **Ses:** kâğıt, kalem, mühür, emir, yürüyüş, hafta, salvo, isabet, zafer ve yenilgi için mevcut on prosedürel taslak; kısa geri bildirimler, kanal sınırı ve kalıcı sessize alma ile bağlandı. Bunlar profesyonel veya tarihsel kayıt değildir; son dinleme bekler.

Bu liste kaynakta uygulanan değişiklikleri anlatır. **Tam polish planı, Rusça/Türkçe bütün ekranların görsel kabulü ve bitmiş oyun hissi tamamlandı sayılmaz.** Yeni mekanik, ekonomi genişletmesi veya olay yönetmeni bu aşamaya eklenmez.

## Sıradaki doğrulama

- [ ] Malzeme düzeltmesini içeren Windows oyununu yeniden derlemek ve bağımsız oyunda savaş başlangıcını doğrulamak.
- [ ] Ayrı test profilinde harita → emir → hafta → dilekçe → yürüyüş → savaş → rapor → atlas döngüsünü tamamlamak; sonucu yalnızca bir kez uygulamak.
- [ ] Son kaynaklarla bütün ekranları RU/TR'de incelemek: taşma, okunurluk, kaydırma, sayı standardı ve dil değiştiren günlükler.
- [ ] Ses düzeylerini, tekrarı, eşzamanlı ses sınırını ve sessize almayı oyun içinde dinleyerek kontrol etmek.
- [ ] Sonuçları ve kalan sınırlamaları bu dosyada ve `POLISH_PLAN.md` içinde ayrı kaydetmek.

## Bilinen sınırlamalar

- Tarayıcı 0.1 öğretici senaryosu kolaydır: konsey olayında stok dağıtmak savaşmadan zafer getirebilir.
- Tarayıcı 0.1 savaş arazisi dekoratiftir; tek asker türü ve ortak grup kontrolü vardır. Unity savaş kaynakları ayrı bir aktarım ve sunum çalışmasıdır.
- Harita tarihî sınırları birebir yansıtmaz.
- Tarayıcı 0.1 kayıtları kullanılan tarayıcıya ve adrese bağlıdır; Unity kayıtlarıyla ortak değildir. Tamamlanmamış savaş kaydedilmez.
- Tarayıcı 0.1 arayüzü Rusçadır. Yeni Unity oyununun Rusça/Türkçe desteği zorunludur; kaynakların bulunması tam arayüz doğrulaması sayılmaz.
- Tarayıcı 0.1 önceden doğrulanmış referanstır. Unity'nin kaynak, EditMode ve ilk derleme kanıtları vardır; son malzeme düzeltmesinden sonra bağımsız oyunun tam döngüsü ve bütün ekranların kabulü bekler. Tam hanedan, diplomasi, inşaat ve çok oyunculu sistemler henüz yoktur.

## İlerleme nasıl izlenir?

1. Yalnızca doğrulanan işleri bu dosyada tamamlandı olarak işaretleyin.
2. Windows oyununu `PLAY_GAME.cmd` ile açın; başlatıcı `Unity/Builds/WindowsPolish/` ve `Unity/Builds/Windows/` içindeki mevcut `.exe` dosyalarından en yenisini seçer. `OPEN_UNITY.cmd` editörü açar. Tarayıcı referansı `START.cmd` veya `npm start` ile çalışır.
3. [Issues](https://github.com/DvaGamer/power-above-all/issues), [pull request'ler](https://github.com/DvaGamer/power-above-all/pulls) ve Git geçmişinden değişiklikleri takip edin.
4. [Actions](https://github.com/DvaGamer/power-above-all/actions) sekmesinde otomatik kontrol sonuçlarını inceleyin.
