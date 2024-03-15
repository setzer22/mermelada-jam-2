using Godot;

[Tool]
public class Background : Node2D
{
    public override void _Process(float delta)
    {
        var aspectRatio = Transform.Scale.x / Transform.Scale.y;
        if (GetNode<Sprite>("Background")?.Material is ShaderMaterial material)
        {
            material.SetShaderParam("uv_scale", new Vector2(aspectRatio, 1));
        }
        else
        {
            GD.PrintErr("Background material not found!");
        }
    }
}
