using Godot;
using System;

public class Dialogue : Control
{
    [Export]
    public string d_file; // Filename of the dialogue JSON file

    public bool isAiDialogueBox = false; // Flag indicating if this is an AI dialogue box

    private Godot.Collections.Array dialogue; // Array to store loaded dialogue data
    private int current_dialogue_id = 0; // Index of the current dialogue in the array
    private bool d_active = false; // Flag indicating if dialogue is active

    private float lastupdate; // Timer to control dialogue update frequency

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Hide the dialogue box initially
        GetNode<NinePatchRect>("NinePatchRect").Visible = false;
        // Start loading and displaying dialogue
        Start();
    }

    // Function to start displaying dialogue
    public void Start()
    {
        if (d_active) // If dialogue is already active, return
            return;
        
        d_active = true; // Set dialogue as active
        GetNode<NinePatchRect>("NinePatchRect").Visible = true; // Show the dialogue box

        dialogue = LoadDialogue(); // Load dialogue from JSON file
        current_dialogue_id = -1; // Initialize dialogue index
        lastupdate = 0; // Initialize last update timer
        NextScript(); // Display the next script in dialogue
    }

    // Function to end dialogue
    public void End()
    {
        d_active = false; // Set dialogue as inactive
        GetNode<NinePatchRect>("NinePatchRect").Visible = false; // Hide the dialogue box
    }

    // Function to load dialogue from JSON file
    private Godot.Collections.Array LoadDialogue()
    {
        if (!string.IsNullOrEmpty(d_file)) // Check if dialogue file name is provided
        {
            File file = new File(); // Create a new File instance
            // Attempt to open the JSON file
            if (file.Open("C://Users//dimit//AppData//Roaming//Godot//app_userdata//Ia-dventure/json/" + d_file + ".json", File.ModeFlags.Read) == Error.Ok)
            {
                return JSON.Parse(file.GetAsText()).Result as Godot.Collections.Array; // Parse and return JSON data as array
            }
            else
            {
                GD.PrintErr("Unable to open file: " + d_file); // Print error if file opening fails
            }
        }
        else
        {
            GD.Print("File missing"); // Print message if dialogue file name is missing
        }
        return new Godot.Collections.Array(); // Return an empty array if dialogue loading fails
    }

    // Input event handling function
    public override void _Input(InputEvent @event)
    {
        // If this is an AI dialogue box, update automatically
        if (isAiDialogueBox)
        {
            if (lastupdate > 1.5) // Update dialogue after 1.5 seconds
            {
                lastupdate = 0; // Reset update timer
                NextScript(); // Display the next script in dialogue
            }
            else
            {
                lastupdate += GetProcessDeltaTime() * Engine.TimeScale; // Increment update timer
            }
            return; // Exit input event handling
        }

        // If dialogue is not active, exit input event handling
        if (!d_active)
            return;

        // Check if 'Action2' input action is pressed to display next dialogue script
        if (Input.IsActionPressed("Action2"))
        {
            NextScript(); // Display the next script in dialogue
        }
    }

    // Function to display the next script in dialogue
    public void NextScript()
    {
        current_dialogue_id++; // Increment dialogue index

        // Check if all dialogue scripts have been displayed
        if (current_dialogue_id >= dialogue.Count)
        {
            d_active = false; // Set dialogue as inactive
            GetNode<NinePatchRect>("NinePatchRect").Visible = false; // Hide the dialogue box
            return; // Exit function
        }

        // Get the current dialogue script from the array
        var currentDialogue = (Godot.Collections.Dictionary)dialogue[current_dialogue_id];
        // Set the name and text of the dialogue box
        GetNode<RichTextLabel>("NinePatchRect/Name").Text = (string)currentDialogue["name"];
        GetNode<RichTextLabel>("NinePatchRect/Chat").Text = (string)currentDialogue["text"];
    }
}
