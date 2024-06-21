using Godot;
using System;

public class Key : Node2D
{
	public bool GotKey = false;  // Flag to track whether the key has been obtained by a player.
    
	public void entered(PhysicsBody2D body)
	{
		if (GotKey) return;  // If the key has already been obtained, exit the method.
        
		if (body is Player player)  // Checks if the entering body is of type Player.
		{
			GotKey = true;  // Marks the key as obtained.
			player.HasKey = true;  // Sets the player's HasKey property to true, indicating they now have the key.
            
			// Hide the first Sprite child found
			foreach (Node2D Child in GetChildren())  // Iterates through all child nodes of this node.
			{
				if (Child is Sprite sprite)  // Checks if the child node is a Sprite.
				{
					sprite.Visible = false;  // Hides the Sprite by setting its Visible property to false.
					break;  // Stops iterating further as soon as the first Sprite is found and hidden.
				}
			}
		}
	}
}

