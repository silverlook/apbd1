# Uczelniany System Wypożyczalni Sprzętu

Aplikacja konsolowa w C# (.NET 5) modelująca wypożyczalnię sprzętu na uczelni. Użytkownicy (studenci i pracownicy) mogą wypożyczać laptopy, projektory i aparaty fotograficzne, z egzekwowaniem reguł biznesowych (limity, kary za opóźnienia).

---

## Instrukcja uruchomienia

**Wymagania:** .NET 5 SDK lub nowszy (`dotnet --version`).

```bash
cd UniversityRental
dotnet run
```

Aplikacja uruchamia najpierw **scenariusz demonstracyjny** (automatyczny), a następnie przechodzi do **interaktywnego menu**. Wszystkie operacje dostępne są z menu numerowanego (0–11).

---

## Struktura katalogów

```
UniversityRental/
├── Models/
│   ├── Equipment/        — hierarchia sprzętu: Equipment (baza), Laptop, Projector, Camera
│   ├── Users/            — hierarchia użytkowników: User (baza), Student, Employee
│   └── Rental.cs         — encja wypożyczenia (kto, co, kiedy, kara)
├── Services/
│   ├── UserService.cs    — CRUD użytkowników
│   ├── EquipmentService.cs — CRUD sprzętu + zmiana statusu
│   ├── RentalService.cs  — logika wypożyczeń i zwrotów (egzekwuje reguły biznesowe)
│   └── ReportService.cs  — generowanie raportów i statystyk
├── UI/
│   ├── ConsoleDisplay.cs — wyświetlanie danych (kolorowe wyjście)
│   ├── ConsoleInput.cs   — wczytywanie danych od użytkownika
│   └── ConsoleMenu.cs    — menu główne, deleguje do serwisów
├── Configuration/
│   └── BusinessRules.cs  — wszystkie limity i stawki w jednym miejscu
├── Common/
│   └── Result.cs         — wzorzec Result<T> do jawnej obsługi błędów
├── App.cs                — composition root + scenariusz demonstracyjny
└── Program.cs            — wyłącznie punkt wejścia (3 linie)
```

---

## Decyzje projektowe

### Podział klas — dlaczego taki, a nie inny

Każdy serwis ma **jedną oś zmiany**:
- `EquipmentService` zmienia się, gdy zmienia się model sprzętu lub zasady jego przechowywania.
- `RentalService` zmienia się, gdy zmienia się logika biznesowa wypożyczeń.
- `ReportService` zmienia się, gdy zmienia się format lub zakres raportów.
- `ConsoleDisplay` zmienia się, gdy zmienia się wygląd interfejsu.
- `ConsoleInput` zmienia się, gdy zmienia się sposób wczytywania danych.

Gdyby połączyć np. `ConsoleDisplay` z `ConsoleMenu`, każda zmiana wyglądu wymagałaby modyfikacji klasy zawierającej logikę nawigacji — i odwrotnie.

### Dziedziczenie vs interfejs vs kompozycja

**Klasy `Equipment` i `User` — dziedziczenie (klasa abstrakcyjna):**
Zarówno `Laptop`, `Projector`, `Camera` jak i `Student`, `Employee` dzielą **stan** (Id, Name, Status / Id, FirstName, LastName) oraz **zachowanie** (generowanie Id, FullName). Interfejs zdefiniowałby tylko kontrakt — bez przechowywania stanu — co wymusiłoby duplikację pól w każdym podtypie. Użycie klasy abstrakcyjnej eliminuje tę duplikację i jest semantycznie poprawne: Laptop *jest* Equipment.

**`MaxActiveRentals` — abstrakcyjna właściwość w `User`:**
`RentalService` nie wie, czy ma do czynienia ze `Student` czy `Employee` — pyta tylko `user.MaxActiveRentals`. To eliminuje `if (user is Student) ... else if (user is Employee) ...` w logice biznesowej. Limit jest zdefiniowany w podklasie, ale czerpany z `BusinessRules` — więc zmiana liczby w jednym miejscu propaguje się automatycznie.

**Serwisy — kompozycja:**
`RentalService` przyjmuje `EquipmentService` i `UserService` w konstruktorze zamiast po nich dziedziczyć. Nie ma relacji „jest" — jest relacja „używa". Kompozycja daje niskie sprzężenie: serwisy można testować niezależnie, podmienić implementację bez zmiany `RentalService`.

### Koncentracja reguł biznesowych

