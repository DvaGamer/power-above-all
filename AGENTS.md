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

6 Eylül 2026 güncel talimat: Kullanıcı «Я ухожу на 10 часов поспать. сиди и делай игру все это время» diyerek on saatlik özerk geliştirmeyi başlattı. Önceki durdurma kararı geçersizdir. Çalışma aralığı 5 Eylül 21:22:03 UTC–6 Eylül 07:22:03 UTC (İstanbul 00:22–10:22); kullanıcı yeni yön verirse ona uyulur. Önce mevcut polish temeli güvenli test, derleme ve gerçek görüntülerle doğrulanır; sonra `VISION.md` içindeki kesinleşmiş hedeflere bağlı küçük oynanabilir geliştirmeler ele alınır. Açık vizyon sorularına verilen çalışma varsayımları kullanıcı kararı gibi sunulmaz. Güncel kayıtlar `NOTES.md`, `NIGHT_LOG.md`, `NIGHT_QUEUE.md`; sabah teslimi `NIGHT_REPORT.md`.

İlk doğrulama ve polish aşamasında yeni mekanik eklemeyin. Mevcut harita → emir → hafta → olay → yürüyüş → savaş → sonuç döngüsünü iyileştirin. Ekonomi kurallarını koruyun; nedenleri daha açık gösterin. Harita, tipografi, belge panelleri, mevcut birliklerin hareket/ateş/geri çekilme hissi, sakin animasyonlar ve sefer-savaş geçişi önceliklidir. Temel doğrulandıktan sonraki olası sistem işleri yeni kullanıcı vizyonuna göre küçük, bağlantılı ve ayrıca doğrulanabilir kapsamlarla planlanır; geniş hedeflerin tamamı bitmiş sayılmaz.

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
