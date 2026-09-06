# Taktik kurallar — eşzamanlı ateş ve cephanesiz piyade

**Salt okunur öneri.** Kaynaklar `verification-night.md` taktik incelemesi ile `TacticalBattle.Simulate`, `Shoot`, `OrderVolley`, `EnemyOrders`, `CanAttack`, `SetCondition` ve `UpdateObjective`. Bu görevde savaş kodu değiştirilmedi; yeni Unity/test süreci çalıştırılmadı.

## Doğrulanan yapısal sorunlar

- `Simulate` oyuncu alaylarını önce işleyen listede, hasarı ve bozgunu hemen uygular. Aynı adımda ateşe hazır düşman, sıra kendisine gelmeden bozulabilir. `OrderVolley` ayrıca doğrudan `Shoot` çağırır; yalnız otomatik ateşi toplu çözmek bu ikinci öncelik yolunu açık bırakır.
- `CanAttack`, süvari dışındaki `Ammo=0` alayları mesafeden bağımsız reddeder. Düşman piyadesi yine 12/16 mesafede durur. Konvoyu iki tarafın cephanesiz piyadesi tartışmalı tuttuğunda savaş, sabit emirlerle ilerlemeyebilir. Bu, bütün oyuncu manevralarıyla mutlak sonsuzluk iddiası değildir.
- `SetCondition` komşu birlik moralini de hemen düşürür. Hasar toplulaştırılsa bile bozgun dalgası ayrı düzenlenmezse ikincil sıra etkisi kalır.

## Üç farklı yaklaşım

1. **Her 20 Hz adımda niyetleri birlikte çözmek.** Hareket hazırlığı, ateş/yakın temas niyeti, toplu hasar, bozgun dalgası, hedef kontrolü şeklinde açık aşamalar. Mevcut tempo ve karşılıklı ateşe en küçük kavramsal değişimle adalet getirir.
2. **Kurma–ateşleme–isabet zaman çizgisi.** Atışlar ilan edildiğinde gelecekteki kesin ateşleme/isabet anlarına kaydolur. Daha erken tamamlanan gerçek hazırlık öncelik kazanır. Mermi ve yakın saldırı animasyonlarıyla güçlüdür; ancak yeni zamanlama, iptal kuralları ve daha geniş görsel değişiklik ister.
3. **Görünür inisiyatif çözümü.** Tecrübe, komuta ve hazırlık aynı adımdaki çatışmaların sırasını açıkça belirler; tam eşitlikler birlikte çözülür. Bilinçli bir oyun kuralıdır, fakat yeni denge/arayüz yükü getirir. Listeyi rastgele karıştırmak bunun yerine geçmez.

**Öneri: 1.** İkinci ve üçüncü seçenek sonraya kalmalı. Mevcut siper, arazi ve moral değerleri korunurken iki kanıtlanmış davranış düzeltilir.

## Adım sözleşmesi

1. Adım başındaki durumu dondur. Zamanlayıcılar, AI hedefleri, yürüyüş ve dost birlikten kaçınma bu ortak görüntüden hesaplanıp birlikte uygulanır. `Move` içindeki dost konumlarını sırayla değiştirmekten kaçın; eşit mesafeli hedefte kararlı alay kimliğiyle bağ kır.
2. Güncel konumlarla ikinci bir görüntü oluştur. Her canlı/kaçmayan alay için en fazla bir `AttackIntent` üret: saldırgan, hedef, `Ranged`/`Contact`, nişanlı olup olmadığı. Hazır karşılıklı atışlar aynı görüntüden doğar.
3. Rastgele hasar çekilişleri kararlı alay kimliği sırasına bağlanabilir; bu sıra yalnız rastgele değerin atanmasını belirler, ateş hakkını veya isabetin önce uygulanmasını belirlemez. Önce bütün sonuçlar hesaplanır. Cephane/yorulma/bekleme maliyeti saldırgana bir kez; toplam kayıp/moral/bütünlük etkisi hedefe bir kez uygulanır. Toplam kayıp hedefin adım başı mevcudunu aşamaz. Aynı adımda vurulan alayın önceden hazır atışı iptal edilmez.
4. İlk bozulanları birlikte belirle. Yakındaki dostlara şokları topluca uygula; yeni bozgun varsa bir sonraki dalgada işle. Her alay yalnız ilk kez bozulduğunda şok yayar; dalga sayısı alay sayısıyla sınırlıdır.
5. Hedef sayaçları ve mevcut bitiş koşulları son durumu görür. Karşılıklı bozgun yeni beraberlik sistemi yaratmaz; konvoy görevinin mevcut başarı/yenilgi sözleşmesi korunur.

`OrderVolley` artık doğrudan hasar vermez: `AimedVolleyPending` niyeti koyar, bir sonraki simülasyon adımı tüketir. Aynı adımda tekrarlanan tıklamalar birleşir; otomatik ateş ikinci atış üretmez. Duraklatma sırasında zarar veya cephane tüketimi olmaz. Tüketim anında koşullar değişmişse atış reddedilir ve açıklanır; isabet sesi yalnız gerçekten uygulanan saldırıdan gelir.

