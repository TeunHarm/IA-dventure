using Godot;
using System;


public class Cestlafleche : Control {
    // Exported variable to specify the group name containing the Player node.
    [Export] private string PlayerGroup = "Player";
    
    // Reference to the Player node and the current Point of Interest (POI).
    private Player _player;
    private Node2D _obj;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // Check if the specified PlayerGroup exists in the scene tree.
        if (GetTree().HasGroup(PlayerGroup)) {
            // Retrieve the Player node from the group.
            _player = (Player)GetTree().GetNodesInGroup(PlayerGroup)[0];
            // Set the initial Point of Interest (POI) to the Player's current POI.
            _obj = _player.CurrentPOI;
        }
        else {
            // Print an error message if the PlayerGroup is not found.
            GD.PrintErr("Fleche: Group '" + PlayerGroup + "' not found.");
        }
    }
    
    // Called every frame. Handles updating the arrow's rotation towards the current POI.
    public override void _Process(float delta) {
        // If player reference is null, exit the method.
        if (_player == null) return;
        
        // Update the rotation of the arrow towards the current POI.
        if (_obj != null) {
            // Calculate the angle in radians between the player and the current POI.
            double angleRadians = Math.Atan2(_player.GlobalPosition.y - _obj.GlobalPosition.y,
                                             _player.GlobalPosition.x - _obj.GlobalPosition.x);
            // Set the rotation of the Sprite child node to face the POI.
            this.GetChild<Sprite>(0).Rotation = (float)angleRadians;
        }
        else {
            // If no current POI, reset the rotation of the Sprite child node to zero.
            this.GetChild<Sprite>(0).Rotation = 0;
        }

        // Update _obj to the player's current POI for the next frame.
        _obj = _player.CurrentPOI;
    }
}

