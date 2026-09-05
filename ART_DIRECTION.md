# Power Above All — sanat ve arayüz yönü

Bu belge, proje sahibinin yeni görsel yönünü uygulanabilir ekran kurallarına dönüştürür. Mevcut kodun veya görsellerin bu hedefleri zaten karşıladığını iddia etmez. Bu aşamada yön ve plan hizalanıyor; arayüz kodunda değişiklik yapılmıyor.

Yön, kullanıcının metinle verdiği referans açıklamalarına dayanır. Bu karar için referans ekran görüntüsü veya görsel dosyası eklenmedi; burada görüntü karşılaştırması yapıldığı iddia edilmez.

Sistem ve ekran referanslarının görevleri [REFERENCES.md](REFERENCES.md) içinde ayrılmıştır. Depo belgeleri Türkçe; oyun arayüzü şimdilik Rusçadır.

Sonraki kapsamlı kullanıcı hedefi [DESIGN_V0.2.md](DESIGN_V0.2.md) içinde korunur. Bu görsel belgeyi oyun kuralları için tek kaynak saymayın; güncel uygulama sırası ve açık kararlar ROADMAP.md içindedir.

## Görsel kimlik

Fransa'nın siyasi ve askerî kararlarının alındığı bir çalışma masası hissi hedeflenir. Haritalar, belgeler ve insan yüzleri kararın konusunu açıklar; süsleme bu bilgiyi destekler.

Mevcut **koyu yeşil, parşömen tonları, mat altın ve kırmızı** paleti ile **serif başlıklar** korunur. Daha zengin hiyerarşi; daha çok çerçeve veya süs eklemek yerine boyut, boşluk, kontrast, hizalama ve bilgi gruplamasıyla oluşturulur.

| Görsel unsur | Kullanım kuralı |
| --- | --- |
| Koyu yeşil | Uygulamanın kalıcı çerçevesi, gezinme alanları ve güçlü başlık zeminleri. |
| Parşömen ve kâğıt tonları | Harita ve belge yüzeyleri. Hafif doku kullanılabilir; yazının okunurluğunu bozmaz. |
| Mat altın ve pirinç | Seçim, önemli ayrım çizgileri ve sınırlı vurgu. Her panelde parlayan süs olarak kullanılmaz. |
| Kırmızı | Tehlike, açık, radikalleşme veya olumsuz sonuç. Anlamı metin veya simgeyle de belirtilir. |
| Serif yazı | Başlıklar, kişi ve yer adları, belge karakteri. Sayısal tablolar ve küçük açıklamalarda okunurluk önceliklidir. |
| Bölge renkleri | Harita katmanına bağlı anlam taşır; aynı rengin anlamı katman açıklamasında gösterilir. Seçim yalnızca renkle anlatılmaz. |

Malzeme esinleri: gravürler, litografi estetiği, mürekkep, mum mühürler, kumaş, pirinç, resmî belgeler, dönem haritaları ve askerî haritacılık. **Litografi burada bir görsel esindir; kullanılan bir görselin 1789'dan kalma özgün bir eser olduğu iddia edilmez.** Gerçek tarihsel eser, üniforma, portre, sınır veya arma kullanılacaksa tarih ve kaynak ayrıca doğrulanır; esinlenilmiş çizimle tarihsel belge açıkça ayrılır.

Genel fantastik parşömen arayüzü, ahşap ortaçağ düğmeleri, yoğun altın süslemeler ve fantastik oyun arayüzleri bu yönün dışındadır.

## Harita — ana çalışma alanı

Görsel referans: **Europa Universalis V**. Fransa haritası, kampanyanın ana çalışma alanıdır. Oyuncu bir soruna baktığında nerede olduğunu ve komşu alanlarla ilişkisini kaybetmemelidir.

