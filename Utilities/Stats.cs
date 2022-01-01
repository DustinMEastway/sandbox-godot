using Godot;
using System;

public class Stats : Node {
	[Signal]
	public delegate void Change(Stats stats);
	[Signal]
	public delegate void Die();
	private int? _Health = null;
	private int _MaxHealth = 1;

	[Export]
	public int MaxHealth {
		get => _MaxHealth;
		set {
			_MaxHealth = value;
			EmitSignal(nameof(Change), this);
		}
	}

	public int Health {
		get => _Health ?? MaxHealth;
		set {
			_Health = (value == MaxHealth) ? (int?)null : value;
			EmitSignal(nameof(Change), this);
			if (Health <= 0) {
				EmitSignal(nameof(Die));
			}
		}
	}

	public int TakeDamage(int damage) {
		return Health -= damage;
	}
}
