using Godot;
using System;

public class Grass : Node2D {
	public static readonly PackedScene DeathEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/GrassEffect.tscn");

	private void _OnHurtboxAreaEntered(object area) {
		Die();
	}

	private void Die() {
		var deathEffect = Grass.DeathEffectScene.Instance<Node2D>();
		deathEffect.Transform = Transform;
		GetParent().AddChild(deathEffect);
		QueueFree();
	}
}
