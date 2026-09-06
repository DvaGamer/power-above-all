# İlk bağımsız siyasi girişim

6 Eylül2026; yalnız tasarım, uygulama izni değildir. Kişiler ve olaylar oyun kurgusudur; tarihsel olay veya kullanıcının seçtiği nihai rol sayılmaz. Kaynaklar mevcut `CampaignCore`, `CampaignRoles`, `CampaignPatronTrust`, `CampaignVictoryDecisions` ve `Localization/core.json` gündemleridir. Test, probe, Unity veya oyuncu çalıştırılmadı.

Bugün Morel temsilcilerin karar etkisini, Lefèvre düzenli ekmeği, Valcourt saray yetkilerini, Dumas ikmal ve zafer nüfuzunu istiyor. Kaynak sonuçları ve güven kapıları çalışıyor; fakat bu kişiler çoğunlukla oyuncunun düğmesini bekliyor. İlk girişimin farkı şu olmalı: NPC gerekçesini açıklasın, hazırlığını başlatsın ve oyuncu cevap vermese de ilan ettiği işi yapsın. Girişim yeni zorunlu dilekçe olmamalı.

## Üç farklı küçük konsept

**A — Morel'in tahsilat boykotu.** Başarılı olağanüstü vergi bir bölgeyi gerçek65 direnç eşiğine çıkarırsa Morel temsilcileri örgütlemeye başlar. Vergi önizlemesi bunu önceden söyler; iki hesap sonra bölge hâlâ direniyorsa Morel iki hesaplık olağan gelir boykotu ilan eder. Ekonomi gerçek bölge katkısını kaybeder, kişisel ilişkide açık bir siyasi kopuş olur. Oyuncu mevcut ekmek/vergi tatiliyle direnci azaltabilir veya temsilcilerin denetimini kişisel yetki bedeliyle kabul edebilir. Süre sonunda boykot kendiliğinden biter; normal komutlar kilitlenmez. İdari muhalefeti somutlaştırır, fakat yeni vergi tatiliyle aynı gelir istisnasını paylaşır ve eski anlaşma bozma bedeline ikinci ceza gibi okunabilir.

**B — Dumas'nın yiyecek toplama emri.** Gerçek açlık hesabından sonra komutan, gelecek hesapta açık sürerse askerlerinin bulunduğu yerde yiyecek toplayacağını bildirir. Hazinenin parası otomatik olarak gıdaya dönüşmez; ihtiyaç mevcut üretim/tüketim hesabından çıkar. Oyuncu açığı kapatabilir veya açık emirle yerel toplama işini yasaklayabilir. Cevap yoksa Dumas ilan ettiği işi yapar: orduya sınırlı yiyecek, bölgeye gerilim ve oyuncunun kişisel yetkisine siyasi bedel. Kendi askerlerini koruyan fakat yönetimin yerel planını bozan bir iradedir. **Önerim B.**

**C — Lefèvre'nin bağımsız mahalle iaşesi.** Gerçek başarısız Paris yardımı ve düşük şehir desteği sonrasında kent temsilcisi bir sonraki hesapta yerel üretimi mahallelere ayıracağını duyurur. İki hesap boyunca Paris sakinleşir, fakat ülke stokuna gelen Paris katkısı azalır; oyuncu şehrin yardım üzerindeki bağımsız etkisini görür. Düzenli yardımı gerçekten yeniden ödemek veya açık bir yerel anlaşma yapmak çıkıştır. Bu, askerî el koymadan farklı bir sivil dayanışma hareketidir. Ancak mevcut tek ulusal gıda havuzundan ayrı yerel dağıtım hesabı gerektirir; ilk dilim için B'den daha fazla ekonomik kavram ekler.

## B'nin sınırları ve açık fiyatı

Tetikleyici yalnız **gerçekleşmiş açlık**: başarılı haftada `Food + Forecast.NetFood < 0`, yaşayan ordu ve dolmuş girişim aralığı. “Gıda0” tek başına yeterli değildir; üretim tüketimi karşılayabilir. Maaş veya teçhizat açığı gıda girişimini açmaz. İlk açlığın mevcut kayıp, ikmal, moral ve sadakat sonuçları aynen kalır; NPC aynı anda ikinci ceza uygulamaz.

Hesap sonunda Dumas bir sonraki başarılı haftayı tarih olarak ilan eder. Normal Forecast yaklaşan açlığı zaten önceden göstermelidir; yeni haber bir tam planlama aralığı bırakır. Önerilen aralık dört başarılı haftadır. Veto veya şartların düzelmesi bu aralığı sıfırlamaz; aynı haftada hazırlık açıp kapatarak kazanç üretilemez.

Vade öncesinde üç gerçek yol vardır:

