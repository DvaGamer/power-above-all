# Rol çekirdeği — 6 Eylül gece paketi

- Kaynak: ana ajanın çalışma tasarımı `ROLE_SLICE.md`. Üç rolün kesin dünya/tarih kararı olduğu iddia edilmez.
- Dosyalar: `CampaignCore.cs` yalnız state/partial/Create legacy/hafta engeli/doğrulama kancaları; yeni `CampaignRoles.cs`, `CampaignArchive.cs`, `roles-core.json`.
- Ortak emir, ekonomi, yürüyüş, savaş, asker yeniden toplama ve 200 haftalık sefer kuralları değiştirilmedi. Sekizinci hafta bitişi yoktur.
- API: `Create(roleId)`, `GetMandateTerms(state,regionId)`, `GetObligationTerms(state)`, `CanIssueMandate`/`IssueMandate`, `CanResolveMandate`/`ResolveMandate(state,expectedId,choice)`, `MandateId(obligation)`, `MandateDue(state)`.
- Seçim kimlikleri `fulfil`/`break`; türler `royal_advance`/`civic_pledge`/`field_levy`. Erken ödeme/ret açıktır. Ekmek dilekçesi her iki işlemden önce çözülür. Vadesi gelen yükümlülük sonraki haftayı hiçbir mutasyon yapmadan engeller.
- Terimler yeni nesneler olarak verilir; önizleme nesnesini değiştirmek uygulama hesabını değiştirmez. Nominal siyasi etkiler 0–100'e sınırlanır. Kaynak taşması kısmi kazanç yerine atomik ret üretir.
- Yükümlülük ilk bölgeye bağlıdır. İşlem kimliği tür/veriliş haftası/bölgeden türetilir; eski pencere yeni sözleşmeyi çözemez. Çözülünce yükümlülük temizlenir, bekleme süresi kalır.
- `CampaignArchive.Serialize(state,prettyPrint=true)` v2 üretir; `Deserialize(json)` v1 ve v2 okur. Eksik/yanlış sürüm reddedilir. Eski v1 rolü yoksa veya legacy ise, yeni yükümlülük/bekleme içermiyorsa legacy'ye alınır. Bozuk yeni rol/veri eski moda düşürülmez.
- V2 borç türü/rol/bölge eşleşmesi, hafta sırası, iki haftalık vade, dört haftalık bekleme ve sabit v2 ödeme tutarları doğrulanır. Son takvim haftalarında tutulamayacak söz oluşturulamaz.
- `Create()` yalnız eski `log.begin`; seçili rol `log.role.crown`/`.assembly`/`.army` ekler. Yerelleştirme anahtarları yalnız izin verilen `log.role`, `log.mandate`, `error.role`, `error.mandate` ailelerindedir.
- Statik kaynak incelemesi ve `git diff --check` yapıldı. Bu ajan Unity, derleme veya test başlatmadı. Yeni rol testleri verification_safety; GameApp ve tam kapı ana ajandadır. JsonUtility boş yükümlülük dönüşümü gerçek Unity testinde ayrıca doğrulanmalıdır.

## Gerçek JsonUtility hatasının düzeltmesi

- Ana ajanın `roles-first-20260905-223436-038-2b02fc10` EditMode kapısı: 31/48 geçti, 17 başarısız. 15 hata boş yükümlülüğün kopya/arşivde sınıf nesnesine dönüşmesi; bir gerçek v1 eksik alan geçişi; bir eski sübvansiyon testi aynı boş nesnenin vadesi0 sanılıp haftayı durdurmasıdır. Test beklentileri değiştirilmedi.
- Saf çekirdek korunarak kalıcı alan `List<MandateObligation> Mandates` oldu; mevcut `Obligation` API'si serileştirilmeyen özellik üzerinden aynı kaldı. Yeni sefer açıkça boş liste kurar. Sınıf alanında varsayılan oluşturucu yoktur; eksik v2 listesinin geçerli veri gibi doldurulması amaçlanmaz.
- Doğrulama null listeyi, birden fazla öğeyi, null öğeyi ve içeriği bozuk tek öğeyi reddeder. Boş/default borç nesnesi yok sayılmaz. Yalnız v1 arşiv geçişi eksik/boş listeyi gerçek boş listeye dönüştürür.
- UI ve Core çağrı imzaları korunur. Yeni Unity kapısı ve liste bozulması testleri ana ajan/test ajanı tarafından yapılacaktır; bu düzeltme sonrası başarı henüz iddia edilmez.

## Eksik arşiv alanı için dar okuma düzeltmesi

- Liste kapısı 52/53 geçti: eski 25 test ve üç rolün akışları artık geçiyor. Tek kalan hata, v2 içindeki eksik `Mandates` alanının `FromJson` tarafından boş listeye dönüştürülüp kabul edilmesidir.
- `Deserialize`, geçersiz ve yalnız mevcut okumaya ait liste işaretiyle hazırlanan zarf üzerinde `FromJsonOverwrite` kullanır. Eksik alanın korunması sayesinde v2 geçersiz işareti reddeder. V1 geçişi işareti içerik metniyle değil nesne kimliğiyle tanır. Sürüm varsayılanı0, başlangıç State ise temel doğrulamadan geçemeyen boş durumdur; eksik sürüm/state başarılı kayıt sayılmaz.
- JSON için regex veya ikinci ayrıştırıcı eklenmedi. İç içe nesnelerde eksik alanın ve açık null listenin davranışı yeni gerçek Unity kapısında doğrulanacaktır; bu not 53/53 başarı iddiası değildir.

## Son arşiv biçimi — standart .NET JSON okuyucusu

- Sonraki kapı53/54: eksik liste reddedildi, ancak açık `null` hâlâ Unity tarafından `[]` yapıldı. İşaret yaklaşımı kaldırıldı.
- Kurulu Unity Mono'nun `System.Runtime.Serialization.dll` içindeki `DataContractJsonSerializer` kullanılır; paket veya ikinci JSON ayrıştırıcısı yüklenmedi. Arşiv katmanı artık Unity API'si kullanmaz; UI'nın JsonUtility durum kopyaları boş liste modeliyle devam eder.
- Yalnız üç yeni durum alanına standart `.NET OptionalField` eklendi. Mono deneyi, bunun v1 eksik alanlarını null/0 bırakırken `[]`, açık null ve eksik koleksiyon ayrımını koruduğunu doğruladı. V2 doğrulama null/eksik listeyi reddeder. `Obligation` özelliği JSON'a yazılmaz.
- `Serialize` UTF8, isteğe bağlı girintili JSON üretir. `Deserialize` tipli okuyucudan sonra aynı sürüm/geçiş/alan doğrulamasını uygular; `SerializationException` arşiv API'sinde `ArgumentException` olarak bildirilir.
- Gerçek Core/Roles/Archive kaynakları kurulu Mono ile bağımsız derlendi. Dört rolün boş kayıtları; üç rolün açık ve ödenmiş kayıtları; null/eksik v2 liste reddi; gerçekten alanları eksik v1 geçişi; bozuk JSON ve eksik/null State reddi geçti. Deney kaynakları `ArchiveSerializerProbe.cs`, `ArchiveSerializerIntegrationProbe.cs`; çıktılar `output/` içindedir. Unity veya oyuncu başlatılmadı.
- Ana ajan ayrıca gerçek Unity referanslarıyla Runtime14/Editor3 statik derlemesini hatasız geçtiğini bildirdi. Tam54NUnit/player kapısı henüz bekleniyor.
