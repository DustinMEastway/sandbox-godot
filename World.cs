
using Godot;
using System;

public class World : Node2D {
	public static readonly PackedScene BatScene = ResourceLoader.Load<PackedScene>("res://Enemies/Bat.tscn");
	public static readonly PackedScene PlayerScene = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn");
	public Vector2 SpawnPoint = new Vector2(160, 96);
	private long _BatCount;
	private Vector2[] _BatSpawnPoints = {
		new Vector2(176, -16),
		new Vector2(0, 90),
		new Vector2(304, 208),
		new Vector2(368, -16)
	};
	private Camera2D _Camera;
	private HealthUi _HealthUi;
	private Panel _HelpPanel;
	private Player _Player;
	private Timer _SpawnTimer;
	private long _Wave = 0;
	private Label _WaveLabel;
	private Timer _WaveTimer;
	private YSort _YSort;

	public override void _Ready() {
		_Camera = GetNode<Camera2D>("Camera2D");
		_HealthUi = GetNode<HealthUi>("CanvasLayer/HealthUi");
		_HelpPanel = GetNode<Panel>("HelpPanel");
		_SpawnTimer = GetNode<Timer>("SpawnTimer");
		_YSort = GetNode<YSort>("YSort");
		_WaveLabel = GetNode<Label>("CanvasLayer/WaveLabel");
		_WaveTimer = GetNode<Timer>("WaveTimer");
		_SpawnTimer.Start();
		_WaveTimer.Start();
	}

	public void SpawnPlayer() {
		_HelpPanel.Visible = false;
		_Player = World.PlayerScene.Instance<Player>();
		_Player.Connect(nameof(Player.Ready), this, nameof(_OnPlayerReady));
		_Player.Position = SpawnPoint;
		var cameraTransform = new RemoteTransform2D();
		cameraTransform.RemotePath = "../../../Camera2D";
		_Player.AddChild(cameraTransform);
		_YSort.AddChild(_Player);
	}

	public void SpawnWave() {
		++_Wave;
		_BatCount = _Wave;
		for (long i = 0; i < _BatCount; ++i) {
			var bat = World.BatScene.Instance<Bat>();
			bat.Position = _BatSpawnPoints[(int)GD.RandRange(0, _BatSpawnPoints.Length)];
			bat.IsBoss = i % 3 == 2;
			bat.Connect(nameof(Bat.Ready), this, nameof(_OnBatReady));
			_YSort.AddChild(bat);
		}
		_UpdateWaveLabel();
	}

	private void _OnBatReady(Bat bat) {
		bat.Stats.Connect(nameof(Stats.Die), this, nameof(_OnBatStatsDie));
	}

	private void _OnBatStatsDie() {
		--_BatCount;
		_UpdateWaveLabel();
		if (_BatCount < 1) {
			_WaveTimer.Start();
		}
	}

	private void _OnPlayerReady(Player player) {
		_Player.Stats.Connect(nameof(Stats.Die), this, nameof(_OnPlayerStatsDie));
		_HealthUi.UiStats = _Player.Stats;
	}

	private void _OnPlayerStatsDie() {
		_HelpPanel.RectPosition = _Camera.GetCameraScreenCenter() - (_HelpPanel.RectSize / 2);
		_HelpPanel.Visible = true;
		_HealthUi.UiStats = null;
		_SpawnTimer.Start();
	}

	private void _OnSpawnTimerTimeout() {
		SpawnPlayer();
	}

	private void _OnWaveTimerTimeout() {
		SpawnWave();
	}

	private void _UpdateWaveLabel() {
		_WaveLabel.Text = $"WAVE: {_Wave}\nBATS: {_BatCount}";
	}
}
