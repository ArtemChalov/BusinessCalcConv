using CommunityToolkit.Mvvm.ComponentModel;
using PDSMathLib;

namespace BusinessCalculator.ConverterService;


public partial class DefaultConverterDisplayService : ObservableObject, IConverterDisplayService
{
    private string _FirstInput = "0";
    private string _SecondInput = "0";
    private string _ThirdInput = "0";
    private PdsNumber _FirstValue;
    private PdsNumber _SecondValue;
    private PdsNumber _ThirdValue;

    public DefaultConverterDisplayService(string decimalSeparator, string numGroupSeparator, int maxInputNumber)
    {
        DecimalSeparator = decimalSeparator;
        NumberGroupSeparator = numGroupSeparator;
        MaxInputNumber = maxInputNumber;
    }

    public string FirstInput
    {
        get => _FirstInput;
        private set
        {
            if (value != _FirstInput)
            {
                _FirstInput = value;
                OnPropertyChanged();
            }
        }
    }
    public string SecondInput
    {
        get => _SecondInput;
        private set
        {
            if (value != _SecondInput)
            {
                _SecondInput = value;
                OnPropertyChanged();
            }
        }
    }
    public string ThirdInput
    {
        get => _ThirdInput;
        private set
        {
            if (value != _ThirdInput)
            {
                _ThirdInput = value;
                OnPropertyChanged();
            }
        }
    }
    [ObservableProperty]
    private string _SelectedConverterName = "";

    public PdsNumber FirstValue
    {
        set
        {
            _FirstValue = value;
            FirstInput = _FirstValue.ToString(MinExpanentValue, MaxExpanentValue, true);
        }
    }

    public PdsNumber SecondValue
    {
        set
        {
            _SecondValue = value;
            SecondInput = _SecondValue.ToString(MinExpanentValue, MaxExpanentValue, true);
        }
    }

    public PdsNumber ThirdValue
    {
        set
        {
            _ThirdValue = value;
            ThirdInput = _ThirdValue.ToString(MinExpanentValue, MaxExpanentValue, true);
        }
    }

    public string DecimalSeparator { get; init; }
    public string NumberGroupSeparator { get; init; }
    public int MaxInputNumber { get; init; }

    public int InputCounterFirst { get; set; } = 0;
    public bool HasDecimalFirst { get; set; } = false;
    public int InputCounterSecond { get; set; } = 0;
    public bool HasDecimalSecond { get; set; } = false;
    public int InputCounterThird { get; set; } = 0;
    public bool HasDecimalThird { get; set; } = false;

    public ValueLines CurrentLine { get; set; } = ValueLines.FirstLine;

    public PdsNumber GetDecimalValue()
    {
        ReadOnlySpan<char> input = CurrentLine switch
        {
            ValueLines.FirstLine => FirstInput.AsSpan(),
            ValueLines.SecondLine => SecondInput.AsSpan(),
            ValueLines.ThirdLine => ThirdInput.AsSpan(),
            _ => new char[] {'0'}
        };

        if (PdsNumber.TryParse(input, out PdsNumber result))
        {
            return result;
        }
        else
        {
            return new(default, default);
        }
    }

