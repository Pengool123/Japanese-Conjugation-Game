using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Godot;
using Enums;

namespace jp_conj_game.scripts;

public class Word
{
    private string _line;

    public string GetLine()
    {
        return _line;
    }

    //no def
    private string _wordLine;
    public string GetWordLine()
    {
        return _wordLine;
    }

    private string _noFurigana;

    public string GetNoFuriga()
    {
        return _noFurigana;
    }

    private string _onlyGana;

    public string GetOnlyGana()
    {
        return _onlyGana;
    }
    private string _romaji { get; set; }

    public string GetRomaji()
    {
        return _romaji;
    }

    private List<string> _defs = new List<string>();

    public List<string> GetDefs()
    {
        return _defs;
    }

    private VerbType _type;

    public VerbType GetType()
    {
        return _type;
    }

    private List<string> _enSounds =
    [
        "a", "a", "i", "i", "u", "u", "e", "e", "o", "o",
        "ka", "ga", "ki", "gi", "ku", "gu", "ke", "ge", "ko", "go",
        "sa", "za", "shi", "ji", "su", "zu", "se", "ze", "so", "zo",
        "ta", "da", "chi", "ji", "0", "tsu", "dzu", "te", "de", "to", "do",
        "na", "ni", "nu", "ne", "no",
        "ha", "ba", "pa", "hi", "bi", "pi", "fu", "bu", "pu", "he", "be", "pe", "ho", "bo", "po",
        "ma", "mi", "mu", "me", "mo",
        "1", "ya", "2", "yu", "3", "yo",
        "ra", "ri", "ru", "re", "ro",
        "wa", "wa", "wi", "we", "wo",
        "n", "v", "ka", "ke"
    ];

    public Word(string line = "来[く]る{to come/kaboom}", VerbType type = VerbType.Godan)
    {
        GD.Print(line + type);
        _line = line;
        _type = type;
        _defs = new List<string>();
        
        bool inFurigana = false;
        bool defStart = false;
        string def = "";
        
        foreach (char i in line)
        {
            switch (i)
            {
                case '[':
                    inFurigana = true;
                    _wordLine += i;
                    break;
                case ']':
                    inFurigana = false;
                    _wordLine += i;
                    break;
                case '{':
                    defStart = true;
                    break;
                default:

                    if (!defStart)
                    {
                        _wordLine += i;
                        if (!inFurigana)
                        {
                            _noFurigana += i;
                        }

                        if (isGana(i))
                        {
                            _onlyGana += i;
                        }
                    }
                    else
                    {
                        if (i == '/' || i == '}')
                        {
                            if (def[0] == ' ')
                            {
                                def = def.Substring(1);
                            }
                            if (def[def.Length - 1] == ' ')
                            {
                                def = def.Substring(0, def.Length - 1);
                            }
                            _defs.Add(def);
                            def = "";
                        }
                        else
                        {
                            def += i;
                        }
                    }
                    
                    break;
            }
        }
        toRomaji();
        
    }

    //converts only hiragana to romaji
    private void toRomaji()
    {
        string prevChar = "";
        string tempAddon = "";      //small characters
        int hiraganaOffset = 12353;
        bool smallChar = true;
        
        foreach (char i in _onlyGana)
        {
            int index = i - hiraganaOffset;
            string romaji = _enSounds[index];

            smallChar = true;
            switch (romaji)
            {
                case "1":
                    tempAddon = "ya";
                    break;
                case "2":
                    tempAddon = "yu";
                    break;
                case "3":
                    tempAddon = "yo";
                    break;
                case "0":
                    prevChar = "0";
                    break;
                default:
                    smallChar = false;
                    break;
            }

            if (!smallChar)
            {
                //small tsu
                if (prevChar == "0")
                {
                    _romaji += romaji[0];
                }

                _romaji += romaji;
            }
            //on a small character, check the prev character
            else
            {
                if (prevChar == "ki" ||
                   prevChar == "ni" ||
                   prevChar == "hi" ||
                   prevChar == "mi" ||
                   prevChar == "ri" ||
                   prevChar == "gi")
                {
                    _romaji += prevChar[0] + tempAddon;
                }
                //shi, chi, ji
                else
                {
                    _romaji += prevChar.Substring(0, prevChar.Length - 1) + tempAddon[1];
                }
            }

            prevChar = romaji;
        }
    }
    
    public bool isGana(char character)
    {
        int hiraganaOffset = 12353;
        byte hiraganaMax = 85;
        int index = character - hiraganaOffset;
        
        if (index >= 0 && index < hiraganaMax)
        {
            return true;
        }
        
        return false;
    }

    public void printLine()
    {
        GD.Print($"word line is: {_line} | no furigana is: {_noFurigana} | only gana is: {_onlyGana} | romaji is {_romaji} | the defs are");
        foreach (var i in _defs)
        {
            GD.Print(i);
        }
    }
}