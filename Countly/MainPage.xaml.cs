using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Countly;

public class Counter : INotifyPropertyChanged
{
	private int _value;
	public string Name { get; set; } = "";
	public string ColorHex { get; set; } = MainPage.DefaultCounterColorHex;

	[XmlIgnore]
	public Color AccentColor => Color.FromArgb(ColorHex);

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
	public const string DefaultCounterColorHex = "#6C5CE7";

	private static readonly CounterColor[] CounterColors =
	[
		new("Fioletowy", DefaultCounterColorHex),
		new("Niebieski", "#2563EB"),
		new("Turkusowy", "#0891B2"),
		new("Zielony", "#16A34A"),
		new("Limonkowy", "#65A30D"),
		new("Żółty", "#CA8A04"),
		new("Pomarańczowy", "#EA580C"),
		new("Czerwony", "#DC2626"),
		new("Różowy", "#DB2777"),
		new("Grafitowy", "#475569")
	];

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

		string? selectedColor = await DisplayActionSheet(
			"Kolor licznika",
			"Anuluj",
			null,
			CounterColors.Select(c => c.Name).ToArray());

		if (selectedColor is null or "Anuluj") return;

		string colorHex = CounterColors.First(c => c.Name == selectedColor).Hex;

		counters.Add(new Counter { Name = name, Value = value, ColorHex = colorHex });
		Save();
	}

	private void OnDelete(object sender, EventArgs e)
	{
		var counter = (Counter)((Button)sender).BindingContext;
		counters.Remove(counter);
		Save();
	}

	private sealed record CounterColor(string Name, string Hex);
}
