using Godot;
using System;

public class PlayerHurtSound : AudioStreamPlayer {
	public override void _Ready() {
		Connect("finished", this, "_OnFinished");
	}

	private void _OnFinished() {
		QueueFree();
	}
}