1. **İkmali düzelt.** Sonraki hesabın gıda açığını ortadan kaldır. Paris yardımı açıksa kapatmak tüketimi20 azaltır, fakat mevcut şehir/Paris/güç tepkisi de gelir. Askerî rolde erişilebilir yardım40 gıda getirebilir; diğer rollere bu yetki uydurulmaz. Vergi tatilinin sakinleştirmesi üretimi bazen artırır; yalnız güncel hesap yeterli diyorsa çözüm sayılır. Vergi altın verir, gıda satın almış sayılmaz.
2. **Yerel toplamaya karışma.** Hâlâ açık varsa Dumas en fazla40 gıda toplar; ihtiyaçtan fazlası verilmez. Kamp bölgesinde huzursuzluk+8, elit bağlılığı−6; Dumas hırsı+3 olur. Siyasi bedel mevcut tanıma mantığıyla4 Güç, sadakat hırsa en az eşitse0; otomatik olayın gerçek kaybı kalan güçle sınırlıdır. Bu miktarlar çalışma önerisidir, onaylanmış yeni denge değildir. Yetkinlik, taktik emirler ve kazanılmış sadakatin değeri değişmez.
3. **Toplamayı açıkça yasakla.** Tek seferlik emir hazırlığı kapatır, Dumas ilişkisini4 azaltır. Yiyecek verilmez; açlık gerçekten sürerse normal haftanın sonucu uygulanır. Yeni güç eşiği veya ödeme yoktur. İlişki0'a inse de normal ordu emirleri ve eski patron telafi yolu açık kalır. Veto, kaynak açığını gizleyen bedelsiz çözüm değildir.

Toplama, **yerel huzursuzluk artışının üretim etkisinden sonraki** gerçek açığa göre hesaplanmalı: `min(40, max(0, −(Food + projectedNetFood)))`. Aksi halde tam açık kadar yiyecek verip üretimi düşürerek beklenmedik açlık yaratılır.40 yetmiyorsa haber ve hafta önizlemesi kalan açlığı açıkça göstermelidir. Dumas'nın yardımı zafer veya kıtlıktan kesin kurtuluş sözü değildir.

Girişim ordunun mevcut kampını izler; geçmişte seçilmiş uzaktaki bölgeden görünmez taşıma yapmaz. Haber bunu baştan söyler, hareket önizlemesi yeni hedefi günceller. Yerel gerilim başka bölgede devam eden dört haftalık uzlaşmayı otomatik bozmaz; yalnız o uzlaşmanın mevcut vergi ihlali onu bozar. Sonradan65'e çıkan bölgenin gerçek direnci, üretimi ve kontrolü normal kurallarla yaşar.

## Sıra ve en küçük kalıcı durum

İki düz tarih yeterli adaydır: `DumasForageDueWeek` ve `DumasNextForageWeek`;0 vade yokluğu, sonraki tarih bekleme süresidir. Aktör sabittir, bölge kampı izler; ayrı olay listesi gerekmez. Eski kayıtlarda açıkça boş başlar, eski günlüklerden girişim çıkarılmaz. Son takvim haftasının ötesine haber verilmez.

Hafta komutu önce mevcut dilekçe, vadesi gelmiş rol sözü ve takvim retlerini denetler. Ret varsa NPC saati ve bütün state değişmez. Başarılı komut mevcut zafer teklifini kapatır; zamanı gelen NPC işi ardından olağan ekonomi hesaplanmadan uygulanır. Açık kapanmışsa veya ordu kalmamışsa iş ödülsüz iptal olur. Olağan hafta ve dördüncü vergi tatili hesabı aynı sırayla tamamlanır; yeni hazırlık yalnız bu gerçek hesabın açlık sonucundan doğar.

İkinci haftanın eski dilekçesi ve sonra patron borcu mevcut önceliği korur. NPC haberi konseyde tek okunur satır/portre işaretidir; kendiliğinden modal açmaz, haftayı kilitlemez. Zafer kararı ve NPC veto belgesi isteğe bağlıdır. Bekleyen eski sözün bölgesi, miktarı ve iki haftalık vadesi değişmez. Önizleme, beklenen NPC etkisiyle sonraki gerçek hesabı birlikte göstermelidir.

## On dakikalık örnek ve karar nedeni

Örnek açılıştan hemen sonrası değil, büyütülmüş ordunun ikmali sıkışan bir kampanyadır. Önceki saf Core denemesinde Paris yardımı ve düzenli asker alma gerçek sekizinci hafta açlığı üretmişti; bu yeni NPC'nin oyuncuda denenmiş kanıtı değildir. Oyuncu ilk dakikalarda açlık raporunu ve Dumas'nın gelecek hafta ilanını okur. Sonra haritada toplamanın hangi bölgede gerilim yaratacağını,40 üst sınırının açığı kapatıp kapatmadığını görür.

Bir yol yardımı kapatarak gıda açığını düzeltir; şehir siyasetinde bedel öder, Dumas'nın müdahalesi gerçekleşmez. Diğer yol şehir yardımını koruyup komutanı serbest bırakır: asker daha iyi beslenir, yerel uzlaşma kırılganlaşır ve komutan kendi inisiyatifiyle görünürlük kazanır. Oyuncu açık veto da verebilir, fakat sonraki açlık sonucu silinmez. Son dakikalarda gerçek hafta ve ardından yürüyüş bu tercihi gösterir. Bu nedenle NPC yalnız yeni ceza dağıtmaz; ülkeyi kurtarabilen fakat oyuncunun başka planını zorlayan bir çıkarı vardır.
