# Power Above All — proje durumu

## Güncel aşama: ilk hami raporu

6 Eylül 2026: [ilk görev](FIRST_COMMISSION.md) Unity'de çalışır. Yeni kampanyada üç rol, 28 oyun gününe bağlı farklı hedefler alır; mevcut yardım/söz, siyasi güven, Champagne ve askerî ikmal kararları birlikte değerlendirilir. Koşullar baştan görünür. Sonuç anındaki değerler ve +4/−4 siyasi etki bir kez kaydedilir; rapordan sonra aynı dünya sürdürülebilir. Açık belge sözün vadesini durdurmaz; iki günlük ek süre sonunda ihlal işlenir.

Son kaynak: `first-report-release-20260906-153138-885-781be7cd` **GREEN: 590/590 Unity testi, yeni Windows build, 10 gerçek PNG, 21 kontrol, 2 durum ve 10 browser testi**. Runtime SHA256 `14FD80B878C778997461759D3919239C44BE5F8A4AC3E23F203A8F960EC7C022`. Ana projenin Assets/Packages/ProjectSettings içindeki 310 dosyası test edilen izole kaynakla aynı. **955 RU/TR anahtarı, 33 asset; 96 doğrulama yardımcısı kontrolü.** `PLAY_GAME.cmd` son tam GREEN build'i seçer.

Windows senaryosu: taç rolü 1108 livre, 318 merkez erzakı, 1200 asker ve tutulmuş sözle başarılı oldu; rapor öncesi iktidar57, ödül sonrası61. İkmal göndermeyen ve sözünü ihlal eden ordu 939 asker, yaklaşık2,9moral ve sıfır taşınan erzakla başarısız oldu. Her üç rolün eylemli/eylemsiz karşılaştırması, yüksek hızda tam zaman sınırı, geç ödeme ve kayıt sonrası yinelenmeyen sonuç test edildi. Bunlar bağımsız oyuncu beğenisi veya 30–45 dakikalık oturum süresi kabulü değildir.

Gerçek native inceleme `native-input-20260906-152148-ed417164`: aynı etkileşim kodlu önceki590adayda **exit0 / 3PNG / 4kontrol / 1durum**. İlk görevden Champagne düğmesine gerçek tıklama, seçili bölge ve kamera geçişi doğrulandı. Sonraki kaynak farkı yalnız rapor altında ordu kartını gizlemek ve tamamlanan/bozulan sözün alt metnini düzeltmektir. Bu önceki binary, son build doğrulaması olarak sunulmaz.

Rapor **v13 / dünya schema3** ile saklanır. v12 dünyaları açılır; geçmiş sefere geriye dönük hedef eklenmez. İlk görevi görmek için yeni rol kampanyası başlatılır. Operasyonel düşman ve ayrıntılı muharebe sonrası neden raporu bu aşamada yapılmadı; sonraki kalite işleridir.

Son binary ile RU/TR: 1440×900 tam kapıda; 1366×768 `commission-release-768-20260906-153413-771-eb3d3898`, 1920×1080 `commission-release-1080-20260906-153527-551-d1199eb7`, 2560×1440 `commission-release-1440p-20260906-153705-102-eea6190f` ayrı incelemelerde **native0 / 4PNG / 3kontrol** ile geçti. Root son768RU ordu görevi, 1440pTR meclis görevi ve1440×900RU başarı karelerini gerçekten gördü. Ordu kartının rapor altında kırpılması giderildi; yakın atlasın ve genel UI'nin profesyonel kalite kabulü hâlâ ayrı iştir.

## Önceki dünya ve ikmal aşaması

Son güncelleme: **6 Eylül 2026**. Yeni bağlayıcı yön: tek gerçek-zaman dünyası, tek küresel harita; hafta bir tur değildir, ayrı savaş arenası yoktur. Oyuncu bir kişidir; emir insan, mesafe ve bilgi üzerinden işler. [REALTIME_ARCHITECTURE.md](REALTIME_ARCHITECTURE.md) eski haftalı/ayrı-savaş tasarımının yerine geçer.

