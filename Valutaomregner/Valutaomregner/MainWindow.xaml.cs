using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Valutaomregner
{
    public partial class MainWindow : Window
    {
        // Deklaration af Main-Classens konstanter og Dictionaryet der holder kurser.
        private const string apiKey = "2450d4ff28c6251bc954840d";
        private const string apiUrl = "https://v6.exchangerate-api.com/v6/{0}/latest/{1}";
        private Dictionary<string, decimal> rates;

        public MainWindow()
        {
            InitializeComponent(); // Standard funktion til at initializere selve vinduet, i dette tilfælde MainWindow
            LoadRates(); // Funktionen er beskrevet nedenunder
        }

        // Denne funktion bruger API'en til at hente alle valutaerne og kurserne ned, ved hjælpe af en hjælper-klasse "ExchangeRateData"
        private async void LoadRates()
        {
            // Vi fastsætter den endelige URL ved at kombinere API-nøglen og den valuta vi tager udgangpunkt i. Her tager vi udgangspunkt i "EUR"
            string url = string.Format(apiUrl, apiKey, "EUR");

            // Her bruger vi en HTTP Client for at tilgå URLen, da API-calls fungerer ved hjælp af URLer
            using (HttpClient client = new HttpClient())
            {
                // Vi bruger try / catch for at undgå at programmet crasher hvis vi oplever en fejl vi ikke havde forventet
                try
                {
                    // Vi henter json-outputtet fra API-callet ind i et variabel, og de-serializer det ved hjælp af hjælper-klassen "ExchangeRateData", som har
                    // felter der matcher de dele af API-resultatet vi er interesseret i
                    // Dette resulterer I at de forskellige dele af json stringen vi skal bruge bliver fordelt i vores hjælper-klasse og kan tilgås individuelt uden besvær
                    string json = await client.GetStringAsync(url);
                    ExchangeRateData data = JsonConvert.DeserializeObject<ExchangeRateData>(json);

                    // Vi opdaterer vores main-class field "rates" til at være lig med vores nulavede instance af hjælper-classens "exchange-rates", som er
                    // en af de felter i den json, API-callet returnerer
                    rates = data.conversion_rates;

                    // Vi opdaterer elementerne i vores fra kombo-box ved at bruge nøglerne fra vores rates Dictionary, som er strings såsom "DKK", "EUR" osv
                    cmbFromCurrency.ItemsSource = rates.Keys.ToList();
                    // Vi sætter den nuværende valgte fra-valuta til at være DKK, fordi det nok er en af de valutaer der vil blive brugt mest
                    cmbFromCurrency.SelectedItem = "DKK";

                    // Vi kalder funktionen til at opdatere til-valutaen's kombo-boxs' kurser i forhold til den nuværende valgte valuta
                    UpdateToCurrencyComboBox();
                }
                // Hvis der opstår en fejl, så sørger vi for at det ikke resulterer i et komplet crash af applikationen
                catch (Exception ex)
                {
                    // Vi viser eventuelle fejl i en box, for at man kan debugge det efterfølgende
                    MessageBox.Show("Fejl ved hentning af valutakurser: " + ex.Message);
                }
            }
        }

        // Dette er den funktion der automatisk kaldes når tekst-feltet til "Beløb" opdateres (Når brugeren skriver i det). Her opdaterer vi automatisk outputtet.
        private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateConversion();
        }

        // Denne funktion bliver automatisk kaldet når en valuta-kombo-boksene ændres i
        private void Currency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Her tjekker vi, om det er fra-valutaen der er blevet ændret, ved bruge det automatisk indpasserede parameter "sender"
            if (sender == cmbFromCurrency)
            {
                // Hvis det er fra-valutaen der blev ændret, opdaterer vi kurserne i til-valuta komboboksen så de er relative til den ny-valgte valuta
                UpdateToCurrencyComboBox();
            }
            // I alle tilfælde opdaterer vi resultats-feltet, for at sikre os, at vores omregning er korrekt
            CalculateConversion();
        }

        // Denne funktion bliver automatisk kørt når vi klikker på bytte-knappen
        private void BtnSwapCurrencies_Click(object sender, RoutedEventArgs e)
        {
            // Vi starter med at finde ud af, hvilken valuta der er valgt i fra-valuta komboboksen.
            // Vi beholder den valgte valuta i et variabel, så vi kan gemme det i hukommelsen til når vi overskrider værdien
            string fromCurrency = cmbFromCurrency.SelectedItem as string;

            // Vi omdanner det nuværende valgte element i fra-valuta komboboksen til et CurrencyItem, som vi opbeholder i et variabel
            CurrencyItem toCurrencyItem = cmbToCurrency.SelectedItem as CurrencyItem;

            // Vi sikrer os, at ingen af de to felter har været tomme. Dette burde ikke kunne ske,
            // men hvis det på en måde gjorde, ville det ikke resultere i et program-crash
            if (fromCurrency != null && toCurrencyItem != null)
            {
                // Vi sætter det nuværende element i fra-valuta komboboksen til at være det element der før var i før-valuta komboboksen
                cmbFromCurrency.SelectedItem = toCurrencyItem.CurrencyCode;

                // Vi opdaterer til-valuta komboboksen i forhold til den nye valgte valuta
                UpdateToCurrencyComboBox();

                // Vi omdanner alle elementer fra til-valuta komboboksen til en liste af CurrencyItems
                // Vi finder det element fra denne liste, hvor CurrencyCode matcher det førhenværende element i fra-valuta komboboksen ved hjælp af C# Linq funktionalitet
                var toCurrencyItems = cmbToCurrency.ItemsSource as List<CurrencyItem>;
                var newToCurrencyItem = toCurrencyItems.FirstOrDefault(c => c.CurrencyCode == fromCurrency);

                // Derefter sætter vi det valgte element i til-valuta komboboksen til det ny-fundne element med den korrekte CurrencyCode
                cmbToCurrency.SelectedItem = newToCurrencyItem;

                // Derefter sørger vi selvfølgelig for at opdatere omregningen
                CalculateConversion();
            }
        }

        // Denne funktion opdaterer alle elementerne i til-valuta komboboksen,
        // sådan at deres kurser er sat relativ til den valuta der er valgt i fra-valuta komboboksen
        private void UpdateToCurrencyComboBox()
        {
            // Først sikrer vi os, at hverken vores Dictionary rates eller det valgte element i fra-valuta komboboksen er ikke-eksisterende,
            // altså null. Hvis nogle af dem er, så slutter vi funktionen med det samme for at undgå NullReference Exceptions
            if (rates == null || cmbFromCurrency.SelectedItem == null)
                return;

            // Vi finder det den valgte valuta fra fra-valuta komboboksen, og gemmer det i en string.
            // Vi bruger denne string til at hente kursen fra vores rates Dictionary, som vi gemmer i et andet variabel.
            string fromCurrency = cmbFromCurrency.SelectedItem as string;
            decimal rateFrom = rates[fromCurrency];

            // Vi klargører en liste af CurrencyItems, som vi vil udfylde og bruge til at opdatere til-valuta komboboksen
            List<CurrencyItem> toCurrencyItems = new List<CurrencyItem>();

            // Her definerer vi et loop, hvor vi itererer igennem alle nøglerne i rates Dictionaryet
            foreach (var currencyCode in rates.Keys)
            {
                // Vi definerer kursen af det nuværende element i rates Dictionaryet til et variabel
                decimal rateTo = rates[currencyCode];

                // Vi definerer forholdet mellem den valgte valuta i fra-valuta komboboksen og det nuværende element i løkken.
                // Vi ganger dette med 100, da kursen normalt er opgivet i, hvor mange af en given valuta du kan få for 100 af en anden valuta
                decimal exchangeRate = (rateFrom / rateTo) * 100;

                // Vi tilføjer et nyt element af typen CurrencyItem til listen, hvor vi informerer om CurrencyCode og den nyfundne kurs.
                toCurrencyItems.Add(new CurrencyItem
                {
                    CurrencyCode = currencyCode,
                    ExchangeRate = exchangeRate
                });
            }
            // Dermed har vi for hvor element i vores rate Dictionary lavet et tilsvarende element af CurrencyItem,
            // hvor kursen er opdateret i forhold til den valgte valuta fra fra-valuta komboboksen

            // Vi klargører et string variabel til at holde CurrencyCoden fra det sidst valgte element i til-valuta komboboksen
            string previousToCurrency = null;
            // Hvis det nuværende element fra til-valuta komboboksen er af typen CurrencyItem, så definerer vi det som et lokalt variabel.
            // Et tilfælde hvor dette statement ender i false burde helst aldrig ske, men
            // ligesom flere gang tidligere, prøver vi at helgardere os fra program-crashes.
            if (cmbToCurrency.SelectedItem is CurrencyItem previousItem)
            {
                // Gem CurrencyCode feltet's værdi i vores string, for at huske på, hvilken valuta der er valgt. Dette gør vi,
                // fordi listens bliver "gensat" om lidt, og derfor vil det valgte element gå tabt hvis ikke vi gemmer det
                previousToCurrency = previousItem.CurrencyCode;
            }

            // Her sætter vi til-valutaens komboboks til den liste vi populerede ved hjælp af foreach løkken tidligere. Dette betyder at de tidligere elementer går tabt
            cmbToCurrency.ItemsSource = toCurrencyItems;

            // Vi sikrer os at den string vi bruge til at gemme det førhen valgte element i til-valuta komboboksen ikke er tomt eller en null-værdi
            if (!string.IsNullOrEmpty(previousToCurrency))
            {
                // Hvis det ikke er, sætter vi det nuværende valgte element til at være det element, der har den samme CurrencyCode som det tidligere valgte
                cmbToCurrency.SelectedItem = toCurrencyItems.FirstOrDefault(c => c.CurrencyCode == previousToCurrency);
            }
            else
            {
                // Dette kode skulle helst aldrig køre, men i tilfælde af at der opstår en fejl med stringen sætter vi bare det valgte element til det første i komboboksen.
                // Dette er igen en gardering fra program-crashes. Denne strategi er brugt mange gange i løbet af projektet.
                // Det er fordi det både sørger for programmet ikke crasher, hvilket kan have uforudsete konsekvenser, men det giver også ofte et bedre udgangspunkt for debugging.
                // for eksempel, i dette tilfælde, hvis vi ser at valutaen ændrer sig til at være det første element muligt, ved vi,
                // at der sker en logisk fejl forårsaget af, at den string der skal gemme vores CurrencyCode er gået tabt et sted
                cmbToCurrency.SelectedIndex = 0;
            }
        }

        // Dette er en den funktion der opdaterer resultat's-feltet i forhold til beløb's-feltet
        private void CalculateConversion()
        {
            // Først sikrer vi os at rates ikke er udefineret
            if (rates == null)
                return;

            // Her sørger vi for, at der ikke opstår fejl fordi folk bruger komma istedet for punktum eller vice-versa, ved bare at skifte eventuelle kommaer til punktummer
            string amountText = txtAmount.Text.Replace(",", ".");

            // Her prøver vi at "konvertere" vores string til datatypen decimal. TryParse vil IKKE resultere i et crash hvis det ikke lykkedes,
            // istedet returnerer selve funktionen TryParse true eller false alt efter om operationen lykkedes
            // Det kan vi bruge i forlængelse med et if-statement for at eksekvere forskellige stykker kode alt efter om konversionen var en success eller ej.
            // TryParse kræver derudover at man definerer et lokalt variabel med "out decimal [navn]"
            // i selve funktionen. Dette variabel kan så bruges i det efterfølgende if-statement ved success, og vil holde den konverterede værdi
            if (decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
            {
                // Vi finder de valgte elementer fra fra-valuta og til-valuta komboboksene, og gemmer referencer dem i lokale variabler.
                string fromCurrency = cmbFromCurrency.SelectedItem as string;
                CurrencyItem toCurrencyItem = cmbToCurrency.SelectedItem as CurrencyItem;

                // Vi sikrer os at de fundne elementer ikke er null-værdier
                if (!string.IsNullOrEmpty(fromCurrency) && toCurrencyItem != null)
                {
                    // Vi finder kurserne fra begge valutaer, og opbevarer dem i lokale variabler
                    decimal rateFrom = rates[fromCurrency];
                    decimal rateTo = rates[toCurrencyItem.CurrencyCode];

                    // Vi finder forholdet mellem kurserne
                    decimal exchangeRate = rateTo / rateFrom;

                    // Derefter bruger vi det forhold i forlængelse med det fundne tal i beløb's-feltet til at finde det omregnede beløb i den ønskede valuta
                    decimal convertedAmount = amount * exchangeRate;

                    // Vi sætter teksten i resultat's-boksen til dette tal, og runder ned til 2 decimaler ved hjælp af formattering med "F2"
                    txtConvertedAmount.Text = convertedAmount.ToString("F2", CultureInfo.InvariantCulture);
                }
                else
                {
                    // Hvis en af de valgte elementer fra en af komboboksene er en null-værdi, sæt resultat's-feltets tekst til at være ingenting
                    txtConvertedAmount.Text = string.Empty;
                }
            }
            else
            {
                // Ligeledes, hvis værdien ikke kunne oversættes til datatypen decimal, oftest fordi der er bogstaver, flere kommaer eller lignende,
                // sæt resultat's-feltets tekst til at være ingenting
                txtConvertedAmount.Text = "Det indtastede beløb er ugyldigt";
            }
        }
    }
}