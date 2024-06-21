using Godot;
using System;
using System.Linq;


public class Camera : Camera2D
{
    // Public variable to determine the type of camera (default: "Player").
    public string Type = "Player";
    
    // Default size of the camera viewport.
    Vector2 DefaultSize = new Vector2(360, 405);
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // Initialize variables to store limit nodes.
        Node2D A = null;
        Node2D B = null;

        // Get the name of the current scene.
        string sceneName = GetTree().CurrentScene.Name;

        // Depending on the scene, assign limits A and B accordingly.
        if (sceneName == "Start_village") {
            A = GetLimit("CameraLimit1");
            B = GetLimit("CameraLimit2");
        }
        else {
            if (Type == "Player") {
                A = GetLimit("CameraLimit9");
                B = GetLimit("CameraLimit10");
            }
            else {
                A = GetLimit("CameraLimit9");
                B = GetLimit("CameraLimit10");
            }
        }

        // If both limits are valid, set the camera limits and hide the limit nodes.
        if (A != null && B != null) {
            LimitLeft = (int)Math.Min(A.Position.x, B.Position.x);
            LimitTop = (int)Math.Min(A.Position.y, B.Position.y);
            LimitRight = (int)Math.Max(A.Position.x, B.Position.x);
            LimitBottom = (int)Math.Max(A.Position.y, B.Position.y);

            A.Hide();
            B.Hide();
        }
        else {
            GD.PrintErr("The camera limits aren't valid (check their names).");
        }
        
        // Connect the size_changed signal of the viewport to the UpdateZoom method.
        GetViewport().Connect("size_changed", this, "UpdateZoom");
        // Call UpdateZoom to initialize the camera zoom.
        UpdateZoom();
    }

    // Virtual method to retrieve a camera limit node by name.
    public virtual Node2D GetLimit(String Limit)
    {
        // Attempt to find and return the Node2D limit node.
        if (GetParent().GetParent().GetParent().GetNode<Node2D>(Limit) is Node2D lim)
            return lim;
        
        // Print an error message if the limit node is not found and return null.
        GD.Print("ERROR: Retrieving camera limit (" + Limit + "). Check the name.");
        return null;
    }

    // Method to update the camera zoom based on the viewport size.
    public void UpdateZoom()
    {
        // Calculate the zoom factor based on the default size and current viewport size.
        Vector2 delta = DefaultSize / GetViewport().Size;
        
        // Set the zoom of the camera.
        Zoom = delta;
    }
    
    // Method to update camera limits when the player teleports.
    public void OnPlayerTeleportChangeLimit(string limit1Path, string limit2Path)
    {
        // Retrieve nodes for the new camera limits.
        Node2D A = GetTree().CurrentScene.GetNode<Node2D>(limit1Path);
        Node2D B = GetTree().CurrentScene.GetNode<Node2D>(limit2Path);

        // If both limits are valid, update the camera limits and hide the limit nodes.
        if (A != null && B != null) {
            LimitLeft = (int)Math.Min(A.GlobalPosition.x, B.GlobalPosition.x);
            LimitTop = (int)Math.Min(A.GlobalPosition.y, B.GlobalPosition.y);
            LimitRight = (int)Math.Max(A.GlobalPosition.x, B.GlobalPosition.x);
            LimitBottom = (int)Math.Max(A.GlobalPosition.y, B.GlobalPosition.y);

            A.Hide();
            B.Hide();
        }
        else {
            GD.Print("The camera limits aren't valid (check their names).");
        }
    }
}


