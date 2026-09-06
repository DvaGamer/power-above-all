# Gece geliştirme raporu — çalışma sürüyor

Güncelleme: 6 Eylül 2026, 03:10 UTC. On saatlik görev **07:22:03 UTC / 10:22 İstanbul** saatine kadar aktiftir. Bu bir ara rapordur; uzun vadeli vizyonun tamamlandığı anlamına gelmez.

## Çalışan sürüm

[PLAY_GAME.cmd](PLAY_GAME.cmd) son doğrulanmış Windows oyununu açar. `node play-game.cjs --check` oyunu başlatmadan seçilen yolu ve dosya bütünlüğünü gösterir.

Son tam kapı: [Ordu mevcudu raporu](output/verify/army-establishment-final-20260906-030602-688-f7c2fdcb/REPORT.md), **GREEN**. 304 Unity testi, yeni Direct3D11 build, 37 gerçek PNG, 259 durum kontrolü, 33 JSON kaydı ve 10 tarayıcı testi geçti. Başlatıcı bu 141 dosyalık build'i `complete-build` olarak seçti. Runtime SHA256: `1a31f88bc9ad3dbb72e39227e73766c624fe7bb7ec0daf2319191bef21ac1f3c`.

## Oynanabilir değişiklikler

- Kampanya öncesi üç çalışma rolü seçilir. Her rol farklı bir hamiden yardım alır ve iki hafta içinde karşılık vermeyi taahhüt eder. Sözün ilk bölgesi, vadesi ve bedeli korunur. Tekrarlanan temerrüt kişinin güvenini tüketir; yeni yardım için siyasi telafi gerekir.
- Morel aracılığıyla bir bölgeye dört haftalık vergi tatili verilebilir. Direnç azalırken gerçek vergi gelirinden vazgeçilir. Olağanüstü vergi bu anlaşmayı bozabilir; kişisel iktidar, ilişki ve temsilcilerin desteği etkilenir. Eski rol sözüyle birlikte çalışır.
- Gerçek zaferden sonra iki isteğe bağlı karar görünür: Dumas'ya toparlanma yetkisi verip yorgunluğu azaltmak veya kalan askerlere hükümet adına prim ödemek. İlki generalin hırsını da artırır ve sadakat düşükse kişisel güç harcar. İkincisi gerçek hazineyle sadakat ve yerel denetim kurar. Olağan sonuçla devam etmek mümkündür.
- Zafer belgesi iki seçeneği, bedelleri ve sonuçları yan yana gösterir. Esc yalnız pencereyi kapatır; teklif konseyden yeniden açılır. Ayrı ret düğmesi teklifi kapatır. Yeni hafta ek bir cevap kilidi getirmez.
- Dumas gerçek gıda açlığından sonra, bir sonraki hesap için kendi erzak toplama emrini ilan eder. Oyuncu cevap vermese de emrini değerlendirir. Yerel etkilerden sonraki açık en fazla40 ise tüm ihtiyacı kapatır; yetersiz toplama için ek zarar üretmez. Oyuncu ilişki bedeliyle yasaklayabilir veya normal ikmali düzeltebilir. Toplama kampı izler; yerel huzursuzluk, elit bağlılığı ve generalin hırsı etkilenir.
- Konseyde ve alt bildirimde erzak emri okunabilir. Tarih, yer, gerçek bedeller ve veto açıkça görünür. Ekonomi defterindeki toplama satırı, haftanın uyguladığı aynı hesabı kullanır. Yeni bir zorunlu cevap kilidi yoktur.
- Sürekli ordunun üst sınırı belirlenebilir. İki başarılı haftada en çok200 yaşayan fazla asker rezerve döner; ayrılış hesabı eski mevcudu öder, daha düşük gider sonraki hesapta başlar. Dumas her gerçek gruba tepki verir. Politika durdurulabilir; yeniden asker almak normal fiyatını ister. Hedef0 mümkündür; flama ve askerî harita alanı kaybolur, sonraki hafta garnizon katkısı da biter.
- Hesaplar içindeki açık kâğıt belge, uygulanan hedefi ve taslağı ayırır. Ücret, gıda, ayrılış tarihi ve Dumas'nın gerçek bedeli imzadan önce okunur. Gerçek fareyle800 taslağı seçilip kapatıldığında kampanya aynı kaldı; ayrı onay tıklaması emri bir kez uyguladı. Kayıt/yükleme ve iki haftalık çıkış doğrulandı.
- Arşiv v6 ordu hedefini ve ayrılış tarihini saklar. v1–v5 kayıtları taşınır; eski rol sözleri, v3 bölgesel anlaşmaları, v4 zafer kararları ve v5 Dumas emirleri korunur.

