using Godot;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

public partial class PlayerController : Area2D
{
    [Export] private int _speed = 400;
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private CollisionShape2D _collisionShape;

    private Node _collisionHit;
    private Vector2 _moveDirection;
    
    public Vector2 ScreenBound { private get; set; }
    
    public override void _Ready()
    {
        BodyEntered += body => _collisionHit = body;
    }

    public override void _Process(double delta)
    {
        if (_moveDirection.X != 0)
        {
            _animatedSprite.Animation = "walk";
            _animatedSprite.FlipV = false;
            _animatedSprite.FlipH = _moveDirection.X < 0;
        }
        else if (_moveDirection.Y != 0)
        {
            _animatedSprite.Animation = "up";
            _animatedSprite.FlipV = _moveDirection.Y > 0;
        }

        if (_moveDirection.Length() > 0) _animatedSprite.Play();
        else _animatedSprite.Stop();
        
        Position += _moveDirection * _speed * (float)delta;
        Position = new(Mathf.Clamp(Position.X, 0, ScreenBound.X), Mathf.Clamp(Position.Y, 0, ScreenBound.Y));
    }

    public async GDTask<Node> StartAndWaitForCollision()
    {
        _collisionShape.Disabled = false;
        await GDTask.WaitUntil(() => _collisionHit != null);
        _collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        return _collisionHit;
    }
    
    public void UpdateMoveDirection(Vector2 normalizedMoveDirection) => _moveDirection = normalizedMoveDirection;
}