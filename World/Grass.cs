using Godot;
using System;

public class Grass : Node2D {
	public static readonly PackedScene GrassEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/GrassEffect.tscn");

	private void _OnHurtboxAreaEntered(object area) {
		Die();
	}

	private void Die() {
		var grassEffect = Grass.GrassEffectScene.Instance<Node2D>();
		grassEffect.Transform = Transform;
		GetParent().AddChild(grassEffect);
		QueueFree();
	}
}
