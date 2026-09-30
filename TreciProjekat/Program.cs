using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Akka.Actor;

namespace TreciProjekat
{
    class Program
    {
        static HttpListener listener = new HttpListener();
        static Loger logger = new Loger("log.txt");
        static IActorRef? timAktor;

        static async Task Main(string[] args)
        {
            Konfiguracija konfig = Konfiguracija.Ucitaj();
            int idTima = 33; //Manchester United

            ActorSystem sistem = ActorSystem.Create("FudbalSistem");
            timAktor = sistem.ActorOf(Props.Create<TimAktor>(), "timAktor");

            ApiFootballKlijent klijent = new ApiFootballKlijent(konfig.ApiFootballKljuc, idTima);

            RxKomponenta rx = new RxKomponenta(klijent, timAktor);
            rx.Pokreni();

            listener.Prefixes.Add("http://localhost:5052/");
            listener.Start();
            logger.Zabelezi("Server je pokrenut na http://localhost:5052/");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                _ = ObradiZahtevAsync(context);
            }
        }

        static async Task ObradiZahtevAsync(HttpListenerContext context)
        {
            HttpListenerResponse? response = null;

            try
            {
                HttpListenerRequest request = context.Request;
                response = context.Response;

                logger.Zabelezi($"Primljen zahtev: {request.Url}");

                //server samo prevodi zahtev u poruku ka aktoru, ne zove API sam
                var statistika = await timAktor!.Ask<StatistikaOdgovor>(new DajStatistiku(), TimeSpan.FromSeconds(10));

                string json = JsonSerializer.Serialize(statistika, new JsonSerializerOptions { WriteIndented = true });
                byte[] bajtovi = System.Text.Encoding.UTF8.GetBytes(json);

                response.ContentType = "application/json";
                response.ContentLength64 = bajtovi.Length;
                await response.OutputStream.WriteAsync(bajtovi, 0, bajtovi.Length);
                response.OutputStream.Close();

                logger.Zabelezi($"Zahtev uspesno obradjen, vraceno {statistika.UkupnoUtakmica} utakmica.");
            }
            catch (Exception e)
            {
                logger.Zabelezi($"[GRESKA] Neuspesna obrada zahteva: {e.Message}");

                try
                {
                    if (response != null)
                    {
                        response.StatusCode = 500;
                        byte[] bajtoviGreske = System.Text.Encoding.UTF8.GetBytes("Doslo je do greske na serveru.");
                        response.ContentLength64 = bajtoviGreske.Length;
                        response.OutputStream.Write(bajtoviGreske, 0, bajtoviGreske.Length);
                        response.OutputStream.Close();
                    }
                }
                catch { }
            }
        }
    }
}