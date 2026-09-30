using System.Collections.ObjectModel;
using System.ComponentModel;
namespace Countly;

public class Counter : INotifyPropertyChanged
{
	private int _value;
	public string Name { get; set; } = "";

	public int Value
	{
		get => _value;
		set
		{
			_value = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class MainPage : ContentPage
{
	private ObservableCollection<Counter> counters = new()
	{
		new Counter { Name = "Licznik", Value = 0 }
	};

	public MainPage()
	{
		InitializeComponent();
		CountersList.ItemsSource = counters;
	}

	private void OnPlus(object sender, EventArgs e)
	{
		
	}
}

