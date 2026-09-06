# Gercek zaferden siyasi tercihe oyuncu kaniti

2026-09-06. `tools/victory-campaign.script` prim dalini; ayri `tools/victory-recognize.script` komutanin inisiyatifini tanima dalini hazirlar. Henuz Unity, test, derleme veya player calistirilmadi. Root kaynak freeze sonrasi gate'i yonetir. Iki script de `new` ile baslar, tek son `quit` ile biter ve kendi izole kampanyasini kullanir.

## Neden iki gercek savas

`GameApp.ResolveVictory` basariliysa `Report` hemen ana isolated save'i degistirir. Bu nedenle `save -> bonus -> load` eski teklifi geri getirmez; o yolu iki dal kaniti diye sunmak yanlis olur. `.bak` kopyalama veya state geri yazma API'si eklenmedi. Ilk run tek gercek zafer ve bonus icin yeterlidir; ikinci dal gerekiyorsa ayni daha once kazanmis oyuncu emirleriyle ayri dogal zafer calistirilir. Her sureci ayri 300s player butcesi sinirlar; iki savas tek uzun surece yigilmaz.

Iki piyade merkezi tutar, suvari yandan ilerler, topcu normal shared aimed volley sonrasi serbest atese gecer; sonra birlikler konvoy hedefini zorlar. Mevcut tactical-campaign'in emirler arasindaki battle state/PNG checkpoint'leri de korunur; screenshot/frame yield zamanlamasini sessizce degistiren kisaltma yapilmaz. `battle wait ended 120` dogal sonuca kadar bekler; `BattleWon True` kaybedilen veya bitmeyen savasi kabul etmez. Can, cephane, moral, saat, hedef sayaci veya outcome enjeksiyonu yoktur. Onceki 196 kayip bir tarihsel run olcusudur, yeni run icin sabit beklenti degildir.

## Kanit sirasi

1. `06-natural-outcome.json` gercek raporu; Accept sonrasindaki `07-base-victory.json` bir kez uygulanmis temel zaferi kaydeder. `battle verify-return`, kayip ve yuruyus bedelinin kampanyaya tasinmasini kontrol eder. Yeni pending teklif yalniz bu noktada beklenir.
2. Teklif kaydedilir, pencere normal komutla kapatilir, harita secimi degistirilir ve gercek Load yapilir. `same pending-victory`, pending ID dahil butun kampanya JSON'unun korundugunu dogrular. Kapatmak decline sayilmaz.
3. `08/09` PNG'leri ayni teklifi RU/TR gosterir. Ardindan Normandy secilir; `09-before-*` JSON ve terms PNG'sinde odulun hala gercek Champagne zaferine bagli oldugu incelenir.
4. Yalniz bir basarili secim uygulanir. `10-after-*` JSON sonucu, RU/TR PNG'leri gercek mesajlari kaydeder. Yeni pending false, resolved count1 kalir.
5. Once explicit Save eklemeden harita secimi degistirilip Load yapilir; `same` basarili secimin otomatik kaydini dogrular. Sonra explicit Save/Load ve full equality ayrica kontrol edilir. Eski pending teklif yeniden acilmamalidir.

## Gercek maliyetin okunmasi

Root 09/10 JSON farkini ayni run icinde okur. Bonus: Gold farki tam `ceil(before.Troops/12)`, Dumas Loyalty farki `min(5,100-before.Loyalty)`, Champagne Control farki `min(3,100-before.Control)` olmali. Troops, Food, MilitarySupplies, Morale, Fatigue, Power, diger bolgeler, mandate/accord kimlikleri ve vadeleri degismemeli; Normandy secimi aynen kalmali. Ornegin gercek sag kalan1004 ise84 altin; fixture bu kaybi varsaymaz.

Recognize: Gold0 fark; Power bedeli secim ONCESI Dumas Ambition>Loyalty ise4, degilse0; Fatigue `-min(12,before.Fatigue)`; Relationship `+min(4,100-before.Relationship)`; Ambition `+min(3,100-before.Ambition)`. Loyalty, bolge Control, Competence ve kalan kampanya kaynaklari degismemeli. Her iki dalda PendingVictoryId bosalir ve tek yeni ilgili journal kaydi eklenir. Bu karsilastirma yeni command case'ini tekrar eden unit test degildir; gercek build'in mali transferini dogrular.

Yeni parser ilgili secim/close/panel ve boolean pending sozdizimini dogrular. Mevcut pure test dosyasina iki gercek fixture'in artifact/assert makbuzunu kapsama kontrolu, disaridan sahte prim bedeli argumanini reddetme ve belirsiz boolean beklentisini erken reddetme eklendi. Testler bu ajan tarafindan calistirilmadi; root'a freeze ile teslim edilir. Domain esik, yetersiz kaynak, decline ve tekrar odul testleri diger ajanin Core diliminde kalir; burada state uydurularak yeniden kurulmaz.

