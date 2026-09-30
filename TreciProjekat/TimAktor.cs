using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace TreciProjekat
{
    public class TimAktor : ReceiveActor
    {
        //interno stanje aktora
        private List<Utakmica> utakmice = new List<Utakmica>();

        public TimAktor()
        {
            

            Receive<NoveUtakmice>(poruka =>
            {
                utakmice = poruka.Utakmice;
                Console.WriteLine($"[Aktor] Azurirano stanje, ukupno utakmica: {utakmice.Count}");
            });

            Receive<DajStatistiku>(poruka =>
            {
                double prosek = utakmice.Count > 0
                    ? utakmice.Average(u => u.GoloviNasegTima)
                    : 0;

                var odgovor = new StatistikaOdgovor(utakmice, prosek, utakmice.Count);

                
                Sender.Tell(odgovor); //Sender je aktor (ili entitet) koji je poslao poruku na koju sad odgovaramo
            });
        }
    }
}