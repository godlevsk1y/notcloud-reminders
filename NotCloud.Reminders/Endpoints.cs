using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NotCloud.Reminders.Persistence;
using Task = System.Threading.Tasks.Task;
using TaskModel = NotCloud.Reminders.Persistence.Task;

namespace NotCloud.Reminders;

public static class Endpoints
{
    public static async Task Search(string connection)
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

    public static async Task ListAll(string connection)
    {
        var options = new DbContextOptionsBuilder<RemindersContext>().UseSqlite(connection).Options;
        await using var db = new RemindersContext(options);
        
        var tasks = await db.Tasks.ToListAsync();
        foreach (var task in tasks)
        {
            Console.WriteLine(task);
        }
    }
    public static async Task ListNotDone(string connection)
    {
        var options = new DbContextOptionsBuilder<RemindersContext>().UseSqlite(connection).Options;
        await using var db = new RemindersContext(options);
        
        var tasks = await db.Tasks.Where(t => !t.IsCompleted).ToListAsync();
        foreach (var task in tasks)
        {
            Console.WriteLine(task);
        }
    }
    public static async Task ListOverdue(string connection)
    {
        var options = new DbContextOptionsBuilder<RemindersContext>().UseSqlite(connection).Options;
        await using var db = new RemindersContext(options);
        
        var tasks = await db.Tasks.Where(t => t.DueDate < DateTime.Now).ToListAsync();
        foreach (var task in tasks)
        {
            Console.WriteLine(task);
        }
    }
}