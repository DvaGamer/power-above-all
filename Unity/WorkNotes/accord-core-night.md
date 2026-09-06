# Bölgesel vergi tatili — çekirdek uygulama kaydı

Root'un `next-political-choice.md` sözleşmesi uygulandı: huzursuzluk−10/kontrol+3, sonraki dört **gerçek** vergi hesabında seçili bölgenin olağan katkısı0; son hesabın ardından Morel+4/meclis+5. Eski tutar daha sonra tahsil edilmez. Yeni zorunlu hafta ekranı veya oyun sonu yoktur.

Kaynak sınırı: `CampaignCore.cs`, yeni `CampaignRegionalAccords.cs`, `CampaignArchive.cs`, `accords-core.json`, yeni `RegionalAccordTests.cs`, mevcut `RoleCampaignTests.cs` v3 uyarlaması. GameApp/Cabinet/AutoShots root'un işidir. 6 Eylül root'a SOURCE FREEZE iletildi; bu kayıt sonrası Assets düzenlemesi/derleme/probe/Unity süreçleri root kapısına bırakıldı.

## Çalışan sözleşme

- `GetRegionalAccordTerms`, `GetActiveRegionalAccordTerms`, `CanGrantRegionalAccord`, `GrantRegionalAccord`, `HasRegionalAccord`, `TaxBreaksRegionalAccord` public API. Teklif/etkin durum aynı `RegionalAccordTerms` tipinde: RegionId, UntilWeek, RemainingWeeks, CurrentTaxIncome, ProjectedTaxIncome, TaxForgone, IsActive, Immediate/Fulfil/Break.
- Önizleme durumu kopyalayarak/serileştirerek değiştirmez. `ForecastWithRegionalAccord` asıl Forecast ile aynı gelir/üretim ve tek toplam yuvarlama hesabıdır. TaxForgone aynı yerel koşullardaki istisnalı/istisnasız hesap farkıdır; ilk yatışmanın gelir değişimi ayrı kalır. Sabit dört haftalık fiyat hesaplanmaz.
- Dolu hazine/kullanılmış vergi emri önce reddedilir. Yalnız başarılı vergi asıl bölgedeki anlaşmayı bozar: +10/−3 yerel tepki; Morel−10/meclis−10/Güç−4; ardından normal +100 vergi ve kendi etkileri. Bölge kimliği boşalır, ilk UntilWeek kalır.
- Yeni grant mevcut petition/due önceliklerini korur; olağan açık rol sözüyle birlikte bulunabilir. Mevcut sözün fiyat/yer/vadesi değişmez. Süre sonu dördüncü Forecast ve hafta etkilerinden sonra otomatik işler, yalnız bir kayıt/ödül verir.
- v3 arşiv iki yeni alanın varlığını aynı DCS'nin küçük `IsRequired` projeksiyonuyla doğrular. Null bölge geçersiz; boş string/0 yokluk, boş string/gelecek tarih beklemedir. v1/v2 rol kuralları korunur; eksik yeni alanlar açıkça boş/0'a göç eder, sıfır olmayan yeni anlaşmayı eski sürüm numarasıyla gizlemek reddedilir.

## Gerçek yerel doğrulama

Unity/Editor/player başlatılmadı; NUnit yürütülmedi. Root'un artık bütün çalıştırmaları merkezileştirmesinden **önce** aşağıdakiler çalıştı:

1. `Unity/WorkNotes/compile-accord-tests.cjs`, kurulu Unity'nin gerçek referanslarıyla runtime19/editor3 ve `RegionalAccordTests.cs` + `RoleCampaignTests.cs` dosyalarını derledi: PASS, son koşuda warning yok. Çıktılar `output/PowerAboveAll.Runtime.check.dll`, `output/PowerAboveAll.Editor.check.dll`, `output/PowerAboveAll.AccordTests.check.dll`. İlk yardımcı test derlemesi eski NUnit için mscorlib facade istedi; helper gerçek `NetStandard/compat/2.1.0/shims/netfx` referanslarını ekleyince geçti. Bu bir oyun kaynak hatası değildi.
2. `RegionalAccordArchiveProbe.cs` gerçek beş Core dosyasıyla kurulu Mono/mcs altında derlendi ve `output/RegionalAccordArchiveProbe.exe` çalıştı. Legacy/crown/assembly/army için başlangıç→aktif→dört gerçek vergi→bitiş arşiv turları PASS; önizleme/gerçek gelir eşitliği ve geçmiş gelir borcu olmaması doğrulandı. Eksik/null bölge, eksik/null tarih, sayısal olmayan tarih; gerçek eski alan yokluğuyla v1/v2 geçişi; erken vergi/kalıcı cooldown ve sürüm düşürme reddi PASS.
3. Bu probe bir gerçek sınır hatası buldu: DCS, `"not-a-week"` tarihini Mono'da `TargetInvocationException` içinde `XmlException` olarak çıkarıyordu. Archive yalnız SerializationException yakaladığı için beklenen ArgumentException sınırı bozuldu. Dar filtre yalnız reflection sarmalını açıp SerializationException/XmlException/FormatException/OverflowException türlerini kabul eder; başka TargetInvocationException veya genel çalışma hatasını yakalamaz. İlgili NUnit testi korundu; probe tekrarında bütün maddeler geçti.

