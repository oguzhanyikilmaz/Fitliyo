# Login & CORS Kılavuzu

Bu doküman, tarayıcı olmadan (AJAX, Postman) kimlik doğrulama yöntemlerini ve ABP.io kullanan projede CORS konfigürasyonunun nasıl çalıştığını açıklar.

## Projenin Genel Yapısı (Özet)

-   Sunucu: ABP.io tabanlı .NET 9 web uygulaması içinde API ve UI birlikte host edilir (`FitliyoWebModule`). Ayrı bir API servisi gerekmez.
-   API uçları:
    -   Özel auth denetçisi: `/api/account/*` (`AccountController`)
    -   Otomatik üretilen uçlar: `/api/app/...` (Application katmanındaki AppService’lerden conventional controllers)
-   Kimlik doğrulama: OpenIddict (`OpenIddictDataSeedContributor`)
    -   Token endpoint: `/connect/token`
    -   client_id: `Fitliyo_App`
    -   scope: `Fitliyo openid profile`
-   Multi‑tenancy: Tenant bağlamı token/cookie ile taşınır (`ICurrentTenant`). Consultant kullanıcılar için sub‑tenant (şube) bağlamı eklenir (`SubTenantResolverMiddleware`).

## Kimlik Doğrulama Akışları

### ÖNEMLİ: Yeni Multi-Tenant Authentication Flow

Sistem artık **3 adımlı** bir authentication flow kullanıyor:

