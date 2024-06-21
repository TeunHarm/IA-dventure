using Godot;
using System;


// Define a custom Arrow class inheriting from KinematicBody2D.
public class Arrow : KinematicBody2D {

    // Public variable to hold orientation of the arrow.
    public string _orientation;
    
    // Private variable to manage velocity of the arrow.
    private Vector2 _velocity;
    
    // Private variable for managing the animated sprite of the arrow.
    private AnimatedSprite _animatedSprite;
    
    // Private variables to control arrow speed and stop timing.
    private float Speed;
    private float stoptimer;
    
    // Protected variables to reference collision shapes for arrow's movement areas.
    protected CollisionShape2D _bodyV;
    protected CollisionShape2D _bodyH;
    protected CollisionShape2D _areaDown;
    protected CollisionShape2D _areaUp;
    protected CollisionShape2D _areaRight;
    protected CollisionShape2D _areaLeft;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // Initialize references to child nodes and disable collision shapes initially.
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        
        // Initialize collision shape references.
        _bodyH = GetNode<CollisionShape2D>("CollisionShape2D");
        _bodyV = GetNode<CollisionShape2D>("CollisionShape2D2");
        _areaDown = GetNode<CollisionShape2D>("Area2D/CollisionShape2DDown");
        _areaUp = GetNode<CollisionShape2D>("Area2D/CollisionShape2DUp");
        _areaRight = GetNode<CollisionShape2D>("Area2D/CollisionShape2DRight");
        _areaLeft = GetNode<CollisionShape2D>("Area2D/CollisionShape2DLeft");
        
        // Disable collision shapes initially.
        _bodyH.Disabled = true;
        _bodyV.Disabled = true;
        _areaDown.Disabled = true;
        _areaUp.Disabled = true;
        _areaRight.Disabled = true;
        _areaLeft.Disabled = true;

        // Set initial values for speed and stop timer.
        Speed = 70;
        stoptimer = 0;
    }
    
    // Handles collision when the arrow body enters another body.
    private void _on_Arrow_body_entered(KinematicBody body) {
        // Check if collided with Player or AI.
        if (body.Filename == "res://Characters/Player.tscn" || body.Filename == "res://Characters/AI.tscn") {
            // If collided with Player or AI, damage them and queue arrow for deletion.
            Node current = body;
            if (current is Player player) {
                player.Player_Damage(1);
                QueueFree();  // Queue the arrow for deletion from the scene.
            }
        }
        else {
            // If collided with any other body (assumed to be Enemy), damage them and queue arrow for deletion.
            Node current = body;
            if (current is EnemyBase enemy) {
                enemy.Enemy_Damage(1);
                QueueFree();  // Queue the arrow for deletion from the scene.
            }
        }
    }
    
    // Called every frame. Handles movement and orientation of the arrow.
    public override void _Process(float delta) {
        // Check the current orientation and adjust velocity and animations accordingly.
        switch (_orientation) {
            case "Right":
                _velocity.x = Speed;  // Move to the right with set speed.
                _velocity.y = 0;
                _animatedSprite.Play("Right");  // Play right-facing animation.

                // Enable horizontal collision shape and disable others for right movement.
                _bodyH.Disabled = false;
                _bodyV.Disabled = true;
                _areaDown.Disabled = true;
                _areaUp.Disabled = true;
                _areaRight.Disabled = false;
                _areaLeft.Disabled = true;

                break;

            case "Left":
                _velocity.x = -Speed;  // Move to the left with set speed.
                _velocity.y = 0;
                _animatedSprite.Play("Left");  // Play left-facing animation.

                // Enable horizontal collision shape and disable others for left movement.
                _bodyH.Disabled = false;
                _bodyV.Disabled = true;
                _areaDown.Disabled = true;
                _areaUp.Disabled = true;
                _areaRight.Disabled = true;
                _areaLeft.Disabled = false;

                break;

            case "Up":
                _animatedSprite.Play("Up");  // Play upward animation.
                _velocity.y = -Speed;  // Move upwards with set speed.
                _velocity.x = 0;

                // Enable vertical collision shape and disable others for upward movement.
                _bodyH.Disabled = true;
                _bodyV.Disabled = false;
                _areaDown.Disabled = true;
                _areaUp.Disabled = false;
                _areaRight.Disabled = true;
                _areaLeft.Disabled = true;

                break;

            case "Down":
                _animatedSprite.Play("Down");  // Play downward animation.
                _velocity.y = Speed;  // Move downwards with set speed.
                _velocity.x = 0;

                // Enable vertical collision shape and disable others for downward movement.
                _bodyH.Disabled = true;
                _bodyV.Disabled = false;
                _areaDown.Disabled = false;
                _areaUp.Disabled = true;
                _areaRight.Disabled = true;
                _areaLeft.Disabled = true;

                break;
        }

        // Move the arrow based on its velocity using the built-in MoveAndSlide function.
        _velocity = MoveAndSlide(_velocity);

        // If the arrow has stopped moving (velocity is zero), increment stop timer.
        if (_velocity == new Vector2(0, 0))
            stoptimer += GetProcessDeltaTime();

        // If the arrow has been stationary for more than 2 seconds, queue it for deletion.
        if (stoptimer > 2)
            QueueFree();  // Queue the arrow for deletion from the scene.
    }
}