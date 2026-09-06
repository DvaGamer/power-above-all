# Sonraki kişisel siyaset adımı

6 Eylül2026. Yalnız karar notu; Core, Assets veya araçlar değiştirilmedi, hiçbir süreç çalıştırılmadı. Rollerin nihai kimliği ve yeni sayılar çalışma varsayımıdır. Kurgu kişiler kullanılır; yeni tarihsel iddia yoktur. Kaynak: mevcut `CampaignRoles`, `CampaignPatronTrust`, `CampaignVictoryDecisions`, `CampaignOfficerCommission` ve onları bağlayan `CampaignCore`.

## Bugünkü bağ ve kalan boşluk

Subay beratı artık oyuncunun seçtiği gerçek bir yetki devridir: aynı kampta ilave200 kişi, eski gerçek kaynak bedeli, Dumas'ya sadakat+1; hak geri alınmadan bütçe ordusuna geçilemez. Oyuncu işi bir kişiye yaptırıp sonra yönetimi devralabilir. Ayrıca zafer primi sadakat+5 verir; Dumas'nın Ambition>Loyalty koşulu tanınma ve yiyecek toplamasındaki Power bedelini gerçekten belirler. Dolayısıyla yeni bir bağlılık sayacı gerekmez.

Bu bağlantının bulunabilirliği kritik. Son UI kaynağı ayrıca okundu: `Presentation/CabinetHud.Council` Dumas kartında `CommanderPoliticalTerms` ve officers bağlantısı veriyor; bölge panelinde normal alımdan sonra da officers girişi var. `CabinetOfficerCommission` sadakat/hırsı ve mevcut gider karşılaştırmasını gösteriyor. `CabinetArmyEstablishment` commission engelinin yanından officers'a götürüyor; officers da establishment'a dönüyor. Bunları eksikmiş gibi yeniden önermiyorum. Kalan soru, oyuncunun mevcut bağlantıları ve bedelleri normal kullanımda fark edip etmediğidir.

## Üç farklı küçük mekanik yön

### A — Zaferin siyasi sahipliği

Oyuncu mevcut açık zafer kararında kendi liderliğini öne çıkarır: Dumas'nın sadakatini veya kişisel ilişkisini tüketerek ek kişisel Power kazanır. Diğer yollar mevcut tanınma, ücretli prim ve ilave siyasi işlem yapmamadır. Mevcut `PendingVictoryId` aynı tek karar hakkını sağlar; yeni süre, sayaç veya düzenli ücret yoktur.

Zincir oyuncu→Dumas→zaferin yönetim üzerindeki etkisidir. Orduyu büyütüp kazanmak, generali güçlendirmek veya başarıyı kendine mal etmek ayrılır. Sonraki Dumas girişimi, tanınma fiyatı ve askerî rolün kişisel erişimi, mevcut ilişki/sadakat kuralları üzerinden etkilenebilir.

**Önemli sınır:** bugünkü başlangıçta sadakat60, hırs80; hırs zaten sadakatten yüksek. Sırf sadakati daha da düşürüp hemen Power veren seçenek ilk zaferlerde bedeli hissedilmeyen güçlü bir tercih olabilir. `Influence` veya yeni bir ihanet olasılığını yalnız bu fiyatı gerçek göstermek için eklemeyelim. Bu nedenle A'ya henüz fiyat önermiyorum; karşılaştırmalı gerçek kampanya olmadan uygulama önceliği vermem. Aynı diyaloga bir düğme eklemek teknik olarak küçük, denge açısından otomatik olarak küçük değildir.

### B — Bağlı komutanın kişisel ricayı kabul etmesi

**Mekanik ekleme seçilecekse önerim B.** Açık subay beratında yeni isteğe bağlı yol: Dumas kazanılmış bağlılığı sayesinde tayin hakkını para almadan oyuncuya bırakabilir. Bedeli güvenin kullanılmasıdır; ödeme yaparak yönetimi devralma ve hakkı açık tutma seçenekleri aynen kalır.

Dar aday sözleşme: yaşayan ordu, açık berat ve **Loyalty>=Ambition** gerekir. Oyuncu rica ederse berat kapanır, Dumas sadakati−5 olur; altın, asker, Manpower, kazanılmış savaş sonucu ve o haftanın ExtraRecruitUsed alanı değişmez. Aynı hafta yeniden imza kullanım hakkını yenilemez. Bu aynı−5 adayı, mevcut primin +5 kazanımıyla okunabilir; henüz root tarafından onaylanmış sayı değildir.

Sonuç yalnız daha ucuz ödeme değildir: eşikteki komutanın sadakatini kullanınca hırs yeniden üstün gelir, sonraki tanınma/yiyecek toplaması gerçek Power bedeli taşıyabilir. Para ödeyen oyuncu bu kazanılmış uyumu korur. Böylece “adam bana bağlı; bu bağlılığı şimdi kullanmalı mıyım?” sorusu, mevcut savaş ve devlet maliyesine bağlanır. Başlangıçtan ücretsiz yetki devri değildir. Loyalty100/Ambition100 durumunun önceki emekle kazanılmış olması korunur; yeni teklif yalnız oyuncu isterse o avantajın bir bölümünü harcar.

Asgari API: mevcut `OfficerCommissionTerms` içine uygunluk/gerekçe ve gerçek sadakat deltası; `CanRequestCommissionReturn(state)` / `RequestCommissionReturn(state)`. Yeni kalıcı alan gerekmez. Aynı petition/mandate/calendar guard'ları, arşivde zaten var olan hak ve Used kullanılır.0 asker varsa mevcut ücretsiz normal geri alma en iyi yoldur; yeni rica sadakat harcatmaz.0 Power bu rica veya mevcut geri alma için engel değildir. Bağlılık yetmiyorsa eski altınla geri alma ve normal ekonomik toparlanma açıktır.

