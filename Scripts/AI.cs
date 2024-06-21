using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;


public class AI : Player
{
    // Animated sprites for different attack directions.
    private AnimatedSprite _swordDown;
    private AnimatedSprite _swordUp;
    private AnimatedSprite _swordLeft;
    private AnimatedSprite _swordRight;

    // Starting position of the player.
    private Vector2 PlayerStart;

    // Array of RayCast2D nodes used for detection.
    private Array<RayCast2D> _raycasts = new Array<RayCast2D>();

    // Variables for distance and position tracking.
    private Vector2 startPos;
    private Vector2 LastDist;
    private Vector2 LastPos;
    private float CheckTime;

    // Objective-related variables.
    private int LastObj = 10;
    private int ObjectivesGot = 0;

    // Debug flag and debug text label.
    private readonly bool NeatDebug = true;
    private RichTextLabel DebugText = null;

    // Navigation agent and distance variables.
    private NavigationAgent2D NavAgent = null;
    private float StartDist = 1f;

    // Line2D node for debug visualization.
    private Line2D line;

    // Distance variables.
    private float totalDist;
    private float wallDeath = 0;

    // NEAT variables.
    private bool UseNeat = false;
    private String NeatToLoad = "2024-5-14/best(3)";
    private Godot.Object NeatRunner = null;

    // Arrays for NEAT inputs and outputs.
    public Array<float> LastIn = new Array<float>();
    public Array<float> LastOut = new Array<float>();

    // Override method for handling player scene changes through doors.
    protected override void Player_Change_Scene(Door door)
    {
        if (door.POIDependance == ((Objective)CurrentPOI).Id)
            return;
        base.Player_Change_Scene(door);
    }

    // Method to get the current NEAT runner object.
    public Godot.Object GetNeat()
    {
        if (UseNeat)
            return NeatRunner;
        return null;
    }

    // Method to retrieve all RayCast2D nodes.
    public Array<RayCast2D> GetRayscasts()
    {
        return _raycasts;
    }

    // Method called when a body enters the Area2D node.
    private void _on_Area2D_body_entered(KinematicBody body)
    {
        Node current = body;
        if (current is EnemyBase enemy)
            enemy.Enemy_Death();
    }

    // Initialization method called when the node is ready.
    public override void _Ready() {
        // Initialize animated sprites and play their idle animations.
        _swordDown = GetNode<AnimatedSprite>("AnimatedSprite/AttackDown");
        _swordDown.Play("Idle");
        
        _swordUp = GetNode<AnimatedSprite>("AnimatedSprite/AttackUp");
        _swordUp.Play("Idle");
        
        _swordLeft = GetNode<AnimatedSprite>("AnimatedSprite/AttackLeft");
        _swordLeft.Play("Idle");
        
        _swordRight = GetNode<AnimatedSprite>("AnimatedSprite/AttackRight");
        _swordRight.Play("Idle");
        
        // Call base _Ready method.
        base._Ready();
        
        // Initialize total distance.
        totalDist = 0;
        
        // Play initial animation.
        _animatedSprite.Play("AttackDown");

        // Set speed.
        Speed = 120;

        // Enable RayCast2D nodes and add them to the array.
        foreach (Node child in GetChildren())
        {
            if (child is RayCast2D ray)
            {
                _raycasts.Add(ray);
                ray.Enabled = true;
            }
        }

        // Initialize player start and position variables.
        PlayerStart = GlobalPosition;
        startPos = GlobalPosition;
        LastPos = GlobalPosition;
        LastObj = ((Objective)CurrentPOI).Id;

        // Get debug text label and set visibility.
        DebugText = GetNode<RichTextLabel>("TestText");
        DebugText.Visible = IsTraining && NeatDebug;

        // Get navigation agent and set initial target location.
        NavAgent = GetNode<NavigationAgent2D>("NavAgent");
        NavAgent.SetTargetLocation(CurrentPOI.GlobalPosition);
        StartDist = NavAgent.DistanceToTarget();

        // Add a debug line if in training mode and debug is enabled.
        if (IsTraining && NeatDebug)
        {
            line = new Line2D();
            line.Width = 1f;
            GetParent().AddChild(line);
        }

        // Disable NEAT functionality if not in training mode.
        if (IsTraining)
            UseNeat = false;
        
        // Load and initialize NEAT script if UseNeat flag is true.
        if (UseNeat)
        {
            GDScript script = GD.Load<GDScript>("res://NEAT_usability/standalone_scripts/standalone_neuralnet.gd");
            NeatRunner = (Godot.Object)script.New();
            NeatRunner.Call("load_config", NeatToLoad);
        }
    }

