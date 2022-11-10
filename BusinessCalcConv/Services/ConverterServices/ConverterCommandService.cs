using BusinessCalculator.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessCalculator.ConverterService;
using ConvertersLib;

namespace BusinessCalculator.Services;

public partial class ConverterCommandService : ObservableObject
{
    private readonly IConverterDisplayService _dispService;
    private readonly ConverterComputer _convComputer;
    public ConverterCommandService(IConverterDisplayService displayService, ConverterComputer converterComputer)
    {
        _dispService = displayService;
        _convComputer = converterComputer;
    }
    
    [RelayCommand]
    public void SelectLine(FrameEx selectedFrame)
    {
        if (selectedFrame.Tag is ValueLines line)
        {
            _dispService.CurrentLine = line;
            _dispService.StateReset();

            if (selectedFrame.Parent is Layout layoutContainer)
            {
                foreach (FrameEx frame in layoutContainer.Children.Where(c => c is FrameEx))
                {
                    if (!ReferenceEquals(selectedFrame, frame))
                    {
                        frame.IsChecked = false;
                    }
                }
            }

            selectedFrame.IsChecked = true;
        }
    }

    [RelayCommand]
    public void AddDigit(string digit)
    {
        _dispService.AppendDigit(digit);
    }
    
    [RelayCommand]
    public void Reset()
    {
        var temp = _dispService.CurrentLine;
        _dispService.CurrentLine = ValueLines.FirstLine;
        _dispService.FullReset();
        _dispService.CurrentLine = ValueLines.SecondLine;
        _dispService.FullReset();
        _dispService.CurrentLine = ValueLines.ThirdLine;
        _dispService.FullReset();
        _dispService.CurrentLine = temp;
    }
    
    [RelayCommand]
    public void Compute()
    {
        _convComputer.Compute();
        _dispService.StateReset();
    }

    [RelayCommand]
    public void OpenConverterSelector()
    {
        _convComputer.SelectedConverter = null;
    }

    [RelayCommand]
    public void SelectConverter(ICalcConverter selectedConverter)
    {
        _convComputer.SelectedConverter = selectedConverter;

        ComputeCommand?.Execute(null);
    }
}