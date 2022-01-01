using Godot;
using System;

public class HealthUi : Control {
	private Label _Label;
	private Stats _Stats;

	public Stats Stats {
		get => _Stats;
		set {
			_Stats?.Disconnect(nameof(Stats.Change), this, nameof(_OnStatsChange));
			_Stats = value;
			_Stats?.Connect(nameof(Stats.Change), this, nameof(_OnStatsChange));
		}
	}

	public override void _Ready() {
		_Label = GetNode<Label>("Label");
	}

	private void _OnStatsChange(Stats stats) {
		_Label.Text = "HP: " + stats.Health;
	}
}
