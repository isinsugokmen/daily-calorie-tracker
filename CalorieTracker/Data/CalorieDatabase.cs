// Final Version
using SQLite;
using CalorieTracker.Models;

namespace CalorieTracker.Data;

public class CalorieDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public CalorieDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<FoodEntry>().Wait();
    }

    public Task<List<FoodEntry>> GetAllEntriesAsync()
        => _database.Table<FoodEntry>().ToListAsync();

    public Task<int> AddEntryAsync(FoodEntry entry)
        => _database.InsertAsync(entry);

    public Task<int> DeleteEntryAsync(FoodEntry entry)
        => _database.DeleteAsync(entry);
}