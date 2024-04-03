using TravelApp;

Console.WriteLine("\t\t\tWelcome in TravelApp!\n\n\n");

Main();

static void Main()
{
    var stack = new BasicStack<double>();
    stack.Push(5.7);
    stack.Push(33.4);
    stack.Push(111.9);

    double sum = 0.0;

    while (stack.Count > 0)
    {
        double item = stack.Pop();
        Console.WriteLine($"\tItem: {item}\n");
        sum += item;
    }

    Console.WriteLine($"\tSum: {sum}\n\n");
}

Console.WriteLine("\tPress any button to exit the program");
Console.ReadKey();
