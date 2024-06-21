using Godot;
using System;


public class Player : KinematicBody2D
{
	public Camera CameraRef;

	[Export] public int Speed;

	// NEAT
	public bool IsTraining = false;

	// This is basically the current objective, can be changed at anytime
	// This is used by the AI
	[Export] public int StartPOI = 10;
	public Node2D CurrentPOI = null;
	public ObjectiveDisplay ObjectiveDisplay = null;
	protected Dialogue DialogueBox;

	// Objective conditions
	public bool HasKey = false;

	protected int maxHP;
	protected int HP;
	public bool dead;

	private Control deathMenu;
	private Control pauseMenu;

	[Export] public string currentLevel = "Start village";
	public AStar PathFinding = null;
	public ObjectiveNav Nav = null;

	private Vector2 spawnCoordonates;
	private string ennemyToRespawn;

	protected Vector2 _velocity;

	protected AnimatedSprite _animatedSprite;

	protected CollisionShape2D _attackDown;
	protected CollisionShape2D _attackUp;
	protected CollisionShape2D _attackRight;
	protected CollisionShape2D _attackLeft;

	protected AudioStreamPlayer mousic;

	protected AudioStreamPlayer AmbiantSoundPlayer;

	protected AudioStreamPlayer aieuh;

	protected string _orientation;
	protected string _currentAnim;
	protected bool _attack;
	protected int attackStat;

	protected bool pause;


	public float lasteleport = 3;
	public float lastHit;

	public float deathCounter = 0;



	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_animatedSprite.Play("IdleDown");
		_orientation = "Down";

		_attackDown = GetNode<CollisionShape2D>("AttackDown/CollisionShapeADown");
		_attackUp = GetNode<CollisionShape2D>("AttackUp/CollisionShapeAUp");
		_attackRight = GetNode<CollisionShape2D>("AttackRight/CollisionShapeARight");
		_attackLeft = GetNode<CollisionShape2D>("AttackLeft/CollisionShapeALeft");

		if (!(this is AI))
		{
			mousic = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
			AmbiantSoundPlayer = GetNode<AudioStreamPlayer>("AmbiantSoundPlayer");
			aieuh = GetNode<AudioStreamPlayer>("Enemy");
		}

		_attackDown.Disabled = true;
		_attackUp.Disabled = true;
		_attackRight.Disabled = true;
		_attackLeft.Disabled = true;

		spawnCoordonates = Position;
		ennemyToRespawn = null;

		dead = false;
		_attack = false;
		pause = false;

		if (!IsTraining)
		{
			deathMenu = GetTree().Root.GetNode<Control>("Jeu/Base/DeathMenu");
			pauseMenu = GetTree().Root.GetNode<Control>("Jeu/Base/PauseMenu");
		}

		attackStat = 1;

		lasteleport = 3;
		lastHit = 0;
		maxHP = 15; //Valeur arbitraire
		HP = maxHP;

		/*Nav = new ObjectiveNav();
		GetParent().AddChild(Nav);*/

		if (!IsTraining)
		{
			if (this is AI)
			{
				DialogueBox = GetTree().CurrentScene.GetNode<Dialogue>("Base/Split/ViewportContainer2/Dialogue");
				DialogueBox.isAiDialogueBox = true;
			}
			else
				DialogueBox = GetTree().CurrentScene.GetNode<Dialogue>("Base/Split/ViewportContainer/Dialogue");
		}

