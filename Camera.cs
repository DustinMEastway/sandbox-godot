using Godot;
using System;

public class Camera : Camera2D {
  private Position2D _BottomRight;
  private Position2D _TopLeft;

  public override void _Ready() {
	_BottomRight = GetNode<Position2D>("Limits/BottomRight");
	_TopLeft = GetNode<Position2D>("Limits/TopLeft");

	LimitBottom = (int)_BottomRight.Position.y;
	LimitLeft = (int)_TopLeft.Position.x;
	LimitRight = (int)_BottomRight.Position.x;
	LimitTop = (int)_TopLeft.Position.y;
  }
}
