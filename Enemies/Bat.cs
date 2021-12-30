using Godot;

public class Bat : KinematicBody2D {
	public static readonly PackedScene DeathEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/EnemyDeathEffect.tscn");
	private Stats _Stats;
	private Vector2 _Velocity = Vector2.Zero;

	public float Friction {
		get => 200;
	}

	private void _OnHurtboxAreaEntered(object area) {
		_Stats.TakeDamage((area as Hitbox)?.Damage ?? 0);
		var knockbackDirection = (area as SwordHitbox)?.KnockbackDirection ?? Vector2.Zero;
		_Velocity = knockbackDirection * 150;
	}

	private void _OnStatsDie() {
		Die();
	}

	public override void _PhysicsProcess(float delta) {
		_Velocity = _Velocity.MoveToward(Vector2.Zero, Friction * delta);
		var collision = MoveAndCollide(_Velocity * delta);
		if (collision != null) {
			_Velocity = _Velocity.Bounce(collision.Normal) * .5f;
		}
	}

	public override void _Ready() {
		GetNode<AnimatedSprite>("AnimatedSprite").Play();
		_Stats = GetNode<Stats>("Stats");
	}

	private void Die() {
		var deathEffect = Bat.DeathEffectScene.Instance<Node2D>();
		deathEffect.Transform = Transform;
		GetParent().AddChild(deathEffect);
		QueueFree();
	}
}
