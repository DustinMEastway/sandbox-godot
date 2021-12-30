using Godot;
using System;

public class Stats : Node {
	[Signal]
	public delegate void Die();
	private int? _Health = null;

	[Export]
	public int MaxHealth = 1;

	public int Health {
		get => _Health ?? MaxHealth;
		set {
			_Health = (value == MaxHealth) ? (int?)null : value;
			if (Health <= 0) {
				EmitSignal(nameof(Die));
			}
		}
	}

	public int TakeDamage(int damage) {
		return Health -= damage;
	}
}
