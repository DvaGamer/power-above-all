# Bölgesel vergi uzlaşması: bağımsız kabul ve denge gözlemi

## Kaynak durumu ve çalıştırma

`CampaignRegionalAccords.cs`, `CampaignCore.Forecast/Act/NextWeek`, v3 `CampaignArchive` ve yeni test adları/sözleşmesi salt okunur incelendi. Core ajanı kaynak freeze bildirdi. Bu ajan Unity, oyuncu, derleme veya probe çalıştırmadı; başka ajanın dosyasını değiştirmedi. Root'un00:26:55UTC merkezî koşusu aşağıda gerçek sayılarla değerlendirildi.

Hazır kaynak: `Unity/WorkNotes/RegionalAccordBalanceProbe.cs`. Root'un merkezî yardımcı komutu:

```text
node tools/run-core-probe.cjs RegionalAccordBalanceProbe
```

Yardımcı kendi yeni `output/core-probes` klasöründe derleme/çalıştırma günlüklerini ve kaynak SHA256 makbuzunu üretir. Probe yalnız `Console` ve bellekteki saf Core/Archive çağrılarını kullanır; dosya, insan kaydı, Unity veya duvar saati okumaz/yazmaz. Sonuçlar gerçek oyuncu doğrulaması değildir.

## Karşılaştırılan politikalar

Her politika aynı legacy başlangıçtan başlar ve tam12 ekonomi hesabı ilerler. Bütün politikalarda2. hafta aynı `negotiate` dilekçe kararı verilir. Nüfus/kurum/ordu başlangıçları, salvo veya savaş kayıpları elle değiştirilmez. Her politika haftalık archive turuyla ve archivesiz ikinci kez hesaplanır; son tam canonical JSON eşit olmalıdır.

| Politika | Gerçek Core emirleri | Adil karşılaştırma sınırı |
| --- | --- | --- |
| baseline-stay | Sadece haftalar ve aynı dilekçe | Ordu Paris'te kalır. |
| honour-every-four-stay | Champagne'a0/4/8'de dört haftalık tatil | Baseline-stay ile aynı ordu konumu; gelir, yerel durum ve siyaset farkı uzlaşmadan doğar. |
| break-after-two-stay | Aynı imzalar,2/6/10'da olağanüstü vergi | Verginin100 altınıyla siyasi kaybı birlikte taşır; ücretsiz yeniden imza yoktur. |
| baseline-recruit-1-5-9 |1/5/9'da mevcut ordunun bölgesinde asker alımı | Daha büyük ordunun tüm sonraki masrafı/gıda tüketimi korunur. |
| honour-recruit-1-5-9 | Aynı asker alımı+0/4/8 imzaları | Yalnız matched recruit baseline ile sözün marjinal etkisi karşılaştırılır; başarısız alım olursa sessiz telafi yapılmaz. |
| honour-peaceful-march | İlk imzadan sonra Champagne'a gerçek barışçıl yürüyüş, sonra yenilemeler | Ordu yer değiştirdiği için garnizon etkisi de sonucun parçasıdır; sabit konumlu saf vergi karşılaştırması sayılmaz. |
| honour-poitou-stay | Poitou'da aynı0/4/8 imzaları | Düşük BaseTax'ın daha ucuz gerçek bedel anlamına geldiği varsayılmaz; tam katkı/yuvarlama ölçülür. |

`WEEK` TSV satırı kapanan haftayı, gerçekten tahsil edilen toplam vergiyi, **aynı durumdaki** istisnasız vergi karşı-olgusunu, marjinal vazgeçilen vergiyi, kalan hesap sayısını, eski/son tarihi ve yeniden imzaya kalan süreyi verir. Stoklar, asker/insan gücü, güç/moral/ikmal, seçilen bölge huzursuzluk/kontrolü, Morel ilişkisi, meclis desteği ve ordu yeri de kaydedilir. Bölgenin o an65 eşiğine yeniden çıkıp çıkmadığı ayrıca görünür.

