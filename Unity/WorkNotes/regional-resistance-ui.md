# Bölgesel direnç — yürüyüş öncesi kısa bilgi

6 Eylül 2026. Yalnız UI hazırlığı; Assets değişikliği veya yeni pencere yok. Bölgesel düşman formülü henüz bu plana verilmedi. Sayılar ve nedenler UI içinde üretilmeyecek.

## İncelenen mevcut durum

`CabinetHud.Province`, özellikle mevcut `CanMarch` / `PreviewMarch` / yürüyüş düğmesi / bedel sırası; `CampaignCore.CanMarch` ve `TravelProjection`; `cabinet.json` içindeki `ui.army.*` ve `ui.march.*` okundu.

Gerçek Champagne rota karesi: `output/verify/military-art-final-20260906-012710-424-48b0deff/shots/01-champagne-route.png`. Bu kare eski ama doğrudan seçili Champagne yürüyüşünü gösteriyor. Daha yeni `officer-battle-second-maneuver-20260906-035947-297-9703935f/shots/01-officer-prebattle-ru.png` de açıldı; adı prebattle olsa da Champagne rotası değil, ordunun bulunduğu Île-de-France ve açık Dumas belgesidir. İki kare birbirinin kanıtı sayılmadı.

Champagne karesinde sanki düşman belliymiş gibi doğrudan “Вступить в сражение” düğmesi var; oyuncu asker sayısını üst HUD'da görüyor fakat düşman sayısını veya sebebini henüz göremiyor. Düğme yaklaşık `x16..211 / y501..535`, altında iki satır gerçek hareket bedeli; bölge raporu yaklaşık `y590` civarında başlıyor. Rapor kayar, emirler ise sabit `226 × 540` grubun içinde. Bugünkü asker sayısına bağlı düşman tabanı `TacticalBattle.Begin` içinde `Max(200, RoundToInt(originalTroops * .9f))`; bu, yeni bölgesel sayı veya sebep olarak sunulamaz.

## Seçilen yerleşim

Üç farklı yer kendi içinde karşılaştırıldı: bölge adı/şehir başlığına eklemek, alt durum raporuna yerleştirmek ve doğrudan yürüyüş kararının önüne koymak. **Yürüyüş düğmesinin hemen önü** seçildi: aynı seçili bölgenin tehdidi, oyuncunun kıyaslayacağı asker sayısı ve ardından gerçek eylem tek akışta okunur. Üst başlık uzun Île-de-France adı için dar; alt rapor ise karar verilmeden görülmeyebilir.

Yeni çerçeveli kart veya başlık şeridi yok. Mevcut kâğıt üstünde, aynı `x4 / width195` alanında:

| Sıra | RU | TR | Görünüm |
| --- | --- | --- | --- |
| 1 | `Сопротивление: {N}` | `Direniş: {N}` | 13 px koyu mürekkep, sayı açık ve tam |
| 2 | `Наших солдат: {P}` | `Askerimiz: {P}` | 12 px, canlı mevcut asker sayısı |
| 3 | `{Core'dan gelen kısa gerçek sebep}` | `{Core'dan gelen kısa gerçek sebep}` | 12 px, 1–2 satır; sahte istihbarat aralığı yok |
| 4 | Mevcut yürüyüş / savaş düğmesi | Mevcut yürüyüş / savaş düğmesi | 195 × 34; gerçek kullanılabilirlik korunur |
| 5 | Gerçek erzak, teçhizat ve hareket bedeli | Aynı | Mevcut iki satır, ardından varsa açlık uyarısı |

Hedef ek yükseklik yaklaşık **50–64 px**. Sabit yüksekliğe metin sıkıştırılmaz; gerçek dilde `CalcHeight` ile ölçülür. Neden metni bütün formülü uzun düzyazıyla anlatmaz: kabul edilmiş Core'un gerçekten kullandığı etkenleri adlandıran kısa bir neden olmalı; “askerimiz arttığı için düşman arttı” gibi eski davranışı örtülü sürdürmemeli. Bileşen sayıları Core verirse kendi katkılarıyla yazılabilir. Bu aşamada nüfus, huzursuzluk veya denetimin formüldeki etkisi varsayılmıyor.

Hareket bedeli sayı ve konum olarak korunur. Küçük dil düzeltmesi yapılacaksa, mevcut geçmiş zamanlı “Потрачено ходов” / “Harcanan hareket” yerine önizleme olduğu açık “Стоимость в ходах: {M}” / “Hareket bedeli: {M}” tercih edilebilir; henüz bir bedel ödenmiş gibi anlatılmamalı.

