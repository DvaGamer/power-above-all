# Sistem–referans matrisi

Uygulanan ilk kesitin matrisi; 30–50 kartlık bütün kürasyonun tamamlandığı iddiası değildir.

| PAA sistemi | Primary | Secondary | Contrast | Negative | Ders | Current implementation | Missing polish |
|---|---|---|---|---|---|---|---|
| Dünya atlası | [Natural Earth](Visual/World_Map/Natural_Earth_Layers.md) | [Bailliages](Historical/Maps/Bailliages_1789.md) | Doğruluk / sadeleştirme | [Kendi atlas incelemesi](Design_Lessons.md) | Fiziksel, siyasi, oyun bölgesi ayrı | Global110m/10m + Fransa | Yakın çizgi, komşu sınır kaynakları |
| Tarihli bilgi | [Radio General](Gameplay/Campaign/Radio_General_Reports.md) | [Young](Literature/Primary_Sources/Young_Missing_Letters.md) | [Jefferson–Morris](Literature/Primary_Sources/Jefferson_Morris_Channels.md) | Kamerayla bilgi yenilemek | Bilgi yazar ve gözlem taşır | Bordeaux snapshot/tarihler | Tüm ülke, farklı güven kaynakları |
| İdari emir | [SoW](Gameplay/Combat/SOW_Courier_Command.md) | [Jefferson–Morris](Literature/Primary_Sources/Jefferson_Morris_Channels.md) | [Command Ops](Gameplay/AI/Command_Ops_Delay.md) | Boş zamanlayıcı | Taahhüt/yerel yorum/dönüş | Dört niyet, iki yetki, iki kurye hızı | Öğretim; genel direktifler yok |
| Siyasi bedel | [Campan](Literature/Memoirs/Campan_Antechamber.md) | [Young](Literature/Primary_Sources/Young_Missing_Letters.md) | Resmî güç / sosyal nüfuz | Sadece +3 etiketi | Sonuç başka insanları etkiler | Delmas zor kullanırsa Paris tepkisi/hırs | Raporda küçük insan ayrıntısı |
| Emir kuyruğu | [SoW](Gameplay/Combat/SOW_Courier_Command.md) | [Flashpoint](Gameplay/Combat/Flashpoint_Command_Context.md) | [Command Ops](Gameplay/AI/Command_Ops_Delay.md) | [Çıkışsız emir](Negative_References/Orders_Without_Exit.md) | Eski emir sürer | İki FIFO; tekrar süreyi sıfırlamaz | Kurye/sinyal ritmi |
| HQ / inisiyatif | [Flashpoint](Gameplay/Combat/Flashpoint_Command_Context.md) | [SoW](Gameplay/Combat/SOW_Courier_Command.md) | HITS kamera kilidi alınmaz | [Çıkışsız emir](Negative_References/Orders_Without_Exit.md) | Güvenlik / müdahale dengesi | Fizikî HQ, reserve/flank | Bağlantı overlay, çeşitli temas senaryoları |
| Belge | [Young](Literature/Primary_Sources/Young_Missing_Letters.md) | [Campan](Literature/Memoirs/Campan_Antechamber.md) | Harita sürekli görünür | İşlevsiz süs | Yazar/yer/tarih işlevseldir | RU/TR scroll, rapor ve taslak | Küçük ekran metni, taslak ayrımı |

Audio/music/combat presentation/olay zincirleri kapsamlı karşılaştırması araştırma kuyruğundadır. Eksik kanıt başarı diye doldurulmaz.
