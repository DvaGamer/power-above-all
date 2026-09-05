# Gece günlüğü

En yeni kayıt en alta eklenir. Her tur bir paragraf: ne değişti, kanıt yolu, sırada ne var.
Kurallar `NIGHT_BRIEF.md`, görevler `NIGHT_QUEUE.md`.

---

**5 Eylül 2026, gece kurulumu (Claude oturumu).** Gece çalışmasının zemini kuruldu, oyun kodunda
görsel bir değişiklik yapılmadı. Doğrulananlar: Unity batchmode bu makinede lisans sorunu olmadan
çalışıyor; 21 EditMode testi yaklaşık 3 saniyede geçiyor; Windows derlemesi yaklaşık 15 saniye
sürüyor; derlenmiş oyun `-shots` ve `-script` argümanlarıyla kendini gezdirip 1440x900 PNG yazıyor.
`tools/shots.script` senaryosu sefer haritası, iki dil, dört harita kipi, emir, iki hafta, ekmek
dilekçesi, yürüyüş ve tam bir savaş dahil 15 kare üretti; hepsi `tools/shot-check.py` denetiminden
geçti. Codex'e kare iliştirmenin gerçekten çalıştığı ayrıca sınandı: model kareyi görüp savaş
alanının ekranda küçük kaldığını ve alt şeridin sıkışık olduğunu kendi kelimeleriyle bildirdi.
Yeni dosyalar: `NIGHT_BRIEF.md`, `NIGHT_QUEUE.md`, `NIGHT_LOG.md`, `tools/verify.ps1`,
`tools/night.ps1`, `tools/night-prompt.txt`, `tools/shots.script`, `tools/shot-check.py`,
`Unity/Assets/Scripts/AutoShots.cs`. Kanıt: `output/verify/REPORT.md` ve `output/shots/`.
Sırada: kuyruk maddesi 00, kapıyı gerçek `Unity/` projesinde yeşile getirmek.

**6 Eylül 2026, 00:22 İstanbul — yeni kullanıcı görevi.** Kullanıcı on saat boyunca özerk geliştirme istedi. Vizyon: parti öncesi farklı roller; kişisel iktidar, ülke yönetimi ve savaş birlikte; tarihten beslenen denge ve yaşayan sonuçlarla alternatif tarih. İlk görev 00 için doğrulama güvenliği ve shader kaynağı toparlanıyor. Eski `verify.ps1` kullanıcı süreçlerini kapatıp insan kaydını taşıdığı için önce düzeltilir; yeni süreçler yalnız bize ait olacak. Üç alt ajan güvenli araçlar, savaş ve atlas/kabine incelemelerine ayrıldı. Henüz yeni test/derleme sonucu yok. Süre bitişi 10:22 İstanbul; görev, kontrol noktalarıyla devam eder.

**6 Eylül 2026, 00:48 İstanbul — temel kapı GREEN, 00–01 tamam.** Kanıt `output/verify/baseline-visible-20260905-214616-749-5ef5f22d/REPORT.md`: Unity23/23, taze Windows player,20PNG/26assert/3JSON ve tarayıcı10/10. Standard shader saklama ana kaynağa taşındı, normal build filigransız; development ayrı menüde. Araçlar kullanıcı süreçlerini kapatmıyor, insan kaydını taşımıyor, her koşuyu yeni klasörde tutuyor; hata ve siyah kareleri reddediyor. Gizli DX12 capture hata, gizli DX11 siyah kare verdi; görünür test player önceki canlı önizleme izni kapsamında gerçek görüntü üretti. Kullanıcı kayıt SHA256 değişmedi. Root gerçek atlası gördü; eski görsel kusurlar sürüyor. Sırada yeni sanat paleti, savaş kompozisyonu ve atlas/kabine okunurluğu.
