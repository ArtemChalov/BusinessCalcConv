using CalculatorLib;

namespace BusinessCalculator.Services;

public sealed class FileRepository : IReposytory
{
    private readonly string folderPath = FileSystem.Current.AppDataDirectory;

    public string? LoadData(string fileName)
    {
        string filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
            return null;
    }

    public async Task<string?> LoadDataAsync(string fileName)
    {
        string filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath);
        }
        else
            return null;
    }

    public async void SaveDataAsync(string fileName, string text)
    {
        string filePath = Path.Combine(folderPath, fileName);
        
        await File.WriteAllTextAsync(filePath, text);
    }
}
