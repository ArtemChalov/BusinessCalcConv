using CommunityToolkit.Mvvm.ComponentModel;
using BusinessCalculator.ConverterService;
using ConvertersLib;
using PDSMathLib;
using System.Collections.ObjectModel;

namespace BusinessCalculator.Services;

public sealed partial class ConverterComputer : ObservableObject
{
    private readonly IConverterDisplayService _dispService;
    private ICalcConverter? _SelectedConverter;
    private ConverterItem? _SelectedConvItem1;
    private ConverterItem? _SelectedConvItem2;
    private ConverterItem? _SelectedConvItem3;

    public ConverterComputer(IConverterDisplayService displayService)
    {
        _dispService = displayService;
    }
    
    #region Properties
    
    public ObservableCollection<ICalcConverter> Converters { get; } = new();
    
    public ICalcConverter? SelectedConverter
    {
        get => _SelectedConverter;
        set
        {
            if (_SelectedConverter != value)
            {
                _SelectedConverter = value;
                OnPropertyChanged();
                OnConverterSelected();
            }

            ConverterSelected?.Invoke(_SelectedConverter);
        }
    }

    public ObservableCollection<ConverterItem> List1 { get; } = new();

    public ConverterItem? SelectedConvItem1
    {
        get => _SelectedConvItem1;
        set
        {
            if (_SelectedConvItem1 != value)
            {
                _SelectedConvItem1 = value;
                OnPropertyChanged();
                ComputeSecondLine();
            }
        }
    }

    public ObservableCollection<ConverterItem> List2 { get; } = new();

    public ConverterItem? SelectedConvItem2
    {
        get => _SelectedConvItem2;
        set
        {
            if (_SelectedConvItem2 != value)
            {
                _SelectedConvItem2 = value;
                OnPropertyChanged();
                ComputeFirstLine();
            }
        }
    }

    public ObservableCollection<ConverterItem> List3 { get; } = new();

    public ConverterItem? SelectedConvItem3
    {
        get => _SelectedConvItem3;
        set
        {
            if (_SelectedConvItem3 != value)
            {
                _SelectedConvItem3 = value;
                OnPropertyChanged();
                ComputeFirstEndSecondLine();
            }
        }
    }

    #endregion

    #region Public Mehtods

    public void Compute()
    {
        if (_dispService.CurrentLine == ValueLines.FirstLine)
        {
            ComputeSecondLine();
        }
        else if (_dispService.CurrentLine == ValueLines.SecondLine)
        {
            ComputeFirstLine();
        }
        else
        {
            ComputeFirstEndSecondLine();
        }
    }

    #endregion

    #region Private Methods

    private void ComputeFirstLine()
    {
        if (SelectedConvItem1 is null || SelectedConvItem2 is null) return;

        var secondVal = _dispService.GetDecimalValue();

        if (SelectedConverter?.Items.Count == 3)
        {
            // НДС || °C
            secondVal = PdsNumber.Round(secondVal, 2);
            _dispService.SecondValue = secondVal;
        }

        // без НДС || Kelvin
        var res = SelectedConverter!.Convert(SelectedConvItem2, SelectedConvItem1, secondVal);
        if (res >= 1)
        {
            res = PdsNumber.Round(res, 5);
        }

        if (SelectedConverter.Items.Count == 3)
        {
            if (SelectedConverter is NDSConverter)
            {
                _dispService.ThirdValue = res + secondVal; // в том числе НДС
            }

            if (SelectedConverter is TemperatureConverter && SelectedConvItem3 is not null)
            {
                res = PdsNumber.Round(res, 2);
                var farengate = SelectedConverter!.Convert(SelectedConvItem2, SelectedConvItem3, secondVal); // °F
                farengate = PdsNumber.Round(farengate, 2);
                _dispService.ThirdValue = farengate;
            }
        }
        else
        {
            _dispService.ThirdValue = new PdsNumber();
        }
        
        _dispService.FirstValue = res;
    }

    private void ComputeSecondLine()
    {
        if (SelectedConvItem1 is null || SelectedConvItem2 is null) return;

        var firstVal = _dispService.GetDecimalValue();

        if (SelectedConverter?.Items.Count == 3)
        {
            // без НДС || Kelvin
            firstVal = PdsNumber.Round(firstVal, 2);
            _dispService.FirstValue = firstVal;
        }

        // НДС || °C
        var res = SelectedConverter!.Convert(SelectedConvItem1, SelectedConvItem2, firstVal);
        if (res >= 1)
        {
            res = PdsNumber.Round(res, 5);
        }

        if (SelectedConverter.Items.Count == 3)
        {
            if (SelectedConverter is NDSConverter)
            {
                _dispService.ThirdValue = firstVal + res; // в том числе НДС
            }

            if (SelectedConverter is TemperatureConverter && SelectedConvItem3 is not null)
            {
                res = PdsNumber.Round(res, 2);
                var farengate = SelectedConverter!.Convert(SelectedConvItem2, SelectedConvItem3, res); // °F
                farengate = PdsNumber.Round(farengate, 2);
                _dispService.ThirdValue = farengate;
            }
        }
        else
        {
            _dispService.ThirdValue = new PdsNumber();
        }
        
        _dispService.SecondValue = res;
    }
    
    private void ComputeFirstEndSecondLine()
    {
        if (SelectedConvItem3 is null || SelectedConvItem1 is null || SelectedConvItem2 is null) return;

        var thirdVal = _dispService.GetDecimalValue();
        // в том числе НДС || °F
        thirdVal = PdsNumber.Round(thirdVal, 2);
        _dispService.ThirdValue = thirdVal;
        
        if (SelectedConverter is NDSConverter)
        {
            var withoutNds = SelectedConverter.Convert( SelectedConvItem3, SelectedConvItem1, thirdVal);
            _dispService.FirstValue = withoutNds; // без НДС
            
            _dispService.SecondValue = thirdVal - withoutNds;// НДС
        }

        if (SelectedConverter is TemperatureConverter)
        {
            var celcium = SelectedConverter!.Convert(SelectedConvItem3, SelectedConvItem2, thirdVal); // °C
            celcium = PdsNumber.Round(celcium, 2);
            _dispService.SecondValue = celcium;
            
            var kelvin = SelectedConverter!.Convert(SelectedConvItem2, SelectedConvItem1, celcium); // K
            kelvin = PdsNumber.Round(kelvin, 2);
            _dispService.FirstValue = kelvin;
        }
    }

    private void OnConverterSelected()
    {
        List1.Clear();
        List2.Clear();
        List3.Clear();

        if (_SelectedConverter is null) return;

        if (_SelectedConverter.Items.Count == 3)
        {
            List1.Add(_SelectedConverter.Items[0]);
            List2.Add(_SelectedConverter.Items[1]);
            List3.Add(_SelectedConverter.Items[2]);

            SelectedConvItem1 = List1.FirstOrDefault(c => c.ID == -1);
            SelectedConvItem2 = List2.FirstOrDefault(c => c.ID == 0);
            SelectedConvItem3 = List3.FirstOrDefault(c => c.ID == 1);
        }
        else
        {
            foreach (ConverterItem item in _SelectedConverter.Items)
            {
                List1.Add(item);
                List2.Add(item);
            }

            SelectedConvItem1 = List1.FirstOrDefault(c => c.ID == -1);
            SelectedConvItem2 = List2.FirstOrDefault(c => c.ID == 0);
        }
    }

    #endregion

    #region Events

    public event Action<ICalcConverter?>? ConverterSelected;

    #endregion
}