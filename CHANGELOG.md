# Değişiklik günlüğü

## Geliştirme altında — Unreleased

### 6 Eylül 2026 — gece kontrol noktaları

- Gerçek zafer sonrası isteğe bağlı siyasi karar: Dumas'nın girişimini tanıyarak orduyu toparlamak veya sağ kalan askerlere hükümet adına prim ödemek. Sadakat ve hırs siyasi bedeli belirler; eski savaş sonucu yalnız bir kez uygulanır. Yan yana RU/TR belge, Esc ile haritaya dönüş ve konseyden yeniden açma eklendi.
- Arşiv v4 açık zafer teklifini saklar ve v1–v3 kayıtlarını taşır; v3 bölgesel anlaşmaları korunur. Faydasız prim reddedilir; uzun süreli sadakatle kazanılan ücretsiz komutan desteği korunur.
- Duman ayrı alfa shader'ıyla yumuşak ve saydam çizilir. Opak dörtgen hatası giderildi; erken duman hâlâ hafiftir. Dumas portresinin komşu resimden gelen küçük parçası, kaynak PNG değiştirilmeden gösterim sınırından çıkarıldı.
- Güncel tam kapı: 176 Unity testi, 21 PNG, 38 durum kontrolü, 21 JSON ve 10 tarayıcı testi. İki savaş dünyası art arda kuruldu; doğal zafer ve prim kayıtları önceki sonuçlarla aynı kaldı. Gerçek Esc/TR/RU/fare primi ayrıca native exit0 ile sınandı; ilk Start çağrısının açıklamasız ret sınırı NIGHT_REPORT içinde kayıtlıdır.

### Önceki gece kontrol noktaları

- Üç başlangıç rolü, ayrıcalık karşılığında süreli sözler ve hami güveni: temerrüt yeni yardımı kapatır, siyasi telafi erişimi geri açabilir; eski borç/bölgesel sonuçlar korunur.
- Bölgesel vergi tatili: dört gerçek gelir hesabında bir ilin vergisi bırakılır, direnç azalabilir ve orduya barışçıl geçiş açılabilir. Olağanüstü vergi sözleşmeyi bozar; v3arşiv v1/v2 kayıtlarını açıkça taşır.
- Guaj atlas ve tepe, 12 şehir silueti, 4 kurgusal portre ve doğal yol–sığ geçit bağlantısı; açık mat kâğıt ile koyu mürekkep paleti. İşe alımın haftalık ek gideri ve RU/TR yardım metni okunur.
- Ortak taktik komutlar, aynı adımda karşılıklı salvo ve cephanesiz yakın temas. Gerçek oyuncuda125.803s doğal zafer/196kayıp/+24teçhizat ve tek sefer aktarımı doğrulandı.
- Son tam kapı128Unity/10tarayıcıPASS ve taze141dosyalık Windowsmanifesti. Kullanıcı kaydı korunur; launcher en son bütün kapıları geçen build'i açar. Ayrıntı ve yeniden kullanılan build/nativeinput incelemelerinin sınırları NIGHT_REPORT içinde.

### Önceki geliştirme kayıtları

