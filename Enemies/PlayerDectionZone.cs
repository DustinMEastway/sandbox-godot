using Godot;

public class PlayerDectionZone : Area2D {
	[Signal]
	public delegate void PlayerDetected(Player player);

	[Signal]
	public delegate void PlayerLost(Player player);

	public Player DetectedPlayer;

	public bool PlayerIsDetected {
		get => DetectedPlayer != null;
	}

	private void _OnPlayerDectionZoneBodyEntered(object body) {
		DetectedPlayer = body as Player;
		EmitSignal(nameof(PlayerDetected), DetectedPlayer);
	}

	private void _OnPlayerDectionZoneBodyExited(object body) {
		DetectedPlayer = null;
		EmitSignal(nameof(PlayerLost), body as Player);
	}
}
