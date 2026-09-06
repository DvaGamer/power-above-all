# Subay yetkisi: gerçek Windows fare girdisi

6 Eylül 2026. Koşu: `output/verify/native-input-20260906-034838-5148eccc`. Root oyuncuyu ve gerçek fare girdilerini yönetti; bu ajan tamamlanmış JSON, makbuz, log ve manifest dosyalarını salt okunur inceledi. Hiçbir test, Unity, yeni derleme veya oyuncu başlatılmadı; eski kanıt dosyaları değiştirilmedi.

## Gerçek girdiler ve kanıt sınırı

Root bütün helper çağrılarının exit0 olduğunu ve gerçek giriş PNG'lerini gördüğünü bildirdi. İstanbul saatleri: 06:48:58 giriş (115,518), 06:49:00 wheel−10, 06:49:14 yetkiyi ver (1275,694); 01 checkpoint sonrasında 06:50:18 ek grubu al (1270,766), 06:50:32 wheel−10; 02 checkpoint sonrasında 06:50:55 yetkiyi geri al (1275,692).

İzole script başlangıçta normal `act recruit` yapar. Sonraki üç 55 saniyelik pencerede grant/recruit/revoke semantic komutu yoktur; üç subay kararı gerçek fare girdisidir. Save/load, son budget1400 emri ve iki hafta script'in public komutlarıdır; bunlar ayrıca gerçek fare kanıtı olarak sunulmaz.

## Tamamlanmış sonuç

- `REPORT.md` ve `result.json`: PARTIAL, mevcut derleme tekrar kullanılmış; yeni kaynak, EditMode ve browser bu koşuda incelenmemiş. Player/Frames PASS; 66 komut, 40 assertion, 5 PNG, 6 state JSON; toplam 176.31 saniye. Beş kare de1440×900 ve otomatik `problems` dizileri boş.
- Player PID11388, başlangıç `2026-09-06T03:48:39.0271341Z`, native exit0, timeout=false, tamamlanış `2026-09-06T03:51:30.8218681Z`. Owner PID11172, başlangıç `03:48:38.5794276Z`; sınırlar player240/owner300 saniye. Denetim sırasında iki PID de artık yoktu. Owner log da Native exit0 / PARTIAL diyor.
- Yeniden kullanılan kaynak build: `officer-commission-import-fixed-20260906-034710-694-4b17a317`. Manifest v1:141 kayıt /141 gerçek dosya, bütün uzunluk ve SHA256 değerleri yeniden okundu; eksik veya farklı dosya0. Runtime SHA256 `0F2CACDA5F0E4270128F883460EEBF8BB0B0A36CF59FE5A60EA3D7053F9B2581`. Player, runtime, izole script ve owner kaynak parmak izleri owned receipt ile aynıydı.

## Üç kararın gerçek state farkı

| Checkpoint | Gold / Food / Supplies | Troops / Manpower | Yetki / bu hafta kullanıldı | Dumas Loyalty / Relationship |
| --- | --- | --- | --- | --- |
|00-before-right |720 /340 /105 |1400 /2200 |false /false |60 /50 |
|01-native-granted |720 /340 /105 |1400 /2200 |true /false |60 /50 |
|02-native-extra |600 /320 /90 |1600 /2000 |true /true |61 /50 |
|03-native-revoked |466 /320 /90 |1600 /2000 |false /true |61 /50 |

00→01 yalnız `DumasOfficerCommission` ve tek `log.commission.granted` kaydı değişti. Ücretsiz grant gizli sadakat veya asker vermedi.

01→02 gerçek200 asker için120 Gold,20 Food,15 Supplies,200 Manpower harcandı; Morale76→74. Nested farklar yalnız Île Unrest50→52, army Approval62→64 ve Dumas Loyalty60→61 idi. Tek `log.commission.recruited(region.ile,200,1)` eklendi; global used=true oldu. Başka bölge veya karakter değişmedi.

02→03 yalnız Gold600→466, aktif bayrak true→false ve tek `log.commission.revoked(134)` kaydı değişti. Fiyat ceil(1600/12)=134; eski1400 kişilik117 fiyat uygulanmadı. Asker ve kazanılmış sadakat korundu, kullanılmış haftalık hak sıfırlanmadı. Üç geçişte de eski journal kuyruğu aynen korundu. Week0, Power55, Moves2, Fatigue0 ve seçili/ordu bölgesi Île değişmedi; checkpoint farklarında beklenmeyen başka kampanya eylemi yok.

`03-native-revoked.json` ve `04-native-revoked-loaded.json` ham baytları aynı: SHA256 `BBFA5AAC773DAC83928BCE959B6611F49B7E119400F394D5BC87ADD5D451C592`. İzole save/load, inactive+used=true durumunu korudu.

## Sonraki bütçe ve görsel gözlem

Budget1400 due2 kabulünden sonra ilk hafta used=false assertion geçti ve1600 kişi kaldı. İkinci gerçek hafta, eski1600 kişinin170 Gold giderini yine hesapladı; ardından200 kişiyi rezerve aktardı. İki `log.week` sırasıyla [1,206,170,−12] ve [2,203,170,−14]. Tek `log.establishment.reduced(200,1400,4,1400)` Week2 kaydı vardır.

Son checkpoint Week2: Gold535 / Food294 / Supplies98; Troops1400 / Manpower2200; budget1400, due0, PendingPetition=true; active=false / used=false. Loyalty61 kaldı; azaltım Relationship50→46 yaptı. Yeni asker bütçesi153 Gold /47 Food assertion geçti. Dumas toplama bildirimi ve mandate yoktu; toplam asker+rezerv3600 korundu.

Bu ajan `03-native-revoked.png` dosyasını ayrıca açtı:1440×900 gerçek atlas, Rusça subay metinleri, Gold466 / Food320 / Supplies90 / Troops1600 ve Loyalty61 görünür; footer134 ödemeyi ve asker/sadakatin korunduğunu açıklıyor. Bu karede ham `ui.commission.*` anahtarı görülmedi. Root diğer gerçek input karelerini de gördü; bu ayrı native inceleme kendi başına genel GREEN veya tüm taktik/politik denge kabulü değildir.
