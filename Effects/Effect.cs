using Godot;
using System;

public class Effect : AnimatedSprite {
	/// <inheritdoc />
	public override void _Ready() {
		Play("Animate");
		Connect("animation_finished", this, "_OnAnimatedSpriteAnimationFinished");
	}

	private void _OnAnimatedSpriteAnimationFinished() {
		QueueFree();
	}
}
