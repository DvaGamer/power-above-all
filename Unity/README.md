# Power Above All — Unity projesi

**Editor kuruldu ve lisans etkinleştirildi; ilk Play denemesi kısmen yapıldı.** `Main.unity` açıldı ve sefer haritası çizildi. Bu denemede görülen TacticalBattle yerel nesne başlatma hatası kaynakta düzeltildi. **Düzeltme sonrası yeniden derleme ve açılış doğrulanmadı.** Tam oyun akışı, görseller ve Windows oyun derlemesi tamamlanmış sayılmaz. İlk açılıştaki lisans hatası geçmiş bir engeldir.

Kullanıcı geliştirmeyi durdurdu. Kalan gereksinimler ve kontroller [POLISH_PLAN.md](../POLISH_PLAN.md) içinde korunur.

- Editor sürümü: **6000.3.23f1**.
- Bu bilgisayardaki kurulum: `C:/Users/USER/Tools/Unity/6000.3.23f1/Editor/Unity.exe`.
- Başlangıç sahnesi: `Assets/Scenes/Main.unity`.
- Oyun dilleri: **Rusça ve Türkçe**.
- Geçerli aşama: **0.2 Visual & Feel Polish Pass**; yeni mekanik veya ekonomi genişletmesi yok.

## Açma

1. Deponun kök dizinindeki `OPEN_UNITY.cmd` dosyasını açın. Başlatıcı Node.js kullanır ve bu `Unity/` klasörünü hedefler.
2. `Assets/Scenes/Main.unity` sahnesini açın.
3. Geliştirmeye devam edildiğinde son kaynak düzeltmesinden sonra Play denemesini ve testleri çalıştırın; sonuçları doğrulama kaydına ekleyin.

Başka bir bilgisayarda Hub'a bu alt klasörü proje olarak ekleyin, uygun Editor lisansını etkinleştirin ve aynı Editor sürümünü kullanın. Deponun kökündeki `START.cmd`, ayrı tarayıcı 0.1 referansını açar.

## Kaynak kapsamı

Kaynaklar; bağımsız C# sefer çekirdeğini, prosedürel kabartma atlası, şehir minyatürlerini, belge panellerini, taktik savaşı, kayıt bağlantısını ve Rusça/Türkçe metin tablolarını içerir. Mevcut ikinci hafta ekmek dilekçesi de belge olarak aktarılmıştır.

Görsel iyileştirmeler arasında hareket eden ordu sancağı, bölge ve emir vurguları, kaynak sayılarının geçişi, okunabilir emir gerekçeleri ve karar günlüğü vurgusu bulunur. On prosedürel foley sesi taslak niteliğindedir; bitmiş ses üretimi değildir.

## Doğrulama sınırı

Saf C# çekirdeği bağımsız derleyiciyle kontrol edildi. Editor'deki ilk Play denemesi, haritanın çizildiğini ve ayrıca bir TacticalBattle başlatma hatasını gösterdi. Son düzeltme sonrası yeniden derleme ve çalışma henüz kontrol edilmedi. Kaynakta 14 NUnit editör testi bulunur; **Unity Test Runner içinde çalıştırılmadı**.

Play akışı, Rusça/Türkçe görsel inceleme ve Windows oyun derlemesi ayrıca doğrulanmalıdır. Bağımsız C# derlemesi bunların yerine geçmez. Güncel durum [STATUS.md](../STATUS.md) içinde tutulur.

Editor'deki `Power Above All/Build Windows` komutu derleme aracını çağırır. Henüz başarılı bir oyun derlemesi kaydı yoktur.
