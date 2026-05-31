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
                        "\t- quit – quit the program\n" +
                        "\t- pick – pick the reminder and do sth with it\n" +
                        "\t- add – add reminder\n" +
                        "\t- list [notdone|overdue|all] – list reminders\n";

Console.WriteLine(greeting);

while (true)
{
    Console.Write("Enter command: ");
    var input = Console.ReadLine()!.Split(' ');
    switch (input[0])
    {
        case "quit":
            Console.WriteLine("Bye!");
            return;
        
        case "pick":
            await Endpoints.Pick(connectionString);
            break;
        
        case "add":
            await Endpoints.AddReminder(connectionString);
            break;
        
        case "list":
            if (input.Length == 1)
            {
                await Endpoints.List(connectionString, "all");
            }
            else
            {
                await Endpoints.List(connectionString, input[1..]);
            }
            break;
        
        default:
            Console.WriteLine("Unknown command");
            break;
    }
}