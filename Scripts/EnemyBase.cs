using Godot;
using System;


public class EnemyBase : KinematicBody2D {
	[Export] public int Speed;  // Movement speed of the enemy.
    [Export] public float acceleration;  // Acceleration value for enemy movement.

    // References to player instances and detection flags.
    protected Player player;
    protected Player playerAttacked;
    protected bool moving;

    protected Vector2 _velocity;  // Velocity vector for movement.

    protected AnimatedSprite _animatedSprite;  // Reference to the AnimatedSprite node.
    protected string _currentAnim;  // Current animation state.

    protected string _orientation;  // Current orientation (direction) of the enemy.
    protected bool _attack;  // Flag indicating if the enemy is attacking.

    protected int damage;  // Damage inflicted by the enemy.
    protected int Hp;  // Current health points of the enemy.
    protected float lastHit;  // Time since last attack.
    protected bool dead;  // Flag indicating if the enemy is dead.

    protected Vector2 spawnCoordonates;  // Spawn coordinates of the enemy.
    protected int maxHP;  // Maximum health points of the enemy.

    // Flags indicating player detection in different directions.
    protected bool player_detected_Down;
    protected bool player_detected_Up;
    protected bool player_detected_Right;
    protected bool player_detected_Left;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _orientation = "Down";
        PlayAnim("Idle");  // Play the 'Idle' animation.

        // Initialize movement parameters.
        Speed = 40;
        acceleration = 50;
        
        // Initialize combat parameters.
        damage = 1;
        lastHit = 0;
        Hp = 2;
        dead = false;
        
        // Initialize spawn information.
        spawnCoordonates = GlobalPosition;
        maxHP = Hp;
        
        // Initialize player detection flags.
        player_detected_Down = false;
        player_detected_Up = false;
        player_detected_Right = false;
        player_detected_Left = false;
        
        // Initialize player references.
        player = null;
        playerAttacked = null;
        
