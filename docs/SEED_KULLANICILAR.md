# Fitliyo Seed Kullanıcılar

DbMigrator çalıştırıldıktan sonra aşağıdaki kullanıcılar otomatik oluşturulur. Giriş sayfasında **kullanıcı adı** veya **e-posta** ile giriş yapılabilir.

**Şifre (hepsi):** `Test123!`

| Rol      | Kullanıcı adı | E-posta             | Giriş sonrası yönlendirme |
|----------|----------------|---------------------|----------------------------|
| Admin    | admin          | admin@fitliyo.com   | /admin                    |
| Eğitmen  | egitmen        | egitmen@fitliyo.com | /egitmen                   |
| Öğrenci  | ogrenci        | ogrenci@fitliyo.com | /ogrenci                   |

## Nasıl oluşturulur?

1. **DbMigrator** çalıştırın (ilk kurulum veya migration sonrası):
   - VS Code: Run and Debug → **Fitliyo DbMigrator** → F5
   - Terminal: `dotnet run --project src/Fitliyo.DbMigrator`
2. Seed tamamlandığında log’da “Admin kullanıcı oluşturuldu”, “Eğitmen kullanıcı oluşturuldu”, “Öğrenci kullanıcı oluşturuldu” mesajlarını görebilirsiniz.
3. Frontend’de **Giriş Yap** sayfasından yukarıdaki bilgilerle giriş yapın.

## Teknik not

- Roller: **Admin**, **Trainer**, **Student** (FitliyoIdentityDataSeedContributor).
- Kullanıcılar yoksa oluşturulur; zaten varsa tekrar eklenmez.
- OpenIddict **Fitliyo_App** client’ı (Password grant) DbMigrator seed’i ile tanımlanır; giriş için `client_id=Fitliyo_App` kullanılır.