Statik derleme komutu:

```text
bash -lc 'cd /mnt/c/Users/USER/projects/power-above-all && /mnt/c/Program\ Files/nodejs/node.exe Unity/WorkNotes/compile-accord-tests.cjs'
```

Gerçekte çalıştırılan saf çekirdek komutu (tek bash çağrısında derleme ve ardından çalıştırma):

```text
bash -lc 'cd /mnt/c/Users/USER/projects/power-above-all && /mnt/c/Users/USER/Tools/Unity/6000.3.23f1/Editor/Data/MonoBleedingEdge/bin/mono.exe C:/Users/USER/Tools/Unity/6000.3.23f1/Editor/Data/MonoBleedingEdge/lib/mono/4.5/mcs.exe -r:System.Runtime.Serialization -out:C:/Users/USER/projects/power-above-all/output/RegionalAccordArchiveProbe.exe C:/Users/USER/projects/power-above-all/Unity/WorkNotes/RegionalAccordArchiveProbe.cs C:/Users/USER/projects/power-above-all/Unity/Assets/Scripts/Core/CampaignCore.cs C:/Users/USER/projects/power-above-all/Unity/Assets/Scripts/Core/CampaignRoles.cs C:/Users/USER/projects/power-above-all/Unity/Assets/Scripts/Core/CampaignArchive.cs C:/Users/USER/projects/power-above-all/Unity/Assets/Scripts/Core/CampaignPatronTrust.cs C:/Users/USER/projects/power-above-all/Unity/Assets/Scripts/Core/CampaignRegionalAccords.cs && /mnt/c/Users/USER/Tools/Unity/6000.3.23f1/Editor/Data/MonoBleedingEdge/bin/mono.exe C:/Users/USER/projects/power-above-all/output/RegionalAccordArchiveProbe.exe'
```

Yeni NUnit kapsamı: dört rol; Champagne gerçek barışçıl/zor yürüyüşü; dinamik vergi önizlemesi; dört hesabın sınırı/tek ödül; erken bozma; kullanılan emir/dolu hazine atomikliği; başka bölge/asker alma; açık rol/dilekçe/vade birlikteliği; sıfır kaynak/ordu/Güç;0–100 sınırları; dört geçerli arşiv durumu; beş eksik/null/tür bozulması; altı state bozulması; gerçek eski alan yokluğu; dört sürüm düşürme akışı; takvim sınırı. Önceki rol arşiv testleri artık yeni kaydı v3 bekler; eksik/null Mandates hem gerçek v2 hem v3 için ayrıca reddedilir. Tam gerçek NUnit ve oyuncu sonucu root'tan beklenecek.

## Gerçek root kapısı ve altı oyuncu JSON'unun denetimi

Salt okunur inceleme: `output/verify/regional-accord-20260906-002323-844-cee27fbb/`. Bu incelemede Unity, derleyici veya çekirdek probe çalıştırılmadı; var olan XML/JSON/script dosyaları okundu. `edit-tests.xml` toplam **128/128 Passed** bildiriyor. `RegionalAccordTests` fixture'ı **34** case; mevcut rol arşivindeki missing/null Mandates metodu v2/v3 için **4** case. Önceki `tactical-trust-first-20260905-233324-829-3db06d4c/edit-tests.xml` toplam92 ve aynı eski metodun2 case olduğunu doğruluyor: artış kesin olarak **34 yeni çekirdek case + 2 ek sürüm case = 36**. Ayrı Mono tanısı bu128 Unity testinin içinde sayılmadı.

