using Godot;
using System;
using jp_conj_game.scripts;

public partial class LvlPrev : Node2D
{
	private UButton _backButton;
	private UButton _startButton;

	private RichTextLabel _lvlDesc;
	private RichTextLabel _name;

	private ushort _currVerbLvl;

	public override void _Ready()
	{
		_currVerbLvl = Config.currLvl;
		
		GD.Print($"in lvl prev, the curr lvl is {_currVerbLvl}");
		_backButton = GetNode<UButton>("backB");
		_startButton = GetNode<UButton>("startB");
		_lvlDesc = GetNode<RichTextLabel>("prevDesc");
		_name = GetNode<RichTextLabel>("Name");
		
		SetText(_currVerbLvl, true);

		_backButton.Pressed += OnBackPressed;
		_startButton.Pressed += OnStartPressed;
	}

	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://src/lvl_menu/lvl_menu.tscn");
	}

	private void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://src/lvl/lvl.tscn");
	}

	private void SetText(int lvl, bool isVerb)
	{
		string nameTxt = "temp";
		string descTxt = "temp";
		GD.Print(_name + " 2");
		if (isVerb)
		{
			switch (lvl)
			{
				case 1:
					nameTxt = "Typing practice";
					descTxt = "Simply get used to how to type.";
					break;
				case 2:
					nameTxt = "Polite form";
					descTxt = "Change the stem if need be, and add ~ます to it";
					break;
			}
		}
		else
		{
			switch (lvl)
			{
				
			}
		}

		_name.SetText(nameTxt);
		_lvlDesc.SetText(descTxt);
	}
}
