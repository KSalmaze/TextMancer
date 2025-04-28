using Godot;
using System;

[Tool]
public partial class TextObject : Control
{
    private string _sprite; 
    [Export] public string Sprite
    {
        get
        {
            return _sprite;
        }
        set
        {
            _sprite = value;
            EmitSignal(SignalName.TextObjectChanged);
        }
    }

    [Signal] public delegate void TextObjectChangedEventHandler();
}
