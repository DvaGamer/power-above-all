# Power Above All — proje durumu

Son güncelleme: **6 Eylül 2026**. Gece planından sonraki kullanıcı öncelikleri: küresel coğrafi atlas, kamera/UI, elle görsel üretim ve PLAYER ≠ STATE için iki küçük bağlı deney. Eski gece raporu bu yeni işlerin tamamını kapsamaz.

Küresel fiziksel GIS katmanı, kaynaklı 1789 Fransa katmanı, ölçeğe bağlı detay, serbest kamera/HUD, Paris–Bordeaux yazışması ve HQ/alay deneyleri Unity içinde uygulanmıştır. Son aday `command-input-final-20260906-090548-165-7de71df0`: **523 Unity testi, yeni Windows build, 6 PNG, 2 kontrol, 7 durum kaydı** ve 10 tarayıcı testi. Önceki atlas kapısında 12 PNG ve 6 kontrol; aynı UI kaynak sürümünde RU/TR 1440×900, 1920×1080, 2560×1440, 1366×768 senaryoları geçti. Native incelemelerde çok adımlı zoom, Home, dünya sınırı, F, belge kaydırma, D/E/RMB/MMB/Tab ve kamera sonrası bilgi değişmezliği doğrulandı. Son native senaryoda hiçbir alay seçilmeden HQ taşındı. **Dünya/kamera/profesyonel UI milestone'u henüz tamamlanmadı.**

Hub'a doğru Unity projesi eklendi. [COMMAND_SLICE.md](COMMAND_SLICE.md) çalıştırma ve sınırları açıklar. Referans ağacı, matris, dersler ve kaynak sicili oluşturuldu; **10 kart** hazırdır. İlk kürasyonun 30–50 karta tamamlanması ve ses/video incelemesi bekler. [ART_PRODUCTION_RULES](ART_PRODUCTION_RULES.md) ve [dokuz özgün SVG kaynak varlığı](Art/Canonical/README.md) hazır; tümünün runtime aktarımı henüz yok. Yayın durumu son Git kayıtlarıyla ayrıca doğrulanır.

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

## Son doğrulama

[Son komuta kapısı](output/verify/command-input-final-20260906-090548-165-7de71df0/REPORT.md): **GREEN**, 523/523 Unity testi ve yeni Windows build. Son native HQ incelemesi `native-input-20260906-090716-1d2fe87f`, mevcut bu build üzerinde iki gerçek kare, iki kontrol ve iki durum kaydıyla tamamlandı. Önce ve sonra alay seçimi boşken HQ koordinatı değişti; sağ tıkla hareket ve sefer dönüşü doğrulandı. `PLAY_GAME.cmd` bütünlük kontrolü bu adayı seçiyor.

[Atlas ve kamera kapısı](output/verify/atlas-input-fixed-20260906-084400-338-bf01f165/REPORT.md): **GREEN**, 523/523 Unity EditMode testi, yeni Windows build, 12 PNG, 6 oyuncu kontrolü ve 10 tarayıcı referans testi. Dağıtım 141 dosyalık manifestle doğrulandı; dış süreç çıkışı 0 idi. Ayrıca 779 RU/TR anahtarın 28 Unity varlığında metin/biçim/import yapısı, 93 doğrulama aracı kontrolü ve 51 native yardımcı kontrolü geçti.

Yazışma senaryosunda ilk hafta gerçek huzursuzluk 21 iken son bilinen değer 35 kaldı; ikinci hafta gelen rapor 21'i bildirirken gerçek durum 23'e ilerlemişti. Kamera ve belge hareketleri kampanya bilgisini yenilemedi. Sıkı talimat ile inisiyatif aynı görevlide farklı yerel sonuçlar üretti. Yoldaki emir, kaynak ayırma ve eski arşiv geçişleri test edildi.

Taktik senaryoda emir gönderme ve alınma, HQ hareketi, niyet değiştirme ve sefer durumuna dönüş gerçek player'da çalıştırıldı. Rezervin kayıpta geri çekilmesi ve kanat görevinin süvari karşısında kareye geçmesi Unity testlerinde doğrulandı. Bunlar yeni gecikmenin eğlenceli olduğunu veya küçük ordunun her koşulda daha iyi komutayla kazanacağını kanıtlamaz. Önceki reform ve doğal zafer kanıtları [NIGHT_REPORT.md](NIGHT_REPORT.md) içinde ayrı tutulur.

## Açık sınırlar ve sonraki iş

- Dünya fiziksel coğrafyası vardır; küresel simülasyon ve diğer ülkelerin ayrıntılı içeriği yoktur. Diplomasi, makam/kariyer sistemi, tam rejim yolları ve çok oyunculu oyun gelecekteki kapsamdır.
- Fransa'nın siyasi katmanı kaynaklı 1789 bailliage yeniden kurmasını kullanır; 12 oynanış kümesi tarihî idareyle birebir değildir. Yollar şematik stratejik bağlantılardır; orman ve yerleşim ayrıntıları özgün stilizasyon içerir. Her kaynak katmanının kökeni `Unity/Assets/Resources/World/` içinde kayıtlıdır.
- Sefer arşivi v9, eski v1–v8 verilerini destekler. Devam eden taktik savaş ayrı bir kaydedilebilir savaş arşivi değildir.
- Gecikmeli yönetim yalnız Bordeaux deneyindedir. Oyuncu seyahati, Paris'te yokluğunun siyasi bedeli, bütün ülke için direktifler, generalin bağımsız sefer yönetimi ve HQ yaralanması uygulanmadı.
- Küçük ekran yazısı, ağır bölge sınırları, şehir silüetleri ve yakın plan yüzeyleri daha fazla görsel çalışma gerektirir. Dört çözünürlükte görüntü almak profesyonel responsive UI kabulü değildir.
- Güçlü aktif sivil politika ve pasif kriz yolları ölçüldü; uzun dönem rekabet dengesi, bütün taktikler ve ses kalitesi için daha fazla gerçek oynama gerekir.
- Kullanıcının açık Editor'üne veya oyuncusuna otomatik girdi gönderilmez. Native incelemeler yalnız aracın açtığı ve sahipliğini doğruladığı ayrı player'da yapılır.
- Ayrıntılı kalıcı bulgular [NOTES.md](NOTES.md) ve `Unity/WorkNotes/` altında, başarısız eski denemeler değişmeden `output/` altında kalır.
