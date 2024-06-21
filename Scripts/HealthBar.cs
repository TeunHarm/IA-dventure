using Godot;
using System;
using System.Collections.Generic;


public class HealthBar : Control
{
    [Export]
    private bool
        isAi = false; // This declares a private boolean variable 'isAi' that is exposed to the editor for exporting.

    private Player _player; // Declares a private variable '_player' of type 'Player'.

    private List<StreamTexture>
        HeartIcons = new List<StreamTexture>(); // Creates a list 'HeartIcons' to store StreamTexture objects.

    private List<Sprite> Hearts = new List<Sprite>(); // Creates a list 'Hearts' to store Sprite objects.

    public override void _Ready()
    {
        // Called when the node is added to the scene.

        // Loads textures into 'HeartIcons' list.
        HeartIcons.Add(GD.Load<StreamTexture>("res://Arts/UI/Heart0.png"));
        HeartIcons.Add(GD.Load<StreamTexture>("res://Arts/UI/Heart1.png"));
        HeartIcons.Add(GD.Load<StreamTexture>("res://Arts/UI/Heart2.png"));

        // Creates 5 sprites (hearts) and adds them to the scene.
        for (int i = 0; i < 5; i++)
        {
            Sprite h = new Sprite();
            h.Texture = HeartIcons[2]; // Sets initial texture for the heart.
            h.Position = new Vector2(20 * i, 0); // Sets position of each heart horizontally.
            Hearts.Add(h); // Adds the sprite to the 'Hearts' list.
            AddChild(h); // Adds the sprite as a child node of this node.
        }

        // Checks if the tree has a group named "AI" or "Player" based on the value of 'isAi'.
        if (GetTree().HasGroup(isAi ? "AI" : "Player"))
        {
            _player = (Player)GetTree()
                .GetNodesInGroup(isAi ? "AI" : "Player")
                [0]; // Retrieves the first node in the group and casts it to 'Player'.
        }
        else
        {
            GD.PrintErr(
                "HealthBar: Failed to find owning group."); // Prints an error message if the group is not found.
        }
    }

    public override void _Process(float delta)
    {
        // Called every frame. 'delta' is the elapsed time since the previous frame.

        if (_player == null) return; // If '_player' is null, returns early.

        // Calculates the health percentage of the player and updates the heart textures accordingly.
        float hp = (float)_player.Player_getHP() / 2; // Retrieves player's health points and calculates half of it.
        for (int i = 0; i < Hearts.Count; i++)
        {
            if (i + 1 <= hp)
                Hearts[i].Texture = HeartIcons[2]; // Full heart texture.
            else if (i + 0.5 <= hp)
                Hearts[i].Texture = HeartIcons[1]; // Half heart texture.
            else
                Hearts[i].Texture = HeartIcons[0]; // Empty heart texture.
        }
    }
}