`shots/shots-result.json`: success=true,98 komut,36 assertion,14 PNG,6 JSON, failures boş; tamamlanma `2026-09-06T00:23:47.6774576Z`. Aşağıdaki değerler aynı klasördeki gerçek state JSON'larından okundu:

| State | Hafta | Hazine | Gıda | Güç | Morel ilişkisi | Meclis desteği | Anlaşma bölgesi/bitişi |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 07-accord-signed | 0 | 840 | 360 | 55 | 50 | 45 | champagne / 4 |
| 09-peaceful-passage | 0 | 840 | 342 | 55 | 50 | 45 | champagne / 4 |
| 10-before-fourth-account | 3 | 1026 | 346 | 56,5 | 50 | 57 | champagne / 4 |
| 11-after-fourth-account | 4 | 1090 | 344 | 57 | 54 | 62 | boş / 4 |
| 13-broken-accord | 4 | 1190 | 344 | 53 | 44 | 52 | boş / 8 |
| 14-loaded-broken-accord | 4 | 1190 | 344 | 53 | 44 | 52 | boş / 8 |

- **İmza ve fiyat:** fixture başlangıç69 huzursuzluk/60,5 kontrol beklentilerini geçirmiş;07 JSON Champagne59/63,5 değerini taşıyor. Kaydedilmiş bölge/destek değerleriyle mevcut Forecast formülünün aritmetik kontrolü: imza öncesi207, imza sonrası istisnalı197. Aynı sakinleşmiş bölgede istisna olmasa209; dolayısıyla **TaxForgone12**, toplam önce/sonra farkı10. Bunlar farklı doğru ölçülerdir; “tatilin bedeli10” diye birleştirilmemeli. Bu aritmetik kontrol oyun/probe çalıştırması değildir.
- **Manevra:**09 JSON orduyu Champagne'da,1200 asker/0 hareket/342 gıda/115 askerî malzeme/88 ikmal/20 yorgunlukta gösteriyor; ResolvedBattles boş. Champagne artık61/61,5:59/63,5'ten bu fark, mevcut zor yürüyüşün +2 huzursuzluk/−2 kontrol sonucudur. Bedava yol veya savaş ödülü verilmedi.
- **Dört hesap:**11 JSON'un günlükleri hafta1→4 için vergiyi197,193,204,200; ordu giderini her sefer136; net gıdayı+3,+1,0,−2 gösteriyor. `840 + (197+193+204+200) − 4×136 = 1090`; yürüyüş sonrası `342+3+1+0−2=344`. Dördüncü hesabın sonradan tahsil edilen eski gelir eklemediği sayılardan görülüyor. Anlaşma3. haftada hâlâ aktif,4. haftada tek `log.accord.completed` ile bitmiş. Morel50→54, meclis57→62; önceki45→57 artışı bu fixture'ın `negotiate` dilekçesi seçimine aittir.
- **Paris ve erken bozma:**11→13 arasında hafta değişmemiş. Günlük Paris için grant/broken kaydını4. haftada taşıyor. Paris46/71'den nominal tatil+geri tepki+normal vergi sonrasında58/71 olur; elit sadakati60→56, TaxUsed=true. Hazine+100, Güç−4, Morel−10, meclis−10 doğru. Kimlik boş, UntilWeek8 kalmış. Script'in sonraki grant ve ikinci tax denemelerinin `same after-break` kontrolleri geçmiş.
- **Yükleme:**13 ve14 JSON bütün alanlarıyla eşit; bağımsız metin karşılaştırması `cmp` de0 çıktı. Yalnız tabloda görünen kaynaklar değil, günlük, bölgeler, kişiler ve bekleme tarihi de korunuyor.
- **Rol sözünün sınırı:** bu altı gerçek oyuncu JSON'u `RoleId=legacy`, `NextMandateWeek=0`, `Mandates=[]` taşıyor. Dolayısıyla bu oyuncu rotası aktif rol sözüyle beraberliği tek başına kanıtlamaz. O sözleşme ayrı `RolePromiseKeepsItsOwnRegionPriceAndPriorityWhileHolidayCountsOnlyAdvancedWeeks` Unity case'i ile Passed: kraliyet sözünün asıl bölgesi/150 altın/vadesi, dilekçe ve due önceliği, yalnız gerçekten ilerleyen haftaların vergi tatiline sayılması korunur. Eski rol yolculukları da aynı128'lik kapının içinde kalır.