    // Constructor to add a custom user signal for "death".
    public AI()
    {
        AddUserSignal("death");
    }
	


    protected override void GetInput()
    {
        if (dead) return;

        // Update player's velocity and orientation based on input or AI control.
        if (_velocity.x > 0)
        {
            _orientation = "Right";
        }
        else if (_velocity.x < 0)
        {
            _orientation = "Left";
        }
        else if (_velocity.y > 0)
        {
            _orientation = "Down";
        }
        else if (_velocity.y < 0)
        {
            _orientation = "Up";
        }

        // Forced movement in case AI is stuck.
        if (Input.IsActionPressed("upIA"))
            _velocity = Vector2.Up;
        else if (Input.IsActionPressed("downIA"))
            _velocity = Vector2.Down;
        else if (Input.IsActionPressed("leftIA"))
            _velocity = Vector2.Left;
        else if (Input.IsActionPressed("rightIA"))
            _velocity = Vector2.Right;
        else if (!UseNeat && !IsTraining)
            _velocity = Vector2.Zero;

        // Determine if attacking based on velocity and state.
        _attack = Vector2.Zero == _velocity && !dead;

        if (_attack)
            Attack();
        else {
            _attackDown.Disabled = true;
            _attackUp.Disabled = true;
            _attackRight.Disabled = true;
            _attackLeft.Disabled = true;
        }

        // Normalize velocity and adjust speed.
        _velocity = _velocity.Normalized() * Speed;
        MoveAndSlide(_velocity);
    }

    protected override void AnimationUpdate() {
        //base.AnimationUpdate();

        PlayAnim(_attack ? "Attack" : "Walk");

        _animatedSprite.SpeedScale = Speed > 100 ? 2 : 1;
    }

    protected override void PlayAnim(string Anim) {
        base.PlayAnim(Anim);

        AnimatedSprite[] swords = new[] { _swordDown, _swordUp, _swordLeft, _swordRight };

        if (Anim == "Attack") {
            string[] orients = new[] { "Down", "Up", "Left", "Right" };

            for (int i = 0; i < orients.Length; i++) {
                if (orients[i] == _orientation)
                    swords[i].Play("Attack" + _orientation);
                else
                    swords[i].Play("Idle");
            }
        }
        else {
            foreach (AnimatedSprite sword in swords)
                sword.Play("Idle");
        }
    }

    private int IdleCount = 0;

    public override void _Process(float delta) {
        base._Process(delta);

        NavAgent.GetNextLocation();
        NavAgent.SetVelocity(_velocity);

        if (dead) return;

        if (UseNeat)
        {
            object output = NeatRunner.Call("update", sense());
            if (output != null)
            {
                // Get the outputs
                Array<float> data = new Array<float>();
                foreach (object obj in (Godot.Collections.Array)output)
                    data.Add((float)obj);
                act(data);
            }
        }

        if (IsTraining)
            totalDist = (float)Math.Sqrt((float)Math.Pow(LastPos.x - GlobalPosition.x, 2) + (float)Math.Pow(LastPos.y - GlobalPosition.y, 2));

        // Check for NEAT-related death conditions.
        if (!IsTraining) return;
        if (lasteleport > 1) return;

        CheckTime += delta;
        if (CheckTime < 1f) return;
        CheckTime = 0f;

        if (GlobalPosition.DistanceTo(LastPos) < 2f)
            IdleCount++;
        else
            IdleCount = 0;

        if (IdleCount > 1)
        {
            Player_Death();
            return;
        }

        if (_velocity.Length() > 0.1f || GlobalPosition.DistanceTo(startPos) < 1f)
        {
            IdleCount = 0;
            if (GlobalPosition.DistanceTo(LastPos) < 20f)
            {
                Player_Death();
            }
        }
        LastPos = GlobalPosition;
    }

    public void Move(Vector2 dir) {
        if (dead) return;

        _velocity = dir;
        MoveAndSlide(_velocity);
    }

    float Dist(float Start, float End, float Curr)
    {
        float Init = Math.Abs(Start - End);
        float Dist = Math.Abs(Curr - End);
        if (Init < 1)
            return Math.Min(Dist, 1f);
        return Math.Min(Dist / Init, 1f);
    }

    public void _on_NavAgent_path_changed()
    {
        if (!IsTraining || !NeatDebug) return;

        if (dead)
        {
            line.ClearPoints();
            return;
        }

        Vector2[] local = NavAgent.GetNavPath();
        for (int i = 0; i < local.Length; i++)
            local[i] = line.ToLocal(local[i]);
        line.Points = local;
    }

