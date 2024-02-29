using Godot;
using System;

public partial class MobController : RigidBody2D
{
	[Export] private AnimatedSprite2D _animatedSprite;
	[Export] private VisibleOnScreenNotifier2D _screenNotifier;
	
	public override void _Ready()
	{
		_screenNotifier.ScreenExited += QueueFree;
		var mobTypes = _animatedSprite.SpriteFrames.GetAnimationNames();
		_animatedSprite.Play(mobTypes[Random.Shared.Next(0, mobTypes.Length - 1)]);
	}
}
