using System.Collections.Generic;
using Godot;
using Enums;
namespace jp_conj_game.scripts;

public partial class conjMaker : Label
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
    public string _conjWord { get; private set; }
    private bool _isVerb {get; set;}
    private bool _irregular {get; set;}
    private bool _iAdj { get; set; }
    public Combonation _combonation { get; private set; }
    public TenseType tenseType { get; private set; }
    public Positivity positivityType { get; private set; }
    public Politeness politenessType { get; private set; }
    public EndingType EndingType { get; private set; }
    public ConjType _conjType { get; private set; }
    private VerbEndingType VerbEndingType { get; set; }

    public List<string> Defs { get; private set; }

    //making the word with furigana on screen
    private List<Label> _wordsForScreen;

    //conjugate the romaji word to pass over to answer
    public conjMaker(Word word = null,
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
        
        Defs = word.GetDefs();

        _irregular = EndingType == EndingType.Irregular;
        _iAdj = EndingType == EndingType.I;
        
        _isVerb = word.isVerb;
        _line = word.line;
        EndingType = word.type;
        _conjType = conjType;
        _conjWord = word.romaji;
        char finalChar = _line[_line.Length-1];

        if (word.isVerb)
        {
            switch (finalChar)
            {
                case 'う':
                case 'る':
                    VerbEndingType = VerbEndingType.Uru;
                    break;
                case 'つ':
                    VerbEndingType = VerbEndingType.Tsu;
                    break;
                case 'す':
                    VerbEndingType = VerbEndingType.Su;
                    break;
                case 'ぬ':
                case 'ぶ':
                case 'む':
                    VerbEndingType = VerbEndingType.Nubumu;
                    break;
                case 'く':
                    VerbEndingType = VerbEndingType.Ku;
                    break;
                case 'ぐ':
                    VerbEndingType = VerbEndingType.Gu;
                    break;
            }
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
        if (EndingType == EndingType.Godan || EndingType == EndingType.IrregularGodan)
        {
            if (special) cutOff = (byte)(VerbEndingType == VerbEndingType.Tsu ? 3 : 2);
            else cutOff = (byte)(VerbEndingType == VerbEndingType.Tsu ? 2 : 1);
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

    private void IConvert()
    {
        if (_iAdj)
        {
            if (_conjWord == "ii")
            {
                _conjWord = "yo";
            }
            else
            {
                _conjWord = _conjWord.Substring(0, _conjWord.Length - 1);
            }
        }
    }

    private void ConjNone()
    {
        string ending = "";
        switch (_combonation)
        {
            case Combonation.Polite:
                if (_isVerb)
                {
                    ending = "masu";
                    if (!_irregular)
                    {
                        ChangeStem("i");
                        _conjWord += ending;
                    }
                    else
                    {
                        switch (_conjWord.ToLower())
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
                }
                else
                {
                    _conjWord += "desu";
                }
                break;
            
            case Combonation.PoliteNegative:
                if (_isVerb)
                {
                    ending = "masen";
                    if (!_irregular)
                    {
                        ChangeStem("i");
                        _conjWord += ending;
                    }
                    else
                    {
                        switch (_conjWord)
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
                }
                else
                {
                    if (_iAdj)
                    {
                        IConvert();
                        _conjWord += "kunaidesu";
                    }
                    else
                    {
                        _conjWord += "janaidesu";
                    }
                }
                
                break;
            
            case Combonation.PolitePast:
                if (_isVerb)
                {
                    ending = "mashita";
                    if (!_irregular)
                    {
                        ChangeStem("i");
                        _conjWord += ending;
                    }
                    else
                    {
                        switch (_conjWord)
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
                }
                else
                {
                    if (_iAdj)
                    {
                        IConvert();
                        _conjWord += "kattadesu";
                    }
                    else
                    {
                        _conjWord += "deshita";
                    }
                }
                break;
            
            case Combonation.PoliteNegativePast:
                if (_isVerb)
                {
                    ending = "masendeshita";
                    if (!_irregular)
                    {
                        ChangeStem("i");
                        _conjWord += ending;
                    }
                    else
                    {
                        switch (_conjWord)
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
                }
                else
                {
                    if (_iAdj)
                    {
                        IConvert();
                        _conjWord += "kunakattadesu";
                    }
                    else
                    {
                        _conjWord += "janakattadesu";
                    }
                }
                break;
            
            case Combonation.Plain:
                if (!_isVerb && !_iAdj)
                {
                    _conjWord += "da";
                }
                else
                {
                    _conjWord = _conjWord;
                }
                break;
            
            case Combonation.PlainNegative:
                if (_isVerb)
                {
                    ending = "nai";
                    if (!_irregular)
                    {
                        ChangeStem("a");
                        _conjWord += ending;
                    }
                    else
                    {
                        switch (_conjWord)
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
                }
                else
                {
                    if (_iAdj)
                    {
                        IConvert();
                        _conjWord += "kunai";
                    }
                    else
                    {
                        _conjWord += "janai";
                    }
                }
                break;
            
            case Combonation.PlainPast:
                if (_isVerb)
                {
                    if (!_irregular)
                    {
                        switch (VerbEndingType)
                        {
                            case VerbEndingType.Tsu:
                            case VerbEndingType.Uru:
                                ending = "tta";
                                break;
                            case VerbEndingType.Su:
                                ending = "shita";
                                break;
                            case VerbEndingType.Ku:
                                ending = "ita";
                                break;
                            case VerbEndingType.Gu:
                                ending = "ida";
                                break;
                            case VerbEndingType.Nubumu:
                                ending = "nda";
                                break;
                        }
                        ChangeStem(ending, true);
                    }
                    else
                    {
                        switch (_conjWord)
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
                }
                else
                {
                    if (_iAdj)
                    {
                        IConvert();
                        _conjWord += "katta";
                    }
                    else
                    {
                        _conjWord += "datta";
                    }
                }
                break;
            
            case Combonation.PlainNegativePast:
                if (_isVerb)
                {
                    ending = "nakatta";
                    if (!_irregular)
                    {
                        ChangeStem("a");
                        _conjWord += ending;
                    }
                    else
                    {
                        switch (_conjWord)
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
                }
                else
                {
                    if (_iAdj)
                    {
                        IConvert();
                        _conjWord += "kunakatta";
                    }
                    else
                    {
                        _conjWord += "janakatta";
                    }
                }
                break;
        }
    }

    private void ConjTe()
    {
        if (positivityType == Positivity.Positive)
        {
            if (_isVerb)
            {
                string ending = "";
                if (!_irregular)
                {
                    switch (VerbEndingType)
                    {
                        case VerbEndingType.Uru:
                        case VerbEndingType.Tsu:
                            ending = "tte";
                            break;
                        case VerbEndingType.Su:
                            ending = "shite";
                            break;
                        case VerbEndingType.Ku:
                            ending = "ite";
                            break;
                        case VerbEndingType.Gu:
                            ending = "ide";
                            break;
                        case VerbEndingType.Nubumu:
                            ending = "nde";
                            break;
                    }
                    ChangeStem(ending, true);
                }
                else
                {
                    switch (_conjWord)
                    {
                        case "kuru":
                            _conjWord = "kite";
                            break;
                        case "suru":
                            _conjWord = "shite";
                            break;
                        case "iku":
                            _conjWord = "itte";
                            break;
                        case "aru":
                            _conjWord = "atte";
                            break;
                    }
                }
            }
            else
            {
                if (_iAdj)
                {
                    IConvert();
                    _conjWord += "kute";
                }
                else
                {
                    _conjWord += "de";
                }
            }
        }
        
        //negative te form
        else
        {
            if (_isVerb)
            {
                string ending = "nakute";
                if (!_irregular)
                {
                    ChangeStem("a");
                    _conjWord += ending;
                }
                else
                {
                    switch (_conjWord)
                    {
                        case "kuru":
                            _conjWord = "konakute";
                            break;
                        case "suru":
                            _conjWord = "shinakute";
                            break;
                        case "iku":
                            _conjWord = "ikanakute";
                            break;
                        case "aru":
                            _conjWord = "nakute";
                            break;
                    }
                }
            }
            else
            {
                if (_iAdj)
                {
                    IConvert();
                    _conjWord += "kunakute";
                }
                else
                {
                    _conjWord += "janakute";
                }
            }
        }
    }

    private void ConjTeIru()
    {
        bool wasNeg = positivityType == Positivity.Negative;
        //make it just teiru before anything
        positivityType = Positivity.Positive;
        ConjTe();
        if (wasNeg)
        {
            positivityType = Positivity.Negative;
        }
        _conjWord += "iru";
        EndingType = EndingType.Ichidan;
        
        //conj just the iru part
        ConjNone();
    }

    public void PrintLine()
    {
        GD.Print($"line is {_line} | conj word is {_conjWord}");
    }

}