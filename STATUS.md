# Power Above All — proje durumu

Son güncelleme: **6 Eylül 2026, 06:10 UTC**. Kullanıcı yaklaşık dokuz saatlik geliştirmeden sonra döndü; son doğrulanmış Windows oyunu isteği üzerine açıldı. On saatlik çalışma aralığı 07:22:03 UTC / 10:22 İstanbul'a kadar sürer; sonraki kullanıcı yönü önceliklidir.

Unity 6000.3.23f1 üzerinde **oynanabilir, genişletilmiş bir prototip** vardır. Üç başlangıç rolü, kişisel siyasi bedeller, bölge yönetimi, ekonomi, ordunun hazırlanması ve taktik savaş aynı sefer durumuna bağlıdır. Büyük dünya, tam diplomasi ve rejim yolları henüz tamamlanmış değildir. Kesin kullanıcı tercihleri [VISION.md](VISION.md), bütünsel çalışma önerisi [GAME_VISION_DRAFT.md](GAME_VISION_DRAFT.md), güncel geliştirme raporu [NIGHT_REPORT.md](NIGHT_REPORT.md) içindedir.

## Oyunu açmak

[PLAY_GAME.cmd](PLAY_GAME.cmd) en yeni uygun Windows oyuncusunu seçer. Doğrulanmış adayın çalışma zamanı ve bütün dağıtım dosyaları denetlenir; `node play-game.cjs --check` seçimi oyunu açmadan gösterir. 06:08 kontrolünde seçilen aday `regional-reform-final-20260906-060546-199-b6af33b0` ve bütünlük sonucu `complete-build` idi. Oyunda **«Новая» / «Yeni»** başlangıç rollerini açar.

Unity projesi `Unity/`, başlangıç sahnesi `Assets/Scenes/Main.unity`; [OPEN_UNITY.cmd](OPEN_UNITY.cmd) editörü açar. Windows oyuncusu DX11 kullanır. Daha eski DX12 çıkış hatasının bütünüyle çözüldüğü iddia edilmez. Tarayıcı 0.1 ayrı bir referans prototiptir; [START.cmd](START.cmd) ile açılır ve Unity kaydıyla ortak değildir.

## Uygulanan oynanabilir kapsam

| Alan | Mevcut davranış |
| --- | --- |
| Roller ve siyasi güven | Üç atama, farklı yardım ve iki haftalık borç; ödememe, sonraki yardımın reddi ve kişisel iktidar harcayarak güveni onarma. |
| Bölgesel anlaşma | Dört vergi hesabı boyunca yerel muafiyet; hemen sakinleşme ve denetim; tamamlanma veya erken vergiyle ihlal siyasi sonuç doğurur. |
| Ordu mevcudu | Hedef ve sefer/bütçe politikası; iki başarılı haftada en fazla 200 yaşayan fazla asker rezerve geçer. Sıfır ordu ve normal yeniden kurma desteklenir. |
| Subay atama hakkı | Dumas'ya hak verme, normal alımdan sonra haftalık ücretli ek 200 asker, yaşayan orduya göre bedelli geri alma; bütçe politikasıyla çatışma görünür. |
| Dumas'nın girişimi | Gerçek açlık sonrası ileri tarihli erzak toplama girişimi; önceki normal ikmal, veto ve gerçek yerel/siyasi bedeller ortak haftalık hesapta çözülür. |
| Ekonomi ve halk desteği | Gelir, maaş, üretim, sivil/asker tüketimi ve sonraki stoklar; mevcut halk desteği ve 40/60 eşiklerinin bölgesel etkileri gösterilir. |
| Bölgesel proje | Tek bölgede iaşe veya ticaret; 120 livre ve 4 iktidar peşin; dört uygun haftadan sonra vergi/erzak tabanı değişir. Hazırlık huzursuzluk veya düşük denetimde bekler. Tamamlama ve iptal farklı hamilere ilişki sonucu taşır. |
| Bölgesel direniş | Düşman kuvveti özgün yerel taban, huzursuzluk, denetim ve elit muhalefetinden hesaplanır. Oyuncu ordusunun büyümesi düşmanı otomatik büyütmez. |
| Taktik savaş ve dönüş | Alay emirleri, moral, mevzi ve ikmal; eşzamanlı atış, cephanesiz yakın temas, geri çekilme ve doğal zafer. Gerçek kayıplar/teçhizat/siyasi sonuçlar aynı sefere döner. |
| Zafer kararı | Gerçek zafer sonrası ödül veya tanıma seçimi; yaşayan mevcuda göre bedel, general ve bölge sonuçları; bekleyen karar kayıt/yüklemede korunur. |

