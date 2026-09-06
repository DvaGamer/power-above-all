# Ara gece raporu — kullanıcı döndü, geliştirme sürüyor

**Kesit: 6 Eylül 2026, 06:10 UTC / 09:10 İstanbul.** Kullanıcı yaklaşık 8 saat 47 dakika sonra döndü ve normal Windows oyunu kendisine açıldı. On saatlik çalışma yetkisi 07:22:03 UTC'ye kadar sürüyor. Ortaya çıkan ürün oynanabilir bir prototiptir; tamamlanmış bir grand strategy oyunu değildir.

## Çalışan sürüm ve doğrulama

[PLAY_GAME.cmd](PLAY_GAME.cmd) son doğrulanmış Windows build'ini açar. [Son tam doğrulama](output/verify/regional-reform-final-20260906-060546-199-b6af33b0/REPORT.md) **GREEN**: 496/496 Unity testi, yeni build, 18 PNG, 72 durum kontrolü, 14 kampanya JSON'u ve 10 tarayıcı testi. Build manifesti 141 dosyayı kapsar. 25 yerelleştirme kaynağında 692 anahtarın RU/TR karşılığı doğrulandı.

Runtime SHA256: `cb2419d89a960b4a30940748cc9cd6a266b13072cf74ea77c36493f90a70958a`.

## Oyunda artık bulunanlar

- **Rol ve kişisel iktidar:** kampanya öncesi taç, meclis veya ordu rolü; farklı hamilerden yardım ve vadeli karşılıklar. Sözün bölgesi, tarihi ve bedeli korunur. Tekrarlanan temerrüt güveni tüketir; siyasi telafiyle yeniden ilişki kurulabilir.
- **Ülke yönetimi:** vergi, ekmek, Paris yardımı ve dört gerçek hesaplık bölgesel vergi tatili birbirini etkiler. Ekonomi belgesi gelir, üretim, ordu gideri ve halk desteğinin gerçek sonraki hesaba etkisini açıklar. Dilekçe veya vadesi gelmiş söz haftayı engelliyorsa uygulanmış bir gelecek gösterilmez.
- **Bağımsız siyasi baskı:** Dumas gerçek açlıktan sonra gelecek hafta için erzak toplama girişimi ilan eder. Oyuncu veto edebilir veya ikmali düzeltebilir; toplama, yerel üretim kaybından sonraki ihtiyacı gerçekten karşılayabiliyorsa uygulanır. Yerel huzursuzluk ve generalin hırsı da bedelin parçasıdır.
- **Ordu politikası:** bütçe hedefiyle iki başarılı haftada en çok 200 fazla asker rezerve döner. Ayrılış haftası eski mevcudun bütçesini öder. Sıfır ordu ve yeniden normal alım yolu çalışır. Dumas'ya verilen subay hakkı, aynı kamptaki normal alımdan sonra haftada bir ücretli ek grup sağlar; geri alma bedeli yaşayan askere bağlıdır.
- **Kalıcı bölgesel reform:** erzak veya ticaret yönü için 120 livre ve 4 kişisel güç ödenir. Bölgenin gerçek hafta sonu koşulları uygunsa hazırlık ilerler; dört uygun hesaptan sonraki bütçe değişir. Proje başka bölge seçilince taşınmaz. İptal ek kaynak istemez, sponsor ilişkisini azaltır; yeni proje yeni ödeme ve hazırlık ister.
- **Savaş ve zaferin siyaseti:** seçim, çoklu seçim, hareket, hat/kol/dörtgen, ateş izni, elle salvo ve duraklatma ortak emir yolunu kullanır. Doğal zafer sonrası yetki verme veya askerlere prim ödeme seçenekleri gerçek fiyatlarla uygulanır. Esc teklifi yalnız gizler; konseyden yeniden açılabilir.
- **Kayıtlar:** arşiv v8 yeni reformu saklar; v1–v7 göçleri mevcut söz, tatil, zafer, Dumas, ordu hedefi ve subay hakkını korur. Bozuk veya eksik zorunlu alanlar sessizce yeni kampanyaya çevrilmez.

## Görsel sonuç

