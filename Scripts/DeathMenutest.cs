using Godot;
using System;

public class DeathMenutest : Control
{
	// Method to show the death menu.
	public void showMenu()
	{
		Visible = true;  // Make the menu visible.
	}

	// Method to hide the death menu.
	public void hideMenu()
	{
		Visible = false;  // Hide the menu.
	}
    
	// Signal callback method for when the RespawnButton is pressed.
	private void _on_RespawnButton_pressed()
	{
		GD.Print("Button pressed\n");  // Print a debug message indicating button press.
		hideMenu();  // Hide the menu when the RespawnButton is pressed.
	}
}



