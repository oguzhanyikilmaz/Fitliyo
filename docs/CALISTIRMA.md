# Fitliyo Projesini Çalıştırma ve Kontrol

Bu dokümanda projeyi yerel ortamda nasıl çalıştıracağınız ve kontrol edeceğiniz adım adım anlatılır.

---

## Ön koşullar

- **.NET 9 SDK** yüklü olmalı
- **Node.js 20+** (frontend için)
- **PostgreSQL 16** çalışır durumda
- **Redis** çalışır durumda

Bağlantı bilgileri: `src/Fitliyo.Web/appsettings.json` ve `src/Fitliyo.DbMigrator/appsettings.json` içindeki `ConnectionStrings:Default`.  
**Yerel geliştirme:** DbMigrator’ı yerel PostgreSQL ile kullanmak için `ASPNETCORE_ENVIRONMENT=Development` ile çalıştırın; `appsettings.Development.json` içinde `Host=localhost` ve kendi şifrenizi tanımlayın.

---

## 1. Veritabanı ve ilk kurulum

İlk kez çalıştırıyorsanız veya yeni migration varsa:

1. **DbMigrator çalıştırın** (migration + seed):
   - VS Code: **Run and Debug** → **Fitliyo DbMigrator** seçip F5 veya yeşil play
   - Veya Terminal: `dotnet run --project src/Fitliyo.DbMigrator`
   - Bu işlem bekleyen tüm migration'ları (örn. **AppUserProfiles** tablosu) uygular.

   **AppUserProfiles tablosu yoksa:** DbMigrator'ı bir kez çalıştırmanız yeterlidir. `20260302000000_AddUserProfiles` migration'ı tabloyu oluşturur. Alternatif: `dotnet ef database update --project src/Fitliyo.EntityFrameworkCore --startup-project src/Fitliyo.DbMigrator`

---

## 2. Backend (API) çalıştırma

### VS Code ile

1. **Run and Debug** (Ctrl+Shift+D / Cmd+Shift+D) açın.
2. Üstten **Fitliyo Web (Debug)** seçin.
3. **F5** veya yeşil **Play** ile başlatın.
4. Backend hazır olduğunda Swagger otomatik açılır.
   - **Backend (HTTP):** http://localhost:5000  
   - **Swagger:** http://localhost:5000/swagger

### Terminal ile

```bash
dotnet run --project src/Fitliyo.Web
```

---

## 3. Frontend (Next.js) çalıştırma

Frontend’in backend API’ye istek atabilmesi için **önce backend’in çalışıyor olması** gerekir.

### İlk kez (bağımlılıklar)

```bash
cd frontend
cp .env.example .env.local
# .env.local içinde backend adresini ayarlayın (VS Code launch için aşağıdaki gibi)
npm install
```

**`.env.local` örneği (backend varsayılan olarak 5000 portunda):**

```env
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_OAUTH_CLIENT_ID=Fitliyo_App
```

### VS Code ile

**Seçenek A – Compound (Backend + Frontend birlikte)**  
1. **Run and Debug** → **Backend + Frontend (Web + Next.js)** seçin.  
2. **F5** ile başlatın (önce `build-web` çalışır, sonra backend ve frontend birlikte açılır).  
3. İki terminal açılır: biri backend (Fitliyo Web), biri frontend (Next.js).  
4. **Backend’in ayağa kalkmasını bekleyin:** Backend terminalinde **"Now listening on: http://localhost:5000"** görünene kadar giriş yapmayın. Swagger otomatik açılırsa backend hazır demektir.  
5. Frontend: **http://localhost:3000**

**Seçenek B – Ayrı ayrı**  
1. Önce **Fitliyo Web (Debug)** ile backend’i başlatın (F5).  
2. Sonra **Run and Debug** → **Frontend (Next.js)** ile frontend’i başlatın  
   veya **Terminal → Run Task** → **Frontend: npm run dev**.

### Terminal ile

```bash
cd frontend
npm run dev
```

Tarayıcıda: **http://localhost:3000**

---

## 4. Kontrol listesi

| Adım | Ne yapılır | Nasıl kontrol |
|------|------------|----------------|
| 1 | DbMigrator çalıştı | Log’da hata yok, DB’de tablolar var |
| 2 | Backend açıldı | http://localhost:5000/swagger açılıyor |
| 3 | Frontend açıldı | http://localhost:3000 ana sayfa görünüyor |
| 4 | Giriş denemesi | Giriş sayfası → kullanıcı adı/şifre → rolüne göre /ogrenci, /egitmen veya /admin’e yönlenme |

