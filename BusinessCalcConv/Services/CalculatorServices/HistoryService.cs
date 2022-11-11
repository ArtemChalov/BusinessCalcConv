using CalculatorLib;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace BusinessCalculator.Services
{
    public class HistoryService : IHistoryService
    {
        private const string FILE_NAME = "history.txt";

        private readonly IReposytory _reposytary;

        public HistoryService(IReposytory reposytary)
        {
            _reposytary = reposytary;
        }

        public ObservableCollection<CalcOperation> OperationHistory { get; } = new();

        public void AppendHistoryItem(CalcOperation calcOperation)
        {
            OperationHistory.Insert(0, calcOperation);

            if (OperationHistory.Count > 10)
                OperationHistory.RemoveAt(10);
        }

        public void RemoveHistoryItem(CalcOperation calcOperation) => OperationHistory.Remove(calcOperation);

        public void LoadHistortyList()
        {
            string? rowData = _reposytary.LoadData(FILE_NAME);

            if (!string.IsNullOrEmpty(rowData))
            {
                try
                {
                    List<CalcOperation>? calcOperation = JsonSerializer.Deserialize<List<CalcOperation>>(rowData);

                    if (calcOperation is not null)
                    {
                        foreach (var item in calcOperation)
                        {
                            OperationHistory.Add(item);
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public async Task LoadHistortyListAsync()
        {
            try
            {
                List<CalcOperation>? calcOperation = await new TaskFactory<List<CalcOperation>?>().StartNew(() =>
                    {
                        string? rowData = _reposytary.LoadData(FILE_NAME);
                        if (!string.IsNullOrEmpty(rowData))
                        {
                            return JsonSerializer.Deserialize<List<CalcOperation>>(rowData);
                        }
                        else
                        {
                            return null;
                        }
                    });

                if (calcOperation is not null)
                {
                    foreach (var item in calcOperation)
                    {
                        OperationHistory.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public void SaveHistortyList()
        {
            string json = JsonSerializer.Serialize(OperationHistory);

            _reposytary.SaveDataAsync(FILE_NAME, json);
        }
    }
}
