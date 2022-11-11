using CalculatorLib;
using CalculatorLib.Extentions;

namespace BusinessCalculator.States;

public sealed class SecondInputState : InputStateBase
{
    public SecondInputState(IStateManager stateManager, IDisplayService displayService)
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
            _stateMan.IsCalculated = false;
            _dispServ.InputCounter = 0;
            _dispServ.HasDecimal = false;
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(_stateMan.MathSign).Build();
        }

        _dispServ.AppendDigit(num);
        _stateMan.HasSecondVal = true;
    }

    public override void RemoveDigit()
    {
        if (_stateMan.HasError)
        {
            _stateMan.ResetStateToDefault();
            _dispServ.FullReset();
            return;
        }

        if (_stateMan.HasSecondVal && !_stateMan.IsCalculated)
        {
            _dispServ.RemoveLastDigit();
        }
    }

    public override void SetInputValueWith(decimal rational, int expValue)
    {
        _stateMan.IsCalculated = true;
        
        CalcEngine.Result = rational;
        CalcEngine.ResultExp = expValue;

        // 256 + sqr(25) -> 256 +
        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev)
                                        .Append(_stateMan.MathSign)
                                        .Build();
        // Result display
        _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);
        AppData.SecondValPrev = _dispServ.Input;

        _stateMan.HasSecondVal = true;
    }

    #endregion Input Methods

    #region Calculation

    public override void MathSignInput(string mathSign)
    {
        if (_stateMan.HasSecondVal)
        {
            // Here we need to calculate the value, record the action in the history
            // and put result in place in the first value.

            // Getting the second value.
            (AppData.SecondVal, AppData.SecondExp, AppData.SecondValPrev) = GetSecondValue();

            // Result calculation.
            bool success = CalcEngine.TryCalculate(AppData.FirstVal, AppData.FirstExp, AppData.SecondVal, AppData.SecondExp, _stateMan.MathSign);
            // The result is stored in the CalcEngine class.
            if (success)
            {
                // Result display
                _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);

                // Record in the history
                var historyPreview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(_stateMan.MathSign).Append(AppData.SecondValPrev).Append("=").Build();
                GlobalEvents.RiseOnResultCalculated(historyPreview, CalcEngine.Result, CalcEngine.ResultExp);

                // Preparing to the next operation
                AppData.FirstVal = CalcEngine.Result;
                AppData.FirstExp = CalcEngine.ResultExp;
                AppData.FirstValPrev = _dispServ.Input;

                _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(_stateMan.MathSign).Build();

                // To enter a new value.
                _dispServ.StateReset();
                _stateMan.HasSecondVal = false;
                _stateMan.MathSign = mathSign;
            }
            else
                RiseErrorState();
        }
        else
        {
            // Here we need to change the math sign in the preview line.
            _stateMan.MathSign = mathSign;
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(_stateMan.MathSign).Build();
        }
    }

    public override void Calculate()
    {
        if (_stateMan.HasError)
        {
            _stateMan.ResetStateToDefault();
            _dispServ.FullReset();
            return;
        }

        // Getting the second value.
        (AppData.SecondVal, AppData.SecondExp, AppData.SecondValPrev) = GetSecondValue();

        // Math action display
        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev)
                                        .Append(_stateMan.MathSign)
                                        .Append(AppData.SecondValPrev)
                                        .Append("=")
                                        .Build();

        // Result calculation.
        bool success = CalcEngine.TryCalculate(AppData.FirstVal, AppData.FirstExp, AppData.SecondVal, AppData.SecondExp, _stateMan.MathSign);
        // The result is stored in the CalcEngine class.
        if (success)
        {
            _stateMan.IsCalculated = true;

            // Result display
            _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);

            GlobalEvents.RiseOnResultCalculated(_dispServ.Preview, CalcEngine.Result, CalcEngine.ResultExp);

            // To next calculation
            AppData.FirstValPrev = _dispServ.Input;
            // sqr(3) -> 9
            AppData.SecondValPrev = AppData.SecondVal.ToFormattedString(AppData.SecondExp);

            // To enter a new value.
            _dispServ.StateReset();
            _stateMan.SetState(InputStates.FirstState);
        }
        else
            RiseErrorState();
    }

    #endregion Calculation

    #region Functions

    public override void Sqr()
    {
        // Getting the second value.
        (AppData.SecondVal, AppData.SecondExp, AppData.SecondValPrev) = GetSecondValue();

        PreviewFunction(SQR_PREFIX);

        var success = CalcEngine.TryCalculate(AppData.SecondVal, AppData.SecondExp, AppData.SecondVal, AppData.SecondExp, CalcSettings.MULTIPLY_SIGN);

        ActionsAfterFunction(success);
    }

    public override void Sqrt()
    {
        // Getting the second value.
        (AppData.SecondVal, AppData.SecondExp, AppData.SecondValPrev) = GetSecondValue();

        PreviewFunction(SQRT_PREFIX);

        var success = CalcEngine.TrySqrt(AppData.SecondVal, AppData.SecondExp);

        ActionsAfterFunction(success);
    }

    public override void Invert()
    {
        // Getting the second value.
        (AppData.SecondVal, AppData.SecondExp, AppData.SecondValPrev) = GetSecondValue();

        PreviewFunction(INVERT_PREFIX);

        // Result calculation.
        var success = CalcEngine.TryCalculate(decimal.One, 0, AppData.SecondVal, AppData.SecondExp, CalcSettings.DIVIDE_SIGN);

        ActionsAfterFunction(success);
    }

    public override void ToggleSign()
    {
        if (!_stateMan.HasSecondVal)
        {
            _stateMan.HasSecondVal = true;

            if (_stateMan.IsCalculated)
            {
                CalcEngine.Result = decimal.Negate(CalcEngine.Result);
                AppData.SecondValPrev = $"{NEGATE_PREFIX}({_dispServ.Input})";
            }
            else
            {
                CalcEngine.Result = decimal.Negate(AppData.FirstVal);
                CalcEngine.ResultExp = AppData.FirstExp;
                AppData.SecondValPrev = $"{NEGATE_PREFIX}({_dispServ.Input})";
                _stateMan.IsCalculated = true;
            }
            _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);
        }
        else
        {
            if (_stateMan.IsCalculated)
            {
                AppData.SecondValPrev = $"{NEGATE_PREFIX}({AppData.SecondValPrev})";
                CalcEngine.Result = decimal.Negate(CalcEngine.Result);
            }

            _dispServ.Negate();
        }

        if (_stateMan.IsCalculated)
        {
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev)
                                            .Append(_stateMan.MathSign)
                                            .Append(AppData.SecondValPrev)
                                            .Build();
        }
    }

    public override void Percent()
    {
        decimal baseVal = AppData.FirstVal;
        int baseValExp = AppData.FirstExp;

        (decimal percent, int percentExp, string prev) = GetSecondValue();

        if (_stateMan.MathSign == CalcSettings.MULTIPLY_SIGN || _stateMan.MathSign == CalcSettings.DIVIDE_SIGN)
        {
            baseVal = decimal.One;
            baseValExp = 0;
        }

        bool success = CalcEngine.TryGetPercent(baseVal, baseValExp, percent, percentExp);
        if (success)
        {
            string sPercent = percent.ToFormattedString(percentExp);
            string sBaseVal = baseVal.ToFormattedString(baseValExp);
            // Ru-ru(10% от 120) or (10% from 120)
            AppData.SecondValPrev = $"({sPercent}% {FROM_STR} {sBaseVal})";
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev)
                                            .Append(_stateMan.MathSign)
                                            .Append(AppData.SecondValPrev)
                                            .Build();
            _dispServ.SetInputWith(CalcEngine.Result, CalcEngine.ResultExp);
            _stateMan.IsCalculated = true;
        }
        else
            RiseErrorState();
    }

    #endregion Functions

    #region Rounding

    public override void FinishRound(bool success)
    {
        if (success)
        {
            //if (_stateMan.SecondValIsFuncRes)
            //    _stateMan.SecondValIsFuncRes = false;
            
            _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev).Append(_stateMan.MathSign).Build();
            _stateMan.HasSecondVal = true;
        }
        base.FinishRound(success);
    }


    #endregion Rounding

    #region Support Methods

    private (decimal value, int exp, string preview) GetSecondValue()
    {
        if (_stateMan.HasSecondVal)
        {
            if (_stateMan.IsCalculated) // Obtained after applying the math function
            {
                return (CalcEngine.Result, CalcEngine.ResultExp, AppData.SecondValPrev);
            }
            else // Entered manually
            {
                // 123,2150000 -> 123,215
                _dispServ.TrimZeros();
                return (_dispServ.GetDecimalValue(), 0, _dispServ.Input);
            }
        }
        else // Copy from the first value
        {
            _stateMan.HasSecondVal = true;
            return (AppData.FirstVal, AppData.FirstExp, AppData.FirstValPrev);
        }
    }

    private void PreviewFunction(string prefix)
    {
        if (_stateMan.IsCalculated)
        {
            AppData.SecondValPrev = $"{prefix}({AppData.SecondValPrev})";
        }
        else
        {
            AppData.SecondValPrev = $"{prefix}({AppData.SecondValPrev})";
        }

        // Math action display
        // .. + sqr(2 456,156)
        _dispServ.Preview = _prevBuilder.StartWith(AppData.FirstValPrev)
                                        .Append(_stateMan.MathSign)
                                        .Append(AppData.SecondValPrev)
                                        .Build();
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
        _dispServ.SetInputWith(CalcEngine.ErrorMsg);
        GlobalEvents.RiseCanExecuteChanged(false);
    }

    public override (decimal rational, int expVal) CurrentValue()
    {
        (decimal rational, int expVal, string prev) = GetSecondValue();
        return (rational, expVal);
    }

    #endregion Support Methods
}