- Kalıcı üst alanda tarih, hazine, gıda, ordu ve kritik uyarılar gösterilir. Aynı anda en çok dikkat gerektiren değişim belirginleştirilir.
- Katman seçimi tek bir yerde toplanır: bölgeler/siyasi kontrol, ordu, tehlike, ekonomi ve nüfuz. Aktif katmanın adı ve açıklaması görünür olur.
- Bölge seçimi sınır, ad ve vurgu ile anlaşılır. Seçili bölge ile ordunun bulunduğu yer birbirinden ayırt edilir.
- Ordu işaretleri konum ve güç bilgisini; rota çizgileri hareketin hedefini gösterir. Tehlike işareti, tehdidin niteliğini kısa metinle açıklar.
- Bölge, ekonomi, siyaset ve karakter ayrıntıları haritanın üstünde açılan panellerde sunulur. Panel açılırken ilgili bölge veya konumu anlamaya yetecek harita alanı görünür kalır.
- Paneli kapatınca haritanın yakınlaştırması, seçimi ve etkin katmanı korunur. Dar ekranlarda aynı bağlam; görünür harita bölümü, seçili yer adı ve kısa yol bilgisiyle sürdürülür.
- Harita açıklaması yalnızca etkin katmana ait renkleri ve simgeleri açıklar. Dekoratif doku yolları, sınırları veya ordu işaretlerini bastırmaz.

## İç siyaset — güçler ve çıkarlar

Görsel referans: **Victoria 3**. İç siyaset ekranı, sayılarla birlikte bu sayıların arkasındaki güçleri gösterir. Oyuncunun karşısında yalnızca “destek” göstergeleri bulunmaz.

Her siyasi veya toplumsal güç için şu alanlar aynı bilgi grubunda sunulur:

- Adı, kimleri temsil ettiği ve lideri.
- Nüfuzu ve son değişiminin nedeni.
- Oyuncuya karşı tutumu ve bu tutumun nedeni.
- Somut talebi ve oyuncudan beklediği karar.
- Radikalleşme düzeyi, eğilimi ve varsa kritik sonucu.

Güçler, mevcut nüfuz veya acil risk gibi açık bir sıralama ölçütüyle düzenlenir. Liderin portresi karakter ayrıntısını açar. Bir kararın hangi gücü neden memnun ettiği veya kızdırdığı kararın yanında görünür. Modelde henüz hesaplanmayan ilişkiler, çalışan bir mekanik gibi gösterilmez.

## Karakterler ve olaylar — belirli bir insanla karar vermek

Görsel referans: **Crusader Kings III**. Bir olayın merkezinde kim olduğu ve ne istediği açıkça anlaşılır.

- Başlık bölümünde portre, ad, görev veya siyasi bağ bulunur.
- Oyuncuya kişisel tutum, başlıca çıkarlar ve olayla ilişkisi kısa biçimde gösterilir.
- Olay metni, kişinin talebini ve mevcut koşulu anlatır; seçimler belirli kişi veya kişilere bağlanır.
- Her seçenekte kararın özü ve bilinen doğrudan sonuçlar görünür. Belirsiz sonuçlar kesin rakamlarla vaat edilmez.
- Karakterin kişisel tutumu, temsil ettiği gücün genel tutumuyla karıştırılmaz.
- Portreler ve belge biçimi metnin önüne geçmez. Panelin arkasındaki harita, olayın yerini anlamayı sürdürür.

## Taktik savaş — alayları yönetmek

Ana görsel ve taktik referans: **Napoleon: Total War**. **Warcraft III yalnızca komutların tepkiselliği için referans olarak kalır.** Önceki tek tek asker yönetimi varsayımı bu yönle değiştirilmiştir.

- Seçim ve komut birimi alaydır. Tek asker doğrudan seçilmez veya yönetilmez; asker figürleri birliğin durumunu görselleştirir.
- Alayın konumu, baktığı yön, hat düzeni ve verilen hareket emri birlikte okunur.
- Seçimde menzil ve ateş yönü görünür hâle gelir; arazinin ve diğer birliklerin üzerini gereksiz çizgilerle kapatmaz.
- Kanat tehdidi, moral ve düzen kaybı metin/simge ve görsel durumla belirtilir. Asker sayısı, moralin yerine geçen tek durum göstergesi olmaz.
- Süvari ve topçu, piyadeden siluet, birlik işareti ve görev bilgisiyle ayrılır.
- Birlik paneli tür, mevcut güç, moral, düzen ve etkin emri açıklar. Verilen emir anında görsel karşılık bulur.
- Savaş bitiminde kayıplar, geri çekilme, ordunun yeri ve sefere etkisi tek bir sonuç anlatımında gösterilir. Sonuç yalnızca bir kez uygulanır.

