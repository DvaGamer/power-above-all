# Gece çalışması - kalıcı sözleşme

## 6 Eylül 2026 güncel kullanıcı talimatı

Kullanıcı yeni vizyon seçimlerinden sonra «Я ухожу на 10 часов поспать. сиди и делай игру все это время» dedi. 21:22:03 UTC–07:22:03 UTC arasında devam eden tek özerk çalışma yürütülür. Aşağıdaki eski çağrı başına tek görevde durma kuralı bu oturumda uygulanmaz; küçük kontrol noktalarıyla sıradaki işe devam edilir. Onaylı vizyon `VISION.md` içindedir. Önce güvenilir polish temeli, sonra bu vizyona bağlı küçük oynanabilir geliştirmeler; açık kararlar çalışma varsayımı olarak kaydedilir.

Doğrulama aracı önce güvenli hâle getirilir: kullanıcının açık Unity süreçleri zorla kapatılmaz, kişisel kayıt taşınmaz veya silinmez, önceki çıktılar yeni kanıt sayılmaz. `-shots` zaten ayrı kayıt profili kullanır. Proje kilitliyse izole kaynak kopyası doğrulanır. Önceden var olan çalışma ağacı değişiklikleri `git checkout --` ile topluca geri alınmaz; yalnız kendi yeni değişikliğine hedefli düzeltme yapılır. Yeni doğrulama aracı eski riskli davranışlarının yerine geçer.

Bu dosya, kullanıcı bilgisayarın başında değilken tek başına çalışan ajan için sözleşmedir.
Her turda **önce bu dosya, sonra `NIGHT_QUEUE.md`, sonra `NIGHT_LOG.md` son 40 satırı** okunur.
`AGENTS.md` ve `POLISH_PLAN.md` yürürlüktedir; burada yazan işleyiş kuralları onların üstüne eklenir.

## Gecenin tek hedefi

Sabah kullanıcı `PLAY_GAME.cmd` ile oyunu açtığında, akşamki hâlinden **gözle görülür biçimde daha
iyi ve daha oynanabilir** bir oyun bulmalı. Ölçüt belge sayısı, satır sayısı veya tamamlanmış madde
sayısı değil; ekranda görünen kalitedir.

Sabah teslim edilecek üç şey: çalışan bir Windows derlemesi, en yeni başarılı `output/verify/<koşu>/shots/contact-sheet.jpg`
üzerinde okunabilen ilerleme ve `NIGHT_REPORT.md` içinde dürüst bir özet.

## Değişmez kurallar

1. **Yeşil kapı.** Her commit'ten önce `powershell -NoProfile -ExecutionPolicy Bypass -File tools\verify.ps1`
   çalıştırılır ve **çıkış kodu 0** olmalıdır. Kırmızı ağaç bırakılmaz.
2. **Küçük kontrol noktaları.** Birbirine bağlı değişiklikler doğrulanmış bir kesit oluşturur. Commit mesajı Türkçe ve tek satırdır; on saatlik çalışmayı her commit'te durdurma.
3. **Hatalı denemeler saklanır.** Aynı başarısız komutu tekrarlamak yerine nedenini incele. Gerekirse yalnız kendi yeni değişikliğine hedefli düzeltme yap; başka iş veya önceki kanıt topluca geri alınmaz. Tamamlanmayan işin sınırını kaydet.
4. **Zaman sınırı.** Tek görev yaklaşık 35 dakikayı aşarsa, o ana kadarki çalışan kısım commit
   edilir, kalanı yeni bir görev satırı olarak kuyruğa yazılır.
5. **Dal.** Çalışma dalı `polish/unity-visual-feel`. `main` dalına dokunulmaz, `--force` push yoktur,
   geçmiş yeniden yazılmaz. Push saatte en fazla bir kez.
6. **Kanıt olmadan tamamlandı denmez.** Her tamamlanan madde için `NIGHT_LOG.md` içine dosya yolu
   yazılır: test XML'i, derleme günlüğü veya kare adı. Kanıtı olmayan madde işaretlenmez.
7. **Kapsam.** Önce polish temeli; ardından güncel `VISION.md` hedeflerine bağlı, küçük ve ayrıca doğrulanabilir oynanabilir kesitler. Rol, iktidar, ülke ve savaş arasında gerçek sonuç bağı olmalı. Cevaplanmamış vizyon seçimleri kullanıcı kararı diye sunulmaz.
8. **Testler anlamını korur.** Test geçsin diye doğrulama gevşetilmez. Gerçek bir arşiv sürümü veya tasarım sözleşmesi değişikliği eski sürüm geçiş testleriyle birlikte açıkça güncellenir; yeni davranış ve retlerin atomikliği ayrıca sınanır.
9. **Bağımlılık eklenmez.** Yeni Unity paketi, npm paketi veya harici varlık indirilmez.
10. **Kullanıcının verisi korunur.** `%USERPROFILE%\AppData\LocalLow\Power Above All` altındaki kayıt taşınmaz, silinmez, test için yazılmaz. `-shots` koşuları kendi `.campaign` profilini kullanır; inceleme dil/ses tercihini kalıcılaştırmaz.

## Tur protokolü

Süren özerk çalışma her küçük kontrol noktasında şu sırayı izler:

