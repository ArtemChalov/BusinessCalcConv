using BusinessCalculator.MVVM.ViewModels;

namespace BusinessCalculator.MVVM.Views;

public partial class CalculatorAndroidView : ContentPage
{
	public CalculatorAndroidView(CalculatorViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}