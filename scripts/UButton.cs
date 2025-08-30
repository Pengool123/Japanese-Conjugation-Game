using Godot;
using System;

[GlobalClass]
public partial class UButton : TextureButton
{
	private Color _defColor;

	private bool _active = true;

	public bool Active
	{
		get => _active;
		set
		{
			_active = value;
			if (!_active)
			{
				Modulate = _defColor.Darkened(.5f);
			}
			else
			{
				Modulate = _defColor;
			}
		}
	}
	
	public override void _Ready()
	{
		_defColor = Modulate;

		MouseEntered += OnMouseEnter;
		MouseExited += OnMouseExit;
		Pressed += BPressed;
	}
	
	
	private void OnMouseEnter()
	{
		if (_active) Modulate = _defColor.Darkened(.3f);
	}

	private void OnMouseExit()
	{
		if (_active) Modulate = _defColor;
	}

	private void BPressed()
	{
		//play a click sound here
	}

}
