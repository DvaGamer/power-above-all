# Gece kuyruğu

On saatlik güncel görevde birbirine bağlı maddeler kontrol noktalarıyla sürdürülür. Bitince `[x]`, engellenirse `[!] ENGELLİ - sebep`.
Kanıtı `NIGHT_LOG.md` içine yaz. Kurallar: `NIGHT_BRIEF.md`.

Kuyruk biterse baştan başlanmaz. Bunun yerine `output/shots/contact-sheet.jpg` incelenir,
en zayıf ekran seçilir, yeni bir madde olarak sona yazılır ve o yapılır.

## Önce zemin

- [x] **00. Kapıyı gerçek projede yeşile getir.** `tools\verify.ps1` dosyasını `Unity/` üzerinde
  çalıştır. Kırmızıysa sebebini düzelt. Bu madde bitmeden başka hiçbir maddeye geçme.
  Kanıt: `output/verify/baseline-visible-20260905-214616-749-5ef5f22d/REPORT.md`: GREEN, 23/23 Unity testi, 20 kare, 26 durum kontrolü, 10 tarayıcı testi. Yeni kapı koşu başına ayrı klasör kullanır.
- [x] **01. İnceleme derlemesinden `Development Build` filigranını kaldır.** `BuildTools.BuildWindows`
  içinde `BuildOptions.Development` yerine varsayılan olarak filigransız derleme; hata ayıklama
  gerekirse ayrı bir menü komutu bırakılabilir. Kabul: yeni karelerde filigran yok.

## Savaş ekranı - en büyük kopukluk

- [x] **02. Savaş paletini sefer haritasının kimliğine bağla.** Parlak yeşil zemin ve düz mavi nehir
  yerine fildişi, soluk orman yeşili, toprak ve mat altın. Zemin tek düz renk olmasın; hafif doku
  ve ton kırılması olsun. Hesap ve zamanlama değişmez. Kabul: 12-15 numaralı karelerde palet sefer
  haritasıyla aynı dili konuşuyor.
- [x] **03. Alt komut şeridini oyunun kontrol diline getir.** Gri dikdörtgen düğme sırası yerine
  askerî emir kartı hissi: net durum, hover ve basılı hâli, kullanılamayan emirde sebep metni.
  Kabul: kare 12 ve 13.
- [ ] **04. Savaş alanına kompozisyon ver.** Yol, tepe, tarla sınırı, orman kenarı ve ufuk;
  nehir düz şerit olmaktan çıksın. Rastgele dekor yığını değil, okunur bir sahne. Görüş
  kirlenmesin. Kabul: kare 12 ve 14.
- [ ] **05. Yaylım sahnesi.** Silah kaldırma, kısa hazırlık, ölçülü parlamalar, ortak duman,
  karşı hatta tepki. Mevcut hasar zamanlamasıyla uyumlu kalsın. Kabul: kare 13 ile 14 arasında
  gözle görülür fark ve `shot-check.py` çıktısında `moved` değerinin artması.
- [ ] **06. Duman ve isabet.** Duman birkaç an kalsın, sürüklensin, kademeli dağılsın; sabit daire
  olmasın. İsabet küçük geri tepme veya düzende açılan yerle okunsun. Kabul: kare 13 ve 14.
- [x] **07. Savaş raporunu askerî bildiriye çevir.** Sıradan kart yerine kazanan, gerçek mevcut,
  kayıplar, kısa haber ve tek dönüş düğmesi. Kabul: kare 15.

## Sefer haritası ve panel

- [x] **08. Harita kiplerini gerçekten ayır.** Denetim, huzursuzluk, vergi ve ordu kipleri tek
  bakışta farklı okunsun; her kipin kendi lejantı ve vurgusu olsun. Kabul: kareler 05, 06, 07 belirgin
  biçimde farklı.
- [x] **09. Varsayılan kaydırma çubuğunu ve IMGUI kontrol hissini kaldır.** Sağ panel, sekmeler ve
  düğmeler tek bir kontrol dili izlesin. Kabul: kare 01 ve 09.
