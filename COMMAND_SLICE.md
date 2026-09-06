# Paris–Bordeaux: insan üzerinden yönetim kesiti

Son kullanıcı talimatını uygulayan sınırlı prototip; bütün ülkeye yayılmış idare değildir. Atlas/kamera milestone'u ayrıca devam eder.

Üç seçenek değerlendirildi: bütün Act çağrılarını geciktirmek (mevcut dengeleri ve testleri tek adımda değiştirir); ayrı örnek sahne (mevcut ülke sonuçlarıyla kopuk); mevcut kampanyada bir bölge dosyasını açarak etkinleştirmek (seçildi). Dosya mevcut Guyenne/Bordeaux durumuna, ekonomiye, haftaya ve arşive bağlanır. Eski kayıtlar kendiliğinden yeni bir kriz edinmez.

Oyuncu kurgusal Adrien de Varenne, Paris'tedir. Kamera bilgiyi güncellemez. Bordeaux görevlisi kurgusal Étienne Delmas: yetkin ama ihtirası sadakatinden yüksek. Bunlar gerçek 1789 görevlileri veya tarihî makam sınırları iddiası değildir.

Emir: ekmek, vergi, düzeni sağla veya yeni rapor. Çıkışta kaynak ayrılır; hedef bölge hemen değişmez. Normal kurye6gün, hızlı3gün/12livre, yerel hazırlık2gün; sayılar oynanış içindir, tarihsel posta hızı değildir. Mevcut haftalık model korunur: haftanın iletişim olayları tarih sırasıyla işlenir, oyuncu hafta aralarında diğer siyasi kararları verir. Yerel günlük ekonomi iddiası yoktur.

İki yetki biçimi: müzakereyi açıkça emret veya düzen hedefinde inisiyatif ver. İkinci durumda Delmas zor kullanmayı seçebilir; aynı amaç farklı yerel ve siyasi sonuç verir. Karar nedeni raporda görünür. Yeni emir eski yoldaki emri değiştiremez; yalnız ayrı hızlı rapor isteği yapılabilir. Bu ilk kesitte bütün standing orders editörü, atama/seyahat, yol kesilmesi veya yalan rapor simülasyonu yoktur.

Gerçek bölge, son alınan snapshot ve yoldaki rapor farklı veridir. Harita/HUD snapshot okur. Emir takvimi beklenen teslimi gösterir; uzaktaki gerçek alınma anını Paris telepatik olarak öğrenmez. Rapor geldiğinde gözlem tarihi değişmez. Yerel sonuçlar önce bölgeye, duyurulan siyasi tepkiler rapor dönüşünde merkeze ulaşır.

Kaynaklar: [SoW kurye](References/Gameplay/Combat/SOW_Courier_Command.md), [Command Ops gecikme riski](References/Gameplay/AI/Command_Ops_Delay.md), [Radio General bilgi](References/Gameplay/Campaign/Radio_General_Reports.md), [Young mektupları](References/Literature/Primary_Sources/Young_Missing_Letters.md).

Kabul: gönderme yerel sayıyı değiştirmez; yerel uygulama harita bilgisini yenilemez; dönüş snapshot'ı teslim anındaki gerçek sayıya dönüşmez; arşiv aynı uçuşu devam ettirir; başarısız emir atomiktir; yetki iki farklı sonuç üretir; RU/TR gerçek player belgesi görülür. Bunlar ilk Windows senaryosu ve Unity testleriyle doğrulandı; kullanıcıya ilginç gelmesi henüz kör oynanış testiyle kanıtlanmadı.

## Ayrı taktik kesit

Dosya açık kampanyada başlatılan savaşta Dumas'nın HQ masası ve ekibi fiziksel yer değiştirir. Seçilen polka emir hemen kaydedilir; alınana kadar önceki emir sürer. En çok iki FIFO emir; aynı hedefi tekrar tıklamak zaman kazandırmaz. Mesafe, düzen, yorgunluk, komutan yeteneği, sınırlı arazi/tehdit katkısı .65–4.5sn soyut gecikmeye dönüşür. Menzil tek başına ceza değildir: HQ'yu yaklaştırmak mesafeyi azaltır, yakın düşman karargâhı tehdit eder.

Üç niyet: tut, rezervi koru, kanadı koru. Rezerv ağır kayıp/düşük moralde HQ yakınına çekilir; kanadı koruyan hareketsiz piyade yakındaki süvariye kareyle karşılık verir. Bu kararların nedeni UI'da görünür. Yeni görev yerelde alınmadan eskisi değişmez. Savaşın sonucu mevcut sefer kaydına döner.

Bu henüz ülke emriyle eşzamanlı cephe simülasyonu değildir. Adrien'in Paris'te oluşuna rağmen taktik deneye erişim açık tutulur; oyuncunun cepheye seyahati ve bunun kontrol haklarına etkisi uygulanmamıştır. HQ yaralanması, kurye kesilmesi, taktik bilgi yaşı, bütün standing orders editörü ve generalin bütün kampanyayı kendi yürütmesi yoktur. “Küçük ordu daha iyi komutayla kesin kazanır” sonucu henüz test edilmedi.

## Çalıştırma

Unity Hub → Power Above All → `Assets/Scenes/Main.unity` → Play. Yeni kampanyada üstte **Париж · Открыть кабинет / Paris · Yazışmayı aç**. Bordeaux dosyasında yetki ve kurye seçip bir emir gönder; haftayı ilerlet, ikinci hafta mevcut dilekçeyi çöz, gelen raporu oku. Normal posta ile hızlı postanın dönüş günleri farklıdır. Dosya açıldıktan sonra Champagne gibi bir çatışmaya yürüyüş, HQ deneyini açar. Kullanıcının normal kaydı review senaryolarından ayrıdır.

Kanıt: son komuta kapısı `command-input-final-20260906-090548-165-7de71df0` GREEN, 523 Unity testi; önceki correspondence-atlas-fixed gerçek 35→21 bilgi farkı; command-network-review gönderme/alınma/HQ/sonuç kareleri. Son native `090716-1d2fe87f`, hiçbir alay seçilmeden HQ düğmesi ve sağ tıkla taşımayı doğruladı. `atlas-input-fixed` kamera düzeltmelerinin 12 karelik ayrı kapısıdır. [Karşılaştırma ve açık farklar](References/Design_Lessons.md).
