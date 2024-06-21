using Godot;
using System;



public class MasterScene : Node2D
{
    // Determines whether this instance will create a Player or AI
    [Export] public string Type = "Player";  // Exported variable to specify the type ("Player" or "AI")
    [Export] public bool AutoCreate = true;  // Exported variable to enable/disable automatic creation

    public override void _Ready()
    {
        if (!AutoCreate) return;  // If AutoCreate is false, do not proceed with automatic creation

        // Attempt to find and interact with the PlayerContainer node
        if (GetNode<Node2D>(GetPath() + "/PlayerContainer") is Node2D PlayerCont)
        {
            // Load the appropriate scene based on the Type
            PackedScene playerScene = Type == "Player" ? GD.Load<PackedScene>("res://Characters/Player.tscn") : GD.Load<PackedScene>("res://Characters/AI.tscn");
            
            // Instantiate an instance of the player or AI scene
            Player playerRef = playerScene.Instance<Player>();

            if (playerRef != null)
            {
                // Add the player or AI instance as a child of PlayerContainer
                PlayerCont.AddChild(playerRef);

                // Create and configure a Camera node for the player or AI
                Camera Cam = new Camera();
                Cam.Type = Type;
                Cam.Current = true;
                Cam.Zoom = new Vector2(0.6f, 0.6f);
                Cam.SmoothingEnabled = Type != "Player";
                Cam.SmoothingSpeed = 10;
                playerRef.AddChild(Cam);  // Add Camera node as a child of player or AI
                playerRef.CameraRef = Cam;  // Set CameraRef property in player or AI

                playerRef.AddToGroup(Type);  // Add player or AI instance to a group based on Type

                // Initialize and configure pathfinding for the player or AI
                playerRef.PathFinding = new AStar();
                playerRef.PathFinding.Type = Type;
            }
            else
            {
                GD.Print("MasterScene: Failed to load player resource, make sure its path is correct.");
            }
        }
        else
        {
            GD.Print("MasterScene: Failed to find player container, does it exist in the master scene?");
        }
    }
}
