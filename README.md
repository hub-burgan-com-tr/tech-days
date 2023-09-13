# Burgan Tech Days eBurgan Uygulaması

Bu uygulama Burgan Bankası Tech Days için Ömür UÇUM tarafından hazırlanmış olup dapr'a giriş niteliğindeki eburgan-dapr branch'indeki uygulamanın son halinden önceki halidir. Basit bir mikroservis yapısında kurgulanmıştır.

## Uygulamanın local de çalıştırılması

Bu uygulamanın kendi bilgisayarınız da (local) çalıştırılması için tavsiye ettiğim ilk yöntem 1. Seçenekde detaylarını anlattığım "self-hosted" metodudur. Ayrıca docker compose dosyalarını da hazırlamaya çalıştım fakat pek test edebilme fırsatım olmadı. Bu yüzden docker dosyalarıyla çalışmadan önce sizlerinde kontrol etmesinde fayda vardır.


## Uygulama Mimarisi Hakkında

- **frontend** 	
	ASP.NET 6 ile geliştirilmiş basit bir arayüze sahip uygulamadır. Toplamda 3 farklı ürünün listelendiği ve ziyaretçilerin bu ürünlerin detaylarına bakıp adet belirttikten sonra satın alma işlemi yapabildikleri mikro servis'dir

- **catalog** 
	Bu mikroservis uygulama içerisinde listelenen ürünlerden sorumlu olup in-memory olarak tanımlamalar yapılmıştır.

- **ordering** 
	Siparişlerin alındığı servis olup müşterilere teşekkür pub-sub özelliğini kullanarak mail atmaktadır.
