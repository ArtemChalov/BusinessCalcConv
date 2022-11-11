using BusinessCalculator.Services;
using CalculatorLib;

namespace BusinessCalculator.MVVM.ViewModels;

public class CalculatorViewModel
{
    public CalculatorViewModel(IPanelPresentorManager panelPresMan, IStateManager stateMan, IDisplayService dispServ,
                               IMemoryService memServ, IHistoryService histServ)
    {
        PanelPresentorManager = panelPresMan;
        MemoryService = memServ;
        DisplayService = dispServ;
        HistoryService = histServ;
        CommandService = new(panelPresMan, stateMan, dispServ, memServ, histServ);
        InitHistory();
    }

    #region Properties

    public IPanelPresentorManager PanelPresentorManager { get; }
    public IDisplayService DisplayService { get; }
    public IMemoryService MemoryService { get; }
    public CommandService CommandService { get; }
    public IHistoryService HistoryService { get; }

    #endregion Properties

    private async void InitHistory()
    {
        await HistoryService.LoadHistortyListAsync();

        GlobalEvents.OnResultCalculated += (opPreview, result, resultExp) =>
        {
            // Here we fill the list of history
            (string fVal, string sVal) = CalcOperation.GetParams(opPreview.AsSpan());
            HistoryService.AppendHistoryItem(new CalcOperation(fVal, sVal, result, resultExp));
            HistoryService.SaveHistortyList();
        };
    }
}
