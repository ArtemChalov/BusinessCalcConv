using CalculatorLib;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BusinessCalculator.Services;

[ObservableObject]
public partial class CommandService
{
    private readonly IPanelPresentorManager _calcMan;
    private readonly IStateManager _stateMan;
    private readonly IMemoryService _memService;
    private readonly IDisplayService _dispService;
    private readonly IHistoryService _histService;

    public CommandService(IPanelPresentorManager panelPresMan, IStateManager stateMan, IDisplayService dispServ,  IMemoryService memServ, IHistoryService histServ)
    {
        _calcMan = panelPresMan;
        _stateMan = stateMan;
        _dispService = dispServ;
        _memService = memServ;
        _histService = histServ;
        GlobalEvents.CanExecuteChanged += (value) => CanExecuteCommand = value;
    }

    #region Properties

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MathSignInputCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyFunctionCommand))]
    private bool _CanExecuteCommand = true;

    #endregion Properties

    #region Input Commands

    [RelayCommand]
    public void AppendDigit(string digit) => _stateMan.InputState.AppendDigit(digit);

    [RelayCommand]
    public void RemoveDigit() => _stateMan.InputState.RemoveDigit();

    [RelayCommand]
    public void Reset()
    {
        _stateMan.ResetStateToDefault();
        _dispService.FullReset();
    }

    #endregion Input Commands

    #region Calculation Commands

    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    public void MathSignInput(string mathSign) => _stateMan.InputState.MathSignInput(mathSign);

    [RelayCommand]
    public void CalculateResult() => _stateMan.InputState.Calculate();

    #endregion Calculation Commands

    #region Functions Commands

    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    public void ApplyFunction(string funcName) => _stateMan.InputState.ApplyFunction(funcName);

    #endregion Functions Commands

    #region Rounding Commands

    [RelayCommand]
    public void OpenRoundPanel()
    {
        _calcMan.ButtonsIsVisible = false;
        _calcMan.RoundPanelIsVisible = true;
        _calcMan.HistoryButtonIsVisible = false;
        _calcMan.MemoryButtonIsVisible = false;
        _stateMan.InputState.BeginRound();
    }

    [RelayCommand]
    public void RoundUp() => _stateMan.InputState.RoundUp();
    [RelayCommand]
    public void RoundDown() => _stateMan.InputState.RoundDown();

    [RelayCommand]
    public void CloseRoundPanel(string success)
    {
        _calcMan.RoundPanelIsVisible = false;
        _calcMan.ButtonsIsVisible = true;
        _calcMan.HistoryButtonIsVisible = true;
        _calcMan.MemoryButtonIsVisible = true;

        _stateMan.InputState.FinishRound(success == "0" ? false : true);
    }

    #endregion Rounding Commands

    #region Memory Command

    [RelayCommand]
    public void Memory(string buttonName)
    {
        switch (buttonName)
        {
            case "MC":
                _memService.ResetMemory();
                break;
            case "MR":
                _stateMan.InputState.SetInputValueWith(_memService.Rational, _memService.ExpValue);
                break;
            case "M-":
                (decimal rational_, int expVal_) = _stateMan.InputState.CurrentValue();
                _memService.ChangeMemoryValue(rational_, expVal_, "-");
                break;
            case "M+":
                (decimal rational, int expVal) = _stateMan.InputState.CurrentValue();
                _memService.ChangeMemoryValue(rational, expVal, "+");
                break;
            default:
                break;
        }
    }

    #endregion Memory Command

    #region History Cammnd

    [RelayCommand]
    public void OpenHistory()
    {
        _calcMan.HistoryIsVisible = !_calcMan.HistoryIsVisible;
        _calcMan.ButtonsIsVisible = !_calcMan.HistoryIsVisible;
        _calcMan.MemoryButtonIsVisible = !_calcMan.HistoryIsVisible;
    }

    [RelayCommand]
    public void RemoveHistoryItem(CalcOperation calcOperation)
    {
        _histService.RemoveHistoryItem(calcOperation);
        _histService.SaveHistortyList();
    }

    [RelayCommand]
    public void SetHistoryItemValue(CalcOperation calcOperation)
    {
        _stateMan.InputState.SetInputValueWith(calcOperation.Rational, calcOperation.ExpVal);
        OpenHistoryCommand.Execute(null);
    }

    #endregion History Cammnd
}
