using Godot;
using System;

public class Heart : Area2D
{
    private void _on_Heart_body_entered(KinematicBody2D body)
    {
        if (body is Player body2)
        {
            body2.Player_Heal(2);  // Calls Player_Heal method on the player object to heal by 2 points.
            QueueFree();  // Frees the heart object from the scene hierarchy.
        }
    }
}