		foreach (Objective Obj in GetTree().GetNodesInGroup("Objectives"))
		{
			if (Obj.Id == StartPOI)
			{
				CurrentPOI = Obj;
				break;
			}
		}
	}


	public void PlayAmbiantSound(string path)
	{
		if (this is AI)
			return;
		AudioStream audioStream = GD.Load<AudioStream>(path);
		if (AmbiantSoundPlayer != null)
		{
			AmbiantSoundPlayer.Stream = audioStream;
			AmbiantSoundPlayer.Play();
		}
		else
		{
			GD.Print("Impossible de charger le fichier audio à partir du chemin spécifié.");
		}
	}


	public void _on_Player_area_entered(Area2D area)
	{
	    // Check if the player entered a door and has waited long enough to teleport
	    if (area is Door door && lasteleport >= 4)
	    {
	        Player_Change_Scene(door);
	        lasteleport = 0;
	    }
	    // Check if the player entered a sign and it has a dialogue path
	    else if (area is sign sign && sign.dialoguePath != null)
	    {
	        Play_Dialogue(sign.dialoguePath);
	    }
	}

	public void trigger_music(string path)
	{
	    // AI cannot trigger music
	    if (this is AI)
	        return;
	    
	    // Load the audio stream from the given path
	    AudioStream audioStream = GD.Load<AudioStream>(path);

	    // If the audio stream is valid, play the music
	    if (audioStream != null)
	    {
	        mousic.Stream = audioStream;
	        mousic.Play();
	    }
	    // If the audio stream is invalid, print an error message
	    else
	    {
	        GD.Print("Impossible de charger le fichier audio à partir du chemin spécifié.");
	    }
	}

	public void _on_Area2D_area_exited(Area2D area)
	{
	    // Check if the player exited a sign area and it has a dialogue path
	    if (area is sign sign && sign.dialoguePath != null)
	    {
	        // End the dialogue if the dialogue box is not null
	        if (DialogueBox != null)
	            DialogueBox.End();
	    }
	}

	protected virtual void Player_Change_Scene(Door door)
	{
	    // Do nothing if the player is dead
	    if (dead) return;

	    // If the door is only for the player and this is an AI, do nothing
	    if (door.IsOnlyForPlayer && this is AI)
	        return;

	    // Reset the player's velocity
	    _velocity = new Vector2(0, 0);

	    // Find the appropriate master node for teleportation based on whether the player is an AI or in training
	    Node Master = null;
	    if (this is AI)
	    {
	        Master = IsTraining ? door.FindParent("Master") : door.FindParent("AiMaster");
	    }
	    else
	    {
	        Master = door.FindParent("Master");
	    }

	    // If the master node is not found, print an error and return
	    if (Master == null)
	    {
	        GD.Print("Door: Failed to find master scene for TP.");
	        return;
	    }

	    // Teleport the player to the destination node
	    GlobalPosition = Master.GetNode<Node2D>(door.destination).GlobalPosition;

	    // Update camera limits if CameraRef is not null
	    if (CameraRef != null)
	    {
	        CameraRef.OnPlayerTeleportChangeLimit(
	            Master.GetNode<Node2D>(door.Cameralimit1).GetPath(),
	            Master.GetNode<Node2D>(door.Cameralimit2).GetPath()
	        );
	    }

	    // Update the spawn coordinates if not in training mode
	    if (!IsTraining)
	    {
	        spawnCoordonates = Master.GetNode<Node2D>(door.destination).GlobalPosition;
	    }

	    // Check if the door has an objective and trigger it
	    foreach (Node Child in door.GetChildren())
	    {
	        if (Child is Objective objective)
	        {
	            objective.Entered(this);
	            break;
	        }
	    }

	    // Respawn enemies in the designated group
	    if (door.GroupToRespawn != null)
	    {
	        var enemies = GetTree().GetNodesInGroup(door.GroupToRespawn);
	        ennemyToRespawn = door.GroupToRespawn;
	        for (int i = 0; i < enemies.Count; i++)
	        {
	            EnemyBase enemy = (EnemyBase)enemies[i];
	            if (enemy != null)
	                enemy.Enemy_Respawn();
	        }
	    }

	    // Activate NPCs in the designated group
	    if (door.PnjGroupToActivate != null)
	    {
	        var Pnj = GetTree().GetNodesInGroup(door.PnjGroupToActivate);
	        for (int i = 0; i < Pnj.Count; i++)
	        {
	            PnjBase pnj = (PnjBase)Pnj[i];
	            if (pnj != null)
	                pnj.activated = true;
		    }
		}

	    // Deactivate NPCs in the designated group
	    if (door.PnjGroupToDeactivate != null)
	    {
	        var Pnj = GetTree().GetNodesInGroup(door.PnjGroupToDeactivate);
	        for (int i = 0; i < Pnj.Count; i++)
	        {
	            PnjBase pnj = (PnjBase)Pnj[i];
	            if (pnj != null)
	                pnj.activated = false;
		    }
		}
	}

	// Play the dialogue specified by the dialogue path
	protected virtual void Play_Dialogue(string dialogue_path)
	{
	    // If the dialogue box is not null, set the dialogue file and start the dialogue
	    if (DialogueBox == null) return;
	    DialogueBox.d_file = dialogue_path;
	    DialogueBox.Start();
	}


	protected virtual void _on_Attack_body_entered(KinematicBody body)
	{
	    // Check if the collided body is an enemy
	    Node current = body;
	    if (current is EnemyBase enemy)
	    {
	        // Play damage sound if enemy is hurt but not dead
	        if (AmbiantSoundPlayer != null && enemy.Enemy_GetHP() - attackStat > 0)
	        {
	            RandomNumberGenerator rng = new RandomNumberGenerator();
	            rng.Randomize();
	            int randomInt = rng.RandiRange(1, 4);
	            AmbiantSoundPlayer.Stream =
	                GD.Load<AudioStream>("res://Assets/SoundFX/EnemyDamage" + randomInt + ".wav");
	            AmbiantSoundPlayer.Play();
	        }
	        // Play death sound if enemy is killed
	        else if (AmbiantSoundPlayer != null && enemy.Enemy_GetHP() - attackStat <= 0)
	        {
	            AmbiantSoundPlayer.Stream = GD.Load<AudioStream>("res://Assets/SoundFX/EnemyDeath.wav");
	            AmbiantSoundPlayer.Play();
	        }

	        // Apply damage to the enemy
	        enemy.Enemy_Damage(attackStat);
	    }
	}

	// Delegate method to handle collision with enemies from different attack directions
	protected virtual void _on_AttackDown_body_entered(KinematicBody body)
	{
	    _on_Attack_body_entered(body);
	}

	protected virtual void _on_AttackUp_body_entered(KinematicBody body)
	{
	    _on_Attack_body_entered(body);
	}

	protected virtual void _on_AttackRight_body_entered(KinematicBody body)
	{
	    _on_Attack_body_entered(body);
	}

	protected virtual void _on_AttackLeft_body_entered(KinematicBody body)
	{
	    _on_Attack_body_entered(body);
	}

	// Get the current HP of the player
	public virtual int Player_getHP()
	{
	    return HP;
	}

	// Apply damage to the player
	public virtual void Player_Damage(int damage)
	{
	    // If sufficient time has passed since the last hit
	    if (lastHit >= 1)
	    {
	        PlayAmbiantSound("PlayerDamage");
	        HP -= damage;

	        // Handle player death
	        if (HP <= 0)
	        {
	            PlayAmbiantSound("Quit");
	            Player_Death();
	        }
	        else
	        {
	            PlayAmbiantSound("PlayerDamage");
	        }

	        // Reset hit timer
	        lastHit = 0;
	    }
	}

	// Handle player death
	public virtual void Player_Death()
	{
	    // Return if player is already dead
	    if (dead)
	        return;
	    
	    dead = true;
	    _animatedSprite.Play("Death");

	    // Respawn player if not an AI and death counter is sufficient
	    if (!(this is AI) && deathCounter >= 3)
	    {
	        Player_Respawn();
	    }
	}

	
	public virtual void Player_Respawn()
	{
	    // Check if the player is not an AI
	    if (!(this is AI))
	    {
	        // If there is a death menu, make it visible
	        if (deathMenu != null)
	            deathMenu.Visible = true;
	        // Reset the death counter
	        deathCounter = 0;
	    }

	    // Get all nodes in the group specified by "ennemyToRespawn"
	    var enemies = GetTree().GetNodesInGroup(ennemyToRespawn);

	    // Loop through each enemy and respawn them
	    for (int i = 0; i < enemies.Count; i++)
	    {
	        GD.Print("Respawn");
	        EnemyBase enemy = (EnemyBase)enemies[i];
	        if (enemy != null)
	            enemy.Enemy_Respawn();
	    }

	    // Reset player's state
	    _animatedSprite.Play("IdleDown");
	    dead = false;
	    GlobalPosition = spawnCoordonates;
	    HP = maxHP;
	}

	public virtual void Player_Pause()
	{
	    // Make the pause menu visible and set the player's animation to idle
	    pauseMenu.Visible = true;
	    _animatedSprite.Play("IdleDown");
	}

	public void Player_Heal(int heal)
	{
	    // Increase player's HP by the specified amount
	    HP += heal;
	    // Cap the HP to the maximum value
	    if (HP > maxHP)
	        HP = maxHP;
	}

	protected virtual void GetInput()
	{
	    _velocity = new Vector2();
	    // Set player's speed
	    Speed = 200;

	    // Update player's velocity and orientation based on input
	    bool isMoving = false;
	    if (Input.IsActionPressed("right"))
	    {
	        _velocity.x += 1;
	        _orientation = "Right";
	        isMoving = true;
	    }
	    else if (Input.IsActionPressed("left"))
	    {
	        _velocity.x -= 1;
	        _orientation = "Left";
	        isMoving = true;
	    }
	    else if (Input.IsActionPressed("down"))
	    {
	        _velocity.y += 1;
	        _orientation = "Down";
	        isMoving = true;
	    }
	    else if (Input.IsActionPressed("up"))
	    {
	        _velocity.y -= 1;
	        _orientation = "Up";
	        isMoving = true;
	    }
	    // Pause the game if the "Stop" action is pressed
	    else if (Input.IsActionPressed("Stop"))
	    {
	        pause = true;
	        Player_Pause();
	    }

	    // Check if the player is attacking and not moving
	    _attack = (bool)(Input.IsActionPressed("attack") && !isMoving);
	    if (_attack)
	        Attack();
	    else
	    {
	        // Disable all attack hitboxes if not attacking
	        _attackDown.Disabled = true;
	        _attackUp.Disabled = true;
	        _attackRight.Disabled = true;
	        _attackLeft.Disabled = true;
	    }

	    // Normalize the velocity vector and scale by speed
	    _velocity = _velocity.Normalized() * Speed;
	}


	protected virtual void Attack()
	{
		// If the current frame of the animated sprite is 1
		if (_animatedSprite.Frame == 1)
		{
			// Enable attack hitbox based on the player's orientation
			switch (_orientation)
			{
				case "Right":
					_attackRight.Disabled = false;
					break;
				case "Left":
					_attackLeft.Disabled = false;
					break;
				case "Up":
					_attackUp.Disabled = false;
					break;
				case "Down":
					_attackDown.Disabled = false;
					break;
			}
		}
		else
		{
			// Disable all attack hitboxes if the frame is not 1
			_attackDown.Disabled = true;
			_attackUp.Disabled = true;
			_attackRight.Disabled = true;
			_attackLeft.Disabled = true;
		}
	}

	protected virtual void AnimationUpdate()
	{
		// Return early if the player is in training mode
		if (IsTraining) return;

		// If the player is moving
		if (_velocity != new Vector2(0, 0))
		{
			PlayAnim("Walk");

			// Increase animation speed if the player is running
			_animatedSprite.SpeedScale = Speed > 100 ? 2 : 1;
		}
		// If the player is attacking
		else if (_attack)
			PlayAnim("Attack");
		else
		{
			// Default to idle animation
			_animatedSprite.Play("IdleDown");
			_animatedSprite.SpeedScale = 1;
		}
	}

	protected virtual void PlayAnim(string Anim)
	{
		// Set the current animation
		_currentAnim = Anim;

		// Stop animation if in training mode
		if (IsTraining)
		{
			_animatedSprite.Stop();
			return;
		}

		// Play the appropriate animation based on orientation
		switch (_orientation)
		{
			case "Right":
				_animatedSprite.Play(Anim + "Right");
				break;
			case "Left":
				_animatedSprite.Play(Anim + "Right");
				break;
			case "Up":
				_animatedSprite.Play(Anim + "Up");
				break;
			case "Down":
				_animatedSprite.Play(Anim + "Down");
				break;
		}

		// Flip the sprite horizontally if facing left
		_animatedSprite.FlipH = _orientation == "Left";
	}

	public override void _Process(float delta)
	{
		// Quit the game if the quit action is pressed
		if (Input.IsActionPressed("quit"))
		{
			GetTree().Quit();
			return;
		}

		// Resume the game if the pause menu is hidden
		if (pauseMenu != null && pauseMenu.Visible == false)
			pause = false;
		// Update the game state if not paused
		if (pause == false)
		{
			if (!dead)
			{
				GetInput();
				_velocity = MoveAndSlide(_velocity);
				AnimationUpdate();
			}

			// Update timers
			lastHit += GetProcessDeltaTime() * Engine.TimeScale;
			lasteleport += GetProcessDeltaTime() * Engine.TimeScale;
			// Handle player respawn if dead and not an AI
			if (dead && !(this is AI))
			{
				if (deathCounter > 3)
					Player_Respawn();
				else
					deathCounter += GetProcessDeltaTime() * Engine.TimeScale;
			}
		}
	}
}

