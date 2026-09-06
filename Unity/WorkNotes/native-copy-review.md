# Gerçek Windows girdisiyle giriş, yardım ve konsey metni incelemesi

6 Eylül 2026. Tamamlanan `output/verify/native-input-20260906-053413-56299c5f` kanıtları salt okunur incelendi. Bu ajan oyuncu, derleme, test veya girdi helper'ı başlatmadı; kaynak Assets ve mevcut sonuçlar değiştirilmedi.

## Sonuç ve süreç sınırı

Sonuç `PARTIAL`: önceden tamamlanmış `public-mood-first-20260906-052918-104-2aaa7720` build'i yeniden kullanıldı; `currentSourceVerified=false`. Player 3 PNG / 11 assertion / 3 JSON, frame kontrolü 3/3 geçti. EditMode, build ve browser bu koşuda atlandı. Bu kayıt yeni tam GREEN gate değildir.

Owned player PID 22512, başlangıç `2026-09-06T05:34:13.6924405Z`, bitiş `05:36:19.0600681Z`; native exit 0 ve `timedOut=false`. Owner PID 20788, başlangıç `05:34:13.2555736Z`. Toplam review 128.24 saniye; player sınırı 180, owner sınırı 240 saniyeydi. Protokol `success=true`, 25 komut / 11 assertion; protokol bitişi `05:36:18.2312578Z`, failures boş.

Receipt'teki runtime assembly SHA256: `A2D5345FF5AD30D989A775E84DDF6B49844DA9114375E91522754732FC096B53`. Kopyalanmış script SHA256: `B6EBC6A173F87F1AF3A07BA5476ACC34BEEEA6EA62CCA1FA74C68711A1EFCED4`.

## Gerçek girdi ve değişmeyen kampanya

Root aşağıdaki gerçek girdi karelerini bizzat gördüğünü bildirdi; saatler İstanbul yerel saatidir:

- 08:34:18: Rusça giriş/günlük.
- 08:34:51: Rusça yardım.
- 08:35:11: yardımı kapatma; 08:35:13: Türkçeye geçiş; 08:35:14: Türkçe yardım.
- 08:35:45: yardımı kapatma; 08:35:47: Türkçe konsey; 08:35:48: `Wheel -10` ile Lefevre metnine kaydırma.

Koşunun kopyalanmış script'i yalnızca `new`, başlangıç `lang ru` / `panel journal`, başlangıç gözlemi, 120 saniye gerçek girdi penceresi, readonly assertion/state/shot ve save/load kullanır. Yardım, konsey ve Türkçe geçişi script tarafından taklit edilmez; `act` veya diğer ekonomik/politik işlem yoktur. Girdi penceresinden sonra gerçek `Language tr`, Week 0, Gold 840, Food 360 ve Troops 1200 assertion'ları geçmiştir.

`00-start.json`, `01-copy-native.json` ve gerçek save/load sonrasındaki `02-copy-loaded.json` ham olarak birebir aynıdır. Üç dosyanın ortak SHA256 değeri `3ADF6FCD87D98050E4790A11C375FE61573D57C868FA842C62B430A265DEEFB4`. Bu, gösterilen birkaç stok alanıyla sınırlı bir karşılaştırma değildir: dil/panel/yardım/scroll girdileri ve ardından save/load bütün campaign JSON'unu korumuştur. Görsel metinlerin okunurluğu için kanıt root'un yukarıdaki gerçek kare incelemesidir; bu ajan bu bölümde yeni ekran görüntüsü almadı.

## Önceki başlangıç reddi

`native-input-20260906-053350-5564096d` ilk denemesi yalnızca kopyalanmış `review.script` içerir. İlk shot adı `00-copy-start` idi; root, helper'ın zorunlu `00-start` preflight koşulunu bu nedenle reddettiğini bildirdi. Bu klasörde owned receipt, player log, native exit veya oyuncu görüntüsü yoktur. Sonraki koşuda ad `00-start` olarak düzeltildi ve yukarıdaki süreç/protokol kanıtı oluştu. İlk reddi bir çalışan oyunun çökmesi ya da native exit sonucu olarak sunmuyoruz; eski klasör olduğu gibi korundu.
