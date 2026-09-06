# Önceki referans yönü — arşiv

Bu belge eski kullanıcı yönünü korur; doğrulanmış araştırma kartı veya güncel uygulanmış kapsam değildir. Yeni giriş [REFERENCES.md](../REFERENCES.md). Eski göreli belge adları depo köküne aittir.

Bu belge, proje sahibinin başlangıçta seçtiği dört sistem referansını ve sonradan belirlediği beş görsel/arayüz referansını birlikte korur. Yeni görsel yön, önceki savaş sunumu varsayımlarını aşağıda belirtildiği şekilde günceller. Bunlar geliştirme hedefleridir; özelliklerin tamamı mevcut prototipte bulunmuyor.

## Sistem referansları

### Warcraft III — komutların tepkiselliği

Pratikte korunacak referans, seçime ve komuta hızlı yanıt veren kontrol hissidir. Seçimin, verilen emrin ve sonucunun açıkça görülmesi hedeflenir. Yeni yön doğrultusunda oyuncu tek tek askerleri kontrol etmez; komutların hedefi alaylar ve birlik düzenleridir. Önceki bireysel asker kontrolü varsayımı geçersizdir. Komutanların kişisel özellikleri ve emir yeteneklerinin savaşa etkisi ayrıca tasarlanacaktır. Savaşın düzeni ve görsel dili için aşağıdaki Napoleon: Total War referansı esas alınır.

### Total War — sefer ve dünya haritası

Şehirler, güzergâhlar, orduların konumu, arazi, ikmal, kuşatmalar ve taktik savaşa geçiş gibi harita üzerindeki kararlar için referans alınır. Savaşın kayıplar, geri çekilme, deneyim ve bölge kontrolü üzerindeki sonuçları sefere eksiksiz aktarılmalıdır. Haritanın ve içeriğin merkezinde Fransa yer alır; dünyanın kapsamı ancak bölgesel oyun döngüsü çalışır hâle geldikten sonra genişletilmelidir.

### Europa Universalis V — ekonomi

Nüfus, tüketim, üretim, pazarlar, ticaret ve altyapı arasındaki ilişkiler için referans alınır. Ordu, ekonomiye ve ikmale bağımlıdır. Başlangıçta az sayıda ürün kullanılmalı ve göstergelerin neden değiştiği oyuncuya açıkça anlatılmalıdır. İlk prototipte özgün oyunun tüm karmaşıklığını yeniden üretmek hedeflenmez.

### Crusader Kings III — siyaset ve diplomasi

Karakterlerin kişisel çıkarları, ilişkileri, hırsları, bağlantıları, veraset ve askerî kararların siyasi sonuçları için referans alınır. Bu yaklaşım devrim dönemi Fransa'sına uyarlanmalıdır: saray, toplumsal zümreler, temsilciler, bakanlar, generaller, siyasi kulüpler ve değişen kurumlar. Feodal vasallık ilişkileri 1789'a doğrudan taşınmamalıdır.

## Görsel ve arayüz referansları

Bu beş referans, yeni ekranların bilgi düzenini ve oyuncuya hangi kararları görünür kılacağını belirler. İlgili oyunların arayüzlerini birebir kopyalamak anlamına gelmez.

Bu yönler kullanıcının metin açıklamalarından kaydedildi; referans görsel dosyası veya ekran görüntüsü sunulmadı. Görsel karşılaştırmayla doğrulanmış uygulama sonuçları değildir.

