# Direnç: verifier dış çıkışı ve yeni gerçek savaş kanıtı

6 Eylül 2026. Bu belge salt okunur artifact/kaynak denetimidir. Bu ajan test, derleme, helper veya oyuncu başlatmadı; mevcut receipt/log dosyalarını değiştirmedi.

## GREEN artifact ile dış exit1 ayrımı

Root'un çalıştırdığı `resistance-details-complete-20260906-045150-217-15f6da43` için root şu dış gözlemi bildirdi: exec session63179, son write_stdin sonucu exit_code1 ve boş output. Build'e kadar önceki stdout normaldi. Bu ajanın elinde o host işlemine ait stderr veya exception stack kaydı yoktur.

Doğrudan dosyalardan doğrulananlar:

- EditMode XML:397 toplam/397 geçmiş/0 failed ve397 gerçek test-case.
- Browser XML:10 testcase, failure/error/skipped0.
- Player protocol: success=true,108 komut,19 assertion,9 PNG,6 JSON, boş failures; tamamlanış `2026-09-06T04:52:15.8803333Z`.
- Frames process receipt exit0, tamamlanış `04:52:23.9313634Z`; frames.stderr0 byte. Edit/build/player/frames/stdErr loglarında mevcut dar Assert-CleanLog hata işaretlerinden0 adet bulundu.
- REPORT ve result.json GREEN, altı gate PASSED, failures boş, süre34 saniye. Runtime build hash `2B161F6B587164DD17C1CC4231566C850F89B4CEFE609DB063D8C54D377CBE3A` olarak raporlandı.

Kaynak sınırı: `verify.ps1` finally içinde önce REPORT ve result.json yazar; ardından `Say` ile `Write-Output` çağrısı, finally dışında verdict RED kontrolü ve `exit 0` vardır. Bu nedenle GREEN dosyasının varlığı tek başına son exit0 satırının gerçekten yürüdüğünü ispatlamaz. Burada Say'ın hata verdiği de gözlenmedi; bu bir kök neden iddiası değildir. Eldeki dosyalar dış exit1'in nedenini açıklamaz. Kaydedilmiş gate sonuçları ve dış araç çıkışı ayrı tutulur; mevcut sonuç yeniden yazılmaz veya dış anomali yok sayılmaz.

## Yeni doğal savaşın ilk snapshot'ı

Koşu `resistance-natural-victory-20260906-045323-372-ede571f9`, aynı yeni build. İlk incelemede result.json ve shots-result.json henüz yoktu; aşağıdakiler yalnız02-deployment snapshot'ıdır, tamamlanmış savaş iddiası değildir.

Arrival artık gerçek JSON'da bulunur: FoodCost18, FoodAfter342, MilitarySuppliesAfter115, MovesAfter0, Supply88, Fatigue20, Morale78, Difficult=true, Hungry=false. Önceki deployment evidence'deki eksik Arrival alanı bu yeni snapshot'ta giderilmiştir.

Battle OriginalTroops1200; dört oyuncu Original toplamı1200. EnemyOriginalTroops1114; düşman Original değerleri356+311+245+202=1114. Snapshot pause edilmiş, HasOutcome=false. Tam sonuç ve arşiv karşılaştırması tamamlanan artifact geldikten sonra aşağıya eklenecektir.

## Tamamlanmış doğal zafer ve bağımsız dönüş karşılaştırması

Sonradan result/protocol oluştu: `resistance-natural-victory` PARTIAL, native exit0,141 saniye. Player success69 komut/15 assertion/9 PNG/8 JSON, boş failures; protocol tamamlanışı `2026-09-06T04:55:35.6064051Z`. Frames PASS ve BuildUnchanged141 dosya raporlandı; yeni kaynak/test/browser bu player-only koşuda tekrar çalıştırılmadı.

`06-natural-outcome.json`: gerçek Ended=true / HasOutcome=true / Won=true, Elapsed125.8030777, PlayerHold45 / EnemyHold0. Oyuncu Original1200, EnemyOriginal1114 ve düşman alaylarının Original toplamı1114. Gerçek Casualties196, kalan oyuncu Men toplamı1004, recovered24. Bu kez1114 düşmanlı yeni snapshot'ta da196 ve125.803 gözlendi; eski kayıp veya saat sayısını script'e yazıp sonuç üretme söz konusu değildir. EndingMorale56.9962425 ve CampaignReturnMorale59.9962425.

`08-campaign-return.json` ile **artık gerçekten mevcut Arrival** bağımsız karşılaştırıldı: Troops1200−196=1004, Food342=Arrival.FoodAfter, Supplies115+24=139, Moves0=Arrival.MovesAfter; dönen moral59.9962425 raporla aynı. Ordu ve seçili bölge Champagne; Week0, Gold840, Manpower2400, Power59, Fatigue35, Supply88. Tek resolved ve pending victory kimliği `battle-0-2-ile-champagne`. Dumas Loyalty60/Relationship52/Ambition83; Champagne Unrest49 / Control70.5. Gözlenen kayıp, stok, hareket, moral ve battle ID karşılaştırmalarının hepsi eşleşti.

`08-campaign-return.json` ve `09-loaded-return.json` raw byte-eşit: SHA256 `FC887B89EB3B5E8001AF05D9BE75C24910ECFD7CF3592517A6D6F309D5E06971`. Bu yeni doğal sonuç ve açık zafer gerçek save/load sonrasında korundu. Bütün bunlar yeni player-only koşunun kanıtıdır; önceki full verifier dış exit1 anomalisinin nedenini açıklamaz.
