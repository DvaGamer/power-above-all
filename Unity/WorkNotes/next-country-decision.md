# Devletin bir sonraki yönetim kararı

6 Eylül2026; yalnız tasarım, uygulama yapılmadı. Kişiler ve miktarlar oyun modelidir; yeni tarihsel iddia yoktur. Kaynaklar mevcut `CampaignCore` asker alma, haftalık tüketim ve garnizon kuralları; ortak ekonomi projeksiyonu ve Dumas girişimidir. Root son kapıyı230 Unity testiyle GREEN bildirdi; aşağıdaki yeni politika o kapının parçası değildir.

## Gerçek yönetim boşluğu

Oyuncu mevcut `Act(recruit)` ile200 asker ekleyebiliyor; yaşayan askerleri planlı biçimde sivil insan gücüne döndüremiyor. Büyük orduyu besleyemeyince Paris yardımını kesebilir veya Dumas'nın sınırlı müdahalesine dayanabilir; ordunun sürdürülebilir büyüklüğünü aşağı doğru yönetmek için olağan bir araç yok. Zafer ve kayıplar dışında değişmeyen bu büyüklük, bütçe politikasından çok tek yönlü büyüme düğmesine dönüşüyor.

## Üç farklı devam

**A — Sürekli ordunun hedef mevcudu.** Hükümet “sefer serbestisi” veya “bütçe mevcudu” belirler. İkinci politikada hedefin üzerindeki askerler iki hesaplık hazırlıktan sonra200 kişilik gruplarla mevcut Manpower havuzuna döner. Daha az asker maaş/iaşe giderini gerçekten azaltır, fakat sonraki muharebeye daha küçük kuvvet girer. Bu bir para karşılığı istatistik ödülü değil, aynı insanları ülke ile sahra ordusu arasında taşıyan yönetim kuralıdır. **Önerim A.**

**B — Yiyecek dağıtım önceliği.** İki haftalık hazırlıkla ordu önceliği veya mevcut ortak dağıtım seçilir. Kıtlıkta askerleri önce beslemek sahra kuvvetini korur; sivil açlık ve şehir direnci büyür. Lefèvre karşı çıkar, Dumas destekler. Maddi bir ulusal siyaset olur, ancak sivil/asker açlığı, moral ve kayıp sonuçlarını açıkça ayırmak gerekir. Yeni ikmal girişimi için özellikle korunan eski ikili açlık kuralını küçük bir ek gibi sessizce değiştirmemelidir; ayrı tasarım olarak ertelenmeli.

**C — Yerel idareye yetki devri.** Bir bölge iki hesap boyunca merkezî tahsilattan yerel temsilcilerin yönettiği düzene geçer. Oyuncu bazı olağanüstü müdahale imkânlarından vazgeçer; yerel düzen daha az merkez emrine ihtiyaç duyar. Morel ve yerel elitler kazanır, Valcourt yetki kaybına karşı çıkar. Gelir, direniş ve oyuncunun emir alanını birbirine bağlar. Ancak yalnız “daha az vergi/daha az huzursuzluk” olursa mevcut dört haftalık uzlaşmayı tekrarlar; gerçek yerel karar yetkisi eklenmeden uygulanmamalıdır.

## Önerilen A nasıl politika olur?

Olağan başlangıç “sefer serbestisi”dir; eski davranış korunur. Konseyde veya mevcut ordu belgesinde oyuncu daha düşük bir hedef seçer. İlk küçük dilim200 kişilik adımlarla çalışabilir. Kaynak veya kişisel güç eşiği gerekmez; kararın gerçek bedeli sahra kuvvetidir. Bütçe görünümü, asker maaşı, asker gıdası ve hedefe geçiş tarihini birlikte gösterir.

Hedefin üzerinde mevcut varsa ilk azaltma **iki gerçek haftalık hesap sonra**, her seferinde en fazla200 asker olur. Hedefe ulaşılana kadar aynı aralıkla sürer. Örneğin1200→1000 tek grup;1200→600 üç gruptur. Bu, her tur ayrı ödül satın almak yerine yürürlükte kalan bir yönetim tercihidir. Aynı anda ikinci azaltma kuyruğu açılmaz; hedef değişikliği ilk tarihi ileri atarak bedelsiz erken sonuç üretmemelidir.

Hazırlık sırasında insanlar gerçek orduda kalır: savaşır, kayıp verir, maaş ve gıda tüketir. Ayrı, zarar görmeyen bir “bekleyen asker deposu” yoktur. Tamamlanırken `min(200, max(0, yaşayanTroops−hedef))` kişi ayrılır ve aynı sayı Manpower'a döner. Savaş orduyu zaten hedefin altına indirmişse kimse yoktan geri gelmez. İnsan gücü kapasitesi daha karar anında ayrılacak toplamı karşılayabilmelidir; sessiz silme veya taşma kabul edilemez.

