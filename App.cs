using System.Security.Cryptography.X509Certificates;
using TravelApp.DataProviders;
using TravelApp.Entities;
using TravelApp.Repositories;

namespace TravelApp;

public class App : IApp
{
    private readonly IRepository<Employee> _employeesRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IAudit _audit;
    private readonly IRepository<TravelOffer> _travelOffersRepository;
    private readonly ITravelOffersProvider _travelOffersProvider;

    public App(IRepository<Employee> employeesRepository, 
        IRepository<Customer> customerRepository, 
        IAudit audit, 
        IRepository<TravelOffer> travelOfferRepository,
        ITravelOffersProvider travelOffersProvider) 
    {
        _employeesRepository = employeesRepository;
        _customerRepository = customerRepository;
        _audit = audit;
        _travelOffersRepository = travelOfferRepository;
        _travelOffersProvider = travelOffersProvider;
    }

    public void Run()
    {
        var loopIsActiv = true;
        var employeeRepository = new JsonRepository<Employee>("Employees");
        var customerRepository = new JsonRepository<Customer>("Customers");
        string insertedName;
        string insertedLastName;
        string action = "[System report: Error!]";
        string itemData = "[System report: Error!]";
        var auditRepository = new JsonAuditRepository($"{action}", $"{itemData}");

        employeeRepository.ItemAdded += EventEmployeeAddedWithInfoAboutSender;
        employeeRepository.ItemRemoved += EventItemRemoved;
        employeeRepository.NewAuditEntry += EventNewAuditEntry;
        customerRepository.ItemAdded += EventCustomerAdded;
        customerRepository.ItemRemoved += EventItemRemoved;
        customerRepository.NewAuditEntry += EventNewAuditEntry;

        // Travel Offers: adding sample offers to list
        var offers = GenerateSampleTravelOffers();

        foreach (TravelOffer offer in offers)
        { 
        _travelOffersRepository.Add(offer);
        }

        // Write all sample offers to console
        foreach (var travelOffer in _travelOffersProvider.FilterOffersByPrice(2000))
        {
            Console.WriteLine(travelOffer);
        }

        WelcomeTheUser();
        SelectFromMainMenu();
        ShowEndingMessage();

        void EventEmployeeAddedWithInfoAboutSender(object? sender, Employee item)
        {
            TextColoring(ConsoleColor.Green, $"\n\t{item.GetType().Name} {item.Suffix} {item.FirstName} {item.LastName} added to the list of {item.GetType().Name}s with ID: {item.Id}");
            TextColoring(ConsoleColor.DarkGray, $"\tfrom {sender?.GetType().Name}\n");
            action = auditRepository.Action = $"new {item.GetType().Name} added";
            itemData = auditRepository.ItemData = $"Employee {item.Suffix}, Id: {item.Id}, Name and surname: {item.FirstName} {item.LastName}";
        }

        void EventCustomerAdded(object? sender, Customer item)
        {
            TextColoring(ConsoleColor.Green, $"\n\t{item.GetType().Name} {item.Suffix} {item.FirstName} {item.LastName} added to the list of {item.GetType().Name}s with ID: {item.Id}\n");
            action = auditRepository.Action = $"new {item.GetType().Name} added";
            itemData = auditRepository.ItemData = $"{item.GetType().Name} {item.Suffix}, Id: {item.Id}, Name and surname: {item.FirstName} {item.LastName}";
        }

        void EventItemRemoved(object? sender, PersonalEntitiesBase item)
        {
            TextColoring(ConsoleColor.Magenta, $"\t[{item.GetType().Name}] {item.FirstName} {item.LastName} with ID: {item.Id} was removed from the database.");
            action = auditRepository.Action = $"{item.GetType().Name} removed from the database";
            itemData = auditRepository.ItemData = $"[Delated] {item.GetType().Name} Id: {item.Id}, Name and surname: {item.FirstName} {item.LastName}";
        }

        void EventNewAuditEntry(object? sender, PersonalEntitiesBase item)
        {
            TextColoring(ConsoleColor.Yellow, $"\n\tNew entry about {item.GetType().Name} in the AuditFile!\n");
        }

        static void WriteAllFromEntityListToConsole(IReadRepository<IEntity> repository)
        {
            var items = repository.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        void WriteAllFromAuditFileToConsole()
        {
            var items = auditRepository.ReadAuditFile();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        string InsertFullName()
        {
            Console.WriteLine("\n\tEnter first name (press ENTER to confirm)\n");
            insertedName = Console.ReadLine();
            Console.WriteLine("\n\tEnter last name (press ENTER to confirm)\n");
            insertedLastName = Console.ReadLine();

            return insertedName + insertedLastName;
        }

        void AddNewEmployee(IRepository<Employee> repository)
        {
            InsertFullName();
            repository.Add(new Employee { FirstName = insertedName, LastName = insertedLastName });
            repository.Save();
            auditRepository.AddEntryToFile();
            auditRepository.SaveAuditFile();
        }

        void AddNewManager(IWriteRepository<Employee> repository)
        {
            InsertFullName();
            var suffix = "[ Manager ]";
            repository.Add(new Employee { FirstName = $"{insertedName}", LastName = $"{insertedLastName}", Suffix = $"{suffix}" });
            repository.Save();
            auditRepository.AddEntryToFile();
            auditRepository.SaveAuditFile();
        }

        void SelectTypeOfEmployee(IRepository<Employee> repository)
        {
            while (loopIsActiv)
            {
                TextColoring(ConsoleColor.DarkGreen,
                    "\tChoose one option:\n\n\t" +
                    "> press '1' (+ 'ENTER') - to add a new employee\n\t" +
                    "> press '2' (+ 'ENTER') - to add a new manager\n\t" +
                    "> press 'q' (+ 'ENTER') - to quit\n");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "q":
                        loopIsActiv = false;
                        break;
                    case "1":
                        AddNewEmployee(employeeRepository);
                        break;
                    case "2":
                        AddNewManager(employeeRepository);
                        break;
                    default:
                        TextColoring(ConsoleColor.Red, "\n\tIncorrect selection! Try again!\n");
                        break;
                }
            }

            loopIsActiv = true;
        }

        void AddNewCustomer(IRepository<Customer> repository)
        {
            while (loopIsActiv)
            {
                TextColoring(ConsoleColor.DarkGreen, "\n\tChoose one option:\n\n\t" +
                    "> press '1' (+ press ENTER button to confirm) - to add a new customer\n\t" +
                    "> press '2' (+ press ENTER button to confirm) - to add a new customer with GOLD membership\n\t" +
                    "> press '3' (+ press ENTER button to confirm) - to add a new customer with PLATINUM membership\n\t" +
                    "> press 'q' (+ press ENTER button to confirm) - to quit and back to the main menu\n");

                var input = Console.ReadLine();
                string suffix = null;

                switch (input)
                {
                    case "q":
                        loopIsActiv = false;
                        break;
                    case "1":
                        break;
                    case "2":
                        suffix = "[ GOLD ]";
                        break;
                    case "3":
                        suffix = "[ PLATINUM ]";
                        break;
                    default:
                        TextColoring(ConsoleColor.Red, "\n\tIncorrect selection! Try again.\n");
                        loopIsActiv = false;
                        break;
                }

                if (loopIsActiv)
                {
                    InsertFullName();
                    repository.Add(new Customer { FirstName = insertedName, LastName = insertedLastName, Suffix = suffix });
                    repository.Save();
                    auditRepository.AddEntryToFile();
                    auditRepository.SaveAuditFile();
                }
            }
            loopIsActiv = true;
        }

        void RemoveItemById<T>(IRepository<T> repository)
            where T : class, IEntity
        {
            bool isIdCorrect = true;

            do
            {
                Console.WriteLine($"\n\tEnter the ID of the person you want to remove from the database:\n\t(Press 'q' button + 'ENTER' button to quit and back to main menu)");

                var input = Console.ReadLine();
                if (input == "q")
                {
                    break;
                }

                try
                {
                    isIdCorrect = int.TryParse(input, out int id);
                    repository.Remove(repository.GetById(id));
                    auditRepository.AddEntryToFile();
                }
                catch (Exception exception)
                {
                    TextColoring(ConsoleColor.DarkRed, $"\n\tWarning! Exception catched:\n\t{exception.Message}\n");
                    TextColoring(ConsoleColor.Red, "\tThis ID is not existing! Try again!\n\t(Tip: View the list from the main menu to check the ID of the person you want to remove from the database)\n");
                }
                finally
                {
                    repository.Save();
                    auditRepository.SaveAuditFile();
                }
            } while (false);
        }

        void SelectFromMainMenu()
        {
            var loopIsActive = true;

            while (loopIsActive)
            {
                TextColoring(ConsoleColor.Cyan, "\n________________________________________________________________________________________\n" +
                    "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~  Main Menu  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n\t Select from the options:\n" +
                    "\n\t > press 1 - to show the list of employees" +
                    "\n\t > press 2 - to add a new employee (also a manager)" +
                    "\n\t > press 3 - to remove an employee" +
                    "\n\n\t > press 4 - to show the list of customers," +
                    "\n\t > press 5 - to add a new customer" +
                    "\n\t > press 6 - to remove a customer" +
                    "\n\n\t > press 7 - to read report about all actions on employees and customers" +
                    "\n\n\t > press q - to quit selection and exit from the app " +
                    "\n\n\n\t Press ENTER button to confirm your selection!" +
                    "\n________________________________________________________________________________________\n");

                var selectedOption = Console.ReadLine();

                switch (selectedOption)
                {
                    case "1":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t <<< List of employees >>>\n\n");
                        Console.WriteLine("\n---[employee ID:]------[personal data:]-------------------------------------------------\n");
                        WriteAllFromEntityListToConsole(employeeRepository);
                        break;
                    case "2":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t+ + Adding a new employee + +\n\n");
                        SelectTypeOfEmployee(employeeRepository);
                        loopIsActiv = true;
                        break;
                    case "3":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t- - Removing an employee - -");
                        RemoveItemById<Employee>(employeeRepository);
                        break;
                    case "4":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t <<< List of customers >>>\n\n");
                        Console.WriteLine("\n---[customer ID:]------[personal data:]-------------------------------------------------\n");
                        WriteAllFromEntityListToConsole(customerRepository);
                        break;
                    case "5":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t+ + Adding a new customer + +");
                        AddNewCustomer(customerRepository);
                        break;
                    case "6":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t- - Removing a customer - -");
                        RemoveItemById<Customer>(customerRepository);
                        break;
                    case "7":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t~+~-~[ Report from AuditFile ]~-~+~\n\n");
                        WriteAllFromAuditFileToConsole();
                        break;
                    case "q":
                        loopIsActive = false;
                        break;
                    default:
                        TextColoring(ConsoleColor.Red, " \n\tIncorrect selection. Try again!\n");
                        break;
                }
            }
        }

        static List<TravelOffer> GenerateSampleTravelOffers()
        {
            return new List<TravelOffer>
            {
                new TravelOffer
                {
                    Id = 1001,
                    Country = "Spain",
                    Region = "Andalusia",
                    City = "-",
                    Hotel = "SpAn",
                    PricePerHotelDay = 1110,
                    LowestPrice = 2100
                },
                new TravelOffer
                {
                    Id = 1002,
                    Country = "Spain",
                    Region = "Balearic islands",
                    City = "-",
                    Hotel = "SpBa",
                    PricePerHotelDay = 1210,
                    LowestPrice = 2200
                },
                new TravelOffer
                {
                    Id = 1003,
                    Country = "Spain",
                    Region = "Catalonia",
                    City = "-",
                    Hotel = "SpCa",
                    PricePerHotelDay = 1320,
                    LowestPrice = 2330
                },
                new TravelOffer
                {
                    Id = 1004,
                    Country = "Spain",
                    Region = "La Manga",
                    City = "-",
                    Hotel = "LaMa",
                    PricePerHotelDay = 2310,
                    LowestPrice = 3345
                },
                new TravelOffer
                {
                    Id = 1005,
                    Country = "Spain",
                    Region = null,
                    City = "Madrid",
                    Hotel = "Mad",
                    PricePerHotelDay = 4320,
                    LowestPrice = 7900
                },
                new TravelOffer
                {
                    Id = 1006,
                    Country = "Spain",
                    Region = "Valencia",
                    City = "-",
                    Hotel = "SVal",
                    PricePerHotelDay = 2430,
                    LowestPrice = 3180
                },
                new TravelOffer
                {
                    Id = 1014,
                    Country = "Greece",
                    Region = null,
                    City = "-",
                    Hotel = "Gr",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                new TravelOffer
                {
                    Id = 1013,
                    Country = "Bulgaria",
                    Region = null,
                    City = "-",
                    Hotel = "Bu",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                new TravelOffer
                {
                    Id = 1012,
                    Country = "Croatia",
                    Region = null,
                    City = "-",
                    Hotel = "Cr",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                new TravelOffer
                {
                    Id = 1011,
                    Country = "Italy",
                    Region = null,
                    City = "-",
                    Hotel = "It",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                new TravelOffer
                {
                    Id = 1007,
                    Country = "Turkey",
                    Region = null,
                    City = "Istanbul",
                    Hotel = "TuIs",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                new TravelOffer
                {
                    Id = 1008,
                    Country = "Turkey",
                    Region = null,
                    City = "Central Anatolia",
                    Hotel = "TCAn",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                new TravelOffer
                {
                    Id = 1009,
                    Country = "Turkey",
                    Region = "Turkish Riviera",
                    City = "Alanya",
                    Hotel = "TuTuRiAl",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                },
                 new TravelOffer
                {
                    Id = 1010,
                    Country = "Turkey",
                    Region = "Aegean Riviera",
                    City = "Bodrum",
                    Hotel = "TuAeRiBo",
                    PricePerHotelDay = 0,
                    LowestPrice = 0
                }
            };
        }

        static void WelcomeTheUser()
        {
            TextColoring(ConsoleColor.Green,
                "########################################################################################" +
                "\n________________________________________________________________________________________\n\n\t\t\t\t" +
                "Welcome in TravelApp!" +
                "\n________________________________________________________________________________________\n\n" +
                "########################################################################################\n\n");
        }

        static void ShowEndingMessage()
        {
            TextColoring(ConsoleColor.Green, "\n\tThanks for using TravelApp! See you!");
            TextColoring(ConsoleColor.DarkGray, "\n________________________________________________________________________________________\n\n\t\t\t" +
                "press any button to exit the application\n________________________________________________________________________________________\n");
        }

        static void TextColoring(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}