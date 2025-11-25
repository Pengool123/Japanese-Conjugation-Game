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
    public bool isVerb {get; private set;}
    private bool _irregular {get; set;}
    private bool _iAdj { get; set; }
    public Combonation _combonation { get; private set; }
    public TenseType tenseType { get; private set; }
    public bool activeTense { get; private set; }
    public Positivity positivityType { get; private set; }
    public bool activePositivity { get; private set; }
    public Politeness politenessType { get; private set; }
    public bool  activePoliteness { get; private set; }
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
        MakeWord(word,
            tense,
            positivity,
            politeness,
            conjType,
            x,
            y,
            size);
        
    }

    private void MakeWord(Word word = null,
        TenseType tense= TenseType.Present,
        Positivity positivity= Positivity.Positive,
        Politeness politeness= Politeness.Plain,
        ConjType conjType= ConjType.None,
        int x=1280,
        int y=150,
        int size = 100)
    {
        activePoliteness = true;
        activeTense = true;
        activePositivity = true;
        
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
        
        isVerb = word.isVerb;
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
                activePoliteness = false;
                activeTense = false;
                ConjTe();
                break;
            case ConjType.TeIru:
                ConjTeIru();
                break;
            case ConjType.Presump:
                activePositivity = false;
                activeTense = false;
                ConjPresump();
                break;
            case ConjType.Tai:
                ConjTai();
                break;
            case ConjType.Potential:
                ConjPotential();
                break;
            case ConjType.Passive:
                ConjPassive();
                break;
            case ConjType.Causative:
                ConjCausative();
                break;
            case ConjType.CausativePas:
                ConjCausativePas();
                break;
            case ConjType.Conditional:
                activePoliteness = false;
                activeTense = false;
                ConjConditional();
                break;
            case ConjType.Provisional:
                activeTense = false;
                ConjProvisional();
                break;
            case ConjType.Imperative:
                activePositivity = false;
                activeTense = false;
                ConjImperative();
                break;
            case ConjType.Adverbal:
                activeTense = false;
                activePositivity = false;
                politenessType = Politeness.Plain;
                ConjAdverbal();
                break;
            case ConjType.Naru:
                activeTense = false;
                activePositivity = false;
                politenessType = Politeness.Plain;
                ConjNaru();
                break;
            case ConjType.Suru:
                activeTense = false;
                activePositivity = false;
                politenessType = Politeness.Plain;
                ConjSuru();
                break;
            case ConjType.Sugiru:
                activeTense = false;
                activePositivity = false;
                politenessType = Politeness.Plain;
                ConjSugiru();
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
        if (stem == "a" && VerbEndingType == VerbEndingType.Uru && _conjWord[_conjWord.Length - 2] != 'r')
        {
            _conjWord = _conjWord.Substring(0, _conjWord.Length - 1) + "wa";
            return;
        }
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
                if (isVerb)
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
                if (isVerb)
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
                if (isVerb)
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
                if (isVerb)
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
                if (!isVerb && !_iAdj)
                {
                    _conjWord += "da";
                }
                else
                {
                    _conjWord = _conjWord;
                }
                break;
            
            case Combonation.PlainNegative:
                if (isVerb)
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
                if (isVerb)
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
                if (isVerb)
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
            if (isVerb)
            {
                string ending = "";
                if (!_irregular)
                {
                    if (EndingType == EndingType.Ichidan)
                    {
                        ending = "te";
                    }
                    else
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
            if (isVerb)
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

    private void ConjPresump()
    {
        if (politenessType == Politeness.Polite)
        {
            string ending = "mashou";
            if (!_irregular)
            {
                ChangeStem("i");
            }
            else
            {
                switch (_conjWord)
                {
                    case "kuru":
                        _conjWord = "ki";
                        break;
                    case "suru":
                        _conjWord = "shi";
                        break;
                    case "iku":
                        _conjWord = "iki";
                        break;
                    case "aru":
                        _conjWord = "ari";
                        break;
                }
            }
            _conjWord += ending;
        }
        else
        {
            string ending = "mou";
            if (!_irregular)
            {
                if (EndingType == EndingType.Ichidan)
                {
                    ending = "you";
                    ChangeStem(ending, true);
                }
                else
                {
                    ChangeStem("o");
                    _conjWord += ending;
                }
            }
            else
            {
                switch (_conjWord)
                {
                    case "kuru":
                        _conjWord = "koyou";
                        break;
                    case "suru":
                        _conjWord = "shiyou";
                        break;
                    case "iku":
                        _conjWord = "ikou";
                        break;
                    case "aru":
                        _conjWord = "arou";
                        break;
                }
            }
        }
    }

    private void ConjTai()
    {
        if (_conjWord == "aru" && _irregular)
        {
            MakeWord(word:WordReader.GetNormWord());
            return;
        }
        if (!_irregular)
        {
            ChangeStem("i");
        }
        else
        {
            switch (_conjWord)
            {
                case "kuru":
                    _conjWord = "ki";
                    break;
                case "suru":
                    _conjWord = "shi";
                    break;
                case "iku":
                    _conjWord = "iki";
                    break;
            }
        }
        
        string ending = "tai";

        if (tenseType == TenseType.Present && positivityType == Positivity.Negative)
        {
            ending = "takunai";
        }

        if (tenseType == TenseType.Past && positivityType == Positivity.Positive)
        {
            ending = "takatta";
        }

        if (tenseType == TenseType.Past && positivityType == Positivity.Negative)
        {
            ending = "takunaktta";
        }
        
        _conjWord += ending;
    }

    private void ConjPotential(bool remixed = false)
    {
        string ending = "masu";
        if (!remixed)
        {
            if (!_irregular)
            {
                ChangeStem("e");
                if (EndingType == EndingType.Ichidan)
                {
                    _conjWord += "rare";
                }
            }
            else
            {
                if (_conjWord == "aru")
                {
                    MakeWord(word:WordReader.GetNormWord());
                    return;
                }
                switch (_conjWord)
                {
                    case "kuru":
                        _conjWord = "korare";
                        break;
                    case "suru":
                        _conjWord = "deki";
                        break;
                    case "iku":
                        _conjWord = "ike";
                        break;
                }
            }
        }

        switch (_combonation)
        {
            case Combonation.Polite:
                ending = "masu";
                break;
            case Combonation.PoliteNegative:
                ending = "masen";
                break;
            case Combonation.PolitePast:
                ending = "mashita";
                break;
            case Combonation.PoliteNegativePast:
                ending = "masendeshita";
                break;
            case Combonation.Plain:
                ending = "ru";
                break;
            case Combonation.PlainNegative:
                ending = "nai";
                break;
            case Combonation.PlainPast:
                ending = "ta";
                break;
            case Combonation.PlainNegativePast:
                ending = "nakatta";
                break;
        }
        _conjWord += ending;
    }

    private void ConjPassive()
    {
        if (!_irregular)
        {
            ChangeStem("a");
            if (EndingType == EndingType.Ichidan)
            {
                _conjWord += "rare";
            }
            else
            {
                _conjWord += "re";
            }
        }
        else
        {
            if (_conjWord == "aru")
            {
                MakeWord(word:WordReader.GetNormWord());
                return;
            }
            switch (_conjWord)
            {
                case "kuru":
                    _conjWord = "korare";
                    break;
                case "suru":
                    _conjWord = "sare";
                    break;
                case "iku":
                    _conjWord = "ikare";
                    break;
            }
        }
        ConjPotential(true);
    }

    private void ConjCausativePas()
    {
        if (!_irregular)
        {
            ChangeStem("a");
            if (EndingType == EndingType.Ichidan)
            {
                _conjWord += "saserare";
            }
            else
            {
                _conjWord += "sare";
            }
        }
        else
        {
            if (_conjWord == "aru")
            {
                MakeWord(word:WordReader.GetNormWord());
                return;
            }
            switch (_conjWord)
            {
                case "kuru":
                    _conjWord = "kosaserare";
                    break;
                case "suru":
                    _conjWord = "saserare";
                    break;
                case "iku":
                    _conjWord = "ikasare";
                    break;
            }
        }
        ConjPotential(true);
    }

    private void ConjCausative()
    {
        if (!_irregular)
        {
            ChangeStem("a");
            if (EndingType == EndingType.Ichidan)
            {
                _conjWord += "sase";
            }
            else
            {
                _conjWord += "se";
            }
        }
        else
        {
            if (_conjWord == "aru")
            {
                MakeWord(word:WordReader.GetNormWord());
                return;
            }
            switch (_conjWord)
            {
                case "kuru":
                    _conjWord = "kosase";
                    break;
                case "suru":
                    _conjWord = "sase";
                    break;
                case "iku":
                    _conjWord = "ikase";
                    break;
            }
        }
        ConjPotential(true);
    }

    private void ConjConditional()
    {
        string ending = "";
        if (isVerb)
        {
            if (!_irregular)
            {
                if (EndingType == EndingType.Ichidan)
                {
                    if (positivityType == Positivity.Positive)
                    {
                        ending = "tara";
                    }
                    else
                    {
                        ending = "nakattara";
                    }

                    ChangeStem();
                    _conjWord += ending;
                }
                else
                {
                    if (positivityType == Positivity.Positive)
                    {
                        switch (VerbEndingType)
                        {
                            case VerbEndingType.Uru:
                            case VerbEndingType.Tsu:
                                ending = "ttara";
                                break;
                            case VerbEndingType.Su:
                                ending = "shitara";
                                break;
                            case VerbEndingType.Ku:
                                ending = "itara";
                                break;
                            case VerbEndingType.Gu:
                                ending = "idara";
                                break;
                            case VerbEndingType.Nubumu:
                                ending = "ndara";
                                break;
                        }
                        ChangeStem(ending, true);
                    }
                    else
                    {
                        ChangeStem("a");
                        _conjWord += "nakattara";
                    }
                }
            }
            else
            {
                if (positivityType == Positivity.Positive)
                {
                    switch (_conjWord)
                    {
                        case "kuru":
                            _conjWord = "kitara";
                            break;
                        case "suru":
                            _conjWord = "shitara";
                            break;
                        case "iku":
                            _conjWord = "ittara";
                            break;
                        case "aru":
                            _conjWord = "attara";
                            break;
                    }
                }
                else
                {
                    switch (_conjWord)
                    {
                        case "kuru":
                            _conjWord = "konakattara";
                            break;
                        case "suru":
                            _conjWord = "shinakattara";
                            break;
                        case "iku":
                            _conjWord = "inakattara";
                            break;
                        case "aru":
                            _conjWord = "nakattara";
                            break;
                    }
                }
            }
        }
        else
        {
            if (positivityType == Positivity.Positive)
            {
                if (_iAdj)
                {
                    IConvert();
                    _conjWord += "kattara";
                }
                else
                {
                    _conjWord += "dattara";
                }
            }
            else
            {
                if (_iAdj)
                {
                    IConvert();
                    _conjWord += "kunakattara";
                }
                else
                {
                    _conjWord += "janakattara";
                }
            }
        }
        
    }

    private void ConjProvisional()
    {
        string ending = "";

        if (isVerb)
        {
            if (positivityType == Positivity.Positive)
            {
                activePoliteness = false;
                ending = "reba";
            }
            else
            {
                if (politenessType == Politeness.Polite)
                {
                    ending = "nakereba";
                }
                else
                {
                    ending = "nakya";
                }
            }
        
            if (!_irregular)
            {
                if (positivityType == Positivity.Positive)
                {
                    ChangeStem("e");
                }
                else
                {
                    ChangeStem("a");
                }
            }
            else
            {
                if (positivityType == Positivity.Positive)
                {
                    switch (_conjWord)
                    {
                        case "kuru":
                            _conjWord = "ku";
                            break;
                        case "suru":
                            _conjWord = "su";
                            break;
                        case "iku":
                            ending = "keba";
                            _conjWord = "i";
                            break;
                        case "aru":
                            _conjWord = "a";
                            break;
                    }
                }
                else
                {
                    switch (_conjWord)
                    {
                        case "kuru":
                            _conjWord = "ko";
                            break;
                        case "suru":
                            _conjWord = "shi";
                            break;
                        case "iku":
                            _conjWord = "ika";
                            break;
                        case "aru":
                            _conjWord = "";
                            break;
                    }
                }
            }
            _conjWord = ending;
        }
        else
        {
            activePoliteness = false;
            if (positivityType == Positivity.Positive)
            {
                if (_iAdj)
                {
                    IConvert();
                    _conjWord += "kereba";
                }
                else
                {
                    _conjWord += "narabe";
                }
            }
            else
            {
                if (_iAdj)
                {
                    IConvert();
                    _conjWord += "nakereba";
                }
                else
                {
                    if (politenessType == Politeness.Plain)
                    {
                        activePoliteness = true;
                        _conjWord += "janakereba";
                    }
                    else
                    {
                        _conjWord += "denakereba";
                    }
                }
            }
        }
    }

    private void ConjImperative()
    {
        if (politenessType == Politeness.Polite)
        {
            if (!_irregular)
            {
                string ending = "nasai";
                ChangeStem("i");
                _conjWord += ending;
            }
            else
            {
                switch (_conjWord)
                {
                    case "kuru":
                        _conjWord = "kinasai";
                        break;
                    case "suru":
                        _conjWord = "shinasai";
                        break;
                    case "iku":
                        _conjWord = "ikinasai";
                        break;
                    case "aru":
                        MakeWord(WordReader.GetNormWord());
                        return;
                        break;
                }
            }
        }
        else
        {
            if (!_irregular)
            {
                if (EndingType == EndingType.Ichidan)
                {
                    ChangeStem();
                    _conjWord += "ro";
                }
                else
                {
                    ChangeStem("e");
                }
            }
            else
            {
                
                switch (_conjWord)
                {
                    case "kuru":
                        _conjWord = "koi";
                        break;
                    case "suru":
                        _conjWord = "shiro";
                        break;
                    case "iku":
                        _conjWord = "ike";
                        break;
                    case "aru":
                        MakeWord(WordReader.GetNormWord());
                        return;
                        break;
                }
            }
        }
    }

    private void ConjAdverbal()
    {
        if (_iAdj)
        {
            IConvert();
            _conjWord += "ku";
        }
        else
        {
            _conjWord += "ni";
        }
    }

    private void ConjNaru()
    {
        if (_iAdj)
        {
            IConvert();
            _conjWord += "kunaru";
        }
        else
        {
            _conjWord += "ninaru";
        }
    }

    private void ConjSuru()
    {
        if (_iAdj)
        {
            IConvert();
            _conjWord += "kusuru";
        }
        else
        {
            _conjWord += "nisuru";
        }
    }

    private void ConjSugiru()
    {
        if (_iAdj)
        {
            IConvert();
            _conjWord += "sugiru";
        }
        else
        {
            _conjWord += "sugiru";
        }
    }
    
    public void PrintLine()
    {
        GD.Print($"line is {_line} | conj word is {_conjWord}");
    }

}