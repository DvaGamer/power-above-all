# Dumas — komşu portre kırıntısının UV sınırı

Kaynak `PoliticalPortraits-v1.png` gerçekten açıldı. PNG dosyası veya import ayarları değiştirilmedi. `portrait-alpha-audit.py` yalnız standart Python modülleriyle PNG alfa kanallarını okuyup bağlı adaları ölçer; dosya yazmaz, görüntü oluşturmaz. İlk Pillow denemesinde modül olmadığı için yeni bağımlılık kurulmadı; okuyucu standart kütüphaneye çevrildi.

Gerçek sheet **1254×1254**; her hücre 627×627. Dumas sağ alt hücrede, eski UV `(.5,0,.5,.5)` kaynak x627–1253, üstten y627–1253 alanını alır. Sol sınır, komşu kahverengi ceketli portreye ait ayrı bir adayı içerir:

| Alfa eşiği | Ayrı kırıntı (üstten kaynak koordinatı) | Dumas'nın ana bağlı siluetinin sol sınırı |
| --- | --- | --- |
| ≥1 | 487 piksel, x627–641 / y1024–1079 | x677 |
| ≥16 | 242 piksel, x627–637 / y1026–1068; sınırda küçük ek kuyruk | x680 |
| ≥64 | 217 piksel, x627–636 / y1027–1067 | x680 |
| ≥128 | 202 piksel, x627–636 / y1028–1067 | x681 |

Bu yer, zafer belgesindeki soldaki yaklaşık x1034 / y240–246 çizikle uyuşur. Yalnız bu kaynağın sol 17 pikselini çıkarmak güvenlidir: yeni sol x644, sağ1254, genişlik610. Ana siluete en az33 kaynak piksel mesafe kalır. Bu işlem sheet üzerindeki bütün düşük alfa artıkları temizlendi anlamına gelmez.

Root onayıyla sadece üç çizim fonksiyonu değiştirildi:

- `CabinetHud.Seal`: yalnız variant3, mevcut aspect-fit tamamlandıktan sonra.
- `MandateDocument.DrawPortrait`: yalnız index3, aynı fit sonucu üzerinde.
- `RoleSelection.Portrait`: yalnız quadrant3; önceki bounds/oran korunur.

Her yerde `trim=17/sheet.width`; önce hedef dikdörtgenin xMin'i `target.width × trim / eskiUV.width` kadar artırılır, sonra UV.xMin'e trim eklenir. Sağ kenarlar, yükseklik, kalan resmin ölçeği ve konumu aynı kalır. Son UV `(644/1254,0,610/1254,.5)`; mevcut 89 px fitted portrede yalnız sol2.413 px örnekleme alanı çıkar, Dumas hareket etmez veya büyümez. Diğer portreler ve çevre UI davranışı değiştirilmedi.

Kaynak parçaları tekrar okundu; crop için Unity/player/derleme çalıştırılmadı. Yeni gerçek ekran görüntüsü root tarafından doğrulanmalıdır. Battle smoke ikinci görünürlük adayıyla birlikte kaynak donduruldu.

## Gerçek sonuç

`military-art-final-20260906-012710-424-48b0deff/shots/08-pending-choice-ru.png` ve `09-pending-choice-tr.png` açıldı. Önceki yaklaşıkx1034/y241 çizik artık görünmüyor; Dumas'nın saç, omuz ve üniforma silueti, ölçeği ve yerleşimi aynı. Zafer belgesinde crop kabul edildi. Bu iki kare `Seal` yolunu gösterir; RoleSelection ve MandateDocument çizim yollarının yeni gerçek kareleri bu koşuda ayrıca görülmüş sayılmaz.
