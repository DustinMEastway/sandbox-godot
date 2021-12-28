using Godot;
using System;

public class Player : KinematicBody2D {
	private AnimationTree _AnimationTree;
	private AnimationNodeStateMachinePlayback _AnimationTreeState;

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
			_AnimationTree.Set("parameters/Idle/blend_position", inputVector);
			_AnimationTree.Set("parameters/Run/blend_position", inputVector);
			_AnimationTreeState.Travel("Run");
			Velocity = Velocity.MoveToward(inputVector * MaxSpeed, Acceleration * delta);
		} else {
			_AnimationTreeState.Travel("Idle");
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);
		}

		Velocity = MoveAndSlide(Velocity);
	}

	/// <inheritdoc />
	public override void _Ready() {
		_AnimationTree = GetNode<AnimationTree>("AnimationTree");
		_AnimationTreeState = _AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
	}
}