## Görünüm ve kullanım

Kabul edilen çalışma dili güneşli guaj atlas ve tarihî minyatürlerdir: sıcak kâğıt, adaçayı, yumuşak mavi, mercan ve koyu orman mürekkebi. Deniz derinliği, 12 yerleşimin farklı siluetleri, seçili sınır, savaş arazisi ve bahçe taçları gerçek karelerde incelendi. Varlık kökenleri [ART_ASSETS.md](ART_ASSETS.md), görsel kurallar [ART_DIRECTION.md](ART_DIRECTION.md) içindedir.

Rusça ve Türkçe oyun metni vardır. Bölge raporunun bütün gövdesi tek kaydırılan yapraktır; yoğun Paris uyarılarında emirlere ve nedenlere ulaşılır. Ekonomi ve siyasi belgeler fiyatı, ilgili kişiyi, mevcut sonucu ve koşullu gelecek hesabını gösterir. Ortak 1440×900 tasarım farklı pencere oranlarında eşit ölçeklenir. Sesli geri bildirimler ve kalıcı sessize alma vardır; son ses kalitesi dinleme kabulü hâlâ açıktır.

## Son doğrulama

[Son tam kapı](output/verify/regional-reform-final-20260906-060546-199-b6af33b0/REPORT.md): **GREEN**, 496/496 Unity EditMode testi, yeni Windows build, 18 PNG, 72 oyuncu kontrolü, 14 kampanya JSON'u ve 10 tarayıcı referans testi. Dağıtım 141 dosyalık manifestle doğrulandı; dış süreç çıkışı 0 idi. Ayrıca 692 RU/TR anahtarın 25 Unity varlığında metin/biçim/import yapısı ve 93 araç güvenlik kontrolü geçti.

Reformun beş gerçek bütçesi bağımsız denetimde önceki durumdan yeniden hesaplandı; pending, active, iptal ve yeniden ödeme kayıt çiftleri eşit çıktı. İlk dört bütçede eski ekonomi, beşincide yeni taban kullanıldı. İptal eski stokları geri almadı. RU/TR üst ve alt belge kareleri ayrıca gözle kabul edildi. Bu sonuçlar bütün olası kampanyaların dengeli olduğunu göstermez.

Önceki kesitlerde normal Windows fare/klavye girdisi, doğal savaş zaferi, tam savaş dönüşü ve arşiv doğrulandı. **Yeni reformun ayrı native fare senaryosu ve hazırlanan iki bağlı player rotası henüz çalıştırılmadı.** Şampanya'nın engellenmiş hazırlıktan çıkışı ve Dumas ile birleşimi şu an 227 kontrollü saf Core probe içinde doğrulanmıştır; bunlar yeni player kanıtı diye sunulmaz.

## Açık sınırlar ve sonraki iş

- Geniş dünya haritası, diplomasi, makam/kariyer sistemi, tam rejim yolları, kapsamlı olay kataloğu ve çok oyunculu oyun gelecekteki kapsamdır. Parti sonu ve zaman ölçeğine ilişkin açık vizyon soruları kullanıcı kararı sayılmaz.
- Mevcut 1789 Fransa haritası, sayısal ekonomi ve dört danışman oyun kurgusudur; birebir tarihî rekonstrüksiyon değildir.
- Sefer arşivi v8 eski v1–v7 verilerini destekler; devam eden taktik savaş ayrı bir kaydedilebilir savaş arşivi değildir.
- Güçlü aktif sivil politika ve pasif kriz yolları ölçüldü; uzun dönem rekabet dengesi, bütün taktikler ve ses kalitesi için daha fazla gerçek oynama gerekir.
- Kullanıcı açık oyunu incelerken otomatik fare girdisi veya oyunu kapatma yapılmaz. Sonraki reform fare/bağlı rota kontrolü uygun ayrı test oturumunda yapılabilir.
- Gece değişiklikleri yerel kontrol noktalarıyla korunur; GitHub'a gönderildiği iddia edilmez. Ayrıntılı kalıcı bulgular [NOTES.md](NOTES.md) ve `Unity/WorkNotes/` altında, başarısız eski denemeler değişmeden `output/` altında kalır.
