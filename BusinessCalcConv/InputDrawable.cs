using Font = Microsoft.Maui.Graphics.Font;

namespace BusinessCalculator
{
    public sealed class InputDrawable : IDrawable
    {
        public InputDrawable(float maxFontSize)
        {
            MaxFontSize = maxFontSize;
            _currentFontSize = MaxFontSize;
        }

        public float MaxFontSize { get; set; } = 48;

#if IOS
        //private const float DEFAULT_FONT_SIZE = 84;
        private const float PADDING = 70;
#else
        //private const float DEFAULT_FONT_SIZE = 64;
        private const float PADDING = 20;
#endif
        private string _text = "0";
        private float _currentFontSize;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FontColor = Colors.White;
            canvas.Font = Font.Default;
            SettingFontSize(canvas, dirtyRect.Width);
            canvas.FontSize = _currentFontSize;

            canvas.DrawString(_text, 0, 0, dirtyRect.Width, dirtyRect.Height, 
                HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        internal void OnTextChanged(string text)
        {
            _text = text;
        }

        private void SettingFontSize(ICanvas canvas, float width)
        {
            if (_text.Length > 9)
            {
                SizeF stringSize = canvas.GetStringSize(_text, Font.Default, _currentFontSize);
                if (stringSize.Width > width - PADDING)
                {
                    while (true)
                    {
                        _currentFontSize -= 1.0f;
                        stringSize = canvas.GetStringSize(_text, Font.Default, _currentFontSize);
                        if (stringSize.Width < width - PADDING)
                            return;
                    }
                }
                else
                {
                    while (true)
                    {
                        _currentFontSize += 1.0f;
                        if (_currentFontSize >= MaxFontSize)
                        {
                            _currentFontSize = MaxFontSize;
                            return;
                        }
                        stringSize = canvas.GetStringSize(_text, Font.Default, _currentFontSize);
                        if (stringSize.Width >= width - PADDING)
                        {
                            _currentFontSize -= 1.0f;
                            return;
                        }
                    }
                }
            }
            else
                _currentFontSize = MaxFontSize;
        }
    }
}
