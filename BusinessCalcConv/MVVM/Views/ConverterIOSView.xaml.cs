using BusinessCalculator.MVVM.ViewModels;

namespace BusinessCalculator.MVVM.Views;

public partial class ConverterIOSView : ContentPage
{
	public ConverterIOSView(ConverterViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}