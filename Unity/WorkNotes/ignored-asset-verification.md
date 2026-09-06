# Gecersiz GUID ve atlanan asset kaniti

2026-09-06: `output/verify/officer-commission-first-20260906-033212-825-8f555df6/build.log` icindeki 182-183. satirlar localization metadata GUID hatasini bildiriyor. Ikinci mesaj acikca `does not have a valid GUID` ve `Asset file will be ignored` diyor. Eski otomatik GREEN sonucu gorsel kabul degildir; root ekranda ceviri anahtarlarini gordu. Mevcut kanitlar degistirilmedi.

`Assert-CleanLog` artik bu iki parcayi ayni satirda iceren kesin import kaybini reddeder. Launcher da mevcut build.log icinde ayni kanit varsa GREEN adayi atlar; onceki saglam adayi deneyebilir. Yalniz YAML parser'in string-matching fallback mesaji reddedilmez. Eski logu olmayan makbuzlarin onceki sinirli uyumlulugu korunur.

PS fixture kesin kaybi reddetme ve yalniz fallback mesajini kabul etme durumlarini kapsar. Node fixture daha yeni GREEN + hatali import yerine onceki build'i secmeyi, makbuz/logun degismedigini ve yalniz fallback mesaji olan yeni build'in secilebildigini kapsar. Bu gorevde test, derleme veya oyuncu sureci calistirilmadi; root merkezi kontrolu calistiracak.
