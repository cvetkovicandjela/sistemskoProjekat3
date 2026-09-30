using System.Collections.Generic;

namespace TreciProjekat
{
    //poruka koju salje Rx komponenta aktoru (nove/azurirane utakmice)
    public record NoveUtakmice(List<Utakmica> Utakmice);

    //poruka koju salje web server aktoru (trazi trenutnu statistiku)
    public record DajStatistiku();

    //poruka kojom aktor odgovara na DajStatistiku
    public record StatistikaOdgovor(List<Utakmica> Utakmice, double ProsekGolova, int UkupnoUtakmica);
}