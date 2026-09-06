# Katkı rehberi

Power Above All, arkadaşların da katılabildiği ortak bir projedir. Belgeler, görev açıklamaları ve pull request metinleri için Türkçe kullanıyoruz. Oyun adı ve mevcut kod tanımlayıcıları korunur; yalnızca dil değişsin diye çalışan API'leri yeniden adlandırmayın.

## Erişim ve kurulum

Depo herkese açıktır (Public); klonlamak için özel erişim daveti gerekmez. Doğrudan yazma yetkisi olmayan katkıcılar fork üzerinden pull request gönderebilir. Bu rehber erişim izinlerini kendiliğinden değiştirmez.

1. Git ve Node.js 20 veya üzerini hazırlayın; CI ile aynı ortam için Node.js 24 kullanılabilir.
2. README'deki komutlarla depoyu klonlayın ve `npm start` çalıştırın.
3. `http://127.0.0.1:1789` adresini açın.
4. İlk değişiklikten önce `npm test` ile mevcut durumun çalıştığını doğrulayın.

Uygulamada paket bağımlılığı yoktur. Tarayıcı otomasyonu için kullanılan Playwright CLI ayrı bir geliştirme aracıdır.

## Görev ve dal düzeni

Ekip için önerilen akış:

1. Bir Issue açın veya mevcut görevi üstlendiğinizi belirtin. Beklenen sonucu ve etkilenecek modülleri yazın.
2. Kaydedilmemiş değişiklik yokken `main` dalını güncelleyin.
3. Tek amaçlı bir dal açın: `feature/birlik-secimi`, `fix/kayit-yukleme`, `docs/turkce-rehber` gibi.
4. Değişikliği yapın, ilgili kontrolleri çalıştırın ve komit oluşturun.
5. Dalı GitHub'a gönderip `main` hedefli bir pull request açın. Hazır olmayan çalışmalar için taslak PR kullanın.
6. İnceleme geri bildirimleri ve kontroller tamamlanınca değişikliği birleştirin.

```sh
git switch main
git pull --ff-only
git switch -c feature/birlik-secimi
# Değişiklikleri yaptıktan sonra:
npm test
git add app.js
git commit -m "feat: birlik secimi ekle"
git push -u origin feature/birlik-secimi
```

Yalnızca değiştirdiğiniz ve gözden geçirdiğiniz dosyaları ekleyin. Başka biri aynı modülde çalışıyorsa API ve veri yapısı değişikliklerini önceden konuşun. Bu belge önerilen çalışma düzenini açıklar; dal korumasını veya zorunlu onay kurallarını etkinleştirmez.

## Modül sınırları

| Modül | Sorumluluk |
| --- | --- |
| `simulation.js` | Sefer kuralları. DOM'a bağlı olmamalı; başarısız emir durumu kısmen değiştirmemeli. |
| `app.js` | Kullanıcı eylemleri, harita ve çekirdek bağlantısı. Ekonomi kurallarını burada kopyalamayın. |
| `battle.js` | Sabit zaman adımlı savaş. Sonuç tek kez uygulanmalı; durdurulan savaş animasyon döngüsü bırakmamalı. |
| `index.html`, `styles.css` | Görünüm, klavye erişimi ve ekran boyutlarına uyum. |
| `server.cjs`, `launch.cjs` | Yerel başlatma ve sunucu. |

Kayıt biçimini değiştiren işler mevcut kayıtların nasıl etkileneceğini açıklamalıdır. Sürüm ve geçiş yaklaşımını PR'a yazın.

## Kontroller

- Simülasyon değiştiyse `npm test` çalıştırın. Yeni test gerçek bir kuralı veya hata durumunu doğrulasın.
- JavaScript değiştiyse ilgili dosyada `node --check dosya.js` çalıştırın.
- Arayüz değiştiyse masaüstünde ve dar ekranda deneyin; PR'a ekran görüntüsü ekleyin.
- Kayıt veya savaş bağlantısı değiştiyse kaydet/yükle ve savaş sonrası kampanyaya dönüşü deneyin.
- Yalnızca belge değiştiyse bağlantıları, komutları ve anlatımın kodla uyumunu kontrol edin.

### Tarayıcı kontrolü

Yerel sunucuyu bir terminalde çalıştırın. İkinci terminalde, depo kökünde:

```sh
npx --yes --package @playwright/cli playwright-cli -s=power-above-all-test open http://127.0.0.1:1789
npx --yes --package @playwright/cli playwright-cli -s=power-above-all-test run-code --filename=tests/browser-smoke.js
npx --yes --package @playwright/cli playwright-cli -s=power-above-all-test close
```

İlk çalıştırmada CLI paketini indirmek için internet gerekir. Araç uyumlu bir tarayıcı bulamazsa verdiği kurulum yönergesini izleyin. Senaryo **kullandığı oturumda yeni sefer başlatır ve kayıtları değiştirir**; kişisel oyun oturumunuzda çalıştırmayın.

Ekran görüntüleri çalışma klasörüne göre `output/playwright/` içine yazılır ve Git'e dahil edilmez. GitHub Actions çekirdek testlerini ve sözdizimini denetler; tarayıcı senaryosunu çalıştırmaz.

## Tasarım ve tarih

Yeni mekaniklerde [REFERENCES.md](REFERENCES.md) içindeki hangi ilkenin kullanıldığını ve diğer sistemlerle bağlantısını açıklayın. Fransa ve 1789 odağını koruyun. Tarihî iddialar için kaynak verin; kurgu olaylarını ve basitleştirilmiş değerleri gerçek tarih gibi sunmayın.

Başka oyunlardan kopyalanmış kod, görsel, müzik veya metin yerine özgün içerik ve kullanım hakkı uygun kaynaklar kullanın.

## PR ve belgeler

Sorunu, değişiklikten sonraki davranışı ve nasıl doğrulandığını Türkçe açıklayın. Görsel işlerde önce/sonra görüntüsü; kayıt değişikliklerinde uyumluluk bilgisi ekleyin. İlgisiz düzenlemeleri aynı PR'a dahil etmeyin.

Tamamlanan özellik için gerekiyorsa `CHANGELOG.md`, `STATUS.md` ve README'yi güncelleyin. [ROADMAP.md](ROADMAP.md) içindeki onay bekleyen seçenekleri kesinleşmiş görevler gibi ele almayın.
