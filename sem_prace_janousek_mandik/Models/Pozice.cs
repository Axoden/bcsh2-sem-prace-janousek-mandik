﻿using System.ComponentModel;

namespace sem_prace_janousek_mandik.Models
{
	public class Pozice
	{
		[Browsable(false)]
		public int IdPozice { get; set; }

		public string? Nazev { get; set; }
	}
}