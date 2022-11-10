using BusinessCalculator.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using BusinessCalculator.ConverterService;
using ConvertersLib;

namespace BusinessCalculator.MVVM.ViewModels;

public sealed partial class ConverterViewModel : ObservableObject
{
    public ConverterViewModel(IConverterDisplayService displayService)
    {
        DisplayService = displayService;
        ConverterComputer = new(DisplayService);
        Commands = new(displayService, ConverterComputer);

        Init();
    }

    #region Properties

    public IConverterDisplayService DisplayService { get; }

    public ConverterComputer ConverterComputer { get; }

    public ConverterCommandService Commands { get; }

    [ObservableProperty]
    private bool _ThirdLineIsVisivle = false;

    [ObservableProperty]
    private bool _SelectorIsVisible = false;

    #endregion

    #region Private Methods

    private void Init()
    {
        foreach (var conv in ConverterFactory.GetConverters())
        {
            ConverterComputer.Converters.Add(conv);
        }

        ConverterComputer.SelectedConverter = ConverterComputer.Converters.FirstOrDefault();
        if (ConverterComputer.SelectedConverter?.Items.Count == 3)
        {
            ThirdLineIsVisivle = true;
        }

        ConverterComputer.ConverterSelected += ConverterSelected;
    }

    private void ConverterSelected(ICalcConverter? calcConverter)
    {
        if (calcConverter is null)
        {
            SelectorIsVisible = true;
            return;
        }

        if (SelectorIsVisible)
        {
            SelectorIsVisible = false;
        }

        if (calcConverter?.Items.Count == 3)
        {
            ThirdLineIsVisivle = true;
        }
        else
        {
            ThirdLineIsVisivle = false;
        }
    }

    #endregion
}
