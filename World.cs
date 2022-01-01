
using Godot;
using System;

public class World : Node2D {
	private Player _Player;
	private HealthUi _HealthUi;

	public override void _Ready() {
		_HealthUi = GetNode<HealthUi>("HealthUi");
		_Player = GetNode<Player>("YSort/Player");
		_HealthUi.UiStats = _Player.Stats;
	}
}