## Bağlam kuralları

- **Erişilebilir düşman bölgesi:** Tam `N / P / neden` bloğu, etkin savaş düğmesi, gerçek bedel ve varsa açlık uyarısı. Oran veya zafer yüzdesi eklenmez; asker sayısı tek başına kesin zafer demek değildir.
- **Uzak düşman bölgesi veya hareketi bitmiş ordu:** Bölgesel bilgi mevcutsa `N / P / neden` yine görülebilir; düğme kapalı ve gerçek `CanMarch` engeli yanında kalır. Bu sayı, buraya doğrudan yürünebildiği anlamına gelmez. `CanMarch.RequiresBattle` bu iş için tek başına kullanılamaz: kaynak, komşuluk/hareket kontrolünden önce erken dönebiliyor. Direnç önizlemesi erişim izninden bağımsız Core verisi olmalı.
- **Sakin komşu bölge:** Düşman sayısı uydurulmaz. Kısa `Бой не ожидается. Наших: {P}` / `Çatışma beklenmiyor. Askerimiz: {P}`; sonra gerçek yürüyüş ve bedel. “0 düşman” ancak Core gerçekten bu anlama gelen bir sayı döndürürse kullanılabilir.
- **Ordunun mevcut bölgesi:** Yeni direnç bloğu gösterilmez. Mevcut “ordu burada” ve komşu bölge seçme açıklaması kalır. Böylece Dumas bağlantısı, asker alımı ve Paris desteği olan en yoğun kendi-kamp düzeni gereksiz yere uzamaz.
- **Asker yok:** Yeni karşılaştırma, `P=0` ile saldırı yapılabileceğini ima etmez. Mevcut boş-ordu düğmesi/engeli önce gelir. Uzak bölgenin bilgisini gösterme kararı Core sözleşmesiyle netleştirilebilir; sahte rota bedeli hesaplanmaz.
- **Aynı bölgeye ilişkin karar değişikliği:** Vergi, ekmek veya anlaşma gerçek direnç girdisini değiştiriyorsa sayı aynı seçili bölgede güncellenmeli. Bölge kimliği değişmeden eski rakamı tutan bir UI önbelleği kullanılmamalı. Savaş başlarken Core'un kullandığı sayı bu önizlemeyle aynı kaynaktan gelmeli.

## Taşma sınırı ve kabul

Normal Champagne karesinde yaklaşık 50–64 px ek alan, alttaki raporu küçülterek sığabilir. Bu bir runtime kanıtı değil, mevcut görüntüden çıkarılan yerleşim tahminidir. Özellikle **ordu başka yerdeyken Paris seçili + etkin vergi anlaşmasını bozma uyarısı + aç yürüyüş + Paris desteği** birleşimi dar olabilir. Bu birleşimin bugün gerçek bir karede taştığı iddia edilmiyor; sabit grup ve değişken paragraf boylarından görülen öncelikli kontrol durumudur.

Uygulama kabulü için dört ana görünüm RU/TR incelenmeli: düşman komşu (Champagne), uzak düşman, sakin komşu, kendi kampı. Ayrıca gerçek API ile erişilebilen en yoğun Paris birleşimi çekilmeli. Yeni bloğun uğruna bedel, kapalı düğme nedeni, açlık uyarısı veya Paris desteği kesilemez. Son satırdan sonra kullanılabilir rapor alanı kalmalı; `Mathf.Max(1, ...)` tek başına kabul sayılmaz. Bu sınır aşılıyorsa sayıları küçültmek veya nedeni tooltip'e saklamak yerine, root yerleşim kapsamını yeniden kararlaştırmalı; bu plan sessizce bütün emirleri kaydırılabilir yapmaya yetki vermez.

İstenen minimum Core sözleşmesi: seçili bölge için gerçek direnç sayısı, barış/savaş bağlamı ve kısa nedene yetecek gerçek girdiler; erişim kısıtı için mevcut `CanMarch`; bizim sayı için canlı `state.Troops`. Yeni formül ve API gelmeden lokalizasyonun neden satırı sonlandırılmamalı.

Bu hazırlıkta yalnız dosyalar ve iki mevcut PNG okundu. Unity, derleme, oyuncu veya test çalıştırılmadı; yeni kod eklenmedi.

## Gelen Core sözleşmesi — neden satırının somutlaşması

