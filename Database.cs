using System.Text.Json;

namespace ApprenticeManagement;

public class AppData
{
    public List<Apprentice> Apprentices { get; set; } = new();
    public List<Company> Companies { get; set; } = new();
    public List<VocationalTrainer> Trainers { get; set; } = new();
}

public static class Database
{
    private const string FilePath = "app_data.json";
    public static AppData Data { get; private set; } = new();

    public static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                Data = JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}. Starting fresh.");
            Data = new AppData();
        }
    }

    public static void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Data, options);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error saving data: {ex.Message}");
        }
    }
}