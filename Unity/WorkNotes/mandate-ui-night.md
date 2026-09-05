# Rol kesiti — belge arayüzü

- `ROLE_SLICE.md` okundu. Üç sunum karşılaştırıldı: yalnız sayı defteri, destekçi mektubu, eş sözleşme kartları. Destekçi mektubu seçildi: mevcut farklı portreler, krem kâğıt, vade geldiğinde şarap rengi üst kuşak. Yeni rol seçimi root kapsamındadır.
- Sahiplik: `CabinetHud.cs`, yeni `MandateDocument.cs`, `roles-ui.json` ve iki meta dosyası. `GameApp`/çekirdek değiştirilmedi. Cabinet dört67px sekme kullanır; diğer üç belgenin hesap ve davranışı korunur. `OpenDocument(string)` dış seçime açıktır.
- Manda sekmesi rol, destekçi/ilişki, ayrıcalık, bölge, iki haftalık tarih ve üç etki grubunu gösterir. Yeni emir için `GetMandateTerms`; açık söz için `GetObligationTerms`. Açık sözün ilk bölgesi/tarihleri korunur; erken yerine getirme veya bozma core CanResolve reddini gösterir. Etki rakamları çevrilerde kopyalanmaz.
- `MandatePresentation` ortak biçimlendirme: RoleName, PrivilegeName, PatronId, PortraitIndex, Date ve Effects. Etkiler nominaldir; 0–100 sınırı kısa açıklamayla belirtilir. Geçmişe ait Immediate metni fiilen uygulanmış değer gibi sunulmaz, kabul edilmiş ilk koşullar diye adlandırılır.
- Vade belgesi ekmek dilekçesi varken çizilmez. İki açık seçim, mevcut kaynaklar, varsa gerçek yetersizlik nedeni ve belge üzerinde RU/TR düğmeleri vardır. Tarihler hafta0=5Mayıs1789'dan hesaplanır.
- Statik durum: `roles-ui.json` JSON parse çıkış0; `git diff --check` temiz. Core API yazımı sürerken kaynak derleme yapılmadı. Unity/build/player açılmadı; sonraki gerçek RU/TR karelerde özellikle metin sarma, taşma ve ilk bölge/vade doğrulanacak.
- Root daha sonra Runtime14/Editor3 Roslyn derlemesini PASS bildirdi. Bu derlemedeki `Component.light` ad gölgeleme uyarısı için yerel GUIStyle alanı `lightText` yapıldı. Yeni ekran görüntüsü kabulü henüz yok.

## İlk gerçek rol görüntüleri ve küçük takip

- `output/verify/roles-contract-20260905-225124-107-d5fcba34/shots` içindeki01/02rol seçimi,03/08/12manda sekmeleri ve05/06/10/13/14vade mektupları gerçekten incelendi. İki dilde rol ve mektup metinleri okunuyor; Dumas mektubu arka planda Normandie seçiliyken doğru ilkÎle-de-France bölgesini gösteriyor. Artı işaretlerinin `L.Text` tarafından sayı diye yeniden yorumlanıp silinmesini root düzeltiyor.
- Somut kusur: ayrıcalığı kullanma düğmesi uzun destekçi açıklaması ve tüm koşulların altında, ilk görünümde yok. Yazıyı küçültmek veya sonuçları saklamak yerine erken bir belge içi gezinme düğmesi eklendi: "Условия и подпись ↓ / Koşullar ve imza ↓". Bu düğme işlem yapmaz; ilk etki grubuna kaydırır. Üç etki grubu gerçek imza düğmesinden önce korunur; genel uygunluk açıklaması etki gruplarının altına taşındı, yalnız bölüm aralıkları biraz sıkılaştırıldı.
- Açık sözde aynı işaret koşullara gider; ilk ödeme/bozma düğmelerinin anlamı ve çekirdek kontrolleri değişmedi. `CabinetHud.ShowMandateTerms()` otomatik görüntü incelemesinin aynı kullanıcı gezinmesini çalıştırmasına açıktır.
- Takip kareleri gerekli: her üç rolde ShowMandateTerms sonrası RU/TR alt koşullar ve gerçek imza; açık sözde erken ödeme ve reddin kaynak yetersizlik nedeni. Kaynak değişikliği bu kareler olmadan görsel olarak tamamlandı sayılmaz. Unity/build/player başlatılmadı.

## Hami güveni arayüzü — kaynak hazır

- `patron-trust-plan.md` ve root sözleşmesine göre yalnız `CabinetHud.cs`, yeni `trust-ui.json` ve meta değiştirildi. Ayrı pencere yerine portre altındaki kâğıt notu seçildi; yeni ekran veya konsey düzenlemesi gerekmedi.
- Açık söz yokken hami ilişkisi tam 0 ise normal kimlik açıklamasının yerini role özgü ret alır. Altındaki açık renkli, şarap kenarlı not gerçek `PowerCost` harcamasını ve `RelationshipGain` kazanımını çekirdekten gösterir. 0 maliyet, yanıltıcı bir `+0` yerine `0` görünür; para ve tahıl koşulu arayüzde uydurulmaz.
- “Признать ответственность / Sorumluluğu üstlen” yalnız `CanRepairPatronTrust` onayında kullanılabilir; varsa gerçek ret gerekçesi görünür. Eylem `app.RepairPatronTrust()` çağırır, belge başa döner. Eski bölgesel zarar ve kurum desteği kaybının kaldığı, kaynak gelmediği düğmeden önce açıkça yazılır.
- Açık sözün kimlik metni ve tüm sabit şartları korunur. Açık söz olmayan normal görünüm artık kesin destek vaat etmek yerine yeni avansı/yardımı “görüşmeye hazır” diye anlatır; metinler yeni `ui.trust.identity.*` anahtarlarındadır. Yeni sözün bütün şartları ve çekirdeğin imza reddi aşağıda kalır.
- JSON ayrıştırma ve `git diff --check` geçti. Unity, derleme veya player başlatılmadı; kaynak root entegrasyonuna hazır. Gerçek RU/TR karelerde ilişki 0, onarım sonrası 4 ve 0 ilişkiyle açık söz özellikle doğrulanmalı; onarım düğmesinin ilk görünüm içinde kalması görsel kabul şartıdır.
