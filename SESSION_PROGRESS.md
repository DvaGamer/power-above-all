# Aktif geliştirme — hızlı devam kaydı

Güncelleme: 5 Eylül 2026 23:08 UTC. Görev hâlâ sürüyor; bitiş 6 Eylül 07:22:03 UTC (İstanbul 10:22). Kullanıcı üç deneyimi birlikte, farklı başlangıç rolleri, sonuçlu alternatif tarih ve hoş yarı stilize görsel dil istedi. Kesinleşmeyen dünya/rol/tarih ayrıntıları çalışma varsayımıdır.

## Son oynanabilir kanıt

`output/verify/roles-visible-20260905-230302-558-1717bcb0/REPORT.md` **GREEN**: 56 Unity testi, taze DX11 player, 22 PNG / 29 assertion / 3 JSON, 10 tarayıcı testi, 141 dosya manifesti. `PLAY_GAME.cmd` bu doğrulanmış build'i seçiyor. Üç rol ve iki haftalık sözler çalışıyor; eski kayıt v1 güvenle v2'ye taşınıyor. Root gerçek RU/TR seçim ve borç belgelerini gördü.

Art kontrol noktası commit `19e5fa8`; rol/araç/DX11 değişiklikleri commit hazırlığında. `roles-base-regression-20260905-230755-557-e852bd7c` eski atlas/savaş rotasında tam GREEN: 56 Unity, 27 kare, 26 assertion, 3 JSON, 10 tarayıcı. Aynı build `roles-six-week-20260905-231015-322-3f2f59ba` incelemesinde 6 hafta / 2 geri çekilme / 12 PNG / 40 assertion / 4 JSON ile exit0, kareler PASS; build yeniden kullanıldığı için doğru PARTIAL. İnsan kaydı hash'i 23:10 UTC aynı. Root yeni Dumas konseyi ve savaş RU karesini gördü.

## Sıradaki bağlı işler

1. Rol kontrol noktasını eski rotalarla doğrula, insan kaydı hash'ini kontrol et, commit et.
2. Taktikte eşzamanlı salvo ve mühimmatsız yakın temas kurallarını düzelt; gerçek oyuncu emirleriyle doğal savaş sonunu doğrula. Planlar `Unity/WorkNotes/tactical-rules-plan.md` ve `battle-review-plan.md`.
3. Atlasın 12 şehir siluetini okunur, elle tasarlanmış biçimlerle ayrıştır. `atlas-identity-plan.md`.
4. Denge ölçümünde bulunan siyasi boşluğu kapat: defalarca sözünü bozan aktöre aynı kişi koşulsuz yardım etmemeli. `balance-night.md` ve `history-night.md` tasarım dayanağıdır; sayıların tarihsel veri olduğu iddia edilmez.

## Güvenlik ve çalışma düzeni

- Unity/test/build/player süreçlerini yalnız root başlatır; ajanlar ayrı dosya kapsamlarında çalışır. Açık kullanıcı süreçleri kapatılmaz.
- Tüm shell çağrıları `bash -lc`; tüm dosya düzenlemeleri apply_patch. `rg` ve `qwen-web` bu ortamda bulunamadı.
- Testler `-shots` altında ayrı kayıt kullanır. İnsan kaydı `AppData/LocalLow/Power Above All/Power Above All/campaign-v1.json` SHA256: `18f3c57d89161fc471bc0aa997c8266c01d50ba1def670cd476081f1f5f7b63e`.
- `tools/night.ps1` ve `tools/night-prompt.txt` eski/güvensiz, çalıştırılmaz ve commit'e eklenmez.
- Direct3D12 kapanış çökmesi olay kaydıyla D3D12Core.dll'e izlendi. Taze build DX11 kullanır; görünür player gerçek kare için gereklidir. Gizli helper/editor devam eder.
- Yeniden kullanılan build incelemesi yalnız PARTIAL'dır; tam GREEN test + yeni build + player + kare + tarayıcı kapılarının hepsini ister.
- Savaş görselleri/geri çekilme doğrulandı; doğal taktik zafer ve gerçek insan girdisi henüz tamamlanmadı. Ses dinleme kalitesi iddia edilmez.
