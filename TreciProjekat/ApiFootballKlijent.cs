using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace TreciProjekat
{
    public class ApiFootballKlijent
    {
        private HttpClient client;
        private int idTima;

        public ApiFootballKlijent(string apiKljuc, int idTima)
        {
            this.idTima = idTima;
            client = new HttpClient();
            client.BaseAddress = new Uri("https://v3.football.api-sports.io/");
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKljuc);
        }

        public async Task<List<Utakmica>> DohvatiOdigraneUtakmice()
        {
            string upit = $"fixtures?team={idTima}&season=2023&status=FT";
            HttpResponseMessage odgovor = await client.GetAsync(upit);

            string sadrzaj = await odgovor.Content.ReadAsStringAsync();

            List<Utakmica> sveUtakmice = ParsirajOdgovor(sadrzaj);

            List<Utakmica> poslednjih10 = sveUtakmice
                .OrderByDescending(u => u.DatumUtakmice)
                .Take(10)
                .ToList();

            return poslednjih10;
        }

        private List<Utakmica> ParsirajOdgovor(string json)
        {
            List<Utakmica> rezultat = new List<Utakmica>();

            using (JsonDocument dokument = JsonDocument.Parse(json))
            {
                JsonElement response = dokument.RootElement.GetProperty("response");

                foreach (JsonElement fixture in response.EnumerateArray())
                {
                    JsonElement teams = fixture.GetProperty("teams");
                    JsonElement homeTeam = teams.GetProperty("home");
                    JsonElement awayTeam = teams.GetProperty("away");

                    bool jeDomacin = homeTeam.GetProperty("id").GetInt32() == idTima;

                    JsonElement goals = fixture.GetProperty("goals");
                    int? goloviDomacina = goals.GetProperty("home").ValueKind == JsonValueKind.Null
                        ? null : goals.GetProperty("home").GetInt32();
                    int? goloviGosta = goals.GetProperty("away").ValueKind == JsonValueKind.Null
                        ? null : goals.GetProperty("away").GetInt32();

                    if (goloviDomacina == null || goloviGosta == null)
                        continue;

                    Utakmica utakmica = new Utakmica
                    {
                        JeDomacin = jeDomacin,
                        Protivnik = jeDomacin ? awayTeam.GetProperty("name").GetString()! : homeTeam.GetProperty("name").GetString()!,
                        GoloviNasegTima = jeDomacin ? goloviDomacina.Value : goloviGosta.Value,
                        GoloviProtivnika = jeDomacin ? goloviGosta.Value : goloviDomacina.Value,
                        DatumUtakmice = fixture.GetProperty("fixture").GetProperty("date").GetDateTime()
                    };

                    rezultat.Add(utakmica);
                }
            }

            return rezultat;
        }
    }
}