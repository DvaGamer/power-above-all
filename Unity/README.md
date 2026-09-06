# Power Above All — Unity projesi

**Unity 0.2 oynanabilir prototipi.** Sefer, siyasi kararlar, ekonomi, bölgesel reform ve alay savaşı birlikte çalışır. Son tam doğrulama 496 Unity testi, yeni Windows build, 18 PNG, 72 durum kontrolü, 14 kampanya JSON'u ve 10 tarayıcı testini geçti. Bu, tamamlanmış kampanya veya bütün görsel/ses işlerinin kabulü anlamına gelmez.

Güncel durum [STATUS.md](../STATUS.md), doğrulama kapsamı ve kalan sınırlar [NIGHT_REPORT.md](../NIGHT_REPORT.md), uzun vadeli tasarım [VISION.md](../VISION.md) içindedir.

- Editor sürümü: **6000.3.23f1**.
- Editor, Unity Hub ile kurulabilir; farklı kurulum yolu için `UNITY_EDITOR` ortam değişkeni kullanılabilir.
- Başlangıç sahnesi: `Assets/Scenes/Main.unity`.
- Oyun dilleri: **Rusça ve Türkçe**.
- Geçerli aşama: **Unity 0.2 prototipi**, geliştirme altında; tarayıcı 0.1 ayrı referanstır.

## Açma

1. Deponun kök dizinindeki `OPEN_UNITY.cmd` dosyasını açın. Başlatıcı Node.js kullanır ve bu `Unity/` klasörünü hedefler.
2. `Assets/Scenes/Main.unity` sahnesini açın.
3. Editor'de Play'e basın. Kaynak değişikliğinden sonra ilgili EditMode testlerini ve oyun akışını doğrulayın.

Başka bir bilgisayarda Hub'a deponun kökü yerine bu **Unity alt klasörünü** proje olarak ekleyin, uygun Editor lisansını etkinleştirin ve aynı Editor sürümünü kullanın. Deponun kökündeki `START.cmd`, ayrı tarayıcı 0.1 referansını açar.

Windows oyunu için Editor menüsündeki **Power Above All → Build Windows** komutunu kullanın. Derlenmiş oyun ve yerel doğrulama çıktıları Git klonuna dahil değildir. `PLAY_GAME.cmd`, önce mevcut sağlam GREEN doğrulama build'ini, bulunamazsa normal yerel Windows derlemesini açar. `node play-game.cjs --check` seçim yolunu ve doğrulama düzeyini oyunu başlatmadan gösterir.

## Kaynak kapsamı

Kaynaklar bağımsız C# sefer çekirdeğini, guaj atlası ve şehir siluetlerini, resimli kişileri, belge panellerini, taktik savaşı ve Rusça/Türkçe metin tablolarını içerir. Üç rolün hami sözleri, bölgesel vergi tatili, Dumas'nın erzak girişimi, ordu bütçe hedefi, subay hakkı, zafer kararları ve bölgesel reform aynı kampanyada saklanır. Arşiv v8, v1–v7 kayıtlarını önceki mekanizmaları koruyarak taşır.

Ekonomi önizlemesi gerçek haftayla aynı hesap yolunu kullanır; engellenmiş hafta gerçekleşmiş gibi sunulmaz. Asıl bölgenin durumu düşman kuvvetini belirler. Hareket, çoklu seçim, düzen, ateş ve salvo emirleri ortak taktik API'yi kullanır. Prosedürel foley sesleri taslak niteliğindedir; dinlenerek son kalite kabulü tamamlanmış sayılmaz.

## Doğrulama sınırı

**496/496 Unity EditMode testi** geçti. 25 kaynakta 692 RU/TR anahtarı doğrulandı. Yeni Windows build'in 141 dosyalık manifesti bulunur; başlatıcı manifestli doğrulama adaylarının bütünlüğünü denetler. Ayrı oyuncu senaryolarında doğal savaş sonucu, kampanyaya dönüş ve kayıt/yükleme eşitliği incelendi.

Gerçek Windows girdisiyle seçim, salvo ve çeşitli siyasi belgeler ayrıca incelendi. Reformun native fare senaryosu hazırlanmıştır, henüz yürütülmedi. Yeniden kullanılan build ile yapılan PARTIAL inceleme yeni tam GREEN değildir; otomatik PNG kontrolleri bütün görsellerin kalite kabulü yerine geçmez. Güncel kanıt ayrımı [NIGHT_REPORT.md](../NIGHT_REPORT.md) içindedir.
