using Godot;
using System;

public enum PlayerState {
	Attack,
	Move,
	Roll
}

public class Player : KinematicBody2D {
	public static readonly PackedScene PlayerHurtSoundScene = ResourceLoader.Load<PackedScene>("res://Player/PlayerHurtSound.tscn");
	/// <summary>How quickly the player's <see cref="Velocity"> gets up to <see cref="MaxSpeed"></summary>
	[Export]
	public float Acceleration = 500;
	/// <summary>Max distance per second the player can travel at.</summary>
	[Export]
	public float MaxSpeed = 80;
	private AnimationPlayer _BlinkAnimationPlayer;
	private AnimationTree _AnimationTree;
	private AnimationNodeStateMachinePlayback _AnimationTreeState;
	private Hurtbox _Hurtbox;
	private Vector2 _InputDirection = Vector2.Zero;
	private PlayerState _State = PlayerState.Move;
	private Stats _Stats;
	private SwordHitbox _SwordHitbox;

	/// <summary>How quickly the player's <see cref="Velocity"> slows down to 0.</summary>
	public float Friction {
		get => Acceleration;
	}

	public float MaxRollSpeed {
		get => MaxSpeed * 1.5f;
	}

	public Stats Stats {
		get => _Stats;
	}

	/// <summary>Current distance per second the player is travelling at.</summary>
	public Vector2 Velocity { get; private set; }

	/// <inheritdoc />
 	public override void _PhysicsProcess(float delta) {
		switch (_State) {
			case PlayerState.Attack:
				StateAttack(delta);
				break;
			case PlayerState.Move:
			 	StateMove(delta);
				break;
			case PlayerState.Roll:
				StateRoll(delta);
				break;
		}
	}

	/// <inheritdoc />
	public override void _Ready() {
		_AnimationTree = GetNode<AnimationTree>("AnimationTree");
		_AnimationTreeState = _AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
		_AnimationTree.Active = true;
		_BlinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
		_Hurtbox = GetNode<Hurtbox>("Hurtbox");
		_Stats = GetNode<Stats>("Stats");
		_SwordHitbox = GetNode<SwordHitbox>("HitboxPivot/SwordHitbox");
	}

	private void _OnHurtboxAreaEntered(object area) {
		_Stats.TakeDamage((area as Hitbox)?.Damage ?? 0);
		_Hurtbox.BecomeInvincible(0.6f);
		var playerHurtSound = Player.PlayerHurtSoundScene.Instance<PlayerHurtSound>();
		GetTree().CurrentScene.AddChild(playerHurtSound);
	}

	private void _OnHurtboxInvincibleChange(bool invincible) {
		_BlinkAnimationPlayer.Play((invincible) ? "Start" : "Stop");
	}

	public void _OnStateFinished() {
		_State = PlayerState.Move;
	}

	private void _OnStatsDie() {
		QueueFree();
	}

	private void StateAttack(float delta) {
		_AnimationTreeState.Travel("Attack");
		Velocity = Vector2.Zero;
	}

	private void StateMove(float delta) {
		_InputDirection = new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		).Normalized();

		if (_InputDirection != Vector2.Zero) {
			_AnimationTree.Set("parameters/Attack/blend_position", _InputDirection);
			_AnimationTree.Set("parameters/Idle/blend_position", _InputDirection);
			_AnimationTree.Set("parameters/Roll/blend_position", _InputDirection);
			_AnimationTree.Set("parameters/Run/blend_position", _InputDirection);
			_SwordHitbox.KnockbackDirection = _InputDirection;
			_AnimationTreeState.Travel("Run");
			Velocity = Velocity.MoveToward(_InputDirection * MaxSpeed, Acceleration * delta);
		} else {
			_AnimationTreeState.Travel("Idle");
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);
		}

		Velocity = MoveAndSlide(Velocity);

		if (Input.IsActionJustPressed("attack")) {
			_State = PlayerState.Attack;
		} else if (Input.IsActionJustPressed("roll") && _InputDirection != Vector2.Zero) {
			_State = PlayerState.Roll;
		}
	}

	private void StateRoll(float delta) {
		_AnimationTreeState.Travel("Roll");
		Velocity = _InputDirection * MaxRollSpeed;
		Velocity = MoveAndSlide(Velocity);
	}
}
