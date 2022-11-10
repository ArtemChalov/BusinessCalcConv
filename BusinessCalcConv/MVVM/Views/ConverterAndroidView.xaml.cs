using BusinessCalculator.MVVM.ViewModels;

namespace BusinessCalculator.MVVM.Views;

public partial class ConverterAndroidView : ContentPage
{
	public ConverterAndroidView(ConverterViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}