Krem kâğıt, koyu mürekkep, adaçayı ve mercan renkleri; resimli kurgu kişiler, on iki şehir silueti ve guaj arazi aynı görsel dilde birleşti. Atlantik, Kanal ve Akdeniz daha okunur tonlara ayrıldı. Bahçedeki ağaç siluetleri sadeleştirildi; duraklatılmış karşılaştırmalar simülasyonun değişmediğini gösterdi. Bölge emirleri ve nedenleri tek kaydırılan belgede erişilebilir; RU/TR giriş, yardım ve konsey metinleri gerçek girdilerle incelendi.

## Önemli gerçek kanıtlar

| İnceleme | Gözlenen sonuç |
| --- | --- |
| [Reform ekonomisi](Unity/WorkNotes/regional-reform-runtime-audit.md) | Her haftanın önceki durumundan yapılan beş hesap gerçek stoklarla eşleşti. İlk etkin bütçede aynı koşullarda vergi −6, üretim +4. İptal kaynakları korudu; yeniden imza tam 120/4 ödedi. Beş kayıt/yükleme çifti ham olarak eşit. |
| [Halk desteği](Unity/WorkNotes/public-mood-review.md) | Gerçek sekiz haftada 35→37→40 ve 58→61; ikinci haftada dilekçe açıkken reddedilen hafta bütün kampanyayı korudu. |
| [Bölgesel kuvvetlerle doğal savaş](output/verify/resistance-natural-victory-20260906-045323-372-ede571f9/REPORT.md) | 1200'e karşı 1114, doğal zafer, 196 kayıp ve 1004 sağ kalan. Oyuncunun ordusunu 1600'e çıkarmak bölgesel düşmanı kendiliğinden büyütmez. Bu ek koşu PARTIAL/native0'dır. |
| [Gerçek çoklu seçim ve salvo](Unity/WorkNotes/native-volley-input.md) | Shift ile iki piyade seçildi; topçuya gerçek düğme tıklaması mühimmatı 11→10 indirdi. Space sonrası yeniden doldurma ve duraklatılmış kanıtlar korundu. |
| [Gerçek subay kararları](Unity/WorkNotes/native-officer-commission-input.md) | Fareyle hak verme, ek alım ve geri alma: asker 1400→1600, hazine 720→600→466, sadakat 60→61; kayıt/yükleme eşit. |
| [Gerçek yardım ve konsey okuması](Unity/WorkNotes/native-copy-review.md) | RU/TR yardım ve konsey girdilerinden önce, sonra ve yükleme sonrası üç kampanya JSON'u aynı; native exit0, süre aşımı yok. |

**Native reform incelemesi henüz yapılmadı.** [Üç pencereli senaryo](tools/native-reform.script) ve [girdi yöntemi](Unity/WorkNotes/native-reform-input.md) hazırdır; taslak seçimi, tek Begin ve başka bölge seçiliyken asıl projeyi End ile kapatma gerçek fareyle ayrıca doğrulanacaktır. Son tam kapıdaki reform komutları bu native kanıtın yerine geçmez.

## Dürüst sınırlar

Kampanya sonu, görevden düşme sonrası devam, geniş diplomasi ve alternatif tarih yolları [VISION.md](VISION.md) içinde geliştirme alanıdır. Tek NPC girişimi tam bağımsız siyaset simülasyonu değildir. Mevcut sayılar oyun dengesi içindir; tarihsel istatistik veya tamamlanmış denge iddiası taşımaz.

Her PNG zafer veya görsel kabul değildir. Ayrı konuşlanma incelemeleri geri çekilmeyle tamamlandı. Yeniden kullanılan build'lerin PARTIAL sonuçları yeni tam GREEN sayılmaz. Önceki opak duman ve eksik Unity importu gibi otomatik kontrolden geçmiş görsel kusurlar reddedilip düzeltildi; eski RED ve diğer kanıtlar `output` içinde korunur. Dumanın erken okunurluğu, yoğun etiket yerleşimi ve sesin dinlenerek kalite kabulü hâlâ geliştirme alanıdır.

İncelemeler ayrı kampanya dizinlerini kullandı. Şimdi açık normal kullanıcı oturumu inceleme süreci değildir. Ayrıntılı tarihçe [NOTES.md](NOTES.md), devam noktası [SESSION_PROGRESS.md](SESSION_PROGRESS.md) içindedir.
