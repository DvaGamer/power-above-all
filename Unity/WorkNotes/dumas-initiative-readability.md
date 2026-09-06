# Dumas girişimi — kaynak üzerinden UX incelemesi

İlk incelemede yalnız `CabinetDumasInitiative.cs`, ilgili `ui.forage.*` / `dumas.reason.*` satırları, CabinetHud çağrıları ve mevcut `CampaignDumasInitiative` sözleşmesi okundu. Ardından root'un aşağıdaki gerçek kareleri incelendi. Assets değiştirilmedi; ekran görüntüsü, player veya Unity başlatılmadı. İlk kaynak incelemesindeki yükseklik tahminleri kesin runtime ölçümü değildir.

## Anlam ve korunması gerekenler

Çekirdek ile arayüz uyumlu: girişim önceden duyurulur, sonraki haftada oyuncudan onay istemeden o anda geçerli plana göre uygulanır/iptal olur. Belge kapanması veto değildir. Bölge mevcut `ArmyRegionId` olup orduyla taşınır; bu, eski mandate/accord gibi sabit imza bölgesi değildir ve `camp` bunu açıkça söyler. DueWeek gerçek tarihe çevrilir. FoodGathered ve bölgesel/siyasi etkiler aynı forecast'ten, veto ilişki bedeli gerçek clamped deltadan gelir. Yetersiz olmayan/çok büyük açık/ordusuz dalları iptali açıklıyor. `recalculate` yeni emirlerin tahmini değiştirebildiğini dürüstçe söylüyor.

238 px metin alanı, body14/small13/heading21 ve584 px görünür scroll yüksekliği var. `Paragraph` CalcHeight ile büyür; asıl risk yazının kesilmesi değil, karar kuralı ve veto düğmesinin çok aşağıda kalmasıdır. Portre yanındaki66 px yazar alanı şu anki kısa RU/TR iki satıra yeterli görünüyor. Alt bildirim817 px geniştir; kısa tarihli metin için bariz yatay taşma görünmüyor.

## Üç öncelikli dar iyileştirme

1. **Cevabın gerekmemesini tarihin yanında söyle.** Şu anda bu kritik kural veto düğmesinden de sonra `no_answer` içinde. `due` ve `no_answer` tek erken paragrafta birleştirilebilir; alttaki tekrar kaldırılır. Önerilen RU: «Проверка запасов — {0}, при расчёте недели. Ответ необязателен: закрытие донесения приказ не отменяет.» TR: «Stok kontrolü {0}, hafta hesabında. Cevap şart değil; raporu kapatmak emri iptal etmez.» Başlık ve ReasonKey zaten gelecek hafta ne olacağını anlattığından bilgi kaybı olmaz; tarih ve kapanmanın anlamı korunur.

2. **Gather dalındaki tekrarı çıkar, sayı ve sonuçları tut.** `dumas.reason.gather`, LedgerLine ve `payroll_separate` aynı toplama/miktar/açığın kapanması fikrini üç kez anlatıyor. En dar kod seçeneği: `Disposition==gather` için uzun ReasonKey paragrafını atla, LedgerLine/FoodGathered + local/political etkiler + payroll_separate açıklamasını koru. Yer zaten `camp` satırında var. Diğer üç iptal dalında ReasonKey aynen kalmalı. `intervene_body` de kısalabilir: RU «Если обычного снабжения станет достаточно, Дюма отменит сбор сам.»; TR «Normal ikmal yeterli hâle gelirse Dumas toplamayı kendisi iptal eder.» Ekonomi defterini tekrar tarif eden ikinci cümle kaldırılabilir. Portre zaten adı verdiği için alıntı sonundaki ikinci «— Люсьен Дюма / — Lucien Dumas» imzası da gereksiz; birinci şahıs sesi korunmalı. Bu değişiklikler yaklaşık100–180 px kadar tekrar/padding tasarrufu sağlayabilir; kesin kazanç gerçek karede ölçülmelidir. Her şeyi584 px içine sıkıştırma hedefi önerilmez.

3. **Veto'nun bu tek emre ait olduğunu belirginleştir.** `Запретить этот сбор / Bu toplamayı yasakla` mevcut «Запретить сбор»a göre kapsamı daha açık anlatır;238 px düğmeye sığması beklenir. VetoRelationshipDelta ve gıda açığının otomatik çözülmeyeceği açıklaması düğmenin hemen üstünde kalmalı. Forecast zaten iptal gösteriyorsa (sufficient/too_large/no_army), `intervene_body` koşullu olarak atlanabilir; aynı paragrafı tekrar tekrar anlatmaya gerek yok. Mevcut yetkili veto imkânını sessizce kaldırmak önerilmez: oyuncu isterse emri şimdi kapatabilir, fakat bunun ilişki bedelini görmelidir.

İlk mevcut metinde bariz yanlış mekanik iddiası saptanmadı. Öncelik, «bir NPC emri kendiliğinden uygulayacak» bilgisini belge girişine almak ve maliyet/tek veto yoluna kadar yinelenen metni azaltmaktır. Gather, normal-supply iptali ve too-large dallarının gerçek üst/alt scroll RU/TR kareleri henüz gereklidir.

## Gerçek TR kareleri — 6 Eylül 2026

Root önerilen kısaltmaları uyguladı. `output/verify/dumas-labels-first-20260906-021758-659-0da55b25/shots` içindeki `04-warning-tr`, `05-warning-terms-tr`, `07-forage-economy-tr` tam1440×900 görüntüleri incelendi. Bu gerçek gather örneği Île-de-France, 30 Haziran / kontrol7 Temmuz1789, beklenen toplama+36'dır.

- 04'te portre/alıntı, gerçek tarih, cevap zorunlu olmadığı ve belgenin kapatılmasının emri iptal etmediği açıklaması, hareket eden ordunun mevcut bölgesi, +36 ve yerel/siyasi bedeller ilk görünümde okunur. Metin yatay kesilmiyor; portre/kimlik alanı taşmıyor.
- 05'te mevcut scroll ile şartlar ve bütün müdahale bölümü görünür. Normal ikmalin emri iptal edebileceği, tahminin oyuncu emirlerine göre değiştiği, veto ilişki−4 ve gıda sorumluluğu düğmeden hemen önce açık. `Bu toplamayı yasakla` düğmesi yaklaşık y713–757'de tamamen görünür ve alt kenarla sıkışmaz. Veto, bütün sonraki girişimleri iptal eden kalıcı bir kural gibi sunulmuyor.
- 07 ekonomi tahmini +36 toplamayı ayrı satır olarak, halk−110 / ordu−62 / Paris desteği−20 ve haftalık net+0 yanında gösterir. Gelecek hafta tahmini olduğu üst belge ve tarih bağlamında korunur; bunu gerçekleşmiş kaynak eklenmesi olarak raporlamıyoruz. Alt ekonomi metni mevcut doğal scroll içinde devam eder; beklenmeyen kesilme veya buton örtülmesi görülmedi.

Bu üç kare için ek kısaltma/düzen değişikliği gerekmedi. Diğer iptal dallarının bütün olası uzunlukları bu üç gather karesinden kanıtlanmış sayılmaz. Root'un RU ve alt-veto incelemesi ayrı kabul kanıtıdır.
