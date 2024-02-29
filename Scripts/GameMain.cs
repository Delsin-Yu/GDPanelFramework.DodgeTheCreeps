using Godot;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

/// <summary>
/// This script serves as the entry point of the game, its sole purpose is to create and open the <see cref="MainPanel"/>.
/// </summary>
public partial class GameMain : Node
{
    [Export] private PackedScene _mainPanel;
    [Export] private SceneObjectsModel _sceneObjectsModel;

    public override void _Ready() => ReadyAsync().Forget();
    
    public async GDTaskVoid ReadyAsync()
    {
        await GDTask.NextFrame();
        
        _mainPanel
            .CreatePanel<MainPanel>()
            .OpenPanel(
                _sceneObjectsModel,
                onPanelCloseCallback: _ => GetTree().Quit(),
                closePolicy: ClosePolicy.Delete
            );
    }
}