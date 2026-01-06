using SQLite;

namespace CalorieTracker.Models;

public class FoodEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FoodName { get; set; }
    public int Calories { get; set; }
    public DateTime Date { get; set; }
}