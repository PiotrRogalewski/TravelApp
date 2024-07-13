using TravelApp.DataProviders;
using TravelApp.Entities;
using TravelApp.Repositories;

namespace TravelApp;

public class App : IApp
{
    protected readonly IRepository<Employee> _employeesRepository;
    protected readonly IRepository<Customer> _customersRepository;
    private readonly IAudit _auditRepository;
    private readonly IRepository<TravelOffer> _travelOffersRepository;
    private readonly ITravelOffersProvider _travelOffersProvider;

    public App(IRepository<Employee> employeesRepository,
        IRepository<Customer> customerRepository,
        IAudit audit,
        IRepository<TravelOffer> travelOfferRepository,
        ITravelOffersProvider travelOffersProvider)
    {
        _employeesRepository = employeesRepository;
        _customersRepository = customerRepository;
        _auditRepository = audit;
        _travelOffersRepository = travelOfferRepository;
        _travelOffersProvider = travelOffersProvider;
    }

    public void Run()
    {
        var loopIsActiv = true;
        var employeeRepository = new ListRepository<Employee>();
        var customerRepository = new ListRepository<Customer>();
        var travelOfferRepository = new ListRepository<TravelOffer>();
        var auditRepository = new AuditRepository();
        string insertedName;
        string insertedLastName;
        string action = "[System report: Error!]";
        string itemData = "[System report: Error!]";
        employeeRepository.ItemAdded += EventPersonAdded;
        employeeRepository.ItemRemoved += EventPersonRemoved;
        employeeRepository.ItemsCounted += EventItemsCounted;
        employeeRepository.NewAuditEntry += EventNewAuditEntry;
        customerRepository.ItemAdded += EventPersonAdded;
        customerRepository.ItemRemoved += EventPersonRemoved;
        customerRepository.ItemsCounted += EventItemsCounted;
        customerRepository.NewAuditEntry += EventNewAuditEntry;


        WelcomeUser();
        SelectFromMainMenu();
        ShowEndingMessage();

        // ______________________________________________________________________________________________________________________________________________________________________________________________________________________________
        // Methods for travel offer options:
        void ShowOffersFilteredByPrice()
        {
            // Travel Offers - adding sample offers to the list in program memory
            var offers = GenerateSampleTravelOffers();
            travelOfferRepository.GetJsonFileName("offers_you_are_looking_for");

            foreach (TravelOffer offer in offers)
            {
                _travelOffersRepository.Add(offer);
            }

            while (true)
            {
                // Providing the maximum price for the travel offer requested by the user
                TextColoring(ConsoleColor.DarkGreen, "\n\tEnter the maximum price of the travel offer you are interested in. Use only numbers.\n\tDo not use words, letters or signs! You can use decimal value - for example: \t 4899,99 \n\n\n");

                var input = Console.ReadLine();
                Console.WriteLine("\n");
                if (decimal.TryParse(input, out decimal inputInDecimalValue))
                {
                    // Write all matching sample offers to console
                    foreach (var travelOffer in _travelOffersProvider.DisplayAllCountriesSeparately(/*inputInDecimalValue*/))
                    {
                        TextColoring(ConsoleColor.Yellow, $"\t{travelOffer}");
                        TextColoring(ConsoleColor.DarkCyan, "\n\t---------------------------------------------------------------------------------------------------------------\n");
                    }

                    TextColoring(ConsoleColor.DarkGray, $"\t* - Price for one day of stay for one person including flight in both directions, does not include meals.\n");
                        TextColoring(ConsoleColor.DarkCyan, $"\n\tPress Enter to quit (back to the main menu)");
                    var quitInput = Console.ReadLine();

                    break;
                }
                else
                {
                    TextColoring(ConsoleColor.Red, "\n\tIncorect value!\n\tRemeber! Use only numbers. Don't use words, letters, or signs.\n");
                    continue;
                }
            }
        }

        // ______________________________________________________________________________________________________________________________________________________________________________________________________________________________
        // Events:

        void EventPersonAdded(object? sender, PersonalEntitiesBase person)
        {
            TextColoring(ConsoleColor.Green, $"\n\t{person.GetType().Name} {person.Suffix} {person.FirstName} {person.LastName} added to the list of {person.GetType().Name}s with ID: {person.Id}");
            action = auditRepository.Action = $"new {person.GetType().Name} added";
            itemData = auditRepository.ItemData = $"{person.GetType().Name} {person.Suffix}, Id: {person.Id}, Name and surname: {person.FirstName} {person.LastName}";
            TextColoring(ConsoleColor.DarkGray, $"\t(Message from:\t{sender?.GetType().Name})");
        }

        void EventPersonRemoved(object? sender, PersonalEntitiesBase person)
        {
            TextColoring(ConsoleColor.Magenta, $"\n\t[{person.GetType().Name}] {person.FirstName} {person.LastName} with ID: {person.Id} was removed from the file.");
            action = auditRepository.Action = $"{person.GetType().Name} removed from the file";
            itemData = auditRepository.ItemData = $"[Delated] {person.GetType().Name} Id: {person.Id}, Name and surname: {person.FirstName} {person.LastName}";
        }

        void EventItemsCounted(object? sender, IEntity item)
        {
            TextColoring(ConsoleColor.Cyan, $"\n\tThe list of {item.GetType().Name}s contains {item.ItemCounter} items");
        }

        void EventNewAuditEntry(object? sender, PersonalEntitiesBase item)
        {
            TextColoring(ConsoleColor.Yellow, $"\tNew entry about {item.GetType().Name} in the AuditFile!\n");
        }

        // ______________________________________________________________________________________________________________________________________________________________________________________________________________________________
        // The methods for options in the Main Menu:

        void WriteAllFromJsonFileToConsole(IReadRepository<IEntity> repository)
        {
            employeeRepository.GetJsonFileName("Employees");
            customerRepository.GetJsonFileName("Customers");
            travelOfferRepository.GetJsonFileName("Travel_Offers");

            var items = repository.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        static void WriteAllFromAuditFileToConsole(IAudit repository)
        {
            var items = repository.ReadAuditFile();
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
            employeeRepository.GetJsonFileName("Employees");
            InsertFullName();
            repository.Add(new Employee { FirstName = insertedName, LastName = insertedLastName });
            repository.Save();
            auditRepository.AddEntryToAuditFile();
            auditRepository.SaveAuditFile();
        }

        void AddNewManager(IWriteRepository<Employee> repository)
        {
            employeeRepository.GetJsonFileName("Employees");
            InsertFullName();
            var suffix = "[ Manager ]";
            repository.Add(new Employee { FirstName = $"{insertedName}", LastName = $"{insertedLastName}", Suffix = $"{suffix}" });
            repository.Save();
            auditRepository.AddEntryToAuditFile();
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
                    repository.GetJsonFileName("Customers");
                    InsertFullName();
                    repository.Add(new Customer { FirstName = insertedName, LastName = insertedLastName, Suffix = suffix });
                    repository.Save();
                    auditRepository.AddEntryToAuditFile();
                    auditRepository.SaveAuditFile();
                }
            }
            loopIsActiv = true;
        }

