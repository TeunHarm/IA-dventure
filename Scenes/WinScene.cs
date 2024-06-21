using Godot;
using System;

public class WinScene : Control
{
	private void _on_Button_pressed()
	{
		GetTree().Quit();
	}
}
