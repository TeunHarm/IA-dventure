using Godot;
using System;

public class AngelWave : KinematicBody2D
{
    // Public variable to hold the orientation of the object
    public string _orientation;

    // Private variables to handle movement and animation
    private Vector2 _velocity;
    private AnimatedSprite _animatedSprite;
    private float Speed;

    // Collision shapes for various directions
    protected CollisionShape2D _bodyV;
    protected CollisionShape2D _areaDown;
    protected CollisionShape2D _areaUp;
    protected CollisionShape2D _areaUp2;
    protected CollisionShape2D _areaRight;
    protected CollisionShape2D _areaLeft;
    protected CollisionShape2D _areaLeft2;

    // Method called when the node is added to the scene
    public override void _Ready() {
        // Initialize references to child nodes and disable unnecessary collision shapes
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _bodyV = GetNode<CollisionShape2D>("CollisionShape2D");
        _areaDown = GetNode<CollisionShape2D>("Area2D/CollisionShape2DDown");
        _areaUp = GetNode<CollisionShape2D>("Area2D/CollisionShape2DUp");
        _areaUp2 = GetNode<CollisionShape2D>("Area2D/CollisionShape2DUp2");
        _areaRight = GetNode<CollisionShape2D>("Area2D/CollisionShape2DRight");
        _areaLeft = GetNode<CollisionShape2D>("Area2D/CollisionShape2DLeft");
        _areaLeft2 = GetNode<CollisionShape2D>("Area2D/CollisionShape2DLeft2");

        // Initially disable all collision shapes except the main body
        _bodyV.Disabled = true;
        _areaDown.Disabled = true;
        _areaUp.Disabled = true;
        _areaUp2.Disabled = true;
        _areaRight.Disabled = true;
        _areaLeft.Disabled = true;
        _areaLeft2.Disabled = true;

        // Set initial speed
        Speed = 110;
    }

    // Method called every frame
    public override void _Process(float delta) {
        // Handle movement and animation based on orientation
        switch (_orientation) {
            case "Right":
                _velocity.x = Speed;
                _velocity.y = 0;
                _animatedSprite.Play("Right");
                updateDownRight();
                break;
            
            case "Down":
                _animatedSprite.Play("Down");
                _velocity.y = Speed;
                _velocity.x = 0;
                updateDownRight();
                break;

            case "Left":
                _velocity.x = -Speed;
                _velocity.y = 0;
                _animatedSprite.Play("Left");
                updateLeft();
                break;

            case "Up":
                _animatedSprite.Play("Up");
                _velocity.y = -Speed;
                _velocity.x = 0;
                updateUp();
                break;
        }

        // Move the object according to its velocity and slide along collisions
        _velocity = MoveAndSlide(_velocity);
    }

    // Method to update collision shapes for movement to the left
    private void updateLeft() {
        _bodyV.Disabled = true;

        // Enable the correct collision shape based on the animation frame
        if (_animatedSprite.Frame == 0) {
            _areaLeft.Disabled = false;
            _areaLeft2.Disabled = true;
        }
        else if (_animatedSprite.Frame == 1) {
            _areaLeft.Disabled = true;
            _areaLeft2.Disabled = false;
        }
        else {
            QueueFree(); // Free the object if animation is complete
        }
    }

    // Method to update collision shapes for movement upwards
    private void updateUp() {
        _bodyV.Disabled = false;

        // Enable the correct collision shape based on the animation frame
        if (_animatedSprite.Frame == 0) {
            _areaUp.Disabled = false;
            _areaUp2.Disabled = true;
        }
        else if (_animatedSprite.Frame == 1) {
            _areaUp.Disabled = true;
            _areaUp2.Disabled = false;
        }
        else {
            QueueFree(); // Free the object if animation is complete
        }
    }

    // Method to update collision shapes for movement to the right and downwards
    private void updateDownRight() {
        _bodyV.Disabled = false;

        // Enable the correct collision shape based on the animation frame
        if (_animatedSprite.Frame == 0) {
            _areaDown.Disabled = false;
            _areaRight.Disabled = true;
        }
        else if (_animatedSprite.Frame == 1) {
            _areaDown.Disabled = true;
            _areaRight.Disabled = false;
        }
        else {
            QueueFree(); // Free the object if animation is complete
        }
    }

    // Method called when the object enters another body's collision shape
    private void _on_Arrow_body_entered(KinematicBody body) {
        Node current = body;

        // Check if the colliding body is the player
        if (current is Player player) {
            player.Player_Damage(1); // Damage the player
            QueueFree(); // Destroy the arrow
        }
        else if (current is EnemyBase enemy) {
            enemy.Enemy_Damage(1); // Damage the enemy
            QueueFree(); // Destroy the arrow
        }
    }
}

