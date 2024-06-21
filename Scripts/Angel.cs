using Godot;
using System;


public class Angel : EnemyBase
{
    // Variables declaration
    private string musicPath = "Audio/BossBattle.mp3";
    private bool WaveSpawned = false;
    private bool IsPlayerEnd = true;
    private float deadtime = 0;
    private string sceneToLoadpath;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialization
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _orientation = "Down";
        PlayAnim("Idle");

        Speed = 70;
        acceleration = 18;

        damage = 2;
        lastHit = 0;
        Hp = 5;
        dead = false;

        spawnCoordonates = GlobalPosition;
        maxHP = Hp;

        player_detected_Down = false;
        player_detected_Up = false;
        player_detected_Right = false;
        player_detected_Left = false;

        player = null;
        playerAttacked = null;

        moving = false;
        _attack = false;
    }

    // Called when the detection area detects a body entering
    public override void _on_Detection_area_body_entered(KinematicBody2D body)
    {
        base._on_Detection_area_body_entered(body);
        
        // Check if the body is an AI
        if (body is AI)
        {
            IsPlayerEnd = false;
        }
        // Check if the body is a Player
        else if (body is Player body1)
        {
            player = body1;
            player.trigger_music(musicPath);
            IsPlayerEnd = true;
        }
    }

    // Called when the attack down detector detects a body exiting
    public void _on_AttackDownDetector_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player)
        {
            player_detected_Down = false;
            playerAttacked = null;
        }
    }

    // Called when the attack right detector detects a body entering
    public void _on_AttackRightDetector_body_entered(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Right = true;
            playerAttacked = body1;
        }
    }

    // Called when the attack right detector detects a body exiting
    public void _on_AttackRightDetector_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player)
        {
            player_detected_Right = false;
            playerAttacked = null;
        }
    }

    // Called when the attack left detector detects a body entering
    public void _on_AttackLeftDetector_body_entered(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Left = true;
            playerAttacked = body1;
        }
    }

    // Called when the attack left detector detects a body exiting
    public void _on_AttackLeftDetector_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player)
        {
            player_detected_Left = false;
            playerAttacked = null;
        }
    }

    // Called when the attack down body enters the detection area
    private void _on_AttackDown_body_entered(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Down = true;
        }
    }

    // Called when the attack down body exits the detection area
    private void _on_AttackDown_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Down = false;
        }
    }

    // Called when the attack up body enters the detection area
    private void _on_AttackUp_body_entered(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Up = true;
        }
    }

    // Called when the attack up body exits the detection area
    private void _on_AttackUp_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Up = false;
        }
    }

    // Called when the attack left body enters the detection area
    private void _on_AttackLeft_body_entered(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Left = true;
        }
    }

    // Called when the attack left body exits the detection area
    private void _on_AttackLeft_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Left = false;
        }
    }

    // Called when the attack right body enters the detection area
    private void _on_AttackRight_body_entered(KinematicBody2D body)
    {
        // Check if the body is a Player
        if (body is Player body1)
        {
            player_detected_Right = true;
        }
    }
	
	 private void _on_AttackRight_body_exited(KinematicBody2D body)
    {
        // Check if the body is a Player and update detection flag
        if (body is Player body1)
        {
            player_detected_Right = false;
        }
    }

    // Method to spawn an AngelWave instance based on player detection direction
    private void spawnWave()
    {
        string path = "res://Objets/AngelWave.tscn";
        PackedScene packedScene = GD.Load<PackedScene>(path);
        AngelWave arrow = packedScene.Instance<AngelWave>();
        
        // Position the wave according to player detection direction
        if (player_detected_Left)
        {
            arrow.Position = Position + new Vector2(-35, 0);
            arrow._orientation = "Left";
        }
        else if (player_detected_Up)
        {
            arrow.Position = Position + new Vector2(0, -35);
            arrow._orientation = "Up";
        }
        else if (player_detected_Down)
        {
            arrow.Position = Position + new Vector2(0, 35);
            arrow._orientation = "Down";
        }
        else if (player_detected_Right)
        {
            arrow.Position = Position + new Vector2(35, 0);
            arrow._orientation = "Right";
        }

        GetParent().AddChild(arrow);  // Add the AngelWave to the scene
    }

    // Method to handle damage received by the enemy
    public override void Enemy_Damage(int InDamage)
    {
        // Check if enough time has passed since last hit and enemy is not dead
        if (lastHit >= 0.5 && !dead)
        {
            if (Hp > 0)
            {
                Hp -= InDamage;  // Decrease enemy HP

                // Calculate direction from player and apply recoil
                Vector2 playerPos = player.GlobalPosition;
                Vector2 direction = (GlobalPosition - playerPos).Normalized();
                float recoilDistance = 40f;
                GlobalPosition += direction * recoilDistance;

                PlayAnim("Hurt");  // Play hurt animation
            }
            else
            {
                Enemy_Death();  // Enemy dies if HP drops to zero
            }
            lastHit = 0;  // Reset last hit timer
        }
    }

    // Method to handle attack logic
    protected override void Attack()
    {
        // Check if the enemy is in attack animation and specific frames
        if (_currentAnim == "Attack" &&
            (_animatedSprite.Frame == 3 || _animatedSprite.Frame == 4 || _animatedSprite.Frame == 5))
        {
            if (playerAttacked != null)
                playerAttacked.Player_Damage(damage);  // Damage player if in range

            if (_animatedSprite.Frame == 3 && !WaveSpawned)
            {
                spawnWave();  // Spawn wave on specific frame
                WaveSpawned = true;
            }
            else if (_animatedSprite.Frame != 3)
            {
                WaveSpawned = false;
            }
        }
    }

    // Method to update animation state
    protected override void AnimationUpdate()
    {
        if (_attack)
        {
            PlayAnim("Attack");  // Play attack animation
        }
        else if (moving)
        {
            PlayAnim("Walk");  // Play walk animation
        }
        else
        {
            PlayAnim("Idle");  // Play idle animation

            // Reset animation speed
            _animatedSprite.SpeedScale = 1;
        }
    }

    // Method to play animations
    protected override void PlayAnim(string Anim)
    {
        base.PlayAnim(Anim);  // Call base class method
        
        // Handle Death animation
        if (Anim == "Death")
        {
            _animatedSprite.Play("Die");  // Play Die animation
            return;
        }

        // Handle Hurt animation
        if (Anim == "Hurt")
        {
            switch (_orientation)
            {
                case "Right":
                    _animatedSprite.Play("Hit" + "Down");
                    break;
                case "Left":
                    _animatedSprite.Play("Hit" + "Down");
                    break;
                case "Up":
                    _animatedSprite.Play("Hit" + "Up");
                    break;
                case "Down":
                    _animatedSprite.Play("Hit" + "Down");
                    break;
            }
            return;
        }

        // Play other animations based on orientation
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

    // Process function to handle scene change when dead
    public override void _Process(float delta)
    {
        base._Process(delta);  // Call base class method

        // Check if enemy is dead
        if (dead)
        {
            if (deadtime > 2f)
            {
                // Change scene based on player status
                if (IsPlayerEnd)
                    sceneToLoadpath = "res://scenes/WinScene.tscn";
                else
                    sceneToLoadpath = "res://scenes/LoseScene.tscn";

                GetTree().ChangeScene(sceneToLoadpath);  // Change the scene
                QueueFree();  // Free the enemy node
            }
            else
            {
                deadtime += GetProcessDeltaTime() * Engine.TimeScale;  // Increment dead time
            }
        }
    }
}


