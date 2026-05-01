using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using System.Threading.Tasks;

public partial class MyCustomVisuals : NCreatureVisuals
{
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        base._Ready(); 
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer"); 
        _animPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_animPlayer == null) return;


    }

    private void OnAnimationFinished(StringName animName)
    {

        if (animName == "attack" || animName == "cast" || animName == "hurt")
        {
            _animPlayer.Play("idle_loop");
        }
    }
}