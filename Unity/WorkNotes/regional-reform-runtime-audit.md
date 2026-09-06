# Reform B: ilk gerçek runtime ekonomisi

`output/verify/regional-reform-first-20260906-060159-696-0035bd7a` sonucu GREEN: 496/496 Unity testi, yeni build, 18 PNG / 72 assertion / 14 campaign JSON, 18 frame ve 10 browser kontrolü. Protokol 184 komut, success true, failures boş, bitiş `2026-09-06T06:02:28.0949668Z`; toplam gate44 saniye. Runtime SHA256 `CB2419D89A960B4A30940748CC9CD6A266B13072CF74EA77C36493F90A70958A`.

Bu bağımsız audit mevcut JSON/journal ve kaynak formülü üzerinden hesap yaptı; Core probe, compiler, Unity veya oyuncu çalıştırılmadı. İncelenen ilk iki gerçek PNG'de Normandy'nin başlangıç değeri U30/C80'dir. C74 bu bölgenin başlangıcı değildir; aynı karede Picardy'nin C74 olduğu state'ten de okunur.

## Bütçelerin ayrı karşılaştırılması

Her satırın önceki campaign JSON'undan vergi ve üretim yeniden hesaplandı. Gerçek kaynakta kullanılan float işlemleri `Math.fround` ile aynı sırada, ulusal toplamlar double ve son tam sayı AwayFromZero olarak ele alındı. Bu salt kanıt aritmetiğidir; yeniden oyun çalıştırması değildir. Hafta3 öncesindeki snapshot henüz gerçek petition açıkken alınmıştı; yalnız arada script'in yaptığı `petition negotiate` etkileri (Assembly+12 ve Île Unrest−10; kaynakta ayrıca Crown−8) bu satırın önceki koşuluna uygulandı. Varsayılan bir sonraki hafta ya da kapatılmamış petition ilerlemiş gibi kabul edilmedi.

| Gerçek hesap | Vergi | Üretim | Ordu gideri | Net Gold / Food | Önceki stoklardan beklenen ve gerçek Gold / Food |
| --- | --- | --- | --- | --- | --- |
|1, eski tabanlar |207 |152 |136 |+71 /+2 |791 /362 |
|2, eski tabanlar |204 |150 |136 |+68 /0 |859 /362 |
|3, negotiate sonrası eski tabanlar |217 |149 |136 |+81 /−1 |940 /361 |
|4, eski tabanlar; sonra completion |214 |147 |136 |+78 /−3 |1018 /358 |
|5, ilk gerçek reform tabanları |205 |149 |136 |+69 /−1 |1087 /357 |

Beş gerçek `log.week` dizisi bu Tax/ArmyCost/NetFood değerleriyle tam uyumlu. Hafta4 completion Normandy provisioning / Morel+4 kaydıdır; hafta1–3 kalan adımlar3,2,1; hafta4 sonunda0 ve Morel50→54. Aynı hafta4 öncesi ekonomiyi reform etkinmiş gibi hesaplamak yanlış olur: gerçek dördüncü ödeme hâlâ214 vergidir.

Hafta5 öncesindeki **aynı** state için reform kapalı karşılaştırma Tax211 / Production145 / NetGold75 / NetFood−5; gerçek etkin karşılaştırma Tax205 / Production149 / NetGold69 / NetFood−1. Böylece bu ilk etkin bütçenin koşullu katkısı −6 vergi ve +4 üretim/gıda dengesi. Bu fark, ilk günün draft'ındaki207→202 /152→156 ile karıştırılmadı: haftalar ve negotiate ülkenin vergi koşullarını gerçekten değiştirmiştir. Bu koşuda Dumas, accord, subsidy veya savaş yok; bu mekanizmaların testteki birlikteliği burada oynanmış gibi sunulmaz.

## İptal, yeni ödeme ve bütün state'in korunması

`09-first-reformed-budget`→`10-ended` arasında değişen üst alanlar yalnız Characters, Journal, ReformRegionId ve ReformModeId. Gerçek sponsor Morel54→46; Gold1087, Food357, supplies160, Troops1200, Manpower2400, Power53.5 ve Week5 aynen kaldı. Yeni journal `log.reform.ended [region.normandy,reform.mode.provisioning,character.morel.name,-8,reform.status.active]`; eski journal kuyruğu birebir aynı.

`11-ended-loaded`→`12-new-commerce-project` değişimleri yalnız Gold, Power, Journal ve üç reform alanı. Gerçek yeni bedel Gold1087→967 / Power53.5→49.5; Food357, ordu/rezerv/supplies ve ilişkiler değişmedi. Proje Brittany commerce /4 adım; Morel46 korunur, Valcourt50 henüz ödül almaz. Yeni log120/4/4 başlangıcını kaydeder ve eski journal kuyruğu korunur. Bu açık iptalden sonraki yeni ödeme, eski proje taşınmış gibi sunulamaz.

Ham SHA256 eşitlikleri:

- Pending01=02: `84B7F763480C31C13577EBAA13E34954D026306D49D979B79C2F28D537D78A34`.
- Petition04=05: `D8F633379A52BF8CAC139AC6B1B97ED25558153342B1E78831E050A2301C6903`.
- Active07=08: `2A594B2989757016D924F77DE91B95A4CE4C2F7673BEE4EAD52A4DF13E3B27A0`.
- Ended10=11: `7BB38FD5EC0B136A2868FA234D716EF6818F1C2CC38DEBBDBAEE5801272F6485`.
- Yeni commerce12=13: `5A4A54CF5DF517EF25358AB31DACCB1D1C97A997776F4EFF92F0CE4AEC49448A`.

Log'daki11 `PASS same` iki draft dil çiftini, asıl bölge seçimini, reddedilen petition haftasını ve save/load/dil eşitliklerini içerir. Tek tek14 campaign snapshot incelendi; frame checker'ın ayrı `frames.json` dosyası campaign sayısına dahil edilmedi. Otomatik GREEN ile bütün18 karenin görsel kabulü ayrıdır; bu ajan yalnız başlangıç ve provisioning altını ayrıca açtı.
