using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Enums;
using jp_conj_game.scripts;
using FileAccess = Godot.FileAccess;

public class VerbReader
{
	private static string _path = "res://json/verbList.json";
	private static FileAccess _file;
	private static Json _json;
	public class Json
	{
		public List<string> Irregular { get; set; }
		public List<string> CommonIrrGodan { get; set; }
		public List<string> IrregularGodan { get; set; }
		public List<string> Godan { get; set; }
		public List<string> Ichidan { get; set; }
	}

	public static void Load()
	{
		if (FileAccess.FileExists(_path))
		{
			_file = FileAccess.Open(_path,FileAccess.ModeFlags.ReadWrite);
			string temp = _file.GetAsText();
			
			_json = JsonSerializer.Deserialize<Json>(temp);
		}
		else
		{
			GD.Print("count not open verb list");
		}
	}

	public static void Save()
	{
		if (_file != null && FileAccess.FileExists(_path))
		{
			string temp = JsonSerializer.Serialize(_json, new JsonSerializerOptions{WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping});
			_file.StoreString(temp);
		}
		else
		{
			GD.Print("saving to verb list failed");
		}
	}

	public static List<Word> GetWords(int length = 7)
	{
		int len = length >= 7 ? length : 7;
		int cutOff = (len - 4) / 3;
		string currWord = "";
		List<Word> words = new List<Word>();
		List<int> visited = new List<int>();
		Random random = new Random();
		
		GD.Print(_json.Irregular);
		
		//first get the 4 irregular verbs
		for (int i = 0; i < 4; i++)
		{
			words.Add(new Word(_json.Irregular[i], type:VerbType.Irregular));
		}

		if (!Config.getIrrGStatus())
		{
			int maxIrrGodan = _json.IrregularGodan.Count-1;
			for (int i = 0; i < cutOff; i++)
			{
				int ran = random.Next(maxIrrGodan);
				if (!visited.Contains(ran))
				{
					words.Add(new Word(_json.IrregularGodan[ran], type:VerbType.IrregularGodan));
					visited.Add(ran);
				}
				else
				{
					//if it has gone through all of it just make dupes
					if (visited.Count >= maxIrrGodan)
					{
						visited.Clear();
					}
					i--;
				}
			}
		}
		else
		{
			int maxIrrGodan = _json.CommonIrrGodan.Count-1;
			for (int i = 0; i < cutOff; i++)
			{
				int ran = random.Next(maxIrrGodan);
				if (!visited.Contains(ran))
				{
					words.Add(new Word(_json.CommonIrrGodan[ran], type:VerbType.IrregularGodan));
					visited.Add(ran);
				}
				else
				{
					//if it has gone through all of it just make dupes
					if (visited.Count >= maxIrrGodan)
					{
						visited.Clear();
					}
					i--;
				}
			}
		}

		visited.Clear();
		int maxIchidan = _json.Ichidan.Count-1;
		for (int i = 0; i < cutOff; i++)
		{
			int ran = random.Next(maxIchidan);
			if (!visited.Contains(ran))
			{
				words.Add(new Word(_json.Ichidan[ran], type:VerbType.Ichidan));
				visited.Add(ran);
			}
			else
			{
				//if it has gone through all of it just make dupes
				if (visited.Count >= maxIchidan)
				{
					visited.Clear();
				}
				i--;
			}
		}
		
		visited.Clear();
		int maxGodan = _json.Godan.Count-1;
		//diff equation in case of a decimal
		//(the rest of the list will be godan)
		for (int i = 0; i < length - 4 - cutOff*2; i++)
		{
			int ran = random.Next(maxGodan);
			if (!visited.Contains(ran))
			{
				words.Add(new Word(_json.Godan[ran], type:VerbType.Godan));
				visited.Add(ran);
			}
			else
			{
				//if it has gone through all of it just make dupes
				if (visited.Count >= maxGodan)
				{
					visited.Clear();
				}
				i--;
			}
		}
		
		Shuffle(words);
		return words;
	}
	
	private static void Shuffle<T>(List<T> list)
	{
		Random random = new Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = random.Next(n + 1);
			(list[n], list[k]) = (list[k], list[n]); // swap
		}
	}
	
}
