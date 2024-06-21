using Godot; // Import the Godot namespace
using System; // Import the System namespace (though not used in this script)

public class TimeDisplay : RichTextLabel {

    // Called when the node is added to the scene
    public override void _Ready() {
        // Set the text of the label to display the current time scale when the scene is ready
        Text = "X " + Engine.TimeScale;
    }
    
    // Called every frame with the frame's delta time
    public override void _Process(float delta) {
        // Update the text of the label to display the current time scale
        Text = "X " + Engine.TimeScale;
    }
}
