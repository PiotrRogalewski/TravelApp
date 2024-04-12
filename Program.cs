using TravelApp.Data;
using TravelApp.Entities;
using TravelApp.Repositories;

TextColoring(ConsoleColor.DarkGreen, "########################################################################################\n________________________________________________________________________________________\n\n\t\t\t\t" +
    "Welcome in TravelApp!" +
    "\n________________________________________________________________________________________\n\n########################################################################################\n\n");

var employeeRepository = new SqlRepository<Employee>(new TravelAppDbContext());
var customerRepository = new SqlRepository<Customer>(new TravelAppDbContext());

App();

void App()
{
    AddEmployees(employeeRepository);
    AddManager(employeeRepository);
    AddCustomers(customerRepository);
    selectFromMenu();
}

static void AddEmployees(IRepository<Employee> employeeRepository)
{
    employeeRepository.Add(new Employee { FirstName = "Jan", LastName = "Hoenchaim" });
    employeeRepository.Add(new Employee { FirstName = "Łukasz", LastName = "Chudziński" });
    employeeRepository.Add(new Employee { FirstName = "Monika", LastName = "Nowak" });
    employeeRepository.Add(new Employee { FirstName = "Piotr", LastName = "Kowalski" });
    employeeRepository.Save();
}

static void AddManager(IWriteReposoitory<Manager> managerRepository)
{
    managerRepository.Add(new Manager { FirstName = "Paweł", LastName = "Wilczur" });
    managerRepository.Save();
}

static void AddCustomers(IWriteReposoitory<Customer> customerRepository)
{
    customerRepository.Add(new Customer { FirstName = "Robert", LastName = "Markowski" });
    customerRepository.Add(new Customer { FirstName = "Ania", LastName = "Piotrkowska" });
    customerRepository.Add(new Customer { FirstName = "Kinga", LastName = "Tamowska" });
    customerRepository.Add(new Customer { FirstName = "Józef", LastName = "Skwarski" });
    customerRepository.Add(new Customer { FirstName = "Tadeusz", LastName = "Poniedzielski" });
    customerRepository.Add(new Customer { FirstName = "Iwona", LastName = "Myślińska" });
    customerRepository.Save();
}

static void WriteAllToConsole(IReadRepository<IEntity> repository)
{
    var items = repository.GetAll();
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}

void selectFromMenu()
{
    while (true)
    {
        TextColoring(ConsoleColor.Yellow, "________________________________________________________________________________________\n\t\t\t\t~~~~~~~~~~~~~~~~~~~~~\n\t\t\t\t\t" +
            "Menu\n\n\t Select from the options:\n" +
            "\n\t > press 1 - to select list of employees" +
            "\n\t > press 2 - to select list of customers," +
            "\n\t > press q - to quit the menu and exit from the app " +
            "\n\n\t Press ENTER button to confirm your selection!" +
            "\n________________________________________________________________________________________\n");

        var selectedOption = Console.ReadLine();

        if (selectedOption == "1")
        {
            TextColoring(ConsoleColor.Green, "\t\t\t <<< List of employees >>>\n\n");
            WriteAllToConsole(employeeRepository);
        }
        else if (selectedOption == "2")
        {
            TextColoring(ConsoleColor.Green, "\t\t\t <<< List of customers >>>\n\n");
            WriteAllToConsole(customerRepository);
        }
        else if (selectedOption == "q")
        {
            break;
        }
        else
        {
            TextColoring(ConsoleColor.Red, " \n Incorrect selection. Try again!\n");
        }
    }
}
static void TextColoring(ConsoleColor color, string text)
{
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ResetColor();
}

TextColoring(ConsoleColor.DarkGray, "\n\n________________________________________________________________________________________\n\n\t\t\tpress any button to exit the application\n________________________________________________________________________________________\n");
