using System.Collections.Generic;
using Godot;
using Enums;
namespace jp_conj_game.scripts;

public partial class VerbMaker : Label
{
    public enum Combonation
    {
        Polite,
        PoliteNegative,
        PolitePast,
        PoliteNegativePast,
        Plain,
        PlainNegative,
        PlainPast,
        PlainNegativePast
    }

    private string _line { get; set; }
    public string _currWord { get; private set; }
    public string _conjWord { get; private set; }
    public Combonation _combonation { get; private set; }
    public TenseType tenseType { get; private set; }
    public Positivity positivityType { get; private set; }
    public Politeness politenessType { get; private set; }
    public VerbType _verbType { get; private set; }
    public ConjType _conjType { get; private set; }
    private EndingType _endingType { get; set; }

    //making the word with furigana on screen
    private List<Label> _wordsForScreen;

    //conjugate the romaji word to pass over to answer
    public VerbMaker(Word word = null,
        TenseType tense= TenseType.Present,
        Positivity positivity= Positivity.Positive,
        Politeness politeness= Politeness.Plain,
        ConjType conjType= ConjType.None,
        int x=1280,
        int y=150,
        int size = 100)
    {
        if (word == null)
        {
            GD.Print("the word is null in verb maker");
            return;
        }
        tenseType = tense;
        positivityType = positivity;
        politenessType = politeness;
        
        _line = word.GetLine();
        _currWord = word.GetRomaji();
        _verbType = word.GetType();
        _conjType = conjType;
        _conjWord = word.GetRomaji();
        char finalChar = _line[_line.Length-1];

        switch (finalChar)
        {
            case 'う':
            case 'る':
                _endingType = EndingType.Uru;
                break;
            case 'つ':
                _endingType = EndingType.Tsu;
                break;
            case 'す':
                _endingType = EndingType.Su;
                break;
            case 'ぬ':
            case 'ぶ':
            case 'む':
                _endingType = EndingType.Nubumu;
                break;
            case 'く':
                _endingType = EndingType.Ku;
                break;
            case 'ぐ':
                _endingType = EndingType.Gu;
                break;
        }
        
        var random = new RandomNumberGenerator();
        if (tense == TenseType.Both) {tense = random.Randi() % 2 == 1 ? TenseType.Present : TenseType.Past; }

        if (positivity == Positivity.Both) {positivity = random.Randi() % 2 == 1 ? Positivity.Positive : Positivity.Negative; }

        if (politeness == Politeness.Both) {politeness = random.Randi() % 2 == 1 ? Politeness.Polite : Politeness.Plain; }
        
        tenseType = tense;
        positivityType = positivity;
        politenessType = politeness;

        //make the combination to save on a million if statements in the conj part
        if (politeness == Politeness.Polite)
        {
            if (tense == TenseType.Present)
            {
                if (positivity == Positivity.Positive)
                {
                    _combonation = Combonation.Polite;
                }
                //negative
                else
                {
                    _combonation = Combonation.PoliteNegative;
                }
            }
            //past
            else
            {
                if (positivity == Positivity.Positive)
                {
                    _combonation = Combonation.PolitePast;
                }
                //negative
                else
                {
                    _combonation = Combonation.PoliteNegativePast;
                }
            }
        }
        //plain
        else
        {
            if (tense == TenseType.Present)
            {
                if (positivity == Positivity.Positive)
                {
                    _combonation = Combonation.Plain;
                }
                //negative
                else
                {
                    _combonation = Combonation.PlainNegative;
                }
            }
            //past
            else
            {
                if (positivity == Positivity.Positive)
                {
                    _combonation = Combonation.PlainPast;
                }
                //negative
                else
                {
                    _combonation = Combonation.PlainNegativePast;
                }
            }
        }

        switch (_conjType)
        {
            case ConjType.None:
                ConjNone();
                break;
            case ConjType.Te:
                ConjTe();
                break;
        }
    
        OnScreenJPText textMaker =  new OnScreenJPText();
        _wordsForScreen = textMaker.JpLabel(word,x,y,size);

    }

    public List<Label> GetLabels()
    {
        return _wordsForScreen;
    }
    
    //special for odd cases like the te form
    private void ChangeStem(string stem = "i", bool special = false)
    {
        byte cutOff = 1;
        if (_verbType == VerbType.Godan || _verbType == VerbType.IrregularGodan)
        {
            if (special) cutOff = (byte)(_endingType == EndingType.Tsu ? 3 : 2);
            _conjWord = _conjWord.Substring(0, _conjWord.Length - cutOff) + stem;
        }
        //ichidan
        else
        {
            cutOff = 2;
            if (special)
            {
                _conjWord = _conjWord.Substring(0, _conjWord.Length - cutOff) + stem;
            }
            else
            {
                _conjWord = _conjWord.Substring(0, _conjWord.Length - cutOff);
            }
        }
    }

