using Godot;
using System;

public class Grass : Node2D {
	public static readonly PackedScene GrassEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/GrassEffect.tscn");

	/// <inheritdoc />
	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("attack")) {
			var grassEffect = Grass.GrassEffectScene.Instance<Node2D>();
			grassEffect.Transform = Transform;
			GetParent().AddChild(grassEffect);
			QueueFree();
		}
	}
}
