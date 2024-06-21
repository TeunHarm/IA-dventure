using Godot;
using System;


public class Skeleton_Warrior : EnemyBase {

	protected override void PlayAnim(string Anim) {
		base.PlayAnim(Anim);  // Call base class PlayAnim method

		// Determine which animation to play based on orientation
		switch (_orientation) {
			case "Right":
				_animatedSprite.Play(Anim + "Right");  // Play right-facing animation
				break;
			case "Left":
				_animatedSprite.Play(Anim + "Right"); // Play left-facing animation
				break;
			case "Up":
				_animatedSprite.Play(Anim + "Up");  // Play up-facing animation
				break;
			case "Down":
				_animatedSprite.Play(Anim + "Down");  // Play down-facing animation
				break;
		}
		_animatedSprite.FlipH = _orientation == "Left"; // Flip sprite horizontally if facing left
	}
}
