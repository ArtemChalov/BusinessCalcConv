namespace BusinessCalculator.Controls;

public sealed class GraphicsViewEx : GraphicsView
{
    public GraphicsViewEx() : base()
    {
        Drawable = new InputDrawable(MaxFontSize);
    }

    #region Property Registration

    public static readonly BindableProperty TextProperty = 
        BindableProperty.Create("Text", typeof(string), typeof(GraphicsViewEx), "0", BindingMode.OneWay, propertyChanged: OnTextChanged);

#if WINDOWS
        public static readonly BindableProperty MaxFontSizeProperty =
        BindableProperty.Create("MaxFontSize", typeof(float), typeof(GraphicsViewEx), 48.0f, BindingMode.OneWay, propertyChanged: OnMaxFontSizeChanged);
#elif IOS
        public static readonly BindableProperty MaxFontSizeProperty =
        BindableProperty.Create("MaxFontSize", typeof(float), typeof(GraphicsViewEx), 84.0f, BindingMode.OneWay, propertyChanged: OnMaxFontSizeChanged);
#else
    public static readonly BindableProperty MaxFontSizeProperty = 
        BindableProperty.Create("MaxFontSize", typeof(float), typeof(GraphicsViewEx), 64.0f, BindingMode.OneWay, propertyChanged: OnMaxFontSizeChanged);

#endif


    #endregion Property Registration

    #region Properties

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public float MaxFontSize
    {
        get => (float)GetValue(MaxFontSizeProperty);
        set => SetValue(MaxFontSizeProperty, value);
    }

    #endregion Properties

    #region Property CallBacks

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        GraphicsViewEx graphicsViewEx = (GraphicsViewEx)bindable;

        ((InputDrawable)graphicsViewEx.Drawable).OnTextChanged(newValue?.ToString() ?? "0");
        graphicsViewEx.Invalidate();
    }

    private static void OnMaxFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        GraphicsViewEx graphicsViewEx = (GraphicsViewEx)bindable;

        ((InputDrawable)graphicsViewEx.Drawable).MaxFontSize = (float)newValue;
        graphicsViewEx.Invalidate();
    }

    #endregion Property CallBacks
}