- [x] **10. Bölgelere görsel karakter.** Paris, liman ve kırsal aynı görünmesin: şehir işareti,
  peyzaj vurgusu veya kısa yerel tanım. Tarihî ayrıntı uydurulmaz. Kabul: `upkeep-help-20260906-000313-242-91cc7cbc/shots/01-atlas-ru.png` ve02TR,12 farklı şehir minyatürü; tarihî bina doğruluğu iddiası yok.
- [ ] **11. Seçili bölgenin hissi.** Hafif yükselme, kontur, yumuşak aydınlanma ve yan belgeyle açık
  bağ. Hover tıklamadan önce etkileşimi göstersin. Kabul: kare 03.
- [x] **12. Kullanılamayan her emir sebebini söylesin.** Soluk düğme yetmez: gıda yetersiz, emir
  kullanıldı, ordu burada değil, komşu değil, hareket hakkı bitti. Kabul: kare 03 ve 11.

## Hafta, ekonomi, günlük

- [ ] **13. Hafta sonu ritmi.** Para, gıda ve huzursuzluk değişimi okunur sırayla görünsün.
  Ek tıklama istemesin, simülasyonu tekrar çalıştırmasın. Kabul: kare 08.
- [ ] **14. Sayı standardı.** Aynı işaretler, yuvarlama, RU/TR biçimi ve renk anlamları her yerde
  aynı. Değişen sayı kısa süre yönüne göre vurgulansın. Kabul: kare 01, 04, 08.
- [ ] **15. Günlük hiyerarşisi.** Yeni kayıt yumuşak belirsin, önemli haber rutinden ayrılsın,
  okunan konum sebepsiz sıçramasın. Kabul: kare 04 ve 10.

## Metin ve dil

- [ ] **16. Rusça ve Türkçe taşma taraması.** Her ekran iki dilde kontrol edilir: taşma, eksik
  anahtar, metin ve harita çakışması. `tools\shots.script` içine gereken dil geçişlerini ekle.
  Kabul: iki dilde de kesik metin yok.
- [ ] **17. Debug dilini oyuncu yüzünden çıkar.** "Test savaşı", "senaryo", "olay" gibi teknik
  etiketler kalmasın; kişilerin sesleri birbirinden farklı olsun. Kabul: kare 09 ve 15.

## Oynanabilirlik

- [ ] **18. Seferde bölüm değerlendirmesi.** Kesin zafer/kayıp ve kampanya uzunluğu kullanıcı tarafından seçilmedi. Önce üç konsept değerlendir; toparlanabilen ordu0 durumunu zorunlu oyun sonuna çevirmeden, oyuncunun siyasi/ekonomik sonuçlarını okuyabildiği isteğe bağlı bir değerlendirme düşün. Henüz uygulama yok; eski zorunlu kapanış önerisi güncel vizyon kararı sayılmaz.
- [x] **19. Gerçek player'da uzun tur.** `tools\long-campaign.script` üzerinden en az 6 hafta ve iki
  savaş içeren uzun bir tur çalıştır. Çöken, tutarsız veya iki kez uygulanan sonuç var mı bak.
  Bulduğun her hatayı kuyruğun sonuna madde olarak yaz. Kanıt: `roles-six-week-20260905-231015-322-3f2f59ba`, native0/12PNG/40assert/4JSON. İki savaş scriptli geri çekilmeyle bitti; bu bütünüyle elle oynanmış veya iki doğal zaferli tur diye sunulmaz. Daha sonraki doğal zafer ve gerçek Windows girdi kanıtları NIGHT_LOG içinde ayrıdır.
- [ ] **20. Ses seviyeleri.** Mevcut on prosedürel sesin seviyesi ve tekrarı dinlenmeden kalite
  iddia edilmez. En azından sonucun iki kez çalmadığını ve eşzamanlı ses sınırını koda bakarak
  doğrula. Kabul: `NIGHT_LOG.md` içinde ne kontrol edildiği açıkça yazılı.

## Kapanış

- [ ] **21. `NIGHT_REPORT.md` son hâli.** Gerçekten değişenler, kanıtlar, engelliler, sabah
  bakılacak üç şey. Kareler `output/shots/` içinde bırakılır.
- [ ] **22. Belgeleri gerçeğe hizala.** `STATUS.md`, `POLISH_PLAN.md` ve `CHANGELOG.md` yalnız
  doğrulanmış işleri tamamlanmış gösterir. Push et.