Gameplay ajanı root'un B seçimini ve uygulama sözleşmesini gönderdi; Core henüz source freeze değil. `GetRegionalResistance(state, regionId)` → `RegionalResistanceTerms { RegionId, RequiresBattle, EnemyTroops, BaseTax, MobilizationBase, UnrestPressure, ControlGap, EliteOpposition }`. Üç pay `0..1`; taban `30 × BaseTax`, kuvvet `RoundAway(MobilizationBase × (UnrestPressure + ControlGap + EliteOpposition))`. Huzursuzluk 65'in altındaysa savaş yok ve `EnemyTroops=0`. Bizim ordu büyüklüğü veya `CanMarch` erişimi bu hesabın girdisi değil.

Bu sözleşmeyle neden için iki kısa, ölçülebilir satır önerisi:

| RU | TR |
| --- | --- |
| `Налоги {T} · волнения {U}` | `Vergi {T} · huzursuzluk {U}` |
| `Контроль {C} · лоял. элит {E}` | `Denetim {C} · seçkin sadakati {E}` |

Burada `T=BaseTax`; `U=100×UnrestPressure`, `C=100×(1−ControlGap)`, `E=100×(1−EliteOpposition)` yalnız Core'un mevcut girdi değerlerinin sunumudur. Düşman sayısı UI'da yeniden hesaplanmaz. “Налоги” gerçekten taban vergi kapasitesidir; yürüyüş fiyatı veya o anda toplanacak para değildir. Gerekirse ilk sözcük `База налога` / `Vergi tabanı` olarak açılır; gerçek 195 px ölçümü belirleyici olmalı.

İki neden satırı, üstteki direnç ve bizim asker satırıyla toplam dört satırda hedeflenen yaklaşık 50–64 px bloğu koruyabilir. Eşittir zinciri veya kazanma oranı eklenmez. Sözleşme değişirse metin aynı anda değişmelidir; başlangıç örneği için bile burada henüz yeni düşman rakamı uydurulmadı.

## Root yerleşim kararı — iki satır ve erişilebilir gerekçe

Root, sabit 540 px emir alanının en yoğun durumunu korumak için ilk dört satırlı öneriyi daralttı. **Son yerleşim önerisi**, yürüyüş önünde yalnız iki kuvvet satırı (yaklaşık 36 px), ayrıntılı nedenler ise mevcut kaydırılabilir bölge raporunda üç siyasi meter'ın hemen ardından. Yukarıdaki dört satırlı metin ilk hazırlık seçeneğidir, nihai düzen değildir.

Bu ayrım uygundur; aynı verileri dar emir alanında tekrar etmez. Fakat gerekçenin yerini belli etmek gerekir. Önerilen küçük bağ: ilk `Сопротивление: {N} →` / `Direniş: {N} →` satırı aynı yazı boyunda bir belge içi bağlantı gibi çizilsin; tıklanınca yalnız mevcut `provinceScroll` rapordaki direnç gerekçesine gitsin. Yeni yükseklik, pencere veya kampanya eylemi eklenmez. Üç meter'ın altındaki bir blok, böyle bir yönlendirme olmadan normal karede görünmeyebilir. Salt hover tooltip, gerekçenin tek erişim yolu olmamalı.

Root'un bağlam sınırları da korunur: kendi kampında üst kuvvet tekrarı yok; sıfır ordulu veya uzak seçili bölgenin kuvvet bilgisi raporda mevcut olabilir, ama yürüyüş önünde yanıltıcı eylem özeti çizilmez. Gerçek engel mesajı görünür kalır. Sakin erişilebilir bölge “çatışma beklenmiyor” olarak okunmalı; sıfır sayısı ile ordunun yokluğu birbirine karıştırılmamalı.

Rapordaki ayrıntı, yeni Core verisinden kesin sayıyı ve taban + üç baskı payını açıklayabilir. Root'un verdiği yeni sayılar yalnız sözleşme örnekleridir: Champagne başlangıcı 1114; vergi sonrası 1234; vergi→ekmek 1106; vergi→anlaşma 1136; başlangıçta ekmek veya anlaşma sonrası barış ve 0. Bunlar bu UI hazırlığında oynatılıp doğrulanmadı.

Son kabulde bağlantıyla gerekçeye ulaşma, normal tekerlekle geri gelme, RU/TR ve engelli rota durumları ayrıca görülmeli. Assets yine değiştirilmedi.

## İlk uygulamanın kaynak ve gerçek üst alan incelemesi

Artefakt: `output/verify/regional-resistance-first-20260906-042905-276-5f3716da`. Gerçek tam PNG'ler `00-initial-resistance-ru`, `01-initial-resistance-tr`, `02-prepared-resistance-ru`, `05-taxed-resistance-tr`, `08-peaceful-resistance-ru`, `09-peaceful-resistance-tr` açıldı. `REPORT.md`: GREEN 396 Unity, yeni build, 10 PNG, 94 assertion, 8 state, 10 browser testi.

