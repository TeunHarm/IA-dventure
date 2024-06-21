using Godot;
using System;
using Godot.Collections;

public class ObjectiveNav : Node2D
{
    // The navigation map identifier
    protected RID NavMap = null;

    // Dictionary to store regions by level name
    protected Dictionary<string, RID> Regions = new Dictionary<string, RID>();

    // Called when the node is added to the scene tree
    public override void _Ready()
    {
        // Create a new navigation map
        NavMap = Navigation2DServer.MapCreate();
        
        // Set the navigation map as active
        Navigation2DServer.MapSetActive(NavMap, true);
    }

    // Method to update the navigation region for a specific level
    public void UpdateRegion(string Level)
    {
        // Check if the region for the level already exists
        if (!Regions.ContainsKey(Level))
        {
            // Create a new region for the level
            RID region = Navigation2DServer.RegionCreate();
            
            // Set the transform for the region (typically identity transform)
            Navigation2DServer.RegionSetTransform(region, new Transform2D());
            
            // Set the navigation map for the region
            Navigation2DServer.RegionSetMap(region, NavMap);
            
            // Add the region to the dictionary of regions
            Regions.Add(Level, region);
            
            // Determine the left and right limits of the level
            Vector2 LeftLim = new Vector2(), RightLim = new Vector2();
            GetLevelLimits(Level, ref LeftLim, ref RightLim);
            
            // Create a navigation polygon for the region
            NavigationPolygon navigationPoly = new NavigationPolygon();
            
            // Define vertices of the navigation polygon
            navigationPoly.Vertices = new []{ Vector2.Zero, LeftLim, RightLim };
            
            // Add a polygon to the navigation polygon (in this case, a triangle)
            navigationPoly.AddPolygon(new []{0, 1, 2});
            
            // Set the navigation polygon for the region
            Navigation2DServer.RegionSetNavpoly(region, navigationPoly);
        }
    }

    // Method to get a path from Start to Goal using the navigation map
    public Vector2[] GetPath(Vector2 Start, Vector2 Goal)
    {
        // Retrieve the path using the navigation map
        return Navigation2DServer.MapGetPath(NavMap, Start, Goal, false);
    }

    // Method to determine the left and right limits of a level
    protected void GetLevelLimits(string Level, ref Vector2 LeftLimit, ref Vector2 RightLimit)
    {
        // Retrieve the first TileMap node in the specified level group
        LeftLimit = (Vector2)((TileMap)GetTree().GetNodesInGroup(Level)[0]).GetUsedCells()[0];
        RightLimit = LeftLimit;
        
        // Iterate through all TileMap nodes in the specified level group
        foreach (TileMap Elem in GetTree().GetNodesInGroup(Level))
        {
            // Iterate through each used cell in the TileMap
            foreach (Vector2 MapPos in Elem.GetUsedCells())
            {
                // Convert the map position to global coordinates
                Vector2 Pos = Elem.ToGlobal(Elem.MapToWorld(MapPos));
                
                // Update the left limit coordinates
                if (LeftLimit.x > Pos.x)
                {
                    LeftLimit.x = Pos.x;
                }
                if (LeftLimit.y > Pos.y)
                {
                    LeftLimit.y = Pos.y;
                }
                
                // Update the right limit coordinates
                if (RightLimit.x < Pos.x)
                {
                    RightLimit.x = Pos.x;
                }
                if (RightLimit.y < Pos.y)
                {
                    RightLimit.y = Pos.y;
                }
            }
        }
    }
}

