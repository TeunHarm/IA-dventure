using Godot;
using System;

public class TrainingCam : Camera2D
{
    public override void _Process(float delta)
    {
        base._Process(delta);  // Call base _Process method for Camera2D

        const float Speed = 30f;

        // Move the camera based on keyboard input
        if (Input.IsActionPressed("right")) {
            GlobalPosition += Vector2.Right * Speed;
        }
        if (Input.IsActionPressed("left")) {
            GlobalPosition += Vector2.Left * Speed;
        }
        if (Input.IsActionPressed("down")) {
            GlobalPosition += Vector2.Down * Speed;
        }
        if (Input.IsActionPressed("up")) {
            GlobalPosition += Vector2.Up * Speed;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);  // Call base _Input method for Camera2D

        // Restart the scene if the 'G' key is pressed
        if (Input.IsPhysicalKeyPressed((int)KeyList.G))
        {
            // Call "restart" function in the current scene
            GetTree().CurrentScene.Call("restart");
        }
    }
}

