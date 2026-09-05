# Power Above All — olay tasarımının gelecek yönü

**Karar verildi: koşullara tepki veren olay sistemi, Visual & Feel Polish Pass tamamlandıktan sonra ele alınacak.** Bu belge gelecekteki tasarımı kaydeder; şimdi bir olay yöneticisi, yeni olay türü veya yeni ekonomi mekaniği uygulanmaz.

Mevcut aşamada tarayıcı 0.1'in ikinci haftadaki `grain-petition` kararı Unity'ye aynı davranışla taşınır: üç seçenek, karar verilene kadar haftanın ilerlememesi ve kayıttan sonra da yalnızca bir kez çözülmesi. Sabit ikinci hafta tetiklemesi bu özellik eşitliği çalışmasında korunur. Gelecekteki yön, mevcut prototipin bu davranışını sessizce değiştirme talimatı değildir.

## Olayın kaynağı: dünyanın gerçek durumu

Gelecekte olaylar çoğunlukla takvimde keyfî bir gün geldiği için değil, oyundaki durum ve oyuncunun kararları nedeniyle oluşmalıdır. Kaynaklar; gerçek kaynak açıkları, bölgesel kontrol ve huzursuzluk, siyasi tutumlar, karakter ilişkileri, yakın tarihli kararlar ve daha önce oluşmuş yükümlülükler olabilir.

Bir olayın söylediği şey simülasyonda doğru olmalıdır. Modelde işsizlik hesaplanmıyorsa metin, hesaplanmış işsizliği olayın nedeni gibi göstermez. Bir ordu ikmali alamıyorsa bu olay, ordunun gerçek durumuna ve geçerli güzergâhına bağlanır. Örnek metin, tarihsel alıntı veya doğrulanmış 1789 olayı olarak sunulmaz.

## Dört olay sınıfı

| Sınıf | Görevi |
| --- | --- |
| Tepki | Oyuncunun yakın tarihli bir kararına karakterlerin, grupların veya bölgelerin cevabını göstermek. |
| Kriz | Hesaplanan baskı veya eksikliğin önemli bir eşiğe gelmesini somut bir duruma dönüştürmek. |
| Fırsat | Elverişli koşul, başarılı karar veya iyileşen ilişkinin sağladığı olumlu imkânı göstermek. |
| Zincir | Önceki olayın seçimine, işaretine veya üstlenilen yükümlülüğe bağlı devam durumu oluşturmak. |

Olaylar yalnızca ceza veya kriz üretmemelidir. Başarı, güven kazanımı ve açılan fırsatlar da anlatılmalıdır. Bir devam olayı, ilk olayın sonucunu yok sayarak aynı seçimi tekrar sunmamalıdır.

## Tetikleme verisi ve tekrar kontrolü

Gelecekteki olay tanımı; dünya koşulları, daha önce gerçekleşenleri gösteren işaretler, gerekli sayaçlar ve tekrar bekleme süresi kullanabilir. Bu alanların amacı, olayın neden uygun olduğunu açıklamak ve aynı talebin sürekli dönmesini engellemektir.

- Önkoşullar, olayın güncel dünyaya uygun olup olmadığını belirler.
- İşaretler ve sayaçlar; önceki kararları, tekrarları veya yükümlülükleri izler.
- Bekleme süresi, benzer olayların art arda gelmesini sınırlar.
- Zincir bağı, devamın hangi seçimden doğduğunu açıkça tutar.
- Olay çözüldüğünde sonuç ve tüketilen hak bir kez uygulanır; kayıt/yükleme bunu tekrarlamaz.

Bu veri alanlarının kesin biçimi ve ağırlıkları gelecekte uygulama planında kararlaştırılır. Bu belgede tamamlanmış bir sistem veya çalışır veri şeması ilan edilmez.

## Öncelik ve tempo

Aynı anda uygun olan olaylar, önem ve bağlama göre sıralanır. Sıralamada **durumun ağırlığı, yakın tarihli oyuncu kararı, ilgili bölge, olay zincirinin devamı ve tekrar cezası** dikkate alınır. Burada sabit sayısal ağırlıklar belirlenmez.

Hedef tempo **haftada en fazla bir–iki önemli olaydır**; bu gelecekteki tasarım sınırıdır, mevcut kod davranışı veya her hafta mutlaka iki olay üretme kotası değildir. Uygun olay yoksa zorla bir olay oluşturulmaz. Bütün uygun koşulların aynı anda pencere açmasına izin verilmez.

## Bildirim ile kararın ayrılması

Her sonuç bir seçim penceresi gerektirmez:

- **Bildirim**, oyuncunun bilmesi gereken sonucu kısa biçimde harita, günlük veya raporda gösterir.
- **Karar olayı**, oyuncunun gerçek bir seçim yapmasını ve sonuç üstlenmesini gerektirir.

Seçimlerin bilinen anlık etkileri açıkça gösterilir; belirsiz gelecek sonuçları kesin vaatlere dönüştürülmez. Olayın kimden geldiği, hangi bölgeyle ilgili olduğu ve neden şimdi ortaya çıktığı anlaşılır olmalıdır. Dil değiştirmek olayın koşulunu, dünya durumunu veya daha önce seçilen sonucu değiştirmez.

## İçerik hedefi ve inceleme

Uzun vadede **30–50 nitelikli durum** hedeflenebilir. Bu sayı hemen yazılacak metin kotası veya bu aşamada tamamlanmış olay sayısı değildir. Her durumun farklı bir karar, bağlam veya sonuç zinciri sunması; aynı kalıbın ad değiştirilmiş tekrarından daha önemlidir.

Gelecekteki değerlendirmede şu sorular sorulur:

- Olayın tetiklenmesi gerçek dünya durumuyla açıklanabiliyor mu?
- Metin, ilgili karakter ve bölge hakkında doğru şeyler söylüyor mu?
- Yakın tarihli karar veya önceki olayla ilişkisi görünür mü?
- Olumlu olaylara ve fırsatlara da yer var mı?
- Tekrar ve pencere yoğunluğu kontrol ediliyor mu?
- Kayıt/yükleme ve Rusça/Türkçe değişimi aynı olayı ve sonucu koruyor mu?

Etkin aşama ve gelecekteki işlerin ayrımı [ROADMAP.md](ROADMAP.md), sistemler arası ana yön [DESIGN_V0.2.md](DESIGN_V0.2.md) içinde korunur.
