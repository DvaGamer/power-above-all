# Denge gözlemi — 24 haftalık mevcut çekirdek

Bu sayılar kurgusal oyun modelinin ölçümüdür; tarihsel veri veya yeni denge kararı değildir. `CampaignBalanceProbe.cs`, kurulu Unity'nin Mono/mcs araçlarıyla gerçek `CampaignCore.cs` ve `CampaignRoles.cs` kullanılarak çalıştırıldı. Unity/oyuncu açılmadı, oyun kaynakları ve kişisel kayıt değişmedi.

Koşu sırasında HEAD `19e5fa8d9dfc9576606e444bfb60a7311b32ec50`; rol kaynakları henüz bu commit'in parçası değildir. SHA256 kanıtları:

- `CampaignCore.cs`: `03de1458987befaef8c58af98bb09976587be07f9944c33c650c09a0cd0a0bce`
- `CampaignRoles.cs`: `8fb289f75d17ceae08b25ff3139f6d2b61f2c7f7c3ae58c23b0b53b4b5e796bf`
- `CampaignBalanceProbe.cs`: `115265dfc2d964b5edb91a915eefe881f40c6c165d77757d2cc949fdefe408a4`

İkili çıktı: `output/CampaignBalanceProbe.exe`. Salt bu EXE'yi kurulu `MonoBleedingEdge/bin/mono.exe` ile çalıştırmak bütün haftalık kontrol noktalarını tekrar yazdırır; gelecekte değişen kaynakla yeniden derlemek aynı denge sürümü sayılmaz.

## Yöntem

10 politika, 24 hafta; ayrıca ayrıcalıksız aynı politikanın crown/assembly/army/legacy karşılaştırması. Dört rolün25 haftalık ölçülen durumları eşit çıktı. Bu, başlangıç kaynaklarında gizli rol avantajı olmadığını doğrular.

- Dilekçe ikinci haftada relief veya negotiate ile çözülür. Ayrıcalıklar0/4/8/12/16/20 haftalarında verilir,2/6/10/14/18/22 haftalarında tutulur/bozulur. Hiçbir ödeme zorunlu olarak bozulmak zorunda kalmadı.
- Meclis her defasında en huzursuz bölgeyi hedefler; saray avansı ve sabit ordunun zor alımı Paris'e bağlıdır. Bu çalışmada yürüyüş, savaş, normal olağanüstü vergi ve ayrı ekmek emirleri yoktur.
- Paris yardımı ilk hafta açılır. Genişleme politikasında iki haftada bir200 asker denenir; yalnız gerçek `Act` onayıyla alınır. Bu sabit plan tahmin edilen kıtlıkta yardımı geri çekmez; bütün oyuncu stratejileri için kaçınılmaz yenilgi örneği değildir.
- Kontrol noktası, o haftanın dilekçe/borcu çözüldükten sonra ve yeni emirlerden öncedir. Tabloda12→24 hafta karşılaştırılır. Her hafta `Validate` çağrıldı; durum değerleri doğrudan değiştirilmedi.

## Ölçülen sonuçlar

| Politika | Hazine12→24 | Gıda12→24 | Asker24 | İktidar12→24 | Ort. huzursuzluk12→24 |
| --- | --- | --- | --- | --- | --- |
| Ayrıcalıksız, relief | 1889→3079 | 369→463 | 1200 | 61→67 | 35,2→34,5 |
| Ayrıcalıksız, negotiate | 1648→1936 | 271→0 | 1015 | 61→56 | 62,3→92,6 |
| Saray, borçlar tutulur | 1799→2899 | 369→463 | 1200 | 61→67 | 35,2→34,5 |
| Saray, borçlar bozulur | 2249→3799 | 369→463 | 1200 | 43→31 | 35,2→34,5 |
| Meclis, sözler tutulur | 2006→3556 | 281→329 | 1200 | 61→67 | 30,7→25,5 |
| Meclis, sözler bozulur | 1627→2158 | 377→479 | 1200 | 49→43 | 35,2→34,5 |
| Ordu, tazminatlar ödenir | 1620→2539 | 485→692 | 1200 | 61→67 | 35,9→34,5 |
| Ordu, tazminatlar reddedilir | 1763→2581 | 472→643 | 1200 | 46→37 | 40,2→42,2 |
| Paris yardımı, asker alınmaz | 2048→3617 | 187→220 | 1200 | 61→67 | 24,4→13,4 |
| Paris yardımı + sabit genişleme | 895→781 | 0→0 | 480 | 33,5→0 | 73,1→100 |

24. hafta siyasi destekleri, sırayla **kraliyet / meclis / kentliler / ordu**:

