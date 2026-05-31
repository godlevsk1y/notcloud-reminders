using Microsoft.Extensions.Configuration;
using NotCloud.Reminders;

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
configurationBuilder.AddJsonFile("appsettings.json");

var configuration = configurationBuilder.Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("No connection string found");
    return;
}

const string greeting = "Hey! This is NotCloud!\n" +
                        "This awesome reminder service without seamless cloud sync " +
                        "allows you to manage your reminders!\n" +
                        "Here are some commands: \n" +
                        "\t- 0 – quit the program\n" +
                        "\t- 1 – search the reminder by title\n" +
                        "\t- 2 – add reminder\n" +
                        "\t- 3 – list all reminders\n" +
                        "\t- 4 – list all reminders that are not done yet" +
                        "\t- 5 – list all reminders that are overdue\n\n";

Console.WriteLine(greeting);

while (true)
{
    Console.Write("Enter command: ");
    var input = Console.ReadLine()!;
    switch (input)
    {
        case "0":
            Console.WriteLine("Bye!");
            return;
        
        case "1":
            await Endpoints.Search(connectionString);
            break;
        
        case "2":
            await Endpoints.AddReminder(connectionString);
            break;
        
        case "3":
            await Endpoints.ListAll(connectionString);
            break;
        
        case "4":
            await Endpoints.ListNotDone(connectionString);
            break;
        
        case "5":
            await Endpoints.ListOverdue(connectionString);
            break;
        
        default:
            Console.WriteLine("Unknown command");
            break;
    }
}