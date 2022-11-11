using CalculatorLib;
using CalculatorLib.Extentions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessCalculator.Services
{
    [ObservableObject]
    public sealed partial class MemoryService : IMemoryService
    {
        private const string FILE_NAME = "memory.txt";
        private readonly IReposytory _reposytary;

        public MemoryService(IReposytory reposytary)
        {
            _reposytary = reposytary;
            InitAsync(_reposytary);
        }

        [ObservableProperty]
        private string _MemoryValue = "0";

        public decimal Rational { get; private set; } = decimal.Zero;
        public int ExpValue { get; private set; } = 0;

        private async void InitAsync(IReposytory reposytary)
        {
            string? str = await _reposytary.LoadDataAsync(FILE_NAME);
            
            if (!string.IsNullOrEmpty(str))
            {
                string[] val = str.Split(";");
                if (val.Length == 2)
                {
                    if (decimal.TryParse(val[0], out decimal rat))
                    {
                        Rational = rat;
                        _ = int.TryParse(val[1], out int exp);
                        ExpValue = exp;
                        MemoryValue = Rational.ToFormattedString(ExpValue);
                    }
                    else
                    {
                        MemoryValue = "0";
                        Rational = decimal.Zero;
                        ExpValue = 0;
                    }
                }
                else
                {
                    MemoryValue = "0";
                    Rational = decimal.Zero;
                    ExpValue = 0;
                }
            }
        }

        public void ResetMemory()
        {
            MemoryValue = "0";
            Rational = decimal.Zero;
            ExpValue = 0;
            _reposytary.SaveDataAsync(FILE_NAME, MemToString());
        }

        public void ChangeMemoryValue(decimal rational, int expValue, string mathSign)
        {
            switch (mathSign)
            {
                case CalcSettings.ADD_SIGN:
                    if (AddSubtractMemory(rational, expValue, mathSign))
                    {
                        _reposytary.SaveDataAsync(FILE_NAME, MemToString());
                    }
                    break;
                case CalcSettings.SUBTRACT_SIGN:
                    if (AddSubtractMemory(rational, expValue, mathSign))
                    {
                        _reposytary.SaveDataAsync(FILE_NAME, MemToString());
                    }
                    break;
                default:
                    break;
            }
        }

        private bool AddSubtractMemory(decimal rational, int expValue, string mathSign)
        {
            try
            {
                (Rational, ExpValue) = CalcEngine.AddSubtract(Rational, ExpValue, rational, expValue, mathSign == CalcSettings.ADD_SIGN);

                MemoryValue = Rational.ToFormattedString(ExpValue);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string MemToString() => string.Join(";", Rational.ToString(), ExpValue.ToString());
    }
}
