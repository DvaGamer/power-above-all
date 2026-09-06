# Bölgesel düşman mevcudu: mevcut doğrulamaların bağımlılıkları

6 Eylül 2026. Yalnız kaynak incelemesi: `Unity/Assets/Tests/Editor`, `tools/*.script`, mevcut BattleSetup/Begin ve AutoShots dönüş denetimi. Yeni direnç kuralı henüz tanımlanmadı; bu not formül veya yeni beklenen savaş sonucu önermiyor. Test, derleme veya oyuncu çalıştırılmadı; Assets/tools değiştirilmedi.

## Doğrudan eski sayı bağımlılığı

Bugünkü oran `Unity/Assets/Scripts/Battle/TacticalBattle.cs:138`: düşman toplamı `Max(200, RoundToInt(originalTroops * .9f))`.1200 oyuncu için1080 buradan gelir. `GameApp.cs:354` yalnız oyuncu Troops ve koşullarını BattleSetup'a geçirir; bölgesel düşman mevcudu henüz bir giriş değildir.

İncelenen NUnit kaynaklarında veya review script'lerinde **1080 düşman** ya da **.9 oranı** doğrudan assert edilmiyor. Script'lerde `expect Troops 1004`, `expect ...Casualties 196` veya125.803 saniyelik bitiş assertion'ı da yok.196/1004 sayıları şu Core fixture'larında bulunur:

| Kaynak | Gerçekte neyi sabitler? | Bölgesel düşman kuralı tek başına değiştirmeli mi? |
| --- | --- | --- |
|`CampaignVictoryDecisionTests.cs:19`,61–72 |`Winner` doğrudan `ResolveBattle(..., true,196,68)` çağırır;1200−196=1004, dönüş ve replay/red guard'larını doğrular. |Hayır. Bu seçilmiş Core sonuç girdisidir, simüle edilmiş savaşın196 kaybettiği iddiası değildir. |
|Aynı dosya137–143,187,283,324 |1004 kişinin84 prim bedeli, alımdan sonra1204 kişinin101 bedeli;840−84=756 gibi işlem/arşiv sonuçları. |Hayır; fixture girdisi ve fiyat sözleşmesi aynı kaldıkça bu kesin muhasebe beklentileri korunur. |
|`OfficerCommissionTests.cs:180–184` |1600 kişiye doğrudan196 kayıp sonucu uygulanır;1404 sağ kalan için117 revoke fiyatı ve açık zafer/hak korunur. |Hayır. Metod adındaki “ActualBattleLosses” burada Core'a verilen kayıptır; bu bir doğal taktik proof değildir. |
|`BattlePresentationTests.cs:230` |Hiç savaş adımı çalışmadan geri çekilme ek kaybı `RoundToInt(1200*.035f)`. |Hayır; düşman boyutundan ayrı mevcut retreat sözleşmesidir. |
|`tools/officer-commission.script:158` |Gold196, iki ücretli revoke ve önceki alım/yürüyüşten kalan para. |Hayır; savaş kaybı196 ile sayısal tesadüftür, bu script savaş yapmaz. |

Bu fixture'ları yeni doğal kayıp sayısıyla otomatik değiştirmek, bağımsız transaction testlerini gereksiz yere taktik dengeye bağlar.

## Doğal sonuç isteyen oyun regresyonları

Gerçek davranış bağımlılığı, aynı emirlerin eski düşman karşısında kazanması ve zamanında hedefe varmasıdır. Sayısal kayıp assertion'ı olmaması bu rotaları yeni dengeye karşı bağımsız yapmaz.

| Kaynak | Sabit davranış beklentisi ve yeniden kabul noktası |
| --- | --- |
|`tactical-campaign.script:52–61`, `victory-campaign.script:51–60`, `victory-recognize.script:49–58` |1200 kişilik Champagne başlangıcı,45 saniye artillery arrival,30 saniye volley-ready, sonra120 saniyede doğal `BattleWon True`. Yeni bölgesel koşulla tekrar gözlenmeli. Başarısızlıkta sonucu enjekte etmek veya Won assertion'ını silmek yerine gerçek taktik/fixture amacı değerlendirilir. |
|`military-art-final.script:107–133` |Aynı doğal zafer bloğunun birleşik smoke+victory kopyası. Ayrı victory script'i değiştirilirse bu kopya da bilinçli olarak eşleştirilmeli; geçmiş combined artifacts aynen kalır. |
|`native-victory.script:48–57` |Aynı doğal kazanma rotası popup'ın60 saniyelik gerçek fare penceresinin önkoşuludur. Yeni karşılaşma süresi owner timeout240 planını etkileyebilir; pencereyi eski duvar saatine göre açmak yerine gerçek ready PNG beklenir. Yeni süre ancak gerçek koşudan sonra kabul edilir. |
|`officer-battle.script:69–94` |1600 kişilik Champagne yolu;75 saniye sonra ayrı Line `(4,3)` emri, arrival30 ve ended60 sınırları. Son kabul148.354446 saniye/244 kayıp/1356 sağ kalan gözlemidir, script'te sabit beklenen kayıp değildir. Yeni düşmanla ikinci manevranın da gerçekten işe yaradığı tekrar gösterilmeli. |

