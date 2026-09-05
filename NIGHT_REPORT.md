# Gece geliştirme raporu — çalışma sürüyor

Çalışma aralığı: 6 Eylül 2026 İstanbul 00:22–10:22. Kullanıcı daha erken durdurursa veya yeni yön verirse buna uyulur. Bu belge ara rapordur; on saatin tamamlandığı veya bütün vizyonun uygulandığı iddia edilmez.

## Doğrulanmış kontrol noktası

- Commit: `7aad17e`, `polish/unity-visual-feel`.
- Son sanat kapısı: `output/verify/painted-atlas-final-20260905-221432-805-e022084e/REPORT.md` — **GREEN**. İlk güvenlik temeli yukarıdaki commit'tedir; sanat kontrol noktası ayrıca kaydedilir.
- 25/25 Unity EditMode testi, yeni Windows player, 27 gerçek PNG, 26 durum kontrolü, 3 JSON durum kaydı ve 10/10 tarayıcı çekirdek testi.
- Standard shader saklama ana kaynağa taşındı. Normal Windows build filigransız; development menüsü ayrı.
- Güvenli doğrulama kullanıcı editörlerini kapatmaz, kişisel kaydı taşımaz; her koşu ayrı çıktı/kayıt dizini kullanır. Oyunun inceleme sırasında değiştirdiği dil/ses kalıcı kullanıcı tercihlerini etkilemez.
- `PLAY_GAME.cmd` en yeni tamamı geçen derlemeyi seçer. `node play-game.cjs --check` oyunu açmadan yol ve kanıtı gösterir.
- İnsan kaydı SHA256 aynı: `18f3c57d89161fc471bc0aa997c8266c01d50ba1def670cd476081f1f5f7b63e`.

## Uygulanan ve gerçek karelerde incelenen sanat paketi

- Boyanmış atlas ve yaşayan minyatürler yönünde krem, adaçayı, mavi, mercan ve koyu mürekkep paleti.
- Daha büyük savaş kadrajı; guaj arazi yüzeyi; daha doğal su kıyısı ve ağaç siluetleri; okunur polk ve emir kartları.
- Atlas kaynak etiketleri ve devre dışı emirlerin okunurluğu; Lorraine etiket konumu; veriyle tutarlı harita renkleri ve lejant.
- Dört ayrı kurgusal portre, mat dil kontrolleri, harita bağlamını koruyan kısa kâğıt sevk geçişi.
- 141 dağıtım dosyasının hash manifesti; başlatıcı bütün dosyaları doğrular. Altı haftalık çekirdek testleri geçti, uzun player rotası sıradadır.

Root atlası, ekonomi sayfasını, dört portreyi, dilekçeyi, sevk geçişini, RU/TR savaş ve raporu gerçek PNG'lerde inceledi. Koyu kenar/gölge, 9 piksel eski atlas kalıntısı ve Rusça başlık taşması düzeltildi. Portre varlığının kökeni ve üretim istemi [ART_ASSETS.md](ART_ASSETS.md) içindedir.

## Bilinen sınırlar

- İlk prototipin geniş vizyonu henüz yoktur: farklı oynanabilir roller ve geniş alternatif tarih bu ara raporda bitmiş özellik değildir.
- Savaş sonunda rapor/dönüş test edildi; önceki otomatik görsel rota geri çekilme kullanır. Bu, taktik zaferin veya tüm insan girişlerinin sınandığı anlamına gelmez.
- Ses kaynakları prosedürel taslaktır; dinlenmeden son ses kalitesi ilan edilmez.
- Gizli oyun penceresi DX12'de screenshot hatası, DX11'de siyah kare üretti. Gerçek görüntü için önceki canlı önizleme izni kapsamındaki görünür test player kullanılır; test/editor/helper süreçleri gizli kalır. Siyah kareyi başarılı sayan bir istisna eklenmedi.

## Sonraki inceleme

Sıradaki küçük kesit: üç başlangıç rolü, role özgü ayrıcalık ve iki hafta sonra bölge/ekonomi/siyasi ilişkilere dönen yükümlülük. Tasarım bir çalışma varsayımıdır. Sabah açılacak build ve üç öncelikli inceleme noktası son kontrol noktasına göre güncellenecek.
