using Godot;
using System;

public class SoftCollision : Area2D {
	public bool IsColliding {
		get {
			return GetOverlappingAreas().Count > 0;
		}
	}

	public Vector2 PushVector {
		get {
			var areas = GetOverlappingAreas();

			var area = (areas.Count == 0) ? null : areas[0] as Area2D;
			return area?.GlobalPosition.DirectionTo(GlobalPosition).Normalized() ?? Vector2.Zero;
		}
	}
}
