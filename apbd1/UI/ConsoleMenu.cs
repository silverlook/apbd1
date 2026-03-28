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

    public class ConsoleMenu
    {
        private readonly UserService _userService;
        private readonly EquipmentService _equipmentService;
        private readonly RentalService _rentalService;
        private readonly ReportService _reportService;
        private readonly ConsoleDisplay _display;
        private readonly ConsoleInput _input;

        public ConsoleMenu(
            UserService userService,
            EquipmentService equipmentService,
            RentalService rentalService,
            ReportService reportService)
        {
            _userService = userService;
            _equipmentService = equipmentService;
            _rentalService = rentalService;
            _reportService = reportService;
            _display = new ConsoleDisplay();
            _input = new ConsoleInput();
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                _display.PrintHeader("UCZELNIANY SYSTEM WYPOZYCZALNI SPRZETU");
                Console.WriteLine("   1.  Dodaj uzytkownika");
                Console.WriteLine("   2.  Dodaj sprzet");
                Console.WriteLine("   3.  Wyswietl caly sprzet");
                Console.WriteLine("   4.  Wyswietl dostepny sprzet");
                Console.WriteLine("   5.  Wypozycz sprzet");
                Console.WriteLine("   6.  Zwroc sprzet");
                Console.WriteLine("   7.  Oznacz sprzet jako niedostepny");
                Console.WriteLine("   8.  Aktywne wypozyczenia uzytkownika");
                Console.WriteLine("   9.  Przeterminowane wypozyczenia");
                Console.WriteLine("   10. Raport stanu wypozyczalni");
                Console.WriteLine("   11. Lista uzytkownikow");
                Console.WriteLine("   0.  Wyjscie");
                Console.WriteLine();

                var choice = _input.ReadInt("Wybierz opcje", 0, 11);

                switch (choice)
                {
                    case 1: AddUser(); break;
                    case 2: AddEquipment(); break;
                    case 3: ShowAllEquipment(); break;
                    case 4: ShowAvailableEquipment(); break;
                    case 5: RentEquipment(); break;
                    case 6: ReturnEquipment(); break;
                    case 7: MarkEquipmentUnavailable(); break;
                    case 8: ShowUserActiveRentals(); break;
                    case 9: ShowOverdueRentals(); break;
                    case 10: ShowReport(); break;
                    case 11: ShowUsers(); break;
                    case 0: return;
                }
            }
        }

        private void AddUser()
        {
            _display.PrintHeader("DODAJ UZYTKOWNIKA");
            Console.WriteLine("  Typ:");
            Console.WriteLine("  1. Student");
            Console.WriteLine("  2. Pracownik");
            var type = _input.ReadInt("Wybierz typ", 1, 2);

            var firstName = _input.ReadLine("Imie");
            var lastName = _input.ReadLine("Nazwisko");

            User user;
            if (type == 1)
            {
                var studentId = _input.ReadLine("Nr albumu");
                var faculty = _input.ReadLine("Wydzial");
                user = new Student(firstName, lastName, studentId, faculty);
            }
            else
            {
                var department = _input.ReadLine("Dzial");
                var position = _input.ReadLine("Stanowisko");
                user = new Employee(firstName, lastName, department, position);
            }

            var result = _userService.AddUser(user);
            if (result.IsSuccess)
                _display.ShowSuccess($"Dodano: {user.FullName} [ID: {user.Id}]");
            else
                _display.ShowError(result.Error!);

            _input.WaitForKey();
        }

        private void AddEquipment()
        {
            _display.PrintHeader("DODAJ SPRZET");
            Console.WriteLine("  Typ:");
            Console.WriteLine("  1. Laptop");
            Console.WriteLine("  2. Projektor");
            Console.WriteLine("  3. Aparat/Kamera");
            var type = _input.ReadInt("Wybierz typ", 1, 3);

            var name = _input.ReadLine("Nazwa");

            Equipment equipment;
            switch (type)
            {
                case 1:
                    var ram = _input.ReadInt("RAM (GB)", 1, 256);
                    var storage = _input.ReadInt("Dysk (GB)", 1, 4096);
                    var processor = _input.ReadLine("Procesor");
                    equipment = new Laptop(name, ram, storage, processor);
                    break;
                case 2:
                    var lumens = _input.ReadInt("Jasnosc (lm)", 100, 20000);
                    var hasRemote = _input.ReadBool("Posiada pilot");
                    var resolution = _input.ReadLine("Rozdzielczosc (np. Full HD)");
                    equipment = new Projector(name, lumens, hasRemote, resolution);
                    break;
                default:
                    var mp = _input.ReadDouble("Rozdzielczosc (MP)");
                    var hasVideo = _input.ReadBool("Obsluga wideo");
                    var sensor = _input.ReadLine("Typ sensora");
                    equipment = new Camera(name, mp, hasVideo, sensor);
                    break;
            }

            var result = _equipmentService.AddEquipment(equipment);
            if (result.IsSuccess)
                _display.ShowSuccess($"Dodano: {equipment.GetTypeDescription()} '{equipment.Name}' [ID: {equipment.Id}]");
            else
                _display.ShowError(result.Error!);

            _input.WaitForKey();
        }

        private void ShowAllEquipment()
        {
            _display.ShowEquipmentList(_equipmentService.GetAll(), "Caly sprzet");
            _input.WaitForKey();
        }

        private void ShowAvailableEquipment()
        {
            _display.ShowEquipmentList(_equipmentService.GetAvailable(), "Dostepny sprzet");
            _input.WaitForKey();
        }

        private void RentEquipment()
        {
            _display.PrintHeader("WYPOZYCZ SPRZET");
            _display.ShowEquipmentList(_equipmentService.GetAvailable(), "Dostepny sprzet");
            var equipmentId = _input.ReadLine("ID sprzetu");

            _display.ShowUserList(_userService.GetAll());
            var userId = _input.ReadLine("ID uzytkownika");

            var result = _rentalService.RentEquipment(userId, equipmentId);
            if (result.IsSuccess)
            {
                var rental = result.Value!;
                _display.ShowSuccess(
                    $"Wypozyczono! ID wypozyczenia: {rental.Id}, termin zwrotu: {rental.DueDate:dd.MM.yyyy}");
            }
            else
            {
                _display.ShowError(result.Error!);
            }

            _input.WaitForKey();
        }

        private void ReturnEquipment()
        {
            _display.PrintHeader("ZWROC SPRZET");

            var activeRentals = _rentalService.GetActiveRentals();
            _display.ShowRentalList(activeRentals, _userService, _equipmentService);

            var rentalId = _input.ReadLine("ID wypozyczenia");
            var result = _rentalService.ReturnEquipment(rentalId);

            if (result.IsSuccess)
            {
                var rental = result.Value!;
                if (rental.LateFee > 0)
                    _display.ShowError($"Zwrocono z opoznieniem! Naliczono kare: {rental.LateFee:F2} PLN");
                else
                    _display.ShowSuccess("Sprzet zwrocony w terminie. Brak oplat.");
            }
            else
            {
                _display.ShowError(result.Error!);
            }

            _input.WaitForKey();
        }

        private void MarkEquipmentUnavailable()
        {
            _display.PrintHeader("OZNACZ SPRZET JAKO NIEDOSTEPNY");
            _display.ShowEquipmentList(_equipmentService.GetAll());

            var equipmentId = _input.ReadLine("ID sprzetu");
            var result = _equipmentService.MarkAsUnavailable(equipmentId);

            if (result.IsSuccess)
                _display.ShowSuccess("Sprzet oznaczony jako niedostepny (uszkodzenie/serwis).");
            else
                _display.ShowError(result.Error!);

            _input.WaitForKey();
        }

        private void ShowUserActiveRentals()
        {
            _display.PrintHeader("AKTYWNE WYPOZYCZENIA UZYTKOWNIKA");
            _display.ShowUserList(_userService.GetAll());

            var userId = _input.ReadLine("ID uzytkownika");
            var user = _userService.GetById(userId);

            if (user == null)
            {
                _display.ShowError($"Nie znaleziono uzytkownika o ID {userId}.");
            }
            else
            {
                var rentals = _rentalService.GetActiveRentalsForUser(userId);
                _display.PrintHeader($"Aktywne wypozyczenia: {user.FullName}");
                _display.ShowRentalList(rentals, _userService, _equipmentService);
            }

            _input.WaitForKey();
        }

        private void ShowOverdueRentals()
        {
            _display.PrintHeader("PRZETERMINOWANE WYPOZYCZENIA");
            var overdue = _rentalService.GetOverdueRentals();
            _display.ShowRentalList(overdue, _userService, _equipmentService);
            _input.WaitForKey();
        }

        private void ShowReport()
        {
            var report = _reportService.GenerateReport();
            _display.ShowReport(report);
            _input.WaitForKey();
        }

        private void ShowUsers()
        {
            _display.ShowUserList(_userService.GetAll());
            _input.WaitForKey();
        }
    }
}
