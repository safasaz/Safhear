using Safhear.Services;
using Plugin.Maui.Audio;

namespace Safhear;

public partial class MainPage : ContentPage
{
    private IAudioRecorder? _recorder;
    private bool _kayitYapiliyor = false;
    private readonly WhisperApiService _whisperService = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void DinleButon_Clicked(object sender, EventArgs e)
    {
        if (!_kayitYapiliyor)
            await KayitBaslat();
        else
            await KayitDurdurVeIsle();
    }

    private async Task KayitBaslat()
    {
        var izin = await Permissions.RequestAsync<Permissions.Microphone>();
        if (izin != PermissionStatus.Granted)
        {
            await DisplayAlert("Izin Gerekli",
                "Uygulamanin calisabilmesi icin mikrofon izni gereklidir.",
                "Tamam");
            return;
        }

        _recorder = AudioManager.Current.CreateRecorder();
        await _recorder.StartAsync();

        _kayitYapiliyor = true;
        MetinLabel.Text = "";
        MetinLabel.TextColor = Colors.White;
        DinleButon.Text = "Dinlemeyi Durdur";
        DinleButon.BackgroundColor = Color.FromArgb("#CC3300");
        KayitGostergesi.IsVisible = true;
        DurumLabel.Text = "Konusmayi dinliyorum...";
    }

    private async Task KayitDurdurVeIsle()
    {
        if (_recorder == null) return;

        KayitGostergesi.IsVisible = false;
        DinleButon.IsEnabled = false;
        DurumLabel.Text = "Ses isleniyor...";

        var kayit = await _recorder.StopAsync();
        _kayitYapiliyor = false;

        try
        {
            using var stream = kayit.GetAudioStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var sesVerisi = ms.ToArray();

            if (sesVerisi.Length == 0)
            {
                MetinLabel.Text = "Ses kaydedilemedi. Tekrar deneyin.";
                MetinLabel.TextColor = Color.FromArgb("#FF8C00");
                return;
            }

            DurumLabel.Text = "Sunucuya gonderiliyor...";
            var metin = await _whisperService.SesiMetnedonustur(sesVerisi);

            if (!string.IsNullOrWhiteSpace(metin))
            {
                MetinLabel.Text = metin;
                MetinLabel.TextColor = Colors.White;
            }
            else
            {
                MetinLabel.Text = "Ses anlasılamadi. Lutfen daha net konusup tekrar deneyin.";
                MetinLabel.TextColor = Color.FromArgb("#FF8C00");
            }
        }
        catch (HttpRequestException)
        {
            MetinLabel.Text = "Sunucuya baglanamadi!\n\nPython sunucusunun calistigından ve IP adresinin dogru oldugunden emin olun.";
            MetinLabel.TextColor = Color.FromArgb("#E94560");
        }
        catch (Exception ex)
        {
            MetinLabel.Text = "Hata: " + ex.Message;
            MetinLabel.TextColor = Color.FromArgb("#E94560");
        }
        finally
        {
            DinleButon.IsEnabled = true;
            DinleButon.Text = "Dinlemeye Basla";
            DinleButon.BackgroundColor = Color.FromArgb("#E94560");
            DurumLabel.Text = "Hazir";
        }
    }

    private void TemizleButon_Clicked(object sender, EventArgs e)
    {
        MetinLabel.Text = "Dinlemeye baslamak icin asagidaki butona basin...";
        MetinLabel.TextColor = Color.FromArgb("#AAAACC");
    }
}
