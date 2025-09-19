using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using jp_conj_game.scripts;

public partial class LvlMenu : Node2D
{
	private UButton _backButton;
	private UButton _optionsButton;
	private UButton _leftButton;
	private UButton _rightButton;

	private List<Control> _pages = new List<Control>();
	private RichTextLabel _pageLabel;
	
	private short _stageNum = 0;
	private byte _maxStageNum = 3;

	private ushort _maxVerbLvl;
	private ushort _maxAdjLvl;

	//lvl buttons
	private List<UButton> _lvlButtonsVerbs;
	private List<UButton> _lvlButtonsAdj;
	
	public override void _Ready()
	{
		_backButton = GetNode<UButton>("backB");
		_optionsButton = GetNode<UButton>("optionB");
		_leftButton = GetNode<UButton>("leftB");
		_rightButton = GetNode<UButton>("rightB");
		
		_pages.Add(GetNode<Control>("verbLvls/page1"));
		_pages.Add(GetNode<Control>("verbLvls/page2"));
		_pages.Add(GetNode<Control>("adjLvls/page1"));
		_pages.Add(GetNode<Control>("adjLvls/page2"));

		_backButton.Pressed += OnBackPressed;
		_optionsButton.Pressed += OnOptionsPressed;
		_leftButton.Pressed += OnLeftPressed;
		_rightButton.Pressed += OnRightPressed;

		_maxVerbLvl = Config.GetMaxVerbLvl();
		_maxAdjLvl = Config.GetMaxAdjLvl();
		Config.wasInLvlMenu = true;
		
		_pageLabel = GetNode<RichTextLabel>("pageLabel");
		
		//lvl buttons
		_lvlButtonsVerbs = new List<UButton>();
		_lvlButtonsAdj = new List<UButton>();
		
		bool temp = true;
		byte counter = 1;
		string currLvlString = "";
		while (temp)
		{
			if (counter <= 20)
			{
				currLvlString = "verbLvls/page1/lvlB" + counter;
			}
			else
			{
				currLvlString = "verbLvls/page2/lvlB" + counter;
			}
			
			UButton lvlButton = GetNodeOrNull<UButton>(currLvlString);
			GD.Print(lvlButton);
			if (lvlButton != null)
			{
				_lvlButtonsVerbs.Add(lvlButton);
				counter++;
			}
			else
			{
				temp = false;
			}
		}
		
		temp = true;
		counter = 1;
		while (temp)
		{
			currLvlString = "adjLvls/page1/" + counter;
			
			UButton lvlButton = GetNodeOrNull<UButton>(currLvlString);
			GD.Print(lvlButton);
			if (lvlButton != null)
			{
				_lvlButtonsAdj.Add(lvlButton);
				counter++;
			}
			else
			{
				temp = false;
			}
		}

		counter = 1;
		foreach (UButton lvlButton in _lvlButtonsVerbs)
		{
			byte counter2 = counter;
			lvlButton.Pressed += () => OnLvlButtonPressed(counter2, true);
			GD.Print(_maxVerbLvl);
			if (_maxVerbLvl < counter2)
			{
				GD.Print("cannot activate verb" + counter2);
				lvlButton.Active = false;
			}
			counter++;
		}
		
		counter = 1;
		foreach (UButton lvlButton in _lvlButtonsAdj)
		{
			byte counter2 = counter;
			lvlButton.Pressed += () => OnLvlButtonPressed(counter2, false);
			GD.Print(_maxAdjLvl);
			if (_maxAdjLvl < counter2)
			{
				GD.Print("cannot activate adj" + counter2);
				lvlButton.Active = false;
			}
			counter++;
		}

		UpdateVis();
	}

	private void UpdateVis()
	{
		switch (_stageNum)
		{
			case 0:
				_pageLabel.Text = "Verbs 1";
				break;
			case 1:
				_pageLabel.Text = "Verbs 2";
				break;
			case 2:
				_pageLabel.Text = "Adjectives 1";
				break;
			case 3:
				_pageLabel.Text = "Adjectives 2";
				break;
		}
		for (int i = 0; i < _pages.Count; i++)
		{
			//current page
			if (_stageNum == i)
			{
				_pages[i].Visible = true;
			}
			else
			{
				_pages[i].Visible = false;
			}
		}
	}

	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://src/main_menu/main_menu.tscn");
	}

	private void OnOptionsPressed()
	{
		GetTree().ChangeSceneToFile("res://src/options/options.tscn");
	}

	private void OnLeftPressed()
	{
		_stageNum--;
		if (_stageNum < 0) _stageNum = _maxStageNum;
		UpdateVis();
	}

	private void OnRightPressed()
	{
		_stageNum++;
		if (_stageNum > _maxStageNum) _stageNum = 0;
		UpdateVis();
	}

	private void OnLvlButtonPressed(ushort currLvl, bool isVerb)
	{
		//valid button pressed
		if (currLvl <= _maxVerbLvl)
		{
			Config.currLvl = currLvl;
			Config.isVerb = isVerb;
			GD.Print(currLvl + " " + isVerb);
			GetTree().ChangeSceneToFile("res://src/lvl_prev/lvl_prev.tscn");
		}
	}

}
