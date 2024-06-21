using Godot;
using System;
using System.Collections.Generic;


public class Game : Node {
    private Dictionary<string, PlayerViewportinfo> _viewportinfo = new Dictionary<string, PlayerViewportinfo>();
    private ColorRect _seprator;

    private struct PlayerViewportinfo {
        public string ViewportPath;
        public string camera;
        public string player;
    }

    protected void UpdateSize() {
        // Default size of your viewports
        Vector2 DefaultSize = new Vector2(480, 540);

        // Current size of the game window
        Vector2 WindowSize = GetViewport().Size;

        // Calculate the maximum size for each viewport container
        float viewportSize = WindowSize.x / 2 - 5;

        // Calculate the scale factor based on the smaller dimension to maintain aspect ratio
        float ScaleFactor = Math.Min(viewportSize / DefaultSize.x, WindowSize.y / DefaultSize.y);
        Vector2 ViewportScale = new Vector2(ScaleFactor, ScaleFactor);
        
        // Set the scale and position of the first ViewportContainer
        ViewportContainer viewportContainer1 = GetNode<ViewportContainer>("Base/ViewportContainer");
        viewportContainer1.RectScale = ViewportScale;
        viewportContainer1.SetPosition(new Vector2(viewportSize + 10, (WindowSize.y - DefaultSize.y * ScaleFactor) / 2));
        
        // Set the position and size of the separator (ColorRect)
        _seprator = GetNode<ColorRect>("Base/ColorRect");
        _seprator.SetPosition(new Vector2(viewportSize, 0));
        _seprator.SetSize(new Vector2(10, WindowSize.y));
        
        // Set the scale and position of the second ViewportContainer
        ViewportContainer viewportContainer2 = GetNode<ViewportContainer>("Base/ViewportContainer2");
        viewportContainer2.RectScale = ViewportScale;
        viewportContainer2.SetPosition(new Vector2(viewportSize - DefaultSize.x * ScaleFactor, (WindowSize.y - DefaultSize.y * ScaleFactor) / 2));
        
        // Set the size of the background ColorRect to match the window size
        GetNode<ColorRect>("BG").SetSize(WindowSize);
        
        // Adjust the scales of the pause and death menus
        CanvasLayer pause = GetNode<CanvasLayer>("Base/PauseMenu");
        CanvasLayer death = GetNode<CanvasLayer>("Base/DeathMenu");

        pause.Scale = WindowSize / new Vector2(DefaultSize.y, DefaultSize.x);
        death.Scale = pause.Scale;
    }
}