- Unity 6000.3.23f1 kuruldu: `C:/Users/USER/Tools/Unity/6000.3.23f1/Editor/Unity.exe`. İlk başlatmadaki lisans hatasından sonra kullanıcı lisansı etkinleştirdi; Editor yeniden açıldı ve paket/proje içe aktarımı başladı.
- `OPEN_UNITY.cmd` proje başlatıcısı ve `Unity/Assets/Scenes/Main.unity` başlangıç sahnesi eklendi. Tarayıcı 0.1 referans olarak korunuyor.
- Unity kaynaklarına atlas, belge panelleri, taktik savaş, kayıt bağlantısı ve Rusça/Türkçe yerelleştirme eklendi.
- Mevcut sunuma yumuşak ordu hareketi, bölge seçimi ve emir vurguları, sayı geçişleri, devre dışı emir gerekçeleri ve yeni günlük kaydı vurgusu eklendi. Bölge emirleri panelin üstüne alındı.
- Tarayıcıdaki mevcut ikinci hafta ekmek dilekçesi Unity'de üç seçenekli belge sunumuna aktarıldı.
- On prosedürel foley taslağı eklendi; tamamlanmış ses varlığı olarak değerlendirilmez.
- Saf C# çekirdeği bağımsız derleyiciyle kontrol edildi. 14 NUnit editör testi kaynakta bulunur; Unity Test Runner içinde henüz çalıştırılmadı.
- Editor'de `Main.unity` açıldı ve Play modunda sefer haritası çizildi. Bu denemede görülen TacticalBattle yerel nesne başlatma hatası kaynakta düzeltildi; düzeltme sonrası yeniden derleme ve açılış doğrulanmadı.
- Tam Unity oyun akışı, Rusça/Türkçe görsel inceleme ve Windows oyun derlemesi tamamlanmadı.
- Kullanıcı geliştirmeyi durdurup tüm kalan gereksinimlerin POLISH_PLAN.md içinde korunmasını istedi; uygulama çalışması durduruldu.
- Sonraki aşama **0.2 Visual & Feel Polish Pass** olarak daraltıldı: yeni mekanik veya ekonomi genişletmesi yok. Kapsamlı tasarım gelecekteki işler olarak korundu.
- `cb200f1` çalışma anlık görüntüsü GitHub'a yüklendi. Son yerel değişikliklerin ikinci yüklemesi mevcut kontrolün ardından yapılacak; henüz tamamlanmış olarak kaydedilmedi.
- Depo, kullanıcının açık onayıyla Public olarak kalıyor.

- Beş görsel/arayüz referansı ART_DIRECTION.md içinde tanımlandı.
- Harita bağlamı, somut siyasi güçler, karakter olayları, alay kontrolü ve ekonominin neden-sonuç anlatımı hedef olarak kaydedildi.
- Önceki bağlantılı v0.2 planı kaydedildi; güncel görsel iyileştirme aşamasına dahil olmayan ayrıntılar gelecekteki iş listesine taşındı.
- Siyasi kararların ekonomi, bölgeler, ikmal, savaş ve kişisel iktidar üzerindeki sonuç zinciri projenin ana ilkesi oldu; AGENTS.md ve DESIGN_V0.2.md içine kaydedildi.

- Depo belgeleri ve katkı sürecinin dili Türkçe olarak belirlendi; belgeler Türkçeye çevrildi.
- Proje sahibinin arkadaşlarının da katkı yapacağı ekip çalışması düzenine geçildi. Önceki yalnızca yapay zekâyla geliştirme varsayımı artık geçerli değil.
- Tarayıcı 0.1 arayüzü Rusça kaldı; yeni Unity sürümünde Rusça ve Türkçe birlikte desteklenecek.

## 0.1.0 — ilk taslak

- Power Above All adı ve seferin başlangıcı olarak 5 Mayıs 1789 seçildi.
- Fransa'nın 12 şematik bölgeden oluşan haritası ve iki harita katmanı eklendi.
- Vergiler, tahıl, asker toplama, ordu hareketi, huzursuzluk ve haftalık hesaplamalar uygulandı.
- Üç siyasi destek göstergesi ve bir konsey olayı eklendi.
- Tüfekli askerler, yaylım ateşi, duraklatma ve geri çekilme içeren ayrı bir taktik savaş eklendi.
- Savaş sonucu, kayıpları ve ordunun konumunu sefere yalnızca bir kez uygular hâle getirildi.
- Kayıt sistemi, sekiz haftalık öğretici senaryo ve sonrasında serbest oyun eklendi.
- Simülasyon çekirdeği için otomatik testler ve tarayıcı kontrol senaryosu oluşturuldu.
- Proje sahibinin oyun referansları ve prototipin sınırları belgelendi.
