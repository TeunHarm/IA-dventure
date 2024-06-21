using Godot;
using System;


public class Werewolf : EnemyBase {
    protected int BaseSpeed = 30;   // Base movement speed of the werewolf
    protected bool transformed;    // Flag indicating if the werewolf is transformed
    protected bool transforming;   // Flag indicating if the transformation is in progress
    protected bool dead;           // Flag indicating if the werewolf is dead
    protected int count;           // Counter used during transformation
    protected int death;           // Counter used for death animation

    public override void _Ready() {
        base._Ready();  // Call base class _Ready method
        
        PlayAnim("HumanIdle");  // Play initial animation
        Speed = 0;              // Set initial speed to 0
        acceleration = 15;      // Set acceleration value

        count = 190;            // Initialize transformation countdown
        death = 200;            // Initialize death animation countdown

        transforming = false;   // Set transformation flag to false
        transformed = false;    // Set transformed flag to false
        dead = false;           // Set dead flag to false
        Hp = 2;                 // Set initial health points
    }

    protected override void Enemy_action() {
        if (transformed == false && player != null) {
            transforming = true;    // Start transformation if not already transformed
            PlayAnim("Transformation"); // Play transformation animation
        }

        if (transforming == true) {
            count--;    // Countdown during transformation
            if (count == 0) {
                transforming = false;    // End transformation process
                count = 190;             // Reset transformation countdown
                transformed = true;      // Set as transformed
            }
        }

        if (_animatedSprite.Animation == "Transformation")
            Speed = 0;   // Set speed to 0 during transformation animation
        else {
            Speed = BaseSpeed;  // Set speed to base speed when not transforming
        }

        base.Enemy_action();  // Call base class Enemy_action method
    }

    protected override void PlayAnim(string Anim) {
        if (dead) {
            if (death > 0) {
                death--;    // Countdown during death animation
            }
            else {
                _animatedSprite.Stop(); // Stop animation after death animation completes
            }
        }

        if (Anim == "Transformation") {
            _currentAnim = "Transformation";
            _animatedSprite.Play(Anim); // Play transformation animation
        }
        else if (transformed == false) {
            if (Anim == "Attack") {
                _animatedSprite.Play("Wolf" + Anim);  // Play attack animation as wolf
            }
            else {
                _animatedSprite.Play("Human" + Anim); // Play human form animation
                _currentAnim = "Human" + Anim;
            }
        }
        else {
            _animatedSprite.Play("Wolf" + Anim);  // Play wolf form animation
            _currentAnim = "Wolf" + Anim;
        }

        _animatedSprite.FlipH = _orientation == "Left";  // Flip sprite horizontally based on orientation
    }

    public override void Enemy_Death() {
        base.Enemy_Death();  // Call base class Enemy_Death method
        
        dead = true;  // Set dead flag to true
        _animatedSprite.Play("Wolf" + "Death");  // Play death animation
        _animatedSprite.FlipH = _orientation == "Left";  // Flip sprite horizontally based on orientation
    }
}

