# Fitliyo Web Frontend

Rol bazlı (Öğrenci / Eğitmen / Admin) marketplace arayüzü. Next.js 14, TypeScript, Tailwind CSS.

## API sözleşmesi (zorunlu)

- Tek kaynak: backend repo `docs/openapi/swagger.web.v1.full.json`
- Yeni endpoint veya DTO: önce Swagger, sonra `lib/api-paths.ts` + `lib/types.ts` + ekran.
- Geliştirme kuralları: [`.cursor/rules/frontend-fitliyo.mdc`](../.cursor/rules/frontend-fitliyo.mdc)

## Gereksinimler

- Node.js 20+
- Backend (Fitliyo.Web) çalışır; CORS’ta `http://localhost:3000` (veya kendi origin’iniz) tanımlı olmalı

## Kurulum

```bash
cd frontend
cp .env.example .env.local
# NEXT_PUBLIC_API_URL = backend kökü (örn. http://localhost:5001)
# NEXT_PUBLIC_OAUTH_CLIENT_ID = OpenIddict public client (genelde Fitliyo_App)
npm install
npm run dev
```

[http://localhost:3000](http://localhost:3000)

## Rotalar (özet)

- **Giriş / kayıt:** `/login`, `/register`
- **Öğrenci:** `/student`, `/student/profile`, siparişler, seanslar, …
- **Eğitmen:** `/trainer`, …
- **Admin:** `/admin`, …

Giriş sonrası yönlendirme JWT rol claim’lerine göre (Student → `/student`, Trainer → `/trainer`, Admin → `/admin`).

## Plan

[docs/FRONTEND_PLANI.md](../docs/FRONTEND_PLANI.md) (varsa)
