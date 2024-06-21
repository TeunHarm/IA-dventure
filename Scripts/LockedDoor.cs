using Godot;
using System;

public class LockedDoor : Node2D
{
    public bool IsOpen = false; // Flag to track whether the door is open or closed.

    public void entered(PhysicsBody2D body)
    {
        if (IsOpen) return; // If the door is already open, exit the method.

        if (body is Player player) // Checks if the entering body is a Player.
        {
            if (player.HasKey) // Checks if the player has a key.
            {
                IsOpen = true; // Marks the door as open.

                // Accesses the child node named "Closed" and removes it from the scene.
                Node2D Closed = GetNode<Node2D>("Closed");
                RemoveChild(Closed); // Removes the "Closed" node from the parent (this node).
                Closed.QueueFree(); // Frees the memory associated with the "Closed" node.

                // Sets the "Open" child node visible to show the open door.
                GetNode<Node2D>("Open").Visible = true;
            }
        }
    }
}
