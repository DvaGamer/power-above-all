# Gece geliştirme raporu — çalışma sürüyor

Güncelleme: 6 Eylül 2026, 01:38 UTC. On saatlik görev **07:22:03 UTC / 10:22 İstanbul** saatine kadar aktiftir. Bu bir ara rapordur; uzun vadeli vizyonun tamamlandığı anlamına gelmez.

## Çalışan sürüm

[PLAY_GAME.cmd](PLAY_GAME.cmd) son doğrulanmış Windows oyununu açar. `node play-game.cjs --check` oyunu başlatmadan seçilen yolu ve dosya bütünlüğünü gösterir.

Son tam kapı: [military-art-final raporu](output/verify/military-art-final-20260906-012710-424-48b0deff/REPORT.md), **GREEN**. 176 Unity testi, yeni Direct3D11 build, 21 gerçek PNG, 38 durum kontrolü, 21 JSON kaydı ve 10 tarayıcı testi geçti. Başlatıcı bu 141 dosyalık build'i `complete-build` olarak seçti. Runtime SHA256: `fc1e21937ace6213b4f62fd20cd2e7727fe465ada5baa3e853f713ce61a4cff6`.

## Oynanabilir değişiklikler

- Kampanya öncesi üç çalışma rolü seçilir. Her rol farklı bir hamiden yardım alır ve iki hafta içinde karşılık vermeyi taahhüt eder. Sözün ilk bölgesi, vadesi ve bedeli korunur. Tekrarlanan temerrüt kişinin güvenini tüketir; yeni yardım için siyasi telafi gerekir.
- Morel aracılığıyla bir bölgeye dört haftalık vergi tatili verilebilir. Direnç azalırken gerçek vergi gelirinden vazgeçilir. Olağanüstü vergi bu anlaşmayı bozabilir; kişisel iktidar, ilişki ve temsilcilerin desteği etkilenir. Eski rol sözüyle birlikte çalışır.
- Gerçek zaferden sonra iki isteğe bağlı karar görünür: Dumas'ya toparlanma yetkisi verip yorgunluğu azaltmak veya kalan askerlere hükümet adına prim ödemek. İlki generalin hırsını da artırır ve sadakat düşükse kişisel güç harcar. İkincisi gerçek hazineyle sadakat ve yerel denetim kurar. Olağan sonuçla devam etmek mümkündür.
- Zafer belgesi iki seçeneği, bedelleri ve sonuçları yan yana gösterir. Esc yalnız pencereyi kapatır; teklif konseyden yeniden açılır. Ayrı ret düğmesi teklifi kapatır. Yeni hafta ek bir cevap kilidi getirmez.
- Arşiv v4 açık ve kapanmış zafer kararlarını saklar. v1–v3 geçişleri, eski rol sözleri ve v3 bölgesel anlaşmaları için doğrulamalar vardır.

## Görsel dil ve savaş

Krem kâğıt, adaçayı yeşili, açık mavi, mercan ve koyu mürekkep birlikte kullanılır. Haritada on iki ayrı şehir silueti, guaj arazi ve kurgu kişilerin resimli portreleri bulunur. Savaş alanındaki tepe ve sığ geçit gerçek arazi kurallarıyla uyumludur.

Duman için ayrı ve açık alfa geçişi kullanıldı. Önceki opak beyaz dörtgen hatası gerçek karelerde giderildi; son duman hafif bir izdir, erken evresi hâlâ zayıf görünür. Bu, bitmiş ve güçlü bir salvo gösterisi olarak sunulmaz. Dumas'nın yanındaki komşu portre kırıntısı yalnız gösterilen kaynak bölgesi daraltılarak kaldırıldı; resim dosyası ve ana siluetin ölçeği değiştirilmedi.

Aynı taktik adımda verilen atışlar birlikte çözülür. Cephanesiz piyade ve süvari yakın temasa girebilir. Gerçek oyuncu rotası 125,803 saniyede zafer, 196 kayıp ve 24 ele geçirilen teçhizat verdi; bu her oyuncunun sonucu için garanti değildir. Yeni birleşik koşuda iki savaş dünyası art arda kurulup kapandı ve eski nesneler yeni karşılaşmaya taşınmadı.

## Kanıtlanan siyasi sonuçlar

1004 sağ kalan asker için prim **84 livre** oldu: hazine840→756, Dumas sadakati60→65, Champagne denetimi70,5→73,5. Başka bir bölge seçmek ödülün yerini değiştirmedi. Önceki açık teklif ile yüklenen kayıt, ayrıca ödenmiş sonuç ile iki ayrı yükleme birebir eşitti.

Ayrı gerçek zaferde yetki verme **güç59→55, yorgunluk35→23, ilişki52→56, hırs83→86** üretti; hazine840 kaldı. Bu kayıt da aynı biçimde korundu. Yetki verildikten sonraki haftanın zorlu yürüyüşünde yorgunluk31 olur; olağan sonuçta43 olur. Bu karşılaştırma formül hesabıdır, ikinci bir savaş sonucu iddiası değildir.

Gerçek Windows girdisi [native-victory kaydında](Unity/WorkNotes/native-victory-input.md) incelendi: Esc, konseyden yeniden açma, TR/RU seçimi ve prim düğmesine fare tıklaması. Sahipli süreç native exit0 ile ve süre aşımı olmadan kapandı. Yeniden kullanılan build nedeniyle sonuç PARTIAL'dır. İlk Start çağrısı açıklamasız exit1 döndürdü; aynı sürecin sonraki Inspect ve altı gerçek girdi çağrısı başarılıydı. Başlangıç hatasının nedeni kanıtlanmadı; sonraki hatalar için açık tanı çıktısı eklendi.

## Kalan sınırlar

Kesin rol kadrosu, kampanya sonu, görevden düşme sonrası devam ve geniş alternatif tarih yolları hâlâ [VISION.md](VISION.md) içindeki açık kararlardır. Yeni NPC girişimi yalnız tasarım önerisidir. Mevcut sayılar tarihsel istatistik olarak sunulmaz.

Shift ile çoklu seçim ve elle nişanlı salvo bu son native pencere testinin kapsamı değildir. Sesin dinlenerek kalite kabulü tamamlanmış sayılmaz. Yeni dumanın erken okunurluğu ve sıkışık savaş etiketleri sonraki görsel iyileştirme alanlarıdır.

Kullanıcının kişisel kayıt dosyası korunmuştur; son okunan SHA256 `18f3c57d89161fc471bc0aa997c8266c01d50ba1def670cd476081f1f5f7b63e`. Testler ayrı kayıt dizinleri kullandı. Eski güvensiz `tools/night.ps1` ve `tools/night-prompt.txt` çalıştırılmadı. Ayrıntılı tarihçe [NOTES.md](NOTES.md), devam noktası [SESSION_PROGRESS.md](SESSION_PROGRESS.md) içindedir.
