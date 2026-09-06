# Sıradaki bağlı kesit — gerçek teslimat

İlk uygulanmış adım: fiziksel askerî ikmal. Kaynak kodu ve doğrulaması [STATUS.md](STATUS.md) içinde. İnşaat, yerel sivil üretimin tamamı ve fiziksel asker toplama aşağıdaki sonraki işlerdir; bu belge bunları bitmiş göstermez.

Üç seçenek: (A) harita üstünde gezinen fakat kaynak taşımayan dekoratif arabalar; (B) tüm dünya ekonomisini bir defada yeniden yazmak; (C) mevcut hesapla mutabık yerel stok ve aynı grafikte sonlu yük. C seçildi. [İkmal karşılaştırması](References/Gameplay/Logistics/UGAR_Visible_Supply_Network.md), [kamp atmosferi](References/Historical/Military/NAM_Camp_Food.md) ve [komuta ilkesi](References/Gameplay/Combat/SOW_Courier_Command.md) kullanılacak.

Depo, bağımsız faction/site kimliğiyle stok sahibi olacak. Konvoy, yola çıktığında depodan düşülen tek bir yük taşıyacak; varışta yalnız bir kez ekleyecek. Araba kaybı, yeniden kayıt/yükleme veya tekrar eden olay yük üretemeyecek. Uzak ulusal toplam, yerel ordu stoğu yerine geçmeyecek. Harita katmanı mevcut, yolda ve beklenen miktarı ayıracak; kesilen yolun yeri görünecek.

Rota başlangıcı önce düzeltilmeli: mevcut `WorldRouting.Find` eski ArmyRegionId düğümüne geri dönüyor. Muharebe/geri çekilme sonrası bu, yürünmüş yolun tersine dönüp yanlış merkeze gitmesine yol açabilir. Gerçek konumun yol parçasına bağlanması, engellenen kenarın aşılamaması, alternatif yol ve rota değişiminde ışınlanmama test edilmeli.

Ülke hesabında çift tüketim yasak. Uygulamada haftalık ArmyConsumption dünya için sıfır; askerî tüketim ordunun taşınan Rations stoğundan 15 dakikalık hesapta düşer. Başlangıçtaki oyuncu deposu ve rasyonu merkez stokundan aktarılır. Üst HUD merkez ambarını gösterir; dosya merkezden ayrı yerel/yoldaki/ordu stoklarını gösterir. Paris deposunu yüklemek40merkez erzak+12teçhizat tüketir,96salvo dönüşümü oyun soyutlamasıdır. Sivil üretim henüz merkez defterine akar; fiziksel bölgesel üretim/ambar ağı bitmiş değildir. Yerel deponun ele geçirilmesi içindeki mevcut stoku ve gönderebilme yetkisini devreder. Sivil kıtlık, karnı tok sahra ordusundan aynı anda asker silemez.

İkinci bağ: ücret/insan/teçhizat başvuruda ayrılır, asker birkaç gün fiziksel askerî merkezde toplanır, yeni birlik veya takviye dünyada yol alır. İlk büyük construction örneği depo/barracks; yüzlerce bina tipi açılmayacak. Dumas'nın yetkisi, bütçe politikası ve yerel siyasi bedeller mevcut sistemden korunur.

Kabul: aynı başlangıçta güvenli yol / kesilen yol / yakın depo hazırlığı; stok korunumu, teslim süresi, gecikmiş açlık ve muharebe sonucu. Gerçek Windows'ta normal butonlarla kurulabilmeli. Salt sentetik2:1testi bu oynanabilir döngünün yerine geçmez.