`tools/officer-battle.script` prim/revoke parasını sabitlemez; son gerçek113 bedeli1356 sağ kalandan okunan kanıttı. Önceki120 saniye timeout RED ve sonraki ayrı Line emriyle PARTIAL proof yeni bölgesel kuralın kanıtı olarak yeniden etiketlenmemeli.

## Çatışmaya dolaylı bağlı görsel/girdi fixture'ları

`volley-review.script`, `smoke-uncovered.script`, `labels-review.script`, `native-volley.script` ve `dumas-labels.script:168` sonrası aynı Champagne/topçu yerleşimine, arrival45 ve volley-ready30 durumlarına dayanır. Yeni düşman gücü hedefin hayatta kalması, routing ve sonuçsuz kalma süresini etkileyebilir. Gerçek salvo için Ammo−1, queued tüketim ve paused eşitlik ölçümleri korunur; önce gerçek hazır hedefe varan rota doğrulanır. Eski smoke süresi veya ammo baseline'ı yeni men sayısı olarak yorumlanmaz. `military-art-final.script` ilk bölümü de bu dolaylı bağımlılığı taşır.

`native-battle.script` sabit60/30 saniye pencerelerinde henüz bitmemiş savaş ve gerçek seçim/varış ister; yeni düşman altında bu pencere koşulları ayrıca gözlenir. `long-campaign.script` ve eski `shots.script` ise kasıtlı retreat kullanır,196 kayıplı zafer varsaymaz. Uzun kampanyadaki ilk Food342 beklenen yürüyüş bedelidir; düşman kuvvetinden türetilmez. İlerleyen public ekonomik eylemler farklı gerçek kayıplardan dolaylı etkilenirse yeni koşu incelemesi gerekir, önceden sayıları oynatmak gerekmez.

## Dinamik kalması gereken denetimler

`AutoShots.cs:360–376` içindeki `VerifyBattleReturn`, savaştan önceki campaign ve gerçek kabul edilen snapshot'ı kullanır: Troops=öncekiTroops−**gözlenen**Casualties; zafer/yenilgiye göre gerçek ordu bölgesi; gözlenen CampaignReturnMorale; march preview Food/Moves; gerçek recovered supplies; tek doğru battle ID ve geçmişin korunması. İçinde .9,1080,196 veya1004 yoktur. Bu kontrol yeni bölgesel model için sabit düşman/kayıp rakamına çevrilmemeli.

Mevcut12 farklı script'te13 `battle verify-return` çağrısı vardır; military-art-final iki kez çağırır. Gerçek pending/paid/revoked/budget save-load `same` kontrolleri de yeniden gözlenen duruma bağlıdır ve korunmalıdır. Prim/revoke audit'i her yeni koşuda canlı Troops'un gerçek fiyatına, ilk zafer bölgesine ve diğer alanların değişmezliğine bakmaya devam eder.

Üç doğrudan TacticalBattle fixture kurucusu gelecekte BattleSetup sözleşmesi değişirse açık düşman girdisi gerektirebilir: `BattleCommandTests.cs:28`, `BattlePresentationTests.cs:57`, `TacticalSimulationTests.cs:51`. Bugün yalnız Troops1200 gönderirler. Hazırlanmış simulation duelleri `Activate` ile iki tarafı açık400/400 yapar; bunlar .9 denge testi değildir. Presentation'daki8 alay ve slot/Id beklentileri kuvvet dağıtım yapısına bağlıdır, toplam1080'e değil. Yeni API gerekiyorsa fixture girişleri açıkça tanımlanır; snapshot isolation, aynı-tick saldırı, paused emir, retreat/accept-once assertion'ları zayıflatılmaz.
