using System.Collections.Generic;

namespace Vaktija.ba.Views
{
    class Data
    {
        public static Data data;

        public List<Location> gradovi { get; set; }
        public List<Razlika> razlike { get; set; }
        public List<Razlika> razlike_sandzak { get; set; }
        public List<Takvim> takvim { get; set; }
    }
}
