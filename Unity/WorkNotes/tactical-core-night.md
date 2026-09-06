# Taktik çekirdek — uygulama kaydı

6 Eylül 2026, `b90c7ae` rol checkpoint'inden sonraki çalışma. Root'un onayladığı `tactical-rules-plan.md` seçenek1 uygulandı. Unity/player/derleme çalıştırılmadı; yalnız gerçek Unity referanslarıyla statik C# derlemesi yapıldı.

## Uygulanan sınır

- `TacticalBattle` sealed partial; simülasyon `TacticalBattleSimulation.cs` içinde. Kalıcı alay kimliği, kaynak liste ters çevrilse de hedef eşitliğini ve rastgele atış sırasını sabit tutar.
- 20 Hz adım: ilk görüntüden eşzamanlı AI/hareket/kaçınma; güncel konumların ortak görüntüsünden en çok bir saldırı/alayı; bütün saldırıları hesaplama; toplam kayıp/moral/bütünlük; birlikte bozgun dalgaları; hedef/bitiş. Aynı adımda bozulan düşmanın hazırlanmış atışı iptal edilmez. Raporun ağırlıklı morali de kimlik sırasıyla toplanır.
- `OrderVolley` yalnız `AimedVolleyPending` koyar; tekrarlı emir birleşir. Duraklatılmış/inaktif/bitmiş `Simulate` hiçbir durumu değiştirmez. Bir sonraki adım geçersiz hedefte maliyet/efekt üretmez. Gerçek saldırının reaksiyonu ve dumanı aynı `visualClock` değerini alır.
- Piyade/milis/süvari temas erişimi3,7; ayrı `ContactReload`. Piyade/milis cephanesiz de dövüşür, `FireAtWill=false` yakın savunmayı kapatmaz. AI cephanesizken12/16 mesafesinde durmak yerine2,4'e yaklaşır. Oyuncuya otomatik takip verilmedi; topçuya yakın dövüş verilmedi.
- Temas3,4sn × mevcut yorgunluk çarpanı; iki saldırı türü arasında en az0,6sn toparlanma. Piyade0,18/milis0,14/süvari mevcut0,34 temel katsayıları. Mevcut moral/yan saldırı, süvari-kare ilişkisi korunur; atışa özgü yükseklik/orman/nişan bonusları temasa geçmez. Yorgunluk100 minimum1 kayıp kuralını sıfırlamaz.
- `CanVolley` yalnız menzilli hazır olmayı; `CanAttack` her iki saldırı türünü anlatır. API ajanının snapshot'ı `ContactReload/AimedVolleyPending` taşır. Kartlar yakın temas beklemesini, yeni RU/TR nedenler cephanesiz yaklaşma/temas/yalnız süvari saldırısını gösterir.

## Test kapsamı ve henüz doğrulanmayanlar

Yeni `TacticalSimulationTests.cs`: karşılıklı bozgun + nişanlı kuyruk için normal/ters sıra; çoklu isabette0 altına inmeyen mevcut ve vurulanın karşı atışı; üç panik dalgası/yalnız bir şok; ortak hareket/kaçınma; bitene kadar cephanesiz ve yorgun piyade/milis; ateşi keserken temas savunması; topçu/kaçan/çekilmiş saldırı sınırları; son mermiden sonra kısa temas toparlanması; kuyruk-pause; geçersizleşen hedef; eşit mesafede kimlik seçimi. Gerçek sonuç henüz root Unity gate'inde bekleniyor.

Mevcut `BattlePresentationTests` içindeki üç `Shoot` çağrısı `OrderVolley`→`Simulate` sınırına geçirildi. Saat, cephane, kayıp, efekt, ses, duraklatma ve tek callback assertion'ları korunur. Hazırlık diğer alayların otomatik ateşini durdurarak eski tek atış senaryosunu korur. Sunumdan bağımsız sonuç testine iki yeni alan da eklendi.

Statik runtime17/editor3 dosya PASS, warning yok (kaynak değişikliği öncesi son küçük HUD metni dışında bütün yeni simülasyon dahil). Bu gerçek EditMode testi veya render kanıtı değildir. Yeni sayıların savaş temposu ve kartların RU/TR sığması root'un gerçek görüntüsüyle ayrıca incelenmeli. Tepe LOS, yeni arazi veya genel savaş dengelemesi eklenmedi.
