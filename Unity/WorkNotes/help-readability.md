# Yardım ve asker bakım önizlemesi — okunurluk

Durum: salt okunur inceleme. `CabinetHud.Help`, `ui.help.body` RU/TR, `recruit-preview.json`, `CabinetHud.Observe/Order`, gerçek girdi işleyicileri ve aşağıdaki kareler incelendi. Assets değiştirilmedi; Unity/oyuncu başlatılmadı. Savaş sanatı planına yeni iş eklenmedi.

## Gerçek kare sonucu

- `output/verify/native-input-20260906-000441-029b6bc7/shots/01-help-ru.png` ve `02-help-tr.png` açılıp görüldü. Başlık, beş maddelik sefer akışı, savaş tuşları ve tarihî/sadeleştirilmiş içerik açıklaması iki dilde bütünüyle görünüyor. RU son satırı yaklaşık y545, TR yaklaşık y529; gövde alanı y259–679. Yaklaşık 130–150 px kullanılabilir pay var. Bunlar görüntü üzerinden yaklaşık konumlardır, Unity font ölçümü değildir.
- 590 × 420 içinde metin kesilmiyor; yazı boyutunu küçültmek, kaydırma eklemek veya kapatma düğmesini taşımak gerekmez. Üst küçük talimat satırı → büyük serif başlık → normal açıklama → numaralı akış → ayrı savaş paragrafı hiyerarşisi yeterli. Beş maddenin sık dizilmesi blok görünümü veriyor ama bugünkü uzunlukta okunurluk kusuru oluşturmuyor.
- `output/verify/upkeep-help-20260906-000313-242-91cc7cbc/shots/01-atlas-ru.png` ve `02-atlas-tr.png`: alımın ilk maliyeti ile ek haftalık `−17` livre / `−7` gıda ayrı satırlarda okunuyor. Ordu/yürüyüş ve Paris desteği düğmeleri hâlâ görünür. Alt bölge raporu daha kısa; denetim ve huzursuzluk görünür, devamının kaydırılabildiği çubuk açık. Çakışma yok.
- Ek bakım gerçek önerilen alımdan önce/sonra `Forecast` farkıdır; yardımda 17/7 gibi sabit denge rakamları yazılmamalı. Şimdiki açıklama doğru biçimde oyuncuyu alım önizlemesine ve «Счета / Hesaplar»a yönlendiriyor.

## Öneri

**Mevcut ekranı koru.** Çerçeve veya metin sığdırma düzeltmesi gerekmiyor. Root'un sonraki yeni sistemleri nedeniyle metin büyürse aşağıdaki kısa taslak bir yedektir; mevcut iyi kadraja sırf kozmetik amaçla uygulanması önerilmez. Rolün seçildiği yer, imzanın ayrı olması, vergilerin tepkisi, tekrar eden asker gideri, komşu yürüyüş, dilekçe/söz vadesi ve mevcut savaş kontrolleri korunur.

RU yedek gövde:

```text
Франция, май 1789. Сохраняйте личную власть, распоряжаясь хлебом, налогами и армией.

1. Роль — в новой партии. «Мандат» показывает условия помощи; обещание возникнет только после подписи.
2. Выберите область: приказы и положение — слева. Хлеб снижает волнения; сбор даёт деньги ценой сопротивления.
3. Набор стоит ресурсов сразу и увеличивает расходы каждую неделю. Проверьте прибавку и прогноз в «Счетах».
4. Идите в соседнюю с армией область. При высоких волнениях будет бой.
5. Завершайте недели, отвечайте на прошения. В срок выполните обещание или откажитесь — покровитель отреагирует.

В бою: 1–4 — полк; Shift с выбором — несколько; ПКМ — движение; Пробел — пауза для приказов. Берегите строй и мораль. Без патронов пехота может сражаться вблизи.

Эпоха и названия исторические; границы, советники и расчёты упрощены или вымышлены.
```

TR yedek gövde:

```text
Fransa, Mayıs 1789. Erzak, vergi ve orduyu yönetirken kendi iktidarınızı koruyun.

1. Rolü yeni seferde seçin. «Görev» yardım koşullarını gösterir; sözünüz ancak imzayla başlar.
2. Bir bölge seçin: durumu ve emirler solda. Ekmek huzursuzluğu azaltır; vergi, direniş pahasına para getirir.
3. Asker toplamak hemen kaynak harcar, haftalık gideri de artırır. Ek tutarı ve «Hesaplar» öngörüsünü kontrol edin.
4. Orduya komşu bölgeye yürüyün. Yüksek huzursuzluk varsa muharebe başlar.
5. Haftaları ilerletin, dilekçeleri yanıtlayın. Vadesinde sözünüzü tutun veya bozun; destekçiniz tepki verir.

Muharebe: 1–4 alay seçer; Shift ile seçim çoklu seçimdir; sağ tık hareket; Boşluk emirler için duraklatır. Düzeni ve morali koruyun. Cephanesiz piyade yakın dövüşebilir.

Dönem ve yer adları tarihseldir; sınırlar, danışmanlar ve hesaplar sadeleştirilmiş veya kurgusaldır.
```

Yedek taslak canlı arayüzde ölçülmedi ve kabul edilmiş kaynak değildir. Shift burada tek başına bir düğme değil seçimle kullanılan tuştur; mevcut kod hem sayı tuşlarıyla eklemeyi hem fareyle çoklu seçimi destekler. M ve F5/F9 atlas alt satırında zaten görünür; onları bu gövdeye çoğaltmak mevcut gereksinim değildir.
