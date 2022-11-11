using CalculatorLib;

namespace BusinessCalculator.States
{
    public sealed class StateManager : IStateManager
    {
        private readonly FirstInputState _firstInputState;
        private readonly SecondInputState _secondInputState;

        public StateManager(IDisplayService displayService)
        {
            _firstInputState = new(this, displayService);
            _secondInputState = new(this, displayService);
            InputState = _firstInputState;
        }

        public InputStateBase InputState { get; private set; }

        public string MathSign { get; set; } = CalcSettings.NO_SIGN;
        public bool HasMathSign => !string.IsNullOrEmpty(MathSign);
        public bool IsCalculated { get; set; } = false;
        public bool HasSecondVal { get; set; } = false;

        public bool HasError { get; private set; }

        public void SetState(InputStates inputState)
        {
            if (inputState == InputStates.FirstState)
            {
                InputState = _firstInputState;
            }
            else
            {
                InputState = _secondInputState;
                AppData.SecondVal = decimal.Zero;
                AppData.SecondExp = 0;
                AppData.SecondValPrev = "0";
            }
            HasSecondVal = false;
        }

        public void ResetStateToDefault()
        {
            InputState = _firstInputState;
            AppData.FirstVal = decimal.Zero;
            AppData.FirstExp = 0;
            AppData.FirstValPrev = "0";
            AppData.SecondVal = decimal.Zero;
            AppData.SecondExp = 0;
            AppData.SecondValPrev = "0";
            CalcEngine.Result = decimal.Zero;
            CalcEngine.ResultExp = 0;
            HasSecondVal = false;
            MathSign = CalcSettings.NO_SIGN;
            GlobalEvents.RiseCanExecuteChanged(true);
            IsCalculated = false;
        }
    }
}