        moving = false;
        _attack = false;
    }

    // Called when a body enters the detection area.
    public virtual void _on_Detection_area_body_entered(KinematicBody2D body) {
        if (body is Player body1) {
            _velocity = new Vector2(0, 0);  // Stop enemy movement.
            player = body1;  // Set the player reference.
        }
    }

    // Called when a body exits the detection area.
    public void _on_Detection_area_body_exited(KinematicBody2D body) {
        if (body is Player)
            player = null;  // Clear the player reference.
    }

    // Method to get the current health points (HP) of the enemy.
    public int Enemy_GetHP()
    {
        return Hp;
    }

    // Methods called when the player enters or exits attack collision areas (Down, Up, Right, Left).
    public void _on_AttackDown_body_entered(KinematicBody2D body) {
        if (body is Player body1) {
            player_detected_Down = true;
            playerAttacked = body1;
        }
    }

    public void _on_AttackDown_body_exited(KinematicBody2D body) {
        if (body is Player) {
            player_detected_Down = false;
            playerAttacked = null;
        }
    }

    public void _on_AttackUp_body_entered(KinematicBody2D body) {
        if (body is Player body1) {
            player_detected_Up = true;
            playerAttacked = body1;
        }
    }

    public void _on_AttackUp_body_exited(KinematicBody2D body) {
        if (body is Player) {
            player_detected_Up = false;
            playerAttacked = null;
        }
    }

    public void _on_AttackRight_body_entered(KinematicBody2D body) {
        if (body is Player body1) {
            player_detected_Right = true;
            playerAttacked = body1;
        }
    }

    public void _on_AttackRight_body_exited(KinematicBody2D body) {
        if (body is Player) {
            player_detected_Right = false;
            playerAttacked = null;
        }
    }

    public void _on_AttackLeft_body_entered(KinematicBody2D body) {
        if (body is Player body1) {
            player_detected_Left = true;
            playerAttacked = body1;
        }
    }

    public void _on_AttackLeft_body_exited(KinematicBody2D body) {
        if (body is Player) {
            player_detected_Left = false;
            playerAttacked = null;
        }
    }



	    protected virtual void Enemy_action() {
        // If player is null or dead, stop moving and attacking.
        if (player == null || player.dead) {
            moving = false;
            _velocity = new Vector2(0, 0);
            _attack = false;
        }
        // If player is detected in any direction, initiate attack.
        else if (player_detected_Down || player_detected_Up || player_detected_Right || player_detected_Left) {
            Attack();
            _attack = true;
        }
        // Otherwise, move towards the player's position.
        else {
            _velocity += (player.GlobalPosition - GlobalPosition) / acceleration;

            // Limit velocity to maximum speed.
            if (_velocity.x > Speed)
                _velocity.x = Speed;
            else if (_velocity.x < -Speed)
                _velocity.x = -Speed;

            if (_velocity.y > Speed)
                _velocity.y = Speed;
            else if (_velocity.y < -Speed)
                _velocity.y = -Speed;

            // Determine orientation towards the player.
            double angle = (180f / Math.PI) * GlobalPosition.AngleToPoint(player.GlobalPosition) + 45f;
            if (angle < -180) angle = 180 + (angle + 180);
            if (angle > 180) angle = -180 + (angle - 180);
            angle /= 90f;

            if (angle > 1) {
                _orientation = "Up";
            } else if (angle > 0) {
                _orientation = "Left";
            } else if (angle > -1) {
                _orientation = "Down";
            } else {
                _orientation = "Right";
            }

            moving = true;
            _attack = false;
        }
    }

    // Method to inflict damage on the enemy.
    public virtual void Enemy_Damage(int InDamage) {
        // Ensure enemy can take damage and is not dead.
        if (lastHit >= 0.5 && !dead) {
            Hp -= InDamage;
            if (Hp > 0) {
                PlayAnim("Hurt");
            } else {
                Enemy_Death();  // Enemy dies if health drops to zero.
            }
            lastHit = 0;  // Reset hit timer.
        }
    }

    // Method to respawn the enemy.
    public virtual void Enemy_Respawn() {
        _animatedSprite.Play("Idle");
        _orientation = "Down";

        dead = false;  // Revive the enemy.
        GlobalPosition = spawnCoordonates;  // Move enemy to spawn position.
        Hp = maxHP;  // Reset health points.
    }

    // Method to handle enemy death.
    public virtual void Enemy_Death() {
        if (dead) return;  // Do nothing if already dead.
        dead = true;  // Mark enemy as dead.
        Hp = 0;  // Set health points to zero.

        PlayAnim("Death");  // Play death animation.

        // Disable collision for all child CollisionShape2D nodes.
        foreach (Node2D Child in GetChildren()) {
            if (Child is CollisionShape2D Coll)
                Coll.SetDeferred("disabled", true);
        }

        // Randomly spawn a heart object on death.
        if (new Random().Next(0, 2) == 1) {
            string path = "res://Objets/Heart.tscn";
            PackedScene packedScene = GD.Load<PackedScene>(path);
            Heart heart = packedScene.Instance<Heart>();
            heart.GlobalPosition = GlobalPosition + new Vector2(0, -15);
            GetParent().AddChild(heart);
        }
    }

    // Method to handle the enemy's attack action.
    protected virtual void Attack() {
        // If the attack animation frame allows and player is valid, damage the player.
        if (_animatedSprite.Frame == 1 && (_currentAnim == "Attack" || _currentAnim == "WolfAttack")) {
            if (playerAttacked != null)
                playerAttacked.Player_Damage(damage);
        }
    }



    protected virtual void AnimationUpdate() {
        // Play attack animation if attacking.
        if (_attack)
            PlayAnim("Attack");
        // Play walk animation if moving.
        else if (moving) {
            PlayAnim("Walk");
            
            // Increase animation speed if enemy is moving fast.
            _animatedSprite.SpeedScale = Speed > 100 ? 2 : 1;
        }
        // Play idle animation if not attacking or moving.
        else {
            PlayAnim("Idle");
            
            // Reset animation speed.
            _animatedSprite.SpeedScale = 1;
        }
    }

    // Method to set the current animation.
    protected virtual void PlayAnim(string Anim) {
        _currentAnim = Anim;
    }

    public override void _Process(float delta) {
        // If dead and not currently playing death animation, play death animation.
        if (dead && _currentAnim != "Death")
            PlayAnim("Death");

        // If not dead and cooldown time has passed, perform enemy actions and movement.
        if (!dead && lastHit > 0.5) {
            Enemy_action();
            _velocity = MoveAndSlide(_velocity);
        }

        // If not dead and cooldown time has passed, update animation.
        if (!dead && lastHit > 0.5)
            AnimationUpdate();

        // Increment last hit timer scaled by engine timescale.
        lastHit += GetProcessDeltaTime() * Engine.TimeScale;
    }

}
