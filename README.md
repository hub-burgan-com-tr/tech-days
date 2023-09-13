# eBurgan Dapr Örneği

Bu uygulama Tech Days kapsamında Dapr'ın hızlı bir şekilde sunumu yapılması amacıyla oluşturulmuştur. Uygulama Ömür UÇUM tarafında yazılmıştır

## Uygulamanın local de çalıştırılması

Bu uygulamanın kendi bilgisayarınız da (local) çalıştırılması için tavsiye ettiğim ilk yöntem 1. Seçenekde detaylarını anlattığım "seld-hosted" metodudur. Ayrıca docker compose dosyalarını da hazırlamaya çalıştım fakat pek test edebilme fırsatım olmadı


### 1. Seçenek - CLI üzerinden self-hosted Çalıştırmak

**İhtiyaçlar:** 

1) [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/) makineniz de kurulu olması gerekiyor
2) Kullanıdığınız işlerim sistemine bağlı olarak Docker Desktop uygulamasının da kurulu olması gerekmektedir.

### Dapr cli kurulumu sonrasında terminal açarak `dapr init` komutunu da çalıştırmanız gerekmektedir. Bu saye de self-hosted mode için gerekli alt yapıyı oluşturmuş olursunuz.

### Email gönderim özelliğini kullanabilmek için local de `maildev` adında bir container olmalıdır. Bu container için aşağıdaki komutu kullanabilirsiniz;

`dapr run -p 1080:80 -p 1025:25 maildev/maildev`

### Basit bir onaylama mekanizması olduğundan ve biraz da şahsi istekler yüzünden kredi kartı formatını kontrol ediyorum. Eğer örnek kredi kartı bilgilerine ihtiyaç duyarsanız [buradan](https://support.bluesnap.com/docs/test-credit-card-numbers) alabilirsiniz

3 tane terminal açarak aşağıdaki adımları izleyin;

- [x] `frontend` dosyası içine girerek `start-self-hosted.ps1` dosyasını çalıştırın. Aynı işlemi diğer terminaller de `catalog` ve `ordering` için de yapın.

- [x] Uygulamaların çalışacağı portlar kendi dosyalarında oluşturulumuş olan PowerShell dosyalarında tanımlanmıştır. Bu tanımlara göre 

- [x] `frontend` : `http://localhost:5266/` 
	  `catalog` : `http://localhost:5016/swagger/index.html` 
	  `ordering` : `http://localhost:5293/swagger/index.html` adreslerinden erişilebilir olacaktır.

- [x] `Zipkin` kayıtları için http://localhost:9411/zipkin/? adresini kontrol edebilirsiniz.

- [x] `maildev` container'ı kullanılarak ordering servisi tarafından gönderilen mailleri `http://localhost:1080/#/` adresinden takip edebilirsiniz.


### 2. Seçenek - Docker Compose ile Çalıştırmak

`docker-compose.yml` dosyasının olduğu dizin de sırasyla aşağıdaki komutları çalıştırı;

- [x] `docker-compose build`
- [x] `docker-compose up`

Yukardaki komutları çalıştırdıktan sonra;

- [x] `frontend` : `https://localhost:5001` adreslerinden erişilebilir olacaktır.
- [x] `Zipkin` : `http://localhost:9412/zipkin/`
- [x] `maildev` : `http://localhost:1080/#/`


## Uygulama Mimarisi Hakkında

- **frontend** 	
	ASP.NET 6 ile geliştirilmiş basit bir arayüze sahip uygulamadır. Toplamda 3 farklı ürünün listelendiği ve ziyaretçilerin bu ürünlerin detaylarına bakıp adet belirttikten sonra satın alma işlemi yapabildikleri mikro servis'dir

- **catalog** 
	microservice provides the list of events that tickets can be purchased for. To keep this demo as simple as possible, the catalog microservice returns a hard-coded in-memory list. Created with `dotnet new webapi -o catalog --no-https` (no https because we're going to rely on dapr for securing communication between microservices). A dapr cron job calls a scheduled endpoint on this.

- **ordering** 
	Siparişlerin alındığı servis olup müşterilere teşekkür pub-sub özelliğini kullanarak mail atmaktadır.


## Azure Kubernetes ortamında çalıştırılması
[`aks-deploy.ps1`](aks-deploy.ps1) PowerShell scripti izlenmezi gereken adımları içermektedir. Bu dosyanın direk çalıştırılmaması gerekmektedir. Azure CLI kurulu olduğundan emin olun. Hazırlamış olduğum script örnek kodları içermektedir. Daha önce Azur CLI kullanarak cloud ortamda bir uygulama ayağa kaldırmadıysanız lütfen dikkatli olun ve tüm adımlarınızı kontrol edin.

storage account ve service bus için unique bir isim belirlemeniz gerekmektedir. Ayrıca bu isimleri componentlerin ayarlar dosyalarında da tanımlamalısınız. Bu isimlerin eşleşmemesi uygulamanın çalışma zamanında (runtime) hata vermesini sebep olacaktır