## Görsel dil ve savaş

Krem kâğıt, adaçayı yeşili, açık mavi, mercan ve koyu mürekkep birlikte kullanılır. Haritada on iki ayrı şehir silueti, guaj arazi ve kurgu kişilerin resimli portreleri bulunur. Savaş alanındaki tepe ve sığ geçit gerçek arazi kurallarıyla uyumludur.

Duman için ayrı ve açık alfa geçişi kullanıldı. Önceki opak beyaz dörtgen hatası gerçek karelerde giderildi; son duman hafif bir izdir, erken evresi hâlâ zayıf görünür. Bu, bitmiş ve güçlü bir salvo gösterisi olarak sunulmaz. Dumas'nın yanındaki komşu portre kırıntısı yalnız gösterilen kaynak bölgesi daraltılarak kaldırıldı; resim dosyası ve ana siluetin ölçeği değiştirilmedi.

Aynı taktik adımda verilen atışlar birlikte çözülür. Cephanesiz piyade ve süvari yakın temasa girebilir. Gerçek oyuncu rotası 125,803 saniyede zafer, 196 kayıp ve 24 ele geçirilen teçhizat verdi; bu her oyuncunun sonucu için garanti değildir. Önceki military-art-final koşusunda iki savaş dünyası art arda kurulup kapandı ve eski nesneler yeni karşılaşmaya taşınmadı.

Alay etiketleri figürlerin ve sabit namlu alanının yanına taşınır; ince bağlantı çizgisi hangi birliğe ait olduklarını gösterir. Seçili topçu ve iki piyade gerçek RU/TR karelerinde açıktır. Üç duraklatılmış PNG/JSON çifti birebir eşittir. Yoğun sahnenin bir bölümünde iki panelin köşesi yaklaşık6×3 piksel temas eder; metin kapanmaz. Tamamen çakışmasız yerleşim iddiası yoktur.

## Kanıtlanan siyasi sonuçlar

Ordu mevcudu rotasında1200→1000 için ilk iki hesap136 livre/40 gıda ile tamamlandı; sonra200 kişi rezerve döndü. Sıfır hedefi12 haftada3600 rezerv bıraktı,13. hafta asker olmadan ilerledi; normal200 kişilik işe alımla tekrar ordu kuruldu. Bağlı askerî rol rotasında14. haftanın200 kişilik çıkışı, aynı tarihteki80 livre borcu silmedi. Sonraki açlık kayıpları nedeniyle16. hafta rezerve yalnız76 kişi döndü; kişi sayısı oluşturulmadı. Bunlar son tam kapıdaki gerçek komut sonuçlarıdır.

Sekiz gerçek haftalık hazırlıkta ilk açlık Dumas'nın dokuzuncu hafta emrini doğurdu.36 gıda toplandığında1840 asker kaldı; veto edildiğinde eski açlık sonucu148 asker kaybedildi ve1692 kaldı. Toplama hırsı3 artırdı, kişisel gücü4 azalttı; normal haftalık etkiler ayrıca uygulandı. Açık emir, toplama sonucu ve veto kayıtları yüklemelerde birebir korundu; toplama günlüğü ilan edilmiş dokuzuncu haftaya yazıldı.

