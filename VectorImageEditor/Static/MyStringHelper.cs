using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorImageEditor.Static
{
    public class MyStringHelper
    {
        public static string RimuoviSpazi(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace(" ", "");
        }

        public static string RimuoviMeno(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("-", "");
        }

        public static string SpostituisciSpaziConUnderscore(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace(" ", "_");
        }

        public static string Left(string input, int n)
        {
            // Gestione casi null o vuota
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Se N è maggiore o uguale alla lunghezza, ritorna l'intera stringa
            if (n >= input.Length)
                return input;

            // Se N è minore o uguale a 0, ritorna stringa vuota
            if (n <= 0)
                return string.Empty;

            // Ritorna i primi N caratteri
            return input.Substring(0, n);
        }

        public static string Right(string input, int n)
        {
            // Gestione casi null o vuota
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Se N è maggiore o uguale alla lunghezza, ritorna l'intera stringa
            if (n >= input.Length)
                return input;

            // Se N è minore o uguale a 0, ritorna stringa vuota
            if (n <= 0)
                return string.Empty;

            // Ritorna gli ultimi N caratteri
            return input.Substring(input.Length - n);
        }

        public static string Mid(string input, int start, int n)
        {
            // Gestione casi null o vuota
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Validazione posizione start
            if (start < 0)
                return string.Empty;

            // Se start è fuori dalla stringa
            if (start >= input.Length)
                return string.Empty;

            // Se n è minore o uguale a 0
            if (n <= 0)
                return string.Empty;

            // Calcola quanti caratteri effettivamente disponibili da start in poi
            int disponibili = input.Length - start;

            // Prendi al massimo n caratteri (o tutti quelli disponibili se n è maggiore)
            int daPrendere = Math.Min(n, disponibili);

            return input.Substring(start, daPrendere);
        }
        public static string TogliTipoControllo(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (Left(input, 3) == "tsb")
                return Right(input, input.Length - 3);
            else if (Left(input, 3) == "btn")
                return Right(input, input.Length - 3);
            else
                return string.Empty;
        }
    }
}
