using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Enums;
using jp_conj_game.scripts;
using FileAccess = Godot.FileAccess;

public class WordReader
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
		public List<string> IAdj { get; set; }
		public List<string> NaAdj { get; set; }
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

	public static Word GetNormWord()
	{
		Word newWord = new Word();
		Random random = new Random();
		if (Config.isVerb)
		{
			newWord = new Word(_json.Godan[random.Next(0, _json.Godan.Count - 1)], type: EndingType.Godan, isVerb:true);
		}
		return newWord;
	}

	public static List<Word> GetWords(int length = 7, bool mix = false)
	{
		int len = length >= 7 ? length : 7;
		int cutOff = Config.isVerb ?  (len - 4) / 3 : len/2;
		List<Word> words = new List<Word>();
		List<int> visited = new List<int>();
		Random random = new Random();

		if (!mix && Config.isVerb)
		{
			//first get the 4 irregular verbs
			for (int i = 0; i < 4; i++)
			{
				words.Add(new Word(_json.Irregular[i], type:EndingType.Irregular, isVerb:true));
			}

			words.Concat(makeList(type:EndingType.Ichidan, length:cutOff, isVerb:true));
			words.Concat(makeList(type:EndingType.IrregularGodan, length:cutOff, isVerb:true));
			//the rest are godan
			words.Concat(makeList(type:EndingType.Godan, length: (length - 4) - cutOff*2, isVerb:true));
		}
		else if(!mix && !Config.isVerb)
		{
			words.Concat(makeList(type:EndingType.Na, length:cutOff, isVerb:false));
			words.Concat(makeList(type:EndingType.I, length: length - cutOff, isVerb:false));
		}
		else
		{
			cutOff = (length - 4) / 6;
			for (int i = 0; i < 4; i++)
			{
				words.Add(new Word(_json.Irregular[i], type:EndingType.Irregular, isVerb:true));
			}
			words.Concat(makeList(type:EndingType.Ichidan, length:cutOff, isVerb:true));
			words.Concat(makeList(type:EndingType.IrregularGodan, length:cutOff, isVerb:true));
			words.Concat(makeList(type:EndingType.Godan, length: cutOff, isVerb:true));
			words.Concat(makeList(type:EndingType.Na, length:cutOff, isVerb:false));
			words.Concat(makeList(type:EndingType.I, length: length - cutOff*6, isVerb:false));
		}
		
		
		Shuffle(words);
		return words;
	}

	private static List<Word> makeList(EndingType type = EndingType.Godan, int length = 1, bool isVerb = true)
	{
		List<int> visited = new List<int>();
		List<Word> words = new List<Word>();
		Random random =  new Random();
		
		List<string> array;
		switch (type)
		{
			case EndingType.Godan:
				array = _json.Godan;
				break;
			case EndingType.IrregularGodan:
				if (Config.getIrrGStatus())
				{
					array = _json.CommonIrrGodan;
				}
				else
				{
					array = _json.IrregularGodan;	
				}
				break;
			case EndingType.Ichidan:
				array = _json.Ichidan;
				break;
			case EndingType.Na:
				array = _json.NaAdj;
				break;
			case EndingType.I:
				array = _json.IAdj;
				break;
			default:
				array = _json.Godan;
				break;
		}
		
		int max = array.Count-1;
		for (int i = 0; i < length; i++)
		{
			int ran = random.Next(max);
			if (!visited.Contains(ran))
			{
				words.Add(new Word(array[ran], type:type));
				visited.Add(ran);
			}
			else
			{
				if (visited.Count-1 >= max)
				{
					visited.Clear();
				}

				i--;
			}
		}

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
