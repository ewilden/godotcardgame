using Godot;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        base._Ready();
        GD.Print("Ready from Godot again!", this);
    }
}
