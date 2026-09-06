# Subay beratıyla gerçek savaş rotası

6 Eylül2026. Root isteğiyle yalnız yeni `tools/officer-battle.script` yazıldı. Taktik emirleri daha önce kullanılan `tools/victory-campaign.script` ve `tools/tactical-campaign.script` kaynaklarından alındı. Bu agent hiçbir script, oyun, derleyici veya doğrulama aracı çalıştırmadı. İlk sonuç henüz bilinmiyor; `BattleWon True` başarılması gereken denetimdir, enjekte edilen sonuç değildir.

Yeni army kampanyasında normal alım → berat → ilave alım; gerçek başlangıç1600 asker/2000 Manpower/600 altın/320 yiyecek/90 teçhizat/61 sadakat, açık berat ve kullanılmış haftalık hak kontrol edilir. Champagne'a gerçek march, duraklatma, grup/hat emirleri, normal hareket, hazır topçu voleyi ve gerçek hedef baskısı izler. Rapordan önce hiçbir can, kayıp, düşman durumu veya saat değeri yazılmaz. Orijinal45 saniye hareket/30 saniye hazır olma/120 saniye sonuç bekleme limitleri korunur; rotanın1600 kişiyle gerçekten kazanacağı ancak root koşusunda belli olur.

`battle verify-return`, gözlenen taktik kaybı, seyahat maliyeti, gerçek teçhizat kazanımı, moral, hareket ve çözülmüş savaş kimliğini kampanyaya karşı doğrular. Sonraki JSON'lar canlı asker ve gerçek para fiyatının kanıtıdır. Başarılı rapordan sonra açık berat + PendingVictory gerçek save/load ile aynı kalmalı. Prim61→66 sadakat verir; ardından canlı kadroya bağlı fiyatla berat geri alınır ve normal1000 budget hedefi imzalanır. Haftalık zaman ilerletilmez; Used=true tüm yol boyunca korunmalıdır.

12 PNG ve15 JSON/snapshot hedefi vardır. İki dilde savaş raporu, yüklü açık zafer, ödenmiş prim, gerçek geri alma fiyatı, geri alınmış hak, budget belgesi ve sonuç jurnali çekilir. PNG sayısı ek bir başlangıç/harita resmiyle artırılmamalıdır.

## Root'un ilk gerçek çıktıda kontrol edeceği dinamik eşitlikler

1. `06-officer-natural-outcome.json` gerçek taktik snapshot'ıdır. `07-officer-victory-return.json` için `Troops=1600−gerçekCasualties`; diğer seyahat/geri dönüş eşitlikleri script içindeki verify-return tarafından denetlenir. Zaferde en az bir canlı asker beklenir; kesin kayıp sayısı önden sabitlenmez.
2. `07` ve `08` state JSON'ları tam aynı olmalıdır: açık commission, Used=true, PendingVictory ve sadakat61. Script bunu `remember/same` ile gerçek yüklemeden sonra da denetler.
3. `09-officer-before-bonus.json` → `10-officer-paid-bonus.json`: asker/Manpower aynı; altın farkı `ceil(canlıTroops/12)`; sadakat61→66; PendingVictory temiz; commission ve Used korunur. Orijinal zafer bölgesinin kontrol artışı mevcut gerçek clamp ile+3 olmalıdır. `11` gerçek otomatik kayıt yüklemesidir ve `10` ile tam aynı olmalıdır.
4. `12-officer-before-revocation.json` → `13-officer-rights-reclaimed.json`: aynı canlıTroops/Manpower/sadakat66; altın farkı yine `ceil(canlıTroops/12)`; açık hak false, Used true. Bu aralıkta yeni alım, kayıp veya hafta olmadığı için prim ve geri alma fiyatı aynı gerçek asker sayısından hesaplanır. Jurnalde `log.commission.revoked` gerçek tutarı içerir.
5. `14-officer-postbattle-budget.json` ve gerçek yüklemeden sonraki `15` tamamen aynı olmalıdır. Hedef1000; canlı asker1000'i aşıyorsa due2, aşmıyorsa due0. Her iki dal meşrudur; bilinmeyen savaş kaybını gizlice varsayan bir `expect ArmyReductionDueWeek 2` konmadı. İmza asker veya Manpower'ı değiştirmez. Aktif berat varken önceki budget girişiminin tüm state'i koruduğu ayrıca `same` ile kontrol edilir.

Bu kontroller gerçek çıktı olmadan “geçti” diye kaydedilmez. Core378 testlerinin geçtiği ve ilk UI build'in yalnız root'un localization meta GUID hatası nedeniyle tekrarlandığı root bildirimidir; bu script o eski build'i kullanmadı.

## İlk doğal koşunun gerçek RED incelemesi