**Seed kullanıcılar (DbMigrator sonrası):** Aşağıdaki hesaplarla giriş yapabilirsiniz (şifre: `Test123!`).

| Rol     | Kullanıcı adı | E-posta              |
|--------|----------------|-----------------------|
| Admin  | admin          | admin@fitliyo.com     |
| Eğitmen| egitmen        | egitmen@fitliyo.com   |
| Öğrenci| ogrenci        | ogrenci@fitliyo.com   |

**Not:** Backend’de CORS ayarında frontend origin’i (`http://localhost:3000`) tanımlı olmalı; aksi halde tarayıcı API isteklerini engelleyebilir.

---

## 5. VS Code Launch ve Task özeti

### Launch (Run and Debug)

| Konfigürasyon | Açıklama |
|----------------|----------|
| **Fitliyo Web (Debug)** | Sadece backend (API) |
| **Frontend (Next.js)** | Sadece frontend (npm run dev) |
| **Backend + Frontend (Web + Next.js)** | İkisini birlikte başlatır |
| **Fitliyo DbMigrator** | Migration + seed |
| **Fitliyo Tests** | Uygulama testleri |

### Tasks (Terminal → Run Task)

| Task | Açıklama |
|------|----------|
| **build** / **build-web** | Backend projesini derler |
| **Frontend: npm install** | frontend bağımlılıklarını yükler |
| **Frontend: npm run dev** | Frontend dev sunucusunu başlatır |
| **Fitliyo: Run Web Application** | Backend’i `dotnet run` ile çalıştırır |
| **Fitliyo: Run DbMigrator** | DbMigrator’ı çalıştırır |

---

## 6. Sık karşılaşılan durumlar

- **Backend port:** Varsayılan `http://localhost:5000`. `launch.json` içinde `ASPNETCORE_URLS` ve frontend’teki `NEXT_PUBLIC_API_URL` (.env.local) aynı porta göre olmalı.
- **CORS hatası:** Backend’de `Fitliyo.Web` CORS policy’sine `http://localhost:3000` (ve kullandığınız origin) eklenmeli.
- **Giriş 401 / token alamıyorum:** OpenIddict client (örn. `Fitliyo_App`) Password grant’e izin veriyor olmalı; `NEXT_PUBLIC_OAUTH_CLIENT_ID` ile aynı client id kullanılmalı.

Detaylı dokümantasyon: [docs/README.md](README.md), [docs/FRONTEND_PLANI.md](FRONTEND_PLANI.md).

---

## 7. Backend ayağa kalkmıyorsa / Swagger açılmıyorsa

- **Önce derleyin:** Run and Debug ile **Fitliyo Web (Debug)** çalıştırırken `preLaunchTask: build-web` otomatik çalışır. Hata alıyorsanız **Terminal → Run Task → build** veya `dotnet build src/Fitliyo.Web/Fitliyo.Web.csproj` ile derleyin.
- **Port 5000 kullanımda mı?** Başka bir uygulama 5000 portunu kullanıyorsa backend başlamaz. `lsof -i :5000` ile kontrol edin veya `launch.json` içinde `ASPNETCORE_URLS` ile farklı bir port (örn. `http://localhost:5001`) deneyin.
- **Swagger adresi:** Backend çalıştıktan sonra tarayıcıda **http://localhost:5000/swagger** açın. Otomatik açılmadıysa bu adresi manuel girin.
- **Veritabanı / Redis:** Backend açılırken PostgreSQL veya Redis’e bağlanamıyorsa uygulama çökebilir. Development ortamında `appsettings.Development.json` içinde `ConnectionStrings:Default` ile yerel PostgreSQL kullanın; Redis yoksa geçici olarak devre dışı bırakılabilir (projeye göre).

---

## 7.0 Swagger açılırken "Connection refused" / NpgsqlException (PostgreSQL)

**Belirti:** Swagger açılırken `SocketException: Connection refused`, `NpgsqlException: Failed to connect to 127.0.0.1:5432` hatası.

**Neden:** Backend (Development ortamında) `appsettings.Development.json` içindeki connection string ile PostgreSQL'e bağlanıyor. Adres `localhost:5432` ise ve bilgisayarınızda PostgreSQL çalışmıyorsa bağlantı reddedilir.

**Yapılan varsayılan:** Development için `appsettings.Development.json` artık **uzak sunucu** (161.35.71.138:5433) kullanacak şekilde ayarlandı. Bu sunucuya ağ erişiminiz varsa Swagger normal açılır.

