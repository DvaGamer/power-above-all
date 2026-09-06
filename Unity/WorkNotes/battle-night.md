# Gece savaş sunumu — 6 Eylül 2026

- Başlangıç kanıtı: `output/polish-shots/12-battle-ru.png`, `13-volley.png`, `14-battle-tr.png`, `16-report-ru.png` gerçekten incelendi. Ana ve izole `TacticalBattle.cs` aynıydı. Alan küçük, zemin tek renk, dere düz şerit, ağaçlar eş küreler; Rusça süvari satırı taşıyordu.
- İlk kaynak paketi: güneşli guaj paleti; elle yerleştirilmiş geniş ışık/serinlik alanları ve hafif gren içeren tek 256×256 arazi dokusu; belirgin mat su, üç loblu bahçe taçları, ince fildişi diorama tabanı. Doku ayrı temizlenir. Doku ve dekor savaş RNG'sini kullanmaz.
- Gerçek dere/orman/tepe kuralları, başlangıç konumları, simülasyon, hasar, zamanlama ve sonuç hesabı değiştirilmedi. Su kıyısı eski gerçek dere şeridinin içinde; ağaç gövdeleri gerçek bahçe alanının içinde.
- Kadraj: ortak `ViewLayout.BattleViewport` (0, .19, 1, .77), ortografik boyut31. Üst belgeler `(20,42,640,91)` ve `(934,42,485,91)`; duraklat `(1230,49,176,32)`, çekil `(1230,87,176,32)`. Alan tıklaması y142–729; alt komut şeridi y738'de. Root üst36px dil alanını yönetiyor.
- Alt şeritte seçili polk açık kâğıt, geçerli düzen/ateş emri pirinç ve çizgiyle belirgin. Küçük metin13px; süvarinin ayrı kısa satırı RU/TR taşmasını önler. Diğer emirlerin etkinlik şartları aynı.
- Kaynak değişikliği hazır olduğunda root'a aktarılır. Bu not **yeni Windows derlemesi, ekran kabulü veya test başarısı iddiası değildir**. Ajan Unity/build/player başlatmadı. Yeni karelerde özellikle kuzeydeki birlikler, mavi/mercan ayrımı ve iki dilde alt şerit kontrol edilecek.
- İlk paket statik kontrolü: `git diff --check` temiz; `python3 -m json.tool Unity/Assets/Resources/Localization/battle.json` çıkış0. Root'un ortak `BattleViewport` sabiti kaynakta mevcut. Derleme ve gerçek yeni görüntü kontrolü root kapısını bekliyor.

## İkinci inceleme

- Gerçek yeni kareler: `output/verify/painted-atlas-review-20260905-220606-170-49890db5/shots/15-battle-ru.png`, `17-battle-tr.png`, `19-report-ru.png` incelendi. Alan daha büyük, bütün başlangıç birlikleri görünür, süvari ve seçili emir satırları okunuyor. Sol dış kenarda neredeyse siyah gölge, RU üst açıklamanın ikinci satırında kesilme var. Root altta729–738 atlas artığını düzeltiyor.
- Küçük takip düzeltmesi: yalnız diorama kenarı/tabanı gölge atmaz ve almaz; arazi/askerler korunur. Üst sol belge99px ve açıklama37px yüksekliğe çıkarıldı; alan tıklaması142'de kalır. Root ortam ışığını ayrıca sabitleyecek; renkleri tekrar değiştirmedim.
- Ses kod denetimi, dinleme değildir: üç ses × .65 PCM sınırı × .42 en yüksek kazanç = .819 teorik toplam üst sınır. Susturma mevcut sesleri kesiyor, bitiş koruması ikinci sonuç çağrısını engelliyor.
- Düzeltme: `CabinetAudio` tek zafer/yenilgi sesini en çok .75s bekletir; olağan sesler bu sırada yeni yuva işgal etmez. Aynı bekleyen sonuç yinelenmez; mute/disable/destroy veya süre aşımı kuyruğu siler. Havuz büyümez, çalan klip kesilmez. Manuel çekilmenin mevcut yürüyüş sesi eşlemesi değişmedi.
- Biten savaşın henüz başlamamış yaylım sesleri artık tetiklenmez; dumanın mevcut görsel akışı ve simülasyon zamanlaması korunur. Yeni paket henüz derleme/test/işitsel kabul görmedi.
