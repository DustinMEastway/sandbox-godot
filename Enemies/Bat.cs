using Godot;

public class Bat : KinematicBody2D {
	private int _Health = 3;
	private Vector2 _Velocity = Vector2.Zero;

	public float Friction {
		get => 200;
	}

	private void _OnHurtboxAreaEntered(object area) {
		if (--_Health > 0) {
			var knockbackDirection = (area as SwordHitbox)?.KnockbackDirection ?? Vector2.Zero;
			_Velocity = knockbackDirection * 150;
		} else {
			Die();
		}
	}

	public override void _PhysicsProcess(float delta) {
		_Velocity = _Velocity.MoveToward(Vector2.Zero, Friction * delta);
		var collision = MoveAndCollide(_Velocity * delta);
		if (collision != null) {
			_Velocity = _Velocity.Bounce(collision.Normal) * .5f;
		}
	}

	private void Die() {
		QueueFree();
	}
}
