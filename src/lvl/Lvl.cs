using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using jp_conj_game.scripts;
using Enums;
using Godot.NativeInterop;

public partial class Lvl : Node2D
{
	private List<Word> _WordList;
	private List<conjMaker> _ConjList;
	private List<Sprite2D> _lifeHearts;
	private Sprite2D _tagBox;
	private List<Sprite2D> _tagBoxes;
	
	private Sprite2D _timerBox;
	private Sprite2D _enmArm;
	private Sprite2D _shurikan;
	private Sprite2D _whiteFlash;
	private Sprite2D _blackFlash;
	
	private Label _InputBox;
	private Timer _timer;
	private Timer _flashTimer;

	private AudioStreamPlayer _correctsfx;
	private AudioStreamPlayer _incorrectsfx;
	
	private float _totalTime;
	private float _tempTimerHolder;
	private byte _life;
	private bool _passed;
	private bool _timeStatus;
	private bool _wasRight;
	private byte _wordCount;
	private List<ConjType> _conjType;

	private Politeness _politeness;
	private TenseType _tense;
	private Positivity _positivity;
	
	public override void _Ready()
	{
		_conjType = new List<ConjType>();
		_WordList = new List<Word>();
		ushort currLvl = Config.currLvl;
		_InputBox = GetNode<Label>("inputBox");
		
		_tagBox = GetNode<Sprite2D>("tags/TypeBox");
		_tagBoxes = new List<Sprite2D>();
		
		_timerBox = GetNode<Sprite2D>("timerBox");
		_timer = GetNode<Timer>("Timer");
		_totalTime = Config.GetTimerLen();
		_tempTimerHolder = _totalTime;
		_timer.SetWaitTime(_totalTime);
		_timeStatus = Config.getTimerStatus();
		_wasRight = true;

		_enmArm = GetNode<Sprite2D>("timerSet/EnmArm");
		_shurikan = GetNode<Sprite2D>("timerSet/Shurikan");
		
		_flashTimer = GetNode<Timer>("flashTimer");
		_whiteFlash = GetNode<Sprite2D>("whiteFlash");
		_whiteFlash.Visible = false;
		_blackFlash = GetNode<Sprite2D>("blackFlash");
		_blackFlash.Visible = false;
		
		_correctsfx =  GetNode<AudioStreamPlayer>("correct");
		_incorrectsfx = GetNode<AudioStreamPlayer>("incorrect");
		
		_life = 3;
		_lifeHearts = new List<Sprite2D>();
		_lifeHearts.Add(GetNode<Sprite2D>("Heart"));
		_lifeHearts.Add(GetNode<Sprite2D>("Heart2"));
		_lifeHearts.Add(GetNode<Sprite2D>("Heart3"));
		foreach (var hearts in _lifeHearts)
		{
			hearts.Visible = true;
		}
		
		//bg change
		if (_timeStatus)
		{
			GetNode<Node2D>("timerSet").Visible = true;
			GetNode<Node2D>("noTimerSet").Visible = false;
		}
		else
		{
			_timerBox.Visible = false;
			GetNode<Node2D>("timerSet").Visible = false;
			GetNode<Node2D>("noTimerSet").Visible = true;
			_passed = false;
		}

		_passed = true;
		UpdateIncorrectStuff(false);
		
		_ConjList = new List<conjMaker>();
		
		_politeness = Politeness.Plain;
		_tense = TenseType.Present;
		_positivity = Positivity.Positive;

		_wordCount = 10;
		if (Config.isVerb)
		{
			switch (currLvl)
			{
				//1 is the default
				case 2:
					_politeness = Politeness.Polite;
					break;
				case 3:
					_politeness = Politeness.Polite;
					_positivity = Positivity.Negative;
					break;
				case 4:
					_positivity = Positivity.Negative;
					break;
				case 5:
					_politeness = Politeness.Both;
					_positivity =  Positivity.Both;
					_wordCount = 15;
					break;
				case 6:
					_conjType.Add(ConjType.Te);
					break;
				case 7:
					_conjType.Add(ConjType.Te);
					_positivity = Positivity.Negative;
					break;
				case 8:
					_tense = TenseType.Past;
					break;
				case 9:
					_tense = TenseType.Past;
					_positivity = Positivity.Negative;
					break;
				case 10:
					_tense = TenseType.Past;
					_politeness = Politeness.Plain;
					break;
				case 11:
					_tense = TenseType.Past;
					_positivity = Positivity.Negative;
					break;
				case 12:
					_tense = TenseType.Past;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Te);
					_conjType.Add(ConjType.None);
					_wordCount = 10;
					_totalTime += 5;
					break;
				case 13:
					_tense = TenseType.Both;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.TeIru);
					_wordCount = 15;
					_totalTime += 5;
					break;
				case 14:
					_politeness =  Politeness.Both;
					_conjType.Add(ConjType.Presump);
					_wordCount = 15;
					break;
				case 15:
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Tai);
					_wordCount = 15;
					break;
				case 16:
					_tense = TenseType.Past;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Tai);
					break;
				case 17:
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Passive);
					_wordCount = 15;
					break;
				case 18:
					_tense = TenseType.Past;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Passive);
					_wordCount = 15;
					break;
				case 19:
					_tense = TenseType.Both;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Passive);
					_conjType.Add(ConjType.TeIru);
					_conjType.Add(ConjType.Presump);
					_conjType.Add(ConjType.Tai);
					_totalTime += 10;
					break;
				case 20:
					_tense = TenseType.Both;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Passive);
					_wordCount = 15;
					break;
				case 21:
					_tense = TenseType.Both;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Causative);
					_wordCount = 15;
					break;
				case 22:
					_tense = TenseType.Both;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.CausativePas);
					_wordCount = 15;
					break;
				case 23:
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Conditional);
					break;
				case 24:
					_positivity = Positivity.Both;
					_politeness = Politeness.Both;
					_conjType.Add(ConjType.Provisional);
					_wordCount = 15;
					break;
				case 25:
					_tense = TenseType.Both;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Provisional);
					_conjType.Add(ConjType.CausativePas);
					_conjType.Add(ConjType.Conditional);
					_conjType.Add(ConjType.Causative);
					_conjType.Add(ConjType.Passive);
					break;
			}
		}
		else
		{
			switch (currLvl)
			{
				default:
					break;
				case 2:
					_politeness = Politeness.Polite;
					break;
				case 3:
					_politeness = Politeness.Polite;
					_positivity = Positivity.Negative;
					break;
				case 4:
					_positivity = Positivity.Negative;
					break;
				case 5:
					_positivity = Positivity.Both;
					_politeness = Politeness.Both;
					_wordCount = 15;
					break;
				case 6:
					_conjType.Add(ConjType.Te);
					break;
				case 7:
					_conjType.Add(ConjType.Te);
					_positivity = Positivity.Negative;
					break;
				case 8:
					_tense = TenseType.Past;
					break;
				case 9:
					_tense = TenseType.Past;
					_positivity = Positivity.Negative;
					break;
				case 10:
					_tense = TenseType.Past;
					_politeness = Politeness.Plain;
					break;
				case 11:
					_tense = TenseType.Past;
					_positivity = Positivity.Negative;
					break;
				case 12:
					_tense = TenseType.Past;
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Te);
					_conjType.Add(ConjType.None);
					_wordCount = 10;
					_totalTime += 5;
					break;
				case 13:
					_conjType.Add(ConjType.Adverbal);
					break;
				case 14:
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Conditional);
					_wordCount = 15;
					break;
				case 15:
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Provisional);
					_wordCount = 15;
					break;
				case 16:
					_politeness = Politeness.Both;
					_positivity = Positivity.Both;
					_conjType.Add(ConjType.Provisional);
					_conjType.Add(ConjType.Conditional);
					_conjType.Add(ConjType.Adverbal);
					_wordCount = 10;
					break;
				case 17:
					_conjType.Add(ConjType.Naru);
					break;
				case 18:
					_conjType.Add(ConjType.Suru);
					break;
				case 19:
					_conjType.Add(ConjType.Sugiru);
					break;
				case 20:
					_conjType.Add(ConjType.Sugiru);
					_conjType.Add(ConjType.Suru);
					_conjType.Add(ConjType.Naru);
					break;
			}
		}

		if (!_conjType.Any())
		{
			_conjType.Add(ConjType.None);
		}

		foreach (ConjType conjType in _conjType)
		{
			
			_WordList = WordReader.GetWords(_wordCount);
			
			foreach (Word word in _WordList)
			{
				_ConjList.Add(new conjMaker(word,
					politeness: _politeness,
					tense: _tense,
					positivity: _positivity,
					conjType: conjType));
			}
			_WordList.Clear();
		}
		
		UpdateWord();
		UpdateTags();
		if (_timeStatus)
		{
			_tempTimerHolder = _totalTime;
			_timer.Start();
		}
	}
	
	public override void _Process(double delta)
	{
		if (_timeStatus)
		{
			UpdateTimerBox();
			UpdateEnemy();
		}
	}
	
	private void UpdateTimerBox()
	{
		_timerBox.Scale = new Vector2( (float)(1.174*(_timer.TimeLeft/_totalTime)), 0.028f);
		
		if (_wasRight)
		{
			Color c = _whiteFlash.Modulate;
			c.A = (float)_flashTimer.TimeLeft;
			_whiteFlash.Modulate = c;
		}
		else
		{
			Color c = _blackFlash.Modulate;
			c.A = (float)_flashTimer.TimeLeft;
			_blackFlash.Modulate = c;
		}

		if (_flashTimer.TimeLeft <= 0)
		{
			_whiteFlash.Visible = false;
			_blackFlash.Visible = false;
		}
		
		if (_timer.TimeLeft <= 0)
		{
			WrongWord();
		}
	}

	private void UpdateTags()
	{
		foreach (Sprite2D node in GetNode<Node2D>("tags").GetChildren())
		{
			node.Visible = false;
		}
		_tagBoxes.Clear();

		//politeness
		if (_ConjList[0].activePoliteness)
		{
			Sprite2D politenessBox = (Sprite2D)_tagBox.Duplicate((int)(Node.DuplicateFlags.UseInstantiation));
			_tagBox.GetParent().AddChild(politenessBox);
			_tagBoxes.Add(politenessBox);
			politenessBox.GetNode<RichTextLabel>("type").Text =
				_ConjList[0].politenessType == Politeness.Polite ? "Polite" : "Plain";
			politenessBox.Visible = true;
		}
	
		//do not show if it's present
		if (_ConjList[0].activeTense && _ConjList[0].tenseType == TenseType.Past)
		{
			Sprite2D tenseBox = (Sprite2D)_tagBox.Duplicate((int)(Node.DuplicateFlags.UseInstantiation));
			_tagBox.GetParent().AddChild(tenseBox);
			_tagBoxes.Add(tenseBox);
			tenseBox.GetNode<RichTextLabel>("type").Text = "Past";
		}
	
		//do not show if it's positive
		if (_ConjList[0].activePositivity && _ConjList[0].positivityType == Positivity.Negative)
		{
			Sprite2D positiveBox = (Sprite2D)_tagBox.Duplicate((int)(Node.DuplicateFlags.UseInstantiation));
			_tagBox.GetParent().AddChild(positiveBox);
			_tagBoxes.Add(positiveBox);
			positiveBox.GetNode<RichTextLabel>("type").Text = "Negative";
		}
		
		

		//only show if the type isn't none
		if (_ConjList[0]._conjType != ConjType.None)
		{
			Sprite2D conjType = (Sprite2D)_tagBox.Duplicate((int)(Node.DuplicateFlags.UseInstantiation));
			_tagBox.GetParent().AddChild(conjType);
			_tagBoxes.Add(conjType);
			string text = "";
			
			//form name
			switch (_ConjList[0]._conjType)
			{
				case ConjType.Te:
					text = "Te Form";
					break;
			}
			
			conjType.GetNode<RichTextLabel>("type").Text = text;
		}

		switch (_tagBoxes.Count)
		{
			case 1:
				_tagBoxes[0].Position = new Vector2(1280, 387);
				break;
			case 2: 
				_tagBoxes[0].Position = new Vector2(1280, 355);
				_tagBoxes[1].Position = new Vector2(1280, 420);
				break;
			case 3:
				_tagBoxes[0].Position = new Vector2(1429, 355);
				_tagBoxes[1].Position = new Vector2(1280, 420);
				_tagBoxes[2].Position = new Vector2(1131, 355);
				break;
			case 4:
				_tagBoxes[0].Position = new Vector2(1429, 355);
				_tagBoxes[1].Position = new Vector2(1429, 420);
				_tagBoxes[2].Position = new Vector2(1131, 355);
				_tagBoxes[3].Position = new Vector2(1131, 420);
				break;
		}

		foreach (var temp in _tagBoxes)
		{
			temp.Visible = true;
		}
		
	}

	private void UpdateEnemy()
	{
		var ratio = _timer.GetTimeLeft() / _totalTime;
		//80 degree movement, 50 offset -> starts at 30 degrees and ends at -50 degrees
		_enmArm.RotationDegrees = (float) (80 * _timer.GetTimeLeft()/_totalTime) - 50;
		_shurikan.Position = new Vector2((float)(1147 * ratio + 735) ,897);
		_shurikan.RotationDegrees -= 2;
	}

	private void WrongWord()
	{
		_wasRight = false;
		_blackFlash.Visible = true;
		_flashTimer.Start();
		
		GD.Print("stupid");
		UpdateIncorrectStuff(true);
		RemoveWordOnScreen(false);
		_life--;
		switch (_life)
		{
			case 0:
				_passed = false;
				_lifeHearts[2].Visible = false;
				break;
			case 1:
				_lifeHearts[1].Visible = false;
				break;
			case 2:
				_lifeHearts[0].Visible = false;
				break;
		}
		_ConjList.Add(_ConjList[0]);
		_ConjList.RemoveAt(0);
		if (_timeStatus)
		{
			_totalTime = Config.GetTimerLen() + 5;
			_timer.Start(_totalTime);
		}
		UpdateWord();
	}
	
	private void UpdateWord()
	{
		Label _defLabel = GetNode<Label>("defBox");
		foreach (Label label in _ConjList[0].GetLabels())
		{
			GD.Print($"adding label {label.Text}");
			if (!HasNode(label.GetPath()))
			{
				AddChild(label);
			}
			else
			{
				label.Visible = true;
			}
		}

		_defLabel.Text = "";
		foreach (string def in _ConjList[0].Defs)
		{
			_defLabel.Text += def + "\n";
		}

		UpdateTags();
	}

	private void RemoveWordOnScreen(bool passed)
	{
		if (passed)
		{
			foreach (Label label in _ConjList[0].GetLabels())
			{
				GetParent().RemoveChild(label);
			}
		}
		foreach (Label label in _ConjList[0].GetLabels())
		{
			label.Visible = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey && eventKey.Pressed && !eventKey.Echo)
		{
			switch (eventKey.Keycode)
			{
				case Key.Backspace:
					if (_InputBox.Text != "")
					{
						_InputBox.Text  = _InputBox.Text.Substring(0, _InputBox.Text.Length - 1);
					}
					break;
				case Key.Space:
				case Key.Enter:
					SubmitAnswer();
					break;
				default:
					char temp = (char)eventKey.Unicode;
					if (Char.IsLetter(temp))
					{
						_InputBox.Text += temp;
					}
					break;
			}
		}
	}

	private void CheckWin()
	{
		if (_passed)
		{
			Config.SetMaxVerbLvl((ushort)(Config.GetMaxVerbLvl() + 1));
		}
	}
	private void SubmitAnswer()
	{
		string submited =  _InputBox.Text.ToLower();
		if (submited == _ConjList[0]._conjWord)
		{
			GD.Print("correct!");
			_wasRight = true;
			_whiteFlash.Visible = true;
			_flashTimer.Start();
			_correctsfx.Play();
			
			UpdateIncorrectStuff(false);
			RemoveWordOnScreen(true);
			_ConjList.RemoveAt(0);
			GD.Print(_ConjList.Count);
			//keep playing da game
			if (_ConjList.Count > 0)
			{
				UpdateWord();
				if (_timeStatus)
				{
					//in case it was inc by prev answer being wrong
					_totalTime = _tempTimerHolder;
					_timer.Start(_totalTime);
				}
			}
			else
			{
				_timer.Stop();
				GD.Print("You Won!");
				CheckWin();
				GetTree().ChangeSceneToFile("res://src/lvl_menu/lvl_menu.tscn");
			}
		}
		else
		{
			WrongWord();
		}
		_InputBox.Text = "";
	}

	private void UpdateIncorrectStuff(bool visable)
	{
		GetNode<Control>("IncorrectStuff").Visible = visable;
		if (visable)
		{
			_incorrectsfx.Play();
			GetNode<Label>("IncorrectStuff/PrevAnswer").Text = _ConjList[0]._conjWord;
			GetNode<Label>("IncorrectStuff/UserPrevAnswer").Text = _InputBox.Text.ToLower();
		}
	}
	
}
