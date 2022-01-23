
using Godot;
using System;

public class World : Node2D {
	public static readonly PackedScene PlayerScene = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn");
	public Vector2 SpawnPoint = new Vector2(160, 96);
	private HealthUi _HealthUi;
	private Player _Player;
	private Timer _SpawnTimer;
	private YSort _YSort;

	public override void _Ready() {
		_HealthUi = GetNode<HealthUi>("CanvasLayer/HealthUi");
		_SpawnTimer = GetNode<Timer>("SpawnTimer");
		_YSort = GetNode<YSort>("YSort");
		_SpawnTimer.Start();
	}

	public void SpawnPlayer() {
		_Player = World.PlayerScene.Instance<Player>();
		_Player.Connect(nameof(Player.Ready), this, "_OnPlayerReady");
		_Player.Position = SpawnPoint;
		var cameraTransform = new RemoteTransform2D();
		cameraTransform.RemotePath = "../../../Camera2D";
		_Player.AddChild(cameraTransform);
		_YSort.AddChild(_Player);
	}

	private void _OnPlayerReady(Player player) {
		_Player.Stats.Connect(nameof(Stats.Die), this, "_OnPlayerStatsDie");
		_HealthUi.UiStats = _Player.Stats;
	}

	private void _OnPlayerStatsDie() {
		_HealthUi.UiStats = null;
		_SpawnTimer.Start();
	}

	private void _OnSpawnTimerTimeout() {
		SpawnPlayer();
	}
}