1. `git status --short` ve `git log --oneline -3`. Önceki tur yarım kaldıysa önce onu toparla.
2. Bu dosyayı, `NIGHT_QUEUE.md` dosyasını ve `NIGHT_LOG.md` son 40 satırını oku.
3. Prompt'a iliştirilen kareleri **gerçekten incele**. Gördüğün kusuru kelimelerle yaz.
4. Güncel vizyon ve görülen en önemli eksikliğe göre sıradaki bağlı işi seç. Yeni karar ve gerçek kabul ölçütünü kuyruğa kaydet.
5. Küçük ve hedefli değişiklik yap. Dosyayı baştan yazma, gereksiz yeniden düzenleme yapma.
6. `tools\verify.ps1` çalıştır. Kırmızıysa nedeni düzelt; başarısız çıktı korunur. Unity, player ve probe çalıştırmalarını root koordine eder.
7. Commit et.
8. `NIGHT_QUEUE.md` içindeki kutuyu işaretle, `NIGHT_LOG.md` sonuna tek paragraf ekle:
   ne değişti, kanıt yolu, sırada ne var.
9. 07:22:03 UTC'ye kadar veya kullanıcı durdurana kadar sıradaki yararlı işe devam et. Görevi erken tamamlandı diye işaretleme.

## Görsel kaliteyi nasıl yargıla

Kareleri incelerken şu sorular sorulur ve cevabı hayır olan ekran yeniden ele alınır:

- Bu ekran Power Above All'a mı ait, yoksa herhangi bir strateji oyununa mı benziyor?
- Tek güçlü odak var mı? Göz nereye önce gidiyor?
- Renk, tipografi ve kenarlıklar diğer ekranlarla aynı dili konuşuyor mu?
- Sayı ve etiketler taşmadan, kesilmeden okunuyor mu? Rusça ve Türkçe ayrı ayrı kontrol edildi mi?
- Varsayılan web/IMGUI kontrolü hissi kaldı mı? Gri düğme sırası, standart kaydırma çubuğu,
  işlevsiz süs var mı?
- Oyuncu bu ekrandan bir hikâye anlatabilir mi?

Akşamki kareler üzerinde tespit edilmiş, kanıtı `output/shots/` içinde duran somut kusurlar:

- Savaş ekranının paleti sefer haritasının kimliğiyle uyuşmuyor: parlak yeşil zemin, düz mavi nehir
  şeridi ve koyu gri çubuklar fildişi/orman yeşili/mat altın dilinden kopuk.
- Savaşın alt komut şeridi hazır bir UI-kit düğme sırası gibi duruyor.
- Harita kipleri (denetim, huzursuzluk, vergi, ordu) birbirinden zor ayırt ediliyor.
- Her karede sağ altta `Development Build` filigranı var.
- Sağ paneldeki kaydırma çubuğu varsayılan görünümde.
- Savaş arazisi düz bir dikdörtgen ve düz bir nehirden ibaret; kompozisyon yok.

## Doğrulama araçları

| Komut | Ne yapar |
| --- | --- |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\verify.ps1` | Tam kapı: kapalı projede EditMode, taze Windows build, izole player kareleri, kare denetimi, tarayıcı çekirdeği. Her koşu ayrı `output/verify/<koşu>/REPORT.md` üretir. Açık editör kapatılmaz. |
| `... tools\verify.ps1 -SkipShots` | Hızlı kapı: yalnız testler ve derleme. Ara denemelerde kullan. |
| `python tools\shot-check.py output\shots --baseline output\shots-baseline` | Karelerin bir öncekine göre ne kadar değiştiğini yüzdeyle verir. |
| `tools\shots.script` | Oyunun kendi kendini gezdirdiği senaryo. Yeni ekran çekmek için buraya satır ekle. |

**Aynı Unity projesi açıkken batchmode başlatılmaz.** `verify.ps1` kilidi reddeder; kullanıcı editörü açık bırakılır ve gerekirse izole proje kopyası seçilir.

Doğrulanmış gerçekler, yeniden keşfetmeye gerek yok:

- Editör: `C:\Users\USER\Tools\Unity\6000.3.23f1\Editor\Unity.exe`; batchmode lisansı sorunsuz.
- Güncel test sayısı ve süre her koşunun XML/günlük çıktısından alınır; eski başlangıç sayıları güncel sonuç sayılmaz.
- Windows derlemesi taze ve benzersiz çıktı dizinine yapılır. Yeniden kullanılan build incelemesi yalnız PARTIAL olabilir.
- Derlenmiş oyun `-shots <klasör> -script <dosya>` ile kendini gezdirip 1440x900 PNG yazıyor.
- Batchmode PlayMode testinde `WaitForEndOfFrame` çalışmaz; ekran görüntüsü yalnızca derlenmiş
  oyundan alınır.

## Sabah raporu

Gece bitmeden `NIGHT_REPORT.md` yazılır ve her turda güncellenir:

- Bu gece gerçekten değişen şeyler, her biri için kare adı veya günlük yolu.
- Yeşil kapının son durumu ve zamanı.
- Engelli görevler ve sebepleri.
- Kullanıcının sabah bakması gereken üç şey.
- Dürüstlük: yapılmayan iş yapılmış gibi yazılmaz, tahmin sonuç gibi sunulmaz.