Klasa `BusinessRules` (`Configuration/BusinessRules.cs`) zawiera **wyłącznie stałe**. Żaden limit ani stawka nie jest zahardkodowana gdziekolwiek indziej. Dzięki temu zmiana `StudentMaxActiveRentals` z 2 na 3 wymaga modyfikacji dokładnie jednej linii, a kompilator sprawdzi, czy wszystkie miejsca użycia są nadal spójne.

`Student.MaxActiveRentals` i `Employee.MaxActiveRentals` zwracają te stałe — są one dostępne przez polimorfizm bez switch/if w serwisach.

### Obsługa błędów — wzorzec Result<T>

Naruszenia reguł biznesowych (limit wypożyczeń, status sprzętu) to **przewidywalne porażki** — nie wyjątki. Użycie `Result<T>` zamiast rzucania wyjątków:
- Wymusza na wywołującym jawne sprawdzenie `result.IsSuccess` przed użyciem `result.Value`.
- Nie przerywa stosu wywołań — warstwa UI może spokojnie wyświetlić komunikat błędu i kontynuować.
- Sygnatura metody dokumentuje, że może ona zwrócić błąd (`Result<Rental>` vs `Rental`).

### Separacja warstw

Serwisy (`Services/`) **nie importują** przestrzeni nazw `UniversityRental.UI`. Interfejs konsolowy importuje serwisy, ale nie odwrotnie. Gdyby ktoś chciał dodać interfejs webowy lub API, serwisy pozostają niezmienione.

---

## Mapowanie SOLID

### Single Responsibility Principle (SRP)

| Klasa | Odpowiedzialność |
|-------|-----------------|
| `EquipmentService` | Zarządzanie kolekcją sprzętu i jego statusem |
| `RentalService` | Egzekwowanie reguł biznesowych wypożyczeń |
| `ReportService` | Generowanie agregatów i raportów |
| `ConsoleDisplay` | Renderowanie danych w konsoli |
| `ConsoleInput` | Wczytywanie i walidacja danych wejściowych |
| `BusinessRules` | Przechowywanie wszystkich stałych biznesowych |
| `Rental` | Reprezentacja pojedynczego wypożyczenia (dane + obliczenie kary) |

### Open/Closed Principle (OCP)

Dodanie nowego typu sprzętu (np. `Scanner`) wymaga tylko:
1. Stworzenia klasy dziedziczącej po `Equipment`.
2. Dodania case'a w menu (`ConsoleMenu.AddEquipment`).

Żaden serwis nie wymaga modyfikacji — `RentalService` operuje na `Equipment`, a `GetTypeDescription()` / `GetSpecifications()` to metody polimorficzne.

Analogicznie: dodanie nowego typu użytkownika (np. `Guest`) wymaga stworzenia klasy dziedziczącej po `User` z własnym `MaxActiveRentals` — bez zmiany `RentalService`.

### Liskov Substitution Principle (LSP)

`RentalService.RentEquipment` przyjmuje `string userId` i pobiera `User` z `UserService`. Niezależnie od tego, czy jest to `Student` czy `Employee`, kod działa poprawnie — bo `user.MaxActiveRentals` jest zdefiniowane w obydwu podklasach. Żadna podklasa nie narusza kontraktu klasy bazowej.

Analogicznie `Equipment` — `EquipmentService.GetAvailable()` filtruje listę `Equipment` bez wiedzy o konkretnych typach.

### Interface Segregation Principle (ISP)

`ConsoleInput` i `ConsoleDisplay` są rozdzielonymi klasami zamiast jednej `ConsoleUI`. Klasy, które tylko wyświetlają (np. `ReportService` nie wyświetla — przekazuje dane do `ConsoleDisplay`), nie muszą zależeć od metod wczytujących.

`IReadOnlyList<T>` jest używane jako typ zwracany przez serwisy zamiast `List<T>` — klienci dostają tylko kontrakt do odczytu, nie mają dostępu do mutujących metod listy.

### Dependency Inversion Principle (DIP)

`RentalService` zależy od **instancji** `EquipmentService` i `UserService` wstrzykniętych przez konstruktor (constructor injection). Composition root (`App`) jest odpowiedzialny za składanie zależności. W produkcyjnym systemie można by tu wprowadzić interfejsy `IEquipmentService`, `IUserService` i kontener IoC — bez zmiany logiki `RentalService`.

`User.MaxActiveRentals` to abstrakcja — `RentalService` nie zależy od konkretnych typów `Student`/`Employee`, lecz od abstrakcji zdefiniowanej w klasie bazowej.
