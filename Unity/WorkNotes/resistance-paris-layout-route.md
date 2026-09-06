# Paris emirlerinin uzun yerleşimi için kısa rota

Kaynak: `ResistanceParisLayoutProbe.cs`, public Main sınıfı `ResistanceParisLayoutProbe`. Bu ajan çalıştırmadı. Sıfır hafta,11 başarılı değiştirici Core komutu: bir normal alım, bir anlaşma, sekiz ekmek yardımı, bir barışçıl yürüyüş. Bölge seçimi oyuncu katmanındaki mevcut komuttur; Core probe başlangıçtaki Ile seçimine dokunmaz.

1. Yeni army rolü. Ile'de normal recruit:1400 asker,340 Food.
2. Ile için vergi tatili imzala: Unrest40, Control74; bitiş hafta4.
3. Sırayla brittany, normandy, picardy, champagne, lorraine, burgundy, orleans, poitou bölgelerine birer bread emri: her biri40 Food, sonunda20.
4. Normandy'yi seç ve march: yürüyüş henüz aç değildir,14 Food harcar. Varışta1400 asker, Food6, Moves1; Gold720/Supplies100/Manpower2200. Gerçek attrition olmamıştır.
5. UI'de Ile'yi yeniden seç. Core probe'da seçim başlangıçtan beri Ile'dir. İkinci yürüyüşü, vergiyi veya haftayı uygulamadan RU/TR atlas karelerini al.

Beklenen eşzamanlı içerik: sakin bölge için gerçek resistance summary (düşman0); asker Normandy'de, hareket yapılabilir; geri yürüyüş14 Food ister ve elde6 olduğu için Hungry uyarısı; Ile'deki dört haftalık tatili bozacak tax uyarısı; Paris'in mevcut subsidy satırı. TaxUsed ve BreadUsed false, bölgedeki normalRecruitUsed true; ikinci normal alım şu anda hem kullanılmış hem ordu başka yerde. Bread için6 Food yetersizdir ama mevcut buton/satır gizlenmemelidir. Petition ve mandate yoktur.

Bu düzen kaynakları bilerek tüketen okunabilirlik sınırıdır; dengeli24 haftalık politika veya önerilen normal oyuncu davranışı değildir. Bread kararlarının gerçek şehir onayı ve bölgesel sonuçları korunur. Yeni oyun kuralı, state injection, özel kayıp veya savaş sonucu yoktur. Kesin değerler probe'un kontrol ettiği kaynak aritmetiğidir; root çalıştırmadan doğrulanmış çıktı veya screenshot olarak sunulmaz.
