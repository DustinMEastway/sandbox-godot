using Godot;
using System;

public class HealthUi : Control {
	private const int _HeartSize = 15;
	private TextureRect _HealthUi;
	private TextureRect _MaxHealthUi;
	private Stats _UiStats;

	public Stats UiStats {
		get => _UiStats;
		set {
			_UiStats?.Disconnect(nameof(Stats.Change), this, nameof(_OnUiStatsChange));
			_UiStats = value;
			_UiStats?.Connect(nameof(Stats.Change), this, nameof(_OnUiStatsChange));
			_OnUiStatsChange(value);
		}
	}

	public override void _Ready() {
		_HealthUi = GetNode<TextureRect>("HealthUi");
		_MaxHealthUi = GetNode<TextureRect>("MaxHealthUi");
		_OnUiStatsChange(UiStats);
	}

	private void _OnUiStatsChange(Stats stats) {
		if (_HealthUi == null || _MaxHealthUi == null) {
			return;
		}

		var health = stats?.Health ?? 0;
		var maxHealth = stats?.MaxHealth ?? 0;
		_HealthUi.RectSize = new Vector2(health * _HeartSize, _HealthUi.RectSize.y);
		_MaxHealthUi.RectSize = new Vector2(maxHealth * _HeartSize, _MaxHealthUi.RectSize.y);
	}
}
