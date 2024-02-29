using System;
using Godot;
using GDPanelFramework.Panels;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

public partial class MainPanel : UIPanelArg<SceneObjectsModel, Empty>
{
    [Export] private Button _startButton;
    [Export] private Button _quitButton;
    [Export] private Label _text;
    [Export] private PackedScene _gamePanel;

    private const string GreetingsMessage =
        """
        Dodge the
        Creeps!
        """;
    
    protected override void _OnPanelInitialize()
    {
        _startButton.Pressed += () => StartGameAsync().Forget();
        _quitButton.Pressed += () => ClosePanel(Empty.Default);
    }

    private async GDTask StartGameAsync()
    {
        _startButton.Hide();
        _quitButton.Hide();
        
        await _gamePanel.CreatePanel<GamePanel>().OpenPanelAsync(new(_text, OpenArg));
        
        _text.Text = GreetingsMessage;
        
        await GDTask.Delay(TimeSpan.FromSeconds(1));
        
        _startButton.Show();
        _quitButton.Show();
        _startButton.GrabFocus();
    }
    
    protected override void _OnPanelOpen(SceneObjectsModel sceneObjectsModel)
    {
        _text.Text = GreetingsMessage;
        _startButton.GrabFocus();
    }
}
