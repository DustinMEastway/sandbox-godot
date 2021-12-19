using Godot;
using System;

public class Player : KinematicBody2D {
	private AnimationPlayer _AnimationPlayer;

	/// <summary>How quickly the player's <see cref="Velocity"> gets up to <see cref="MaxSpeed"></summary>
	public float Acceleration {
		get => 500;
	}

	/// <summary>How quickly the player's <see cref="Velocity"> slows down to 0.</summary>
	public float Friction {
		get => Acceleration;
	}

	/// <summary>Max distance per second the player can travel at.</summary>
	public float MaxSpeed {
		get => 80;
	}

	/// <summary>Current distance per second the player is travelling at.</summary>
	public Vector2 Velocity { get; private set; }

	/// <inheritdoc />
 	public override void _PhysicsProcess(float delta) {
		var inputVector = new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		).Normalized();

		if (inputVector != Vector2.Zero) {
			if (inputVector.x > 0) {
				_AnimationPlayer.Play("Run");
			} else {
				_AnimationPlayer.Play("RunLeft");
			}

			Velocity = Velocity.MoveToward(inputVector * MaxSpeed, Acceleration * delta);
		} else {
			_AnimationPlayer.Play("IdleRight");
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);
		}

		Velocity = MoveAndSlide(Velocity);
	}

	/// <inheritdoc />
	public override void _Ready() {
		_AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
}