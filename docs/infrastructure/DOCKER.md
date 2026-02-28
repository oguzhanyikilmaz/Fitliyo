# 🐳 Fitliyo Docker Kurulum ve Kullanım Rehberi

## 🔒 Arama Motoru İndekslemesini Engelleme

Bu proje, üretim ve geliştirme ortamlarında arama motorlarının siteyi indekslemesini engelleyecek şekilde yapılandırılmıştır.

- Nginx üzerinden tüm yanıtlara `X-Robots-Tag: noindex, nofollow, noarchive, nosnippet` başlığı eklenir.
- `robots.txt` statik olarak ve Nginx üzerinden `Disallow: /` döndürür.
- Uygulama pipeline'ında da güvenli olması için aynı `X-Robots-Tag` başlığı middleware ile eklenir.

Uygulama ayağa kalktıktan sonra doğrulama için:

```bash
# Başlık kontrolü
curl -skI https://localhost/ | grep -i "x-robots"

# robots.txt kontrolü
curl -sk https://localhost/robots.txt
```

Nginx konfigürasyonu değiştiğinde yeniden yüklemek için:

```bash
docker compose restart reverse-proxy
```

## 📋 Genel Bakış

Bu rehber, Fitliyo projesini Docker ortamında çalıştırmak için gerekli adımları açıklar. Docker Compose kullanarak tüm servisleri tek seferde başlatabilirsiniz.

## 🆕 Yeni Özellikler

### ✅ Environment-Based Configuration
- **Development**: Uzak sunucu servisleri (165.22.28.145)
- **Production**: Docker servisleri (localhost/sqlserver/redis/rabbitmq/elasticsearch)
- **Otomatik Seçim**: Environment'a göre otomatik konfigürasyon

### 🔒 Güvenlik Geliştirmeleri
- **Exception Handling**: Development'ta detaylar görünür, Production'da gizli
- **Information Disclosure**: Hassas bilgiler korunur
- **Attack Prevention**: Saldırganlar sistem hakkında bilgi alamaz

### 🔄 Restart Policy
- **Maksimum 5 Deneme**: Servisler 5 kez başarısız olursa durur
- **Otomatik Recovery**: Geçici hatalar otomatik düzelir
- **Resource Tasarrufu**: Sonsuz döngü engellenir

### 📊 Monitoring ve Logging
- **Health Checks**: Tüm servisler izlenir
- **Restart Monitoring**: Restart sayıları takip edilir
- **Performance Monitoring**: Resource kullanımı analiz edilir

## 🚀 Hızlı Başlangıç

### 1. Docker Compose ile Başlatma

```bash
# Tüm servisleri başlat
docker-compose up -d

# Logları izle
docker-compose logs -f

# Belirli bir servisin loglarını izle
docker-compose logs -f Fitliyo-web
```

### 2. Servisleri Durdurma

```bash
# Tüm servisleri durdur
docker-compose down

# Verileri de sil (dikkatli olun!)
docker-compose down -v
```

## 🔧 Environment Configuration

### Environment-Based Configuration

Proje, environment'a göre farklı konfigürasyon dosyaları kullanır:

| Environment | Dosya | Kullanım |
|-------------|-------|----------|
| **Development** | `appsettings.Development.json` | Geliştirme ortamı (uzak sunucu) |
| **Production** | `appsettings.Production.json` | Docker/Production (localhost) |

### Docker'da Environment Ayarları

Docker Compose'da environment variable'ları şu şekilde ayarlanmıştır:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production  # appsettings.Production.json kullanır
```

### Restart Policy

Uygulama servisleri için restart policy ayarları:

```yaml
restart: on-failure:5  # Maksimum 5 kez yeniden başlatma denemesi
```

**Restart Policy Seçenekleri:**
- `no`: Hiç yeniden başlatma
- `always`: Her zaman yeniden başlat
- `on-failure`: Sadece hata durumunda yeniden başlat
- `on-failure:5`: Maksimum 5 kez yeniden başlatma denemesi
- `unless-stopped`: Manuel durdurulmadığı sürece yeniden başlat

### Güvenlik Ayarları

#### Exception Handling

Environment'a göre exception handling ayarları:

**Development Ortamında:**
```csharp

  SendExceptionsDetailsToClients= true;
  SendStackTraceToClients= false

```

**Production Ortamında:**
```csharp

  SendExceptionsDetailsToClients= false;
  SendStackTraceToClients= false

