using System.Text.Json;

namespace AssetRipper.TypeTreeCompression.UnitySerialization
{
	public sealed class UnityInfo
	{
		private string version = "";
		private List<UnityString> strings = new();
		private List<UnityClass> classes = new();

		public string Version 
		{ 
			get => version; 
			set => version = value ?? ""; 
		}

		public List<UnityString> Strings 
		{ 
			get => strings; 
			set => strings = value ?? new(); 
		}

		public List<UnityClass> Classes 
		{ 
			get => classes; 
			set => classes = value ?? new(); 
		}

		public static UnityInfo ReadFromJsonFile(string jsonPath)
		{
			string text = File.ReadAllText(jsonPath);
			return JsonSerializer.Deserialize<UnityInfo>(text) ?? throw new Exception($"Failed to deserialize {jsonPath}");
		}
	}
}