# Bölgesel anlaşma — arayüz kaynak incelemesi

Durum: yalnız kaynak incelemesi; yeni anlaşma kareleri henüz verilmedi. `CabinetRegionalAccord.cs`, ilgili `CabinetHud` çağrıları, `accords-ui.json`, `CampaignRegionalAccords` terimleri ve mevcut alım/yürüyüş koşulları incelendi. Assets değiştirilmedi. Unity/oyuncu/derleme başlatılmadı; geçit veya başka dekor yeniden düzenlenmedi.

## Doğru korunmuş sunum

- Dört eski sekme korunur; konseydeki teklif kısa bir girişten iç belgeye gider. İç belgede Morel, ilk bölge, dört hesap, o andaki gelir, sözün tutulması/bozulması ve rol sözünden ayrılık açıklanıyor.
- Vergi tatili borç gibi sunulmuyor; kayıp gelir sonradan istenmiyor. Dördüncü hesap sonrası sonuç otomatik ve ayrı bir ödeme seçimi gerektirmiyor. Daha sakin bölgeden barışçıl geçiş mümkün olsa da yeni asker/kıtlık yüzünden direnç geri gelebilir; metin geçiş garantisi vermiyor.
- Aktif belge `GetActiveRegionalAccordTerms` kullanır; haritada başka yer seçilse de ilk bölge aynı. “Bu ili göster” bağlama dönmek için yeterlidir. İki sistemin ayrı taahhütleri birleşmiyor.

## En küçük üç takip

1. **Sol vergi uyarısı ve kalan rapor.** İlk uzun uyarı, bütün emirler kullanılabilir Paris görünümüne yaklaşık 60–70 px ekleme riski taşıyordu. Root bunu inceleme sırasında üç açık satıra çevirdi; beş sayı da `accordPreview.Break` üzerinden geliyor. Bu değişiklik doğru: etikette zaten sözün bozulduğu yazdığı için aynı açıklamayı tekrar etmek gereksiz. Tam bedeli “bölge tekrar kızar” gibi belirsiz bir cümleye indirmeyin. Mevcut üç satır korunarak önce gerçek kare görülmeli. Kaynakta `reportHeight = Max(1, 786-reportTop)` vardır; bu, raporun kullanılabilir bir yüksekliğe sahip olmasını garanti etmez. Ordusu Paris'te, ekmek/vergi/asker toplama emirleri henüz kullanılmamış, kaynakları yeterli ve Paris anlaşması aktif olan durumda tax warning + recruit ilk fiyatı + upkeep + Paris desteği birlikte görünmeli. İkinci dar kontrol, aynı yerde dört ayrı işe alım kaynak eksiğidir; uzun RU kaynak adı ek satır doğurabilir.

   İnceleme sırasında ileri sürülen “Paris dışında ordu + Hungry + kullanılabilir recruit” en kötü durum varsayımı **geçersizdir**: `Act(recruit)` ordunun kendi bölgesini ister. Bu düzeltme root'a hemen iletildi; geçersiz birleşim kabul senaryosu yapılmamalı. Dışarıdaki orduyla açılan açlık açıklaması, işe alım fiyat/upkeep satırlarının yerini alan kısa ret tarafından kısmen veya tamamen dengelenir. Gerçek yeni kare olmadan taşma veya kesin piksel yüksekliği iddia edilmez.

2. **Önizleme gelir kaybının zamanını doğru söyle.** İmzasız `GetRegionalAccordTerms.TaxForgone`, bölgenin imzadan sonraki `Immediate −10/+3` durumunda istisnalı/istisnasız gelir farkıdır. Şimdiki `ui.accord.forgone` “при нынешнем положении / mevcut koşullarda” diyor. Aktif belge için doğru; teklif için “после уступки / ödünden sonraki koşullarda” daha kesindir. Bunun yapılması, ülkenin `CurrentTaxIncome → ProjectedTaxIncome` değişimi ile `TaxForgone` farklı çıktığında yanlış fiyat izlenimini azaltır. Sayıları birbirine eşitlemek veya dörtle çarpmak gerekmez; küçük koşullu metin yeterlidir.

3. **Uzun iç belgenin imzasını gerçek aşağı kaydırma karesinde kontrol et.** Portre, bölge, teklif, gelir açıklaması ve iki sonuç grubu nedeniyle belge ilk 584 px görünümden uzundur; kaynak bu uzunluğu ölçüp kaydırır. Ayrı bir ekran gerekmez. `accord-campaign.script` içindeki 520/1100 kaydırma ve RU/TR imza kareleri yeterli ilk kanıt olacaktır. İmza yanında doğru bölgeyi/gelir bedelini bulmak güçse kısa bir bölge/tarih satırı veya aynı belge içinde sonuçlara giden işaret düşünülebilir; henüz görülmemiş kadraja varsayımla yeni gezinme eklemeyin. Ana eylemden önce tam koşullar korunmalı.

## Root tarafından zaten yapılan düzeltme

