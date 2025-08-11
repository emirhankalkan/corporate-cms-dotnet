# Corporate CMS - Kurumsal İçerik Yönetim Sistemi

Modern ASP.NET MVC (.NET 9) tabanlı kurumsal içerik yönetim sistemi.

## Güncel Öne Çıkanlar (Son Eklemeler)
- Google OAuth entegrasyonu
- Genişletilmiş kullanıcı profili (DisplayName, Bio, AvatarUrl, LastLoginAt)
- Global Exception Middleware (tüm beklenmeyen hatalar loglanır / özel hata sayfası)
- Güvenli resim yükleme: Uzantı + MIME + boyut (2MB) doğrulaması, rastgele dosya adı
- Duyuru ve Sayfalarda SEO dostu Slug alanı (duyurulara eklendi, benzersiz index)
- Basit pagination (duyurular Index action parametreleri ile hazır)

## Özellikler

### Temel Özellikler
- **Sayfa Yönetimi**: Dinamik sayfa ekleme, düzenleme, silme
- **Menü Yönetimi**: Hiyerarşik menü yapısı
- **Slider Yönetimi**: Ana sayfa slider'ları
- **Duyuru Sistemi**: Duyuru ekleme/düzenleme
- **Kullanıcı Yetkilendirmesi**: Rol bazlı erişim kontrolü

### Teknik Özellikler
- **ASP.NET MVC** (.NET 9) framework
- **Entity Framework Core** ORM + Migrations
- **ASP.NET Identity** kimlik doğrulama (genişletilmiş ApplicationUser)
- **SQLite** veritabanı (geliştirme için)
- Basitleştirilmiş mimari (ek servis katmanı kaldırıldı)

## Sistem Mimarisi

```
CorporateCMS/
├── Areas/Admin/              # Admin paneli area
│   ├── Controllers/          # Admin kontrolcüleri
│   └── Views/                # Admin görünümleri
├── Controllers/              # Genel kontrolcüler
├── Data/                     # Entity Framework context
├── Models/                   # Veri modelleri
│   └── ViewModels/           # Görünüm modelleri
├── Middleware/               # Global hata yakalama vb.
├── Views/                    # Genel görünümler
└── wwwroot/                  # Statik dosyalar
```
  
## Kullanıcı Rolleri

| Rol | Açıklama | Yetkiler |
|-----|----------|----------|
| **SuperAdmin** | Sistem yöneticisi | Tüm yetkiler |
| **Admin** | Site yöneticisi | Kullanıcı yönetimi hariç tüm yetkiler |
| **Editor** | İçerik editörü | İçerik oluşturma/düzenleme |
| **Viewer** | Görüntüleyici | Sadece okuma |

## Kurulum

### Gereksinimler
- .NET 9 SDK
- Visual Studio Code veya Visual Studio 2022
- SQLite
 
### Adımlar

1. **Projeyi klonlayın**
```bash
git clone https://github.com/emirhankalkan/corporate-cms-dotnet.git
cd corporate-cms-dotnet
```

2. **Bağımlılıkları yükleyin**
```bash
dotnet restore
```

3. **Migration uygula**
```bash
dotnet ef database update
```

4. **Çalıştır**
```bash
dotnet run
```

5. **Tarayıcıda açın**
```
http://localhost:5084
```

## Google OAuth
appsettings.json içine client id/secret yerine prod'da ortam değişkenleri veya User Secrets kullanın:
```
Authentication:Google:ClientId
Authentication:Google:ClientSecret
```

User Secrets örnek (development):
```bash
dotnet user-secrets set "Authentication:Google:ClientId" "XXX"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YYY"
```

## Varsayılan Admin Kullanıcısı
- **Email**: admin@kurumsalcms.com
- **Şifre**: Admin123!
- **Rol**: SuperAdmin

## Güvenli Resim Yükleme Kuralları
- Uzantılar: .jpg .jpeg .png .gif .webp
- MIME: image/jpeg, image/png, image/gif, image/webp
- Maksimum: 2MB
- Rastgele dosya adı GUID + uzantı
- Başarılı güncellemede eski dosya silinir

## Slug Kuralları
- Türkçe karakterler sadeleştirilir
- Harf + rakam + tire
- Ardışık tireler teklenir
- Boşluk -> '-'
- Benzersiz index (Announcement.Slug ve Page.Slug)

## Production İçin Yayınlama
```bash
dotnet publish -c Release -o publish
```
Reverse proxy (Nginx/IIS) arkasına koyun; ortam değişkenleri ile ConnectionStrings__DefaultConnection ve Google secret değerlerini verin.

## Migration İş Akışı
```bash
dotnet ef migrations add AddAnnouncementSlug
# Değişiklikleri inceleyin
dotnet ef database update
```

## Temizlik / Kullanılmayan Kod
Servis katmanı kaldırıldı; tüm kontrolcümler doğrudan DbContext kullanıyor.

## Katkıda Bulunma
1. Fork edin
2. Feature branch (`git checkout -b feature/slug-improvement`)
3. Commit (`git commit -m "Add feature"`)
4. Push (`git push origin feature/slug-improvement`)
5. PR açın

---
Corporate CMS – Modern, basit ve yönetilebilir içerik yönetim sistemi