        void RemoveItemById<T>(IRepository<T> repository)
            where T : class, IEntity
        {
            do
            {
                Console.WriteLine($"\n\tEnter the ID of the person you want to remove from the file:\n\t(Press 'q' button + 'ENTER' button to quit and back to main menu)");

                var input = Console.ReadLine();
                if (input == "q")
                {
                    break;
                }

                try
                {
                    int.TryParse(input, out int id);
                    repository.Remove(repository.GetById(id));
                    auditRepository.AddEntryToAuditFile();
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

        // ______________________________________________________________________________________________________________________________________________________________________________________________________________________________
        // This method launches the main menu (the core of the upper  'Run'  method):
        void SelectFromMainMenu()
        {
            var loopIsActive = true;

            while (loopIsActive)
            {
                TextColoring(ConsoleColor.Cyan, "\n_________________________________________________________________________________________\n" +
                    "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~  Main Menu  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n\t Select from the options:\n" +
                    "\n\t > press '1' - to show the list of employees" +
                    "\n\t > press '2' - to add a new employee (also a manager)" +
                    "\n\t > press '3' - to remove an employee" +
                    "\n\t > press '4' - to show the list of customers," +
                    "\n\t > press '5' - to add a new customer" +
                    "\n\t > press '6' - to remove a customer" +
                    "\n\t > press '7' - to read report about all actions on employees and customers" +
                    "\n\n\t > press '8' - to show the travel offers filtered by the price \n\t   (you will be able to choose this price in the next step)" +

                    "\n\n\t > press q - to quit selection and exit from the app " +
                    "\n\n\n\t Press ENTER button to confirm your selection!" +
                    "\n_________________________________________________________________________________________\n");

                var selectedOption = Console.ReadLine();

                switch (selectedOption)
                {
                    case "1":
                        TextColoring(ConsoleColor.DarkGreen, "\t\t\t <<< List of employees >>>\n\n");
                        Console.WriteLine("\n---[employee ID:]------[personal data:]-------------------------------------------------\n");
                        WriteAllFromJsonFileToConsole(employeeRepository);
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
                        WriteAllFromJsonFileToConsole(customerRepository);
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
                        WriteAllFromAuditFileToConsole(auditRepository);
                        break;
                    case "8":
                        ShowOffersFilteredByPrice();
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
    }

    // ______________________________________________________________________________________________________________________________________________________________________________________________________________________________
    // This method generates sample travel offers, this data is temporary (no file)
    static List<TravelOffer> GenerateSampleTravelOffers()
    {
        return new List<TravelOffer>
            {
                new TravelOffer
                {
                    Country = "Spain\t",
                    Region = "Andalusia",
                    City = "city10",
                    Hotel = "SpAn",
                    PricePerHotelDay = 1110,
                    LowestPrice = 2100u
                },
                new TravelOffer
                {
                    Country = "Spain\t",
                    Region = "Balearic islands",
                    City = "city9",
                    Hotel = "SpBa",
                    PricePerHotelDay = 1210,
                    LowestPrice = 2200
                },
                new TravelOffer
                {
                    Country = "Spain\t",
                    Region = "Catalonia",
                    City = "city8",
                    Hotel = "SpCa",
                    PricePerHotelDay = 1320,
                    LowestPrice = 2330
                },
                new TravelOffer
                {
                    Country = "Spain\t",
                    Region = "La Manga",
                    City = "city7",
                    Hotel = "LaMa",
                    PricePerHotelDay = 2310,
                    LowestPrice = 3345
                },
                new TravelOffer
                {
                    Country = "Spain\t",
                    Region = null,
                    City = "Madrid",
                    Hotel = "Mad",
                    PricePerHotelDay = 4320,
                    LowestPrice = 7900
                },
                new TravelOffer
                {
                    Country = "Spain\t",
                    Region = "Valencia",
                    City = "city6",
                    Hotel = "SVal",
                    PricePerHotelDay = 2430,
                    LowestPrice = 3180
                },
                new TravelOffer
                {
                    Country = "Greece\t",
                    Region = null,
                    City = "city5",
                    Hotel = "Gr",
                    PricePerHotelDay = 1000,
                    LowestPrice = 3500
                },
                new TravelOffer
                {
                    Country = "Bulgaria",
                    Region = null,
                    City = "city4",
                    Hotel = "Bu",
                    PricePerHotelDay = 1000,
                    LowestPrice = 1500
                },
                new TravelOffer
                {
                    Country = "Croatia\t",
                    Region = null,
                    City = "city3",
                    Hotel = "Cr",
                    PricePerHotelDay = 1000,
                    LowestPrice = 2700
                },
                new TravelOffer
                {
                    Country = "Italy\t",
                    Region = null,
                    City = "city2",
                    Hotel = "It",
                    PricePerHotelDay = 1000,
                    LowestPrice = 3100
                },
                new TravelOffer
                {
                    Country = "Turkey\t",
                    Region = null,
                    City = "Istanbul",
                    Hotel = "TuIs",
                    PricePerHotelDay = 1000,
                    LowestPrice = 2900
                },
                new TravelOffer
                {
                    Country = "Turkey\t",
                    Region = "Central Anatolia",
                    City = "city1",
                    Hotel = "TCAn",
                    PricePerHotelDay = 1000,
                    LowestPrice = 2800
                },
                new TravelOffer
                {
                    Country = "Turkey\t",
                    Region = "Turkish Riviera",
                    City = "Alanya",
                    Hotel = "TuTuRiAl",
                    PricePerHotelDay = 1000,
                    LowestPrice = 3000
                },
                new TravelOffer
                {
                    Country = "Turkey\t",
                    Region = "Aegean Riviera",
                    City = "Bodrum",
                    Hotel = "TuAeRiBo",
                    PricePerHotelDay = 1000,
                    LowestPrice = 3200
                }
            };
    }

    static void WelcomeUser()
    {
        TextColoring(ConsoleColor.Green,
            "\n_=*****===*****===*****===*****===*****===*****===*****===*****===*****===*****===*****=_" +
            "\n_________________________________________________________________________________________\n\n\t\t\t\t" +
            "  Welcome in TravelApp!" +
            "\n_________________________________________________________________________________________\n" +
            "\n_=*****===*****===*****===*****===*****===*****===*****===*****===*****===*****===*****=_");
    }

    static void ShowEndingMessage()
    {
        TextColoring(ConsoleColor.Green, "\n\tThanks for using TravelApp! See you!");
        TextColoring(ConsoleColor.DarkGray, "\n_________________________________________________________________________________________\n\n\t\t\t" +
            "press any button to exit the application\n_________________________________________________________________________________________\n");
    }

    static void TextColoring(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}