using Godot;
using System;

public class PnjBase : KinematicBody2D
{
    // Protected fields
    protected string _orientation;         // Current orientation of the character
    protected AnimatedSprite _animatedSprite;  // Reference to the AnimatedSprite node
    protected Vector2 _velocity;           // Velocity vector for movement

    protected int nextPos;                 // Index of the next position in the movement sequence
    protected int maxPos;                  // Total number of registered positions

    // Exported variables
    [Export] private float speed;          // Movement speed
    
    [Export] private Vector2 pos1;         // Position 1 in the movement sequence
    [Export] private Vector2 pos2;         // Position 2 in the movement sequence
    [Export] private Vector2 pos3;         // Position 3 in the movement sequence
    [Export] private Vector2 pos4;         // Position 4 in the movement sequence
    
    public bool activated;                 // Flag indicating if the PNJ is activated
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        activated = false;                 // PNJ is initially deactivated
        _orientation = "Down";            // Initial orientation is down
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("Idle");     // Start with the idle animation
        
        // Determine the number of registered positions
        if (pos4 != new Vector2(0, 0))
            maxPos = 4;
        else if (pos3 != new Vector2(0, 0))
            maxPos = 3;
        else if (pos2 != new Vector2(0, 0))
            maxPos = 2;
        else
            maxPos = 1;
        
        // Set initial next position based on the number of registered positions
        if (maxPos > 1)
            nextPos = 2;
        else
            nextPos = 1;
    }

    // Method to handle movement between positions
    protected void movment()
    {
        switch (nextPos)
        {
            // Move towards position 1
            case 1 :
                if (Position.x < pos1.x)
                    _velocity.x += speed;
                else if (Position.x > pos1.x)
                    _velocity.x += -speed;
                if (Position.y < pos1.y)
                    _velocity.y += speed;
                else if (Position.y > pos1.y)
                    _velocity.y += -speed;
                
                // Check if position 1 is reached and update next position
                if (approx(Position, pos1) && maxPos > 1)
                {
                    nextPos = 2;
                    _velocity = new Vector2(0, 0);  // Stop movement
                }
                break;

            // Move towards position 2
            case 2 :
                if (Position.x < pos2.x)
                    _velocity.x += speed;
                else if (Position.x > pos2.x)
                    _velocity.x += -speed;
                if (Position.y < pos2.y)
                    _velocity.y += speed;
                else if (Position.y > pos2.y)
                    _velocity.y += -speed;
                
                // Check if position 2 is reached and update next position
                if (approx(Position, pos2))
                {
                    if (maxPos > 2)
                        nextPos = 3;
                    else
                        nextPos = 1;
                    _velocity = new Vector2(0, 0);  // Stop movement
                }
                break;

            // Move towards position 3
            case 3 :
                if (Position.x < pos3.x)
                    _velocity.x += speed;
                else if (Position.x > pos3.x)
                    _velocity.x += -speed;
                if (Position.y < pos3.y)
                    _velocity.y += speed;
                else if (Position.y > pos3.y)
                    _velocity.y += -speed;
                
                // Check if position 3 is reached and update next position
                if (approx(Position, pos3))
                {
                    if (maxPos > 3)
                        nextPos = 4;
                    else
                        nextPos = 1;
                    _velocity = new Vector2(0, 0);  // Stop movement
                }
                break;

            // Move towards position 4
            case 4 :
                if (Position.x < pos4.x)
                    _velocity.x += speed;
                else if (Position.x > pos4.x)
                    _velocity.x += -speed;
                if (Position.y < pos4.y)
                    _velocity.y += speed;
                else if (Position.y > pos4.y)
                    _velocity.y += -speed;
                
                // Check if position 4 is reached and update next position
                if (approx(Position, pos4))
                {
                    nextPos = 1;
                    _velocity = new Vector2(0, 0);  // Stop movement
                }
                break;
        }
        
        // Clamp velocity to prevent excessive speed
        if (_velocity.x > 15)
            _velocity.x = 15;
        else if (_velocity.x < -15)
            _velocity.x = -15;
        if (_velocity.y > 15)
            _velocity.y = 15;
        else if (_velocity.y < -15)
            _velocity.y = -15;
    }

    // Method to approximate if two vectors are close to each other
    protected bool approx(Vector2 vec1, Vector2 vec2)
    {
        return Math.Abs(vec1.DistanceTo(vec2)) < 1.5f;
    }

    // Method to update animation based on movement
    protected virtual void AnimationUpdate() 
    {
        // Update animation only if moving and multiple positions are registered
        if (maxPos != 1 && _velocity != new Vector2(0, 0))
        {
            // Determine orientation based on velocity direction
            if (Math.Abs(_velocity.x) > Math.Abs(_velocity.y))
            {
                if (_velocity.x < 0)
                    _orientation = "Left";
                else
                    _orientation = "Right";
            }
            else
            {
                if (_velocity.y < 0)
                    _orientation = "Up";
                else
                    _orientation = "Down";
            }
            PlayAnim("Walk");   // Play walking animation
        }
        else 
            PlayAnim("Idle");   // Play idle animation if not moving
    }
    
    // Method to play animations
    protected virtual void PlayAnim(string Anim)
    {
        if (Anim != "Idle")
        {
            // Play animation based on current orientation
            switch (_orientation)
            {
                case "Right":
                    _animatedSprite.Play(Anim + "Right");
                    break;
                case "Left":
                    _animatedSprite.Play(Anim + "Left");
                    break;
                case "Up":
                    _animatedSprite.Play(Anim + "Up");
                    break;
                case "Down":
                    _animatedSprite.Play(Anim + "Down");
                    break;
            }
        }
        else
            _animatedSprite.Play(Anim);   // Play idle animation
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!activated)
            return;   // If not activated, do nothing
        
        movment();            // Perform movement calculations
        AnimationUpdate();    // Update animation based on movement
        _velocity = MoveAndSlide(_velocity);   // Move the PNJ based on calculated velocity
    }
}