    // NEAT-related method for sensing environment inputs.
    public Array<float> sense()
    {
        Array<float> Data = new Array<float>();
        Data.Resize(1 + 16); // Adjust array size as needed for inputs.

        // Check if the objective has changed.
        if (((Objective)CurrentPOI).Id > LastObj)
        {
            LastObj = ((Objective)CurrentPOI).Id;
            startPos = GlobalPosition;
            ObjectivesGot++;
            NavAgent.SetTargetLocation(CurrentPOI.GlobalPosition);
            StartDist = NavAgent.DistanceToTarget();
        }

        // Example inputs based on environment state.
        int i = 0;
        Data[i++] = Mathf.Clamp(NavAgent.DistanceToTarget() / (StartDist * 2f), 0f, 1f);

        // Example angle detection inputs.
        float angle = (Mathf.Rad2Deg(GlobalPosition.DirectionTo(NavAgent.GetNextLocation()).Angle()) + 180f);
        if (angle <= 45)
            Data[i++] = 1;
        else
            Data[i++] = 0;
        if (angle > 45 && angle <= 90)
            Data[i++] = 1;
        else
            Data[i++] = 0;
        // Repeat for other angles as needed.

        if (NeatDebug)
            DebugText.Text = Data[0].ToString("F2") + "\n" + (Data[1] * 360f).ToString("F0") + "°";

        // Example raycast detection inputs.
        foreach (RayCast2D ray in _raycasts)
        {
            if (ray.IsColliding())
            {
                float dist = ray.GetCollisionPoint().DistanceTo(ray.GlobalTransform.origin);
                Data[i++] = dist / 100f; // Normalize or adjust as necessary.
            }
            else
            {
                Data[i++] = 1f; // No collision detected.
            }
        }

        LastIn = Data;
        return Data;
    }

    // NEAT-related method for acting based on network outputs.
    public void act(Array<float> network_output)
    {
        if (network_output.Count <= 0) return;

        LastOut = network_output;

        // Example actions based on network outputs.
        if (network_output[0] > 0.5f)
        {
            _velocity = Vector2.Right;
        }
        else if (network_output[1] > 0.5f)
        {
            _velocity = Vector2.Left;
        }
        else if (network_output[2] > 0.5f)
        {
            _velocity = Vector2.Up;
        }
        else if (network_output[3] > 0.5f)
        {
            _velocity = Vector2.Down;
        }
    }

    // Method to calculate fitness score.
    public float get_fitness()
    {
        if (startPos == GlobalPosition)
            return 0;

        if (((Objective)CurrentPOI).Id > LastObj)
        {
            LastObj = ((Objective)CurrentPOI).Id;
            ObjectivesGot++;
        }

        float Dist = 1f - Mathf.Clamp(NavAgent.DistanceToTarget() / (StartDist * 1.2f), 0f, 1f);
        float Dist2 = Mathf.Clamp(totalDist, 0f, (ObjectivesGot + 1) * 2.5f);
        if (ObjectivesGot > 0)
            return Math.Max(ObjectivesGot * 10f + Dist * 5 + Dist2 - wallDeath, 0f);
        else
            return Math.Max(ObjectivesGot * 10f + Dist * 5 + Dist2 - wallDeath, 0f) / 2;
    }

    // Method to handle player death event.
    public override void Player_Death()
    {
        base.Player_Death();

        // Stop the sword animations.
        AnimatedSprite[] swords = new[] { _swordDown, _swordUp, _swordLeft, _swordRight };
        foreach (AnimatedSprite sword in swords)
            sword.Play("Idle");

        // Emit death signal.
        EmitSignal("death");

        // Clear debug line if in training mode and debug is enabled.
        if (IsTraining && NeatDebug)
            line.ClearPoints();
    }

    // Method called during a new generation to reset agent state.
    public void resetAgent()
    {
        if (GetTree() == null) return;

        // Reset player position and state variables.
        GlobalPosition = PlayerStart;
        startPos = PlayerStart;
        LastPos = PlayerStart;
        HP = 5;
        lasteleport = 3;
        ObjectivesGot = 0;
        currentLevel = "Start village";
        dead = false;

        // Find the starting objective.
        foreach (Objective Obj in GetTree().GetNodesInGroup("Objectives")) {
            if (Obj.Id == StartPOI) {
                CurrentPOI = Obj;
                break;
            }
        }
        LastObj = ((Objective)CurrentPOI).Id;
        HasKey = false;
		    
        NavAgent.SetTargetLocation(CurrentPOI.GlobalPosition);
        StartDist = NavAgent.DistanceToTarget();
        totalDist = 0;
        wallDeath = 0;
		    
        if (IsTraining && NeatDebug)
            line.ClearPoints();
    }
}

