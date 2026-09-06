# Oynanabilirlik geçişi — 6 Eylül 2026

Bu bir tamamlanmış oyun ilanı değildir. Yeni hedef tek haritada yeniden oynanabilir ve anlaşılır seferdir. İlk bağlı çalışma; siyasi rol → hazırlık → gerçek yürüyüş → aynı-harita muharebe → örgütsel kayıp/geri çekilme → ikmal ve dinlenme döngüsüdür.

## Artık çalışan bağlar

Saat duraklatılır; I saniye, II saat, III gün ölçeğidir. Render zamanı simülasyonu sürüklemez; büyük aralıklar sınırlı iş bütçesiyle hesaplanır. Temas hız politikasını uygular. Takvim, ülke hesabı ve yazışma yerel çatışma sırasında devam eder.

Altı görev aynı alay verisine bağlıdır: merkez, iki kanat, yedek, süvari ve batarya. Cephe açılması, dost ateş koridoru, görüş, yorgunluk, düzen, HQ ve geri yol sonuç üretir. Yerel komutan cephanesi bitince kendi arabasına döner veya kaynak yoksa çekilir. Yeni emirler teslim ve yeniden düzenlenme süresine tabidir. Düşük moral/düzen orduyu tüm askerleri ölmeden yenebilir.

Paris'ten çıkan ikmalde stok çıkışta düşer, araba dünyada yol alır, yalnız teslimde orduya eklenir. Yol kesilebilir ve yük ele geçirilebilir. Oyuncu nerede kaldığını, ne taşıdığını ve yaklaşık süresini görebilir. Ordu ayrıldıysa araba eski buluşma yerinde bekler. Ulusal stok ile taşınan rasyon ayrıdır; aynı erzak haftalık hesapta tekrar tüketilmez. Dinlenme düzen/moral/yorgunluğa, cephane ise sonlu araba stoğuna bağlıdır.

Kayıt v12; eski dünya şeması açıkça reddedilir. Kayıt/yükleme emir kuyruğunu, hareketi, muharebeyi ve yoldaki yükü korur. Aynı ordunun arena kopyası üretilmez.

## Balans kanıtı ve sınırı

Üç sabit rastgele başlangıçta aynı bileşimle1200vs2400 karşılaştırıldı. Hazırlıksız küçük kuvvet kaybeder; karşı tarafın yorgun/cephanesi sınırlı olması, savunma yükseltisi ve zamanında yedek kullanımı sonucu tersine çevirebilir. Sayı eksiğini kapatan gizli bonus yoktur. Bu sentetik tekrar testidir; gerçek Fransa'da bütün koşulların normal emirlerle kurulabildiğinin kanıtı değildir.

Gerçek Windows seferinde Paris→Normandiya yürüyüşü, gerçek konvoy, teslim ve ara kayıt doğrulandı. İlk ikmal karelerinde5,6gün erzak,33,6saat yol ve teslimde11,2gün erzak görüldü. Yakın Paris sembolünün yükü kapatması screenshot incelemesinde bulundu ve semantik ölçekte küçültüldü. Kaynaklar: [UGAR ikmal karşılaştırması](References/Gameplay/Logistics/UGAR_Visible_Supply_Network.md), [UG cephe/yedek](References/Gameplay/Combat/UG_Front_Reserves_Fire_Lanes.md), [NAM kamp](References/Historical/Military/NAM_Camp_Food.md), [SoW komuta](References/Gameplay/Combat/SOW_Courier_Command.md). Ayrıntılı koşu kanıtı [STATUS.md](STATUS.md).

Yeni bağlı denge deneyi, gerçek Windows'taki Normandiya varış kaydından yalnız normal ikmal emirleri ve saatle14gün ilerler. Saf tekrar sonucu (canlı oyuncu tercihi deneyi değildir):