Marjinal vazgeçilen vergi, o haftanın **aynı yerel durum/meclis desteği** üzerinden tek muafiyetin bedelidir. Baseline ile12 hafta sonundaki Gold farkına eşit olması gerekmez: ilk sakinleşme üretimi/kontrolü etkiler, tutulan söz meclisi güçlendirir ve garnizon yeri farklı politikada farklıdır. Cumulative toplam bu ayrımı korur; dört kez ilk fiyat diye uydurulmaz.

## Savaş karşılaştırmasının sınırı

`tactical-trust-first-20260905-233324-829-3db06d4c` ayrı gerçek karşılaşmada125.803 saniyelik doğal zafer,196 kayıp ve24 askerî malzeme kazancı göstermişti. Bu sadece tarihli dış referans olarak yazılır. Probe `ResolveBattle` çağırmaz; bu196 kaybı yeni sürüm/rol/ekonomi için kanıtlanmış yeni savaş gibi enjekte etmez. Uzlaşma başlangıç Champagne69'u59'a indirip barışçıl erişim açsa da zor yolun18 gıda, iki hareket ve mevcut ikmal/malzeme bedelini gerçekten öder; savaş geçmişi/konvoy ödülü üretmez.

## Bağımsız kabul kontrolleri

- Teklifin salt okunurluğu ve imzadan sonraki gerçek Forecast ile tam vergi eşitliği; aktif teklifin **imzalı** bölge/tarihi izlemesi ve aynı durumdaki vergi karşı-olgusuyla uyuşması.
- İkinci aynı hafta imzası, erken bozmadan sonra başka bölgede imza ve kullanılmış ikinci tax komutunun günlük dahil tam JSON'u koruyarak reddi. Bozma ilk dört haftalık tarihi değiştiremez.
- Son tatilli ekonomi hesabı dördüncü hesaptır; ödül hesap kapandıktan sonra bir kez gelir. Ara haftalarda Morel/meclis ödülü kopyalanamaz. Arşiv turu soğuma/bitim tarihini değiştiremez.
- Ek yolculuk: assembly rolünün açık sözü+aynı bölgede uzlaşma, asker alımı, archive,2. haftanın dilekçe ve rol vadesi engelleri, asıl rol sözünü yerine getirme, sonra4. haftada ayrı uzlaşma ödülü. Engellenen haftalar sözün süresini tüketmez;5. hafta yeni modal olmadan ilerler.
- Açıkça etiketli sıfır Gold/Food/MilitarySupplies/Troops/Power sınır fixture'ı, tatil sırasında normal hafta ve dilekçe müzakeresiyle toparlanabilmelidir. Bu başlangıç bir oynanmış sefer sonucu gibi raporlanmaz.

## Şimdiki bulgu

Kaynak incelemesinde doğrulanmış ücretsiz döngü, yeni bir ilerleme kilidi veya önizleme uyumsuzluğu bulunmadı. Aynı haftada grant→tax→grant döngüsü eski `AccordUntilWeek` korunarak durduruluyor; tax retleri söz bozma işleminden önce; bitiş ödülü gerçek dördüncü hesap sonrasında. Dört hafta sonunda yeniden imza ile destek kazanılması sözleşmenin kasıtlı, süre ve vergi maliyeti olan yoludur; sonuç ölçülmeden exploit diye adlandırılmamalıdır.

`GetRegionalAccordTerms` soğumada da bilgi teklifi döndürebilir; eylem izni `CanGrantRegionalAccord` tarafından ayrıca reddedilir. Bu API ayrımı tek başına hata değildir. UI, bilgi mevcut diye imzayı kabul edilmiş saymamalıdır; root'un UI/protokol gate'i bu katmanı ayrıca sınar.

## Gerçek merkezî probe sonucu —2026-09-06 00:26:55UTC

Kanıt: `output/core-probes/RegionalAccordBalanceProbe-2026-09-06T00-26-55-871Z-da5dcaaa`. `result.json` kaynak SHA256 değerlerini taşır; compile ve probe native çıkışları0, verdictPASS, toplam1327 kabul kontrolü. `probe.stdout.log` yedi politika için84 gerçek haftalık satır, özetler ve ek yolculukları içerir. Bu Unity/player/NUnit sonucu değildir; kaynaklar saf Core olarak yürütülmüştür. Archivesiz eş koşularla tam JSON eşitliği de geçmiştir.

