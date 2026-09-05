# Gece geliştirme raporu — çalışma sürüyor

Çalışma aralığı: 6 Eylül 2026 İstanbul 00:22–10:22. Kullanıcı daha erken durdurursa veya yeni yön verirse buna uyulur. Bu belge ara rapordur; on saatin tamamlandığı veya bütün vizyonun uygulandığı iddia edilmez.

## Doğrulanmış kontrol noktası

- Dal `polish/unity-visual-feel`; sanat kontrol noktası `19e5fa8`. Yeni rol kesiti ayrı commit hazırlığındadır.
- Son rol kapısı: `output/verify/roles-visible-20260905-230302-558-1717bcb0/REPORT.md` — **GREEN**.
- 56/56 Unity EditMode testi, yeni Direct3D11 Windows player, 22 gerçek PNG, 29 durum kontrolü, 3 JSON durum kaydı ve 10/10 tarayıcı çekirdek testi.
- Standard shader saklama ana kaynağa taşındı. Normal Windows build filigransız; development menüsü ayrı.
- Güvenli doğrulama kullanıcı editörlerini kapatmaz, kişisel kaydı taşımaz; her koşu ayrı çıktı/kayıt dizini kullanır. Oyunun inceleme sırasında değiştirdiği dil/ses kalıcı kullanıcı tercihlerini etkilemez.
- `PLAY_GAME.cmd` en yeni tamamı geçen derlemeyi seçer. `node play-game.cjs --check` oyunu açmadan yol ve kanıtı gösterir.
- İnsan kaydı SHA256 aynı: `18f3c57d89161fc471bc0aa997c8266c01d50ba1def670cd476081f1f5f7b63e`.

## Uygulanan ve gerçek karelerde incelenen sanat paketi

- Boyanmış atlas ve yaşayan minyatürler yönünde krem, adaçayı, mavi, mercan ve koyu mürekkep paleti.
- Daha büyük savaş kadrajı; guaj arazi yüzeyi; daha doğal su kıyısı ve ağaç siluetleri; okunur polk ve emir kartları.
- Atlas kaynak etiketleri ve devre dışı emirlerin okunurluğu; Lorraine etiket konumu; veriyle tutarlı harita renkleri ve lejant.
- Dört ayrı kurgusal portre, mat dil kontrolleri, harita bağlamını koruyan kısa kâğıt sevk geçişi.
- 141 dağıtım dosyasının hash manifesti; başlatıcı bütün dosyaları doğrular. Altı haftalık çekirdek testleri geçti; önceki build'de görünür DX11 ile altı hafta ve iki geri çekilme de tamamlandı. Yeni rol kaynaklarında eski rotalar ayrıca denetleniyor.

Root atlası, ekonomi sayfasını, dört portreyi, dilekçeyi, sevk geçişini, RU/TR savaş ve raporu gerçek PNG'lerde inceledi. Koyu kenar/gölge, 9 piksel eski atlas kalıntısı ve Rusça başlık taşması düzeltildi. Portre varlığının kökeni ve üretim istemi [ART_ASSETS.md](ART_ASSETS.md) içindedir.

## Bilinen sınırlar

- Üç çalışma rolü ve ayrıcalık/söz döngüsü uygulandı: saray avansı, meclis tahıl sözü, ordunun zor alımı. Kazanç, vade, ödeme ve ihlal bedelleri imzadan önce görünür; sonuçlar kaynaklara, bölgelere, desteğe ve kişisel ilişkilere döner. Bu kesit tüm kariyer veya geniş alternatif tarih sisteminin tamamı değildir.
- Savaş sonunda rapor/dönüş test edildi; önceki otomatik görsel rota geri çekilme kullanır. Bu, taktik zaferin veya tüm insan girişlerinin sınandığı anlamına gelmez.
- Ses kaynakları prosedürel taslaktır; dinlenmeden son ses kalitesi ilan edilmez.
- Gizli oyun penceresi DX12'de screenshot hatası, DX11'de siyah kare üretti. Gerçek görüntü için önceki canlı önizleme izni kapsamındaki görünür test player kullanılır; test/editor/helper süreçleri gizli kalır. Siyah kareyi başarılı sayan bir istisna eklenmedi.

## Sonraki inceleme

Sıradaki kesit taktik kuralların doğruluğu ve gerçek emirlerle doğal savaş akışıdır. Paralel olarak atlas şehirleri ayrışacak. Sabah açılacak build ve üç öncelikli inceleme noktası son kontrol noktasına göre güncellenecek.
