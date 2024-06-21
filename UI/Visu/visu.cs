using Godot;
using System;
using Godot.Collections;
using Array = System.Array;
using Object = Godot.Object;

public class visu : Panel
{
// UI elements
    private Container inputs; // Container for input nodes
    private VBoxContainer outputs; // Container for output nodes

    // Reference to AI
    private AI ai; // Reference to the AI script

    // Input and Output labels
    private String[] Ins = { "Distance", "0 < Angle < 45", "45 < Angle < 90", "90 < Angle < 135", "135 < Angle < 180", "180 < Angle < 225", "225 < Angle < 270", "270 < Angle < 315", "315 < Angle < 360", "Dist rayon B", "Dist rayon BD", "Dist rayon D", "Dist rayon HD", "Dist rayon H", "Dist rayon HG", "Dist rayon G", "Dist rayon BG" };
    private String[] Outs = { "Right", "Left", "Up", "Down" }; // Output labels

    // Arrays to hold references to UI elements
    private Array<RichTextLabel> InValues = new Array<RichTextLabel>(); // Array for input values
    private Array<RichTextLabel> OutValues = new Array<RichTextLabel>(); // Array for output values
    private Array<TextureRect> OutIcons = new Array<TextureRect>(); // Array for output icons

    // Icons for on and off states
    private Texture onIcon; // Texture for on state
    private Texture offIcon; // Texture for off state

    // Array to hold Line2D elements representing raycast lines
    private Array<Line2D> rayLines = new Array<Line2D>(); 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialize UI elements
        inputs = GetNode<Container>("HBoxContainer/VBoxContainer/CenterContainer/Inputs");
        outputs = GetNode<VBoxContainer>("HBoxContainer/VBoxContainer2/CenterContainer/Outputs");

        // Load icons
        onIcon = GD.Load<Texture>("res://UI/Visu/nodeOn.png");
        offIcon = GD.Load<Texture>("res://UI/Visu/node.png");

        // Find AI node in the scene
        if (!GetTree().HasGroup("AI"))
        {
            GD.PrintErr("AI Visu: AI not found.");
            return;
        }
        ai = GetTree().GetNodesInGroup("AI")[0] as AI; // Get the AI node from the scene
        if (ai is null)
        {
            GD.PrintErr("AI Visu: AI is null.");
            return;
        }

        // Load node scene reference
        PackedScene nodeRef = GD.Load<PackedScene>("res://UI/Visu/Node.tscn");

        // Create input nodes
        for (int i = 0; i < Ins.Length; i++)
        {
            Control node = nodeRef.Instance<Control>(); // Instance a new node from scene
            node.GetNode<RichTextLabel>("VBoxContainer/Name").BbcodeText = "[center]" + Ins[i] + "[/center]"; // Set the node's name
            InValues.Add(node.GetNode<RichTextLabel>("VBoxContainer/CenterContainer/TextureRect/Value")); // Add RichTextLabel to input values array
            inputs.AddChild(node); // Add node to inputs container
        }

        // Create output nodes
        for (int i = 0; i < Outs.Length; i++)
        {
            Control node = nodeRef.Instance<Control>(); // Instance a new node from scene
            node.GetNode<RichTextLabel>("VBoxContainer/Name").BbcodeText = "[center]" + Outs[i] + "[/center]"; // Set the node's name
            OutValues.Add(node.GetNode<RichTextLabel>("VBoxContainer/CenterContainer/TextureRect/Value")); // Add RichTextLabel to output values array
            OutIcons.Add(node.GetNode<TextureRect>("VBoxContainer/CenterContainer/TextureRect")); // Add TextureRect to output icons array
            outputs.AddChild(node); // Add node to outputs container
        }

        // Create raycast lines for visualization
        foreach (RayCast2D ray in ai.GetRayscasts())
        {
            Line2D line = new Line2D(); // Create a new Line2D node
            line.Visible = false; // Set initial visibility to false
            line.AddPoint(new Vector2(0, 0)); // Add starting point
            line.AddPoint(ray.CastTo); // Add ending point based on raycast's CastTo position
            line.Width = 1; // Set line width
            line.Antialiased = true; // Enable antialiasing
            line.EndCapMode = Line2D.LineCapMode.Round; // Set end cap mode
            rayLines.Add(line); // Add Line2D node to rayLines array
            ai.GetParent().AddChild(line); // Add Line2D node to AI's parent node in the scene
        }
    }

    // Process function called every frame
    public override void _Process(float delta)
    {
        base._Process(delta); // Call base class _Process method

        // Check if node is visible
        if (!Visible) return;

        // Update input values
        for (int i = 0; i < InValues.Count; i++)
        {
            if (i < ai.LastIn.Count)
                InValues[i].BbcodeText = "[center]" + ai.LastIn[i] + "[/center]"; // Update input value text
            else
                InValues[i].BbcodeText = "[center]nan[/center]"; // Display 'nan' if no input value available
        }
        
        // Update output values and icons
        for (int i = 0; i < OutValues.Count; i++)
        {
            if (i < ai.LastOut.Count)
            {
                OutValues[i].BbcodeText = "[center]" + ai.LastOut[i] + "[/center]"; // Update output value text
                OutIcons[i].Texture = ai.LastOut[i] > 0.5f ? onIcon : offIcon; // Update output icon based on value
            }
            else
                OutValues[i].BbcodeText = "[center]nan[/center]"; // Display 'nan' if no output value available
        }

        // Update raycast lines for visualization
        for (int i = 0; i < rayLines.Count; i++)
        {
            // Position raycast line and set color based on collision
            rayLines[i].GlobalPosition = ai.GetRayscasts()[i].GlobalPosition;
            rayLines[i].SetPointPosition(1, ai.GetRayscasts()[i].IsColliding() ? rayLines[i].ToLocal(ai.GetRayscasts()[i].GetCollisionPoint()) : ai.GetRayscasts()[i].CastTo);
            rayLines[i].DefaultColor = ai.GetRayscasts()[i].IsColliding() ? Colors.Aqua : Colors.Red;
        }
    }

    // Input event handling
    public override void _Input(InputEvent @event)
    {
        base._Input(@event); // Call base class _Input method

        // Toggle visibility on 'toggle_visu' action
        if (Input.IsActionPressed("toggle_visu"))
        {
            Visible = !Visible; // Toggle visibility
            
            // Toggle visibility of raycast lines
            foreach (Line2D ray in rayLines)
            {
                ray.Visible = Visible; // Set ray visibility
            }
        }
    }
}
