using Godot;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

public partial class PlayerController : Area2D
{
    [Export] private int _speed = 400;
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private CollisionShape2D _collisionShape;

    private Vector2 _moveDirection;
    
    public Vector2 ScreenBound { private get; set; }

    public override void _Process(double delta)
    {
        if (!Mathf.IsZeroApprox(_moveDirection.Length()))
        {
            _animatedSprite.Play();
            _animatedSprite.Animation = _moveDirection.Y != 0 ? "up" : "walk";
            _animatedSprite.FlipH = _moveDirection.X < 0;
            _animatedSprite.FlipV = _moveDirection.Y > 0;
        }
        else _animatedSprite.Stop();

        Position += _moveDirection * _speed * (float)delta;
        Position = new(Mathf.Clamp(Position.X, 0, ScreenBound.X), Mathf.Clamp(Position.Y, 0, ScreenBound.Y));
    }

    public async GDTask<Node> StartAndWaitForCollision()
    {
        _collisionShape.Disabled = false;
        var hit = await ToSignal(this, SignalName.BodyEntered);
        _collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        return (Node)hit[0];
    }
    
    public void UpdateMoveDirection(Vector2 normalizedMoveDirection) => _moveDirection = normalizedMoveDirection;
}