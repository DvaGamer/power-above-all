# Aktif geliştirme — hızlı devam kaydı

Güncel: **01:40 UTC, 6 Eylül 2026**. On saatlik görev **07:22:03 UTC / 10:22 İstanbul** saatine kadar sürer. Erken final veya goal complete yok. Kullanıcı farklı başlangıç rolleri; kişisel iktidar, ülke yönetimi ve savaş; sonuçlu alternatif tarih; hoş, yarı stilize ve özenle tasarlanmış görseller istedi. Kesin rol kadrosu ve kampanya sonu hâlâ çalışma varsayımıdır.

## Son doğrulanmış paket

Dal `polish/unity-visual-feel`; önceki commit `3040767`. Root yeni paketin yerel commit'ini hazırlıyor. Açık Unity/player/owner yok. Güvensiz eski `tools/night.ps1` ve `tools/night-prompt.txt` untracked kalır; çalıştırılmaz ve commit'e alınmaz.

Son tam kapı `military-art-final-20260906-012710-424-48b0deff`: **GREEN**, 176 Unity testi, 21 PNG, 38 kontrol, 21 JSON, 10 tarayıcı testi ve 141 dosyalık build. Runtime SHA256 `fc1e21937ace6213b4f62fd20cd2e7727fe465ada5baa3e853f713ce61a4cff6`. `node play-game.cjs --check` bu build'i complete-build olarak seçti; `tools/verified-player.cjs` bir kütüphanedir, CLI kontrolü değildir.

Yeni oynanabilir davranış `CampaignVictoryDecisions.cs` içindedir. Gerçek zaferin eski sonucu uygulandıktan sonra tek PendingVictoryId açılır. Dumas'yı tanımak Fatigue−12 / Relationship+4 / Ambition+3; Ambition>Loyalty ise Power4 gerektirir. Prim ceil(currentTroops/12) Gold, Loyalty+5 ve zafer bölgesine Control+3 verir. İki fayda da sıfırsa prim atomar reddedilir. Decline ek etkisiz kapatır. Esc yalnız pencereyi saklar. Başarılı hafta/yürüyüş teklifi kapatır; eski dilekçe ve rol vadesi önceliği sürer.

Arşiv v4 PendingVictoryId varlığını/non-null değerini zorunlu tutar. v1–v3 eski zafer tekliflerini türetmez. Accord gerekli alan kontrolü Version>=3, accord migration Version<3 olarak kalmalıdır; CurrentVersion'a göre eski v3 anlaşmasını sıfırlamak yanlıştır. RoleCampaign/RegionalAccord eski sürüm testleri adresli uyarlandı.

## Gerçek kanıtlar ve sınırlar

- Birleşik kapı önce smoke-uncovered emirlerini smoke- artefakt adlarıyla, sonra victory-campaign'i aynı player'da yürüttü. Eski duman/alaylar ikinci savaş dünyasına taşınmadı. İkinci bölümün 12 JSON'u önceki bonus koşusuyla birebir aynı.
- Doğal zafer125.803 saniye,196 kayıp,+24 teçhizat; 1004 sağ kalan. Prim840→756 Gold, Dumas60→65 Loyalty, Champagne70.5→73.5 Control. Seçili Normandy etkilenmedi. Açık teklif ve ödenmiş sonuç save/load eşitliğini korudu.
- Ayrı `victory-recognition-20260906-012252-141-2481db02` PARTIAL/native0: 13 PNG / 25 kontrol / 12 JSON. Power59→55, Fatigue35→23, Dumas52→56 Relationship /83→86 Ambition, Gold840 aynı. İki yükleme aynı.
- Yeni shader `PowerAboveAll/PowderWashAlpha`: maske64×32, mask alpha .66, yaş katsayısı .46, tint(.96,.95,.88), tek açık alfa pass. Yaşam, adet, drift ve simülasyon aynı. Root ve visual ajan beyaz dörtgenlerin kalktığını, çok hafif dumanı ve açık polk siluetlerini gördü. Erken duman hâlâ zayıf; güçlü bir salvo gösterisi tamamlandı denmez.
- Dumas'nın komşu portreden gelen sol17px parçası üç çizim fonksiyonunda çıkarıldı; PNG değişmedi, hedef dikdörtgen ile UV birlikte kırpıldı. Root/ajan yeni RU/TR zafer penceresinde çiziksiz aynı silueti gördü. Rol ve vade portre yolları bu son rotada ayrıca görüntülenmedi.
- `native-input-20260906-013205-6d0f5541`: gerçek Escape, konseyden açma, TR/RU ve fare primi; PARTIAL/native0/noTimeout,200.47s,9PNG/18kontrol/8JSON. İlk Start çağrısı exit1/boş çıktı verdi; aynı owner üzerindeki Inspect ve altı girdi exit0 oldu. İlk neden bilinmiyor. Sonradan helper'a yalnız stdout hata/stack trap eklendi; bitmiş receipt'e Inspect gerçekten reddedilip tanıyı yazdı.
- Native helper40 saf test, tools47 güvenlik testi,523 RU/TR girdi PASS. Native timeout açık180..300, varsayılan180; owner=player+60. Bu zafer koşusu240/300 kullandı. Shift ve elle nişanlı salvo sonraki native iş olabilir; henüz kanıtlanmadı.

## Sıradaki iş

Önce bu doğrulanmış paketin commit'ini bitir ve notes'a hash yaz. Sonra `Unity/WorkNotes/next-opponent-choice.md` tasarımını oku: üç NPC girişimi arasında Dumas'nın gerçek açlık sonrası ilan ettiği yerel rekvizisyon öneriliyor. **Henüz uygulama onayı verilmedi veya Assets yazılmadı.** Tetikleyici, gerçek tahmin hesabı, müdahale ve eski vade sırası root tarafından netleştirilmelidir. Diğer görsel aday `regiment-label-readability.md`: üç konsept ve önerilen alan dışı kısa bağlantı çizgili etiket; henüz uygulanmadı.

Üç alt ajan mevcut; hepsi son görevlerini tamamladı. Yeni bounded iş için `followup_task` gerekir, yalnız send_message çalışmayı başlatmaz. Root entegrasyon ve bütün compile/probe/Unity/player/native başlatmalarının tek sahibidir. Ajanlar ayrı dosyalarda çalışır ve kendi süreçlerini başlatmaz.

## Ortam

Her komut `bash -lc` içinde; dosyalar yalnız apply_patch ile düzenlenir. Windows araçları `node.exe` ve `powershell.exe`, WSL okuma araçları grep/find/python3. Büyük dosyanın gerekli kısmını oku; önce NOTES'a bak. Gereksiz qwen-web tekrarları yok: helper ve rg bu ortamda bulunamadı.

İnsan kaydı `C:/Users/USER/AppData/LocalLow/Power Above All/Power Above All/campaign-v1.json`; son SHA256 `18f3c57d89161fc471bc0aa997c8266c01d50ba1def670cd476081f1f5f7b63e`. İnceleme profili -shots altında ayrı .campaign kullanır. Kullanıcı süreçleri kapatılmaz. Yeni oyuncu görünür, yardımcılar/editor gizlidir. Ayrıntılı tarihçe NOTES, ara teslim NIGHT_REPORT içindedir.
