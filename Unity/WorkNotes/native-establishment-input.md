# Hedef mevcut: gerçek taslak ve ayrı imza girdisi

6 Eylül 2026. `native-input-20260906-030000-dd2720cd` koşusu salt okunur incelendi. Player/Unity/derleyici/test veya input aracı bu ajan tarafından başlatılmadı; Assets, araçlar ve tamamlanmış artefaktlar değiştirilmedi.

## Sahiplik ve gerçek sonuç

Korunan derleme `army-establishment-first-20260906-025643-395-97035b66`, runtime SHA256 `2bcfccd13a0cd562d068723e91515d4bd2ef0b70b4298a98534df9daf7829c57`. EXE SHA256 `82efd5a8c297a5046f6bb72c240dab8972e64003851935eba0a46ad121ce0305`.

Owner13380 başlangıcı03:00:00.1931617 UTC; player15664 başlangıcı03:00:00.6294016 UTC. Native çıkış0, timeoutfalse, bitiş03:01:57.6165717 UTC. İnceleme anındaki dar salt okunur süreç sorgusu her iki kayıtlı PID'nin de bulunmadığını doğruladı.180 saniyelik player/240 saniyelik owner sınırı korunmuş.

Sonuç **PARTIAL**,121.48 saniye,43 komut/19 assertion/5 PNG/4 JSON; yeni derleme, Unity ve browser atlanmış. Native receipt EXE/runtime/script/sahip kimliğini kaydeder. Buna ek olarak bu ajan, koşudan sonra özgün full-gate manifestindeki bütün141 dosyayı diskten yeniden SHA256/boyut/yol üzerinden okudu: gerçek dosya sayısı141, eksik/farklı dosya0. Bu ek salt okunur kontrol eski receipt'e yazılmadı ve PARTIAL elle GREEN'e çevrilmedi.

## Gerçek ve semantik eylemlerin sınırı

Root Start'ın exit0 döndüğünü, aşağıdaki bütün input çağrılarının exit0 olduğunu ve karşılık gelen Temp PNG'lerini tek tek gördüğünü bildirdi. Zamanlar İstanbul; koordinatlar1440×900 client üzerindedir.

| Zaman | Gerçek eylem | Amaç |
| --- | --- | --- |
| 06:00:32 |1280,300 sol tık | Hesaplar içindeki ordu belgesine giriş |
| 06:00:33 |1308,416 sol tık |800 hazır hedefini yalnız taslakta seçmek |
| 06:00:35 |1275,216 sol tık | Belgeden Hesaplar'a dönmek |
| 06:01:16 |1280,615 üzerinde tekerlek−8 | Şartları ve imza düğmesini okumak |
| 06:01:31 |1280,685 sol tık |800 hedefini ayrı bir gerçek emir olarak imzalamak |

Script ilk45 saniyelik pencereden sonra tam `same untouched-draft` kontrolünü yapar. **Belgeyi tekrar açan** `panel establishment` semantik sunum komutudur; ikinci bir native giriş tıklaması yapılmış gibi sunulmaz. İkinci65 saniyelik pencerede gerçek imza beklenir. Script hiçbir `establishment budget` veya `establishment campaign` komutu çağırmaz. Sonraki save/load ve iki haftalık hesap doğrulaması semantiktir.

Bu ajan ayrıca gerçek `02-reopened-draft.png` ve `03-native-order.png` dosyalarını açtı. İlkinde800 sayısı “yeni hedef · taslak”, ikincide aynı800 “yürürlükteki hedef” olarak görünür. Güncel1200 birlik/136 gider/40 gıda ile sonraki1000 birlik/120 gider/34 gıda ayrıdır; mevcut koşullara bağlı tahmin ve tasarrufun sonraki hesapta başlaması okunur.

## Tam durum incelemesi

Phase1 `same untouched-draft` gerçek runtime protokolünde geçti; başlangıç snapshot'ı ayrı JSON dosyası olarak yazılmadığı için iki dosyanın raw eşitliği diye sunulmaz. `01-draft-uncommitted.json`: Week0/campaign/target0/due0,Gold840/Food360/Supplies120/Troops1200/Manpower2400, Dumas ilişkisi50, Île seçimi/kampı ve Moves2.

01→03 bütün alanlar üzerinden karşılaştırıldı. Değişenler yalnız ArmyPolicyId campaign→budget, ArmyTargetTroops0→800, ArmyReductionDueWeek0→2 ve tek `log.establishment.budget_scheduled` kaydıdır (Week0, args `[800,2]`). Önceki günlük kuyruğu bütünüyle aynı; stok, asker, kişi, bölge ve hareketlerde ek tıklama/ücretsiz aktarım etkisi yoktur. `03-native-order.json` ve `04-loaded-order.json` SHA256 olarak birebir eşittir.

`05-first-batch.json`: Week2/Troops1000/Manpower2600,Gold979/Food362/Supplies136, Dumas50→46, hedef800, sonraki due4 ve PendingPetitiontrue. Tek reduced kaydı Week2 `[200,1000,4,800]`; haftalık günlükler `[1,207,136,2]` ve `[2,204,136,0]`. Her iki hesabın eski ordu gideri136 korunmuş; yeni dilekçe earned200 transferini engellememiş.400 kişinin hepsi tek haftada ayrılmamış.

## Kapsam dışındaki açık görsel sorun

Bu kanıt800 hedefine aittir. Ayrı sıfır hedef player koşusunda, Troops0 olmasına rağmen atlas ordu bayrağının görünmesi root tarafından bulundu; UI düzeltmesini root yapıyor. Bu native800 kabulü sıfır ordunun bütün görsellerini kabul etmiş sayılmaz. Başka DPI, klavye veya savaş sonucu bu koşuda sınanmadı.