| Politika | Destekler | İlgili kişisel ilişki |
| --- | --- | --- |
| Ayrıcalıksız relief | 65 / 50 / 50 / 84 | Dört kişi50 |
| Ayrıcalıksız negotiate | 57 / 57 / 35 / 68 | Dört kişi50 |
| Saray tut / boz | 77 / 50 / 50 / 84 — 0 / 50 / 50 / 84 | Valcourt74 / 0 |
| Meclis tut / boz | 65 / 62 / 50 / 84 — 65 / 0 / 50 / 84 | Morel74 / 0 |
| Ordu öde / reddet | Her ikisinde65 / 50 / 50 / 84 | Dumas74 / 14 |
| Paris yardımı / genişleme | 65 / 50 / 100 / 84 — 65 / 50 / 0 / 0 | Lefevre74 / 57 |

## Baskı ve toparlanmanın kaynağı

1. **Ekmek dilekçesi güçlü bir eşik oluşturuyor.** Relief kent desteğini35→50 yapar;40 altındaki bütün bölgelerde haftalık +2 huzursuzluk baskısı durur. Negotiate bunu yapmaz. Ek eylem verilmezse23. hafta ilk açlık, iki açlık haftasında185 asker kaybı oluştu. Bu tercih tek başına kötü sayılmaz; ardından kent desteğine müdahale edilmesi gerekiyor.
2. **Paris yardımı kendini dolaylı besleyebiliyor.** Asker büyümezken4. haftada kent desteği62 olur;60 üzerindeki destek ülke huzursuzluğunu azaltır. Gıda12. haftada187,16. haftada184,24. haftada220: ilk düşüşün ardından üretim toparlanması görüldü. Para ve askerî malzeme krizi oluşmadı.
3. **Aynı yardım büyüyen orduyla kırılgan.** Yalnız dört asker alma emri başarılı:1200→2000. İlk açlık8. hafta; sonraki17 hafta açlık, toplam1520 kayıp. Sekiz sonraki asker alma emri gerçek kaynak kontrolünden reddedildi. Başarısız yardım kent desteğini düşürür, huzursuzluk üretimi azaltır, yeni açlık doğar. Son stok hâlâ781 hazine/227 malzeme,1600 insan gücüdür: bağlayıcı eksik para veya insan değil gıdadır.
4. **Meclis ilişkisinin ekonomik bedeli çalışıyor.** Altı söz bozma Meclis desteğini0'a indirir;24. hafta hazine2158, ayrıcalıksız relief çizgisinden921 az. Tutulan sözler yerel huzuru ve Meclis desteğini yükseltir; son hazine3556, ortalama huzursuzluk25,5. Ödenen toplam240 gıda tek kaynak bedelini görünür tutuyor.
5. **Ordunun yerel siyasi izi çalışıyor.** Tazminat reddinde Paris huzursuzluğu92, kontrol38, elit sadakati0; ödemede0/100/48. Dumas ilişkisi14'e iner. Buna rağmen sabit duran ordunun morali ve ikmali100, kurum desteği84: komutanla kişisel ilişkinin doğrudan emir davranışı henüz yok. Red politikasının son hazinesi ödemeden yalnız42 fazla; tasarrufun çoğu zayıflayan yerel gelirle kayboluyor.
6. **Sarayın hafızası henüz davranışa dönüşmüyor.** Altı avans reddi sonunda kraliyet desteği0 ve Valcourt ilişkisi0; iktidar31 hâlâ10 eşiğinin üzerinde. Avans almaya devam edilebildi, hazine3799 oldu. Bu, mali sistemin çöküşü değil; kızgın hamiden yardım almanın somut bir sonraki siyasi bedelinin eksik olduğuna dair kanıttır.

Hiçbir politikada maaş veya askerî malzeme açığı oluşmadı. Savaş/yürüyüş dışarıda olduğu için bu sonuç bütün kampanyada ikmalin önemsiz olduğu anlamına gelmez.

## Sonraki bağlı özellik için çıkarım

Öncelik yeni genel kaynak çubukları yerine **haminin açık koşul ve tepkisi** olabilir: ağır biçimde bozulan söz, bir sonraki avansın şartını ve karakterin talebini değiştirsin. Her ret kuralının oyuncunun görebildiği bir onarma yolu bulunmalı; ilişkisi0 olan oyuncuyu mevcut tek onarma eylemine erişemediği kalıcı kilide sokmamalı. Saray için bu ihtiyaç ölçüldü; Meclis ve orduda mevcut ekonomik/bölgesel sonuçlar zaten daha görünür.

İkinci ihtiyaç gıda tehlikesini, ordu büyütme ve sürdürme kararında erkenden göstermek. Mevcut hazine fazlası otomatik olarak gıdaya dönüşmediği için yeni ekonomik seçenek önerilecekse hangi siyasi bedelle tahıla erişim sağladığı açıklanmalı. Bu rapor hiçbir maliyeti, eşiği veya yetki kuralını değiştirmez.
