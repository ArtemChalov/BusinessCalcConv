using BusinessCalculator.MVVM.ViewModels;
using BusinessCalculator.MVVM.Views;
using BusinessCalculator.ConverterService;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BusinessCalculator;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

#if IOS    
        builder.Services.AddSingleton<CalculatorIOSView>();
        builder.Services.AddSingleton<ConverterIOSView>();
#else
        builder.Services.AddSingleton<CalculatorAndroidView>();
        builder.Services.AddSingleton<ConverterAndroidView>();
#endif
        // Calculator init
        builder.Services.AddSingleton<CalculatorViewModel>();
        builder.Services.AddSingleton<ConverterViewModel>();

        builder.Services.AddSingleton<IConverterDisplayService>(
            new DefaultConverterDisplayService(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator,
                                               NumberFormatInfo.CurrentInfo.NumberGroupSeparator,
                                               16));

        return builder.Build();
	}
}
