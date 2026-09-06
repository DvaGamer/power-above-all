# Dumas bildiriminde gerçek Windows girdisi

6 Eylül 2026. Salt okunur kabul; bu inceleme ajanı player, Unity, test, derleyici veya girdi aracı başlatmadı. Gerçek pencerenin sahibi ve bütün fare eylemlerinin uygulayıcısı root'tur. Assets/tools ve tamamlanmış kanıt klasörleri değiştirilmedi.

## Korunan koşu ve sınırlar

`output/verify/native-input-20260906-022151-e4eb7bdb` mevcut `dumas-labels-first-20260906-021758-659-0da55b25` GREEN derlemesini kullanır. Runtime SHA256 `72628a77ed862d41e2c9538e4637d32a0a7bd33c36cd3e537b71c1fb4f917fe3`, EXE SHA256 `82efd5a8c297a5046f6bb72c240dab8972e64003851935eba0a46ad121ce0305`. Sahibin PID'si18400, player PID20348; player başlangıcı02:21:52.1618378 UTC. Native çıkış0, timeoutfalse, bitiş02:23:29.1155608 UTC. Sınırlar player180/owner240 saniye.

Makbuz ve rapor **PARTIAL**,100.62 saniye,49 komut,14 assertion,4 PNG ve4 JSON gösterir. Yeniden derleme, Unity ve browser atlanmıştır; bu tek başına yeni bir GREEN değildir. `native-dumas.script` sekiz haftalık gerçek kampanyayı semantik emirlerle hazırlar;90 saniyelik girdi penceresinde `forage veto`, doğrudan state yazımı veya gizli karar komutu yoktur. Gerçek veto sonrası dokuzuncu hafta ve save/load tekrar semantik doğrulama ile yürütülür.

## Root'un gerçek eylemleri

Root ilk Start'ın exit0 olduğunu ve şu üç aracın da exit0 döndüğünü, her gerçek Temp karesini açıp gördüğünü bildirdi. Zamanlar İstanbul'dur; ekran1440×900 istemci koordinatlarıdır:

| Zaman | Gerçek girdi | Gözlenen işlev |
| --- | --- | --- |
| 05:22:08 | Alt bildirime sol tık800,875 | Dumas belgesi açılır |
| 05:22:20 |1270,620 üzerinde tekerlek−8 | Veto bedeline ve düğmesine kaydırılır |
| 05:22:35 |1280,734 sol tık | Açık veto uygulanır |

Bu ajan Temp karelerinin tamamını yeniden açmadı; root'un zamanlı gözlemi olarak kaydedildi. Otomatik `02-native-veto.png` bu ajan tarafından ayrıca açıldı: kapanmış emir açıklaması, tek veto haberi, aynı Île seçimi ve765 altın/1840 asker görülüyor.

## Durum karşılaştırması

`01-before-native` → `02-native-veto` bütün alanlar üzerinden karşılaştırıldı. Değişenler yalnız `DumasForageDueWeek`9→0, Dumas `Relationship`50→46 ve başa eklenen tek `log.dumas.vetoed` kaydıdır (`Week=8`, args `[region.ile,4]`). Günlüğün eski kuyruğu tamamen aynı. Gold765/Food0/Troops1840/Power53.5/Supply75/Morale76, seçili bölge ve kampÎle, Moves2 ile cooldown12 değişmemiştir. Bu pencere aralığında kampanyada ek tıklama, yürüyüş veya kaynak emri etkisi görülmez; UI odağı/hover durumuna ilişkin daha geniş bir iddia yapılmaz.

Raw SHA256 karşılaştırması ayrıca gerçek girdinin önceki semantik kabul ile tam eşitliğini doğruladı:

| Native JSON | İlk GREEN Dumas JSON | Sonuç |
| --- | --- | --- |
|01-before-native |01-allow-warning | Birebir aynı |
|02-native-veto |06-vetoed-before-week | Birebir aynı |
|03-native-veto-week |08-vetoed-week-nine | Birebir aynı |
|03-native-veto-week | Native04-loaded | Birebir aynı |

Dokuzuncu hafta Gold806/Troops1692/Power48.5/Supply50/Morale61; veto eski açlık sonucunu kaldırmaz. Cooldown12 kalır.14 assertion içinde son tam save/load eşitliği de vardır;15 kontrol diye raporlanmaz. Burada klavye, farklı DPI veya başka pencere altında tıklama kanıtı iddia edilmez.