- Vergi uyarısı üç satırdır: RU “Вдобавок: волнения … / Контроль … · Морель … / Сословия … · власть …”; TR eşdeğeri. Beş değer `Change` ile gerçek `Break` alanlarından gelir. Kaynakta bu güncel sürüm yeniden okundu; eski sabit sayılarla ilgili risk kapanmıştır.
- Görsel kabul için Paris/RU/TR sol emirler ve vergi uyarısı, belge teklif/alt imza ve başka bölge seçiliyken aktif ilk bölge kareleri beklenir. Root kendi Assets sahipliğini ve build kapısını yürütür.

## İlk gerçek anlaşma kareleri

- Root `regional-accord-20260906-002323-844-cee27fbb` kapısını GREEN bildirdi. Bu incelemede aynı dizinin `shots/05-accord-offer-tr.png`, `06-accord-sign-tr.png`, `08-tax-forecast-ru.png`, `12-paris-accord-tax-warning.png` dosyaları gerçekten açıldı. Diğer on kare yeniden incelenmiş gibi sayılmadı; root RU teklif/imza karesini ayrıca gördü.
- TR teklifte portre, bölge ve anlık etkiler okunuyor. Sonraki dört hesabın tarihi ve ülke vergi geliri `207 → 197` açık. Yeni “Ödünden sonra…” metni, `12` livre karşılaştırmasının ilk sakinleştirmeden sonraki bölgeye ait olduğunu doğru açıklıyor; eski zaman ifadesi riski kapandı.
- TR alt belgede tutulma/bozulma sonuçları, yeni anlaşma bekleme süresi, ayrı görev sözü ve tam imza düğmesi görünüyor. Üstteki önceki paragrafın yalnız son satırlarının kalması normal kaydırma kırpmasıdır; metin veya düğme çakışması yok. Yeni gezinme işareti veya pencere ekleme gerekçesi oluşmadı.
- RU ekonomide dahil edilen Champagne tatili, `−12` mevcut karşılaştırma, dört kalan hesap ve toplam `+197` vergi geliri okunuyor. Maaş, teçhizat ve net `+61` aynı görünümde. Alt açıklamanın devamı doğal kaydırma sınırındadır.
- Paris karesinde üç satırlık ilave vergi bedeli gerçekten okunuyor. Vergi etiketi anlaşmanın bozulduğunu söylemeye devam ediyor; işe alım, yürüyüş ve Paris desteği düğmeleri görünür. Alt raporda denetim değeri/çubuğu ile huzursuzluk satırı bulunuyor, kaydırma işareti belirgin. Bu karede ordu Champagne'da olduğundan işe alım kısa konum reddi gösterir; alım fiyatı + haftalık bakım görünen en uzun varyant bu kareyle kabul edilmiş sayılmaz.
- Root'un bulduğu yuvarlama `60.5 → 63.5` değerlerini `60 → 64` gibi gösteriyor; TR teklif karesinde de aynı görülüyor. Root anlık etki önizlemesini bir ondalık haneyle düzeltmeyi üstlendi; bu incelemede Assets değiştirilmedi.
- **Görsel sonuç:** Seçilen dört karede beklenmeyen kırpma, üst üste binen kontrol veya okunmayan imza yok. Geometriyi/sekme düzenini yeniden kurmak gerekmiyor. Paris'te ordu kendi bölgesindeyken bütün emirler açık ve çoklu kaynak eksikleri ayrı karede hâlâ doğrulanabilir; bu sınır korunur. Kaynak freeze sürüyor, Unity/oyuncu çalıştırılmadı.

## Paris bütün emirler ve ilk bölge — sonraki gerçek kabul

- `accord-layout-final-20260906-002826-992-56dba0b4/REPORT.md` tamamlanmış GREEN olarak okundu: 128 Unity testi, taze build, 4 kare/7 assertion/1 state, 10 tarayıcı testi. Ancak bundan sonra aynı dizinin `shots/01-paris-all-orders-ru`, `02-paris-all-orders-tr`, `03-original-region-tr`, `04-original-region-ru` PNG'leri açıldı.
- İlk iki karede ordu Paris'te; alımın ilk kaynak fiyatı, ek haftalık bakım, üç satırlı vergi ihlal bedeli ve Paris desteği birlikte görünüyor. Düğme veya açıklama örtüşmüyor. Alt rapor daralmış olsa da ilk denetim değeri/çubuğu ve kaydırma çubuğu bütünüyle seçiliyor. Bütün emirlerin açık olduğu asıl uzun varyantın belirsizliği kapandı; bu görünümü yeniden boyutlandırmak için somut kesilme kanıtı yok.
- Üçüncü/dördüncü karede harita ve sol rapor Champagne, anlaşmanın sağ belgesi Île-de-France. Dört kalan hesap, ilk bitiş tarihi ve “Bu ili göster / Показать эту провинцию” düğmesi görünür. Etkin sözün bölgesi harita seçimiyle karışmıyor; bağlama dönme eylemi yerinde.
- Çoklu kaynak eksiği ayrı kadrajı bu dört dosyada yoktur; bu olası varyant görülmüş gibi sayılmaz. Mevcut gerçek kabul için yeni düzeltme önerilmiyor. Bu ajan yalnız doküman yazdı; Assets/source/script freeze, Unity/oyuncu açmama kuralı korundu.
