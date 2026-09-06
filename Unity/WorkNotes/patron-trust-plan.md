# Hami güveni — küçük sözleşme önerisi

Durum: yalnız tasarım; mevcut sözleşme veya denge değiştirilmedi. Kaynaklar: `CampaignCore.ChoosePetition/Act/ResolveBattle`, `CampaignRoles.GetObligationTerms/CanIssueMandate` ve `balance-night.md`. Tarihî iddia değildir.

## Somut açık

24 haftalık kraliyet temerrüt rotasında Valcourt ilişkisi ve taç desteği 0, Güç 31; oyuncu yeni 120 altın avansını hâlâ alabiliyor. İlişki şu anda erişimi değiştirmiyor. `negotiate` yalnız bir kez çıkan tahıl dilekçesidir: meclis +12, taç −8, Paris huzursuzluğu −10; hiçbir kişinin ilişkisini onarmaz. Vergi taç desteğini +1 artırır ama Valcourt ilişkisini artırmaz. Ekmek/sübvansiyon yalnız Lefèvre'yi; savaş Dumas'yı etkiler. Üç sözün yerine getirilmesi ilgili hami ilişkisini +4 artırır; yalnız kraliyet/meclis sözleri ayrıca kurum desteği +5 verir.

## Üç farklı yol

1. **Yeni ilişki ve destek eşikleri.** Örneğin iki göstergenin belirli yüzdeleri yeni yardımı açar. Okunur fakat keyfî eşikler getirir; Valcourt için tekrarlanabilir mevcut onarım yoktur. Ordu sözünü yerine getirmek kurum desteği vermediği için iki göstergeyi birlikte kilitlemek ayrıca tuzak yaratabilir.
2. **Ödenmemiş söz defteri.** Yeni yardım için önce eski borcu kapat. Geçmiş karar çok görünür olur; fakat yeni kalıcı durum/arşiv sürümü gerekir. Eski `break` kapatılmış bir anlaşmadır; sonradan borca dönüştürülemez. Kırpılan günlükten borç çıkarmak güvenilir değildir. Tahıl sıfırken 40 tahıl borcunu şart koşmak yeni çıkmaz ekler.
3. **Tükenen kişisel güven + açık siyasi telafi.** Yalnız ilişki tam 0 iken yeni ayrıcalık durur; mevcut 0–100 sınırı dışında yeni yüzde yoktur. Normal emirler, haftalar ve mevcut anlaşma sürer. Görünür siyasi telafi yeni ilişki kurar. **Önerilen küçük dilim budur.**

## Önerilen davranış

- Yeni yardım için bugünkü koşulların tamamı korunur: açık dilekçe/anlaşma yok, dört haftalık aralık, Güç en az 10, bölge/ordu koşulları. Bunlara yalnız ilgili haminin `Relationship > 0` koşulu eklenir. Kurum desteği ayrı görünür ve mevcut vergi/hafta sonuçlarını üretir; ikinci bir erişim kilidi olmaz.
- Açık anlaşma varken **hiçbir koşulu yeniden hesaplama**: `GetObligationTerms` aynı ödeme, yer ve tarihi gösterir. Erken veya vadesinde ödeme hâlâ ilişki +4 getirir; 0 ilişki ödeme hakkını engellemez. Önceden kapanmış temerrüt yeniden açılmaz.
- Açık anlaşma yok ve hami ilişkisi 0 ise tek görünür eylem: **“Siyasi sorumluluğu üstlen”**. İlgili mevcut temerrüdün Güç maliyetini uygula (kraliyet −6 / meclis −4 / ordu −5), ilgili yerine-getirme ilişki kazanımını uygula (+4). Bunlar mevcut nominal etkilerdir; yeni oran eklenmez. Altın, tahıl, bölge veya kurum desteği değişmez; geri ödeme sayılmaz ve anında kaynak vermez.
- Telafi kaynak veya asgari Güç gerektirmez; Güç mevcut kuralla 0'a sıkışabilir. Böylece güven katmanı fakirlik yüzünden kalıcı kilit eklemez. İlişki artık 4 olduğu için eylem kaybolur; tekrar tekrar ilişki toplama yoktur. Yeni yardımın Güç/cooldown koşulları ayrıca korunur. Mevcut ağır açlık çöküşünün başka bir toparlanma sorunu olduğu açıkça ayrılır.
- İleti birlikte gösterir: “Valcourt yeni avans vermiyor: ilişki 0. Sorumluluğu üstlen: Güç −6, ilişki +4. Sonraki avans için ayrıca Güç 10 ve hafta X gerekir.” Taç desteği 0 iken bu seçimin onu yükseltmediği önizlemede görünür.