```

**Güvenlik Avantajları:**
- ✅ **Development**: Exception detayları görünür (debugging için)
- 🔒 **Production**: Exception detayları gizli (güvenlik için)
- 🔒 **Information Disclosure**: Hassas bilgiler korunur
- 🔒 **Attack Prevention**: Saldırganlar sistem hakkında bilgi alamaz

## 🌐 Servis Bağlantıları

### Docker Network'te Servis İsimleri

| Servis | Container Name | Internal URL | Açıklama |
|--------|----------------|--------------|----------|
| **Database** | `Fitliyo-sqlserver` | `sqlserver:1433` | SQL Server 2022 |
| **DB Backup** | `Fitliyo-db-backup` | — (port yok) | Self-contained yedekleme sidecar |
| **Redis** | `Fitliyo-redis` | `redis:6379` | Cache |
| **RabbitMQ** | `Fitliyo-rabbitmq` | `rabbitmq:5672` | Message broker |
| **Elasticsearch** | `Fitliyo-elasticsearch` | `elasticsearch:9200` | Arama ve loglama |
| **Kibana** | `Fitliyo-kibana` | `kibana:5601` | Log görselleştirme |

### Database Backup Sidecar (Fitliyo-db-backup)

`Fitliyo-db-backup` container'ı tamamen sunucu içinde çalışan, dış servise bağımlı olmayan otomatik yedekleme servisidir.

- **Image**: `mcr.microsoft.com/mssql-tools:latest`
- **Zamanlama**: Her gün UTC 02:00 (Türkiye 05:00)
- **Saklama**: Günlük 3 gün, pre-deploy 7 gün
- **Volume**: `/root/data/sqlserver-backups:/backups` (SQL Server ile paylaşımlı)

```bash
# Manuel yedek al
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh manual

# Yedekleri listele
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh list

# Deploy öncesi yedek
docker exec Fitliyo-db-backup /bin/bash /scripts/backup.sh pre-deploy build-152
```

Detaylar: [`docs/operations/DB_BACKUP_RESTORE.md`](../operations/DB_BACKUP_RESTORE.md)

### Production Configuration

`appsettings.Production.json` dosyası Docker servis isimlerini kullanır:

```json
{
  "ConnectionStrings": {
    "Default": "Server=sqlserver;Database=FitliyoDB;..."
  },
  "Redis": {
    "Configuration": "redis:6379"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq"
  },
  "Elasticsearch": {
    "ConnectionString": "http://elasticsearch:9200"
  }
}
```

## 📊 Servis Portları

| Servis | External Port | Internal Port | URL |
|--------|---------------|---------------|-----|
| **Fitliyo Web** | `43332` | `80` | `http://localhost:43332` |
| **SQL Server** | `1433` | `1433` | `localhost:1433` |
| **Redis** | `6379` | `6379` | `localhost:6379` |
| **RabbitMQ** | `5672` | `5672` | `localhost:5672` |
| **RabbitMQ Management** | `15672` | `15672` | `http://localhost:15672` |
| **Elasticsearch** | `9200` | `9200` | `http://localhost:9200` |
| **Kibana** | `5601` | `5601` | `http://localhost:5601` |

## 🔍 Monitoring ve Logging

### Health Checks

Tüm servisler health check ile izlenir:

```bash
# Servis durumlarını kontrol et
docker-compose ps

# Health check loglarını gör
docker-compose logs | grep "health"
```

### Restart Monitoring

Restart policy durumlarını izleme:

```bash
# Restart sayılarını görüntüle
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.RestartCount}}"

# Belirli servisin restart sayısını kontrol et
docker inspect Fitliyo-web --format='{{.RestartCount}}'

# Restart loglarını görüntüle
docker-compose logs Fitliyo-web | grep -i restart
```

### Log Yönetimi

```bash
# Tüm logları görüntüle
docker-compose logs

# Belirli servisin loglarını görüntüle
docker-compose logs Fitliyo-web

# Canlı log takibi
docker-compose logs -f Fitliyo-web

# Son 100 satır log
docker-compose logs --tail=100 Fitliyo-web

# Hata loglarını filtrele
docker-compose logs | grep -i error

# Warning loglarını filtrele
docker-compose logs | grep -i warning
```

### Elasticsearch ve Kibana

- **Elasticsearch**: `http://localhost:9200`
- **Kibana**: `http://localhost:5601`

## 🛠️ Geliştirme Ortamı

### Local Development

Geliştirme sırasında uzak sunucuya bağlanmak için:

```bash
# Environment'ı Development olarak ayarla
set ASPNETCORE_ENVIRONMENT=Development

# Uygulamayı çalıştır
dotnet run --project src/Fitliyo.Web
```

**Development Özellikleri:**
- ✅ **Exception Details**: Hata detayları görünür
- ✅ **Debugging**: Kolay hata ayıklama
- ✅ **Remote Services**: Uzak sunucu servisleri kullanılır
- ✅ **Hot Reload**: Kod değişiklikleri anında yansır

### Docker Development