**Yerel PostgreSQL kullanmak isterseniz:** PostgreSQL'i kurup 5432'de çalıştırın; `src/Fitliyo.Web/appsettings.Development.json` içinde `Host=localhost;Port=5432;...` ve kendi şifrenizi yazın. Önce DbMigrator'ı Development ortamında çalıştırın.

---

## 7.1 Backend erişilebilir mi? Nasıl test edilir?

1. **Tarayıcı:** http://localhost:5000/swagger açın. Sayfa açılıyorsa backend ayakta ve erişilebilir.
2. **Terminal (curl):**
   ```bash
   curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/swagger/index.html
   ```
   Çıktı `200` ise backend yanıt veriyor.
3. **Token endpoint (curl):**
   ```bash
   curl -X POST http://localhost:5000/connect/token \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "grant_type=password&username=admin&password=Test123!&client_id=Fitliyo_App"
   ```
   Başarılı ise JSON içinde `access_token` döner. 403 alıyorsanız büyük ihtimalle **CORS** (frontend origin izinli değil). Backend’de `App:CorsOrigins` içinde frontend adresi (örn. `http://localhost:3000`) tanımlı olmalı; `FitliyoWebModule` içinde CORS açık olmalı.

---

## 8. 403 Forbidden — POST /connect/token

**Belirti:** Tarayıcıda frontend (örn. localhost:3000) ile giriş yaparken `POST http://localhost:5000/connect/token` **403 Forbidden** dönüyor.

**Olası nedenler:**

1. **CORS:** Backend, frontend origin’ine (http://localhost:3000) izin vermiyor. Tarayıcı cross-origin isteği engeller veya sunucu 403 döner.
   - **Çözüm:** `src/Fitliyo.Web/appsettings.Development.json` içinde `App:CorsOrigins` tanımlı olmalı; `http://localhost:3000` eklenmiş olmalı. `FitliyoWebModule` içinde `ConfigureCors` ve `app.UseCors()` kullanılıyor olmalı.
2. **Backend kapalı:** Backend (Fitliyo.Web) çalışmıyorsa istek localhost:5000’e gidemez; genelde "Connection refused" veya "Failed to fetch" görülür. 403 alıyorsanız istek sunucuya ulaşıyor demektir.
3. **OpenIddict / client_id:** `client_id=Fitliyo_App` ve Password grant’in açık olduğundan emin olun (DbMigrator seed ve appsettings’teki OpenIddict uygulaması).

**Hızlı kontrol:** Aynı isteği **Swagger** veya **curl** ile (Origin header olmadan) yapın. 200 ve token dönüyorsa sorun büyük ihtimalle CORS’tur; CORS’u yukarıdaki gibi ekleyin/güncelleyin.

---

## 9. "Connection refused" (DbMigrator / PostgreSQL)

**Hata:** `Failed to connect to ... Connection refused`

**Neden:** DbMigrator, `appsettings.json` içindeki `ConnectionStrings:Default` ile verilen sunucu:port’a (örn. 161.35.71.138:5432) bağlanamıyor. Bağlantı reddedildiği için:

- Uzak sunucuda PostgreSQL çalışmıyor olabilir,
- Firewall / security group 5432 portunu dışarıya açmıyor olabilir,
- Bilgisayarınız o sunucuya ağ üzerinden erişemiyor olabilir.

**Yerel PostgreSQL ile çalıştırmak için:**

1. PostgreSQL’i bilgisayarınızda kurup çalıştırın (port 5432).
2. DbMigrator’da `appsettings.Development.json` kullanılsın diye ortamı **Development** yapın ve bu dosyada connection string’i **localhost** yapın:
   - `src/Fitliyo.DbMigrator/appsettings.Development.json` içinde `Host=localhost`, `Password=` kısmını kendi postgres şifrenizle doldurun.
3. DbMigrator’ı Development ortamında çalıştırın:
   - Terminal: `ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/Fitliyo.DbMigrator`
   - VS Code: Launch’ta “Fitliyo DbMigrator” için `env` içine `"ASPNETCORE_ENVIRONMENT": "Development"` ekleyin (zaten varsa değiştirmeyin).

Böylece DbMigrator yerel PostgreSQL’e bağlanır; uzak sunucu (161.35.71.138) kullanmak istediğinizde `appsettings.json` (ve ortamı Production veya boş) ile çalıştırmanız yeterli.
