using Godot;
using System;


public class ObjectiveDisplay : RichTextLabel {
    // The group name of the player node
    [Export] private string PlayerGroup = "Player";
    
    // Reference to the Player node
    private Player _player;

    // Called when the node is added to the scene tree
    public override void _Ready() {
        // Check if the player group exists in the scene tree
        if (GetTree().HasGroup(PlayerGroup))
        {
            // Get the first node in the player group and cast it to Player
            _player = (Player)GetTree().GetNodesInGroup(PlayerGroup)[0];
            
            // Assign this ObjectiveDisplay instance to the player's ObjectiveDisplay property
            _player.ObjectiveDisplay = this;

            // Update the displayed objective information
            UpdateObj();
        }
        else
        {
            // Print an error message if the specified player group is not found
            GD.PrintErr("ObjectiveDisp: Group " + PlayerGroup + " not found.");
        }
    }

    // Update the displayed objective information
    public void UpdateObj() {
        // Check if the player's current point of interest (POI) is an Objective
        if (_player.CurrentPOI is Objective obj)
            Text = "Objective: " + obj.Identifier;  // Display the identifier of the current objective
        else
            Text = "Objective: " + "None";  // Display "None" if there is no current objective
    }
}