| Plan | Kalan asker | Moral | Yorgunluk | Merkez erzak | Taşınan erzak |
|---|---:|---:|---:|---:|---:|
| Hiç gönderme |1116/1200|20,3|100|322|0|
| Varışta ve7.gün gönder |1200/1200|84|0|322|32,62|
|6.ve12.gün gönder |1200/1200|81|0|322|42,74|

Geç teslimatta aç kalınan dönemin eksik yemeği geriye dönük tahsil edilmez; daha fazla son stok, daha düşük moral ve risk karşılığıdır. İlk deneyde aç ordu dinlenme indirimi yüzünden yorulmuyordu; dünya saatinden geçen regresyonla düzeltildi. Haftalık kapanış artık sahra yorgunluğunu sebepsiz12puan silmez. Yalnız bağımsız tüketim testinin geçmesi bu hatayı bulmamıştı.

Son tam doğrulama: `playable-balance-20260906-135642-436-62670a8a`, 577/577 Unity testi, yeni Windows oyuncusu, 8 gerçek kare, 11 durum kontrolü ve 10 tarayıcı regresyonu. Önceki aynı UI'lı adayda doğal muharebe 710 saniye gerçek player çalışmasıyla sonuçlandı; 1034 askerle geri çekilme ve kalan dünyada devam doğrulandı. Bunlar oyuncunun eğlendiğini ölçen bağımsız oyun testleri değildir.

## Şimdiki oyunda en yüksek getirili on devam işi

1. Mevcut üç rol/mandat etrafında30–45dakikalık açık sefer hedefi ve anlaşılır yenilgi/başarı raporu. Yeni kaynak sayısı artırılmadan bir oturumun neden tekrar oynanacağını belirlemek.
2. İlk yürüyüş/ikmal/komut/yedek kararını harita üstünde öğreten kısa bağlamsal yönlendirme. Gecikme, kalan erzak ve iletişim nedenini ezberletmeden göstermek.
3. Mevcut ikinci ordunun yol ve depo tehdidine tepki veren operasyonel planı. Salt sabit Champagne karşılaşmasından çıkıp hazırlık kararını kampanyada üretmek.
4. Muharebe sonrası bir günlük temas korumasını gerçek toparlanma, takip ve güvenli geri yol kararlarıyla değiştirmek; yeni üçüncü kuvvetin müdahalesini sınamak.
5. Gerçek yol–nehir kesişimlerini ve geçitleri doğrulamak. Mevcut şematik yolların suyu görünmez şekilde kesmesi ve5km yerel bağlantı kestirmeleri kaldırılmalı.
6. Mevcut asker alma/terhis bedellerini fiziksel toplanma ve zaman alan takviyeye çevirmek. Anlık200asker eklemenin yerine mevcut insan/para/teçhizat ilişkisini korumak.
7. Haftalık siyasi cooldown ve bölgesel sonuçları uygun tarihli sürelere geçirmek. Merkezi göstergelerin saha bilgisiyle karıştırıldığı kalan raporları düzeltmek.
8. Önce yalnız mevcut depo/askerî merkez için süreli inşaat ve yerel üretim; bütün dünya sanayisini aynı anda açmamak.
9. Muharebe nedenlerini birkaç saniye koruyan rapor, görünür ateş/komuta koridoru ve doğal yerel arazi kütleleri. Kamera yakınlığında tanınan36figür, henüz bitmiş animasyon seti değildir.
10. Mevcut özgün ses taslaklarının gerçek oyun içinde dinlenmesi; salvo/topçu ayrımı, mesafe ve tekrar koruması. Bağlantının çalışması profesyonel ses kabulü sayılmaz.

Geniş ticaret, tam dünya siyaseti, seyahat, genel bina kataloğu ve bütün tarih olayları bu geçişte tamamlanmadı. Reference Library16karttır; istenen30–50kart ve kapsamlı müzik/edebiyat araştırması açık kalır. Kalite ölçütü test sayısı değil, aynı başlangıçtan farklı anlaşılır kararlarla farklı sonuç çıkarabilen bir oturumdur.
