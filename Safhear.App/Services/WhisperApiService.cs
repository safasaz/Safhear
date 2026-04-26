using System.Text.Json;

namespace Safhear.Services;

public class WhisperApiService
{
    // Android emulatorunde 10.0.2.2 → host makinenin localhost'u demektir.
    // Gercek fiziksel cihaz kullaniyorsaniz bilgisayarinizin WiFi IP'sini yazin:
    //   Ornek: private const string SunucuAdresi = "http://192.168.1.100:5000";
    private const string SunucuAdresi = "http://10.0.2.2:5000";

    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(60)
    };

    public async Task<string> SesiMetnedonustur(byte[] sesVerisi)
    {
        using var form = new MultipartFormDataContent();

        var sesIcerigi = new ByteArrayContent(sesVerisi);
        sesIcerigi.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
        form.Add(sesIcerigi, "audio", "kayit.wav");

        var yanit = await _httpClient.PostAsync($"{SunucuAdresi}/transcribe", form);
        yanit.EnsureSuccessStatusCode();

        var json = await yanit.Content.ReadAsStringAsync();
        using var belge = JsonDocument.Parse(json);

        if (belge.RootElement.TryGetProperty("metin", out var metin))
            return metin.GetString() ?? string.Empty;

        if (belge.RootElement.TryGetProperty("hata", out var hata))
            throw new Exception(hata.GetString());

        return string.Empty;
    }
}
