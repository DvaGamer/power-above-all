# Bölgesel reform B: bağımsız test kaynağı teslimi

6 Eylül 2026. `regional-reform-contract.md` ve domain ajanının gerçek Core/API kaynağı esas alındı. Bu ajan test, compiler, Unity, oyuncu veya Git başlatmadı. Aşağıdaki sayılar kaynak attribute sayısıdır; PASS sonucu değildir. Çalıştırma ve kabul root'a aittir.

## Kaynak kapsamı

Yeni `Assets/Tests/Editor/RegionalReformTests.cs` ve 32 karakterlik GUID meta dosyası: **62 NUnit case**. Dört gerçek başarılı hesap boyunca eski bütçe, beşinci hesapta iki modun gerçek farkı, değişmeyen özgün bölge tabanları, ayrı AwayFromZero payları ve salt okunur kopuk DTO'lar denetlenir.

Hafta öncesi hazır U64/C55'in gerçek şehir baskısı sonrası U66 ile durması; hazır olmayan U65'in gerçek yüksek şehir desteği sonrası U64 ile ilerlemesi ayrı vakalardır. U/C eşit sınırları, Champagne'da iki haftalık bekleme sonrası gerçek bread ile ilerleme, petition'i doğuran haftanın kazanılmış adımı ve sonraki petition/mandate atomik retleri kapsanır. Sorgu tekrarları haftayı veya adımı ilerletmez. Bunlar Core kurallarıdır; Unity pause/input kanıtı olarak sunulmaz.

Başlangıcın 120 Gold / 4 Power tam bedeli, eksik bedelde atomik ret, tam bedelden sonra sıfır stokla iptal, hazırlanırken veya çalışırken iptal, eski stok/askerlerin korunması, ayrı sponsorla yeniden dört adım ve tam ödeme, clamp'li +4/−8 ilişki, accord önce Morel96→100 yaptığında reform ödülünün gerçek 0 olması ve son takvim haftasında çıkış denetlenir. Son başlangıç MaximumWeek−5; eski duraklayan proje takvim sonunda yüklenebilir ve iptal edilebilir.

Ekonomik karşılaştırma fixture'ları aynı gerçek state'in yalnız reform alanları temizlenmiş kopyasını kullanır. Forecast karşılaştırması DTO serileştirmesine güvenmez; bütün dokuz gerçek int alanını karşılaştırır. Dumas fixture'ında uyarı önce gerçek açlık hesabından doğar; kontrollü birim sınırı için sonra aynı U/Troops şartları kurulur. Bu koşulda üretim artışı forage ihtiyacını azaltırken NetFood farkı 0 kalmalıdır; gerçek hafta tek bir toplama/yerel etki uygular. Birim fixture'ları doğal oyuncu rotası veya taktik savaş kanıtı değildir.

Aynı ve farklı bölgede çalışan tatilin TaxForgone hesabı reform lensini korur; reformun kendi bölgesi muafsa bugünkü TaxIncomeDelta 0 kalır. Asıl BaseTax'tan gelen direniş doğrudan yeniden ölçeklenmez. v8 birleşik state; aktif reform, NPC uyarısı, mandate, accord, commission ve Core battle receipt'i korur. Ayrı yol iki haftalık army reduction'ın dört haftalık hazırlığı erken tamamlamadığını ve 80 Gold mandate borcunu silmediğini doğrular.

## Eski arşiv kontrollerinin korunması

Altı eski test dosyasında yalnız 23 güncel `Version` beklentisi/`Replace` başlangıç sabiti 7→8 taşındı: ArmyEstablishmentTests, CampaignVictoryDecisionTests, DumasInitiativeTests, OfficerCommissionTests, RegionalAccordTests, RoleCampaignTests. Açık `Version:7` bozuk commission fixture'ı eski sürüm kanıtı olarak korundu.

Mevcut sürüm parametreleri silinmedi. Eski alan varlığı/null/tip testlerine **27 ek v8 case** eklendi: army 7, victory 2, Dumas 5, accord 5, role 2, commission 6. Commission testi artık hem v7 hem v8'i ayrı ayrı denetler; eski altı v7 vaka korunur. Yeni dosya ayrıca v1–7 göçünün her sürümde desteklenen eski mekanizmaları koruduğunu, gizlenmiş ücretli projenin eski sürüm etiketiyle silinemediğini, v8'in üç alanı kapalı durumda bile istediğini ve imkânsız bölge/mod/adım bileşimlerini reddettiğini denetler.

Toplam beklenen ilave case **89**: yeni dosyada62 + eski arşiv matrislerinde27. Başka kaynak paketi test eklemezse önceki407 üzerine496 keşfedilmesi beklenir. Eski test davranış beklentileri gevşetilmedi; geçerli kapalı reform varsayılanları eski arşiv göçünde kabul edilen sözleşmedir, yeni migration fixture'ı ise reform alanlarını fiziksel olarak yeniden adlandırarak yokluğunu da sınar.

Domain ajanı Core/Archive source freeze bildirdi ve listelenen eşik/bütçe/accord sırasını sözleşmeyle uyumlu buldu. Bu test kaynağı da root çalıştırmasına teslim edildi; yeni sonuç görünmeden başarı iddiası yapılmaz.
