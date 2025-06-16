using Godot;

public partial class CustomCursor : Node
{
    [Export] public Texture2D CursorTexture { get; set; }
    [Export] public Vector2 Hotspot { get; set; } = Vector2.Zero;

    public override void _Ready()
    {
        SetCustomCursor();
    }

    private void SetCustomCursor()
    {
            Input.SetCustomMouseCursor(
                image: CursorTexture,
                shape: Input.CursorShape.Arrow,
                hotspot: Hotspot 
            );
    }
}