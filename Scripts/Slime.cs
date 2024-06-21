using Godot;
using System;


public class Slime : EnemyBase {
	// Base speed of the slime
	protected int BaseSpeed = 30;

	// Method called when the node is added to the scene
	public override void _Ready() {
		base._Ready(); // Call the base class _Ready method
        
		// Initialize slime-specific properties
		Speed = 0; // Initial speed is 0
		acceleration = 15; // Acceleration of the slime

		Hp = 1; // Health points of the slime
	}

	// Method defining the actions of the enemy
	protected override void Enemy_action() {
		// Change speed based on the current frame of the animation
		if (_animatedSprite.Frame > 1 && _animatedSprite.Frame < 5)
		{
			Speed = BaseSpeed; // Set speed to base speed if frame is between 2 and 4
		}
		else
		{
			Speed = 0; // Set speed to 0 otherwise
		}
        
		base.Enemy_action(); // Call the base class Enemy_action method
	}

	// Method to play animations
	protected override void PlayAnim(string Anim) {
		base.PlayAnim(Anim); // Call the base class PlayAnim method
        
		_animatedSprite.Play(Anim); // Play the specified animation
		_animatedSprite.FlipH = _orientation == "Left"; // Flip the sprite horizontally if orientation is left
	}

	// Method called when the enemy dies
	public override void Enemy_Death() {
		base.Enemy_Death(); // Call the base class Enemy_Death method
        
		_animatedSprite.Play("Death"); // Play the death animation
		_animatedSprite.FlipH = _orientation == "Left"; // Flip the sprite horizontally if orientation is left
	}
}

