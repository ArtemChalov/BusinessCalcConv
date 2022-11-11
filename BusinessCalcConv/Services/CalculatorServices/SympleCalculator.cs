using CalculatorLib;
using System.Globalization;

namespace BusinessCalculator.Services;

public class SympleCalculator : ISympleCalculator
{
    public static string DIVIDE_BY_ZERO_ERROR = CultureInfo.CurrentCulture.TextInfo.CultureName.ToLower() == "ru-ru"
       ? "Деление на ноль невозможно" : "Division by zero is not possible";
    public static string INVALID_INPUT_ERROR = CultureInfo.CurrentCulture.TextInfo.CultureName.ToLower() == "ru-ru"
        ? "Неверный ввод" : "Invalid input";
    public static string NUMBER_OUT_OF_RANGE_ERROR = CultureInfo.CurrentCulture.TextInfo.CultureName.ToLower() == "ru-ru"
       ? "Число вне диапазона" : "Number out of range";

    public SympleCalculator(int accuracy)
    {
        Accuracy = accuracy;
    }

    public string ErrorMsg { get; private set; } = "";
    public int Accuracy { get; } = 16;

    public bool TryCalculate(decimal firstVal, decimal secondVal, string mathSign, out decimal result)
    {
        if (mathSign == CalcSettings.DIVIDE_SIGN && secondVal == decimal.Zero)
        {
            ErrorMsg = DIVIDE_BY_ZERO_ERROR;
            result = decimal.Zero;
            return false;
        }

        try
        {
            switch (mathSign)
            {
                case CalcSettings.ADD_SIGN:
                    result = decimal.Round(decimal.Add(firstVal, secondVal), Accuracy);
                    return true;
                case CalcSettings.SUBTRACT_SIGN:
                    result = decimal.Round(decimal.Subtract(firstVal, secondVal), Accuracy);
                    return true;
                case CalcSettings.MULTIPLY_SIGN:
                    result = decimal.Round(decimal.Multiply(firstVal, secondVal), Accuracy);
                    return true;
                case CalcSettings.DIVIDE_SIGN:
                    result = decimal.Round(decimal.Divide(firstVal, secondVal), Accuracy);
                    return true;
                default:
                    result = firstVal;
                    return true;
            }
        }
        catch (Exception)
        {
            result = 0;
            ErrorMsg = NUMBER_OUT_OF_RANGE_ERROR;
            return false;
        }
    }

    public bool TrySqr(decimal value, out decimal result)
    {
        try
        {
            result = decimal.Round(decimal.Multiply(value, value), Accuracy);
            return true;    
        }
        catch (Exception)
        {
            result = 0;
            ErrorMsg = NUMBER_OUT_OF_RANGE_ERROR;
            return false;
        }
    }

    public bool TrySqrt(decimal value, out decimal result)
    {
        try
        {
            result = (decimal)(Math.Round(Math.Sqrt((double)value), 15));
            return true;
        }
        catch (Exception)
        {
            result = 0;
            ErrorMsg = INVALID_INPUT_ERROR;
            return false;
        }
    }

    public bool TryInvert(decimal value, out decimal result)
    {
        if (value == decimal.Zero)
        {
            ErrorMsg = DIVIDE_BY_ZERO_ERROR;
            result = decimal.Zero;
            return false;
        }

        try
        {
            result = decimal.Round(decimal.Divide(decimal.One, value), Accuracy);
            return true;
        }
        catch (Exception)
        {
            result = 0;
            ErrorMsg = INVALID_INPUT_ERROR;
            return false;
        }
    }

    public bool TryGetPercent(decimal baseVal, decimal percent, out decimal result)
    {
        try
        {
            decimal onePercent = decimal.Multiply(baseVal, 0.01M);
            result = decimal.Round(decimal.Multiply(onePercent, Math.Abs(percent)), Accuracy);
            return true;
        }
        catch (Exception)
        {
            result = 0;
            ErrorMsg = NUMBER_OUT_OF_RANGE_ERROR;
            return false;
        }
    }
}
