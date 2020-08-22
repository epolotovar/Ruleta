using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.roulette.entities
{
    public class ClsBet
    {
        [Required]
        public int Idroulette { get; set; }
        [Range(0, 36)]
        public int? Number { get; set; }
        public bool Negro { get; set; }
        public bool Rojo { get; set; }
        [Range(1, 10000)]
        public decimal Money { get; set; }
    }
}
