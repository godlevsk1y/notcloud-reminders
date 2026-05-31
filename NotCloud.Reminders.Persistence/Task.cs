using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NotCloud.Reminders.Persistence;

public class Task
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime DueDate { get; set; }
    
    public bool IsCompleted { get; set; }

    public override string ToString()
    {
        return $"Title: {Title}\n" +
               $"Description: {Description}\n" +
               $"DueDate: {DueDate}\n" +
               $"IsCompleted: {IsCompleted}";
    }
}