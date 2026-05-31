using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NotCloud.Reminders.Persistence;
using Task = System.Threading.Tasks.Task;
using TaskModel = NotCloud.Reminders.Persistence.Task;

namespace NotCloud.Reminders;

public static class Endpoints
{
    public static async Task Pick(string connection)
    {
        Console.Write("Enter the name of the reminder: ");
        var name = Console.ReadLine();
        
        var options = new DbContextOptionsBuilder<RemindersContext>().UseSqlite(connection).Options;
        await using var db = new RemindersContext(options);
        
        var task = await db.Tasks.Where(t => t.Title == name).FirstOrDefaultAsync();
        if (task is null)
        {
            Console.WriteLine("The reminder doesn't exist.");
            return;
        }

        Console.WriteLine(task.Title);
        Console.Write("task(what to do)> ");
        var whatToDo = Console.ReadLine()!;

        switch (whatToDo)
        {
            case "complete":
                task.IsCompleted = true;
                await db.SaveChangesAsync();
                return;
            
            case "nothing":
                return;
            
            case "delete":
                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
                return;
            
            default:
                Console.WriteLine("Unknown command.");
                return;
        }
    }

    public static async Task AddReminder(string connection)
    {
        Console.Write("Enter the title of the reminder: ");
        var title = Console.ReadLine()!;
        
        Console.Write("Enter the description of the reminder: ");
        var description = Console.ReadLine()!;
        
        Console.Write("Enter the due date: ");
        var dueDate = DateTime.Parse(Console.ReadLine()!);

        var task = new TaskModel()
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            IsCompleted = false
        };
        
        var options = new DbContextOptionsBuilder<RemindersContext>().UseSqlite(connection).Options;
        await using var db = new RemindersContext(options);

        try
        {
            await db.Tasks.AddAsync(task);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            Console.WriteLine("The reminder with this title already exists.");
            return;
        }

        Console.WriteLine("The reminder is successfully added.");
    }

    public static async Task List(string connection, params string[] filters)
    {
        var options = new DbContextOptionsBuilder<RemindersContext>().UseSqlite(connection).Options;
        await using var db = new RemindersContext(options);

        IQueryable<TaskModel> query = db.Tasks;
        if (filters.Contains("notdone"))
        {
            query = query.Where(t => !t.IsCompleted);
        }
        if (filters.Contains("overdue"))
        {
            query = query.Where(t => t.DueDate < DateTime.Today);
        }
        
        var tasks = await query.ToListAsync();
        foreach (var task in tasks)
        {
            Console.WriteLine(task);
        }
    }
}