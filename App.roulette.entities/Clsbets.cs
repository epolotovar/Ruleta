using System;
using System.Collections.Generic;
using System.Text;

namespace App.roulette.entities
{
    public class Clsbets
    {
        public int Idroulette { get; set; }
        public string IdUser { get; set; }
        public int? Number { get; set; }
        public bool? Negro { get; set; }
        public bool? Rojo { get; set; }
        public decimal Money { get; set; }
    }
}
