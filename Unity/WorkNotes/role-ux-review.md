# Rol akışı — bağımsız UX incelemesi

Durum: yalnız inceleme; Assets, oyuncu ve derleme değiştirilmedi. Yeni güven notu henüz bu görüntülerde yoktur. Kaynaklar `GameApp`, `CabinetHud`, `RoleSelection`, `MandateDocument`, `CampaignRoles` ve ilgili yerelleştirme tablolarıdır.

## Gerçek görüntü kanıtı

- `C:/Users/USER/AppData/Local/Temp/codex-shot-2026-09-06_02-17-55.png`: eski kampanyanın üzerinde yeni sefer onayı; “Начать заново” düğmesi ve kaydın değişeceğini söyleyen metin.
- Aynı dizindeki `02-18-13`: rol seçimi hâlâ iptal edilebilir; `02-18-22`: eski kampanyaya dönüş; `02-18-44`: meclis kartı seçili; `02-18-54`: meclis görevi başlamış, kaynaklar başlangıç değerlerinde ve sağ belge henüz imzalanmamış koşulları gösteriyor.
- `output/verify/roles-visible-20260905-230302-558-1717bcb0/shots`: `02-appointments-tr`, `03b-crown-terms-ru`, `04b-crown-early-settlement`, `14-army-deadline-ru` incelendi. Son karede arka planda Normandie seçiliyken mektup doğru ilk Île-de-France bölgesini açıkça tutuyor.

## En fazla üç öncelikli iyileştirme

1. **Yeni sefer onayındaki eylemi doğru adlandır.** Onay düğmesi “Начать заново / Yeniden başlat” diyor fakat `CabinetHud.Confirm` yalnız `BeginRoleSelection` çağırıyor; kaydı değiştiren işlem `StartCampaign`. Gerçek `02-17-55 → 02-18-13 → 02-18-22` dizisi bu ara adımın geri alınabildiğini gösteriyor. Aynı pencere korunarak düğme “Выбрать назначение → / Görev seç →”, açıklama “Новая кампания заменит текущую после принятия назначения. До этого можно вернуться. / Yeni görev kabul edildiğinde mevcut sefer değişir. O zamana kadar geri dönebilirsiniz.” yapılabilir. İkinci bir yeni onay eklemeyin. Rol ekranında Escape çalışırken bu eski onayda Escape işleyicisi kaynakta yok; küçük takipte iptalle aynı yolu kullanması tutarlılık sağlar, fakat bu tuş bu incelemede canlı denenmedi.

2. **Önizleme ile verilmiş sözü metinde ayır.** Rolü kabul etmek yardım vermez ve söz başlatmaz (`CampaignCore.Create(roleId)` yalnız rolü kaydeder). Buna rağmen `02-18-54` sağda doğrudan “Срок: 19 мая 1789”, rol kartlarında “Получите / alın” ve “Через две недели / İki hafta sonra” gösteriyor. Kullanıcı ilk yardımın ve iki haftalık sayacın atamayla başladığını düşünebilir. Mevcut kartın/alt açıklamanın içinde “Полномочие применяется отдельным распоряжением / Yetki ayrı bir emirle kullanılır” ve imzasız belgede “Если подписать сейчас, срок: … / Şimdi imzalanırsa vade: …” yeterli. Aktif sözün tarih metnini değiştirmeyin. Aynı anlam düzeltmesi meclis yazısında gerekli: `civic_pledge` şimdi tahıl vermez; şimdi huzursuzluk −18 ve denetim +6 sağlar, sonra 40 gıda teslim edilir. Yeni `ui.trust.identity.assembly` içindeki “вернуть / geri vermek” ve `ui.trust.refusal.assembly` içindeki “помощь продовольствием / tahıl yardımı” yerine “доставить / teslim etmek” ve “поручиться за новое обещание / yeni söze kefil olmak” kullanılmalı. Böylece meclis yetkisi yanlışlıkla tahıl kredisi gibi sunulmaz.

