using System;
using Godot;

public class GameCamera : Camera2D
{
    [Export]
    private float ViewportScrollThreshold = 0.8f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // Move the camera when the player gets near an edge
        var viewportWidthInGameUnits = this.GetViewportRect().Size.x / this.Zoom.x;

        var playerGroup = GetTree().GetNodesInGroup("Player");
        if (playerGroup.Count > 0)
        {
            var player = playerGroup[0] as Node2D ?? throw new Exception("Player not node2d??");
            
            var maxX = this.Position.x + viewportWidthInGameUnits * ViewportScrollThreshold * 0.5f;
            var minX = this.Position.x - viewportWidthInGameUnits * ViewportScrollThreshold * 0.5f;

            Vector2 newPos = this.Position;
            if (player.Position.x > maxX)
            {
                newPos.x += player.Position.x - maxX;
            }
            else if (player.Position.x < minX)
            {
                newPos.x -= minX - player.Position.x;
            }
            this.Position = newPos;
        }
    }
}
