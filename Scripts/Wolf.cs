using Godot;
using System;

public class Wolf : EnemyBase
{
	public override void _Ready()
	{
		base._Ready();  // Call base _Ready method in EnemyBase
		Speed = 60;     // Set the speed of the wolf
		acceleration = 10;  // Set the acceleration
	}

	protected override void PlayAnim(string Anim)
	{
		base.PlayAnim(Anim);  // Call base PlayAnim method in EnemyBase

		// Handle specific animation logic for the wolf
		if (Anim == "Idle")
		{
			// Stop the animated sprite and reset to the first frame when idle
			_animatedSprite.Stop();
			_animatedSprite.Frame = 0;
			return;
		}

		// Play animations based on orientation
		switch (_orientation)
		{
			case "Right":
				_animatedSprite.Play(Anim + "Side");
				break;
			case "Left":
				_animatedSprite.Play(Anim + "Side");
				break;
			case "Up":
				_animatedSprite.Play(Anim + "Up");
				break;
			case "Down":
				_animatedSprite.Play(Anim + "Down");
				break;
		}

		// Flip the sprite horizontally based on orientation
		_animatedSprite.FlipH = _orientation == "Right";
	}
}
