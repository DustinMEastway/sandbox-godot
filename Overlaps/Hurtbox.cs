using Godot;
using System;

public class Hurtbox : Area2D {
	public static readonly PackedScene HitEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/HitEffect.tscn");

	[Signal]
	public delegate void InvincibleChange(bool invincible);
	private CollisionShape2D _CollisionShape;
	private bool _Invincible = false;
	private Timer _Timer;

	public bool Invincible {
		get => _Invincible;
		set {
			_Invincible = value;
			_CollisionShape.SetDeferred("disabled", _Invincible);
			EmitSignal(nameof(InvincibleChange), _Invincible);
		}
	}

	[Export]
	public bool ShowHit = true;

	public void BecomeInvincible(float invincibleDurationInSeconds) {
		Invincible = true;
		_Timer.Start(invincibleDurationInSeconds);
	}

	private void _OnHurtboxAreaEntered(object area) {
		if (!ShowHit) {
			return;
		}

		var hitEffect = Hurtbox.HitEffectScene.Instance() as Node2D;
		hitEffect.GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.AddChild(hitEffect);
	}

	private void _OnTimerTimeout() {
		Invincible = false;
	}

	public override void _Ready() {
		_CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_Timer = GetNode<Timer>("Timer");
	}
}
