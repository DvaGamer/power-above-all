# 24 haftalık bölgesel direniş karşılaştırması

`RegionalResistanceTrajectoryProbe.cs` / public Main sınıfı `RegionalResistanceTrajectoryProbe`. Yalnız yeni WorkNotes kaynağı; Core, taktik, testler ve Assets değiştirilmedi. Bu ajan çalıştırmadı; root'un merkezi probe aracına hazırdır.

İki bağımsız `Create()` legacy başlangıcı kullanılır. Birinde campaign politikası korunur; diğerinde yalnız hafta0'da `SetArmyEstablishment(state,"budget",1000)` uygulanır. Ardından24 başarılı NextWeek, ikisinde de aynı hafta2 dilekçesi için negotiate. Asker alma, yürüyüş, vergi, ekmek, sübvansiyon, sözleşme, zafer sonucu ve karakter müdahalesi yoktur. State alanlarına doğrudan yazılmaz. Dumas'ın mevcut otomatik toplaması engellenmez; ilan ve uygulama, gerçek bütçe/gıda durumundan doğar. Bu nedenle karşılaştırma “diğer tüm son durumlar eşit” değildir: yalnız oyuncu tercihleri eşittir; ekonominin ürettiği sonuçlar ayrışabilir.

Her turun başlangıcında Forecast ve Dumas terms okunur; başarılı hesabın Gold/Food sonucu aynı önizlemeyle karşılaştırılır. Released, gerçek Manpower artışıdır; Lost, önceki canlılar−son canlılar−Released olarak ayrılır. Böylece demobilizasyon kayıp, açlık da rezerv üretimi gibi gösterilmez. Açlık ve ödeme baskısı ortak önizlemeden hesaplanır; aktif toplamanın gerçek Food bileşeni ayrıca kaydedilir. Siyasi/bölgesel göstergeler için ikinci bir simülasyon yazılmaz: her hafta doğrudan gerçek state ve GetRegionalResistance okunur.

Çıktı bölümleri:

- `COMPARISON`:12 bölgenin başlangıç U/C/E ve düşmanı, her iki rotanın hafta24 U/C/E ve düşmanı, ilk düşmanlık haftası, bütün görülen eşik geçişleri, son kuvvet farkı ve bölgesel tepe değer/haftası. İlk düşmanlık−1 ise24 hafta boyunca görülmemiştir;0 ise başlangıçta vardır. U/C/E sırası Unrest/Control/EliteLoyalty'dir.
- `RESISTANCE_WEEK`:0–24 arasında düşman bölge sayısı ve en büyük kuvvet/bölge. Örnekleme, o haftanın zorunlu dilekçe cevabından sonradır; ara GUI kareleri veya sürekli zaman iddiası değildir.
- `ECONOMY_WEEK`:önce/sonra asker ve stoklar; gerçek rezerv aktarımı ve kayıp; vergi/ordu hesabı, açlık/ödeme baskısı, toplama miktarı ve önceki plandaki Dumas disposition. Bu satırlar siyasi ayrışmanın hangi ekonomik haftayla birlikte başladığını incelemeyi sağlar.
- `TRANSITION`:65 eşiği aşılınca veya geri inilince hafta, durum ve gerçek U/C/E. `SUMMARY`, son enemyMax ile bütün rotanın tepesini ayrı yazar; bunlar karıştırılmaz.

Önizlemenin archive metnini değiştirmediği her gözlemde kontrol edilir; her final save/load aynı state ve12 kuvveti korumalıdır. Aynı ilk bölgesel göstergelerin politika imzasıyla değişmediği de karşılaştırılır. Bütün ordunun24 hafta yerinde tutulduğu pasif yönetim bir baskı senaryosudur; normal oyuncunun tek makul davranışı olarak sunulmaz. Gerçek savaş yapılmadığından kazanma yüzdesi, kaç kişinin yettiği veya otomatik kayıp tahmin edilmez. Henüz sayısal sonuç veya PASS yoktur.
