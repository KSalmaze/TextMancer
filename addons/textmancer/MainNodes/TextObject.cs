using Godot;
using System;

[Tool]
public partial class TextObject : Control, IComparable<TextObject>
{
    [Export] public Vector2I ObjectPosition;
    [Export] public int Layer;
    
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

    public bool VerifyPosition()
    {
        return true;
    }
    
    public int CompareTo(TextObject obj)
    {
        return Layer.CompareTo(obj.Layer);
    }

    [Signal] public delegate void TextObjectChangedEventHandler();
}
