using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.UI
{
    public class ConsoleInput
    {
        public string ReadLine(string prompt)
        {
            Console.Write($"  {prompt}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        public int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write($"  {prompt}: ");
                if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"  Wprowadz liczbe calkowita ({min}-{max}).");
            }
        }

        public bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write($"  {prompt} (t/n): ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "t" || input == "tak") return true;
                if (input == "n" || input == "nie") return false;
                Console.WriteLine("  Wprowadz 't' lub 'n'.");
            }
        }

        public double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write($"  {prompt}: ");
                var raw = Console.ReadLine()?.Replace(',', '.') ?? "";
                if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    return value;
                Console.WriteLine("  Wprowadz liczbe (np. 24.1).");
            }
        }

        public void WaitForKey(string message = "Nacisnij Enter, aby kontynuowac...")
        {
            Console.WriteLine($"\n  {message}");
            Console.ReadLine();
        }
    }
}