3. **İmza/erken çözüm düğmelerinde sözün yerini koru.** `04b-crown-early-settlement` görünümünde iki eylem okunuyor, fakat bölgenin adı üst kaydırmada kalmış; yalnız “Обещание относится к этой области” görünüyor. Haritada başka yer seçilince ekrandaki tek açık bölge adı sol paneldeki yeni seçim olabilir. Vade mektubu bunu doğru çözüyor; küçük yan belgede de gerçek eylemlerden hemen önce `terms.RegionId` ve `terms.DueWeek` ile kısa bir yer/tarih satırı gösterilebilir. Bu, imza önizlemesinde hedefi, açık sözde değişmeyen ilk bölgeyi tekrarlar; yeni pencere veya harita odağını zorla değiştirmek gerekmez. Kabul: başka bölge seçiliyken aşağı kaydırılmış ödeme/bozma düğmelerinin yanında ilk bölge hâlâ okunmalı.

## Çift tıklama ve korunmuş davranışlar

- İncelenen kaynakta aynı sözün iki kez ödenmesine yol açan bir yol bulunmadı: `ResolveMandate` önce `CanResolveMandate` ile sabit kimliği denetler, ilk başarıdan sonra `Obligation = null` yapar; yinelenen kimlik reddedilir. İmza mevcut söz ve bekleme aralığı tarafından engellenir. `StartCampaign`, seçim kapanınca ikinci çağrıyı reddeder. Bu sonuç kaynak incelemesidir; yeni çift tıklama oyuncu testi yürütülmedi.
- İlk onay düğmesinin koordinatları meclis kartının alanına denk gelir. Kullanıcı o düğmeye çift tıklarsa ikinci tıklama yeni rol ekranında meclisi seçebilir; bu koordinatlardan çıkarılan bir olasılıktır, kayıt değişimi veya canlı yeniden üretim kanıtı değildir. Son kabul hâlâ ayrı düğme olduğundan üç öncelikli kusurdan biri yapılmadı.
- Dilekçe önce, vade mektubu sonra çizilir; arka plandaki kabinet devre dışıdır. Vade belgesi iki sonucu, kaynak kontrolünü ve kendi RU/TR kontrollerini koruyor. İlk bölge/tarih bilgisi `GetObligationTerms` üzerinden geliyor.
- Dumas portresinin solundaki küçük leke gerçek hem rol hem vade karelerinde var; portre bitmap kaynağına aittir. Bu incelemede raster düzenlenmedi. Sonraki görsel düzenleme imagegen ile ve ayrı kaynak kabulüyle yapılmalıdır.

## Onaylanan dar takip uygulandı

- Root üç öneriyi ve meclis metni düzeltmesini kabul etti. `CabinetHud.cs`, `trust-ui.json`, yeni `role-clarity.json` ve metası güncellendi; başka Assets değiştirilmedi.
- Eski onay penceresi korunuyor. Düğme artık rol seçimine gittiğini ve açıklama arşivin yalnız atama kabul edilince değişeceğini söylüyor. İmzasız ayrıcalıkta atamanın henüz söz olmadığı ve ayrı emir gerektiği yazıyor; tarih “şimdi imzalanırsa” diye koşullu. Aktif sözün ödeme/bozma bölümünde ilk bölge ve vade, `GetObligationTerms` değerleriyle tekrar ediliyor; yükseklik `Paragraph` üzerinden ölçülüyor.
- Meclis güven metni artık tahılı teslim etmeyi ve yeni söze kefaleti anlatıyor; tahıl kredisi veya geri verme iddiası yok. Escape eklenmedi; bu takip yalnız kabul edilen metin/bağlam sınırındadır.
- İki JSON ayrıştırması ve `git diff --check` PASS. Unity/oyuncu/derleme/commit yapılmadı. Kaynak root genel derlemesi için freeze; sonraki karelerde artan paragraf yüksekliği ve alt bağlam görünürlüğü doğrulanmalı.
