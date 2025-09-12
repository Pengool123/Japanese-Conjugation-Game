using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Godot;
using Enums;

namespace jp_conj_game.scripts;

public class Word
{
    public string line { get; private set; }

    //no def
    public string wordLine { get; private set; }

    public string noFurigana { get; private set; }

    public string onlyGana;
    
    public string romaji { get; private set; }

    public bool isVerb { get; private set; }


    private List<string> _defs = new List<string>();

    public List<string> GetDefs()
    {
        return _defs;
    }

    public EndingType type { get; private set; }


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

    public Word(string inputLine = "来[く]る{to come}", EndingType type = EndingType.Godan, bool isVerb = true)
    {
        
        GD.Print(line + type);
        line = inputLine;
        this.type = type;
        this.isVerb = isVerb;
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
                    wordLine += i;
                    break;
                case ']':
                    inFurigana = false;
                    wordLine += i;
                    break;
                case '{':
                    defStart = true;
                    break;
                default:

                    if (!defStart)
                    {
                        wordLine += i;
                        if (!inFurigana)
                        {
                            noFurigana += i;
                        }

                        if (isGana(i))
                        {
                            onlyGana += i;
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
        
        foreach (char i in onlyGana)
        {
            int index = i - hiraganaOffset;
            string rom = _enSounds[index];

            smallChar = true;
            switch (rom)
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
                    romaji += rom[0];
                }

                romaji += rom;
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
                    romaji += prevChar[0] + tempAddon;
                }
                //shi, chi, ji
                else
                {
                    romaji += prevChar.Substring(0, prevChar.Length - 1) + tempAddon[1];
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
        GD.Print($"word line is: {line} | no furigana is: {noFurigana} | only gana is: {onlyGana} | romaji is {romaji} | the defs are");
        foreach (var i in _defs)
        {
            GD.Print(i);
        }
    }
}