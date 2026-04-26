"""
İşitme Engelli Yardımcı - Python Sunucu
----------------------------------------
Bu sunucu mobil uygulamadan gelen ses dosyasını
OpenAI Whisper modeli ile Türkçe metne çevirir.
"""

from flask import Flask, request, jsonify
import whisper
import tempfile
import os

app = Flask(__name__)

# Whisper modelini yükle (ilk seferde ~150MB indirir)
# "base" modeli hız/doğruluk dengesi için idealdir.
# Daha iyi doğruluk için "small" veya "medium" kullanabilirsiniz.
print("Whisper modeli yükleniyor, lütfen bekleyin...")
model = whisper.load_model("base")
print("Model hazır! Sunucu başlatılıyor...\n")


@app.route("/transcribe", methods=["POST"])
def transcribe():
    """
    Mobil uygulamadan gelen ses dosyasını alır ve metne çevirir.
    Beklenen: multipart/form-data, 'audio' alanında ses dosyası
    Dönen: {"metin": "çevrilen metin"}
    """
    if "audio" not in request.files:
        return jsonify({"hata": "Ses dosyası bulunamadı"}), 400

    audio_file = request.files["audio"]

    # Geçici dosyaya kaydet (Whisper dosya yolu ister)
    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as tmp:
        audio_file.save(tmp.name)
        tmp_path = tmp.name

    try:
        # Whisper ile Türkçe olarak metne çevir
        result = model.transcribe(tmp_path, language="tr")
        metin = result["text"].strip()

        print(f"Çevrilen metin: {metin}")
        return jsonify({"metin": metin})

    except Exception as e:
        print(f"Hata: {e}")
        return jsonify({"hata": str(e)}), 500

    finally:
        # Geçici dosyayı temizle
        if os.path.exists(tmp_path):
            os.unlink(tmp_path)


@app.route("/saglik", methods=["GET"])
def saglik():
    """Sunucunun çalışıp çalışmadığını kontrol etmek için."""
    return jsonify({"durum": "çalışıyor"})


if __name__ == "__main__":
    print("Sunucu http://0.0.0.0:5000 adresinde çalışıyor")
    print("Durdurmak için CTRL+C\n")
    app.run(host="0.0.0.0", port=5000, debug=False)
