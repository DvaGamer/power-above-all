# Hedef ordu mevcudu: bağımsız kabul kenarları

6 Eylül 2026, checkpoint c329851 sonrası. Root `next-country-decision.md` içindeki A politikasını onayladı. Bu ilk inceleme yalnız plan ve mevcut Core sırası üzerindedir; yeni `CampaignArmyEstablishment` uygulaması henüz okunabilir değildi. Aşağıdakiler mevcut üründe bulunmuş beş hata olarak sunulmaz; yeni kodun bozabileceği, somut kaynak aritmetiğine bağlı kabul örnekleridir. Unity, test, probe, player ve derleyici çalıştırılmadı; Assets/tools değiştirilmedi.

## 1. Eski mevcutla hesap, sonraki dönemde tasarruf

1200 askerin1000 hedefine indiği vade hesabı hâlâ136 altın ordu gideri ve40 asker gıdası kullanır. Daha küçük1000 kişinin120 altın/34 gıdası ilk kez **sonraki** hesapta geçerlidir. Mevcut `NextWeek` ortak `BuildWeekProjection` sonucunu bir kez stoklara uygular; yeni aktarım bu hesabı yeniden yürütmemelidir.

Somut yanlış sonuç: vade haftasının mevcut yiyecek açığı1–6 iken gelecek6 gıda tasarrufunu şimdiki NetFood'a eklemek açlığı veya bekleyen Dumas toplamayı geçmişe dönük kaldırır. Aynı haftanın Dumas planı eski yaşayan birlik ihtiyacını kullanmalıdır. Hesap sonunda yeni duyuru değerlendirildiğinde ise aktarım sonrası kalan askerler kullanılmalıdır. Gerçek açlıktan doğan duyurunun **gelecek** tahmini küçülen ordu sayesinde sufficient olabilir; bu önceki açlık etkilerinin geri alınması değildir.

## 2. Savaş/açlık kayıpları sonrası yalnız gerçek kalanlar döner

1200 asker,1000 hedef ve vade haftasında gerçek%8 açlık kaybı: önce96 kişi kaybolur;1104 yaşayan askerden yalnız104 kişi ayrılır. Manpower+104, Troops1000; insan toplamı kayıptan sonra korunur. Dumas tepkisi sözleşmedeki tek gerçek partiye aittir;200 kişilik sahte transfer yoktur.1–199 kişilik son parti bir hata veya sıfıra yuvarlanacak miktar değildir.

Başka sınır: hazırlık sırasında gerçek muharebe1200 ordudan201 kayıp verdiyse999 kişi kalır.1000 hedefi için aktarım0, Manpower artışı0 ve ayrılan partiye ilişkin Dumas tepkisi0 olmalıdır. Eski hedefin üstündeki planlanmış200 kişiyi ayrı, kayıptan korunmuş rezerv gibi geri vermek asker yaratır. Buradaki201 sınır örneğidir; gerçek player muharebesinde201 kayıp ölçülmüş gibi sunulmaz.

## 3. Yalnız ilk parti değil, bütün dönüş için kapasite

Mevcut MaximumStock100000000. Manpower=MaximumStock−200, Troops1600, hedef1000 durumunda ilk200 kişi sığar ama planın600 kişilik toplam dönüşü sığmaz. Sözleşme karar anında tüm dönüş kapasitesini gerektiriyorsa kabul atomik olarak reddedilmelidir. Troops azaltıp Manpower'ı `Stock` ile tavanda kırpmak yaşayan insan kaybettirir.

Mevcut recruit200 kişiyi aynı Manpower havuzundan çıkarır; asker/insan gücü toplamını değiştirmez ama120 altın/20 yiyecek/15 malzeme ödetir. Politikayı kapatıp yeniden recruit etmek bu bedelleri iade etmemeli ve askerleri ücretsiz geri getirmemelidir. Bugünkü Core'da başka olağan Manpower ekleme yolu görünmedi; hayalî eşzamanlı sistemler için geniş bir altyapı önerilmedi.

## 4. Açık tarihin korunması ile yeni tarihin açılması ayrı

