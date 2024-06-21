using Godot;
using System;


public class Skeleton_Archer : EnemyBase {
    private float attackCooldown;   // Cooldown between attacks
    private bool attacked;          // Flag to track if attack has been executed

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        base._Ready();  // Call base class _Ready method
        
        Speed = 30;             // Set movement speed
        acceleration = 40;      // Set acceleration
        
        attackCooldown = 5;     // Initial attack cooldown
        attacked = false;       // Flag indicating if the enemy has attacked
    }

    // Called when a body enters the detection area.
    public new void _on_Detection_area_body_entered(KinematicBody2D body) {
        // Check if the detected body is the Player or AI
        if (body.Filename == "res://Characters/Player.tscn" || body.Filename == "res://Characters/AI.tscn") {
            Node current = body;
            if (current is Player player2) {
                player = player2;   // Assign player reference if detected
            }
        }
    }

    // Perform actions specific to this enemy type.
    protected override void Enemy_action() {
        // If player is dead, stop moving and attacking
        if (player != null && player.dead) {
            moving = false;
            _velocity = new Vector2(0,0);
            _attack = false;
        }
        // If player is detected or attack cooldown is low, initiate attack
        else if (player_detected_Down || player_detected_Up || player_detected_Right || player_detected_Left || attackCooldown < 0.51) {
            Attack();
            _attack = true;
        }
        // If player is detected, move towards player's position
        else if (player != null) {
            // Adjust velocity based on player's position relative to enemy
            if (player.GlobalPosition.x < GlobalPosition.x &&  player.GlobalPosition.x - GlobalPosition.x > player.GlobalPosition.y - GlobalPosition.y ) {
                _velocity.x += ((player.GlobalPosition.x + 70) - GlobalPosition.x) / acceleration;
                _velocity.y += (player.GlobalPosition.y - GlobalPosition.y) / acceleration;
            }
            else if (player.GlobalPosition.y < GlobalPosition.y && player.GlobalPosition.y - GlobalPosition.y > player.GlobalPosition.x - GlobalPosition.x) {
                _velocity.y += ((player.GlobalPosition.y + 70) - GlobalPosition.y) / acceleration;
                _velocity.x += (player.GlobalPosition.x - GlobalPosition.x) / acceleration;
            }
            else if (player.GlobalPosition.x > GlobalPosition.x &&  player.GlobalPosition.x - GlobalPosition.x > player.GlobalPosition.y - GlobalPosition.y ) {
                _velocity.x += ((player.GlobalPosition.x - 70) - GlobalPosition.x) / acceleration;
                _velocity.y += (player.GlobalPosition.y - GlobalPosition.y ) / acceleration;
            }
            else if (player.GlobalPosition.y > GlobalPosition.y && player.GlobalPosition.y - GlobalPosition.y > player.GlobalPosition.x - GlobalPosition.x) {
                _velocity.y += ((player.GlobalPosition.y - 70) - GlobalPosition.y) / acceleration;
                _velocity.x += (player.GlobalPosition.x - GlobalPosition.x) / acceleration;
            }
            else {
                _velocity.y += ((player.GlobalPosition.y - 70) - GlobalPosition.y) / acceleration;
                _velocity.x += (player.GlobalPosition.x - GlobalPosition.x) / acceleration;
            }

            // Limit velocity to maximum speed
            if (_velocity.x > Speed)
                _velocity.x = Speed;
            else if (_velocity.x < -Speed)
                _velocity.x = -Speed;

            if (_velocity.y > Speed)
                _velocity.y = Speed;
            else if (_velocity.y < -Speed)
                _velocity.y = -Speed;

            // Determine orientation based on movement direction
            if ((player.GlobalPosition.y - GlobalPosition.y) < (player.GlobalPosition.x - GlobalPosition.x) &&
                (player.GlobalPosition.y - GlobalPosition.y) < 0)
                _orientation = "Up";
            else if ((player.GlobalPosition.y - GlobalPosition.y) < (player.GlobalPosition.x - GlobalPosition.x))
                _orientation = "Down";
            else if (((player.GlobalPosition.x - GlobalPosition.x) < 0))
                _orientation = "Left";
            else
                _orientation = "Right";

            moving = true;  // Set moving flag to true
            _attack = false;  // Reset attack flag
        }
        else {
            _velocity = new Vector2(0, 0);  // Stop moving if no player detected
            moving = false;
        }
    }

    // Execute attack logic.
    protected override void Attack() {
        // If attack cooldown allows, perform attack
        if (attackCooldown > 2) {
            attackCooldown = 0;  // Reset attack cooldown
            PlayAnim("Attack");  // Play attack animation
            attacked = false;    // Reset attack flag
        }
        // If attack cooldown is low and not attacked yet, launch an arrow
        else if (attackCooldown > 0.5 && !attacked) {
            string path = "res://Objets/Arrow.tscn";  // Path to the arrow scene
            PackedScene packedScene = GD.Load<PackedScene>(path);  // Load arrow scene
            Arrow arrow = packedScene.Instance<Arrow>();  // Instance the arrow

            // Position the arrow based on detected direction
            if (player_detected_Left) {
                arrow.Position = Position + new Vector2(-10, 0);
                arrow._orientation = "Left";
            }
            else if (player_detected_Up) {
                arrow.Position = Position + new Vector2(0, -10);
                arrow._orientation = "Up";
            }
            else if (player_detected_Down) {
                arrow.Position = Position + new Vector2(0, 10);
                arrow._orientation = "Down";
            }
            else if (player_detected_Right) {
                arrow.Position = Position + new Vector2(10, 0);
                arrow._orientation = "Right";
            }

            GetParent().AddChild(arrow);  // Add arrow to the scene
            attacked = true;  // Set attacked flag to true
        }
    }

    // Called when the enemy dies.
    public override void Enemy_Death() {
        base.Enemy_Death();  // Call base class Enemy_Death method
        GD.Print("***Bruit d'os qui tombent par terre***");  // Print death sound message
    }

    // Update animation based on movement and orientation.
    private new void AnimationUpdate() {
        if (moving) {
            // Update walking animation
            switch (_orientation) {
                case "Right":
                    _animatedSprite.Play("WalkRight");
                    break;
                case "Left":
                    _animatedSprite.Play("WalkRight");  // Assuming there's no specific "WalkLeft" animation
                    break;
                case "Up":
                    _animatedSprite.Play("WalkUp");
                    break;
                case "Down":
                    _animatedSprite.Play("WalkDown");
                    break;
            }

            // Increase animation speed if enemy is moving fast
            _animatedSprite.SpeedScale = Speed > 100 ? 2 : 1;
        }
        else {
            // Update idle animation
            switch (_orientation) {
                case "Right":
                    _animatedSprite.Play("IdleRight");
                    break;
                case "Left":
                    _animatedSprite.Play("IdleRight");  // Assuming there's no specific "IdleLeft" animation
                    break;
                case "Up":
                    _animatedSprite.Play("IdleUp");
                    break;
                case "Down":
                    _animatedSprite.Play("IdleDown");
                    break;
            }

            _animatedSprite.SpeedScale = 1;  // Reset animation speed scale
        }

        _animatedSprite.FlipH = _orientation == "Left";  // Flip sprite horizontally if facing left
    }

    // Play animation with orientation.
    protected override void PlayAnim(string Anim) {
        base.PlayAnim(Anim);  // Call base class PlayAnim method

        // Play animation based on orientation
        switch (_orientation) {
            case "Right":
                _animatedSprite.Play(Anim + "Right");
                break;
            case "Left":
                _animatedSprite.Play(Anim + "Right");  // Assuming there's no specific "AnimLeft" animation
                break;
            case "Up":
                _animatedSprite.Play(Anim + "Up");
                break;
            case "Down":
                _animatedSprite.Play(Anim + "Down");
                break;
        }

        _animatedSprite.FlipH = _orientation == "Left";  // Flip sprite horizontally if facing left
    }

    // Process function called every frame.
    public override void _Process(float delta) {
        if (attackCooldown < 0.7)
            Attack();  // Check and perform attack if cooldown is low

        // If not dead and sufficient time has passed since last hit and cooldown is high enough, perform enemy actions
        if (!dead && lastHit > 0.5 && attackCooldown > 0.7) {
            Enemy_action();  // Perform enemy action
            _velocity = MoveAndSlide(_velocity);  // Move using MoveAndSlide method
        }

        // Update animation if not dead, sufficient time has passed since last hit, and cooldown is high enough
        if (!dead && lastHit > 0.5  && attackCooldown > 0.7)
			AnimationUpdate();

		lastHit += GetProcessDeltaTime();
		attackCooldown += GetProcessDeltaTime();
	}
}
