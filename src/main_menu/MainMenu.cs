using Godot;
using System;
using System.Text.Json;
using jp_conj_game.scripts;

public partial class MainMenu : Node2D
{
	private UButton _playButton;
	private UButton _optionsButton;
	private UButton _quitButton;
	
	
	public override void _Ready()
	{
		//set up all global stuff
		WordReader.Load();
		Config.Load();
		Config.wasInLvlMenu = false;

		Word lemp =  new Word("うたわれるもの二[ふた]人[り]の白[はく]皇[おろ]時計[とけい]料[りょう]{to be goated/the best}");
		lemp.printLine();
		
		
		_playButton = GetNode<UButton>("playB");
		_optionsButton = GetNode<UButton>("optionB");
		_quitButton = GetNode<UButton>("quitB");

		_playButton.Pressed += OnPlayPressed;
		_optionsButton.Pressed += OnOptionsPressed;
		_quitButton.Pressed += OnQuitPressed;
		
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://src/lvl_menu/lvl_menu.tscn");
	}

	private void OnOptionsPressed()
	{
		GetTree().ChangeSceneToFile("res://src/options/options.tscn");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	public override void _Process(double delta)
	{
		
	}
}
