# Bahçe tacı: değişiklik öncesi gerçek karşılaştırma karesi

`orchard-before-20260906-051301-350-76b5caca`: PARTIAL/native0,38 saniye,4 PNG/31 assertion/7 JSON; BuildUnchanged141 dosya raporlandı. Bu ajan tamamlanmış JSON ve02 RU PNG'yi salt okunur inceledi; yeni test, derleme veya oyuncu başlatmadı.

00 deploymentRU =01 deploymentTR raw byte-eşit: SHA256 `18B6E839319452D8BEC6CD6A8378A7BCC8BF97B65994EF2D0BB02741B3D8903A`. İkisinde t0, pause=true,1200 oyuncu/1114 düşman ve outcome yoktur.

02 contactRU =03 contactTR =04 pause-held raw byte-eşit: SHA256 `63A4327A92206B1BE33DEE2901D61E8A6F0E8D9BEF4904AC1B4F027B38CF6BFC`. Pause=true, t24.699909, PlayerHold3.425018, EnemyHold0, outcome yoktur. Oyuncu topçusu Ammo10; sahada karşılıklı atış gerçekleşmiştir. Bu bitmemiş snapshot'ta Casualties0 sonucu henüz üretilmediği içindir; oyuncu mevcut toplamı384+328+246+206=1164 ile36 kişi başlangıçtan eksiktir.

Gerçek02 RU görüntüsünde20 yuvarlak ağaç tacı yaklaşık x418..602 / y328..452 alanını kaplıyor. Oyuncu Line384 kişiyle (−16.0792,−0.24234) civarında, hedefi(−7.00578,3.98698); alt ağaç sıraları bazı gövdeleri ve asker siluetlerini örtüyor. Oyuncu cavalry246 kişiyle (−9.94452,9.91370), enemy Line356 kişiyle (−13.98506,9.61981) üst/sağ bahçe kenarı yakınında. Seçili süvari, yakındaki kalabalık ve bahçe içindeki hat görsel örtüşmeyi değerlendirmek için gerçek sahne sağlar; men/konum enjeksiyonu yoktur.

Candidate geldiğinde ayrı realtime koşunun tüm JSON'unun otomatik byte-eşit olması varsayılmaz. Önce her koşunun kendi pause eşitliği, sonra gerçekten gözlenen t/konum/men ve aynı ekran bölgesindeki okunurluk karşılaştırılmalı. RU/TR tüm PNG eşitliği de beklenmez; UI dili değişir. Eski before artifact ve script değişmedi.

## Tamamlanmış candidate ile gerçek karşılaştırma

`orchard-candidate-20260906-051614-489-fea6b772`: GREEN,55 saniye,407/407 Unity,4 PNG/31 assertion/7 JSON ve10 browser; failures boş. Root00/02 karelerini, visual ajan dört before/after çiftini gördü ve yeni taç/örtüşmeyi kabul ettiğini bildirdi. Bu ajan ayrı olarak bütün yedi JSON dosyasını okudu ve SHA256 karşılaştırdı; hiçbir süreç başlatmadı.

Bu **gerçek iki koşuda** yedi before/candidate JSON çifti byte-eşit çıktı. Önceden beklenen zaman eşitliği varsayılmadı. Deployment00=01 ve contact02=03=04 her candidate içinde de aynı hash'lere sahiptir; üstteki iki hash candidate'da değişmedi. İki temas snapshot'ı aynı t24.699909 / Hold3.425018 / pause=true / HasOutcome=false değerlerini, aynı sekiz alayın Men/Ammo/konum/hedeflerini içerir. Görsel before ve candidate karşılaştırması bu ölçülmüş örnekte aynı gerçek oyun anına dayanır.

İki gerçek retreat report05 byte-eşit: SHA256 `5143F7F59069104F1CE37A7DE13007A73B0FC013D8A09297EA2B1B481EABA759`. Gözlenen toplam kayıp77; bu36 savaş kaybı ve kalan1164 kişiye gerçek yuvarlanmış retreat41 ek kaybıyla tutarlıdır. İki campaign return06 da byte-eşit: SHA256 `3E321ECBFD638B14838B27CF760463D2B16EB5AEB1FF60AFE509B1776D1A3F7C`; Troops1123, Food342, Supplies115, Morale56.8881073. Gameplay sonucunun bu ölçülmüş rota boyunca değişmediği kanıtlanmıştır; sonraki farklı realtime koşular için otomatik eşitlik garantisine çevrilmez.
