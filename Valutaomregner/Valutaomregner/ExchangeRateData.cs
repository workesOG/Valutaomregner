using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valutaomregner
{
    // Hjælper-klasse til de-serialization af json-stringet vi får fra API-callet.
    public class ExchangeRateData
    {
        // Disse 3 properties bryder normale navngivnings-konventioner. Dette er fordi de skal matche felter fra json-stringet fra API-callet.
        // En af felterne i den json vi får tilbage er for eksempel "base_code" som er den valuta der er taget udgangspunkt i
        // Jeg har vedhæftet et eksempel af, hvordan responsen fra et sådant API-call kan se ud i bilag.
        // Her vil du kunne se de forskellige felter, og hvordan de passer med disse properties
        public string result { get; set; }
        public string base_code { get; set; }
        public Dictionary<string, decimal> conversion_rates { get; set; }
    }
}
