# Sabit bir aktif sivil politika —24 haftalık karşılaştırma

Kaynak: `ActiveCivilPolicyProbe.cs`, public Main sınıfı `ActiveCivilPolicyProbe`. Yalnız WorkNotes'ta yeni deney kaynağıdır; oyun veya test kaynakları değiştirilmez. Bu ajan çalıştırmadı, sonuçlar henüz bilinmiyor.

**Soru:** başkentteki ordunun üç yakın önceliğini —Champagne, Normandy, Picardy— mevcut sivil araçlarla korumak mümkün mü ve bunun gerçek bedeli nedir? Bütün ülkenin mutlaka sakin olması, zafer veya en iyi politika hedeflenmez. Öncelikler baştan sabittir; sonuçlara bakarak bölge değiştiren arama, rastgele tarama veya geleceği gören state kopyası yoktur.

Her iki rota legacy Create, hafta0 budget1000,24 başarılı hafta, hafta2 aynı negotiate. Ordu Ile'de kalır; alım, savaş, hareket, vergi, rol ayrıcalığı ve NPC veto yoktur. Pasif rota önceki receipt'in Gold2292/Food61/Troops1000/Manpower2600/Power67,11 düşman bölge ve enemyMax2228 sonucunu yeniden kontrol eder; uyuşmazsa karşılaştırma sessizce farklı baseline kullanmaz.

Aktif rotanın her haftanın başındaki sabit karar sırası:

1. Başkent sübvansiyonu kapalı ve Food≥240 ise aç. Açıksa, mevcut Forecast ile hesabın sonunda80 Food kalmayacaksa kapat. Yeniden açılması tekrar240 Food gerektirir; aynı dar stok aralığında sürekli aç/kapat yoktur. Kapatmanın mevcut siyasi bedeli korunur.
2. Aktif sözleşme yoksa üç öncelikten Unrest≥65 olan en huzursuzuna vergi tatili teklif et. Eşitlikte öncelik dizisi kullanılır. Core'un cooldown/diğer retleri kaydedilir; veri değiştirilerek atlanmaz.
3. Unrest≥55 olan en huzursuz önceliğe haftada en fazla bir bread:40 Food harcandıktan sonra80 ve mevcut Forecast'in negatif NetFood büyüklüğü kalabiliyorsa. Bu bugünkü ihtiyaca karşı muhafazakâr bir stok koşuludur; bread'in varsayılan gelecekteki üretim kazancı önce harcanmaz. Uygun hedef veya stok yoksa neden yazılır.

Bu eşikler **deneyin açıklanmış heuristic tercihleri**dir, yeni oyun kuralları değildir. İlk haftada kaynak aritmetiğine göre Champagne tatili69→59, ardından bread59→44; bread şehir onayına+2 verir. Başkent ödemesinin+3 etkisiyle urban35'in40 sınırına ulaşabilmesi, kontrol edilecek önemli ortak mekanizmadır. Bu erken müdahalenin bütün ülkeye dolaylı fayda sağlaması mümkün; bunu yalnız üç bölgelik doğrudan etki diye sunmayacağız. Sayısal sonucun gerçekleştiği henüz iddia edilmez.

**Zamanlama ve ölçüm:** hafta0 emirlerinden sonra ilk durum; sonra her başarılı hesabın zorunlu dilekçesi çözülür, hafta24 dışında gelecek hesap için aynı politika uygulanır ve durum yazılır. Hafta24'te25. haftaya ait yeni masraf yapılmaz. Böylece STATUS, o haftanın yönetim kararları sonrasındaki görünür düşman payıdır. Hem12 ülke bölgesi hem3 öncelik ayrı sayılır; nihai tablo12 bölgenin gerçek U/C/E ve direnişini karşılaştırır.

Her gerçek emir COMMAND satırında başarı/ret, kaynak ve siyasi farklarıyla; oluşturduğu yeni journal kayıtları ayrıca yazılır. Heuristic nedeniyle verilmemiş emir DECISION'dır, Core reddi gibi sunulmaz. Başarısız gerçek emir save metnini aynen korumalıdır. Zorunlu NextWeek/dilekçe reddedilirse deney açıkça durur; hiçbir guard state enjeksiyonuyla aşılmaz.

SETTLEMENT, gerçek ortak Forecast'in vergi/ordu/üretim/iaşe, sübvansiyon ve Dumas miktarını; Gold/Food sonrası eşitliğini, kayıpları ve yedeğe geçenleri kaydeder. Aktif tatilin **her gerçek hesabındaki** TaxForgone toplanır; dört çarpı sabit bedel yazılmaz. BreadFood gerçek emir stok farkıdır. SubsidyFoodBudgeted talep edilen haftalık bileşendir; aç hafta olursa “tam teslim edilmiş yardım” diye adlandırılmaz. Son kaynaklar, açlık/ödeme haftaları, NPC toplaması ve tüm retler SUMMARY'dedir. Final archive roundtrip doğrulanır.

Bu deney bir politika örneğinin etkisini ve fiyatını ölçer; bütün sivil mekaniklerin ayrı nedensel ağırlığını izole etmez. Üç araç birlikte uygulanır, dolayısıyla başka deney olmadan kazancı yalnız bread'e veya yalnız tatilin imzasına bağlamayız. Savaş yapılmaz; kuvvet sayısından kazanma ihtimali çıkarılmaz. Root'un çalıştırması bitmeden PASS veya başarılı barış sonucu yoktur.
