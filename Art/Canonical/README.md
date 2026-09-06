# Özgün görsel alfabe — ilk dokuz kaynak varlık

Bu SVG'ler elle belirlenmiş path, renk ve kontrollü asimetriyle üretildi; imagegen veya ticari artwork içermez. Bunlar katmanları düzenlenebilir kaynak varlıklardır. Hepsi runtime'da otomatik kullanılan sprite değildir; mevcut Unity mesh yorumlarıyla karşılaştırılarak kademeli aktarılır. Renk/çizgi/malzeme kuralları [ART_PRODUCTION_RULES](../../ART_PRODUCTION_RULES.md).

| Varlık | Dil / kullanım |
|---|---|
| [Tree](tree.svg) | Ayrı gövde, koyu silüet, iki büyük taç kütlesi, az iç iz; kümede karakter korunur |
| [Building](building.svg) | Taş gövde, taşan mavi çatı, asimetrik kule ve kapı; şehir parçalarının tabanı |
| [Road](road.svg) | Kesintili toprak izi; burada örnek eğri, oyun konumu için GIS veya doğruluk etiketi gerekir |
| [River](river.svg) | Kesintisiz mavi akış, ince kıyı; örnek biçim gerçek nehir verisi yerine kullanılmaz |
| [Cloud](cloud.svg) | Geniş birkaç lop ve tek alt gölge, gürültüsüz kenar |
| [Soldier](soldier.svg) | Şapka/baş/gövde/yön/tüfek okunur; tek temel duruş, henüz sekiz pozluk animasyon değil |
| [Musket smoke](musket_smoke.svg) | Beş aşama: flash/küçük/kabarma/seyrelme/artık; ritim gerçek salvo ile ayrıca ayarlanacak |
| [UI icon](ui_icon.svg) | Tahıl;3pxyuvarlak uçlu tek çizgi ailesi, sayı ve etiketle kullanılır |
| [Document ornament](document_ornament.svg) | Metinden düşük kontrast, kırık köşe ve küçük rozet |

Üretim notu: PAA şehir/orman runtime kodu CampaignMap.cs / CampaignMapGeography.cs; VFX TacticalBattle.cs; UI CabinetHud.cs. Bu kaynak paketin hazırlanması haritanın görsel milestone'unu bitirmez. Küçük ölçekte çizgi en az1fizikselpiksel kalmalı; ayrıntı kaybolursa eklemeyin, sadeleştirin.