Week0 hedef1000/due2; Week1 mevcut recruit ile ordu büyürse due2 korunmalıdır. `currentWeek+2` değerini her recruit veya hedef değişiminde yeniden yazmak azaltmayı sürekli ertelemeye izin verir. Ters yönde hedef değiştirmek de işlemi erkene çekmemelidir. Gerçek hedefe ulaşılıp açık due kalmadıktan sonra yeni recruit hedefin üstüne çıkarıyorsa ancak o zaman yeni iki haftalık hazırlık gerekir.

Önizleme bu ayrımı açıkça göstermelidir: bugünkü ordu ve ödemeler gerçek mevcutta kalır, bir sonraki fiilî aktarım tarihi mevcut kuyruğun tarihi olur. Kaynak yetersizliğinden reddedilen recruit/yanlış bölge emri veya reddedilen politika değişikliği eski tarihi değiştiremez.

## 5. Yeni doğan dilekçe bitmiş hesabı geri alamaz

Politika Week0'da başlatılırsa ilk vade2; mevcut NextWeek aynı ikinci hesabın sonunda PendingPetition oluşturur. Başlangıç guard'ları zaten geçilmiş bu hesabın aktarımını, sonradan oluşan dilekçeye bakarak durdurmak yanlıştır. Dilekçe ve vadesi gelen patron sözü yalnız **sonraki** reddedilen hesapta bütün durumu/tarihleri korumalıdır. İkinci guard eklenerek due2 geçmişte bırakılmamalıdır.

Dört hesaplık anlaşmanın son vergi istisnası eski sırayla hesaplanır, sonra anlaşma tamamlanır. Ayrılacak insanlar rol sözünün ilk bölgesini,80/150 altın ya da40 yiyecek borcunu yeniden hesaplatmaz. Başarılı NextWeek eski zafer teklifini kapatır; yalnız politika hedefinin seçilmesi bugünkü Troops'u veya bugünkü zafer priminin fiyatını değiştirmemelidir.

## Aktarım sırasının incelenecek noktası

Mevcut sıra: giriş guard'ları → ortak ekonomi/NPC planı → mevcut NPC etkileri → eski stok/teçhizat hesabı ve tarih → eski açlık/maaş/teçhizat kayıpları → bölgesel sonuçlar/dilekçe/anlaşma bitişi → yeni Dumas duyurusu. Yeni parti en azından bütün mevcut asker kayıplarından sonra, yeni Dumas duyurusundan önce olmalıdır. Sıfır hedef ilk dilime alınırsa garnizonun o hesap mı sonraki hesap mı kalkacağı ayrıca kesinleştirilmelidir; yalnız pozitif hedefli dilime sıfır hedef davranışı varsayılmadı.

Somut kaynak freeze gelince bu noktalar gerçek API/Archive/UI ile yeniden karşılaştırılacak. Gerçek kampanya script'i ancak root'un API-only probe sayılarıyla hazırlanacak.

## İlk uygulama incelemesi

Root son sözleşmeyi netleştirdi: sıfır hedef onaylıdır. Vade haftasının eski bütçesi **ve bütün bölgesel etkileri** önce tamamlanır; sonra gerçek aktarım yapılır. Son garnizonun kaybı ancak sonraki hesapta etkili olur. Yeni Dumas duyurusu kalan Troops0 ise doğmaz. Önceki sıfır hedef belirsizliği kapanmıştır.

Yeni `CampaignArmyEstablishment.cs`, Core hooks ve Archive v6 salt okunur incelendi; bu anda diğer ajan test/probe yazıyordu, freeze veya çalışan test sonucu henüz yoktu.

