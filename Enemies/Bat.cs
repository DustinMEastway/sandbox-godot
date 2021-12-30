using Godot;

public enum BatState {
	Chase,
	Idle,
	Wander
}

public class Bat : KinematicBody2D {
	public static readonly PackedScene DeathEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/EnemyDeathEffect.tscn");

	[Export]
	public float Acceleration = 500;
	[Export]
	public float Friction = 200;
	[Export]
	public float MaxSpeed = 50;
	private AnimatedSprite _AnimatedSprite;
	private PlayerDectionZone _PlayerDectionZone;
	private BatState _State = BatState.Chase;
	private Stats _Stats;
	private Vector2 _Velocity = Vector2.Zero;

	private void _OnHurtboxAreaEntered(object area) {
		_Stats.TakeDamage((area as Hitbox)?.Damage ?? 0);
		var knockbackDirection = (area as SwordHitbox)?.KnockbackDirection ?? Vector2.Zero;
		_Velocity = knockbackDirection * 150;
	}

	private void _OnPlayerDectionZonePlayerDetected(Player player) {
		_State = BatState.Chase;
	}


	private void _OnPlayerDectionZonePlayerLost(Player player) {
		_State = BatState.Idle;
	}

	private void _OnStatsDie() {
		Die();
	}

	public override void _PhysicsProcess(float delta) {
		switch (_State) {
			case BatState.Chase:
				StateChase(delta);
				break;
			case BatState.Idle:
				StateIdle(delta);
				break;
			case BatState.Wander:
				StateWander(delta);
				break;
		}
	}

	public override void _Ready() {
		_AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_AnimatedSprite.Play();
		_PlayerDectionZone = GetNode<PlayerDectionZone>("PlayerDectionZone");
		_Stats = GetNode<Stats>("Stats");
	}

	private void Die() {
		var deathEffect = Bat.DeathEffectScene.Instance<Node2D>();
		deathEffect.Transform = Transform;
		GetParent().AddChild(deathEffect);
		QueueFree();
	}

	private void StateChase(float delta) {
		var destination = _PlayerDectionZone.DetectedPlayer?.GlobalPosition ?? GlobalPosition;
		var moveDirection = Position.DirectionTo(destination).Normalized();
		_Velocity = _Velocity.MoveToward(moveDirection * MaxSpeed, Acceleration * delta);
		_AnimatedSprite.FlipH = _Velocity.x < 0;
		MoveAndSlide(_Velocity);
	}

	private void StateIdle(float delta) {
		_Velocity = _Velocity.MoveToward(Vector2.Zero, Friction * delta);
		var collision = MoveAndCollide(_Velocity * delta);
		if (collision != null) {
			_Velocity = _Velocity.Bounce(collision.Normal) * .5f;
		}
	}

	private void StateWander(float delta) {
	}
}
