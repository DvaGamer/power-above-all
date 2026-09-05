# Power Above All — çalışma notları

## Geçerli kararlar

- SON KAPSAM: Kullanıcı mevcut durumu hemen özel GitHub'a yüklemeyi ve ardından “0.2 Visual & Feel Polish Pass” istedi. Yeni mekanik yok; ekonomi değişmez. Mevcut döngünün harita/UI/savaş/geri bildirim/animasyon/olay/geçiş/metin kalitesi iyileştirilir. Unity kararı geçerli. Üç alt ajan bu sınıra yönlendirildi; genişleme tasarımı gelecek iş listesi oldu.

- Kullanıcı bundan sonra bağımsız görevlerde daima mümkün olduğunca çok subagent kullanımını istedi. Şu an4slotun tamamı kullanılıyor: rootentegrasyon+3Unitymodül. Ajan bitirince bağımsız test/gözdengeçirme/verifikasyona yönlendir.
- UnityHub3.21.1.65535 wingetUnity.UnityHub ilekuruldu. UnityEditor için6000.3.23f1 LTS seçildi: resmîsayfa26Aug2026 release,revision09d2ecc7fb28,Windowsinstaller https://download.unity3d.com/download_unity/09d2ecc7fb28/Windows64EditorInstaller/UnitySetup64-6000.3.23f1.exe . Unity6.3testframeworkresmîsayfasıcorepackage1.6line gösteriyor.

- EN GÜNCEL KARAR: Kullanıcı "строй сразу в юнити" diyerek Unity'yi seçti ve uygulamanın başlamasını istedi. Ardından ilk Unity sürümünün hem Rusça hem Türkçe olmasını belirtti. Eski motor seçimi bekliyor ve Rusça-arayüzü-koru varsayımları yeni Unity geliştirmesi için geçersizdir; tarayıcı0.1 geçmişdurum olarak kalır.
- Kullanıcı yeni combat/art yönü verdi: yarı stilize tarihî minyatür/diorama, doğalpalet, net siluetler; beceri APM değil karar. Moral, arazi, düzen, kanat, yedek, yorgunluk, cohesion ve zamanlama; hızlı riskli ve yavaş sabırlı yaklaşımlar; yerelperdeğişimler ve geri dönüşler. GenelDPS/garanti sayızaferi yok.
- Kullanıcının özgünlük kuralı AGENTS.md içine aynen eklendi: Never implement the first obvious solution. For major features, generate 3 materially different concepts internally and implement the one that best reinforces Power Above All's identity.
- Unity işi başlamış durumda: simulationaltajan CampaignCore.cs+core.json+UnityNUnitTests; interfacealtajan CampaignMap.cs+CabinetHud.cs+cabinet.json; battlealtajan TacticalBattle.cs+battle.json. Root GameApp,Llocalization,editorbuildtools,projectsettings,kurulumdoğrulama veGit'i yapar. Unity proje klasörü Unity/.
- Unity/UnityHub standartProgramFiles,AppData vePATH üzerinde bulunamadı; yalnızCdrive var,191GB boş. winget1.29.290 var. ResmîUnity6 sayfası qwen-web scrape ileUnity6.3LTS varlığını doğruladı; editorinstall araştırılıyor.

- Son kullanıcı mesajı önceki üç ayrı yol seçimini geçersiz kılan kapsamlı Visual & Gameplay Target v0.2 sağladı. Yeni ana kural bağlantılı sonuç zinciridir; oyuncu ülkenin başarısından önce kendi siyasi hayatta kalması ve kontrolünü hedefler. Dört geliştirme birlikte ele alınır: güçler/karakterler, bölgeler/harita kipleri, alay savaşı/moral/düzenler, ekonomiyle bağlı ikmal.
- Güncel ROADMAP.md yedi küçük kontrol noktasıyla tek bir birleşik sefer önerir. Açık kalan seçimler yalnızca teknoloji (tarayıcı/Unity) ve kesin oyuncu kimliği (kurgusal konsey üyesi/XVI.Louis/Necker). Kullanıcıya yeni iki soru gönderildi; eski öncelik/soyut oyuncu rolü soruları artık geçerli değil. Henüz yeni oyun özelliği uygulanmadı.

