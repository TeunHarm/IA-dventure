using Godot;
using System;


public class Cow : KinematicBody2D {
    // Exported variable to define the initial speed of the cow.
    [Export] public int Speed = 10;
    
    // Public variables for velocity and movement control parameters.
    public Vector2 velocity;
    public float deltamov = 5;
    public float deltaa = 5;
    public int dir = 4;
    
    // Reference to the AnimatedSprite node for animation control.
    private AnimatedSprite _animatedSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // Initialize _animatedSprite to control animations.
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("Idle");  // Play the 'Idle' animation initially.
    }

    // Method to handle cow's movement logic.
    public void GetInput() {
        velocity = new Vector2(0, 0);
        
        // Increment deltaa with elapsed time.
        deltaa += GetProcessDeltaTime();
        
        // Check if deltaa has reached deltamov to decide next movement.
        if (deltaa >= deltamov) {
            deltaa = 0;  // Reset deltaa.
            Random rnd = new Random();

            // Generate a random number for direction and movement parameters.
            int num = rnd.Next(0, 4);  // Random number between 0 and 3 (inclusive).
            deltamov = rnd.Next(1, 10);  // Random number between 1 and 9 (inclusive).
            Speed = rnd.Next(5, 20);  // Random number between 5 and 19 (inclusive).

            dir = num;  // Set the direction.

            // Adjust sprite flipping based on movement direction.
            if (num == 1 && !_animatedSprite.FlipH) {
                _animatedSprite.FlipH = true;  // Flip sprite horizontally.
            }
            else if (num == 0 && _animatedSprite.FlipH) {
                _animatedSprite.FlipH = false;  // Unflip sprite horizontally.
            }
        }

        // Set velocity based on the selected direction.
        switch (dir) {
            case 0:
                velocity.x += 1;  // Move right.
                break;

            case 1:
                velocity.x -= 1;  // Move left.
                break;

            case 2:
                velocity.y += 1;  // Move down.
                break;

            case 3:
                velocity.y -= 1;  // Move up.
                break;

            default:
                velocity.x = 0;
                velocity.y = 0;
                break;
        }
        
        // Normalize velocity vector and scale it by Speed.
        velocity = velocity.Normalized() * Speed;
        
        // Play 'Moving' animation if velocity is non-zero, otherwise play 'Idle'.
        if (velocity.x != 0 || velocity.y != 0)
            _animatedSprite.Play("Moving");
        else 
            _animatedSprite.Play("Idle");
    }

    // Called every physics frame. Handles movement using MoveAndSlide.
    public override void _PhysicsProcess(float delta) {
        GetInput();  // Update movement inputs.
        velocity = MoveAndSlide(velocity);  // Move the cow based on velocity and slide on collisions.
    }
}