Root koşusu `output/verify/officer-battle-natural-20260906-035220-452-d01ac87c/`: native1, `battle wait ended`120 gerçek saniyede sonuç bulmadı. Eski çıktı ve script değiştirilmedi. `shots/05-officer-committed-forces.json` ve `shots/battle-timeout-65.json` dosyalarının yalnız Battle bölümleri okundu.

İlk emir snapshot'ı t24,7499; timeout t144,7542. Sonuç henüz üretilmemiştir: Active=true, Paused=false, Ended=false, HasOutcome=false, iki Hold0. Bu yüzden snapshot'ın outcome alanındaki Casualties0 gerçek “hiç kayıp yok” demek değildir. Kalan oyuncu men toplamı449+372+289+246=1356; başlangıç1600'e göre o anda244 kişi eksiktir. Bu aritmetik tamamlanmış savaş raporu değildir.

Line(slot1)449 kişi, moral74, Ammo6, Commandable=true; konumu(−7,274;3,862), hedefi(−7,006;3,987). Militia372 ve cavalry289 kişiyle routed+withdrawn; oyuncu artillery246/moral69,70/Ammo2 ile hâlâ aktiftir. Düşmanın üç saha birliği routed+withdrawn; kalan artillery211/moral51,70/Ammo0, konumu(22,068;20,424). Dolayısıyla burada sıfır cephaneli düşman topçusunu izleyerek beklemek hedefi kendiliğinden kazandırmaz.

Neden: `TacticalBattleCommands.MoveSelected` çoklu seçimde birliklerin merkezden offset'ini korur. İki piyadeye verilen(1,4) noktası bütün birimlerin gideceği ortak nokta değildir; Line yaklaşık(−7,4), Militia(9,4) alır. Timeout'ta Line konvoydan≈11,31 uzaktadır. `UpdateObjective` oyuncu piyade/süvarisi için<6,5 yakınlık ve düşman için9 içinde itiraz yokluğu ister;45 saniye birikmelidir. Routed/withdrawn hesaba girmez. Artillery kendi başına birikim sağlayamaz. İlk snapshot'taki PlayerHold3,35, sonraki birlik kaçışlarıyla sıfıra dönmüştür.

Root'a önerilen anlamlı ikinci emir fazı: ilk çatışma emirlerinden75 saniye sonra (yaklaşık t99,75) pause + yeni snapshot; **yalnız slot1 replace**, line, fire free, `move 4 3`; pause off. `wait arrived30` ve gerçek arrival snapshot'ından sonra `wait ended60`.11,31 birimlik yol için Line'ın taban1,65×0,84 hızı yaklaşık8,2 saniye verir; arazi/yorgunluk/çarpışma nedeniyle30 hareket sınırı önerilir.60 sonuç sınırı varıştan sonra gereken45 tutuşa pay bırakır. Bu eski emri daha uzun beklemek değil, sağlam kalan piyadeye hedefi gerçekten işgal etme emridir.

Risk açık: t≈100'deki düşman durumu henüz gözlenmedi. Son snapshot'ta düşman topçusu Ammo0 olsa da erken fazda birkaç mermisi kalmış olabilir; kalan saha birliği9 içinde itiraz edebilir. Yeni snapshot bunu gösterecek. Artillery AI kendi dalında erken döner; PlayerHold>3 olduğunda piyade gibi konvoya koşmaz. Mevcut hedefi(18,15) de9 dışındadır. Oyuncu topçusu kendi mevzisinde ateşe devam edebilir; konvoyu almak için uzaktaki düşman topçusunu kovalamak şart değildir. Kaynak, denge veya savaş sonucu değişikliği önerilmedi.

## İkinci gerçek koşu: ayrı Line emriyle doğal zafer

`output/verify/officer-battle-second-maneuver-20260906-035947-297-9703935f` tamamlandı: PARTIAL / native exit0,172 saniye,186 komut,65 assertion,13 PNG ve17 JSON. Root03b manevra,04 RU savaş raporu ve08 RU geri alma fiyatını açıp gördü. Bu ajan makbuz, dinamik snapshot ve state farklarını salt okunur denetledi; test, Unity, derleme veya oyuncu başlatmadı. Bu mevcut build incelemesidir; yeni kaynak, EditMode ve browser atlanmış, ayrı GREEN ilan edilmemiştir.

İlk RED'nin `result.json` dosyası yeniden okundu: native1,155 saniye, Player FAILED;120 saniyelik sonuç bekleme sınırında bitmemiş savaş olduğu önceki bölümdeki gibi kalır. İki immutable `review.script` dosyasının diff'i yalnız tek emir bloğu değişimini gösterir: eski `battle wait ended 120` yerine75 saniye gerçek bekleme, pause ve05b snapshot, **slot1 replace / line / fire free / move4,3**, devam,30 saniyelik arrival sınırı ve60 saniyelik sonuç sınırı. İlk sonuç veya eski kanıtlar değiştirilmedi; yeni başarı yalnız aynı eski emri daha uzun beklemek değildir. İki koşu da aynı `officer-commission-import-fixed-20260906-034710-694-4b17a317` oyuncusunu kullandı.

