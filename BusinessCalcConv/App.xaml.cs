namespace BusinessCalculator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

#if IOS    
        MainPage = new AppIOSShell();
#else
        MainPage = new AppAndroidShell();
#endif
	}
}