**Üst özet bu altı karede okunur.** Direnç satırı, ok, bizim asker sayısı, yürüyüş/savaş düğmesi ve iki satır bedel kesilmiyor. İlk RU/TR karelerinde 1114 karşısında 1200; hazırlık RU karesinde aynı 1114 karşısında 1600 var. Vergi TR karesinde 1234 görünür. Barış RU/TR karelerinde sayının yerine açık “Мирный проход” / “Çatışmasız geçiş” geliyor; bedel görünmeye devam ediyor.

08/09'da anlaşmayı bozacak verginin üç ek sonuç satırı da tam görünür. Yalnız alttaki rapor daralıyor; son meter'ın kaydırma alanı kenarında kesilmesi doğal. Bu kareler Champagne'dır; Paris desteği + açlık + uzak ordu birleşiminin kabulü sayılmaz. Altı karede ayrıntılı direnç raporu açılmamış; kaynakta ok bağlantısı bulunması, gerçek tıklama ve alt bölümün görsel kabulü değildir. Root'un native ok tıklaması ve iki kaydırılmış PNG'si bekleniyor.

### Kaynak metnine dar öneri

`CabinetRegionalResistance.cs` ve `resistance-ui.json` okundu. `origin`, üç faktörü hemen alttaki `factors` listesinden önce yeniden uzun cümleyle sayıyor. Buna karşılık Core'un hesap tabanı `MobilizationBase` raporda gösterilmiyor. `threshold` de mevcut haliyle “бой начинается” diyerek yürüyüş eylemi olmadan savaş başlıyormuş gibi okunabilir.

Root'a önerilen kısaltma:

- RU origin: `База области: {0}. Силы — база × сумма долей ниже.`
- TR origin: `Bölge tabanı: {0}. Kuvvet = taban × aşağıdaki payların toplamı.`
- `{0}`: Core'un `MobilizationBase` değeri; UI yeniden düşman hesabı yapmaz. Üç mevcut `factors` satırı aynen korunur.
- RU threshold: `Поход при недовольстве от 65 — бой. Уступки могут открыть мирный проход, но расходы останутся.`
- TR threshold: `Huzursuzluk 65 ve üzeriyse yürüyüş savaşa dönüşür. Tavizler barışçıl geçiş açabilir; yürüyüş bedeli kalır.`

Bu öneri hem tekrarları azaltır hem tam sayının nereden geldiğini gösterir. Gerçek fontta kaç satır olacağı henüz alt bölüm PNG'sinde görülmedi; sabit bir piksel kazancı vaat edilmez. Bu incelemede Assets düzenlenmedi, süreç başlatılmadı.

## Ayrıntı ve en yoğun Paris durumu — gerçek inceleme

Root önce `resistance-details-fixed-20260906-045029-174-37b55300` koşusunda 00–04 karelerini üretti; sonraki yanlış `accord sign` script komutu nedeniyle o koşu RED kaldı. İnceleme anında düzeltilmiş `resistance-details-complete-20260906-045150-217-15f6da43` de tamamlanmıştı. Tekrarlı eski görüntüler yerine **bu yeni koşunun dokuz PNG'sinin tamamı** açıldı. `REPORT.md`: GREEN 397/397 Unity, yeni build, 9 PNG, 19 assertion, 6 state, 10 browser testi. Eski RED kayıt değiştirilmedi.

### Okunur ve kabul edilebilir kısımlar

- `00/01-reason-top/bottom-ru` ve `02/03-reason-top/bottom-tr`: başlık, direnç 1114, taban 750, kısa çarpım açıklaması ve üç pay okunur. RU: 69/100, 39,5/100, 40/100. Alt karelerde yürüyüşün savaş eşiği, taviz ve kalan yol bedeli bütünüyle görünür. İki scroll konumu birlikte metni tamamlıyor; üst karede sonraki paragrafın kenardan kesilmesi burada normal kaydırmadır. Daha fazla metin kısaltması gerekmiyor.
- `04-distant-peace-tr`: Provence sakin görünüyor, ama mevcut düğme kapalı ve yalnız komşu bölgeye hareket edilebileceği açıklaması açık. Bölgenin barış durumu, doğrudan yürüyüş izniyle karıştırılmıyor.
- `07/08-zero-army-resistance-ru/tr`: üst HUD'da ordu 0, haritada bayrak yok. Hareket düğmesi “В строю нет солдат” / “Orduda asker yok”, altında asker gerektiği açıklaması görünür. Bölge raporu hâlâ gerçek 1564 direnci, taban 750 ve 93 / 75,5 / 40 paylarını gösteriyor. Bu, düşmanın bizim ordu sıfırlandı diye kaybolmadığını oyuncuya anlatıyor. RU alt eşik paragrafı scroll kenarında devam ediyor; TR aynı paragrafı tam gösteriyor.

