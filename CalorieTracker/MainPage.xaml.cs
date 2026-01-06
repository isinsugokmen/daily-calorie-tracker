using CalorieTracker.Data;
using CalorieTracker.Models;
using Microsoft.Maui.Storage;

namespace CalorieTracker;

public partial class MainPage : ContentPage
{
    private readonly CalorieDatabase _database;

    // 🎯 User-defined daily goal
    private int _dailyGoal;

    // 📅 Selected date
    private DateTime _selectedDate = DateTime.Today;

    public MainPage(CalorieDatabase database)
    {
        InitializeComponent();
        _database = database;

        // 🔹 Load saved goal or default
        _dailyGoal = Preferences.Get("DailyGoal", 2000);
        goalCalorieEntry.Text = _dailyGoal.ToString();

        // 🔹 DatePicker default
        _selectedDate = DateTime.Today;
        datePicker.Date = _selectedDate;

        LoadData();
    }

    // 📅 DatePicker changed
    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        _selectedDate = e.NewDate.Date;
        LoadData();
    }

    private async void LoadData()
    {
        var allEntries = await _database.GetAllEntriesAsync();

        // 🔹 Filter by selected date
        var selectedDayEntries = allEntries
            .Where(x => x.Date.Date == _selectedDate)
            .ToList();

        foodList.ItemsSource = selectedDayEntries;

        int totalCalories = selectedDayEntries.Sum(x => x.Calories);
        totalCaloriesLabel.Text = $"Total Calories: {totalCalories}";

        // 🧠 DAILY CALORIE FEEDBACK (goal-based)
        if (totalCalories < _dailyGoal * 0.6)
        {
            calorieStatusLabel.Text = "You are eating too little today ⚠️";
            calorieStatusLabel.TextColor = Colors.Orange;
        }
        else if (totalCalories <= _dailyGoal)
        {
            calorieStatusLabel.Text = "Your calorie intake is in the ideal range ✅";
            calorieStatusLabel.TextColor = Colors.Green;
        }
        else
        {
            calorieStatusLabel.Text = "You exceeded your daily calorie goal ❌";
            calorieStatusLabel.TextColor = Colors.Red;
        }

        // 📊 PROGRESS BAR UPDATE
        double progress = (double)totalCalories / _dailyGoal;
        progress = Math.Min(progress, 1.0); // %100’ü geçmesin

        calorieProgressBar.Progress = progress;

        // 🎨 Progress color
        if (progress < 0.6)
        {
            calorieProgressBar.ProgressColor = Colors.Orange;
        }
        else if (progress <= 1.0)
        {
            calorieProgressBar.ProgressColor = Colors.Green;
        }
        else
        {
            calorieProgressBar.ProgressColor = Colors.Red;
        }

        // (Opsiyonel) progress text
        progressTextLabel.Text = $"{totalCalories} / {_dailyGoal} kcal";
    }

    // 💾 Save daily goal
    private async void OnSaveGoalClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(goalCalorieEntry.Text, out int goal) || goal <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid calorie goal", "OK");
            return;
        }

        _dailyGoal = goal;
        Preferences.Set("DailyGoal", goal);

        await DisplayAlert("Saved", "Daily calorie goal saved ✅", "OK");
        LoadData();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(foodNameEntry.Text) ||
            string.IsNullOrWhiteSpace(calorieEntry.Text))
        {
            await DisplayAlert("Error", "Please enter all fields", "OK");
            return;
        }

        if (!int.TryParse(calorieEntry.Text, out int calories))
        {
            await DisplayAlert("Error", "Calories must be a number", "OK");
            return;
        }

        var entry = new FoodEntry
        {
            FoodName = foodNameEntry.Text,
            Calories = calories,
            Date = _selectedDate   // seçili gün
        };

        await _database.AddEntryAsync(entry);

        foodNameEntry.Text = "";
        calorieEntry.Text = "";

        LoadData();
    }

    private async void OnDeleteInvoked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var entry = swipeItem?.BindingContext as FoodEntry;

        if (entry == null)
            return;

        bool confirm = await DisplayAlert(
            "Delete",
            $"Delete {entry.FoodName}?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _database.DeleteEntryAsync(entry);
        LoadData();
    }
}
