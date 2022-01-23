
using Godot;
using System;

public class World : Node2D {
	public static readonly PackedScene PlayerScene = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn");
	public Vector2 _SpawnPoint = new Vector2(160, 96);
	private Player _Player;
	private HealthUi _HealthUi;
	private YSort _YSort;

	public override void _Ready() {
		_HealthUi = GetNode<HealthUi>("CanvasLayer/HealthUi");
		_YSort = GetNode<YSort>("YSort");
		SpawnPlayer();
	}

	public void SpawnPlayer() {
		_Player = World.PlayerScene.Instance<Player>();
		_Player.Connect(nameof(Player.Ready), this, "_OnPlayerReady");
		_Player.Position = _SpawnPoint;
		var cameraTransform = new RemoteTransform2D();
		cameraTransform.RemotePath = "../../../Camera2D";
		_Player.AddChild(cameraTransform);
		_YSort.AddChild(_Player);
	}

	private void _OnPlayerReady(Player player) {
		_HealthUi.UiStats = _Player.Stats;
	}
}
