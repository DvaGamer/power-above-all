# Power Above All — Proje durumu

Son güncelleme: **5 Eylül 2026**. Kullanıcı geliştirmeyi durdurup gereksinimlerin plana kaydedilmesini istedi. Devam işleri [POLISH_PLAN.md](POLISH_PLAN.md) içinde korunur.

**Güncel durum:** Unity 6000.3.23f1 kuruldu, kullanıcı lisansı etkinleştirdi, Editor'de `Main.unity` açıldı ve Play modunda sefer haritası çizildi. Bu denemede TacticalBattle yerel nesne başlatma hatası görüldü; kaynakta düzeltildi. **Düzeltme sonrası yeniden derleme ve açılış henüz doğrulanmadı.** Tam oyun akışı, Rusça/Türkçe görsel inceleme, ses ve Windows oyun derlemesi tamamlanmış sayılmaz.

Geçerli aşama **0.2 Visual & Feel Polish Pass**: mevcut döngünün sunumu, okunurluğu ve oyun hissi. Yeni mekanik veya ekonomi genişletmesi yok. Unity oyununun dilleri **Rusça ve Türkçe**; belgeler Türkçedir.

## Tamamlanan tarayıcı 0.1 işleri

- [x] Oyun adı Power Above All, ortam Fransa, 1789 olarak belirlendi.
- [x] 12 şematik bölge ve iki harita katmanı oluşturuldu.
- [x] Ekonomi, ordu hareketi, emirler, siyasi destek, olaylar ve kayıt sistemi uygulandı.
- [x] Tüfekli birliklerle taktik çatışma ve sonuçların sefer haritasına aktarılması eklendi.
- [x] Masaüstü ve dar ekranlara uyum sağlayan arayüz tamamlandı.
- [x] START.cmd ile yerel sunucu ve tarayıcı başlatma doğrulandı.
- [x] [GitHub deposu](https://github.com/DvaGamer/power-above-all) oluşturuldu; kullanıcının açık onayıyla şimdi **Public** olarak kalıyor.
- [x] İlk uygulama komiti `7e4723d`, main dalına yüklendi.
- [x] Dört oyun referansı kalıcı tasarım rehberi olarak kaydedildi.

## Tarayıcı 0.1 doğrulama kaydı

- [x] Uzun ve tekrar edilebilir seferler dahil tarayıcı çekirdeği testleri geçti.
- [x] Tarayıcıda harita, emirler, elle/otomatik kayıt, ekonomi, siyaset ve konsey olayı doğrulandı.
- [x] Geri çekilme ve tamamlanan savaşın kayıpları doğru şekilde uygulandı.
- [x] 1440×960 ve 390×844 ekranlar denendi; yatay taşma görülmedi.
- [x] İlk sürümün [GitHub Actions denetimi başarılı](https://github.com/DvaGamer/power-above-all/actions/runs/33987434163).

Ekran görüntüleri test bilgisayarındaki `output/playwright/` klasöründedir; Git'e dahil edilmez.

## Ekip hazırlığı

- [x] Arkadaşların da geliştirmeye katılacağı kararı kaydedildi.
- [x] Türkçe belgeler, katkı rehberi, Issue/PR şablonları ve yerel belge bağlantıları kontrol edildi.
- [x] Beş görsel referans ART_DIRECTION.md içine işlendi; bağlantılı v0.2 planı ROADMAP.md içinde taslak olarak kaydedildi.
- [x] `cb200f1` çalışma anlık görüntüsü GitHub'a yüklendi.
- [ ] Son yerel değişikliklerin ikinci anlık görüntüsü; yükleme tamamlanmadan yapılmış sayılmaz.

## Unity kaydı — kısmen denenmiş kaynaklar

- [x] Motor seçimi: Unity.
- [x] Dil gereksinimi: yeni oyunda Rusça ve Türkçe.
- [x] Saf C# sefer çekirdeği Windows .NET derleyicisiyle derlendi.
- [x] Bağımsız C# kontrolleri: başlangıç, atomik kaynak işlemi, düşmanca yürüyüş, savaş sonucu, tekrar sonucu reddetme, ikmal etkileri, 200 hafta ve bozuk durum reddi.
- [x] 14 Unity/NUnit editör testi kaynak olarak eklendi.
- [x] Unity 6000.3.23f1 kuruldu: `C:/Users/USER/Tools/Unity/6000.3.23f1/Editor/Unity.exe`.
- [x] `OPEN_UNITY.cmd` başlatıcısı `Unity/` projesini hedefliyor; başlangıç sahnesi `Assets/Scenes/Main.unity`.
- [x] İlk lisans hatasından sonra kullanıcı lisansı etkinleştirdi; Editor yeniden açıldı.
- [x] `Main.unity` açıldı; Play modunda sefer haritası çizildi.
- [x] Bu denemede görülen TacticalBattle yerel nesne başlatma hatası kaynakta düzeltildi.
- [ ] Düzeltme sonrası yeniden derleme, yeniden açılış ve Play denemesi.
- [ ] Bu testlerin Unity Test Runner içinde çalıştırılması.
- [ ] Unity editöründe tam oyun akışının çalışma zamanı doğrulaması.
- [ ] Rusça/Türkçe arayüz ve kayıtlı günlüklerin dil değişiminde görsel doğrulanması.
- [ ] Oyunun Unity'den derlenip açılması.

## Bir sonraki aşama — 0.2 Visual & Feel Polish Pass

Kaynakta bulunan sunum: 12 bölgeli kabartma atlas ve bilgi katmanları; şehir minyatürleri ve ordu sancağı; belge panelleri, siyasi güçler ve hesap defteri; taktik savaş ve sefer bağlantısı; Rusça/Türkçe metinler. Yumuşak ordu hareketi, seçili bölgenin yükselmesi, emir sonrası bölge vurgusu, kaynak sayılarının geçişi, son günlük kaydı vurgusu ve devre dışı emir gerekçeleri eklendi. Bunların tamamı görsel olarak onaylanmış değildir.

Tarayıcıdaki mevcut ikinci hafta ekmek dilekçesi Unity'ye belge olarak aktarıldı. Kâğıt, kalem, mühür, emir, yürüyüş, hafta, salvo, isabet, zafer ve yenilgi için on prosedürel foley taslağı bulunur; tamamlanmış profesyonel ses varlıkları değildir.

- [ ] Mevcut haritanın ve belge panellerinin hiyerarşisini iyileştirmek.
- [ ] Mevcut savaşın seçimini, emir geri bildirimini ve okunurluğunu iyileştirmek.
- [ ] Rusça ve Türkçede taşma, eksik anahtar ve tutarsız terimleri gidermek.
- [ ] Unity çalışma zamanı ve görsel kontrollerini tamamlayıp kanıtlarını kaydetmek.

**Kapsam sınırı:** yeni mekanik ve ekonomi genişletmesi yok. Kullanıcının durdurma isteği doğrultusunda uygulama devam ettirilmiyor; kalan görsel gereksinimler ve kontroller planda saklanıyor.

## Bilinen sınırlamalar

- Tarayıcı 0.1 öğretici senaryosu kolaydır: konsey olayında stok dağıtmak savaşmadan zafer getirebilir.
- Tarayıcı 0.1 savaş arazisi dekoratiftir; tek asker türü ve ortak grup kontrolü vardır. Unity savaş kaynakları ayrı bir aktarım ve sunum çalışmasıdır.
- Harita tarihî sınırları birebir yansıtmaz.
- Tarayıcı 0.1 kayıtları kullanılan tarayıcıya ve adrese bağlıdır; Unity kayıtlarıyla ortak değildir. Tamamlanmamış savaş kaydedilmez.
- Tarayıcı 0.1 arayüzü Rusçadır. Yeni Unity oyununun Rusça/Türkçe desteği zorunludur; kaynakların bulunması tam arayüz doğrulaması sayılmaz.
- Baştan sona doğrulanmış referans tarayıcı 0.1'dir. Unity'nin ilk Play denemesi kısmi kanıttır; son düzeltmenin yeniden denenmesi ve tam akış kontrolü bekler. Tam hanedan, diplomasi, inşaat ve çok oyunculu sistemler henüz yoktur.

## İlerleme nasıl izlenir?

1. Yalnızca doğrulanan işleri bu dosyada tamamlandı olarak işaretleyin.
2. Tarayıcı referansını START.cmd veya npm start ile, Unity projesini OPEN_UNITY.cmd ile açın.
3. [Issues](https://github.com/DvaGamer/power-above-all/issues), [pull request'ler](https://github.com/DvaGamer/power-above-all/pulls) ve Git geçmişinden değişiklikleri takip edin.
4. [Actions](https://github.com/DvaGamer/power-above-all/actions) sekmesinde otomatik kontrol sonuçlarını inceleyin.