Bu kurallar, mevcut prototipin bütün bu mekanikleri içerdiği anlamına gelmez. Görsel birlik sayısı ve sistemdeki asker sayısı farklı ölçeklerdeyse arayüz bunu açıklar.

## Ekonomi — ekmeğin yolunu ve açığın nedenini görmek

Görsel referans: **Anno 1800**. Ekonomi ekranı önce oyuncunun sorununu, ardından bunun hesabını açıklar.

Bir gıda veya kaynak ayrıntısı şu sırayı izler:

1. **Şimdi:** mevcut stok ve bu haftanın net değişimi.
2. **Kaynak:** hangi bölgeden veya üretimden ne kadar geldiği.
3. **Kullanım:** halkın, ordunun ve diğer tüketicilerin payı.
4. **Açık:** eksikliğin bulunduğu yer ve hesaplanabilen nedeni.
5. **Sonraki hafta:** tahmin, tahmini değiştiren etkenler ve oyuncunun kullanabileceği karar.

“Ekmek nereden geliyor, açık nerede, gelecek hafta neden kötüleşecek?” sorularının cevabı aynı görünümde bulunmalıdır. Toplamın yanında net değişim, değişimin yanında nedeni gösterilir. Baz üretim, gerçekleşen üretim, tüketim ve stok birbirinden açıkça ayrılır. Tahminler mevcut modelin sınırlarını aşan kesinlikte sunulmaz.

**Oyuncunun ana sorunu yaklaşık iki saniyede fark etmesi bir tasarım hedefidir.** Bu süre henüz kullanıcı testiyle ölçülmüş bir başarı veya garanti değildir. İncelemede önce ekranın ana mesajı sorulur; gerekiyorsa gerçek kullanıcı gözlemiyle süre ve anlaşılabilirlik değerlendirilir.

## Gelecek arayüz incelemeleri için kabul listesi

Bu liste tasarım değerlendirmesini tutarlı kılmak içindir; ek bir kullanıcı izni veya onay zorunluluğu oluşturmaz. Uygulanmayan mekanikler değerlendirmede “henüz uygulanmadı” olarak işaretlenir.

- [ ] Ana kampanya alanı Fransa haritası; panel açılması konum bağlamını ortadan kaldırmıyor.
- [ ] Etkin harita katmanı, renklerin anlamı, seçili bölge ve ordu konumu ayırt ediliyor.
- [ ] Panel kapatıldığında harita seçimi ve görünümü korunuyor; dar ekranda bağlam kaybolmuyor.
- [ ] Siyasette her gösterge somut bir güce bağlanıyor; lider, nüfuz, tutum, talep ve radikalleşme bulunuyor.
- [ ] Karakter veya olayın merkezindeki kişinin portresi, adı, kişisel tutumu ve çıkarı okunuyor.
- [ ] Seçenekler somut kişilere ve bilinen sonuçlara bağlanıyor; belirsizlik saklanmıyor.
- [ ] Savaş emirleri tek askere değil alaya veriliyor; hat, yön, menzil, kanat ve moral okunuyor.
- [ ] Süvari, topçu ve piyade birbirinden ayrılıyor; emirlerin görsel karşılığı açık.
- [ ] Ekonomi ekranında stok, üretim, tüketim, açık nedeni ve sonraki hafta tahmini ayrılıyor.
- [ ] Ana ekonomik sorun kısa bakışta anlaşılır; iki saniye hedefi ölçülmüş sonuç gibi yazılmıyor.
- [ ] Mevcut palet ve serif kimlik korunurken başlıklar, sayılar ve eylemler arasında açık hiyerarşi var.
- [ ] Kırmızı uyarılar ve harita anlamları yalnızca renge bağlı değil; dokular yazıyı veya işaretleri örtmüyor.
- [ ] Fantastik/ortaçağ arayüz kalıpları kullanılmıyor; tarihsel eser iddialarında kaynak ve dönem doğrulanmış.
- [ ] Hedeflenen tasarım, çalışan özellik ve tarihsel bilgi birbirinden açıkça ayrılmış.