1. **Tenant Discovery** (Erişilebilir tenant'ları keşfet)
2. **SubTenant Selection** (Şube/alt-tenant seçimi - opsiyonel)
3. **Login with Tenant** (Seçilen tenant ile giriş)

### Akış 1: Cookie + Token Tabanlı Giriş (Önerilen)

Bu akış **hem cookie hem de OAuth2 token** döndürür, böylece her iki yöntemle de API çağrısı yapabilirsiniz.

#### Adım 1: Tenant Discovery

**Endpoint:** `POST /api/account/discover-tenants`

**Request:**

```json
{
    "username": "user@example.com",
    "password": "password123"
}
```

**Response:**

```json
{
    "tenants": [
        {
            "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            "tenantName": "Company A",
            "accessType": "Direct",
            "consultantType": null
        },
        {
            "tenantId": "ffffffff-1111-2222-3333-444444444444",
            "tenantName": "Company B",
            "accessType": "ConsultantAccess",
            "consultantType": "RegisteredConsultant"
        }
    ],
    "requiresSelection": true
}
```

**Önemli Notlar:**

-   Bu endpoint sadece kullanıcının **erişebildiği tenant'ları** listeler
-   `requiresSelection: true` ise kullanıcı bir tenant seçmeli
-   `requiresSelection: false` ise tek tenant var, direkt login yapılabilir

#### Adım 2: SubTenant Selection (Opsiyonel)

**Ne zaman gerekli?**

-   Seçilen tenant'ın **birden fazla şubesi/alt-tenant'ı** varsa
-   Kullanıcı consultant tipindeyse ve belirli şubelere erişim kısıtlaması varsa

**Endpoint:** `POST /api/account/get-subtenants`

**Request:**

```json
{
    "username": "user@example.com",
    "password": "password123",
    "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
}
```

**Response:**

```json
{
    "subTenants": [
        {
            "id": "11111111-2222-3333-4444-555555555555",
            "name": "Istanbul Branch (Merkez)",
            "title": "Istanbul Merkez Şube",
            "registrationNo": "123456",
            "code": "IST-001"
        },
        {
            "id": "66666666-7777-8888-9999-aaaaaaaaaaaa",
            "name": "Ankara Branch (Şube 1)",
            "title": "Ankara 1. Şube",
            "registrationNo": "789012",
            "code": "ANK-001"
        }
    ],
    "requiresSelection": true,
    "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "tenantName": "Company A"
}
```

**Önemli Notlar:**

-   `requiresSelection: true` → Kullanıcı bir SubTenant seçmeli
-   `requiresSelection: false` → Tek SubTenant var veya tümüne erişim var
-   **Consultant kullanıcılar:** Sadece yetkili oldukları SubTenant'lar döner
-   **Normal kullanıcılar:** Tenant'a ait tüm aktif SubTenant'lar döner

#### Adım 3: Login with Tenant

**Endpoint:** `POST /api/account/login-with-tenant`

**Request:**

```json
{
    "username": "user@example.com",
    "password": "password123",
    "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "subTenantId": "11111111-2222-3333-4444-555555555555",
    "rememberMe": true
}
```

**Response:**

```json
{
    "userId": "99999999-8888-7777-6666-555555555555",
    "username": "user@example.com",
    "email": "user@example.com",
    "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "tenantName": "Company A",
    "success": true,
    "accessToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEyMzQ1Njc4OTAiLCJ0eXAiOiJKV1QifQ...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "refreshToken": "CfDJ8KJHGfdshjkHGFDShjkHGFDShjkGHFDShjk..."
}
```

**Önemli Notlar:**

-   **Cookie:** Sunucu otomatik olarak authentication cookie set eder
-   **Token:** Response body'de `accessToken` ve `refreshToken` döner
-   **Kullanım:** Sonraki API çağrılarında **ya cookie ya da Bearer token** kullanabilirsiniz
-   `subTenantId`: **Opsiyonel** - Eğer SubTenant seçimi yapılmadıysa `null` gönderin

#### İstemci Notları:

**AJAX/Fetch (Cookie kullanımı):**

```javascript
await fetch("https://demohr.com.tr/api/account/login-with-tenant", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include", // ÖNEMLİ: Cookie için gerekli
    body: JSON.stringify({
        /* ... */
    }),
});
```

**AJAX/Fetch (Token kullanımı):**

```javascript
const loginResp = await fetch(
    "https://demohr.com.tr/api/account/login-with-tenant",
    {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            /* ... */
        }),
    }
);
const { accessToken } = await loginResp.json();

// Sonraki API çağrılarında:
await fetch("https://demohr.com.tr/api/app/assets", {
    headers: { Authorization: `Bearer ${accessToken}` },
});
```

**Postman:**

-   Cookie kullanımı: "Send cookies" enabled olmalı
-   Token kullanımı: Authorization tab → Type: Bearer Token

### Akış 2: Sadece Token Tabanlı Giriş (Legacy - Önerilmez)

**Not:** Bu akış artık önerilmiyor. Yukarıdaki `login-with-tenant` endpoint'i hem cookie hem token döndürdüğü için daha kullanışlıdır.

**Endpoint:** `POST /connect/token`

**Request:**

```
Content-Type: application/x-www-form-urlencoded
Abp-TenantId: aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee

grant_type=password&username=user@example.com&password=password123&client_id=Fitliyo_App&scope=Fitliyo openid profile
```

**Response:**

```json
{
    "access_token": "eyJhbGc...",
    "token_type": "Bearer",
    "expires_in": 3600,
    "refresh_token": "CfDJ8..."
}
```

**Dezavantajları:**

-   Multi-step tenant/subtenant discovery akışını desteklemiyor
-   Tenant ID'yi manuel olarak bilmeniz gerekiyor
-   SubTenant context'i desteklemiyor

## CORS Konfigürasyonu

CORS, Fitliyo.Web içinde development ve production ortamlarında cross-origin istekleri desteklemek için etkinleştirilip yapılandırılmıştır.

-   appsettings key: App:CorsOrigins
    -   Örnek (development):
        -   https://localhost:44332 (self)
        -   http://localhost:44332 and https://localhost:44332 (typical SPA dev)
        -   https://demohr.com.tr (production host)
-   Web modül konfigürasyonu:
    -   Varsayılan CORS politikası ConfigureServices içinde kaydedilir.
    -   Middleware hattında CORS, app.UseCors() ile etkinleştirilir.

### Allowed origin ekleme/güncelleme

src/Fitliyo.Web/appsettings.Development.json dosyasını düzenleyin (gerekirse Production için de):

-   App.CorsOrigins: Virgülle ayrılmış liste, örn.
    -   "https://localhost:44332,http://localhost:44332,https://demohr.com.tr"
-   Sayfanız https ise, tamamen nitelikli https URL'leri kullanın; aksi halde mixed-content hataları oluşur.

### Notlar ve En İyi Uygulamalar

-   Cookie‑tabanlı akışta, tarayıcı fetch isteklerinde credentials: 'include' gereklidir.
-   Token‑tabanlı akış, tarayıcı dışı istemciler için idealdir; tenant bağlamını seçmek için Abp‑TenantId veya \_\_tenant değerini iletmeyi unutmayın.
-   Konsolda URL şeması hataları (ör. "chrome") görürseniz, https:// ile başlayan tam URL kullandığınızdan ve kodu new-tab sayfasından değil, sitenizden çalıştırdığınızdan emin olun.
-   Cross-origin isteklerde, origin'inizin App:CorsOrigins içinde olduğundan ve http/https karıştırmadığınızdan emin olun.

## İlgili Kod Sembolleri

-   `AccountController`: tenant keşfi ve login endpointleri
-   `DiscoverTenantsAsync`: POST /api/account/discover-tenants
-   `LoginWithTenantAsync`: POST /api/account/login-with-tenant
-   `UserTenantResolverService`: iş kuralları (şifre kontrolü, aktif kullanıcı, danışman tip denetimleri)

### JavaScript fetch örnekleri

-   Yanlış (protokol yok):

```
// Hatalı: 'demohr.com.tr/...' şemasız olduğu için
// tarayıcı bunu chrome://new-tab-page/ tabanına göre çözer
// ve URL scheme "chrome" desteklenmedi hatası üretir.
fetch('demohr.com.tr/api/account/login', { /* ... */ });
```

-   Doğru örnekler

1. Tenant keşfi (cookie akışı başlangıcı)

```
await fetch('https://demohr.com.tr/api/account/discover-tenants', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username: 'admin', password: 'admin' })
});
```

2. Seçilen tenant ile giriş (cookie + token)

```
await fetch('https://demohr.com.tr/api/account/login-with-tenant', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include',
  body: JSON.stringify({
    username: 'admin',
    password: 'admin',
    tenantId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
    subTenantId: null,
    rememberMe: true
  })
});
```

3. Giriş sonrası korumalı endpoint (cookie ile)

```
await fetch('https://demohr.com.tr/api/asset-management/assets', {
  credentials: 'include'
});
```

4. Token alma (OpenIddict OAuth2 - Resource Owner Password)

```
const body = new URLSearchParams({
  grant_type: 'password',
  username: 'admin',
  password: 'admin',
  client_id: 'Fitliyo_App', // OpenIddict:Applications:Fitliyo_App:ClientId (appsettings.json / appsettings.Production.json)
  scope: 'Fitliyo openid profile' // 'Fitliyo' scope OpenIddictDataSeedContributor'da seed edilir; standart 'openid','profile' desteklenir
});

const tokenResp = await fetch('https://demohr.com.tr/connect/token', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded',
    'Abp-TenantId': 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee' // veya ?__tenant=<name>
  },
  body
});
const tokenJson = await tokenResp.json();
const accessToken = tokenJson.access_token;
```

#### client_id ve scope nereden alınır?

-   client_id: Fitliyo.Web uygulama ayarlarında (appsettings.json veya appsettings.Production.json) OpenIddict:Applications:Fitliyo_App:ClientId anahtarında tanımlıdır. Gerçek değer: Fitliyo_App.
-   scope: Gerçek değer: Fitliyo openid profile. 'Fitliyo' API scope'u [OpenIddictDataSeedContributor] içinde CreateScopesAsync metodunda seed edilir; 'openid' ve 'profile' standard OpenID kapsamlarıdır. İsteğe bağlı olarak 'email', 'phone', 'roles', 'address' kapsamları da seed edilmiştir ve ihtiyaç halinde eklenebilir.

5. Token ile korumalı endpoint

```
await fetch('https://demohr.com.tr/api/asset-management/assets', {
  headers: { Authorization: `Bearer ${accessToken}` }
});
```

Notlar:

-   Kodunuzu sitenizin sayfasında çalıştırın; new-tab konsolundan çalıştırmak URL’i chrome:// şemasına göre çözer.
-   Sayfanız https ise API çağrılarınız da https olmalı (mixed-content engellenir).

Belirli istemci origin'lerini (ör. https://localhost:3000 veya bir staging alanı) eklememi isterseniz, adresleri paylaşın; App:CorsOrigins değerini buna göre güncellerim.

### Hızlı fetch örnekleri (IIFE stilinde)

#### Tam Akış Örneği (3 Adım)

```javascript
// ADIM 1: Tenant Discovery
(async () => {
    const discoverResp = await fetch(
        "https://demohr.com.tr/api/account/discover-tenants",
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: "admin",
                password: "Admin@2025",
            }),
        }
    );
    const { tenants, requiresSelection } = await discoverResp.json();
    console.log("Available Tenants:", tenants);

    if (!requiresSelection && tenants.length === 1) {
        console.log("Tek tenant var, direkt login yapılabilir");
    }

    // Kullanıcı bir tenant seçer (örnek: ilk tenant)
    const selectedTenant = tenants[0];
    console.log("Selected Tenant:", selectedTenant.tenantName);
})();
```

```javascript
// ADIM 2: SubTenant Selection (Opsiyonel)
(async () => {
    const subTenantsResp = await fetch(
        "https://demohr.com.tr/api/account/get-subtenants",
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username: "admin",
                password: "Admin@2025",
                tenantId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            }),
        }
    );
    const { subTenants, requiresSelection } = await subTenantsResp.json();
    console.log("Available SubTenants:", subTenants);

    // Kullanıcı bir SubTenant seçer (örnek: ilk SubTenant)
    const selectedSubTenant = subTenants[0];
    console.log("Selected SubTenant:", selectedSubTenant.name);
})();
```

```javascript
// ADIM 3: Login with Tenant (Cookie + Token)
(async () => {
    const loginResp = await fetch(
        "https://demohr.com.tr/api/account/login-with-tenant",
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include", // Cookie kullanımı için
            body: JSON.stringify({
                username: "admin",
                password: "Admin@2025",
                tenantId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
                subTenantId: "11111111-2222-3333-4444-555555555555", // Opsiyonel
                rememberMe: true,
            }),
        }
    );
    const loginData = await loginResp.json();
    console.log("Login Success:", loginData);
    console.log("Access Token:", loginData.accessToken);
    console.log("Expires In:", loginData.expiresIn, "seconds");
})();
```

#### Kısa Yol: Direkt Login (Tenant ve SubTenant biliyorsanız)

```javascript
(async () => {
    const loginResp = await fetch(
        "https://demohr.com.tr/api/account/login-with-tenant",
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({
                username: "admin",
                password: "Admin@2025",
                tenantId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
                subTenantId: "11111111-2222-3333-4444-555555555555",
                rememberMe: true,
            }),
        }
    );
    const { accessToken } = await loginResp.json();

    // Token ile API çağrısı
    const assetsResp = await fetch(
        "https://demohr.com.tr/api/app/asset?skipCount=0&maxResultCount=10",
        {
            headers: { Authorization: `Bearer ${accessToken}` },
        }
    );
    const assets = await assetsResp.json();
    console.log("Assets:", assets);
})();
```

#### Cookie ile API Çağrısı (Token olmadan)

```javascript
(async () => {
    // Önce login (credentials: 'include' ile)
    await fetch("https://demohr.com.tr/api/account/login-with-tenant", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
            username: "admin",
            password: "Admin@2025",
            tenantId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            rememberMe: true,
        }),
    });

    // Sonraki çağrılar (token gerekmez, cookie yeterli)
    const assetsResp = await fetch(
        "https://demohr.com.tr/api/app/asset?skipCount=0&maxResultCount=10",
        {
            credentials: "include", // Cookie gönderir
        }
    );
    const assets = await assetsResp.json();
    console.log("Assets:", assets);
})();
```

**Önemli Notlar:**

1. **Endpoint değişti:** Artık `/api/account/login-with-tenant` kullanılıyor (eski `/api/account/login` değil)
2. **3 Endpoint var:**
    - `/api/account/discover-tenants` → Tenant listesi
    - `/api/account/get-subtenants` → SubTenant listesi (opsiyonel)
    - `/api/account/login-with-tenant` → Login (cookie + token)
3. **Response içeriği:** `accessToken`, `tokenType`, `expiresIn`, `refreshToken`, `userId`, `tenantId`, `tenantName`
4. **Cookie + Token:** Hem cookie hem token döner, istediğinizi kullanabilirsiniz

## API Kullanım Kılavuzu (Genel)

### Multi‑Tenancy kullanım kuralları

-   Token alma (ROPC) sırasında tenant bağlamı ZORUNLU: `Abp-TenantId: <tenant-guid>` header’ını ekleyin (alternatif: `?__tenant=<tenant-name>` query string). Bu bağlam, üretilen access token içine taşınır.
-   Bearer ile yapılan SONRAKİ çağrılarda genellikle `Abp-TenantId` eklemek GEREKMEZ; backend, token’dan tenant’ı çözer. Yalnızca tenant değiştirmek istediğinizde yeniden token alın veya `Abp-TenantId` ile geçici bağlam sağlayın.
-   Cookie akışında tenant, `login-with-tenant` çağrısındaki `tenantId` ile bağlanır; sonraki cookie’li isteklerde header gerekmez. Tenant değiştirmek için yeniden login yapmanız gerekir.
-   Consultant kullanıcılar için sub‑tenant (şube) seçimi akışı geçerlidir: girişten sonra sub‑tenant seçimi veya `LoginWithTenantRequest.SubTenantId` ile bağlam set edilebilir; backend ilgili claim ve cache mekanizmalarıyla işleme alır.

### Temel İlkeler

-   Base URL: https://demohr.com.tr (Development için: https://localhost:44332)
-   Header’lar:
    -   Content-Type: application/json (JSON isteklerde)
    -   Authorization: Bearer <access_token> (token akışı)
    -   Abp-TenantId: <tenant-guid> veya `__tenant=<tenant-name>` (multi‑tenancy bağlamı; token üretiminde zorunlu, sonraki çağrılarda opsiyonel)
-   Hata yönetimi: 400 (BadRequest), 401 (Unauthorized), 403 (Forbidden), 404 (NotFound), 409 (Conflict), 500 (ServerError)
-   Tarih/Sayı formatı: ISO 8601 tarih (UTC), ondalıklar için nokta.

### Ortak CRUD ve listeleme paterni

-   ABP Conventional Controllers ile Application katmanındaki AppService’ler otomatik olarak `/api/app/...` altında yayınlanır. Detaylı ve hatasız endpoint isimleri için Swagger’ı kullanın: https://demohr.com.tr/swagger
-   Listeleme parametreleri (yaygın): `skipCount`, `maxResultCount`, `sorting`
-   Örnek (token ile):

```
GET https://demohr.com.tr/api/app/asset?skipCount=0&maxResultCount=10&sorting=name asc
Authorization: Bearer <access_token>
```

### cURL Örnekleri

#### Adım 1: Discover Tenants

```bash
curl -X POST "https://demohr.com.tr/api/account/discover-tenants" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@2025"}'
```

**Response:**

```json
{
    "tenants": [
        {
            "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            "tenantName": "Company A",
            "accessType": "Direct"
        }
    ],
    "requiresSelection": true
}
```

#### Adım 2: Get SubTenants (Opsiyonel)

```bash
curl -X POST "https://demohr.com.tr/api/account/get-subtenants" \
  -H "Content-Type: application/json" \
  -d '{
    "username":"admin",
    "password":"Admin@2025",
    "tenantId":"aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
  }'
```

**Response:**

```json
{
    "subTenants": [
        {
            "id": "11111111-2222-3333-4444-555555555555",
            "name": "Istanbul Branch",
            "code": "IST-001"
        }
    ],
    "requiresSelection": true,
    "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "tenantName": "Company A"
}
```

#### Adım 3: Login with Tenant (Cookie + Token)

```bash
curl -X POST "https://demohr.com.tr/api/account/login-with-tenant" \
  -H "Content-Type: application/json" \
  -c cookies.txt -b cookies.txt \
  -d '{
    "username":"admin",
    "password":"Admin@2025",
    "tenantId":"aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "subTenantId":"11111111-2222-3333-4444-555555555555",
    "rememberMe":true
  }'
```

**Response:**

```json
{
    "userId": "99999999-8888-7777-6666-555555555555",
    "username": "admin",
    "tenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "tenantName": "Company A",
    "success": true,
    "accessToken": "eyJhbGc...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "refreshToken": "CfDJ8..."
}
```

#### Cookie ile Protected Endpoint Çağrısı

```bash
curl -X GET "https://demohr.com.tr/api/app/asset?skipCount=0&maxResultCount=10" \
  -b cookies.txt
```

#### Token ile Protected Endpoint Çağrısı

```bash
# Önce token al
TOKEN=$(curl -s -X POST "https://demohr.com.tr/api/account/login-with-tenant" \
  -H "Content-Type: application/json" \
  -d '{
    "username":"admin",
    "password":"Admin@2025",
    "tenantId":"aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "rememberMe":true
  }' | jq -r '.accessToken')

# Token ile API çağrısı
curl -X GET "https://demohr.com.tr/api/app/asset?skipCount=0&maxResultCount=10" \
  -H "Authorization: Bearer $TOKEN"
```

#### Legacy Token Alma (Önerilmez)

```bash
curl -X POST "https://demohr.com.tr/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -H "Abp-TenantId: aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee" \
  -d "grant_type=password&username=admin&password=Admin@2025&client_id=Fitliyo_App&scope=Fitliyo%20openid%20profile"
```

### Axios örneği (token ile)

```
import axios from 'axios';
const client = axios.create({ baseURL: 'https://demohr.com.tr' });
const tokenResp = await client.post('/connect/token', new URLSearchParams({
  grant_type: 'password', username: 'admin', password: 'Admin@2025', client_id: 'Fitliyo_App', scope: 'Fitliyo openid profile'
}), { headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'Abp-TenantId': 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee' } });
const accessToken = tokenResp.data.access_token;
const data = (await client.get('/api/app/asset?skipCount=0&maxResultCount=10', { headers: { Authorization: `Bearer ${accessToken}` } })).data;
```

### .NET HttpClient örneği (token ile)

```
using var http = new HttpClient { BaseAddress = new Uri("https://demohr.com.tr") };
var form = new FormUrlEncodedContent(new[]{
  new("grant_type","password"), new("username","admin"), new("password","Admin@2025"), new("client_id","Fitliyo_App"), new("scope","Fitliyo openid profile")
});
form.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
var tokenReq = new HttpRequestMessage(HttpMethod.Post, "/connect/token") { Content = form };
tokenReq.Headers.Add("Abp-TenantId", "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
var token = await (await http.SendAsync(tokenReq)).Content.ReadFromJsonAsync<dynamic>();
http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", (string)token.access_token);
var resp = await http.GetAsync("/api/app/asset?skipCount=0&maxResultCount=10");
var json = await resp.Content.ReadAsStringAsync();
```

### Python (requests) örneği

```
import requests
base = 'https://demohr.com.tr'
headers = { 'Content-Type': 'application/x-www-form-urlencoded', 'Abp-TenantId': 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee' }
payload = { 'grant_type':'password', 'username':'admin', 'password':'Admin@2025', 'client_id':'Fitliyo_App', 'scope':'Fitliyo openid profile' }
tr = requests.post(f'{base}/connect/token', headers=headers, data=payload)
access = tr.json()['access_token']
res = requests.get(f'{base}/api/app/asset?skipCount=0&maxResultCount=10', headers={ 'Authorization': f'Bearer {access}' })
print(res.json())
```

### Dosya yükleme (multipart/form-data)

```
const fd = new FormData();
fd.append('file', fileInput.files[0]);
await fetch('https://demohr.com.tr/api/app/files/upload', {
  method: 'POST',
  headers: { Authorization: `Bearer ${accessToken}` },
  body: fd
});
```

Not: Gerçek upload endpoint adını Swagger’dan doğrulayın.

### Fetch ile genel API çağrıları

1. Listeleme (GET)

```
(async () => {
  const resp = await fetch('https://demohr.com.tr/api/app/asset?skipCount=0&maxResultCount=10&sorting=name%20asc', {
    headers: { Authorization: `Bearer ${accessToken}` }
  });
  const json = await resp.json();
  console.log(json);
})();
```

2. Detay (GET by id)

```
(async () => {
  const resp = await fetch('https://demohr.com.tr/api/app/asset/00000000-0000-0000-0000-000000000001', {
    headers: { Authorization: `Bearer ${accessToken}` }
  });
  const json = await resp.json();
  console.log(json);
})();
```

3. Oluşturma (POST)

```
(async () => {
  const resp = await fetch('https://demohr.com.tr/api/app/asset', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${accessToken}` },
    body: JSON.stringify({ serialNumber: 'ABC123', brand: 'HP', model: 'EliteBook', categoryId: '...', statusId: '...', purchaseDate: '2024-01-01', purchasePrice: 5000 })
  });
  const json = await resp.json();
  console.log(json);
})();
```

4. Güncelleme (PUT)

```
(async () => {
  const resp = await fetch('https://demohr.com.tr/api/app/asset/00000000-0000-0000-0000-000000000001', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${accessToken}` },
    body: JSON.stringify({ serialNumber: 'ABC123', brand: 'HP', model: 'EliteBook Pro', categoryId: '...', statusId: '...', purchaseDate: '2024-01-01', purchasePrice: 6000 })
  });
  const json = await resp.json();
  console.log(json);
})();
```

5. Silme (DELETE)

```
(async () => {
  const resp = await fetch('https://demohr.com.tr/api/app/asset/00000000-0000-0000-0000-000000000001', {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${accessToken}` }
  });
  console.log(resp.status); // 204 beklenir
})();
```

### Postman kurulum ipuçları

-   Token akışı için: Method POST /connect/token, Body -> x-www-form-urlencoded, Header -> Abp-TenantId.
-   Cookie akışı için: Login‑with‑tenant çağrısında Cookies’i etkinleştirin (Cookie Jar). Sonraki isteklerde cookie otomatik gönderilir.

### Sık hatalar ve çözümler

-   400 BadRequest: CSRF -> [AccountController] üzerinde `IgnoreAntiforgeryToken` var; token akışında gerekmez.
-   Mixed Content: Sayfa https ise API’yi de https kullanın.
-   CORS: Origin’iniz `App:CorsOrigins` içinde değilse 403/blocked. [FitliyoWebModule] içinde CORS aktif, appsettings’ten yönetilir.

### İlgili Semboller

-   `AccountController` (auth akışları): /api/account/\*
-   `OpenIddictDataSeedContributor` (client_id ve scope tanımı)
-   `FitliyoWebModule` (CORS ve Anti‑forgery konfigürasyonları)

3. Token alma (OpenIddict OAuth2 - Resource Owner Password)

```
(async () => {
  const body = new URLSearchParams({
    grant_type: 'password',
    username: 'admin',
    password: 'Admin@2025',
    client_id: 'Fitliyo_App', // OpenIddict:Applications:Fitliyo_App:ClientId
    scope: 'Fitliyo openid profile'
  });

  const tokenResp = await fetch('https://demohr.com.tr/connect/token', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
      'Abp-TenantId': 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee' // veya ?__tenant=<name>
    },
    body
  });
  const tokenJson = await tokenResp.json();
  console.log(tokenJson);
})();
```

4. Giriş sonrası korumalı endpoint (cookie ile)

```
(async () => {
  const resp = await fetch('https://demohr.com.tr/api/asset-management/assets', {
    credentials: 'include'
  });
  const json = await resp.json();
  console.log(json);
})();
```

5. Token ile korumalı endpoint

```
(async () => {
  const accessToken = '<access_token>';
  const resp = await fetch('https://demohr.com.tr/api/asset-management/assets', {
    headers: { Authorization: `Bearer ${accessToken}` }
  });
  const json = await resp.json();
  console.log(json);
})();
```