    private void ConjNone()
    {
        bool irregular = _verbType == VerbType.Irregular ? true : false;
        string ending = "";
        switch (_combonation)
        {
            case Combonation.Polite:
                ending = "masu";
                if (!irregular)
                {
                    ChangeStem("i");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord.ToLower())
                    {
                        case "kuru":
                            _conjWord = "ki" + ending;
                            break;
                        case "suru":
                            _conjWord = "shi" + ending;
                            break;
                        case "iku":
                            _conjWord = "iki" + ending;
                            break;
                        case "aru":
                            _conjWord = "ari" + ending;
                            break;
                    }
                }
                break;
            case Combonation.PoliteNegative:
                ending = "masen";
                if (!irregular)
                {
                    ChangeStem("i");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord)
                    {
                        case "kuru":
                            _conjWord = "ki" + ending;
                            break;
                        case "suru":
                            _conjWord = "shi" + ending;
                            break;
                        case "iku":
                            _conjWord = "iki" + ending;
                            break;
                        case "aru":
                            _conjWord = "ari" + ending;
                            break;
                    }
                }
                break;
            case Combonation.PolitePast:
                ending = "mashita";
                if (!irregular)
                {
                    ChangeStem("i");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord)
                    {
                        case "kuru":
                            _conjWord = "ki" + ending;
                            break;
                        case "suru":
                            _conjWord = "shi" + ending;
                            break;
                        case "iku":
                            _conjWord = "iki" + ending;
                            break;
                        case "aru":
                            _conjWord = "ari" + ending;
                            break;
                    }
                }
                break;
            case Combonation.PoliteNegativePast:
                ending = "masendeshita";
                if (!irregular)
                {
                    ChangeStem("i");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord)
                    {
                        case "kuru":
                            _conjWord = "ki" + ending;
                            break;
                        case "suru":
                            _conjWord = "shi" + ending;
                            break;
                        case "iku":
                            _conjWord = "iki" + ending;
                            break;
                        case "aru":
                            _conjWord = "ari" + ending;
                            break;
                    }
                }
                break;
            case Combonation.Plain:
                _conjWord = _currWord;
                break;
            case Combonation.PlainNegative:
                ending = "nai";
                if (!irregular)
                {
                    ChangeStem("a");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord)
                    {
                        case "kuru":
                            _conjWord = "ko" + ending;
                            break;
                        case "suru":
                            _conjWord = "shi" + ending;
                            break;
                        case "iku":
                            _conjWord = "iki" + ending;
                            break;
                        case "aru":
                            _conjWord = "nai";
                            break;
                    }
                }
                break;
            case Combonation.PlainPast:
                if (!irregular)
                {
                    switch (_endingType)
                    {
                        case EndingType.Tsu:
                        case EndingType.Uru:
                            ending = "tta";
                            break;
                        case EndingType.Su:
                            ending = "shita";
                            break;
                        case EndingType.Ku:
                            ending = "ita";
                            break;
                        case EndingType.Gu:
                            ending = "ida";
                            break;
                        case EndingType.Nubumu:
                            ending = "nda";
                            break;
                    }
                    ChangeStem("i");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord)
                    {
                        case "kuru":
                            _conjWord = "kita";
                            break;
                        case "suru":
                            _conjWord = "shita";
                            break;
                        case "iku":
                            _conjWord = "kita";
                            break;
                        case "aru":
                            _conjWord = "atta";
                            break;
                    }
                }
                break;
            case Combonation.PlainNegativePast:
                ending = "nakatta";
                if (!irregular)
                {
                    ChangeStem("a");
                    _conjWord += ending;
                }
                else
                {
                    switch (_currWord)
                    {
                        case "kuru":
                            _conjWord = "ko" + ending;
                            break;
                        case "suru":
                            _conjWord = "shi" + ending;
                            break;
                        case "iku":
                            _conjWord = "ika" + ending;
                            break;
                        case "aru":
                            _conjWord = "na" + ending;
                            break;
                    }
                }
                break;
        }
    }
    
    private void ConjTe(){}

    public void PrintLine()
    {
        GD.Print($"line is {_line} | conj word is {_conjWord}");
    }

}