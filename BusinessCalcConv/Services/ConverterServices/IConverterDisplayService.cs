using PDSMathLib;

namespace BusinessCalculator.ConverterService;

public interface IConverterDisplayService
{
    string FirstInput { get; }
    string SecondInput { get; }
    string ThirdInput { get; }
    string SelectedConverterName { get; set; }

    PdsNumber FirstValue { set; }
    PdsNumber SecondValue { set; }
    PdsNumber ThirdValue { set; }

    string DecimalSeparator { get; init; }
    string NumberGroupSeparator { get; init; }
    int MaxInputNumber { get; init; }

    int InputCounterFirst { get; set; }
    bool HasDecimalFirst { get; set; }
    int InputCounterSecond { get; set; }
    bool HasDecimalSecond { get; set; }
    int InputCounterThird { get; set; }
    bool HasDecimalThird { get; set; }

    ValueLines CurrentLine { get; set; }

    PdsNumber GetDecimalValue();
    void SetInputWith(PdsNumber number);
    void SetInputWith(string value);

    void AppendDigit(string digit);
    void RemoveLastDigit();
    void TrimZeros();
    void FullReset();
    void StateReset();

    int MinExpanentValue { get; set; }
    int MaxExpanentValue { get; set; }
}