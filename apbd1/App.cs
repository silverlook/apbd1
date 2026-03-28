using apbd1.Models.Equipment;
using apbd1.Models.Users;
using apbd1.Services;
using apbd1.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd1
{
    public class App
    {
        private readonly UserService _userService;
        private readonly EquipmentService _equipmentService;
        private readonly RentalService _rentalService;
        private readonly ReportService _reportService;
        private readonly ConsoleMenu _menu;
        private readonly ConsoleDisplay _display;

        public App()
        {
            _userService = new UserService();
            _equipmentService = new EquipmentService();
            _rentalService = new RentalService(_equipmentService, _userService);
            _reportService = new ReportService(_equipmentService, _userService, _rentalService);
            _menu = new ConsoleMenu(_userService, _equipmentService, _rentalService, _reportService);
            _display = new ConsoleDisplay();
        }

        public void Run()
        {
            RunDemo();

            Console.WriteLine();
            _display.ShowInfo("Scenariusz zakonczony. Nacisnij Enter, aby przejsc do menu interaktywnego...");
            Console.ReadLine();

            _menu.Run();
        }

        // -------------------------------------------------------------------------
        // SCENARIUSZ DEMONSTRACYJNY
        // -------------------------------------------------------------------------
        private void RunDemo()
        {
            Console.Clear();
            _display.PrintHeader("SCENARIUSZ DEMONSTRACYJNY — UCZELNIANY SYSTEM WYPOZYCZALNI");

            // ── KROK 1: Dodawanie sprzetu ─────────────────────────────────────────
            _display.PrintHeader("KROK 1: Dodawanie sprzetu (2x Laptop, 2x Projektor, 2x Kamera)");

            var laptop1 = new Laptop("Dell XPS 15", 16, 512, "Intel Core i7-12700H");
            var laptop2 = new Laptop("MacBook Pro 14", 32, 1024, "Apple M2 Pro");
            var projector1 = new Projector("Epson EB-X51", 3800, true, "XGA");
            var projector2 = new Projector("BenQ MH550", 3500, false, "Full HD");
            var camera1 = new Camera("Canon EOS 850D", 24.1, true, "APS-C CMOS");
            var camera2 = new Camera("Sony Alpha A7 III", 24.2, true, "Full-frame BSI CMOS");

            foreach (var eq in new Equipment[] { laptop1, laptop2, projector1, projector2, camera1, camera2 })
            {
                _equipmentService.AddEquipment(eq);
                _display.ShowSuccess($"[{eq.Id}] {eq.GetTypeDescription(),-14} {eq.Name}  |  {eq.GetSpecifications()}");
            }

            // ── KROK 2: Dodawanie uzytkownikow ───────────────────────────────────
            _display.PrintHeader("KROK 2: Dodawanie uzytkownikow (2x Student, 2x Pracownik)");

            var student1 = new Student("Anna", "Kowalska", "123456", "Informatyki");
            var student2 = new Student("Marek", "Nowak", "234567", "Elektroniki");
            var employee1 = new Employee("Jan", "Wisniewski", "Katedra IT", "Profesor");
            var employee2 = new Employee("Ewa", "Zielinska", "Administracja", "Specjalista");

            foreach (var u in new User[] { student1, student2, employee1, employee2 })
            {
                _userService.AddUser(u);
                _display.ShowSuccess($"[{u.Id}] {u.FullName,-22}  {u.GetRoleDescription()}");
            }

            // ── KROK 3: Poprawne wypozyczenia ─────────────────────────────────────
            _display.PrintHeader("KROK 3: Poprawne wypozyczenia");

            var r1 = _rentalService.RentEquipment(student1.Id, laptop1.Id);
            _display.ShowSuccess($"Anna wypozyczyla '{laptop1.Name}'  [ID: {r1.Value!.Id}, termin: {r1.Value.DueDate:dd.MM.yyyy}]");

            var r2 = _rentalService.RentEquipment(student1.Id, camera1.Id);
            _display.ShowSuccess($"Anna wypozyczyla '{camera1.Name}'  [ID: {r2.Value!.Id}, termin: {r2.Value.DueDate:dd.MM.yyyy}]");

            var r3 = _rentalService.RentEquipment(employee1.Id, projector1.Id);
            _display.ShowSuccess($"Jan Wisniewski wypozyczyl '{projector1.Name}'  [ID: {r3.Value!.Id}]");

            // ── KROK 4: Naruszenia regul biznesowych ──────────────────────────────
            _display.PrintHeader("KROK 4: Naruszenia regul biznesowych (oczekiwane bledy)");

            Console.WriteLine();
            Console.WriteLine("  [Proba 1] Wypozyczenie sprzetu zajętego (laptop1 jest u Anny):");
            var fail1 = _rentalService.RentEquipment(student2.Id, laptop1.Id);
            _display.ShowError(fail1.Error!);

            Console.WriteLine();
            Console.WriteLine("  [Proba 2] Przekroczenie limitu studenta (Anna ma juz 2 aktywne):");
            var fail2 = _rentalService.RentEquipment(student1.Id, laptop2.Id);
            _display.ShowError(fail2.Error!);

            Console.WriteLine();
            Console.WriteLine("  [Proba 3] Wypozyczenie sprzetu niedostepnego (kamera2 idzie do serwisu):");
            _equipmentService.MarkAsUnavailable(camera2.Id);
            var fail3 = _rentalService.RentEquipment(employee2.Id, camera2.Id);
            _display.ShowError(fail3.Error!);

            // ── KROK 5: Zwrot w terminie ──────────────────────────────────────────
            _display.PrintHeader("KROK 5: Zwrot w terminie");

            var ret1 = _rentalService.ReturnEquipment(r3.Value!.Id);
            _display.ShowSuccess($"Jan Wisniewski zwrocil '{projector1.Name}' — brak kary ({ret1.Value!.LateFee:F2} PLN).");

            // ── KROK 6: Opozniony zwrot z kara ───────────────────────────────────
            _display.PrintHeader("KROK 6: Opozniony zwrot (termin minal 5 dni temu => kara 50,00 PLN)");

            // Symulacja: termin zwrotu ustawiony na 5 dni wstecz
            var lateRental = _rentalService.RentEquipment(
                student2.Id, projector2.Id,
                dueDate: DateTime.Now.AddDays(-5));

            _display.ShowInfo(
                $"Marek wypozyczyl '{projector2.Name}' z terminem {lateRental.Value!.DueDate:dd.MM.yyyy} (juz po terminie).");

            var lateReturn = _rentalService.ReturnEquipment(lateRental.Value!.Id, DateTime.Now);
            _display.ShowError(
                $"Opozniony zwrot! Kara: {lateReturn.Value!.LateFee:F2} PLN (5 dni x {Configuration.BusinessRules.LateFeePerDay:F2} PLN/dzien)");

            // ── KROK 7: Raport koncowy ────────────────────────────────────────────
            _display.PrintHeader("KROK 7: Raport koncowy stanu wypozyczalni");
            _display.ShowReport(_reportService.GenerateReport());

            _display.ShowInfo("Aktywne wypozyczenia Anny (2 sztuki):");
            _display.ShowRentalList(
                _rentalService.GetActiveRentalsForUser(student1.Id),
                _userService, _equipmentService);
        }
    }
}