6283 kontrollü saf Core incelemesi gerçek komutlarla normal ikmalin de emri iptal edebildiğini gösterdi. Daha küçük orduyla on ikinci haftada Paris yardımını kapatmak20 gıda açığını giderir; bu politikanın kendi güç ve Paris huzursuzluğu bedeli kalır. Askerî rolün40 gıda yardımı da yeterli olabilir, fakat on dördüncü haftaya80 livre borç bırakır. Bu iki dal [gerçek oyuncu incelemesinde](output/verify/dumas-intervene-20260906-022412-198-6555cda1/REPORT.md) de geçti: native exit0,81 kontrol,10 PNG ve10 JSON. Root iptal açıklamasını ve80 livre borcunu RU/TR karelerinde gördü; yükleme borcu korudu. Build yeniden kullanıldığı için bu ek sonuç PARTIAL'dır.

1004 sağ kalan asker için prim **84 livre** oldu: hazine840→756, Dumas sadakati60→65, Champagne denetimi70,5→73,5. Başka bir bölge seçmek ödülün yerini değiştirmedi. Önceki açık teklif ile yüklenen kayıt, ayrıca ödenmiş sonuç ile iki ayrı yükleme birebir eşitti.

Ayrı gerçek zaferde yetki verme **güç59→55, yorgunluk35→23, ilişki52→56, hırs83→86** üretti; hazine840 kaldı. Bu kayıt da aynı biçimde korundu. Yetki verildikten sonraki haftanın zorlu yürüyüşünde yorgunluk31 olur; olağan sonuçta43 olur. Bu karşılaştırma formül hesabıdır, ikinci bir savaş sonucu iddiası değildir.

Gerçek Windows girdisi [native-victory kaydında](Unity/WorkNotes/native-victory-input.md) incelendi: Esc, konseyden yeniden açma, TR/RU seçimi ve prim düğmesine fare tıklaması. Sahipli süreç native exit0 ile ve süre aşımı olmadan kapandı. Yeniden kullanılan build nedeniyle sonuç PARTIAL'dır. İlk Start çağrısı açıklamasız exit1 döndürdü; aynı sürecin sonraki Inspect ve altı gerçek girdi çağrısı başarılıydı. Başlangıç hatasının nedeni kanıtlanmadı; sonraki hatalar için açık tanı çıktısı eklendi.

## Kalan sınırlar

Kesin rol kadrosu, kampanya sonu, görevden düşme sonrası devam ve geniş alternatif tarih yolları hâlâ [VISION.md](VISION.md) içindeki açık kararlardır. Dumas'nın tek girişimi tam bir bağımsız siyasi simülasyon değildir. Mevcut sayılar tarihsel istatistik olarak sunulmaz.

Gerçek Shift ile iki piyade seçimi ve elle topçu salvosu doğrulandı: tek tıklama11→10 mühimmat, ardından Space ile13,689 saniyelik yeniden doldurma durdu; duraklatılmış JSON/PNG aynı kaldı. Yeni Dumas belgesinde gerçek alt bildirim tıklaması, mouse wheel ve veto da görüldü; sahipli oyuncu exit0 ile süre aşmadan kapandı. Sesin dinlenerek kalite kabulü tamamlanmış sayılmaz. Dumanın erken okunurluğu hâlâ geliştirme alanıdır.

Kullanıcının kişisel kayıt dosyası korunmuştur; son okunan SHA256 `18f3c57d89161fc471bc0aa997c8266c01d50ba1def670cd476081f1f5f7b63e`. Testler ayrı kayıt dizinleri kullandı. Eski güvensiz `tools/night.ps1` ve `tools/night-prompt.txt` çalıştırılmadı. Ayrıntılı tarihçe [NOTES.md](NOTES.md), devam noktası [SESSION_PROGRESS.md](SESSION_PROGRESS.md) içindedir.