Oyuncu sefer serbestisine dönerek henüz uygulanmamış azaltmaları durdurabilir. Ayrılmış askerler bedelsiz anında geri gelmez: mevcut recruit hâlâ120 altın/20 gıda/15 malzeme/200 insan gücü ister. Bütçe politikası sürerken olağan asker alma tamamen kilitlenmez; geçici büyüme hedef üstü azaltmayı yeniden planlar ve önizlemede tarihi görünür. Böylece acil bir savaş için kısa süreli takviye mümkündür, fakat sonraki mevcut değişimi unutulmaz.

## Kim kazanır, kim karşı çıkar?

Hazine ve gıda stoku daha yavaş tükenir. Bu, Morel'in vergi tatilini sürdürmeye veya Lefèvre'nin düzenli Paris yardımını finanse etmeye alan açar; otomatik yeni destek puanı verilmez. Dumas daha küçük bir savaş aracı ve daha az askerî nüfuz görür. İlk aday tepki, gerçekten ayrılan her grup için ilişki−4'tür; mevcut veto tepkisi ölçeğindedir ve açıkça gösterilmelidir. Hiç kimse ayrılmayan iptal/kayıp durumuna bu ceza uygulanmaz. Yeni darbe, gizli itaatsizlik veya ücretsiz yetkinlik değişimi yoktur.

Sıfır hedef de düşünülürse garnizon etkisi ayrıca görünmelidir: ordu0 olduğunda mevcut bölge haftalık−3 huzursuzluk ve+2 kontrol katkısını kaybeder. Normal insan gücü/asker alma toparlanması açık kalır. İlk uygulama yalnız kısmi azaltmayı sunabilir; sıfırın gizli oyun sonu olması kabul edilemez.

## Sayılar ve mevcut sistemlerle bağ

1200→1000 örneğinde, diğer koşullar sabitken mevcut formüller asker giderini136→120, asker gıdasını40→34 yapar: haftada16 altın ve6 gıda tasarrufu. Teçhizatın36 altını ortadan kalkmaz. Bunlar kaynak aritmetiğidir, yeni player ölçümü değildir. Bir grup ağır açlığı tek başına bitirmeyebilir; arayüz tam gerçek Forecast'i göstermelidir.

Azaltma, vade haftasının **eski mevcutla hesaplanmış** bütçesinden sonra olur. İlk tasarruf sonraki hesapta başlar; geri ödeme veya o haftaya ikinci maaş hesabı yoktur. Mevcut Dumas müdahalesi aynı haftanın eski gerçek ihtiyacına göre uygulanır. Daha sonra küçülen ordu yeni açlık riskini azaltabilir; bu ortak projection yoluna sonraki tarih olarak eklenir, ayrı sahte ekonomi oluşturulmaz.

İkinci haftanın dilekçesi ve dueMandate retleri her değişiklikten önce kalır. Dört vergi hesabı, eski sözün bölgesi/fiyatı ve zafer teklifinin başarılı haftada kapanması değişmez. Personel azaltması hesap sonunda otomatik olur; yeni zorunlu modal açılmaz. Yeni Dumas duyurusu değerlendirileceğinde artık gerçekten kalan ordu kullanılır. Küçük kalıcı aday: politika kimliği, hedef mevcut ve sonraki azaltma haftası; ayrı personel listesi gerekmez.

## On dakikalık gösterim ve karar gerekçesi

İlk iki dakikada oyuncu1200 kişilik ordunun maliyetini görür ve1000 bütçe hedefini seçer. İlk hesapta henüz tasarruf olmadığını okur. İkinci hesabın sonunda200 kişinin Manpower'a döndüğünü, üçüncü hesap için16 altın/6 gıda farkını ve Dumas'nın tepkisini görür. Bu sırada eski dilekçeye olağan cevabını verir.

Sonraki dakikalarda aynı dirençli komşuya daha küçük orduyla gitmeyi veya tasarrufu koruyup uzlaşmayı seçer. Yeni savaş gerektiğinde eski recruit fiyatıyla geri büyümek, iki haftalık kararın fırsat maliyetini görünür yapar. Gösterim yeni politika testini gerektirir; mevcut ekranlardan tamamlanmış oyuncu kanıtı çıkarılmaz. Bu devamın değeri, daha güçlü sayı vermesinde değil, oyuncuya devletin taşıyabileceği orduyu planlama ve gerektiğinde kararının maddi bedelini ödeme imkânı vermesindedir.