- Proje sahibi sonraki aşamanın planını kendisiyle kararlaştırmak istiyor. ROADMAP.md taslaktır; son v0.2 mesajıyla dört sistemin birlikte geliştirilmesi ve kişisel iktidar hedefi belirlenmiştir. Teknoloji ve kesin oyuncu kimliği açık kalmıştır. Yeni oyun özellikleri bu kararlar beklenirken uygulanmadı.
- Yeni görsel yön ART_DIRECTION.md içine kaydedildi: EU V harita, Victoria 3 siyasi güçler, CK3 karakter/olay, Napoleon: Total War alay kontrolü, Anno 1800 ekonomik neden-sonuç okunurluğu. Devlet çalışma odası/atlas; koyu yeşil, kâğıt, mat altın, kırmızı ve serif korunur. Referans görsel dosyası eklenmedi; kaynak kullanıcı metnidir.
- Türkçe README/STATUS/NOTES/REFERENCES/CHANGELOG, CONTRIBUTING, Issue/PR şablonları, paket açıklaması ve Actions adları hazırlandı. GitHub depo açıklaması Türkçeye güncellendi. Henüz arkadaş daveti gönderilmedi veya dal koruması etkinleştirilmedi.
- Ekip taşınabilirliği için tests/browser-smoke.js ekran görüntüsü yolları göreli output/playwright/ yapısına geçirildi; aynı CLI ile göreli dosya yazımı ve JavaScript sözdizimi kontrol edildi. Çekirdek testleri tekrar10/10 geçti.11 Markdown dosyasının yerel bağlantıları ve Türkçe metin kontrolü geçti; README'de Rusça düğmelerin çeviri tablosu bilerek korundu.

- Proje sahibi İngilizce **Power Above All** adını seçti. Güncel proje dizini: `C:/Users/USER/projects/power-above-all`. Eski `france-1789` dizin adı artık kullanılmıyor.
- Dönem, 1789 Fransız Devrimi; önceki 1830 varsayımı iptal edildi. Fransa, haritanın ve içeriğin merkezinde.
- **Depo belgeleri ve katkı sürecinin dili Türkçe.** Oyun arayüzü şimdilik Rusça kalıyor; İngilizce oyun adı değişmiyor.
- Proje sahibinin arkadaşları geliştirmeye katılacak. Önceki “insanlardan oluşan bir geliştirme ekibi yok” varsayımı bu kararla geçersiz oldu.
- Proje sahibi alt ajanların kullanımına açıkça izin verdi. İlk taslakta arayüz (`styles.css`), simülasyon (`simulation.js` ve testler) ve savaş (`battle.js`) paralel geliştirildi; ana ajan `app.js`, `index.html`, belgeler, entegrasyon ve tarayıcı doğrulamasını üstlendi. Bu ilk görev paylaşımı, gelecekte dosya sahipliğini sınırlamaz.
- İlerleme `STATUS.md`, çalıştırılabilir `START.cmd` ve doğrulanmış aşamaların Git commit'leri üzerinden takip ediliyor.
- Kullanıcının dört kalıcı oyun referansı `REFERENCES.md` içinde 1789'a uyarlanarak kaydedildi: **Warcraft III**, **Total War**, **Europa Universalis V**, **Crusader Kings III**.

## Teknik temel ve çalıştırma

- İlk taslak, derleme aracı ve haricî çalışma zamanı bağımlılığı gerektirmeyen HTML/CSS/JavaScript uygulaması. Oyun döngüsünü sınayan bir tarayıcı prototipi; hazır bir Unity projesi değil. Simülasyon çekirdeği arayüzden ayrı.
- Unity standart kurulum klasöründe bulunamadı. Bu kontrol, makinenin tüm dizinlerinde Unity olmadığı anlamına gelmez.
- Windows Node `v24.18.0` ve npm/npx kullanılabiliyor. WSL ortamında `rg` ve `node` bulunmadığından dosya aramalarında `find`, JavaScript çalıştırırken Windows Node kullanıldı.
- Bu makinede kabuk komutları `bash -lc '...'` biçiminde çalıştırılmalı; dosyalar yalnızca `apply_patch` ile düzenlenmeli.
- `START.cmd`, `launch.cjs` dosyasını çağırır. Başlatıcı `127.0.0.1:1789` adresini kontrol eder, gerekirse gizli bir Node sunucusu başlatır ve tarayıcıyı açar. `launch.cjs --check` ve `START.cmd` başarıyla çalıştırıldı.
- Doğrulanmış ana erişim adresi: `http://127.0.0.1:1789`. `file://` kontrolü Playwright CLI politikası tarafından engellendi; engeli aşma girişiminde bulunulmadı. Yerel dosya üzerinden çalıştırma doğrulanmış sayılmamalı.
- Çekirdek testlerini bu makinede çalıştıran komut: `bash -lc '/mnt/c/Program\ Files/nodejs/node.exe --test C:/Users/USER/projects/power-above-all/tests/simulation.test.cjs'`.
- Tarayıcı doğrulaması için `C:/Users/USER/.codex/skills/playwright/SKILL.md` okundu. Playwright CLI, `cmd.exe /c npx --yes --package @playwright/cli playwright-cli` üzerinden kullanılabiliyor. CLI `run-code --filename` ile dosya kabul ediyor ve `async (page) => { ... }` biçiminde bir JavaScript işlevi bekliyor.
- Kullanıcının etkileşimde bulunabildiği görünür tarayıcı oturumu `power-above-all`; bu oturumun durumunu sıfırlamayın. Otomatik kontroller için ayrı `power-above-all-check` oturumu kullanıldı. Yeni oturumda sunucunun veya bu tarayıcı oturumlarının hâlâ açık olduğunu varsaymayın.

