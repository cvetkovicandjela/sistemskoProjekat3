using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Akka.Actor;

namespace TreciProjekat
{
    public class RxKomponenta
    {
        private ApiFootballKlijent klijent;
        private IActorRef timAktor;
        private IDisposable? pretplata;

        public RxKomponenta(ApiFootballKlijent klijent, IActorRef timAktor)
        {
            this.klijent = klijent;
            this.timAktor = timAktor;
        }

        public void Pokreni()
        {
            pretplata = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(30), TaskPoolScheduler.Default)
                .SelectMany(_ => Observable.FromAsync(() => klijent.DohvatiOdigraneUtakmice()))
                .Subscribe(
                    utakmice =>
                    {
                        timAktor.Tell(new NoveUtakmice(utakmice));
                        Console.WriteLine($"[Rx] Poslato aktoru {utakmice.Count} utakmica, vreme: {DateTime.Now:HH:mm:ss}");
                    },
                    greska =>
                    {
                        Console.WriteLine($"[Rx] Greska prilikom prikupljanja podataka: {greska.Message}");
                    }
                );
        }

        public void Zaustavi()
        {
            pretplata?.Dispose();
        }
    }
}