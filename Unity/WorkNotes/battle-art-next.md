# Savaş kompozisyonu 04 — sonraki tek vurgu

Durum: salt okunur görsel değerlendirme ve öneri. Assets değiştirilmedi, Unity/oyuncu başlatılmadı. Amaç mevcut güneşli guaj dilini sürdürmek; yeni savaş kuralı, engel, görüş kesici veya tarihî bina iddiası eklemek değildir.

## İncelenen gerçek kareler

- `output/verify/roles-base-regression-20260905-230755-557-e852bd7c/shots/15-battle-ru.png` ve `output/verify/painted-atlas-final-20260905-221432-805-e022084e/shots/15-battle-ru.png`: açık çayır, mavi dere, koyu bahçe ve alt emir şeridi aynı resim dilinde. Sol üstteki gerçek tepe çok az okunuyor. Suyun kıyısı kaynakta hafif oynasa da bu ölçekte büyük ölçüde düz bir şerit; geçit geniş dikdörtgen toprak parçası gibi. Bahçenin düzenli 4 × 5 dikimi bir meyve bahçesi olarak anlaşılabilir; bunu kendiliğinden ormana benzetmek gerekmiyor.
- Yeni `output/verify/tactical-trust-first-20260905-233324-829-3db06d4c/shots/02-deployment-paused.png` ve `05-contested-crossing.png` de gerçekten incelendi. Yeni emir/durma sunumu aynı araziyi koruyor. Beşinci karede birlikler bahçe kenarında ve geçitte toplandığı için küçük ayrıntıları artırmak yoğunluğu büyütür; açık zeminin önemli bölümü sakin kalmalı.

## Kural sınırları

- Tepe: `TerrainHeight`, merkez `(−20, 15)`, normalize yarıçaplar `(13, 12)`, kosinüs profili, tepe yüksekliği `2.9`. Elips dışında bu tepe yoktur. Yüksekten ateş etkisi ayrı bir çizgili alan değil, gerçek saldıran/hedef yükseklik farkı `> 1` koşuludur.
- Bahçe: `−27 < x < −10`, `−5 < z < 10`. Yirmi mevcut gövde bu alanın içindedir. Bu bölümde var olan koruma/yavaşlama etkisi vardır; yeni çit veya bina koruması yoktur.
- Dere kuralı: `abs(x−6) < 2` ve `abs(z−1) > 4`; dolayısıyla geçit `−3 ≤ z ≤ 5`. Dere taban geometrisi daha dar `abs(x−6) < 1.6`, yükseklik `−.42`. Su meshinin hafif dalgalı kıyısı mevcut dar kanal içinde tutulur.
- Yol, tarla çizgileri ve çit dekoratiftir. Yeni vurgu onları koruma, hız bonusu veya geçilmez engel gibi göstermemeli. Birlik konumu, görüş hesabı, rastgele sayı akışı ve kamera değişmeyecek.

## Üç farklı küçük yön

| Yön | Tek görünür vurgu | Uygulama kapsamı | Kazanç ve sınır |
| --- | --- | --- | --- |
| **A — Guaj tepenin hacmi** | Bahçenin arkasında güneşli tepe ve tek yumuşak serin yamaç | Yalnız `PaintMeadow` içinde gerçek elipse bağlı geniş ton alanları; geometri ve bütün sahne ışığı aynı | Şu anda metin olmadan okunmayan tek ana arazi unsurunu görünür kılar. Çizgi, tabela, taş veya yeni korunma işareti yok. **Seçilen yön.** |
| **B — Bahçenin gölgeli kenarı** | Yirmi ağacın mevcut düzeni içinde koyu, okunur tek taç kenarı | Bahçe taçlarının 2–3 büyüklük ritmi ve sınırlı sıcak/serin ayrımı; aynı gövde konumları, sayı ve gerçek bahçe alanı | Sağdaki açık tarlaya karşı solda güçlü bir kütle verir. Bahçe zaten açıkça okunuyor; yeni `05` karesinde birlikleri örten daha yoğun taçlar risktir. Tepe sorununu çözmediği için sonraya bırakılır. |
| **C — Açık renkli sığ geçit** | Altın hedef halkasının altında kuru geçidin doğal bir toprak açıklığı olması | Mevcut kare geçit parçasını yol yönüne bağlanan alçak düzensiz yüzeyle anlat; iki su kolu gerçek `z=−3/5` uçlarında kalır | Dikey dere ve eğik yolu tek merkezde bağlar. Köprü, kaya yığını, çit veya yeni engel eklenmez. Mevcut hedef halkası zaten güçlü bir merkez; şu an en zayıf tepeyi düzeltmediği için ilk tercih değildir. |

## Seçilen A için somut dar paket

1. Mevcut `PaintMeadow` içindeki geniş radyal aydınlatma, gerçek tepe elipsinin `d < 1` maskesiyle sınırlı, asimetrik bir ışık/gölge kompozisyonuna dönüştürülür. Şimdiki görsel aydınlatma yarıçapı `(16,15)` olduğu için tek başına gerçek kabarıklığı tarif etmiyor.
2. `TerrainHeight` sonlu farklarından veya aynı kosinüs profilinin eğiminden yamaç yönü çıkarılır. Bir tarafta türetilmiş güneşli çayır `#C6D19F`, karşı yamaçta serin çayır `#7F9E80` geniş ve yumuşak karışımlarla kullanılır. Tepe tam açık sarı bir disk, koyu halka veya kontur çizgisi olmaz. Maske elips kenarında sıfıra yumuşakça iner; dış çayır ve gerçek bahçe alanı normal rengini korur.
3. Tepe yüzeyi geniş fırça hissiyle kalır; yeni yüksek frekanslı gren, rastgele lekeler, tek tek çimenler veya dekor yoktur. Diğer dere/bahçe/yol/furrow/çit malzemeleri ve birlikler bu pakette değiştirilmez. Bir vurgu seçildi diye diğer iki öneri aynı pakete eklenmez.
4. Doku var olan `meadowPainting` yaşam döngüsünü kullanır; yeni dosya veya ekstra kalıcı materyal gerekmez. Mesh yüksekliği, `TerrainHeight`, birim hareketi/hasarı ve kamera aynı kalır. Bu paket mevcut geometriyi anlatır; yükseklik avantajının sabit bir bölge bonusu olduğunu ima etmez.

## Görsel kabul

- Aynı yerleşim ve kameradaki önce/sonra karelerde, üstteki arazi açıklamasını okumadan bahçenin arkasında tepe olduğu anlaşılmalı; yalnız renklendirilmiş düz bir daire görünmemeli.
- Birliklerin mavi/mercan ayrımı ve sarı hedef halkası tepe renginden daha net kalmalı. Tüm ekranın aydınlığı değiştirilmemeli; hoş açık çayır ile koyu bahçenin ilişkisi korunmalı.
- Yeni `02` başlangıç ve `05` yoğun temas kadrajlarına eşdeğer gerçek kareler kontrol edilmeli. Ağaçlar, su kanalı, gerçek geçit, alay kartları ve hareket yolu çizgileri öncekiyle aynı okunmalı.
- Sahneye ek tabela/duvar/çit/taş/bina gelmemeli. Yeni tepe vurgusu tek görünür değişiklik olarak okunmalı. B ve C ancak ayrı değerlendirme ve ayrı kare karşılaştırmasından sonra düşünülebilir.

Bu öneri tek başına 04 maddesinin bütün kapsamını tamamlamış sayılmaz; düz su şeridi ve geometrik geçit için C seçeneği ayrı sonraki aday olarak kalır.
