using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valutaomregner
{
    // Dette er den klasse vi bruger som element i vores til-valuta komboboks, for at opbevare en kurs i forhold til den valgte valuta.
    public class CurrencyItem
    {
        // String der opbevarer CurrencyCode, for eksempel "DKK", "EUR" osv., for nemt at kunne konvertere et CurrencyItem element
        // til bare en string, da fra-valuta komboboksen bruger strings som element
        public string CurrencyCode { get; set; }
        // Kursen for den nuværende valgte valuta i fra-valuta komboboksen
        public decimal ExchangeRate { get; set; }

        // En funktion der returnerer et string der viser de to properties korrekt formatteret. Bruges i vores xaml fil ved definitionen af til-valuta komboboksen,
        // som bruger det til at se hvordan den skal formattere denne datatype
        public string DisplayName
        {
            get { return $"{CurrencyCode} ({ExchangeRate:N2})"; }
        }
    }
}
