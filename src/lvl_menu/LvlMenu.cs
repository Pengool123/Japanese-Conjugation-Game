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

	private ushort _maxLvl;

	//lvl buttons
	private List<UButton> _lvlButtonsVerbs;
	private List<UButton> _lvlButtonsAdj;
	
	public override void _Ready()
	{
		_backButton = GetNode<UButton>("backB");
		_optionsButton = GetNode<UButton>("optionB");
		_leftButton = GetNode<UButton>("leftB");
		_rightButton = GetNode<UButton>("rightB");

		_backButton.Pressed += OnBackPressed;
		_optionsButton.Pressed += OnOptionsPressed;
		_leftButton.Pressed += OnLeftPressed;
		_rightButton.Pressed += OnRightPressed;

		_maxLvl = Config.GetMaxVerbLvl();
		Config.wasInLvlMenu = true;
		
		//lvl buttons
		_lvlButtonsVerbs = new List<UButton>();
		_lvlButtonsAdj = new List<UButton>();
		
		bool temp = true;
		byte counter = 1;
		while (temp)
		{
			string currLvlString = "verbLvls/lvlB" + counter;
			UButton lvlButton = GetNodeOrNull<UButton>(currLvlString);
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
			string currLvlString = "adjLvls/lvlB" + counter;
			UButton lvlButton = GetNodeOrNull<UButton>(currLvlString);
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
			GD.Print(_maxLvl);
			if (_maxLvl < counter2)
			{
				lvlButton.Active = false;
			}
			counter++;
		}
		
		counter = 1;
		foreach (UButton lvlButton in _lvlButtonsAdj)
		{
			byte counter2 = counter;
			lvlButton.Pressed += () => OnLvlButtonPressed(counter2, false);
			GD.Print(_maxLvl);
			if (_maxLvl < counter2)
			{
				lvlButton.Active = false;
			}
			counter++;
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

	private void OnLeftPressed(){}

	private void OnRightPressed(){}

	private void OnLvlButtonPressed(ushort currLvl, bool isVerb)
	{
		//valid button pressed
		if (currLvl <= _maxLvl)
		{
			Config.currLvl = currLvl;
			Config.isVerb = isVerb;
			GD.Print(currLvl + " " + isVerb);
			GetTree().ChangeSceneToFile("res://src/lvl_prev/lvl_prev.tscn");
		}
	}

}
