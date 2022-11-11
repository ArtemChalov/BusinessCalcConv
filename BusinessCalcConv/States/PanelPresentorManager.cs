using CalculatorLib;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessCalculator
{
    [ObservableObject]
    public sealed partial class PanelPresentorManager : IPanelPresentorManager
    {
        [ObservableProperty]
        private bool _ButtonsIsVisible = true;

        [ObservableProperty]
        private bool _RoundPanelIsVisible = false;

        [ObservableProperty]
        private bool _HistoryIsVisible = false;

        [ObservableProperty]
        private bool _MemoryButtonIsVisible = true;

        [ObservableProperty]
        private bool _HistoryButtonIsVisible = true;
    }
}
