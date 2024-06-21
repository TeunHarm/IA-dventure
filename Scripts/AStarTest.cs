using Godot;
using System;

public class AStarTest : Timer
{
    private Player player;

    Line2D Line;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = (Player)GetTree().GetNodesInGroup("Player")[0];
        player.PathFinding.UpdateTiles(GetTree(), player.currentLevel);
        //player.Nav.UpdateRegion(player.currentLevel);
        OneShot = false;
        Start(2);
        Connect("timeout", this, "Update");
        Line = new Line2D();
        Line.Width = 1f;
        player.GetParent().AddChild(Line);
    }

    protected void Update()
    {
        Vector2[] path = player.PathFinding.Path(player.GlobalPosition, player.CurrentPOI.GlobalPosition);
        //Vector2[] path = player.Nav.GetPath(player.GlobalPosition, player.CurrentPOI.GlobalPosition);
        //Vector2[] path = Navigation2DServer.MapGetPath(player.GetRid(), player.GlobalPosition, player.CurrentPOI.GlobalPosition, false);
        if (path != null)
        {
            GD.Print("Objective distance: ", path.Length);

            Line.ClearPoints();
            foreach (Vector2 point in path)
            {
                Line.AddPoint(Line.ToLocal(point));
            }
        }
    }
}
