using Godot;
using System;
using System.Globalization;
using Godot.Collections;
using Object = Godot.Object;




public class InputCapture : Node {
	[Export]
	private bool DoCapture = true;

	private File CSV;
	private Player Player;
	private Array<RayCast2D> Raycasts;

	private Node2D LastPOI = null;
	private string LastPOIHash;

	private int _captureCount = 0;
	private string _lastLine = "";

	private int _lastInput = 0;

	public override void _Ready()
	{
		if (!DoCapture)
			return;
		
		CSV = new File();
		CSV.Open("user://input-" + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "-" + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + ".csv", File.ModeFlags.Write);
		
		if (!CSV.IsOpen())
		{
			GD.Print("InputCapture: Failed to create file, aborting.");
			DoCapture = false;
			return;
		}

		if (GetTree().HasGroup("Player"))
		{
			GD.Print("InputCapture: Found player group!");
			Player = GetTree().GetNodesInGroup("Player")[0] as Player;
		}
		else
			// Try to get it by path
			Player = GetNode<Player>("Base/ViewportContainer/Viewport/Master/Player");
		
		if (Player is null)
		{
			GD.Print("InputCapture: Player is null, check the groups.");
			DoCapture = false;
			return;
		}

		Raycasts = new Array<RayCast2D>();

		foreach (Node child in Player.GetChildren())
		{
			if (child is RayCast2D)
			{
				RayCast2D ray = child as RayCast2D;
				Raycasts.Add(ray);
				ray.Enabled = true;
			}
		}

		if (Raycasts.Count < 1)
		{
			GD.Print("InputCapture: Error, player has no raycasts, aborting capture.");
			DoCapture = false;
		}
		
		GD.Print("InputCapture: Starting capture!");
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		
		if (!DoCapture)
			return;

		if (!CSV.IsOpen())
			return;

		// Do not capture every single frame
		_captureCount++;

		if (_captureCount > 1)
			_captureCount = 0;

		if (_captureCount > 0)
			return;
		
		// Add the current pressed action
		String line = GetActionCode().ToString();
		line += ";" + _lastInput;
		_lastInput = GetActionCode();
		
		// Add current POI distance
		if (!(Player.CurrentPOI is null))
		{
			if (LastPOI != Player.CurrentPOI)
			{
				LastPOI = Player.CurrentPOI;
				LastPOIHash = ((Objective)Player.CurrentPOI).HashObjective();
			}
			
			line += ";" + (Player.CurrentPOI.GlobalPosition.x - Player.GlobalPosition.x);
			line += ";" + (Player.CurrentPOI.GlobalPosition.y - Player.GlobalPosition.y);
		}
		else
		{
			line += ";0";
			line += ";0";
		}

		// Add the raycasts with object types
		foreach (RayCast2D ray in Raycasts)
		{
			String obj = GetObject(ray);

			if (obj == "Obstacle")
				line += ";" + MakeDistance(ray.GlobalPosition, ray.GetCollisionPoint(), 20000);
			else
				line += ";0";
			
			if (obj == "Enemy")
				line += ";" + MakeDistance(ray.GlobalPosition, ray.GetCollisionPoint(), 20000);
			else
				line += ";0";
			
			if (obj == "Objective")
				line += ";" + MakeDistance(ray.GlobalPosition, ray.GetCollisionPoint(), 20000);
			else
				line += ";0";
		}

		// Only store new frames
		if (_lastLine == line)
			return;

		_lastLine = line;

		// Store it in the file
		CSV.StoreString(line + "\n");
	}

	public override void _ExitTree()
	{
		if (DoCapture && CSV.IsOpen())
		{
			String path = CSV.GetPathAbsolute();
			CSV.Close();
			GD.Print("InputCapture: Saved CSV file at " + path + ".");
		}
		
		base._ExitTree();
	}

	private int GetActionCode()
	{
		Dictionary<String, int> actions = new Dictionary<string, int>();
		actions.Add("up", 1);
		actions.Add("down", 2);
		actions.Add("left", 3);
		actions.Add("right", 4);
		//actions.Add("run", 5);
		actions.Add("attack", 0);

		foreach (var action in actions)
		{
			if (Input.IsActionPressed(action.Key))
				return action.Value;
		}

		return 0;
	}

	private String GetObject(RayCast2D ray)
	{
		if (ray.IsColliding())
		{
			Object hit = ray.GetCollider();

			if (hit is EnemyBase)
				return "Enemy";

			if (hit is Objective)
				return "Objective";
			
			return "Obstacle";
		}

		return "Nothing";
	}

	private float MakeDistance(Vector2 A, Vector2 B, float Divider)
	{
		if (B == null)
			return 99999999;
		
		float Dist = A.DistanceSquaredTo(B) / Divider;
		return Dist <= 0.001f ? 0 : Dist;
	}
}

