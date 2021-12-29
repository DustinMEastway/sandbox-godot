using Godot;
using System;

public class DieEffect : Node2D {
	/// <inheritdoc />
	public override void _Ready() {
		GetNode<AnimatedSprite>("AnimatedSprite").Play("Die");
	}

	private void _OnAnimatedSpriteAnimationFinished() {
		QueueFree();
	}
}
