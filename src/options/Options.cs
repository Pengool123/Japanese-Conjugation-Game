using Godot;
using System;
using jp_conj_game.scripts;

public partial class Options : Node2D
{
	private UButton _backButton;
	private UButton _applyButton;
	private UButton _commIG;
	private UButton _activeTimer;
	private UButton _incTimer;
	private UButton _decTimer;
	
	private RichTextLabel _timerStartInput;

	private bool _onlyCommIrr;
	private bool _activeT;
	private ushort _timerStartTime;
	
	public override void _Ready()
	{
		_backButton = GetNode<UButton>("backB");
		_applyButton = GetNode<UButton>("applyB");
		_commIG = GetNode<UButton>("commonIrrGodan");
		_activeTimer = GetNode<UButton>("timerStatus");
		_incTimer = GetNode<UButton>("timerInput/timeInc");
		_decTimer = GetNode<UButton>("timerInput/timeDec");
		_timerStartInput = GetNode<RichTextLabel>("timerInput/RichTextLabel");

		_timerStartTime = Config.GetTimerLen();
		_timerStartInput.Text = Config.GetTimerLen().ToString();

		_onlyCommIrr = Config.getIrrGStatus();
		_commIG.GetNode<Sprite2D>("checkmark").Visible = _onlyCommIrr;
		
		_activeT = Config.getTimerStatus();
		_activeTimer.GetNode<Sprite2D>("checkmark").Visible = _activeT;

		_backButton.Pressed += OnBackPressed;
		_applyButton.Pressed += OnApplyPressed;
		_commIG.Pressed += OnCommIGPressed;
		_activeTimer.Pressed += OnActiveTimerPressed;
		_incTimer.Pressed += OnTimeIncPressed;
		_decTimer.Pressed += OnTimeDecPressed;

	}

	private void OnBackPressed()
	{
		if (Config.wasInLvlMenu)
		{
			GetTree().ChangeSceneToFile("res://src/lvl_menu/lvl_menu.tscn");
		}
		else
		{
			GetTree().ChangeSceneToFile("res://src/main_menu/main_menu.tscn");
		}
	}

	private void OnApplyPressed()
	{
		Config.setIrrGStatus(_onlyCommIrr);
		Config.setTimerStatus(_activeT);
		Config.SetTimerLen(_timerStartTime);
	}

	private void OnCommIGPressed()
	{
		_onlyCommIrr = !_onlyCommIrr;
		_commIG.GetNode<Sprite2D>("checkmark").Visible = _onlyCommIrr;
	}

	private void OnActiveTimerPressed()
	{
		_activeT = !_activeT;
		_activeTimer.GetNode<Sprite2D>("checkmark").Visible = _activeT;
	}

	private void OnTimeIncPressed()
	{
		GD.Print("Time inc");
		if (_timerStartTime >= 1 && _timerStartTime < 120)
		{
			GD.Print("Time inc");
			_timerStartTime += 1;
			_timerStartInput.Text = _timerStartTime.ToString();
		}
	}

	private void OnTimeDecPressed()
	{
		GD.Print("Time dec");
		if (_timerStartTime > 1 && _timerStartTime <= 120)
		{
			GD.Print("Time dec");
			_timerStartTime -= 1;
			_timerStartInput.Text = _timerStartTime.ToString();
		}
	}

}
