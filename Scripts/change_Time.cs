using Godot; // Import the Godot namespace
using System; // Import the System namespace (though not used in this script)

public class change_Time : Node
{
    // Define constants for different time scales
    private const float TimeScale2 = 10.0f;
    private const float TimeScale1 = 0.5f;

    // Called when the node is added to the scene
    public override void _Ready()
    {
        // Set the initial time scale to 0.5 when the scene is ready
        Engine.TimeScale = 0.5f;
    }

    // Called every frame with the frame's delta time
    public override void _Process(float delta)
    {
        // Check if the "return to menu" action is pressed
        if (Input.IsActionPressed("return to menu"))
        {
            // Change the current scene to the main menu
            GetTree().ChangeScene("res://Scenes/Main_Menu.tscn");
        }

        // Check if the "Pause" action is pressed
        if (Input.IsActionPressed("Pause"))
            Engine.TimeScale = 0f; // Pause the game by setting time scale to 0

        // Check if the "Time1" action is pressed
        else if (Input.IsActionPressed("Time1"))
            Engine.TimeScale = 1.0f; // Set the time scale to normal speed

        // Check if the "Time2" action is pressed
        else if (Input.IsActionPressed("Time2"))
            Engine.TimeScale = TimeScale1; // Set the time scale to the first defined speed (2.0f)

        // Check if the "Time3" action is pressed
        else if (Input.IsActionPressed("Time3"))
            Engine.TimeScale = TimeScale2; // Set the time scale to the second defined speed (10.0f)
    }
}
