using CalculatorLib;
using CalculatorLib.Extentions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace BusinessCalculator.Services;

[ObservableObject]
public partial class DisplayService : IDisplayService
{
    private string _Input = "0";

    public DisplayService(string decimalSeparator, string numGroupSeparator, int maxInputNumber)
    {
        DecimalSeparator = decimalSeparator;
        NumberGroupSeparator = numGroupSeparator;
        MaxInputNumber = maxInputNumber;
    }

    public string Input
    {
        get => _Input;
        private set
        {
            if (value != _Input)
            {
                _Input = value;
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty]
    private string _RoundInput = "3";

    [ObservableProperty]
    private string _Preview = "";

    public string DecimalSeparator { get; init; }
    public string NumberGroupSeparator { get; init; }
    public int MaxInputNumber { get; init; }

    public int InputCounter { get; set; } = 0;
    public bool HasDecimal { get; set; } = false;

    public decimal GetDecimalValue() => decimal.Parse(Input);

    public void SetInputWith(decimal value, int exp) => Input = value.ToFormattedString(exp);
    public void SetInputWith(string value) => Input = value;

    public void AppendDigit(string num)
    {
        if (InputCounter == 0)
        {
            if (num == DecimalSeparator)
            {
                SetInputWith("0,");
                HasDecimal = true;
            }
            else
            {
                if (HasDecimal)
                {
                    Input += num;
                    InputCounter++;
                    OnPropertyChanged(nameof(Input));
                }
                else
                    SetInputWith(num);

                if (num != "0")
                    InputCounter = 1;
            }

            return;
        }

        if (HasDecimal)
        {
            if (num == DecimalSeparator)
                return;

            if (InputCounter == MaxInputNumber)
                return;

            Input += num;
            OnPropertyChanged(nameof(Input));
            InputCounter++;
        }
        else
        {
            if (num == DecimalSeparator)
            {
                Input += num;
                OnPropertyChanged(nameof(Input));
                HasDecimal = true;
                return;
            }

            if (InputCounter == MaxInputNumber)
                return;

            bool isNegate = Input.StartsWith('-');

            Span<char> inputSp = new Span<char>(new char[Input.Length + 2]);
            int j = inputSp.Length - 1;
            inputSp[j--] = num[0];

            InputCounter++;
            // formatting
            int groupCounter = 1;
            for (int i = Input.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(Input[i]))
                {
                    inputSp[j--] = Input[i];
                    groupCounter++;
                    if (groupCounter == 3)
                    {
                        inputSp[j--] = NumberGroupSeparator[0];
                        groupCounter = 0;
                    }
                }
            }
            j++;
            for (; j < inputSp.Length; j++)
            {
                if (char.IsDigit(inputSp[j]))
                    break;
            }

            if (isNegate)
                inputSp[--j] = '-';

            SetInputWith(inputSp[j..].ToString());
        }
    }
    public void RemoveLastDigit()
    {
        if (HasDecimal)
        {
            if (Input[^1] == DecimalSeparator[0])
            {
                HasDecimal = false;
                SetInputWith(Input[..^1]);
            }
            else
            {
                SetInputWith(Input[..^1]);
                InputCounter--;
            }
        }
        else
        {
            Span<char> input = new Span<char>(Input.ToArray());

            // 3 <--
            if (input.Length == 1)
            {
                SetInputWith("0");
                InputCounter = 0;
                return;
            }

            // 26 345 <--
            input = input[..^1];

            bool isNegate = false;
            int i = 0;

            if (input[0] == '-')
            {
                isNegate = true;
                i++;
                // -2 <--
                if (input.Length == 1)
                {
                    SetInputWith("0");
                    InputCounter = 0;
                    return;
                }
            }

            InputCounter--;
            // formatting
            for (; i < input.Length; i++)
            {
                if (input[i] == NumberGroupSeparator[0])
                {
                    (input[i - 1], input[i]) = (input[i], input[i - 1]);
                }
            }

            for (i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                    break;
            }
            if (isNegate)
                input[--i] = '-';

            SetInputWith(input[i..].ToString());
        }
    }
    public void TrimZeros()
    {
        if (!HasDecimal) return;

        for (int i = Input.Length - 1, k = 0; i >= 0; i--, k++)
        {
            if (Input[i] == '0')
                continue;
            else
            {
                if (Input[i] == DecimalSeparator[0])
                    k++;
                Input = Input[..^k];
                if (Input == "-0")
                    Input = "0";
                break;
            }
        }
    }
    public void Negate()
    {
        if (Input.StartsWith('-'))
            Input = Input[1..];
        else
        {
            if (Input == "0") return;

            Input = $"-{Input}";
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FullReset()
    {
        StateReset();
        Input = "0";
        Preview = "";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void StateReset()
    {
        InputCounter = 0;
        HasDecimal = false;
    }
}
