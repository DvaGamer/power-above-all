# Power Above All — Proje durumu

Son güncelleme: **5 Eylül 2026**. İlk tarayıcı prototipi 0.1 hazır, doğrulandı ve özel GitHub deposuna yüklendi.

## Tamamlanan işler

- [x] Oyun adı Power Above All, ortam Fransa, 1789 olarak belirlendi.
- [x] 12 şematik bölge ve iki harita katmanı oluşturuldu.
- [x] Ekonomi, ordu hareketi, emirler, siyasi destek, olaylar ve kayıt sistemi uygulandı.
- [x] Tüfekli birliklerle taktik çatışma ve sonuçların sefer haritasına aktarılması eklendi.
- [x] Masaüstü ve dar ekranlara uyum sağlayan arayüz tamamlandı.
- [x] START.cmd ile yerel sunucu ve tarayıcı başlatma doğrulandı.
- [x] [Özel GitHub deposu](https://github.com/DvaGamer/power-above-all) oluşturuldu.
- [x] İlk uygulama komiti `7e4723d`, main dalına yüklendi.
- [x] Dört oyun referansı kalıcı tasarım rehberi olarak kaydedildi.

## Doğrulama kaydı

- [x] 200 haftalık tekrar edilebilir seferler dahil 10 çekirdek testi geçti.
- [x] Tarayıcıda harita, emirler, elle/otomatik kayıt, ekonomi, siyaset ve konsey olayı doğrulandı.
- [x] Geri çekilme ve tamamlanan savaşın kayıpları doğru şekilde uygulandı.
- [x] 1440×960 ve 390×844 ekranlar denendi; yatay taşma görülmedi.
- [x] İlk sürümün [GitHub Actions denetimi başarılı](https://github.com/DvaGamer/power-above-all/actions/runs/33987434163).

Ekran görüntüleri test bilgisayarındaki `output/playwright/` klasöründedir; Git'e dahil edilmez.

## Ekip hazırlığı

- [x] Arkadaşların da geliştirmeye katılacağı kararı kaydedildi.
- [x] Türkçe belgeler, katkı rehberi, Issue/PR şablonları ve yerel belge bağlantıları kontrol edildi.
- [x] Beş görsel referans ART_DIRECTION.md içine işlendi; bağlantılı v0.2 planı ROADMAP.md içinde taslak olarak kaydedildi.
- [ ] Türkçe belgelerin GitHub'a yüklenmesi.

## Bir sonraki aşama — seçim bekliyor

Proje sahibi önce planı görüşüp yol seçmek istiyor. Aşağıdaki işler henüz geliştirme onayı almadı; ayrıntılar [ROADMAP.md](ROADMAP.md) dosyasında.

- [x] Öncelik: siyaset/karakter, bölge/harita, alay savaşı ve ikmali birleştiren v0.2.
- [x] Oyuncunun hedefi: kişisel siyasi hayatta kalma ve devlet üzerinde kontrol.
- [ ] Kesin kimlik: kurgusal konsey üyesi, XVI. Louis veya Jacques Necker.
- [ ] Teknoloji: bir sınırlı tarayıcı aşaması daha veya hemen Unity.
- [ ] Seçilen yol için somut görevler ve tamamlanma ölçütleri.

## Bilinen sınırlamalar

- Öğretici senaryo kolaydır: konsey olayında stok dağıtmak savaşmadan zafer getirebilir.
- Savaş arazisi dekoratiftir; tek asker türü ve ortak grup kontrolü vardır.
- Harita tarihî sınırları birebir yansıtmaz.
- Kayıtlar tarayıcıya ve kullanılan adrese bağlıdır. Tamamlanmamış savaş kaydedilmez.
- Oyun arayüzü Rusçadır; belgelerin Türkçeleştirilmesi arayüzü değiştirmez.
- Bu bir tarayıcı prototipidir. Tam hanedan, diplomasi, inşaat, 3B ordu ve çok oyunculu sistemler henüz yoktur.

## İlerleme nasıl izlenir?

1. Yalnızca doğrulanan işleri bu dosyada tamamlandı olarak işaretleyin.
2. Oynanabilir sürümü START.cmd veya npm start ile açın.
3. [Issues](https://github.com/DvaGamer/power-above-all/issues), [pull request'ler](https://github.com/DvaGamer/power-above-all/pulls) ve Git geçmişinden değişiklikleri takip edin.
4. [Actions](https://github.com/DvaGamer/power-above-all/actions) sekmesinde otomatik kontrol sonuçlarını inceleyin.