**Sınır:** doğal erişim geç olabilir. Dumas'nın hırsı zaferle artar; her bonusla yalnız farkın bir kısmı kapanır. Tek savaşlık gösterime doğrudan state yazarak ulaşılabilir oyuncu davranışı demeyelim. Gerçek bir uzun kampanya kaydında eşik oluştuğunda bu seçeneğin değeri yüksektir; ilk on dakikayı iyileştirmek için uygun ilk araç olmayabilir.

### C — Siyasi saf değiştirme

Oyuncu açık sözlerini kapattıktan sonra başka mevcut patronun desteğine geçer. Bu, generali kendi tarafına çekmekten farklı bir deneyimdir: oyuncu ilişkiler arasında manevra yapar ve hangi kişinin aracılığıyla devlet işi göreceğini değiştirir. Saray avansı, meclis uzlaşması veya askerî yardım mevcut gerçek yetkilerdir; yeni kredi türü veya vergi tatili kopyası eklenmez.

Mevcut `RoleId` güncel siyasi makam olarak yorumlanırsa başka sayaç gerekmeyebilir. Açık `Obligation` varken geçiş reddedilir; `NextMandateWeek` korunur, başka role geçmek yeni yardımın bekleme süresini sıfırlamaz. Eski patronla ilişki kaybı ve gerçek kişisel Power bedeli düşünülebilir; yeni patronun erişim şartı kendi mevcut ilişkisidir. Subay beratı, ordu sınırı, açık vergi anlaşması veya savaş geçmişi değişmez.

**Sınır:** şu an RoleId başlangıç makamıdır. Onu serbestçe değiştirilebilir patron ilişkisi saymak teknik kolaylık uğruna oyuncu kimliğini değiştirebilir. Kullanıcı nihai rolleri henüz seçmedi. Bu yüzden C ilk küçük patch olmamalı; “makam mı, ittifak mı değişiyor?” kararını açıkça vermeden mevcut alanı yeniden anlamlandırmayalım.0 Power için geçiş bedeli ertelenebilir; normal oyun ve eski patronla toparlanma yolu asla geçişi zorunlu kılmamalıdır.

## Kalan oturumu mevcut seçenekleri anlaşılır yapmaya ayırma seçeneği

**Şimdilik önerim, yeni mekanik yerine erişim ve neden-sonuç görünürlüğü.** Bu yalnız metin cilası değildir: oyuncunun zaten sahip olduğu araçlarla kendi siyasi planını kurabilmesini sağlar. Yeni aylık haber, görev listesi sayacı veya genel zorunlu eğitim penceresi gerektirmez.

1. **Bulunabilirlik deneyi:** oyuncuyu mevcut normal alım düğmesinden başlat; ekran dışındaki Dumas kartının yerini veya officers komutunu söylemeden ikinci grup yolunu bulmasını gözle. Kart bağlantısını çoğaltmadan önce mevcut bölge girişinin işe yarayıp yaramadığını gör. Gerçek mouse/keyboard rotası komutla doğrudan `panel officers` açmaktan daha güçlü kanıttır.
2. **Belgede karar önceliği:** mevcut officers belgesinde imza/geri alma düğmeleri alım açıklamaları ve yetki bedelinden sonra gelir. Yeni görüntülerde gerekli kaydırma ve ilk ekrandaki bilgi değerlendirilsin. Sorun doğrulanırsa bugünkü karar, gerçek fiyat ve sonraki etkiden oluşan kısa üst özet; ayrıntılı kurallar aşağıda önerilebilir. Bedeli görünmeden hızlı imza düğmesi koymak veya bütün hükümleri tekrarlamak amaç değildir.
3. **Yolun uçları:** budget engelinden zaten var olan officers bağlantısına geç, yetkiyi geri al, yine mevcut bağlantıyla budget'a dön.0 asker, yetersiz altın ve Used=true durumlarında bu yolun anlamı ve odak/scroll davranışı ayrı gözlemlensin. Mevcut sıfır-gider yanılsaması düzeltmesi ve gerçek azaltma/ilk düşük bütçe tarihleri korunmalı; kaynaktaki düzeltme taze RU/TR görüntüsünde gerçekten okunuyor mu diye bakılmalı.

Bu kapsam mevcut kişi kartı ve belgeler üzerinde bir kullanıcı yolu denemesidir; sorun görülmeden yeni panel veya tekrar bağlantı ekleme önerisi değildir. Kabul kanıtı: yeni bir oyuncunun normal alım/berat seçimini, hakkı geri alıp küçülmeyi ve0-ordu toparlanmasını kod adları veya ezberlenmiş komutlarla değil görünür arayüzle tamamlaması. Root'un doğal savaş rotası, ücretin gerçek kayıpla değiştiğini ve açık hak+PendingVictory'nin yüklemede korunduğunu gözlemlemek için hazırdır; çıktıları görülmeden geçti denmemeli.

## Karar önerisi

Önce mevcut oyuncu yolunu görünür kıl ve doğal savaş→prim→yetki geri alma→bütçe rotasının sonuçlarını incele. Ardından bir yeni mekanik seçilecekse **B**, mevcut kazanılmış sadakati açık bir pazarlık aracına dönüştürür; yeni kalıcı sayaç veya dönemlik bedel eklemez. A'nın ilk zaferlerde gerçek bedeli ve C'nin oyuncu kimliği belirsizliği çözülene kadar ikisini uygulamayalım.
