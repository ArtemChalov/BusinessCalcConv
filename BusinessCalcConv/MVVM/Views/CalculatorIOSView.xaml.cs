using BusinessCalculator.MVVM.ViewModels;

namespace BusinessCalculator.MVVM.Views;

public partial class CalculatorIOSView : ContentPage
{
	public CalculatorIOSView(CalculatorViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}