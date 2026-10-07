using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
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
	private readonly ObservableCollection<Counter> counters = new();
	private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "counters.xml");
	
	public MainPage()
	{
		InitializeComponent();
		Load();
		CountersList.ItemsSource = counters;
	}

	private void Save()
	{
		var serializer = new XmlSerializer(typeof(List<Counter>));
		using var stream = File.Create(filePath);
		serializer.Serialize(stream, counters.ToList());
	}

	private void Load()
	{
		if (File.Exists(filePath))
		{
			var serializer = new XmlSerializer(typeof(List<Counter>));
			using var stream = File.OpenRead(filePath);
			var list = (List<Counter>)serializer.Deserialize(stream);
			foreach (var c in list) counters.Add(c);
		}
	}

	private void OnPlus(object? sender, EventArgs e)
	{
		var counter = (Counter)((Button)sender!).BindingContext;
		counter.Value++;
		Save();
	}

	private void OnMinus(object? sender, EventArgs e)
	{
		var counter = (Counter)((Button)sender!).BindingContext;
		counter.Value--;
		Save();
	}
	
	private async void OnAdd(object sender, EventArgs e)
	{
		string? name = await DisplayPromptAsync("Nowy licznik", "Nazwa:");
		if (string.IsNullOrWhiteSpace(name)) return;

		string? start = await DisplayPromptAsync("Nowy licznik", "Wartość początkowa:",
			initialValue: "0", keyboard: Keyboard.Numeric);
		int.TryParse(start, out int value);

		counters.Add(new Counter { Name = name, Value = value });
		Save();
	}

	private void OnDelete(object sender, EventArgs e)
	{
		var counter = (Counter)((Button)sender).BindingContext;
		counters.Remove(counter);
		Save();
	}
}