## En küçük API ve kabul akışları

Yeni kalıcı alan gerekmez: rol→hami eşlemesi ve mevcut ilişki yeterli. Önerilen `GetPatronRepairTerms`, `CanRepairPatronTrust`, `RepairPatronTrust` aynı ActionResult/nominal-effect önizleme yaklaşımını izler. Kontrol geçmeden hiçbir alan/günlük değişmez. Dilekçe önceliği korunur; açık söz için telafi yerine anlaşma paneli açılır.

1. Valcourt/taç 0, Güç31, süre dolmuş: avans reddedilir; telafi sonrası ilişki4/Güç25/taç0; avans artık normal koşullarla mümkündür. Tekrar telafi reddedilir.
2. Valcourt0, açık150-altın söz: ödeme hâlâ aynı fiyatla geçerli; ilişki4/taç+5. Eski sözün yeni güven kuralı yüzünden daha pahalılaşmadığı arşiv turunda da doğrulanır.
3. Dumas0 ve bütün ekonomik kaynaklar0: telafi mümkündür; yeni askeri yardım hâlâ mevcut Güç/yaşayan ordu koşullarına bağlıdır. Meclis0 için tek kullanımlık `negotiate` yeniden icat edilmez. Normal eylemler ve haftalar güven gerekçesiyle bloke edilmez.

Bu öneri önceki temerrüt maliyetine ek yeni siyasi bedel getirir; uygulanmadan önce sonraki 24 haftalık rota ile ölçülmelidir. Doğrudan patron güvenine odaklanan sınırlı bir seçimdir, kapsamlı diplomasi değildir.

## 0b108de sonrasında uygulama denetimi

`CampaignPatronTrust.cs`, `PatronTrustTests.cs` ve `CanIssueMandate/ResolveMandate` salt okunur incelendi. Root'un bildirdiği gerçek kapı92 Unity testi PASS; ayrıca22 haftalık güven akışı GREEN. Bu denetimde yeni süreç çalıştırılmadı.

- Geçerli sefer durumundan ulaşılabilen yeni ücretsiz döngü veya eski sözün fiyatını değiştiren hata bulunmadı. Telafi `NextMandateWeek` alanını değiştirmez; ilişki0→4 olduğunda ikinci telafi reddedilir. Açık söz telafiyi engeller ama kendi eski tutarıyla ödenebilir.
- `GetPatronRepairTerms` önceki önerinin nominal bedeli yerine kalan Güç ile sınırlanmış **gerçek** bedeli gösterir: örneğin Güç2,5 için kraliyet maliyeti2,5. Bu, uygulanan tutarla tutarlıdır. Güç0'daki telafi yeni yardımın Güç10 eşiğini kaldırmaz.
- “Telafi → avans → hemen temerrüt” tekrar edilebilir bir siyasi stratejidir; aynı haftada sınırsız kaynak değildir. İlk ihraç dört haftalık beklemeyi kurar, ihlal/telafi Güç harcar. Güç sınırına yakınken gerçek toplam bedelin sınırlanması mevcut0 tabanının açık sonucudur; yeni kaynak üretim açığı değildir.
- Dumas ilişkisi0 iken mevcut zafer +2 ilişki vererek yeni yardım erişimini onarabilir. Yürüyüş ve gerçek muharebe gerektiren bu yol yararlı doğal toparlanmadır; diğer patronlara yanlışlıkla uygulanmaz.
- Mevcut13 test önizleme saflığı, yalnız kişisel değişiklik, reddin atomikliği, üç eski söz, sıfır stok/Güç, kesirli maliyet, dilekçe önceliği, doğal beş temerrüt ve eski rolü kapsıyor. Küçük ek entegrasyon önerisi: bekleme sırasında telafi → yeni ihraç reddi; süre dolunca ihraç/erken ihlal/telafi → aynı haftada ikinci ihraç yine reddi. Kaynakta bu davranış doğru; birleşik akışı ayrıca sabitlemek yararlı olur.
