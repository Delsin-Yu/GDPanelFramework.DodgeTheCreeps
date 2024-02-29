using System;
using System.Threading;
using GDPanelFramework.Panels;
using Godot;
using GodotPanelFramework;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

public partial class GamePanel : UIPanelArg<GamePanel.OpenContext, Empty>
{
    public record struct OpenContext(Label CenterText, SceneObjectsModel SceneObjectsModel);

    [Export] private Label _score;
    [Export] private double _startNewGameWaitTime;
    [Export] private double _mobSpawnTime;
    [Export] private PackedScene _mobPrefab;
    [Export] private PackedScene _playerPrefab;

    private Vector2 _screenBound;

    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        _screenBound = GetViewportRect().Size;
    }

    protected override void _OnPanelOpen(OpenContext openContext) => StartNewGameAsync(openContext).Forget();

    private const string Player_Up = "player_up";
    private const string Player_Down = "player_down";
    private const string Player_Left = "player_left";
    private const string Player_Right = "player_right";
    
    private async GDTask StartNewGameAsync(OpenContext openContext)
    {
        var (centerText, (mobContainer, playerContainer, mobSpawnLocation, playerSpawn)) = openContext;

        foreach (var child in mobContainer.GetChildren()) child.QueueFree();

        var playerInstance = _playerPrefab.Instantiate<PlayerController>();
        playerContainer.AddChild(playerInstance);
        playerInstance.Position = playerSpawn.Position;
        playerInstance.ScreenBound = _screenBound;
        SetupControls(true);

        centerText.Text = "Get Ready!";
        _score.Text = "0";

        await GDTask.Delay(TimeSpan.FromSeconds(_startNewGameWaitTime));

        centerText.Hide();

        using var mobSpawnerCTS = new CancellationTokenSource();
        using var scoreTimerCTS = new CancellationTokenSource();
        StartScoreCount(mobSpawnerCTS.Token).Forget();
        StartMobSpawn(scoreTimerCTS.Token).Forget();

        await playerInstance.StartAndWaitForCollision();

        playerInstance.QueueFree();

        mobSpawnerCTS.Cancel();
        scoreTimerCTS.Cancel();

        centerText.Show();
        centerText.Text = "Game Over";

        SetupControls(false);

        await GDTask.Delay(TimeSpan.FromSeconds(2));

        ClosePanel(Empty.Default);

        return;

        void SetupControls(bool enable)
        {
            ToggleInputVector(
                enable,
                Player_Down,
                Player_Up,
                Player_Left,
                Player_Right,
                rawInput =>
                {
                    var normalized = rawInput.Normalized();
                    playerInstance.UpdateMoveDirection(normalized);
                },
                CompositeInputActionState.Update
            );
            ToggleInputVector(
                enable,
                Player_Down,
                Player_Up,
                Player_Left,
                Player_Right,
                _ => playerInstance.UpdateMoveDirection(Vector2.Zero),
                CompositeInputActionState.End
            );
        }

        async GDTaskVoid StartScoreCount(CancellationToken cancellationToken)
        {
            var currentScore = 0;
            var oneSecond = TimeSpan.FromSeconds(1);
            while (!cancellationToken.IsCancellationRequested)
            {
                await GDTask
                    .Delay(oneSecond, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                currentScore++;
                _score.Text = currentScore.ToString();
            }
        }

        async GDTaskVoid StartMobSpawn(CancellationToken cancellationToken)
        {
            var mobSpawnTime = TimeSpan.FromSeconds(_mobSpawnTime);
            while (!cancellationToken.IsCancellationRequested)
            {
                await GDTask
                    .Delay(mobSpawnTime, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();

                var mob = _mobPrefab.Instantiate<MobController>();

                mobSpawnLocation.ProgressRatio = Random.Shared.NextSingle();

                var direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

                mob.Position = mobSpawnLocation.Position;

                direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
                mob.Rotation = direction;

                var velocity = new Vector2(Mathf.Lerp(150f, 250f, Random.Shared.NextSingle()), 0);
                mob.LinearVelocity = velocity.Rotated(direction);

                mobContainer.AddChild(mob);
            }
        }
    }
}