using Godot;
using System;

public class Hurtbox : Area2D {
	public static readonly PackedScene HitEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/HitEffect.tscn");

	[Export]
	public bool ShowHit = true;

	private void _OnHurtboxAreaEntered(object area) {
		if (!ShowHit) {
			return;
		}

		var hitEffect = Hurtbox.HitEffectScene.Instance() as Node2D;
		hitEffect.GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.AddChild(hitEffect);
	}
}