| Referans | Power Above All için uygulanacak yön |
| --- | --- |
| **Europa Universalis V** | Harita ana çalışma alanıdır. Bölge renkleri, ordu konumları ve tehlikeler okunur olmalı; bölge/siyasi kontrol, ordu, tehlike, ekonomi ve nüfuz katmanları arasında geçiş yapılmalıdır. Ayrıntı panelleri açıldığında harita bağlamı görünür kalır. |
| **Victoria 3** | İç siyaset, gerçek siyasi ve toplumsal güçler üzerinden gösterilir. Her gücün nüfuzu, oyuncuya karşı tutumu, lideri, talepleri ve radikalleşmesi görünür olmalıdır. Yalnızca üç soyut destek çubuğu nihai tasarım sayılmaz. |
| **Crusader Kings III** | Karakter ve olay ekranlarında portre, ad, kişisel tutum ve çıkarlar birlikte görünür. Karar, belirli bir kişiye veya belirli kişilerin taleplerine bağlanır. |
| **Napoleon: Total War** | Taktik savaş alaylar, hatlar, menzil, kanatlar, moral, süvari ve topçu üzerine kurulur. Oyuncu tek tek askerleri yönetmez. Birliğin düzeni ve görevi görsel olarak anlaşılır olmalıdır. |
| **Anno 1800** | Ekonomi neden-sonuç ilişkisiyle okunur: ekmek nereden geliyor, açık nerede oluşuyor, gelecek hafta neden kötüleşecek? Oyuncunun temel sorunu iki saniyede fark etmesi bir tasarım hedefidir; ölçülmüş bir sonuç değildir. |

Malzeme ve uygulama kuralları [ART_DIRECTION.md](../ART_DIRECTION.md) dosyasında tanımlanır. Koyu yeşil, parşömen tonları, mat altın, kırmızı ve serif yazı karakterleri korunur; bilgi hiyerarşisi zenginleştirilir.

## Oyunun kendi yaklaşımı

Güncel kapsamlı hedef [DESIGN_V0.2.md](../DESIGN_V0.2.md) içindedir. Ana ilke çok sayıda bağımsız mekanik değil, oyuncunun siyasi kararından başlayıp ekonomiyi, bölgeyi, ikmali ve savaşı etkileyerek tekrar kişisel iktidara dönen anlaşılır sonuç zincirleridir.

Adı: **Power Above All**. Mekân Fransa, başlangıç tarihi 5 Mayıs 1789. Ana tema, eski düzen çözülürken kişisel iktidar, çıkar hesabı, nüfuz ve kararların bedelidir. İlk taslakta oyuncu, basitleştirilmiş bir kraliyet konseyini yönetir; nihai oyuncu karakterinin rolü ayrıca tasarlanacaktır.

Temel döngü: siyasi karar → ekonomik sonuçlar → askerî imkânlar → savaş → nüfuzun, kaynakların ve karakterlerin konumlarının değişmesi.

## Kullanım ilkeleri

- Büyük bir mekanik eklerken hangi referanstan yararlanıldığını ve diğer sistemlerle nasıl ilişki kurduğunu kaydedin.
- Tasarım ilkelerinden ve oyun hissinden esinlenin; bu oyunların adlarını, metinlerini, kodlarını, haritalarını, görsellerini ve seslerini kopyalamayın.
- Tarihsel gerçekleri, oyun için yapılan basitleştirmeleri ve gelecekte eklenecek özellikleri birbirinden ayırın.
- İlk tarayıcı taslağı, oyun döngüsünü ve arayüzü sınar. Dört referanstaki sistemlerin tamamının uygulanmış olduğu anlamına gelmez.
- Tasarım çakışmalarında yeni kullanıcı yönünü uygulayın: harita ana çalışma alanı, siyasette somut güçler ve kişiler, savaşta alay kontrolü, ekonomide açık neden-sonuç anlatımı.

## İlk senaryonun tarihsel kaynağı

Genel Meclis (États généraux), 5 Mayıs 1789'da Versay'da açıldı. Aşağıdaki resmî kaynak `qwen-web scrape` ile okundu:

https://en.chateauversailles.fr/discover/history/key-dates/summoning-estates-general-1789

Taslak harita, büyük ölçekli oyun bölgelerinden oluşur; tarih atlası değildir. Kaynak miktarları ve senaryodaki çatışmalar oyun için kurgulanmıştır.