Güncel uygulama [273e32d](https://github.com/DvaGamer/power-above-all/commit/273e32d649349c80242a5a3bfe530aaa1c020647) olarak `research/reference-library` dalına gönderildi. [Taslak PR #2](https://github.com/DvaGamer/power-above-all/pull/2) yeni gerçek-zaman/komuta/ikmal kapsamıyla güncellendi; bu uygulama commit'inin push ve PR CI kontrolleri geçti. CI tarayıcı regresyonudur; Unity/Windows kanıtları aşağıdadır. `main` ile birleşmedi.

Güncel aday `playable-balance-20260906-135642-436-62670a8a` **GREEN: 577/577 Unity testi, yeni Windows build, 8 gerçek PNG, 11 kontrol, 3 durum ve 10 tarayıcı testi**. Runtime SHA256 `039585725F0CB16652590EAE5BD351E69AD23F5717E03131983CE563A985C022`; 141 dağıtım dosyası doğrulandı. `PLAY_GAME.cmd` son GREEN adayı bütünlük kontrolünden geçirerek seçer. 908 RU/TR anahtarı 32 asset içindedir. Bu kalite kabulünün tamamlandığı anlamına gelmez.

Son adayın RU muharebe ve TR teslim kareleri gerçekten incelendi: komut/erzak bilgisi okunur, teslim sonrası 11,2 günlük erzak ve 256 salvo görünür. Yakın şehir simgesi artık konvoyu kapatmıyor. Kalan görsel açıklar: seyrek yerel arazi, zaman zaman yakınlaşan alay etiketleri, düz kâğıt düğmeler ve yetersiz sonuç hiyerarşisi. Kaynak kodun derlenmesi bunların kabulü değildir.

Önceki 576 adayında `natural-live-7efd0631-20260906-134422-118-63891e49` **710 saniyelik gerçek player incelemesi; native exit 0, 6 PNG, 8 kontrol, 3 durum** ile bitti. Sonuç enjekte edilmedi; kraliyet ordusu 1200 kişiden 1034 kişiye düşüp aynı haritada geri çekildi. Son TR sonuç karesi incelendi. `native-input-20260906-133837-a0e3ed05` gerçek UI tıklamasıyla konvoyu gönderdi; stok 80→40 erzak ve 192→96 mühimmat oldu, teslim ve kayıt/yükleme geçti. Bunlar player-only **PARTIAL** raporlardır; yeni kaynak için ayrı tam kapının yerine geçmezler. Son 577 adayındaki iki davranış düzeltmesi aç ordunun dinlenerek bedava yorgunluk silmesini engeller.

Çalışan bağ: WorldClock0/1/3600/86400 → fiziksel iki ordu/yol → aynı haritada muharebe → altı cephe görevi, geciken emir, görüş/ateş hattı, HQ/cephane arabası, moral/düzen → kalıcı geri çekilme → sonlu depo/konvoy/rasyon/dinlenme. Haftalık ülke hesabı ve günlük Bordeaux yazışması aynı saatte sürer. Ayrı arena açılmaz. Arşiv v12/schema3 önceki dünya biçimini açıkça reddeder; eski dosyayı değiştirmez. [Oynanabilirlik raporu](PLAYABILITY_REPORT.md) kararları ve on devam işini açıklar.

RU/TR için 1440×900 son tam koşuda; 1366×768 `playable-768-20260906-132918-500-4594c8ad`, 2560×1440 `playable-1440p-20260906-133109-666-83614eda`, 1920×1080 `playable-1080-20260906-133241-409-1e607a80` önceki aynı UI kaynaklı 576 adayı üzerinde **native 0 / 8 PNG / 11 kontrol / 3 durum** ile geçti. Root 768 RU ikmal / TR savaş ve 1440p RU savaş / TR ikmal karelerini gördü. Sonraki runtime farkları var olan ses taslaklarının yeni muharebeye bağlanması ve iki yorgunluk düzeltmesidir; artwork/layout değişmedi.

**Henüz bitmedi:** bina/inşaat, bütün bölgesel üretim/lojistik ağı, fiziksel asker toplama, operasyonel düşman planları, takip ve bütün siyasi süreçlerin aktarımı. Askerî ikmal kesiti ülke ekonomisinin tamamını fiziksel üretime çevirmiş değildir. Üç eşli testte hazırlık1200vs2400dezavantajını tersine çevirir; tüm koşulların normal kampanyada kurulması henüz kabul edilmedi. Tam dengelenmiş oyun veya profesyonel görsel/ses milestone iddiası yok. Reference Library16karttır;30–50kart hedefi açıktır.

## Önceki katmanların doğrulama geçmişi

Küresel fiziksel GIS katmanı, kaynaklı 1789 Fransa katmanı, ölçeğe bağlı detay, serbest kamera/HUD, Paris–Bordeaux yazışması ve HQ/alay deneyleri Unity içinde uygulanmıştır. Son aday `command-input-final-20260906-090548-165-7de71df0`: **523 Unity testi, yeni Windows build, 6 PNG, 2 kontrol, 7 durum kaydı** ve 10 tarayıcı testi. Önceki atlas kapısında 12 PNG ve 6 kontrol; aynı UI kaynak sürümünde RU/TR 1440×900, 1920×1080, 2560×1440, 1366×768 senaryoları geçti. Native incelemelerde çok adımlı zoom, Home, dünya sınırı, F, belge kaydırma, D/E/RMB/MMB/Tab ve kamera sonrası bilgi değişmezliği doğrulandı. Son native senaryoda hiçbir alay seçilmeden HQ taşındı. **Dünya/kamera/profesyonel UI milestone'u henüz tamamlanmadı.**

Hub'a doğru Unity projesi eklendi. [COMMAND_SLICE.md](COMMAND_SLICE.md) çalıştırma ve sınırları açıklar. Referans ağacı, matris, dersler ve kaynak sicili oluşturuldu; **10 kart** hazırdır. İlk kürasyonun 30–50 karta tamamlanması ve ses/video incelemesi bekler. [ART_PRODUCTION_RULES](ART_PRODUCTION_RULES.md) ve [dokuz özgün SVG kaynak varlığı](Art/Canonical/README.md) hazır; tümünün runtime aktarımı henüz yok. Yayın durumu son Git kayıtlarıyla ayrıca doğrulanır.

GitHub: uygulama commit'i `7946a03`, `research/reference-library` dalında ve [taslak PR #2](https://github.com/DvaGamer/power-above-all/pull/2) içinde yayımlandı. Bu commit'in push ve PR CI kontrolleri geçti; CI tarayıcı simülasyonunu, yerel raporlar Unity/Windows doğrulamasını kapsar. Bu kesit henüz `main` ile birleştirilmedi.

Unity 6000.3.23f1 üzerinde **oynanabilir, genişletilmiş bir prototip** vardır. Üç başlangıç rolü, kişisel siyasi bedeller, bölge yönetimi, ekonomi, ordunun hazırlanması ve taktik savaş aynı sefer durumuna bağlıdır. Büyük dünya, tam diplomasi ve rejim yolları henüz tamamlanmış değildir. Kesin kullanıcı tercihleri [VISION.md](VISION.md), bütünsel çalışma önerisi [GAME_VISION_DRAFT.md](GAME_VISION_DRAFT.md), güncel geliştirme raporu [NIGHT_REPORT.md](NIGHT_REPORT.md) içindedir.

## Oyunu açmak

[PLAY_GAME.cmd](PLAY_GAME.cmd) en yeni uygun Windows oyuncusunu seçer. Doğrulanmış adayın çalışma zamanı ve bütün dağıtım dosyaları denetlenir; `node play-game.cjs --check` seçimi oyunu açmadan gösterir. Oyunda **«Новая» / «Yeni»** başlangıç rollerini açar. Üst HUD'daki **«Париж · Открыть кабинет» / «Paris · Yazışmayı aç»** Bordeaux yazışma deneyini etkinleştirir.

Unity projesi `Unity/`, başlangıç sahnesi `Assets/Scenes/Main.unity`; [OPEN_UNITY.cmd](OPEN_UNITY.cmd) editörü açar. Windows oyuncusu DX11 kullanır. Daha eski DX12 çıkış hatasının bütünüyle çözüldüğü iddia edilmez. Tarayıcı 0.1 ayrı bir referans prototiptir; [START.cmd](START.cmd) ile açılır ve Unity kaydıyla ortak değildir.

## Uygulanan oynanabilir kapsam

| Alan | Mevcut davranış |
| --- | --- |
| Roller ve siyasi güven | Üç atama, farklı yardım ve iki haftalık borç; ödememe, sonraki yardımın reddi ve kişisel iktidar harcayarak güveni onarma. |
| Bölgesel anlaşma | Dört vergi hesabı boyunca yerel muafiyet; hemen sakinleşme ve denetim; tamamlanma veya erken vergiyle ihlal siyasi sonuç doğurur. |
| Ordu mevcudu | Hedef ve sefer/bütçe politikası; iki başarılı haftada en fazla 200 yaşayan fazla asker rezerve geçer. Sıfır ordu ve normal yeniden kurma desteklenir. |
| Subay atama hakkı | Dumas'ya hak verme, normal alımdan sonra haftalık ücretli ek 200 asker, yaşayan orduya göre bedelli geri alma; bütçe politikasıyla çatışma görünür. |
| Dumas'nın girişimi | Gerçek açlık sonrası ileri tarihli erzak toplama girişimi; önceki normal ikmal, veto ve gerçek yerel/siyasi bedeller ortak haftalık hesapta çözülür. |
| Ekonomi ve halk desteği | Gelir, maaş, üretim, sivil/asker tüketimi ve sonraki stoklar; mevcut halk desteği ve 40/60 eşiklerinin bölgesel etkileri gösterilir. |
| Bölgesel proje | Tek bölgede iaşe veya ticaret; 120 livre ve 4 iktidar peşin; dört uygun haftadan sonra vergi/erzak tabanı değişir. Hazırlık huzursuzluk veya düşük denetimde bekler. Tamamlama ve iptal farklı hamilere ilişki sonucu taşır. |
| Bölgesel direniş | Düşman kuvveti özgün yerel taban, huzursuzluk, denetim ve elit muhalefetinden hesaplanır. Oyuncu ordusunun büyümesi düşmanı otomatik büyütmez. |
| Taktik savaş ve dönüş | Alay emirleri, moral, mevzi ve ikmal; eşzamanlı atış, cephanesiz yakın temas, geri çekilme ve doğal zafer. Gerçek kayıplar/teçhizat/siyasi sonuçlar aynı sefere döner. |
| Zafer kararı | Gerçek zafer sonrası ödül veya tanıma seçimi; yaşayan mevcuda göre bedel, general ve bölge sonuçları; bekleyen karar kayıt/yüklemede korunur. |

## Görünüm ve kullanım

Kabul edilen çalışma dili güneşli guaj atlas ve tarihî minyatürlerdir: sıcak kâğıt, adaçayı, yumuşak mavi, mercan ve koyu orman mürekkebi. Deniz derinliği, 12 yerleşimin farklı siluetleri, seçili sınır, savaş arazisi ve bahçe taçları gerçek karelerde incelendi. Varlık kökenleri [ART_ASSETS.md](ART_ASSETS.md), görsel kurallar [ART_DIRECTION.md](ART_DIRECTION.md) içindedir.

Rusça ve Türkçe oyun metni vardır. Bölge raporunun bütün gövdesi tek kaydırılan yapraktır; yoğun Paris uyarılarında emirlere ve nedenlere ulaşılır. Ekonomi ve siyasi belgeler fiyatı, ilgili kişiyi, mevcut sonucu ve koşullu gelecek hesabını gösterir. Ortak 1440×900 tasarım farklı pencere oranlarında eşit ölçeklenir. Sesli geri bildirimler ve kalıcı sessize alma vardır; son ses kalitesi dinleme kabulü hâlâ açıktır.

## Önceki komuta kesitinin doğrulama arşivi

[Son komuta kapısı](output/verify/command-input-final-20260906-090548-165-7de71df0/REPORT.md): **GREEN**, 523/523 Unity testi ve yeni Windows build. Son native HQ incelemesi `native-input-20260906-090716-1d2fe87f`, mevcut bu build üzerinde iki gerçek kare, iki kontrol ve iki durum kaydıyla tamamlandı. Önce ve sonra alay seçimi boşken HQ koordinatı değişti; sağ tıkla hareket ve sefer dönüşü doğrulandı. `PLAY_GAME.cmd` bütünlük kontrolü bu adayı seçiyor.

[Atlas ve kamera kapısı](output/verify/atlas-input-fixed-20260906-084400-338-bf01f165/REPORT.md): **GREEN**, 523/523 Unity EditMode testi, yeni Windows build, 12 PNG, 6 oyuncu kontrolü ve 10 tarayıcı referans testi. Dağıtım 141 dosyalık manifestle doğrulandı; dış süreç çıkışı 0 idi. Ayrıca 779 RU/TR anahtarın 28 Unity varlığında metin/biçim/import yapısı, 93 doğrulama aracı kontrolü ve 51 native yardımcı kontrolü geçti.

Yazışma senaryosunda ilk hafta gerçek huzursuzluk 21 iken son bilinen değer 35 kaldı; ikinci hafta gelen rapor 21'i bildirirken gerçek durum 23'e ilerlemişti. Kamera ve belge hareketleri kampanya bilgisini yenilemedi. Sıkı talimat ile inisiyatif aynı görevlide farklı yerel sonuçlar üretti. Yoldaki emir, kaynak ayırma ve eski arşiv geçişleri test edildi.

Taktik senaryoda emir gönderme ve alınma, HQ hareketi, niyet değiştirme ve sefer durumuna dönüş gerçek player'da çalıştırıldı. Rezervin kayıpta geri çekilmesi ve kanat görevinin süvari karşısında kareye geçmesi Unity testlerinde doğrulandı. Bunlar yeni gecikmenin eğlenceli olduğunu veya küçük ordunun her koşulda daha iyi komutayla kazanacağını kanıtlamaz. Önceki reform ve doğal zafer kanıtları [NIGHT_REPORT.md](NIGHT_REPORT.md) içinde ayrı tutulur.

## Açık sınırlar ve sonraki iş

- Dünya fiziksel coğrafyası vardır; küresel simülasyon ve diğer ülkelerin ayrıntılı içeriği yoktur. Diplomasi, makam/kariyer sistemi, tam rejim yolları ve çok oyunculu oyun gelecekteki kapsamdır.
- Fransa'nın siyasi katmanı kaynaklı 1789 bailliage yeniden kurmasını kullanır; 12 oynanış kümesi tarihî idareyle birebir değildir. Yollar şematik stratejik bağlantılardır; orman ve yerleşim ayrıntıları özgün stilizasyon içerir. Her kaynak katmanının kökeni `Unity/Assets/Resources/World/` içinde kayıtlıdır.
- Güncel sefer arşivi v12/schema3'tür. Eski v9 komuta kesitinin arena kaydı kısıtı geçmiş uygulamaya aittir; yeni dünya, devam eden çatışmayı ve aynı alayların durumunu kaydeder. Desteklenmeyen eski dünya şeması açıkça reddedilir.
- Gecikmeli yönetim yalnız Bordeaux deneyindedir. Oyuncu seyahati, Paris'te yokluğunun siyasi bedeli, bütün ülke için direktifler, generalin bağımsız sefer yönetimi ve HQ yaralanması uygulanmadı.
- Küçük ekran yazısı, ağır bölge sınırları, şehir silüetleri ve yakın plan yüzeyleri daha fazla görsel çalışma gerektirir. Dört çözünürlükte görüntü almak profesyonel responsive UI kabulü değildir.
- Güçlü aktif sivil politika ve pasif kriz yolları ölçüldü; uzun dönem rekabet dengesi, bütün taktikler ve ses kalitesi için daha fazla gerçek oynama gerekir.
- Kullanıcının açık Editor'üne veya oyuncusuna otomatik girdi gönderilmez. Native incelemeler yalnız aracın açtığı ve sahipliğini doğruladığı ayrı player'da yapılır.
- Ayrıntılı kalıcı bulgular [NOTES.md](NOTES.md) ve `Unity/WorkNotes/` altında, başarısız eski denemeler değişmeden `output/` altında kalır.
