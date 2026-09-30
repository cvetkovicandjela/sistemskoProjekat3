using System;
using System.IO;
using System.Text.Json;

namespace TreciProjekat
{
    class Konfiguracija
    {
        public string ApiFootballKljuc { get; set; } = "";

        public static Konfiguracija Ucitaj()
        {
            string putanja = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
            string sadrzaj = File.ReadAllText(putanja);
            Konfiguracija? konfig = JsonSerializer.Deserialize<Konfiguracija>(sadrzaj);
            return konfig ?? new Konfiguracija();
        }
    }
}