Kaynak envanteri: her iki fixture 103 komut,13 PNG,12 JSON ve25 assertion iceriyor; artifact isimleri benzersiz. `HasPendingVictory False` baslangic gozlemi haric, ilk komuttan Accept dahil taktik dizi mevcut `tactical-campaign.script` ile birebir ayni. Bu ajan yalniz dosya metnini okudu ve dizileri karsilastirdi. Root daha sonra47/47 safety checks ve her iki fixture parser'inin PASS oldugunu bildirdi; diagnostic full gate'in175/175 EditMode sonucu bu yeni siyasi fixture'lerin runtime kaniti degildir. Kaynaklar freeze teslim edildi.

## Normal Windows girdisi icin salt okunur hazirlik

1440x900 canvas merkezleri kaynak rect'lerinden: recognize(508,562), bonus(919,562), ordinary decline(582,712), gecici close(992,712), RU(1013,162), TR(1081,162). Mevcut native helper DPI ve letterbox donusumunu uygular; bunlar fiziksel masaustu koordinati degildir. Esc `VictoryDecision` icinde acikca ele alinir, olayi tuketir ve yalniz pencereyi saklar. Arkadaki GUI kontrolleri devre disi, GameApp map update'i BlocksMapInput ile engelli; kaynakta acik bir click-through yolu bulunmadi.

Yeniden acmak icin once Council sekmesi(1189,169), sonra belge basindaki acma dugmesi kullanilir. Dugmenin canvas x merkezi1279'dur; y, baslik/giris/entry metinlerinin gercek font yuksekligi ve scroll'a baglidir. Root yeni gercek kareden y'yi okumali; tahmini koordinat sabit UI kaniti sayilmaz. Enter/ok tuslari bu popup'ta acik secim kisayolu degildir; test edilmis gibi gosterilmemeli. Press'in klavye odagi icin ayri cizimi yok; tam klavye erisilebilirligi bu dar mouse/Esc provasinin kapsami degildir.

Mevcut native owner ilk kareyi `00-start.png` ister ve player180s/owner240s sinirlarini makbuzda dogrular. Bu iki semantic victory fixture `01` ile baslar ve zafer sonrasi insan/native girdisi beklemez; dogrudan native helper'a verilmemelidir. Yaklasik125s dogal savas sonra kalan yaklasik50s icinde Esc/reopen/close/reopen/tek karar dizisi onceden planlanmali veya root dar ve acik bir yeni timeout sozlesmesini ayrica uygulamalidir. Iki mali dal ayni pending tekliften gercek native tiklamalarla ardisik alinamaz; ikinci dal ikinci gercek zafer ister. Bu incelemede helper veya Assets degistirilmedi, girdi/surec baslatilmadi.

## Gercek bonus dali: 2026-09-06 01:12:14 UTC

Root run `output/verify/victory-bonus-first-20260906-011214-319-4c893010`: PARTIAL,145s,native0,103 komut/25 assert/13 PNG/12 JSON,13 otomatik kare kontrolu.141 reused build dosyasi ad/boyut/SHA olarak degismedi. Runtime `9C1B41362FEA8FE6DC308786AFAF75FA1049B05A8E063E40295B8B18FF7C58AD`, kaynak build `smoke-diagnostic-20260906-011014-153-05cbab94`; bu build'in175 EditMode testinin gecmesi onceki gate kanitidir. Bu oyuncu incelemesinde yeni build/EditMode/browser atlandi. Ayni build'deki reddedilmis beyaz duman sunumu askeri popup kabulunden ayri kalir.

Yeni gercek outcome: Won=true,125.8030777 sim saniyesi,196 kayip,+24 konvoy malzemesi.1004 sag kalanin gercek primi ceil(1004/12)=84; `09-before-bonus` ile `10-after-bonus` arasinda Gold840 -> 756. Pending `battle-0-2-ile-champagne` -> bos, resolved listesi ayni tek ID. Normandy secimi iki JSON'da da normandy, ordu Champagne'da kalir. Dumas Loyalty60 -> 65, Champagne Control70.5 -> 73.5; Normandy Control80 ayni.

Butun top-level ve nested regiment disi kampanya alanlari karsilastirildi: yalniz Gold, PendingVictoryId, Regions/champagne/Control, Characters/dumas/Loyalty ve bir yeni `log.victory.bonus` journal kaydi degisti. Journal eski kuyrugu tamamen korundu. Power59, Fatigue35, Food342, MilitarySupplies139, Competence78, Ambition83 ve Relationship52 ayni. Eski soz/anlasma verilerine yeni sure veya bolge aktarilmadi.

Ham JSON tam esitlikleri:07-base ==08-loaded-pending;10-after-bonus ==11-loaded-autosave;10-after-bonus ==12-loaded-explicit-save. Son isolated archive Version4. Basarili prim otomatik kaydi da explicit kaydi da eski teklifi geri getirmiyor.

Bu ajan 08 RU,09 TR,10 original-region popup ve11 RU sonuc tam PNG'lerini acti. Popup84 bedeli, temel+3 sonrasi hirs83, iki secenegin etkileri, gecici kapatma ve ordinary decline ayrimini okunur gosteriyor; metin kesilmesi gorulmedi. Normandy arka planda seciliyken popup Champagne'a bagli kaldi. Gercek karede yeniden acma dugmesinin merkezi(1279,344), Council(1189,169); bu koordinat bilgisi native tiklama yapildigi iddiasi degildir.

