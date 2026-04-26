# İşitme Engelli Yardımcı

**Teknofest - Erişilebilir Yaşam Teknolojileri**

İşitme engelli bireyler için gerçek zamanlı ses-metin dönüştürme uygulaması.  
Yakındaki kişilerin konuşmasını dinler ve ekranda büyük yazıyla gösterir.

---

## Proje Yapısı

```
Safhear/
├── backend/                    ← Python sunucu Whisper AI
│   ├── app.py                  ← Ana sunucu kodu
│   └── requirements.txt        ← Python kütüphaneleri
└── Safhear.App/          ← C# .NET mobil uygulama
    ├── MainPage.xaml           ← Ana ekran tasarımı
    ├── MainPage.xaml.cs        ← Ana ekran kodu
    ├── Services/
    │   └── WhisperApiService.cs ← Sunucuyla iletişim
    └── Platforms/Android/      ← Android izinleri
```

## Nasıl Çalışır?

```
[Yakındaki kişi konuşur]
        ↓
[Mobil Uygulama mikrofonu kaydeder]
        ↓
[Ses → Python sunucusu]
        ↓
[Whisper AI → Türkçe metne çevirir]
        ↓
[Metin → Mobil uygulamaya geri döner]
        ↓
[İşitme engelli birey büyük yazıyı okur]
```

---

## Kurulum

### 1. Python Sunucusu

**Gereksinimler:** Python 3.8+, pip, **ffmpeg**

**ffmpeg kurulumu:**
- Windows: `winget install ffmpeg` veya https://ffmpeg.org/download.html
- macOS: `brew install ffmpeg`
- Linux: `sudo apt install ffmpeg`

**Sunucuyu başlat:**
```bash
cd backend
pip install -r requirements.txt
python app.py
```

> İlk çalıştırmada Whisper modeli (~150 MB) otomatik indirilir.

### 2. Mobil Uygulama (C# .NET MAUI)

**Gereksinimler:** .NET 8 SDK, Visual Studio 2022

**IP adresi ayarı:**  
`Services/WhisperApiService.cs` dosyasını açın ve `SunucuAdresi` değişkenini güncelleyin:

```csharp
// Android emülatörü için (varsayılan):
private const string SunucuAdresi = "http://10.0.2.2:5000";

// Gerçek Android cihaz için (bilgisayarınızın WiFi IP'si):
private const string SunucuAdresi = "http://192.168.1.100:5000";
```

> IP adresinizi öğrenmek için: Windows'ta `ipconfig`, Mac/Linux'ta `ifconfig`

**Derleme ve yükleme:**
```bash
cd Safhear.App
dotnet build -t:Run -f net8.0-android
```

---

## Kullanım

1. `python backend/app.py` ile sunucuyu başlatın
2. Mobil uygulamayı açın
3. **"Dinlemeye Başla"** butonuna basın
4. Yakınınızdaki biri konuşsun
5. **"Dinlemeyi Durdur"** butonuna basın
6. Çevrilen metin büyük yazıyla ekranda görünür

---

## Teknolojiler

| Katman | Teknoloji | Açıklama |
|--------|-----------|----------|
| Mobil Uygulama | C# / .NET MAUI | iOS ve Android |
| Ses Kaydı | Plugin.Maui.Audio | Mikrofon erişimi |
| Sunucu | Python / Flask | REST API |
| Konuşma Tanıma | OpenAI Whisper | Yerel AI model, ücretsiz |

---

## Notlar

- Whisper `base` modeli Türkçe için yeterince doğrudur. Daha iyi sonuç için `app.py` dosyasında `"base"` yerine `"small"` veya `"medium"` yazabilirsiniz (daha fazla RAM gerektirir).
- Sunucu ve telefon aynı WiFi ağında olmalıdır.
- Plugin.Maui.Audio versiyonu için NuGet'ten son sürümü kontrol edin.
