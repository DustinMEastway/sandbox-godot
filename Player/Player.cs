using Godot;
using System;

public class Player : KinematicBody2D {
	private Vector2 _Velocity = Vector2.Zero;

 	public override void _PhysicsProcess(float delta) {
		_Velocity = new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		);

		MoveAndCollide(_Velocity);
	}
}
