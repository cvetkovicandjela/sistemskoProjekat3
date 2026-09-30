using System;

namespace TreciProjekat
{
    public class Utakmica
    {
        public string Protivnik { get; set; } = "";
        public int GoloviNasegTima { get; set; }
        public int GoloviProtivnika { get; set; }
        public DateTime DatumUtakmice { get; set; }
        public bool JeDomacin { get; set; }

        
        public int GolRazlika => GoloviNasegTima - GoloviProtivnika; //izracunava se svaki put kad se pozove
    }
}