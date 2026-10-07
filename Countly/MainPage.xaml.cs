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
	private readonly ObservableCollection<Counter> counters = new()
	{
		new Counter { Name = "Licznik", Value = 0 }
	};

	public MainPage()
	{
		InitializeComponent();
		CountersList.ItemsSource = counters;
	}

	private void OnPlus(object? sender, EventArgs e)
	{
		var counter = (Counter)((Button)sender!).BindingContext;
		counter.Value++;
	}

	private void OnMinus(object? sender, EventArgs e)
	{
		var counter = (Counter)((Button)sender!).BindingContext;
		counter.Value--;
	}
	
	private async void OnAdd(object sender, EventArgs e)
	{
		string? name = await DisplayPromptAsync("Nowy licznik", "Nazwa:");
		if (string.IsNullOrWhiteSpace(name)) return;

		string? start = await DisplayPromptAsync("Nowy licznik", "Wartość początkowa:",
			initialValue: "0", keyboard: Keyboard.Numeric);
		int.TryParse(start, out int value);

		counters.Add(new Counter { Name = name, Value = value });
	}
}
