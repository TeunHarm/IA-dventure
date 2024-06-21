using Godot;
using System;
using Godot.Collections;


public class AStar : AStar2D
{
    [Export] public string Type = "Player";
    [Export] public int GridSize = 32;

    private Vector2 LeftLimit;
    private Vector2 RightLimit;
    private Array<TileMap> CurrentMaps = new Array<TileMap>();

    public int Distance(Vector2 A, Vector2 B)
    {
        int APoint = GetClosestPoint(A), BPoint = GetClosestPoint(B);

        if (APoint < 0 || GetPointPosition(APoint).DistanceTo(A) > GridSize)
        {
            return -1;
        }
        if (BPoint < 0 || GetPointPosition(BPoint).DistanceTo(B) > GridSize)
        {
            return -1;
        }
        
        return GetPointPath(APoint, BPoint).Length;
    }

    public Vector2[] Path(Vector2 A, Vector2 B)
    {
        int APoint = GetClosestPoint(A), BPoint = GetClosestPoint(B);

        if (APoint < 0 || GetPointPosition(APoint).DistanceTo(A) > GridSize)
        {
            return null;
        }
        if (BPoint < 0 || GetPointPosition(BPoint).DistanceTo(B) > GridSize)
        {
            return null;
        }

        return GetPointPath(APoint, BPoint);
    }


    public void UpdateTiles(SceneTree Tree, string Level)
    {
        CurrentMaps.Clear();
        LeftLimit = (Vector2)((TileMap)Tree.GetNodesInGroup(Level)[0]).GetUsedCells()[0];
        RightLimit = LeftLimit;
        
        foreach (TileMap Elem in Tree.GetNodesInGroup(Level))
        {
            // Add all the tilemaps with collisions to the list
            foreach (int id in Elem.TileSet.GetTilesIds())
            {
                bool Found = false;
                foreach (Dictionary Shape in Elem.TileSet.TileGetShapes(id))
                {
                    if (Shape["shape"] != null)
                    {
                        CurrentMaps.Add(Elem);
                        Found = true;
                        break;
                    }
                }
                if (Found)
                    break;
            }
            
            // Calculate the limits of the grid
            foreach (Vector2 MapPos in Elem.GetUsedCells())
            {
                Vector2 Pos = Elem.ToGlobal(Elem.MapToWorld(MapPos));
                if (LeftLimit.x > Pos.x)
                {
                    LeftLimit.x = Pos.x;
                }
                if (LeftLimit.y > Pos.y)
                {
                    LeftLimit.y = Pos.y;
                }
                
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
        
        // Clear all the old points
        foreach (int id in GetPoints())
            RemovePoint(id);
        
        // Add the new points
        int currInd = 0;
        int xSize = (int)(Math.Floor(RightLimit.x - LeftLimit.x)) / GridSize;
        int ySize = (int)(Math.Floor(RightLimit.y - LeftLimit.y)) / GridSize + 1;
        int[] Neigbors = { -1, -xSize, -xSize - 1, -xSize + 1 };
        for (int y = (int)Math.Floor(LeftLimit.y); y < RightLimit.y; y += GridSize)
        {
            for (int x = (int)Math.Floor(LeftLimit.x); x < RightLimit.x; x += GridSize)
            {
                AddPoint(currInd, new Vector2(x, y));
                
                // Make the connections
                foreach (int Neighbor in Neigbors)
                {
                    if (currInd + Neighbor > 0 && currInd + Neighbor < xSize*ySize)
                    {
                        ConnectPoints(currInd, currInd + Neighbor);
                    }
                }
                
                currInd++;
            }
        }
        
        // Connect the points
        /*int[] Neigbors = { 1, -1, xSize, xSize + 1, xSize - 1, -xSize, -xSize + 1, -xSize - 1 };
        currInd = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                foreach (int Neighbor in Neigbors)
                {
                    if (currInd + Neighbor > 0 && currInd + Neighbor < xSize*ySize)
                    {
                        ConnectPoints(currInd, currInd + Neighbor);
                    }
                }
                currInd++;
            }
        }*/
    }
    

    public override float _ComputeCost(int fromId, int toId)
    {
        bool hasColl = false;
        foreach (TileMap Map in CurrentMaps)
        {
            Vector2 CellPos = Map.WorldToMap(Map.ToLocal(GetPointPosition(toId)));
            int CellId = Map.GetCell((int)CellPos.x, (int)CellPos.y);
            if (CellId != TileMap.InvalidCell)
            {
                foreach (Dictionary Shape in Map.TileSet.TileGetShapes(CellId))
                {
                    if (Shape["shape"] != null)
                    {
                        hasColl = true;
                        break;
                    }
                }
                if (hasColl)
                    break;
            }
        }
        
        return hasColl ? 9999f : 0.1f;
    }

    public override float _EstimateCost(int fromId, int toId)
    {
        return GetPointPosition(fromId).DistanceTo(GetPointPosition(toId));
    }
}