### 12. hafta sonu

| Politika | Gold | Food | Asker | Askerî malzeme | Toplam tahsil edilen vergi | Marjinal vazgeçilen vergi | Morel / Meclis | Hedef huzursuzluk / kontrol |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| baseline-stay |1648 |271 |1200 |216 |2440 |0 |50 /57 | Champagne93 /24.5 |
| honour-every-four-stay |1599 |289 |1200 |216 |2391 |152 |62 /72 | Champagne63 /60.5 |
| break-after-two-stay |1714 |265 |1200 |216 |2206 |56 |20 /27 | Champagne100 /30.5 |
| baseline-recruit-1-5-9 |920 |65 |1800 |132 |2426 |0 |50 /57 | Champagne93 /24.5 |
| honour-recruit-1-5-9 |873 |84 |1800 |132 |2379 |149 |62 /72 | Champagne63 /60.5 |
| honour-peaceful-march |1512 |276 |1200 |211 |2304 |196 |62 /72 | Champagne29 /91.5 |
| honour-poitou-stay |1568 |293 |1200 |216 |2360 |171 |62 /72 | Poitou35 /83.5 |

Bütün politikalarda12. hafta moral100, ikmal100; kırma dışında güç61, üç kez kırmada49. Asker alımında insan gücü1800, diğerlerinde2400; reddedilmiş alım0. Bu12 haftada kıtlık kaynaklı asker kaybı görülmedi. İlk4 ve8 haftalık honor-vs-baseline Gold farkları sırasıyla−41 ve−57;12'de−49'dur.

### Siyasi hamlenin belirgin güçlü olduğu yer

Champagne'da sürekli söz tutmak, bu başlangıçta yalnız49 net Gold karşılığında18 ek Food,12 Morel ilişkisi,15 meclis desteği,30 daha düşük huzursuzluk ve36 daha yüksek kontrol verdi.152 marjinal vergi bırakılmış olsa da daha iyi yerel durum ve meclis geri beslemesi bunun103'ünü baseline karşılaştırmasında telafi etmiş oldu. Bu103 yalnız meclise atfedilemez; kontrol/huzursuzluk da gerçek vergi formülüne girer.

Son dört hesabın vergi gelirleri honor için200/197/194/190, baseline için198/195/192/188: devam eden muafiyete rağmen her hafta2 fazla vergi. Bu, **nakit sıkışıklığı yoksa anlaşmayı sürekli açık tutmanın güçlü varsayılan siyasi politika olabileceği** somut denge sinyalidir. Yine de12 hafta toplamında Gold daha azdır; dört haftalık süre, tek bölge sınırı, muafiyet bedeli ve kırılgan geçiş gerçek kalır. Kesin baskın strateji veya ücretsiz exploit kanıtlanmış değildir.36 hafta, meclis tavanı, farklı dilekçeler ve savaş baskısı bu probe'da oynanmadı; ileriye kesin kazanç extrapolasyonu yapılmaz.

Asker almayan honor hedefinde huzursuzluk3.,4. ve8. haftalarda tekrar65'e ulaştı. İlk imzanın59'a indirdiği bölge, genel şehir desteği düşükken ordu tutulmazsa aynı dört haftalık söz içinde yeniden direnebiliyor. “Vergi tatili dört hafta askerî geçiş garantisidir” yorumu bu veriye aykırıdır. Ordu gerçekten Champagne'a yerleşince12. hafta29/91.5 olur ve bütün12 haftada hedef düşman eşiğine dönmez.

Poitou'nun BaseTax'ı daha düşük olmasına rağmen sözlerin marjinal bedeli171, Champagne'ın152'sinden yüksektir. Aynı Morel/meclis ödülü için Champagne31 daha fazla Gold bırakır; Poitou4 daha fazla Food üretir ve farklı bir bölgeyi istikrara kavuşturur. Sadece BaseTax'a bakarak ucuz güven çiftçiliği sonucu çıkarılamaz.

### Erken kırma: kısa vadeli para, ölçülen devam maliyeti