## GitHub ve doğrulama kanıtları

- Proje sahibi **özel (private)** GitHub deposunun oluşturulmasını ve projenin yüklenmesini açıkça onayladı. Yalnızca proje dosyaları bu kapsama girer; hesap dosyaları ve komşu klasörler girmez.
- Depo: https://github.com/DvaGamer/power-above-all . `gh repo view`, `isPrivate: true` ve `visibility: PRIVATE` değerlerini doğruladı.
- `origin`: `https://github.com/DvaGamer/power-above-all.git`; ana dal: `main`.
- İlk uygulama `7e4723d` commit'iyle GitHub'a yüklendi. GitHub Actions çalıştırması `33987434163`, sözdizimi kontrollerini ve 10 çekirdek testini başarıyla tamamladı. Bu kayıt, Türkçe belge değişikliklerinin de yüklenmiş olduğu anlamına gelmez.
- Simülasyon çekirdeğinin 10 `node:test` testi geçti; ana ajan aynı testleri yeniden başarıyla çalıştırdı. Kontroller, bozuk kayıtların reddedilmesini, kaynak sınırlarını, savaş sonucunun tek sefer uygulanmasını ve tekrarlanabilir 200 haftalık seferleri kapsıyor.
- `tests/browser-smoke.js`, ayrı `power-above-all-check` oturumunda 13 kontrol grubunu geçti; JavaScript hatası sayısı **0**. Gerçek taktik savaş tamamlanıp kazanıldı. İki çatışmanın ardından sefere **624 asker** döndü. Manuel kayıt ve otomatik kaydın tam olarak geri yüklenmesi kontrol edildi.
- Son tarayıcı kontrolünde de **0 JavaScript hatası** görüldü. **1440 × 960** ve **390 × 844** boyutlarında yatay taşma yoktu. Zafer, geri çekilme ve tekrar savaşa girme, asker ve hareket haklarını doğru biçimde azalttı.
- `output/playwright/` içindeki `desktop.png`, `economy.png`, `battle.png` ve `mobile.png` ekran görüntüleri görsel olarak incelendi.
- İlk sürümün README, STATUS, CHANGELOG ve REFERENCES belgeleri GitHub'da bulunuyor. Güncel değişikliklerin yüklenme durumunu yeni bir işlem öncesinde ayrıca kontrol edin.

## Oyun döngüsü, sınırlar ve tarih

- İlk döngü: bölge seç → karar ver → ordu gönder veya çatışmayı oyna → haftayı bitir → ekonomik sonuçları gör. Kayıtlar tarayıcıda yerel olarak tutuluyor.
- Harita, tarihsel adlar taşıyan 12 büyük oyun bölgesinden oluşuyor; 1789 idari sınırlarının birebir karşılığı değil. Kaynak miktarları, oyuncunun yetkileri ve senaryodaki çatışmalar oyun için basitleştirildi veya kurgulandı.
- Temel vergi ve tahıl göstergeleri, asker toplama maliyeti ve huzursuzluk kurallarıyla ilgili hatalı açıklamalar düzeltildi. Huzursuzluk katmanı kapalıyken yanıltıcı renk açıklaması artık gösterilmiyor.
- Denge öğretici düzeyde: hiçbir bölgesel emir vermeden ikinci haftadaki olayda `relief` seçmek, sekizinci haftada zafer getiriyor. Doğrulanan sonuç: **2057** hazine, **341** tahıl, **1200** asker, **36** huzursuzluk. Kapsamlı dengeleme henüz yapılmadı.
- Tarihsel açılış: Genel Meclis (États généraux), **5 Mayıs 1789'da Versay'da** açıldı. Senaryo bu gün başlıyor. Resmî kaynak `qwen-web scrape` ile okundu: https://en.chateauversailles.fr/discover/history/key-dates/summoning-estates-general-1789 .
