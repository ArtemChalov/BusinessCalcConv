using CalculatorLib;
using CalculatorLib.Extentions;

namespace BusinessCalculator.States;

public sealed class FirstInputState : InputStateBase
{
    public FirstInputState(IStateManager stateManager, IDisplayService displayService) 
        : base(stateManager, displayService)
    {

    }

    #region Input Methods

    public override void AppendDigit(string num)
    {
        if (_stateMan.HasError)
        {
            _stateMan.ResetStateToDefault();
            _dispServ.FullReset();
        }

        if (_stateMan.IsCalculated)
        {
            _dispServ.StateReset();
        }

        if (_dispServ.InputCounter == 0)
            _dispServ.Preview = "";
        _dispServ.AppendDigit(num);
        _stateMan.IsCalculated = false;
    }

    public override void RemoveDigit()
    {
        if (_stateMan.HasError)
        {
            _stateMan.ResetStateToDefault();
            _dispServ.FullReset();
            return;
        }

        if (!_stateMan.IsCalculated)
            _dispServ.RemoveLastDigit();
    }

    public override void SetInputValueWith(decimal rational, int expValue)
    {
        _stateMan.IsCalculated = true;
        _dispServ.Preview = "";
        CalcEngine.Result = rational;
        CalcEngine.ResultExp = expValue;
        // Result display
        _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);
        AppData.FirstValPrev = _dispServ.Input;
    }

    #endregion Input Methods

    #region Calculation

    public override void MathSignInput(string mathSign)
    {
        _stateMan.MathSign = mathSign;

        // Getting the first value.
        (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = GetFirstValue();

        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(mathSign).Build();
        // To enter a new value.
        _dispServ.StateReset();
        // Sets HasSecondValue to false.
        // Sets SecondValue to zero.
        _stateMan.SetState(InputStates.SecondState);
    }

    public override void Calculate()
    {
        if (_stateMan.HasError)
        {
            _stateMan.ResetStateToDefault();
            _dispServ.FullReset();
            return;
        }

        // Getting the first value.
        (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = GetFirstValue();

        // If we have a math sign, we have pressed equals twice in a row.
        // In this case, the second value is not empty and we can perform the math operation again.
        if (_stateMan.HasMathSign)
        {
            // Math action display
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev)
                                            .Append(_stateMan.MathSign)
                                            .Append(AppData.SecondValPrev)
                                            .Append("=")
                                            .Build();

            bool success = CalcEngine.TryCalculate(AppData.FirstVal, AppData.FirstExp, AppData.SecondVal, AppData.SecondExp, _stateMan.MathSign);
            if (success)
            {
                // Result display
                _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp); 

                GlobalEvents.RiseOnResultCalculated(_dispServ.Preview, CalcEngine.Result, CalcEngine.ResultExp);

                // To next calculation
                AppData.FirstValPrev = _dispServ.Input;
            }
            else
                RiseErrorState();
        }
        else
        {
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append("=").Build();
            // To next calculation
            AppData.FirstValPrev = _dispServ.Input;
        }

        // To enter a new value.
        _dispServ.StateReset();
    }

    #endregion Calculation

    #region Functions

    public override void Sqr()
    {
        // Getting the first value.
        (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = GetFirstValue();

        PreviewFunction(SQR_PREFIX);

        var success = CalcEngine.TryCalculate(AppData.FirstVal, AppData.FirstExp, AppData.FirstVal, AppData.FirstExp, CalcSettings.MULTIPLY_SIGN);

        ActionsAfterFunction(success);
    }

    public override void Sqrt()
    {
        // Getting the first value.
        (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = GetFirstValue();

        PreviewFunction(SQRT_PREFIX);

        var success = CalcEngine.TrySqrt(AppData.FirstVal, AppData.FirstExp);

        ActionsAfterFunction(success);
    }

    public override void Invert()
    {
        // Getting the first value.
        (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = GetFirstValue();

        PreviewFunction(INVERT_PREFIX);

        // Result calculation.
        var success = CalcEngine.TryCalculate(decimal.One, 0, AppData.FirstVal, AppData.FirstExp, CalcSettings.DIVIDE_SIGN);

        ActionsAfterFunction(success);
    }

    public override void ToggleSign()
    {
        if (_stateMan.IsCalculated)
        {
            AppData.FirstValPrev = $"{NEGATE_PREFIX}({AppData.FirstValPrev})";
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Build();
            CalcEngine.Result = decimal.Negate(CalcEngine.Result);
        }

        _dispServ.Negate();
    }

    public override void Percent()
    {
        if (!_stateMan.HasMathSign)
        {
            (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = (decimal.Zero, 0, "0");
            (CalcEngine.Result, CalcEngine.ResultExp) = (AppData.FirstVal, AppData.FirstExp);
        }
        else
        {
            (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev) = GetFirstValue();
            if (!_stateMan.HasSecondVal)
            {
                _stateMan.HasSecondVal = true;
                if (_stateMan.MathSign == CalcSettings.ADD_SIGN || _stateMan.MathSign == CalcSettings.SUBTRACT_SIGN)
                {
                    AppData.SecondVal = AppData.FirstVal;
                    AppData.SecondExp = AppData.FirstExp;
                }
                else
                {
                    AppData.SecondVal = decimal.One;
                    AppData.SecondExp = 0;
                }
            }

            bool success = CalcEngine.TryGetPercent(AppData.FirstVal, AppData.FirstExp, AppData.SecondVal, AppData.SecondExp);
            if (success)
            {
                string sBase = AppData.FirstVal.ToFormattedString(AppData.FirstExp);
                string sPercent = AppData.SecondVal.ToFormattedString(AppData.SecondExp);
                // Ru-ru(10% от 120) or (10% from 120)
                AppData.FirstValPrev = $"({sPercent}% {FROM_STR} {sBase})";
            }
            else
                RiseErrorState();
        }

        _stateMan.IsCalculated = true;
        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Build();
        _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);
    }

    #endregion Functions

    #region Rounding

    public override void FinishRound(bool success)
    {
        if (success)
        {
            _dispServ.Preview = "";
        }

        base.FinishRound(success);
    }

    #endregion Rounding

    #region Support Methods

    private (decimal value, int exp, string preview) GetFirstValue()
    {
        if (_stateMan.IsCalculated) // Obtained after applying the math function
        {
            return (CalcEngine.Result, CalcEngine.ResultExp, AppData.FirstValPrev);
        }
        else // Entered manually
        {
            // 123,2150000 -> 123,215
            _dispServ.TrimZeros();
            return (_dispServ.GetDecimalValue(), 0, _dispServ.Input);
        }
    }

    private void PreviewFunction(string prefix)
    {
        AppData.FirstValPrev = $"{prefix}({AppData.FirstValPrev})";
        // sqr(2 456,156)
        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Build();
    }

    private void ActionsAfterFunction(bool success)
    {
        // The result is stored in the CalcEngine class.
        if (success)
        {
            _stateMan.IsCalculated = true;

            // Result display
            _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);
        }
        else
            RiseErrorState();
    }

    protected override void RiseErrorState()
    {
        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(_stateMan.MathSign).Build();
        _dispServ.SetInputWith(CalcEngine.ErrorMsg);
        GlobalEvents.RiseCanExecuteChanged(false);
    }

    public override (decimal rational, int expVal) CurrentValue()
    {
        (decimal rational, int expVal, string prev) = GetFirstValue();
        return (rational, expVal);
    }

    #endregion Support Methods
}
