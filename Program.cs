using TravelApp.Entities;
using TravelApp.Repositories;

var loopIsActiv = true;
var itemIsAdded = true;
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

App();

void App()
{
    WelcomeTheUser();
    SelectFromMainMenu();
    ShowEndingMessage();
}

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

void EventItemRemoved(object? sender, EntityBase item)
{
    TextColoring(ConsoleColor.Magenta, $"\t[{item.GetType().Name}] {item.FirstName} {item.LastName} with ID: {item.Id} was removed from the database.");
    action = auditRepository.Action = $"{item.GetType().Name} removed from the database";
    itemData = auditRepository.ItemData = $"[Delated] {item.GetType().Name} Id: {item.Id}, Name and surname: {item.FirstName} {item.LastName}";
}

void EventNewAuditEntry(object? sender, EntityBase item)
{
    TextColoring(ConsoleColor.Yellow, "\n\tNew entry in the AuditFile!\n");
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

void AddEmployee(IRepository<Employee> repository)
{
    InsertFullName();
    repository.Add(new Employee { FirstName = insertedName, LastName = insertedLastName });
    repository.Save();
    auditRepository.AddEntryToFile();
    auditRepository.SaveAuditFile();
}

void AddManager(IWriteRepository<Manager> repository)
{
    InsertFullName();
    var suffix = "[ Manager ]";
    repository.Add(new Manager { FirstName = $"{insertedName}", LastName = $"{insertedLastName}", Suffix = $"{suffix}" });
    repository.Save();
    auditRepository.AddEntryToFile();
    auditRepository.SaveAuditFile();
}

void AddNewEmployee(IRepository<Employee> repository)
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
                AddEmployee(employeeRepository);
                break;
            case "2":
                AddManager(employeeRepository);
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
            "> press '2' (+ press ENTER button to confirm) - to add a customer with GOLD membership\n\t" +
            "> press '3' (+ press ENTER button to confirm) - to add a customer with PLATINUM membership\n\t" +
            "> press 'q' (+ press ENTER button to confirm) - to quit and back to main menu\n");

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
            auditRepository.AddEntryToFile();
            repository.Save();
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

static void WelcomeTheUser()
{
    TextColoring(ConsoleColor.Green,
        "########################################################################################" +
        "\n________________________________________________________________________________________\n\n\t\t\t\t" +
        "Welcome in TravelApp!" +
        "\n________________________________________________________________________________________\n\n" +
        "########################################################################################\n\n");
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
                AddNewEmployee(employeeRepository);
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

//-------------------------------------------NOTES-------------------------------------------


//void EmployeeAdded(Employee employee)
//{
//    if (eventIsOn)
//    {
//        TextColoring(ConsoleColor.DarkGray, $"\tEmployee {employee.FirstName} added to the list of employees");
//    }
//}


//static void AddEmployees(IRepository<Employee> employeeRepository)
//{
//    var employees = new[]
//    {
//        new Employee { FirstName = "Jan", LastName = "Hoenchaim" },
//        new Employee { FirstName = "Łukasz", LastName = "Chudziński" },
//        new Employee { FirstName = "Monika", LastName = "Nowak" },
//        new Employee { FirstName = "Piotr", LastName = "Kowalski" }
//    };

//    employeeRepository.AddBatch(employees);
//}

//static void AddCustomers(IRepository<Customer> customerRepository)
//{
//    var customers = new[]
//    {
//        new Customer { FirstName = "Robert", LastName = "Markowski" },
//        new Customer { FirstName = "Ania", LastName = "Piotrkowska" },
//        new Customer { FirstName = "Kinga", LastName = "Tamowska" },
//        new Customer { FirstName = "Józef", LastName = "Skwarski" },
//        new Customer { FirstName = "Tadeusz", LastName = "Poniedzielski" },
//        new Customer { FirstName = "Iwona", LastName = "Myślińska" }
//    };

//    customerRepository.AddBatch(customers);
//}