# Halk desteği — gerçek görüntü ve kısa metin önerisi

6 Eylül 2026. Salt okunur inceleme; Assets değiştirilmedi, test/Unity/oyuncu/derleme/Git başlatılmadı. Root'un `output/verify/public-mood-first-20260906-052918-104-2aaa7720` koşusundaki sekiz tam 1440×900 PNG açıldı: 00/01 başlangıç, 02/03 37→40, 06/07 engellenmiş dilekçe, 08/09 58→61. Root raporu GREEN407/12PNG/106assert/8JSON; bu belgede bütün otomatik sonuçların bağımsız yeniden doğrulandığı iddia edilmiyor.

## Gerçek karelerde görülenler

- 00/01: kentli desteği 35/100 ve haftalık genel etki +2, yeni metnin tamamı ve Paris raporuna geçiş düğmesi okunuyor. TR'de önceki sübvansiyon paragrafının daha fazlası üstte görünüyor; bu bottom-scroll metin yüksekliği farkı, yeni blok kesilmesi değil.
- 02/03: mevcut destek 37, gelecek hesap 40 ve bölge başına +0 doğru ayrılıyor. Ancak üstteki ortalama huzursuzluk “40→40” görünürken değişim −0,6; iki uçta hassasiyet eksikliği gerçek görsel sorun. Root bunu sayı biçimiyle düzeltmeyi üstlendi; beş yeni metnin değişmesi tek başına çözmez.
- 08/09: mevcut destek 58, gelecek hesap 61 ve bölge başına −1 okunuyor. Etkiyi bütün ortalama değişimle karıştırmamak için destek alt başlığı ve üstteki ortalama satırı ayrı kalmalı.
- 06/07: gerçek ekmek dilekçesi modalı açık. Sağda karartılmış hesap belgesinde bloke nedeni ve mevcut 43 destek görünüyor; sonraki hesabın sayısal satırı gösterilmiyor. Modalın üç gerçek seçeneği ve dilleri tam okunuyor. Bunun üzerine ikinci uyarı veya yeni popup gerekmiyor.
- Gözlenen temel yoğunluk sorunu, `ui.mood.order` içinde açlık/maaş/garnizon/yerel kararların anlatılıp hemen sonraki `ui.economy.unrest.reason` içinde yeniden sıralanması. İlk paragraf yalnız sübvansiyonun **hesaptan önce** işlendiğini anlatsın; mevcut genel tahmin açıklaması başka nedenleri taşımaya devam etsin.

## Beş mevcut entry için önerilen kesin RU/TR değerleri

Key ve placeholder sayıları korunur. Başlık doğrudan desteği adlandırır; ayrı mevcut/gelecek değerleri kaybolmaz. Eşik cümlesi 40'ı orta aralığa, 60'ı yüksek aralığa dahil eder; kesirli destekler nedeniyle “40–59” denmez.

| Key | RU | TR |
| --- | --- | --- |
| `ui.mood.title` | `ПОДДЕРЖКА ГОРОЖАН` | `KENTLİ DESTEĞİ` |
| `ui.mood.current` | `Сейчас: {0}/100.` | `Şimdi: {0}/100.` |
| `ui.mood.next` | `В конце недели: {0}/100 → {1} волнений в каждой области.` | `Hafta sonunda: {0}/100 → her bölgede {1} huzursuzluk.` |
| `ui.mood.rules` | `В неделю: ниже {0} — {2}; от {0} до {1} (не включая) — {3}; от {1} — {4}.` | `Haftalık etki: {0} altı {2}; {0} dahil, {1} hariç {3}; {1} ve üzeri {4}.` |
| `ui.mood.order` | `Выплата или срыв помощи Парижу меняет поддержку до расчёта волнений.` | `Paris yardımının ödenmesi veya aksaması, desteği huzursuzluk hesabından önce değiştirir.` |

`ui.economy.unrest.reason` burada yeniden yazılmaz. Eski `ui.mood.order` RU'da altı, TR'de yaklaşık beş satırdı; kısa önerinin daha az yer kaplaması beklenir, fakat yeni build olmadan kesin satır sayısı veya piksel kazancı iddia edilmez. Formül kaynakta aynı kalır: destek <40 için +2, 40≤destek<60 için +0, destek≥60 için −1. `next` yalnız gerçek hafta geçişi mümkünken görünmeye devam etmelidir. Sübvansiyon düğmesinin kendisinin anlık destek verdiği söylenmez.

## Kabul durumu

Mevcut sekiz karede yeni blok açısından clipping veya kontrast engeli yok. Mevcut ilk adayın ortalama sayı hassasiyeti ve tekrarlı metni düzeltilmeli. Yukarıdaki metinler henüz kaynakta uygulanmış veya gerçek oyuncuda görülmüş sayılmaz; root'un yeni kısa metin ve hassas ortalama ile RU/TR kareleri son kabulü verecek.
