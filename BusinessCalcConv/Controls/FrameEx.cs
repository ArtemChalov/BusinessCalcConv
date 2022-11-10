namespace BusinessCalculator.Controls;

public sealed class FrameEx : Frame
{
    #region Property Registration

    public static readonly BindableProperty TagProperty =
        BindableProperty.Create("Tag", typeof(object), typeof(GraphicsViewEx), null, BindingMode.TwoWay);

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create("IsChecked", typeof(bool), typeof(GraphicsViewEx), false, BindingMode.TwoWay);

    #endregion Property Registration

    #region Properties

    public object? Tag
    {
        get => (object?)GetValue(TagProperty);
        set => SetValue(TagProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    #endregion Properties
}