Docker'da geliştirme yapmak için:

```yaml
# docker-compose.override.yml
version: '3.8'
services:
  Fitliyo-web:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    volumes:
      - ./src:/app/src  # Hot reload için
    restart: on-failure:5  # Development'ta da restart policy
```

### Environment Karşılaştırması

| Özellik | Development | Production |
|---------|-------------|------------|
| **Exception Details** | ✅ Görünür | 🔒 Gizli |
| **Remote Services** | ✅ Uzak sunucu | 🔒 Localhost |
| **Debugging** | ✅ Kolay | 🔒 Kısıtlı |
| **Security** | 🔒 Düşük | ✅ Yüksek |
| **Performance** | 🔒 Orta | ✅ Yüksek |

## 🔧 Troubleshooting

### Yaygın Sorunlar

#### 1. Port Çakışması
```bash
# Kullanılan portları kontrol et
netstat -an | findstr :43332

# Docker'ı durdur ve yeniden başlat
docker-compose down
docker-compose up -d
```

#### 2. Database Bağlantı Sorunu
```bash
# SQL Server container'ının durumunu kontrol et
docker-compose logs sqlserver

# Database'e bağlan
docker exec -it Fitliyo-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FitliyoPassword123!
```

#### 3. Redis Bağlantı Sorunu
```bash
# Redis container'ının durumunu kontrol et
docker-compose logs redis

# Redis CLI'ya bağlan
docker exec -it Fitliyo-redis redis-cli
```

#### 4. RabbitMQ Bağlantı Sorunu
```bash
# RabbitMQ container'ının durumunu kontrol et
docker-compose logs rabbitmq

# Management UI'ya eriş
# http://localhost:15672 (Fitliyo/FitliyoPassword123!)
```

### Log Analizi

```bash
# Hata loglarını filtrele
docker-compose logs | grep -i error

# Warning loglarını filtrele
docker-compose logs | grep -i warning

# Belirli tarihten sonraki loglar
docker-compose logs --since="2024-01-01T00:00:00"

# Restart durumlarını analiz et
docker-compose logs | grep -i "restart\|exit\|failed"

# Exception loglarını filtrele
docker-compose logs | grep -i "exception\|error\|fatal"
```

## 📦 Docker Image Yönetimi

### Image'ları Temizleme

```bash
# Kullanılmayan image'ları sil
docker image prune -a

# Tüm container'ları ve image'ları sil
docker system prune -a
```

### Image'ları Yeniden Build Etme

```bash
# Tüm image'ları yeniden build et
docker-compose build --no-cache

# Belirli servisi yeniden build et
docker-compose build Fitliyo-web
```

## 🔐 Güvenlik

### Environment Variables

Hassas bilgileri environment variable olarak saklayın:

```bash
# .env dosyası oluştur
DB_PASSWORD=your_secure_password
REDIS_PASSWORD=your_redis_password
```

### Docker Secrets (Production)

Production ortamında Docker Secrets kullanın:

```yaml
secrets:
  db_password:
    file: ./secrets/db_password.txt
  redis_password:
    file: ./secrets/redis_password.txt
```

## 📈 Performance Monitoring

### Resource Kullanımı

```bash
# Container resource kullanımını görüntüle
docker stats

# Belirli container'ın resource kullanımı
docker stats Fitliyo-web
```

### Memory ve CPU Limitleri

```yaml
services:
  Fitliyo-web:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '0.5'
        reservations:
          memory: 512M
          cpus: '0.25'
```

## 🚀 Production Deployment

### Production Environment

```bash
# Production environment variable'larını ayarla
export ASPNETCORE_ENVIRONMENT=Production

# Docker Compose ile başlat
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Health Monitoring

```bash
# Health check endpoint'lerini kontrol et
curl http://localhost:43332/health

# Tüm servislerin health durumunu kontrol et
docker-compose ps
```

### Production Güvenlik Özellikleri

- 🔒 **Exception Details**: Hassas bilgiler gizli
- 🔒 **Stack Trace**: Stack trace gizli
- 🔒 **Information Disclosure**: Sistem bilgileri korunur
- 🔒 **Attack Prevention**: Saldırganlar sistem hakkında bilgi alamaz
- ✅ **Restart Policy**: Maksimum 5 kez yeniden başlatma
- ✅ **Health Checks**: Tüm servisler izlenir
- ✅ **Logging**: Kapsamlı loglama

### Performance Monitoring

```bash
# Container resource kullanımını görüntüle
docker stats

# Belirli container'ın resource kullanımı
docker stats Fitliyo-web