- Gerçek aktarım `CompleteRegionalAccordAfterWeek` sonrasında ve `AnnounceDumasInitiativeAfterWeek` öncesinde. Açlık/maaş/teçhizat kayıpları, garnizon ve yeni dilekçe bundan önce işleniyor. Ekonomi planı yeniden hesaplanmıyor. Böylece due2 hesabında yeni doğan dilekçe aktarımı engellemiyor.
- Parti `min(200, max(0, yaşayanTroops−hedef))`; aynı sayı Troops'tan çıkıp Manpower'a ekleniyor, Stock ile sessiz kesme yok. Sıfır partide Dumas ilişkisi veya reduced günlüğü yok. İmzalama guard'ı ve validasyon bütün fazla kuvvetin rezerv kapasitesine sığmasını gerektiriyor.
- Recruit sonrası, gerçek barışçıl yürüyüş kaybı sonrası ve battle casualties sonrası Refresh çağrısı var. Hedef üstünde açık due korunuyor; hedef altına düşünce temizleniyor. Tamamlanan parti kalan fazlalık için yeni iki haftayı kuruyor. Takvim sonunda tarih kurulamayan mevcut bütçe politikasının fazla ordusu due0 ile açıkça temsil edilebiliyor; yeni tamamlanamaz plan başvurusu reddediliyor.
- Mevcut politika/aynı hedef, yanlış politika/hedef, kapasite, dilekçe, dueMandate ve takvim retleri mutasyondan önce. Politika seçimi Troops veya PendingVictory'yi değiştirmiyor. Yalnız sonraki gerçek hafta eski zafer teklifini kapatıyor.
- V6 için üç typed zorunlu alanın ayrı okuması var; eski v1–v5 sürümleri yalnız boş/campaign, hedef0/due0 durumundan campaign'e göçüyor. Önceki accord/victory/Dumas eşikleri3/4/5 olarak bağımsız kalmış. V6 eksik/null ve gerçek eski arşiv geçişlerinin çalışma zamanı kabulünü root testleri doğrulamalı; kaynak incelemesi bu sonuçları olmuş saymaz.

Somut Core karşı örneği bulunmadı. UI'ya aktarılan önemli anlam sınırı: `NextBatchTroops`, `ManpowerAfterBatch`, `ArmyCostAfterBatch` bugün yaşayan asker ve bugünkü malzeme stoğu üzerinden **koşullu** karşılaştırmadır. Vade haftasının olası asker kaybını ve iki hesapta değişecek teçhizat stoğunu öngörmez. Açlık örneğinde nominal200 yerine gerçek104 dönebilir; bugünkü Supplies<120 ile sıfır asker gideri36 iken vade geldiğinde stok120'yi geçmiş olabilir. İki hafta sonrası kesin sayı gibi sunmak yanıltır. Root'a “bugünkü mevcut/stoklarla, yeni kayıp olmazsa” açıklaması önerildi; gerçek mevcut haftanın Forecast'i ayrı ve kesin ortak plandır.

## İlk gerçek player kapısının durum kabulü

`army-establishment-first-20260906-025643-395-97035b66` raporu, protokolü ve10 campaign JSON dosyası doğrudan okundu: **GREEN304 Unity/13PNG/71assert/10JSON/10browser/141 build dosyası**,41 saniye,152 komut. Runtime SHA256 `2bcfccd13a0cd562d068723e91515d4bd2ef0b70b4298a98534df9daf7829c57`. Root00/01 RU ve03 TR taslak karelerini görüp okuduğunu bildirdi; bu inceleme ajanı o görsel kabulü kendi görmüşlüğü olarak sunmaz.

- Raw SHA256 eşitlikleri01=02,04=05,07=08.0. hafta budget1000/due2 imzası1200Troops/2400Manpower ve stokları değiştirmemiş.1. hafta aynı insanlar,Gold911/Food362;2. hafta1000/2600,Gold979/Food362 ve PendingPetition=true. Dumas ilişkisi50→46, due0. Tek `log.establishment.reduced` Week2 args `[200,1000,4,1000]`.
- İki haftanın journal bütçesi sırasıyla `[1,207,136,2]` ve `[2,204,136,0]`; iki hesap da eski136 ordu giderini ödemiş. Yeni dilekçe gerçekleşen partiyi yutmamış. Reddedilen hafta ve yüklemelerde tam same kontrolü protokolde geçmiş.
- Negotiate sonrası recruit:Gold979→859,Food362→342,Supplies136→121,1000→1200Troops/2600→2400Manpower; yeni due4. Hedef900 değişikliğinde aynı stoklar ve due4 korunmuş. Campaign iptalinde askerler/rezerv aynı kalıp yalnız politika/target/due değişmiş; yeniden asker üretilmemiş.3. haftaGold939/Food341/Troops1200, due0; Dumas46 kalmış ve ikinci reduced kaydı yok.
- Gerçek kaynak verileri pure probe ile aynı. Assets/tools değiştirilmedi, yeni süreç veya test başlatılmadı. Sıfır hedef ve birleşik borç yolu bu ilk gate'in kapsamı değildir; ayrı koşuları bekler.
