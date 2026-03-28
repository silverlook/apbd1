using apbd1.Models;
using apbd1.Models.Equipment;
using apbd1.Models.Users;
using apbd1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1.UI
{

    public class ConsoleDisplay
    {
        public void ShowEquipmentList(IReadOnlyList<Equipment> equipment, string title = "Sprzet")
        {
            PrintHeader(title);
            if (!equipment.Any())
            {
                Console.WriteLine("  Brak sprzetu.");
                return;
            }

            foreach (var item in equipment)
            {
                ConsoleColor statusColor;
                switch (item.Status)
                {
                    case EquipmentStatus.Available: statusColor = ConsoleColor.Green; break;
                    case EquipmentStatus.Rented: statusColor = ConsoleColor.Yellow; break;
                    case EquipmentStatus.Unavailable: statusColor = ConsoleColor.Red; break;
                    default: statusColor = ConsoleColor.White; break;
                }

                Console.Write($"  [{item.Id}] {item.GetTypeDescription(),-14} {item.Name,-28} Status: ");
                PrintColored($"{item.Status,-12}", statusColor);
                Console.WriteLine();
                Console.WriteLine($"         Spec: {item.GetSpecifications()}");
            }
        }

        public void ShowUserList(IReadOnlyList<User> users, string title = "Uzytkownicy")
        {
            PrintHeader(title);
            if (!users.Any())
            {
                Console.WriteLine("  Brak uzytkownikow.");
                return;
            }

            foreach (var user in users)
                Console.WriteLine($"  [{user.Id}] {user.FullName,-25} {user.GetRoleDescription()}");
        }

        public void ShowRentalList(
            IReadOnlyList<Rental> rentals,
            UserService userService,
            EquipmentService equipmentService)
        {
            if (!rentals.Any())
            {
                Console.WriteLine("  Brak wypozyczen.");
                return;
            }

            foreach (var rental in rentals)
            {
                var user = userService.GetById(rental.UserId);
                var equipment = equipmentService.GetById(rental.EquipmentId);

                Console.WriteLine($"  [{rental.Id}]");
                Console.WriteLine($"    Uzytkownik:  {user?.FullName ?? rental.UserId}");
                Console.WriteLine($"    Sprzet:      {equipment?.Name ?? rental.EquipmentId}");
                Console.WriteLine($"    Wypozyczono: {rental.RentalDate:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"    Termin:      {rental.DueDate:dd.MM.yyyy HH:mm}");

                if (rental.IsReturned)
                {
                    Console.WriteLine($"    Zwrocono:    {rental.ReturnDate:dd.MM.yyyy HH:mm}");
                    if (rental.LateFee > 0)
                    {
                        Console.Write("    Kara:        ");
                        PrintColored($"{rental.LateFee:F2} PLN", ConsoleColor.Red);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("    Kara:        brak");
                    }
                }
                else if (rental.IsOverdue)
                {
                    Console.Write("    Status:      ");
                    PrintColored("PRZETERMINOWANE", ConsoleColor.Red);
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("    Status:      ");
                    PrintColored("Aktywne", ConsoleColor.Yellow);
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }

        public void ShowReport(RentalReport report)
        {
            PrintHeader("RAPORT STANU WYPOZYCZALNI");
            Console.WriteLine($"  Wygenerowano: {report.GeneratedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine();

            Console.WriteLine("  SPRZET:");
            Console.WriteLine($"    Lacznie:        {report.TotalEquipment}");
            Console.Write("    Dostepny:       ");
            PrintColored(report.AvailableEquipment.ToString(), ConsoleColor.Green);
            Console.WriteLine();
            Console.Write("    Wypozyczony:    ");
            PrintColored(report.RentedEquipment.ToString(), ConsoleColor.Yellow);
            Console.WriteLine();
            Console.Write("    Niedostepny:    ");
            PrintColored(report.UnavailableEquipment.ToString(), ConsoleColor.Red);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("  UZYTKOWNICY:");
            Console.WriteLine($"    Lacznie:        {report.TotalUsers}");
            Console.WriteLine();

            Console.WriteLine("  WYPOZYCZENIA:");
            Console.WriteLine($"    Lacznie:        {report.TotalRentals}");
            Console.WriteLine($"    Aktywne:        {report.ActiveRentals}");
            Console.WriteLine($"    Zakonczone:     {report.CompletedRentals}");
            Console.Write("    Przeterminowane:");
            PrintColored($" {report.OverdueRentals}", report.OverdueRentals > 0 ? ConsoleColor.Red : ConsoleColor.White);
            Console.WriteLine();
            Console.Write("    Laczne kary:    ");
            PrintColored($"{report.TotalLateFees:F2} PLN", report.TotalLateFees > 0 ? ConsoleColor.Red : ConsoleColor.Green);
            Console.WriteLine();

            PrintSeparator();
        }

        public void ShowSuccess(string message)
        {
            Console.Write("  [OK]  ");
            PrintColored(message, ConsoleColor.Green);
            Console.WriteLine();
        }

        public void ShowError(string message)
        {
            Console.Write("  [ERR] ");
            PrintColored(message, ConsoleColor.Red);
            Console.WriteLine();
        }

        public void ShowInfo(string message)
        {
            Console.WriteLine($"  [i]   {message}");
        }

        public void PrintHeader(string title)
        {
            Console.WriteLine();
            PrintColored($"  === {title.ToUpper()} ===", ConsoleColor.Cyan);
            Console.WriteLine();
        }

        public void PrintSeparator()
        {
            Console.WriteLine(new string('-', 60));
        }

        private static void PrintColored(string text, ConsoleColor color)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = prev;
        }
    }
}