Üç olağanüstü vergi300 Gold verdi; normal vergi tahsilatı baseline'dan234 düşük kaldığından12. hafta net nakit avantajı yalnız66 oldu. Buna karşılık güç−12, Morel−30, meclis−30, Food−6; son haftanın vergi geliri160, baseline188. Hedef huzursuzluk100'e ulaştı. Kırma, kaynakların tümünde söz tutmadan üstün değildir; bütçe sıkışıklığı için gerçek ama pahalı tercih olarak davranır.

Kontrolün baseline'dan6 yüksek kalması geri sarılmayan geçmiş etkidir: ilk iki sakin haftada kontrol kaybı önlenmiştir. Kırma eski bölge fotoğrafını geri yüklemez; anlaşılmış nominal karşı tepkiyi o günkü durum üstüne uygular. Sonraki imza tarihleri yine4/8/12; tax→grant aynı hafta reddi tam JSON korunarak geçmiştir.

### Ordu, gıda ve garnizon bağı gerçekten çalışıyor

Üç asker alımı baseline'a600 asker ekledi;12. hafta728 daha az Gold,206 daha az Food ve84 daha az askerî malzeme bıraktı. Ayrışma elle konmuş kayıp değildir:360 Gold/60 Food/45 malzeme doğrudan alım bedeli; artan ordunun haftalık masrafı354 Gold, tüketimi144 Food ve39 malzeme; kalan14 Gold/2 Food farkı alımların huzursuzluk/üretim etkisiyle uyuşur.600 ek asker için kalan Food65; söz tutan eş politikada84. Tatil ordunun artan giderini ortadan kaldırmaz, bu eş çiftte47 Gold karşılığında19 Food ve aynı siyasi/yerel getiriyi ekler.

Barışçıl yürüyüşlü honor, ordusu Paris'te kalan honor'a göre87 daha az Gold,13 daha az Food ve5 daha az malzemeyle biter. İlk18 Food/5 malzeme yürüyüş bedeli gerçekten ödenmiştir; garnizonun Champagne'a taşınması ve Paris'te kalmaması sonraki ekonomi/yerel durumu değiştirmiştir. Bu farkı savaş zaferiyle denk bir sefer diye sunmak doğru değildir. Hiçbir savaş kaybı veya24 malzemelik konvoy ödülü enjekte edilmedi.

Ek assembly-sözü→asker-alımı→dilekçe/vade→ayrı-uzlaşma yolculuğu5. haftaya kadar geçti; Morel58, meclis64. İki ayrı taahhüdün tarihi ve önceliği korundu. Sıfır stok/güç/ordu sınır fixture'ı da yeni modal kilidi olmadan5. haftada827 Gold/197 Food'a toparlandı. Bu fixture gerçek oyuncunun başlangıcı veya oynanmış yoksullaşma sonucu sayılmaz.

### Morel güveninin sıfırdan geri kazanılması

Genel sözün rol bağımsız Morel ödülü olması tasarlanmış bir siyasi yoldur. Morel0 durumunda da imza atabilmek, bedelsiz aynı haftalık güven onarımı değildir: dört gerçek vergi hesabı beklenir, iptal ödülü vermez ve ilk tarihe kadar yeni imza açılamaz. Kaynak sözleşmesi bu yolu bütün rollere açar. Bu probe'ın normal yedi politikası Morel50'den başlar; sıfır güvenin özel yürütümü yapılmış gibi iddia edilmez. Başlangıçta0→4 sonucu ancak ayrı kaynak kuralı çıkarımıdır. Anında patron onarımının zaman farkı ve assembly rolü şartı korunur.

Sonuç: bu ölçümde kabul hatası, ücretsiz tekrarlama veya softlock bulunmadı. En güçlü denge adayı, nakit stresi düşük başlangıçta sürekli honor'un küçük net bütçe bedeline karşı çok kanallı ve giderek kendini finanse eden getirisi. UI veya kaynak bu inceleme sırasında değiştirilmedi; sayısal ayar gerekip gerekmediği ayrı tasarım kararıdır.

Native savaş belgesinin gelecek sırası da Digit4→Digit1 olarak düzeltildi. Önceki gerçek koşunun ilk Digit1 için bağımsız geçiş kanıtı taşımadığı açıkça korundu; eski artefaktlar değiştirilmedi.