11 sonuc PNG'si kasanin normal kisa sayac animasyonunun ortasinda829 gosteriyor; ayni andaki authoritative JSON756, metindeki maliyet84 dogru. Bu mali aktarim hatasi degildir. Sonraki fixture revizyonunda secimden sonra yaklasik0.6s bekleyip son sayiyi gostermek faydali olur; tamamlanmis ciktiya veya frozen script'e bu incelemede dokunulmadi. Recognize dalinin sonucu bu bonus run'indan turetilmez.

Sonraki root istegiyle iki fixture'de yalniz `10-after-*` state ile `lang ru` arasina `wait 0.6` eklendi. Simulasyon/outcome veya karar bedeli degismedi; mali sonuc JSON'u beklemeden once kalir, ardindaki PNG sayac gecisini bitirmeye zaman verir. Yeni script envanteri104 komut/13 PNG/12 JSON/25 assert. Onceki103 komutluk run ciktilari oldugu gibi korunur. Bu dar fixture revizyonu freeze teslim edildi; tekrar calistirma root tarafindadir.

## Gercek recognize dali:2026-09-06 01:22:52 UTC

Root run `output/verify/victory-recognition-20260906-012252-141-2481db02`: PARTIAL,145s,native0,104 komut/25 assert/13 PNG/12 JSON,13 otomatik kare kontrolu;141 reused build dosyasi degismedi. Kaynak build `powder-alpha-20260906-011819-635-7d706a18`, runtime SHA256 `59916824F63F1A535C8EBB8B093F72775069442979CE3C6B7ED3722801A41CFC`. Yeni kaynak derlemesi/EditMode/browser bu player-only run'da atlandi. Protokol tamamlanma01:25:05.4227004 UTC. Bu ajan yalniz sonucu/JSON'lari ve iki PNG'yi okudu.

Ayri gercek dogal sonuc yine Won=true,125.8030777 sim saniyesi,196 kayip,+24 konvoy malzemesi,1004 sag kalan oldu. Bu yeni kayit kendi outcome'undan okunmustur; eski bonus sonucundan turetilmedi.09-before-recognition ->10-after-recognition: Power59 ->55, Fatigue35 ->23, Dumas Relationship52 ->56, Ambition83 ->86. Gold840 ->840; Food342 ve MilitarySupplies139 ayni. Temel zaferin Ambition83>Loyalty60 durumu once hesaplanmis4 guc bedeliyle uyumlu.

Butun top-level ve bolge/karakter alanlari karsilastirildi: sadece yukaridaki dort deger, PendingVictoryId ve tek yeni `log.victory.recognize` journal kaydi degisti; eski journal kuyrugu tamamen korundu. Loyalty/Competence, bolge Control, diger kaynaklar ve mandate/accord verileri degismedi. Pending ID bosaldi, ayni tek resolved battle kaldi. Ordu Champagne, harita secimi Normandy olarak korundu.

Ham07-base ==08-loaded-pending; ham10-after-recognition ==11-loaded-autosave ==12-loaded-explicit-save. Otomatik ve explicit kayit eski teklifi tekrar acmadi.11 RU ve12 TR sonuc tam PNG'leri acildi: eklenen0.6s sonrasi Gold840 vePower55 tamamlanmis degerleri gosteriyor; mali/siyasi mesajlar iki dilde okunuyor. Bu semantic branch kabuludur, gercek native mouse/keyboard kaniti degildir.

## Ayni process'te ikinci dunya: birlesik tam gate

`military-art-final-20260906-012710-424-48b0deff` tam GREEN:176 Unity/yeni build/171 komut/38 assert/21 PNG/21 JSON/10 browser,203s; runtime `FC1E21937ACE6213B4F62FD20CD2E7727FE465ADA5BAA3E853F713CE61A4CFF6`. Onceki duman muharebesi retreat ile bitip yeni kampanya acildiktan sonra **01-before-encounter'dan12-loaded-explicit-save'e kadar12 raw JSON'un tamami** ayri `victory-bonus-first` verileriyle byte-esit. Yeni baslangic/deployment/order/outcome, pending, asil bolgeye prim ve iki kayit yuklemesi icin ek durum farki yok. Yeni outcome yine gercek Won=true/125.8030777s/196 kayip; eski bozulan dunyanin kayip veya resolved kaydi yeni kampanyaya tasinmamis.

07/08 pending roundtrip ve10/11/12 committed roundtrip ayrica dogrulandi. Second-world02 PNG'sinde eski birlik/duman kalintisi gorulmedi;11-bonus-result-ru artik tam756 altin gosteriyor, yeni0.6s cekim beklemesi onceki ara829 goruntusunu gidermis. Duman/kare pause esitligi icin `volley-review-plan.md` bu gate bolumune bakilir. Bu ajan yalniz tamamlanmis kanitlari okudu, kaynak/test/player/girdi degistirmedi.