# Memory ve CPU kullanımını analiz et
docker stats --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}"
```

Bu rehber ile Fitliyo projesini Docker ortamında güvenli ve verimli bir şekilde çalıştırabilirsiniz! 🎉

---

## 🌐 Domain ve SSL (Natro) ile Nginx Reverse Proxy Kurulumu

Bu bölüm, Natro'dan aldığınız domain/SSL sertifikasını DigitalOcean üzerindeki droplet'te, docker içindeki Nginx reverse proxy ile kullanmanız için adım adım kurulum talimatlarını içerir.

### 1) DNS Ayarları
- Nameserver'lar DigitalOcean'a yönlendirilmiş olmalıdır:
  - `ns1.digitalocean.com`, `ns2.digitalocean.com`, `ns3.digitalocean.com`
- DigitalOcean → Networking → Domains → `demohr.com.tr`:
  - A `@` → `<Droplet IPv4>`
  - A `www` → `<Droplet IPv4>` (veya CNAME `www` → `@`)
- Natro PositiveSSL doğrulaması için CNAME ekleyin (Natro panelindeki “Kayıt Adı” ve “Kayıt Değeri” ile):
  - DO DNS panelinde CNAME oluşturun. Host kısmına yalnızca alt alan adını (örn: `_86cb6…`) girin; target olarak `…comodoca.com` değerini kullanın.

Doğrulama kontrolü:
```bash
dig +short CNAME _xxxxxxxx.demohr.com.tr
```

### 2) CSR ve Özel Anahtar (lokalde oluşturma)
Mac/Linux üzerinde:
```bash
mkdir -p ~/demohr-ssl && cd ~/demohr-ssl
openssl req -new -newkey rsa:2048 -nodes \
  -keyout demohr.com.tr.key \
  -out demohr.com.tr.csr \
  -subj "/C=TR/ST=Istanbul/L=Istanbul/O=DemoHR/OU=IT/CN=demohr.com.tr" \
  -addext "subjectAltName=DNS:demohr.com.tr,DNS:www.demohr.com.tr"
```
Natro SSL panelinde “Apache + OpenSSL” seçip CSR içeriğini yapıştırın. CNAME doğrulaması tamamlanınca sertifika dosyalarını indirin.

### 3) Sertifikayı Nginx için hazırlama
İndirilen dosyalar iki şekilde gelebilir:
- `demohrcomtr_AllCertificate.crt` (tercih edilen, tüm zincir):
  ```bash
  cp ~/demohr-ssl/demohrcomtr_AllCertificate.crt docker/nginx/certs/demohr.com.tr.fullchain.pem
  cp ~/demohr-ssl/demohr.com.tr.key           docker/nginx/certs/demohr.com.tr.key
  chmod 600 docker/nginx/certs/demohr.com.tr.key
  ```
- Sunucu sertifikası + CA bundle ayrı ise:
  ```bash
  cat demohrcomtr.crt CA_bundle.crt > docker/nginx/certs/demohr.com.tr.fullchain.pem
  cp  demohr.com.tr.key                 docker/nginx/certs/demohr.com.tr.key
  chmod 600 docker/nginx/certs/demohr.com.tr.key
  ```

Anahtar eşleşmesini doğrulayın (hash’ler aynı olmalı):
```bash
openssl x509 -noout -modulus -in docker/nginx/certs/demohr.com.tr.fullchain.pem | openssl md5
openssl rsa  -noout -modulus -in docker/nginx/certs/demohr.com.tr.key          | openssl md5
```

### 4) Nginx reverse proxy’yi başlatma
```bash
docker compose up -d reverse-proxy
docker logs --tail=100 Fitliyo-nginx | cat
```
Başarılı durumda 80/443 portları dinliyor olmalı:
```bash
ss -ltnp | grep -E ':80|:443'
curl -I https://demohr.com.tr
```

### 5) Güvenlik duvarı ve doğrulama
- DigitalOcean Firewall’da 80/tcp ve 443/tcp açık olmalı.
- Sunucuda UFW kullanıyorsanız:
```bash
sudo ufw allow 80,443/tcp
```

### 6) Sorun Giderme
- Nginx sürekli yeniden başlıyorsa ve logda `key values mismatch` varsa, kullanılan `.key` dosyası sertifikayla eşleşmiyordur. Doğru KEY’i yerleştirip tekrar başlatın.
- `connection_upgrade` hatası için Nginx konfig başında şu blok bulunur:
```nginx
map $http_upgrade $connection_upgrade { default upgrade; '' close; }
```
- `listen ... http2` uyarısı bilinen bir uyarıdır; işleyişi engellemez.

### 7) Yenileme (Renewal)
Sertifikayı yenilediğinizde yeni `fullchain.pem` ve `.key` dosyalarını aynı yollara kopyalayın ve Nginx’i yeniden başlatın:
```bash
docker compose restart reverse-proxy
```
