using Godot;

public enum BatState {
	Chase,
	Idle,
	Wander
}

public class Bat : KinematicBody2D {
	public static readonly PackedScene DeathEffectScene = ResourceLoader.Load<PackedScene>("res://Effects/EnemyDeathEffect.tscn");
	public static readonly Shader BossShader = ResourceLoader.Load<Shader>("res://Shaders/Boss.gdshader");
	public static BatState[] IdleStates = new BatState[] { BatState.Idle, BatState.Wander };

	[Export]
	public float Acceleration = 500;
	[Export]
	public float Friction = 200;
	[Export]
	public bool IsBoss = false;
	[Export]
	public float MaxSpeed = 50;
	[Export]
	public float WanderRange = 35;
	private AnimatedSprite _AnimatedSprite;
	private AnimationPlayer _BlinkAnimationPlayer;
	private Hurtbox _Hurtbox;
	private Vector2 _InitialPosition;
	private PlayerDectionZone _PlayerDectionZone;
	private SoftCollision _SoftCollision;
	private BatState _State = BatState.Chase;
	private Stats _Stats;
	private Vector2 _WanderPosition;
	private Timer _WanderTimer;
	private Vector2 _Velocity = Vector2.Zero;

	private void _OnHurtboxAreaEntered(object area) {
		_Stats.TakeDamage((area as Hitbox)?.Damage ?? 0);
		var knockbackDirection = (area as SwordHitbox)?.KnockbackDirection ?? Vector2.Zero;
		_Velocity = knockbackDirection * 150;
		_Hurtbox.BecomeInvincible(0.2f);
	}

	private void _OnHurtboxInvincibleChange(bool invincible) {
		_BlinkAnimationPlayer.Play((invincible) ? "Start" : "Stop");
	}

	private void _OnPlayerDectionZonePlayerDetected(Player player) {
		_State = BatState.Chase;
		_WanderTimer.Stop();
	}

	private void _OnPlayerDectionZonePlayerLost(Player player) {
		_OnWanderTimerTimeout();
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
		_InitialPosition = GlobalPosition;
		_AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_AnimatedSprite.Play();
		_BlinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
		_Hurtbox = GetNode<Hurtbox>("Hurtbox");
		_PlayerDectionZone = GetNode<PlayerDectionZone>("PlayerDectionZone");
		_SoftCollision = GetNode<SoftCollision>("SoftCollision");
		_Stats = GetNode<Stats>("Stats");
		_WanderTimer = GetNode<Timer>("WanderTimer");
		_OnWanderTimerTimeout();

		var material = _AnimatedSprite.Material as ShaderMaterial;
		if (IsBoss && material != null) {
			material.Shader = Bat.BossShader;
			_Stats.IsBoss = true;
		}
	}

	private void Die() {
		var deathEffect = Bat.DeathEffectScene.Instance<Node2D>();
		deathEffect.Transform = Transform;
		GetParent().AddChild(deathEffect);
		QueueFree();
	}

	private void AccelerateTowards(float delta, Vector2 destination) {
		var moveDirection = GlobalPosition.DirectionTo(destination).Normalized();
		_Velocity = _Velocity.MoveToward(moveDirection * MaxSpeed, Acceleration * delta);
		_AnimatedSprite.FlipH = _Velocity.x < 0;

		Move(delta);
	}

	private void Move(float delta, bool collide = false) {
		// keep out of the way of other bats
		if (_SoftCollision.IsColliding) {
			_Velocity += _SoftCollision.PushVector * delta * MaxSpeed * 4;
		}

		if (collide) {
			var collision = MoveAndCollide(_Velocity * delta);
			if (collision != null) {
				_Velocity = _Velocity.Bounce(collision.Normal) * .5f;
			}
		} else {
			MoveAndSlide(_Velocity);
		}
	}

	private void StateChase(float delta) {
		var destination = _PlayerDectionZone.DetectedPlayer?.GlobalPosition ?? GlobalPosition;
		AccelerateTowards(delta, destination);
	}

	private void StateIdle(float delta) {
		_Velocity = _Velocity.MoveToward(Vector2.Zero, Friction * delta);
		Move(delta, true);
	}

	private void StateWander(float delta) {
		if (GlobalPosition.DistanceTo(_WanderPosition) > Acceleration * delta) {
			AccelerateTowards(delta, _WanderPosition);
		}
	}

	private void _OnWanderTimerTimeout() {
		if (!_Stats.IsAlive) {
			return;
		}

		_State = IdleStates[(int)GD.RandRange(0, IdleStates.Length)];
		_WanderTimer.Start((float)GD.RandRange(1, 3));

		if (_State == BatState.Wander) {
			_WanderPosition = _InitialPosition + new Vector2(
				(float)GD.RandRange(-WanderRange, WanderRange),
				(float)GD.RandRange(-WanderRange, WanderRange)
			);
		}
	}
}
