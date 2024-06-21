using Godot;
using System;


public class Door : Area2D {
	// Properties to export for editor visibility and manipulation.
	[Export] public string destination;            // Destination scene or area identifier.
	[Export] public string Cameralimit1;           // First camera limit associated with the door.
	[Export] public string Cameralimit2;           // Second camera limit associated with the door.
	[Export] public bool IsOnlyForPlayer;          // Flag indicating if the door is exclusive to the player.
	[Export] public int POIDependance;             // Point of interest dependency identifier.
	[Export] public string GroupToRespawn;         // Group that triggers respawn actions upon passing through the door.
	[Export] public string PnjGroupToActivate;     // Group of NPCs to activate upon passing through the door.
	[Export] public string PnjGroupToDeactivate;   // Group of NPCs to deactivate upon passing through the door.
	[Export] public string TargetLevel;            // Level or scene to load upon passing through the door.
}