Gerçek zaman çizelgesi:

- 05b: Elapsed 99.751488, Paused=true, Ended=false, Hold 0. Line Id0: 449 kişi, Ammo 6, komuta edilebilir; konum (−7.27425, 3.86185), eski hedef (−7.00578, 3.98698).
- 05c: Elapsed 108.051994, SelectionArrived=true, seçili yalnız Id0; konum (3.74895, 3.01919), hedef (4, 3), 449 kişi. Yeni yol ≈8.300507 saniye sürdü. Hold 4.75, EnemyHold 0; tutuşun tam varıştan önce menzile girişle başlaması gerçek kuralla uyumludur.
- 06: Elapsed 148.354446, Ended=true, HasOutcome=true, Won=true, PlayerHold 45 / EnemyHold 0. OriginalTroops 1600, Casualties 244, MilitarySuppliesRecovered 24. EndingMorale 46.27956; kampanyaya dönen moral 49.27956. Savaş kimliği `battle-0-2-ile-champagne`.

`battle verify-return` geçti.07 dönüş state'i: Troops1356 / Manpower2000, Gold600 / Food298 / Supplies109; Champagne ordu/seçim bölgesi, Moves0, Fatigue35, Power59. Supplies hesabı90−5 gerçek yürüyüş+24 kazanım=109. Commission ve used=true, Loyalty61, Relationship52, Ambition83; tek resolved battle ve aynı kimlikle pending victory vardır.244 kayıp bu tamamlanmış savaşın gerçek sonucudur; önceki timeout anındaki eksik kişilerin aritmetiğiyle aynı sayı olması ilk koşuyu başarıya çevirmez.

## Gerçek fiyatlar, state farkları ve arşiv

09→10 prim: canlı1356 askerden ceil(1356/12)=113 Gold;600→487. Dumas Loyalty61→66, yalnız asıl Champagne Control70.5→73.5, PendingVictoryId boş. Troops, Manpower, Relationship, Ambition, commission ve used aynı. Değişen üst alanlar yalnız Gold/Regions/Characters/Journal/PendingVictoryId; nested farklar yalnız Champagne Control ve Dumas Loyalty. Tek `log.victory.bonus(region.champagne,113,5,3)` eklendi; eski journal kuyruğu aynı.

12→13 geri alma: aynı canlı1356 kişi için ikinci113 ödeme, Gold487→374. Değişen alanlar yalnız Gold/Journal/DumasOfficerCommission. Asker ve Loyalty66 aynı; hak false, global used=true. Tek `log.commission.revoked(113)` ve aynı eski journal kuyruğu. Önceki aktif hakla budget1000 reddi script'in `same` kontrolünden geçti.

13→14 budget imzası: yalnız policy=budget, target1000, due2 ve tek `log.establishment.budget_scheduled(1000,2)` kaydı. Week0 ve1356/2000 kişi korunur; bu koşuda hafta ilerletilmedi, gerçek azaltım henüz yapılmadı.14→15 aynı emrin gerçek save/load'udur, iki haftalık bütçe hesabı olarak sunulmaz.

Üç ham JSON çifti byte-eşit:

| Çift | SHA256 |
| --- | --- |
|07 dönüş =08 açık zafer yükleme |`2AC64E3D5E42B5C5260031FCB1EDAFDF8D341216E48E88F95086E1167C283D2F` |
|10 prim =11 prim yükleme |`0A5B8E92B42FD681E3E3ABDE68CA34FA05CACC6C0AD4AC7A21AC73A5BCA2540F` |
|14 budget =15 budget yükleme |`E0D37BA9A83A9540837F6DEFB9E97D17E292B8CC5D5EB6916F87563B65725DB1` |

Reused manifest v1 yeniden okundu:141 kayıt /141 gerçek dosya; tüm boyutlar ve SHA256 değerleri aynı, eksik/farklı0. Runtime SHA256 `0F2CACDA5F0E4270128F883460EEBF8BB0B0A36CF59FE5A60EA3D7053F9B2581`. Son player log Auto shots passed ve normal Input Shutdown ile biter; native0 raporu ayrıca mevcuttur. İzole protocol tamamlanışı `2026-09-06T04:02:26.7775923Z`. Bulgular gerçek taktik emirleri ve sonuç taşınmasının bu rotasına aittir; bu komutlar Windows fare/klavye girdisi kanıtı olarak sunulmaz.
