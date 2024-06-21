using Godot;
using System;

public class PauseMenu : Control {
	// Called when the node is added to the scene
	public override void _Ready() {
		// Initialization code can be placed here if needed
	}

	// Method to hide the menu by setting the Visible property to false
	public void hideMenu() {
		Visible = false;
	}

	// Signal handler for when the BackButton is pressed
	private void _on_BackButton_pressed() {
		// Call the hideMenu method to hide the menu
		hideMenu();
	}

	// Signal handler for when the QuitButton is pressed
	private void _on_QuitButton_pressed() {
		// Quit the application
		GetTree().Quit();
	}
}


