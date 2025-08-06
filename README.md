# Kurumsal CMS - Corporate Content Management System

Modern ASP.NET MVC (.NET 9) tabanlı kurumsal içerik yönetim sistemi.

## 🚀 Özellikler

### Temel Özellikler
- ✅ **Sayfa Yönetimi**: Dinamik sayfa ekleme, düzenleme, silme
- ✅ **Menü Yönetimi**: Hiyerarşik menü yapısı
- ✅ **Slider Yönetimi**: Ana sayfa slider'ları
- ✅ **Duyuru Sistemi**: Duyuru ekleme/düzenleme
- ✅ **Kullanıcı Yetkilendirmesi**: Rol bazlı erişim kontrolü

### Teknik Özellikler
- ✅ **ASP.NET MVC** (.NET 9) framework
- ✅ **Entity Framework Core** ORM
- ✅ **ASP.NET Identity** kimlik doğrulama
- ✅ **SQLite** veritabanı (geliştirme için)
- ✅ **Bootstrap 5** responsive tasarım
- ✅ **TinyMCE** rich text editor
- ✅ **Modern JavaScript** (ES6+)

## 🏗️ Sistem Mimarisi

```
CorporateCMS/
├── Areas/Admin/              # Admin paneli area
│   ├── Controllers/         # Admin kontrolcüleri
│   └── Views/              # Admin görünümleri
├── Controllers/            # Genel kontrolcüler
├── Data/                   # Entity Framework context
├── Models/                 # Veri modelleri
│   └── ViewModels/        # Görünüm modelleri
├── Views/                 # Genel görünümler
└── wwwroot/               # Statik dosyalar
    ├── css/admin.css      # Admin panel stilleri
    └── js/admin.js        # Admin panel JavaScript
```

## 🎯 Kullanıcı Rolleri

| Rol | Açıklama | Yetkiler |
|-----|----------|----------|
| **SuperAdmin** | Sistem yöneticisi | Tüm yetkiler |
| **Admin** | Site yöneticisi | Kullanıcı yönetimi hariç tüm yetkiler |
| **Editor** | İçerik editörü | İçerik oluşturma/düzenleme |
| **Viewer** | Görüntüleyici | Sadece okuma yetkisi |

## 🚀 Kurulum

### Gereksinimler
- .NET 9 SDK
- Visual Studio Code veya Visual Studio 2022
- SQLite (dahili)

### Adımlar

1. **Projeyi klonlayın**
```bash
git clone <repository-url>
cd CorporateCMS
```

2. **Bağımlılıkları yükleyin**
```bash
dotnet restore
```

3. **Veritabanını oluşturun**
```bash
dotnet ef database update
```

4. **Projeyi çalıştırın**
```bash
dotnet run
```

5. **Tarayıcıda açın**
```
https://localhost:5001
```

## 👤 Varsayılan Admin Kullanıcısı

Sistem ilk çalıştığında otomatik olarak admin kullanıcısı oluşturulur:

- **Email**: admin@kurumsalcms.com
- **Şifre**: Admin123!
- **Rol**: SuperAdmin

## 🎨 Admin Panel

Admin paneline erişim için:
```
https://localhost:5001/Admin
```

### Ana Özellikler
- 📊 Dashboard - Sistem istatistikleri
- 📄 Sayfa Yönetimi - İçerik oluşturma/düzenleme
- 🧭 Menü Yönetimi - Site navigasyonu
- 🖼️ Slider Yönetimi - Ana sayfa görselleri
- 📢 Duyuru Yönetimi - Haberler ve duyurular

## 🔧 Geliştirme

### Yeni Migration Oluşturma
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Proje Derleme
```bash
dotnet build
```

### Testler Çalıştırma
```bash
dotnet test
```

## 🏷️ Kullanılan Teknolojiler

- **Backend**: ASP.NET MVC (.NET 9), Entity Framework Core, ASP.NET Identity
- **Frontend**: HTML5, CSS3, Bootstrap 5, JavaScript (ES6+)
- **Veritabanı**: SQLite (geliştirme), SQL Server (production)
- **Editor**: TinyMCE Rich Text Editor
- **İkonlar**: Font Awesome 6

## 📝 API Endpoints

### Admin Area
```
GET  /Admin                    - Dashboard
GET  /Admin/Pages              - Sayfa listesi
POST /Admin/Pages/Create       - Yeni sayfa oluştur
GET  /Admin/Pages/Edit/{id}    - Sayfa düzenle
POST /Admin/Pages/Delete/{id}  - Sayfa sil
```

### Public Area
```
GET  /                         - Ana sayfa
GET  /{slug}                   - Dinamik sayfa görüntüleme
```

## 🔒 Güvenlik

- ✅ CSRF Token koruması
- ✅ SQL Injection koruması (EF Core)
- ✅ XSS koruması (HTML encoding)
- ✅ Rol bazlı yetkilendirme
- ✅ Güvenli dosya yükleme

## 📈 Performans

- Lazy loading
- Output caching
- Image optimization
- Minified CSS/JS
- Database indexing

## 🚀 Production Deployment

### IIS Deployment
1. Publish projeyi:
```bash
dotnet publish -c Release -o ./publish
```

2. IIS'e deploy edin
3. Connection string'i production SQL Server için güncelleyin

### Docker (Opsiyonel)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "CorporateCMS.dll"]
```

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 İletişim

- **Geliştirici**: CorporateCMS Team
- **Email**: info@kurumsalcms.com
- **Website**: https://kurumsalcms.com

---

**⚡ Kurumsal CMS** - Modern, güvenli ve ölçeklenebilir içerik yönetim sistemi