    public void SetInputWith(PdsNumber number)
    {
        switch (CurrentLine)
        {
            case ValueLines.FirstLine:
                FirstInput = number.ToString(MinExpanentValue, MaxExpanentValue);
                break;
            case ValueLines.SecondLine:
                SecondInput = number.ToString(MinExpanentValue, MaxExpanentValue);
                break;
            case ValueLines.ThirdLine:
                ThirdInput = number.ToString(MinExpanentValue, MaxExpanentValue);
                break;
            default:
                return;
        }
    }
    public void SetInputWith(string value)
    {
        switch (CurrentLine)
        {
            case ValueLines.FirstLine:
                FirstInput = value;
                break;
            case ValueLines.SecondLine:
                SecondInput = value;
                break;
            case ValueLines.ThirdLine:
                ThirdInput = value;
                break;
            default:
                return;
        }
    }
    public void AppendDigit(string num)
    {
        string input = CurrentLine switch
        {
            ValueLines.FirstLine => FirstInput,
            ValueLines.SecondLine => SecondInput,
            ValueLines.ThirdLine => ThirdInput,
            _ => "0"
        };

        int inputCounter = CurrentLine switch
        {
            ValueLines.FirstLine => InputCounterFirst,
            ValueLines.SecondLine => InputCounterSecond,
            ValueLines.ThirdLine => InputCounterThird,
            _ => 0
        };

        if (inputCounter == 0)
        {
            if (num == DecimalSeparator)
            {
                SetInputWith("0,");
                SetHasDecimal(true);
            }
            else
            {
                if (GetHasDecimal())
                {
                    input += num;
                    inputCounter++;
                    SetInputCounter(inputCounter);
                    SetInputWith(input);
                }
                else
                {
                    SetInputWith(num);
                }

                if (num != "0")
                {
                    SetInputCounter(1);
                }
            }

            return;
        }

        if (GetHasDecimal())
        {
            if (num == DecimalSeparator)
                return;

            if (inputCounter == MaxInputNumber)
                return;

            input += num;
            SetInputWith(input);
            inputCounter++;
            SetInputCounter(inputCounter);
        }
        else
        {
            if (num == DecimalSeparator)
            {
                input += num;
                SetInputWith(input);
                SetHasDecimal(true);
                return;
            }

            if (inputCounter == MaxInputNumber)
                return;

            bool isNegate = input.StartsWith('-');

            Span<char> inputSp = new Span<char>(new char[input.Length + 2]);
            int j = inputSp.Length - 1;
            inputSp[j--] = num[0];

            inputCounter++;
            SetInputCounter(inputCounter);
            // formatting
            int groupCounter = 1;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(input[i]))
                {
                    inputSp[j--] = input[i];
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
        string input = CurrentLine switch
        {
            ValueLines.FirstLine => FirstInput,
            ValueLines.SecondLine => SecondInput,
            ValueLines.ThirdLine => ThirdInput,
            _ => "0"
        };

        int inputCounter = CurrentLine switch
        {
            ValueLines.FirstLine => InputCounterFirst,
            ValueLines.SecondLine => InputCounterSecond,
            ValueLines.ThirdLine => InputCounterThird,
            _ => 0
        };

        if (GetHasDecimal())
        {
            if (input[^1] == DecimalSeparator[0])
            {
                SetHasDecimal(false);
                SetInputWith(input[..^1]);
            }
            else
            {
                SetInputWith(input[..^1]);
                inputCounter--;
                SetInputCounter(inputCounter);
            }
        }
        else
        {
            Span<char> inputSp = new Span<char>(input.ToArray());

            // 3 <--
            if (inputSp.Length == 1)
            {
                SetInputWith("0");
                SetInputCounter(0);
                return;
            }

            // 26 345 <--
            inputSp = inputSp[..^1];

            bool isNegate = false;
            int i = 0;

            if (inputSp[0] == '-')
            {
                isNegate = true;
                i++;
                // -2 <--
                if (inputSp.Length == 1)
                {
                    SetInputWith("0");
                    SetInputCounter(0);
                    return;
                }
            }

            inputCounter--;
            SetInputCounter(inputCounter);
            // formatting
            for (; i < inputSp.Length; i++)
            {
                if (inputSp[i] == NumberGroupSeparator[0])
                {
                    (inputSp[i - 1], inputSp[i]) = (inputSp[i], inputSp[i - 1]);
                }
            }

            for (i = 0; i < inputSp.Length; i++)
            {
                if (char.IsDigit(inputSp[i]))
                    break;
            }
            if (isNegate)
                inputSp[--i] = '-';

            SetInputWith(inputSp[i..].ToString());
        }
    }

    public void TrimZeros()
    {
        if (!GetHasDecimal()) return;

        string input = CurrentLine switch
        {
            ValueLines.FirstLine => FirstInput,
            ValueLines.SecondLine => SecondInput,
            ValueLines.ThirdLine => ThirdInput,
            _ => "0"
        };

        for (int i = input.Length - 1, k = 0; i >= 0; i--, k++)
        {
            if (input[i] == '0')
                continue;
            else
            {
                if (input[i] == DecimalSeparator[0])
                    k++;
                input = input[..^k];
                if (input == "-0")
                    input = "0";
                break;
            }
        }
        SetInputWith(input);
    }

    public void FullReset()
    {
        StateReset();
        switch (CurrentLine)
        {
            case ValueLines.FirstLine:
                FirstInput = "0";
                break;
            case ValueLines.SecondLine:
                SecondInput = "0";
                break;
            case ValueLines.ThirdLine:
                ThirdInput = "0";
                break;
            default:
                return;
        }
    }

    public void StateReset()
    {
        switch (CurrentLine)
        {
            case ValueLines.FirstLine:
                InputCounterFirst = 0;
                HasDecimalFirst = false;
                break;
            case ValueLines.SecondLine:
                InputCounterSecond = 0;
                HasDecimalSecond = false;
                break;
            case ValueLines.ThirdLine:
                InputCounterThird = 0;
                HasDecimalThird = false;
                break;
            default:
                return;
        }
    }

    private void SetHasDecimal(bool value)
    {
        switch (CurrentLine)
        {
            case ValueLines.FirstLine:
                HasDecimalFirst = value;
                break;
            case ValueLines.SecondLine:
                HasDecimalSecond = value;
                break;
            case ValueLines.ThirdLine:
                HasDecimalThird = value;
                break;
            default:
                return;
        }
    }

    private bool GetHasDecimal()
    {
        return CurrentLine switch
        {
            ValueLines.FirstLine => HasDecimalFirst,
            ValueLines.SecondLine => HasDecimalSecond,
            ValueLines.ThirdLine => HasDecimalThird,
            _ => false
        };
    }

    private void SetInputCounter(int value)
    {
        switch (CurrentLine)
        {
            case ValueLines.FirstLine:
                InputCounterFirst = value;
                break;
            case ValueLines.SecondLine:
                InputCounterSecond = value;
                break;
            case ValueLines.ThirdLine:
                InputCounterThird = value;
                break;
            default:
                return;
        }
    }

    public int MinExpanentValue { get; set; } = -5;
    public int MaxExpanentValue { get; set; } = 15;
}