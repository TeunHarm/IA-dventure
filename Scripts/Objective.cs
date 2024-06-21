using Godot;
using System;



public class Objective : Node2D {
	// Radius around the objective area, if greater than 0, it will create a detection area
	[Export] private float Radius = 50;

	// Unique identifier for the objective
	[Export] public string Identifier = "Default";

	// Unique numeric ID for the objective
	[Export] public int Id = 0;

	// ID of the next objective to proceed to after completing this one
	[Export] private int NextPOI = 0;

	// Whether the previous objective is required to validate this one
	[Export] private bool PrevPoiRequiered = false;

	// ID of the objective to go to if the requirement is not met
	[Export] private string Requirement = "";

	// ID of the objective that blocks progression if this objective is blocked
	[Export] private int BlockedPOI = -1;

	// Called when the node is added to the scene tree
	public override void _Ready() {
		// If Radius is greater than 0, create an area for detecting player entry
		if (Radius > 0)
		{
			Area2D area = new Area2D();
			area.Connect("body_entered", this, "Entered");
			CollisionShape2D coll = new CollisionShape2D();
			CircleShape2D circle = new CircleShape2D();
			circle.Radius = Radius;
			coll.Shape = circle;
			area.AddChild(coll);
			area.SetCollisionMaskBit(2, true); // Set collision mask bit (assuming layer 2)
			AddChild(area); // Add detection area as a child of this objective node
		}
	}
	
	// Method called when a player enters the objective area
	public void Entered(Node body) {
		if (body is Player player) {
			// Check if the previous objective must be completed before this one
			if (PrevPoiRequiered && ((Objective)player.CurrentPOI).Id != Id)
				return;  // Return if the previous objective is not completed

			// Move to the next objective if no requirements exist
			if (Requirement == "") {
				player.CurrentPOI = GetObjective(NextPOI);
				if (player.ObjectiveDisplay != null)
					player.ObjectiveDisplay.UpdateObj();
			}
			else {
				// Check if the required condition exists in the player
				if (player.Get(Requirement) == null) {
					GD.Print("ERROR: Requirement {" + Requirement + "} for objective {" + Name +
							 "} does not exist in player.");
				}
				else if (player.Get(Requirement) is bool cond) {
					// Move to the next objective based on the condition
					player.CurrentPOI = cond ? GetObjective(NextPOI) : GetObjective(BlockedPOI);
					if (player.ObjectiveDisplay != null)
						player.ObjectiveDisplay.UpdateObj();
				}
			}
		}
	}

	// Retrieve the objective node with the specified ID from the scene tree
	private Node2D GetObjective(int OtherId) {
		foreach (Objective Obj in GetTree().GetNodesInGroup("Objectives"))
			if (Obj.Id == OtherId)
				return Obj;

		return null;  // Return null if the objective with the specified ID is not found
	}

	// Generate a hash code for the objective identifier
	public String HashObjective() {
		return Convert.ToString(Identifier.GetHashCode(), 2);  // Convert the identifier's hash code to a binary string
	}
}

