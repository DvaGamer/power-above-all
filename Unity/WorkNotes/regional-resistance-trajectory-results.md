# 24 haftalık direniş: gerçek Core probe sonuçları

Kaynak: `output/core-probes/RegionalResistanceTrajectoryProbe-2026-09-06T04-42-40-070Z-c47ff7f7/probe.stdout.log` ve aynı klasörün `result.json` kaydı. Root'un çalıştırdığı probe **1631 check PASS**, compile/probe exit0;04:42:40.070–04:42:40.918 UTC. Bu belge yalnız hazır çıktıları okur; yeni çalıştırma yapılmadı. Receipt açıkça `unityOrPlayerVerified:false` der. Core kaynak hash'i `fbb43bb1b60986a6c23a00f2019a8b3afd1159eb84577721ba05ec1c625d0cb0`, direnç partial hash'i `0011c005397b406fa53cb3fa99027093143809bf9b0ebfa53bf43becdb001b2f`.

İki legacy başlangıcı, aynı hafta2 negotiate; ordu sürekli Ile'de. Tek tercih farkı, ikinci rotada hafta0 budget1000.24 hafta boyunca vergi, ekmek, sübvansiyon, savaş, alım veya başka gönüllü müdahale yok. Bu pasif yönetim sınırıdır; makul oyuncunun tek davranış biçimi değildir.

## Bölgeler ve65 eşiği

İlk düşmanlık haftaları iki rotada **aynı**. Başlangıçta yalnız Champagne; Provence hafta7, Lorraine9, Languedoc11, Picardy/Poitou12, Brittany14, Guyenne15, Burgundy16, Normandy18, Orleans19. Hafta19'dan itibaren11/12 bölge düşman; Ile24 haftanın tamamında sakin. Geriye doğru eşik geçişi yok.

U/C/E, gerçek Unrest/Control/EliteLoyalty'dir. Aşağıdaki iki sağ durum hafta24'e aittir; kuvvetler aynı bölgesel formülün çıktısıdır.

| Bölge | İlk düşmanlık haftası | Campaign U/C/E | Campaign düşman | Budget1000 U/C/E | Budget düşman |
| --- | ---: | --- | ---: | --- | ---: |
| Brittany |14 |100/43/60 |1418 |86/43/60 |1318 |
| Normandy |18 |94/59/60 |1680 |78/59/60 |1526 |
| Picardy |12 |100/35/60 |1414 |90/35/60 |1346 |
| Ile |— |30/100/60 |0 |14/100/60 |0 |
| Champagne |0 |100/0/60 |1800 |100/0/60 |1800 |
| Lorraine |9 |100/23.5/60 |1559 |95/23.5/60 |1523 |
| Burgundy |16 |97/51.5/60 |1614 |81/51.5/60 |1475 |
| Orleans |19 |91/63.5/60 |1156 |75/63.5/60 |1045 |
| Poitou |12 |100/35.5/60 |1166 |89/35.5/60 |1103 |
| Guyenne |15 |99/47.5/60 |1838 |83/47.5/60 |1685 |
| Languedoc |11 |100/31/60 |1756 |92/31/60 |1688 |
| Provence |7 |100/15/60 |2228 |100/15/60 |2228 |

Her iki rotanın son ve bütün24 haftalık en büyük kuvveti **Provence2228, hafta24**. Başlangıç maksimumu Champagne1114'tür. Champagne1800'e hafta21'de ulaşır ve kalır; bu kuvvetin bütün bölgeler için genel üst sınırı değildir. Provence'ın hafta23 kuvveti campaign2198, budget2178; hafta24'te U100 sınırına ikisi de gelince fark kaybolur.

## Politika neyi değiştirdi?

| Hafta24 | Campaign | Budget1000 |
| --- | ---: | ---: |
| Canlı asker |1015 |1000 |
| Yedek insan |2400 |2600 |
| Gerçek asker kaybı |185 |0 |
| Terhisle yedeğe geçen |0 |200 |
| Gold |1936 |2292 |
| Food |0 |61 |
| MilitarySupplies |312 |334 |
| Power |56 |67 |
| Açlık / ödenemeyen hafta |2 /0 |0 /0 |
| Dumas'ın gerçekten topladığı Food |0 |0 |

Budget hafta2 hesabından **sonra**200 kişiyi yedeğe geçirdi; o hesabın maliyeti hâlâ136 Gold/40 Food. Hafta3'ten itibaren120/34. Campaign hafta23'te96, hafta24'te89 kişiyi açlıktan kaybetti. Hafta24 planındaki Dumas sonucu `too_large`; o haftanın gerçek NetFood değeri−41 ve toplama0. Bu çıktıda aday yerel zararlı hesabın kesin shortfall sayısı yazılmadığından41'i NPC'nin ayrı aday ihtiyacı diye sunmuyoruz.

Budget'ın Gold üstünlüğü356:21 hesapta16 ve son hesapta8, toplam344 gerçek ordu tasarrufu; son haftanın124 yerine136 vergi geliri ayrıca12 fark yaratır. Food61 ve Power+11 farkı da açlığa girilmeyen gerçek sonuçlarla birlikte okunmalıdır; yalnız bir başlangıç bonusu değildir.

## Nedensellik ve denge sınırı

Başlangıç urban approval35, negotiate onu artırmıyor. Bu müdahalesiz rotada eski hafta kuralı garnizonsuz bölgelere+2 Unrest veriyor;65'e ulaştıktan sonra Control her hafta3 düşüyor. Bu yüzden orduyu200 azaltmak, açlık başlamadan **aynı bölgesel göstergeleri ve aynı isyan tarihlerini** üretir. Yeni formül bunları asker sayısına bağlayan otomatik bir çarpan eklememiştir.

Campaign'ın son iki aç haftası ayrıca toplam+16 Unrest verir;100 sınırına ulaşan bölgede gerçek fark daha küçüktür. İsyanların hepsi bu haftalardan önce başladığı için iki rotanın final Control değerleri eşittir. EliteLoyalty bütün bölgelerde60 kalır: hiçbir Dumas toplaması veya elit değiştiren eylem uygulanmamıştır. Dolayısıyla sağdaki kuvvet farkları bu çalıştırmada **yalnız gerçek Unrest farkından** gelir; Control ve elite etkilerinin bağımsız büyüklüğünü bu rota deneysel olarak ölçmez.

Somut denge riski: maliyet düşürmek asker ve yiyeceği korurken tek başına ülkeyi sakinleştirmiyor. Hafta19'da11 düşman bölgeye ulaşılması, yönetimi tamamen bekleten oyuncu için güçlü bir yayılma baskısıdır. Bunun aşırı olup olmadığını bu iki pasif rota tek başına kanıtlamaz; mevcut siyasi araçları kullanan aynı uzunluktaki rotalarla karşılaştırmak gerekir. Bu rapor yeni sayı değişikliği önermiyor.2228 kişinin savaşta ne kadar zor olduğu,1000/1015 askerin kazanıp kazanamayacağı veya gerekli kayıp sayısı gerçek savaş olmadan çıkarılamaz. Salt okunurluk,24 başarılı hesap ve final archive eşitliği PASS'tır; UI/oynanış başarısı iddiası değildir.
