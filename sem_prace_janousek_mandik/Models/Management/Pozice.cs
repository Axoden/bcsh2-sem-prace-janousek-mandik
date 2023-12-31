﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sem_prace_janousek_mandik.Models.Management
{
    public class Pozice
    {
        [Browsable(false)]
        public int IdPozice { get; set; }

        [Required]
		[RegularExpression("^[A-Z][a-zA-Z]{2,}", ErrorMessage = "Musí začínat velkým písmenem a musí obsahovat minimálně 3 znaky.")]
		[DisplayName("Název pozice: ")]
        public string? Nazev { get; set; }
    }
}