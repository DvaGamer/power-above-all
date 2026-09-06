# Etiket A — kısa gerçek oyuncu incelemesi

Yeni fixture: `tools/labels-review.script`; `volley-review.script` değiştirilmedi. Yalnız mevcut AutoShots komutları kullanılır; kaynak/state enjeksiyonu, efekt çağırma, arayüz gizleme veya yeni public API yok. Bu ajan fixture'ı çalıştırmadı; root ortak gate'in sahibidir.

Rota, güvenilir volley hazırlığını korur: iki hat piyadesi −8,0 grup hedefi; süvari −10,10 sütun; topçu14,−6 ve tüm oyuncu menzillileri hold fire. Varış/hazırlık beklenir, topçu tek gerçek nişanlı yaylım yapar ve bütün erken/gelişmiş/geç görüntülerde seçili kalır. Ardından piyadeler normal ayrı hareket emirleriyle −4,2 ve1,4'e yaklaşır; grup hareketi eski16world aralığını koruyacağı için iki ayrı emir gerekir. Birlikler gerçekten yürür; ikisi yeniden seçilip varışta duraklatılır. Aynı yoğun sahne RU/TR kaydedilir, normal retreat/rapor aktarımıyla bitirilir.

| Artefakt | Somut kabul |
| --- | --- |
| `labels-01-arty-ready-ru` | Seçili topçu etiketi kendi asker/namlu önünü örtmez, iki metin satırı tamdır. Bağlantı ilgili alaya yakın ve oksuzdur |
| `labels-02-before-accepted-order.json` → `03-arty-early-ru` | SeçiliId3/PlayerSlot4 korunur; kendi mühimmatının bir azalması gerçek atışı ayırır. Ortadaki düşman dumanı topçu kanıtı sayılmaz. .12s bekleme, ilk75ms flash'ı yakalama garantisi değildir |
| `03-arty-early-ru` = `04-arty-early-held-ru` | Aynı dilde .75s pause çifti: JSON aynı kalmalı, PNG'de etiket/duman/figür kayması olmamalı |
| `05-arty-grown-ru`, `06-arty-late-ru` | Aktif +1.1s ve +2s sonrasında seçili kendi topçunun etiket/siluet/duman ilişkisi görünür. Puff sayısı/yaşı etiket yanını oynatmamalı; savaşın diğer hareketleri gerçek çakışma yaratırsa yeniden yerleşim meşrudur |
| `07-infantry-dense-ru` = `08-infantry-dense-held-ru`; `09-infantry-dense-tr` = `10-infantry-dense-held-tr` | SeçiliId kümesi{0,1}, iki gerçek varış; her dilde .75s pause çifti. İki tam dost etiketi ayırt edilir, düşmanın görünen kritik/isabet bilgisi kaybolmaz; RU/TR aynı duraklatılmış dünya konumlarıdır |
| `labels-11-campaign-return-ru` | Mevcut `battle verify-return`, BattleActiveFalse ve ResolvedBattleCount1 normal dönüşü doğrular; zafer koşusu değildir |

Toplam10PNG,12JSON hedeflenir. Battle wait tavanları toplam105s, açık sabit beklemeler yaklaşık9.1s; normal tamamlanma150s altında hedeflenir. Uzun doğal zafer/ended120 beklemesi yoktur. Komut kareleri, PNG yazma ve başlangıç yükü ayrıca zaman alır; AutoShots'un her PNG için10s hata beklemesiyle patolojik depolama yavaşlığında mutlak150s garantisi verilmez. Gerçek süre gate raporunda okunmalıdır.

Yoğun fallback'in beklenen bedeli: on adayın hiçbiri temiz değilse düşük öncelikli panel kısmen alan paylaşabilir. Kabul, her yerde sıfır piksel çakışması değildir; seçili iki panelin metni tam ve birbirinden ayırt edilir olmalı, ince bağlantı alayı yanlış göstermemeli, panel header/pausa/alt emir alanına girmemelidir. Sürekli yer sıçraması, seçili topçunun namlusunun tekrar kapatılması, tam metin kaybı veya pauzada aynı durumun değişmesi reddir.

Gerçek sonuç: root'un birleşik `dumas-labels-first-20260906-021758-659-0da55b25` gate'i, önce iki Dumas kampanyası ve ardından bu etiket rotasıyla toplam99s / GREEN tamamlandı. Bütün üç PNG ve JSON pause çifti SHA-256 açısından birebir aynı; RU/TR yoğun merkez JSON'ları da aynı. Seçili topçu ve iki piyadenin kendi gövdeleri açık, bütün metinleri okunur. 05/06'da iki diğer panelin yaklaşık6×3 px köşe teması düşük örtme fallback'inin gerçek, metni kapatmayan sınırıdır. Ayrıntılı kare gözlemleri `regiment-label-readability.md` dosyasındadır. Bu ajan çalıştırma yapmadı.