### Gerçek ret: Paris'te rapor alanı çöküyor

`05-paris-hungry-orders-ru` ve `06-paris-hungry-orders-tr` beklenen zor birleşimi gerçekten içeriyor: ordu Normandy'de, seçili Île-de-France, erzak 6, canlı asker 1400, vergi anlaşması uyarısı, aç yürüyüş ve Paris desteği.

Emirlerin kendisi, tüm vergi sonuçları, iki yürüyüş bedeli satırı, açlık uyarısı ve Paris desteği düğmesi/açıklaması iki dilde de görünür. **Buna rağmen bölge raporu kullanılabilir değil.** RU'da desteğin altındaki yaklaşık y776 çizgisinden sonra okunur rapor satırı yok; yalnız y788 yakınında küçük kaydırma izi var. TR'de y783 yakınında “Denetim 74” satırının sadece üst parçası görünüyor. Bu, uzun bir belgenin doğal olarak kenarda kesilmesi değil: rapor viewport'u neredeyse sıfır yüksekliğe düşmüş.

Sonuç olarak aynı karede görünen barış geçişi oku, nedeni böyle bir viewport'a getiriyor; açıklama fiilen okunamaz. Kaynaktaki `Mathf.Max(1,786-reportTop)` bu durumu saklıyor. **Paris worst-case bu haliyle görsel olarak kabul edilmedi.** Normal ayrıntı ve sıfır ordu karelerinin kabulü bu ret durumunu çözmez. Root'a somut konumlar ve fark bildirildi; Assets değiştirilmedi. Yerleşim çözümü ve gerçek yeni Paris RU/TR kareleri gerekli.

## Tek kaydırılan belge düzeltmesi — görsel kabul

Yeni artefakt: `output/verify/resistance-unified-dispatch-20260906-045736-445-e8fe43ec`. Root teknik sonucu GREEN 397 Unity, 11 PNG, 19 assertion, 6 JSON, 10 browser testi, dış exit 0 olarak bildirdi. Bölge başlığı sabit kalırken emirler ve rapor artık y246–786 aralığındaki **tek ortak belge** içinde kayıyor.

Bağımsız incelemede sekiz tam PNG açıldı: `00-reason-top-ru`, `02-reason-top-tr`, `05-paris-hungry-orders-ru`, `06-paris-hungry-orders-tr`, `06b-paris-hungry-report-tr`, `06c-paris-hungry-report-ru`, `07-zero-army-resistance-ru`, `08-zero-army-resistance-tr`.

**Yeni yerleşim görsel olarak kabul edilir.** Önceki Paris viewport çökmesi bu karelerde giderilmiş:

- `05/06`: başlangıçtaki emirler, üç vergi sonucu satırı, kuvvet özeti, gerçek yürüyüş bedeli, açlık uyarısı ve Paris desteği eski konumlarında okunuyor. Kaydırma çubuğu artık tüm belgeyi temsil ediyor; aşağıdaki rapor için ayrı bir piksel yüksekliğinde viewport yok.
- `06b/06c`: aynı Paris durumunun barış raporu, eşik ve yol bedeli açıklaması, nüfus/vergi/hasat değerleri ile Normandy'deki ordunun sayısı ve hareket/moral/ikmal/yorgunluk bilgileri tam boy alanda okunabiliyor. RU'da en alttaki yorgunluk çubuğunun devamı scroll sınırına geliyor; sayı ve başlık açık. Bu, eski çökmüş rapor sorunu değildir.
- `00/02`: Champagne için 1114, taban 750, üç pay ve eşik açıklaması artık tek karede bütünüyle görülüyor. RU/TR metin uzunluğu kullanılabilir alanı zorlamıyor.
- `07/08`: direnç 1564 ve bizim ordu 0, hem raporda hem üst HUD'da anlaşılır; bayrak yok. Alt rapor da aynı kullanılabilir genişlik ve yüksekliği koruyor.

Bu kabul gerçek görüntü düzenine ilişkindir. Root aynı build'de native incelemeyi başlatıyor; gerçek ok tıklaması, tekerlekle emirlere geri dönüş ve girdi hedeflerinin doğruluğu bu PNG'lerle kanıtlanmış sayılmadı. Assets değiştirilmedi ve hiçbir süreç başlatılmadı.