## Yakın temas: küçük, ayrı bir saldırı türü

- Hat piyadesi, milis ve süvari yakın temasa girebilir; topçuya bu paketle süngü savaşı verilmez. Kaçan/çekilmiş alay saldırmaz. Başlangıç temas eşiği mevcut süvari erişimi olan 3,7 dünya birimidir; ölçü görsel ayak iziyle ayrıca incelenir.
- Cephane yalnız menzilli saldırıda harcanır. Piyade cephanesi sıfırken yakın temas saldırısı geçerlidir. `FireAtWill=false` tüfek ateşini durdurur; fiziksel temastaki savunmayı kapatmaz. Oyuncu mevcut ilerleme emriyle temasa girebilir; cephanesiz diye kendi kendine uzaktaki düşmanı kovalamaz.
- Yeni `ContactReload` yakın saldırı aralığını izler. Örnek ilk değer: 3,4 saniye, yorgunlukla uzar. Menzilli `Reload` ayrı kalır; bir saldırı diğer türün zamanlayıcısına en az 0,6 saniye toparlanma koyar. Böylece son tüfek atışı piyadeyi sekiz saniye savunmasız bırakmaz; mesafe eşiğini geçip çıkmak da anlık çift saldırı sağlamaz.
- Deneme yakın hasarı: `sqrt(Men) × katsayı × bütünlük × yorgunluk × tecrübe`. Hat katsayısı 0,18, milis 0,14 başlangıç önerisidir; süvarinin mevcut temel katsayısı korunur. Moral şoku kayba eklenir; asıl hedef yok etmekten önce bozmadır. Atışa özel nişan, topçu/kare ve yükseklik-atış bonusu yakın temasa taşınmaz. Süvariye karşı karenin mevcut koruması korunur. Kaynakta zaten var olan flanş ve moral ilişkileri kullanılır; yeni zırh/çarpışma sistemi eklenmez.
- AI cephanesiz piyadeyi 12/16 mesafede durdurmaz: düzeni/morali yeterliyse temasa yaklaşır. Zayıf birliğin geri çekilmesi mevcut `Routed` veya açık geri çekilme davranışıyla anlatılmalı; görünmez bekleme veya otomatik teslim eklenmemeli. Kartta cephane yokluğu ile yakın temas hazırlığı ayrı okunur.

## Anlamlı regresyon kapısı

1. Aynı komuta, hazır atış ve tek yaylımla bozulabilecek moralle iki eşit alay: normal/ters alay listesinde ikisi de bir cephane tüketir; kimlik bazında son durum aynıdır. Ek test aynı anda gelen iki saldırının toplam kaybını mevcutla sınırlar.
2. Nişanlı oyuncu emri ile hazır AI atışı aynı adımda: ikisi de çözülür; iki tıklama iki atış olmaz. Ara görsel güncellemeler sonucu değiştirmez.
3. Üç yakın dost alayda bozgun dalgası: ters liste sırası aynı kaçanları ve aynı moral değerlerini üretir; her bozgun şoku yalnız bir kez oluşur.
4. Konvoy yanında cephanesi sıfır iki piyade, başlangıç mesafesi 4: AI temasa girer, cephane negatifleşmeden kayıp/moral/pozisyon değişir. Uygun süre sonunda sabit çatışma kilidi kırılır; bütün savaşları zorla bitiren zaman aşımı eklenmez.
5. Ateşi kesmiş piyade temasta savunur fakat uzaktaki düşmana tüfek atmaz; geri çekilen alay vuramaz; sıfır cephaneli topçu piyade gibi saldırmaz. Yorgunluk 100 yakın saldırıyı kesin sıfır hasara kilitlemez.
6. Mevcut aynı zaman damgası, duraklatılmış efekt/ses ve tek rapor/callback testleri korunur. Doğrudan özel `Shoot` çağıran testler yeni niyet→çözüm sınırından geçerek aynı oyuncu sonucunu doğrulamalı; bozuk eski sırayı koruyan bir kısayol bırakılmamalı.

## Tepe/LOS — ayrı ve sonraki karar

Model gerçek `TerrainHeight(x,z)` ve bu fonksiyondan üretilen görünen arazi mesh'i içeriyor; tepe yüksekliği merkezde 2,9. Bu yüzden ileride uç noktalar arasındaki araziyi örnekleyerek doğrudan ateş hattı denetlenebilir. Mevcut sistem yalnız yüksek uç noktanın ateş bonusunu hesaplıyor; engelleme şu anda vaat edilmiş davranış değil.

Dar LOS değişikliği yapılacaksa asker/top namlu yüksekliği, arazi örnekleme adımı ve teğet isabet toleransı ortak fonksiyonda tanımlanmalı. Seçili ateş yayı ve atış reddi aynı sonucu göstermeli: tepeye çarpan nişan çizgisi ve kısa RU/TR neden. Yeni orman gizlenmesi, balistik topçu, bina çarpışması veya yeni arazi türü bu iki düzeltmeye eklenmemeli. Önce mevcut tepenin kadrajda örtü gibi okunup okunmadığını root gerçek karede değerlendirmelidir.
