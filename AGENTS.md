# Power Above All — proje çalışma ilkeleri

Never implement the first obvious solution. For major features, generate 3 materially different concepts internally and implement the one that best reinforces Power Above All's identity.

## Özgünlük ve güncel teknik karar

- Kullanıcı doğrudan Unity'ye geçilmesini istedi. Yeni oyun geliştirmesi Unity projesinde yapılır; tarayıcı sürümü referans prototiptir.
- Oyun ilk Unity sürümünden itibaren Rusça ve Türkçe olmalıdır. Görünen metinler çeviri tablolarından gelir; dil tercihi kaydedilir. Depo belgeleri Türkçedir.
- Büyük özellik veya görsel için Power Above All'a özgü fikri belirleyin. Referans oyunlardan yalnızca ilkeleri alın; hazır strateji UI-kit düzenlerini, işlevsiz süsleri veya kopya mekanikleri doğrudan uygulamayın.
- Siyasi kişiler farklı seslere, çıkar ve gündemlere sahip olmalıdır; ciddi olayların sonuçları seçimin ardından yaşamaya devam etmelidir.
- Savaş yarı stilize tarihî diorama hissi taşır. Karar becerisi; konum, aldatma, hazırlık, tempo, moral, yedekler ve gerektiğinde plan değişimiyle ortaya çıkar. Sayı üstünlüğü tek başına sonuç belirlemez.
- Büyük bir ekleme için ayırt edici kimlik, sistemler arası etkileşim ve oyuncunun anlatabileceği sonuçları kontrol edin. İlk akla gelen şablon çözüm gibi görünüyorsa tasarımı yeniden ele alın.

## Ana kural: neden-sonuç zincirleri

### Etkin aşama: 0.2 Visual & Feel Polish Pass

5 Eylül 2026: Kullanıcı bütün istekleri plana kaydedip durmamızı istedi. Geliştirme durduruldu; yeni talimat gelmeden devam etmeyin. Güncel tam kontrol listesi ve yeniden başlama noktası `POLISH_PLAN.md`; olay sistemi açıkça polish sonrasına bırakıldı (`EVENT_DIRECTION.md`).

Yeni mekanik eklemeyin. Mevcut harita → emir → hafta → olay → yürüyüş → savaş → sonuç döngüsünü iyileştirin. Ekonomi kurallarını koruyun; nedenleri daha açık gösterin. Harita, tipografi, belge panelleri, mevcut birliklerin hareket/ateş/geri çekilme hissi, sakin animasyonlar ve sefer-savaş geçişi önceliklidir. Önceki genişletilmiş sistem hedefleri gelecek iş listesidir. Unity kararı geçerlidir; aktarımı yeni sistem eklemek için gerekçe olarak kullanmayın.

Yeni bir mekanik en az bir başka ana sistemle etkileşmelidir. Amaç mekanik sayısını artırmak değil, oyuncunun geçmiş kararının bugünkü sonucunu anlayabilmesidir:

**Siyasi karar → ekonomik sonuç → bölgesel sonuç → askerî imkân/sorun → savaş → siyasi güç.**

Önemli karar için yararlananı, kaybedeni, sadakati artanı, tehlikeli hâle geleni ve gelecekteki yükümlülüğü açıklayın. Önemli göstergeler şimdi / neden / sonraki dönem sorularını yanıtlasın.

## Tasarım kaynakları

- Güncel kapsam ve seçimler: `ROADMAP.md`, `STATUS.md`.
- v0.2 hedefi: `DESIGN_V0.2.md`.
- Görsel kurallar: `ART_DIRECTION.md`.
- Referansların görevleri: `REFERENCES.md`.
- Ortak çalışma: `CONTRIBUTING.md`.

Kullanıcının yeni kararları bu belgelerdeki eski varsayımların önüne geçer. Onay bekleyen planları tamamlanmış veya kesinleşmiş özellikler gibi anlatmayın.

## Sabit yön

- Fransa, 5 Mayıs 1789; kişisel siyasi hayatta kalma ve devlet üzerindeki kontrol.
- Harita ana çalışma alanıdır; belge panelleri bağlamı korur.
- Kraliyet çalışma odası, tarihî atlas, askerî raporlar; mevcut koyu yeşil/kâğıt/mat altın/serif kimlik.
- Oyuncu tekil askerleri değil alayları yönetir; moral ve düzen önemlidir.
- Ekonomi, bölgeler, ikmal ve savaş sonuçları siyasi aktörleri etkiler.
- Tarihî gerçek, kurgu olay ve basitleştirilmiş oyun verisi açıkça ayrılır.

## Belgeler ve doğrulama

Kullanıcı bağımsız işler için mümkün olan en yüksek düzeyde alt ajan kullanımını açıkça istedi. Mevcut eşzamanlı kapasiteyi somut, ayrı dosya/sorumluluk kapsamlarıyla değerlendirin; ana ajan entegrasyon ve doğrulamayı sürdürsün. Boşta kalan ajanlara bağımsız inceleme veya sıradaki uygun işi verin. Aynı dosyada çakışan eşzamanlı değişikliklerden kaçının.

Ekip belgelerini ve yeni açıklamaları Türkçe yazın. Mevcut oyun arayüzünün dilini veya kod API'lerini kapsam dışı olarak değiştirmeyin. Çalışan prototipi koruyun; ilgili testleri çalıştırın, sınırları ve tamamlanan işleri kaydedin. Hedeflenen tasarım ile uygulanmış mekanikleri ayırın.
