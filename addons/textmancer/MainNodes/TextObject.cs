using Godot;
using System;
using System.Text.Json;

[Tool]
public partial class TextObject : Control, IComparable<TextObject>
{
    private Vector2I _position;

    [Export]
    public Vector2I ObjectPosition
    {
        get => _position;
        set
        {
            if (VerifyPosition())
            {
                _position = value;
                EmitSignal(SignalName.TextObjectChanged);
            }
        }
    }
    
    [Export] public int Layer;
    [Export(PropertyHint.File, "*.json")]
    public string JsonFilePath = "";
    
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

    [Export] public bool testing
    {
        get => false;
        set => Update();
    }
    
    public override void _Ready()
    {
    }

    public bool VerifyPosition()
    {
        return true;
    }
    
    public void Update()
    {
        var file = FileAccess.Open(JsonFilePath, FileAccess.ModeFlags.Read);
        string jsonText = file.GetAsText();
        file.Close();
        
        JsonDocument json = JsonDocument.Parse(jsonText);
        Sprite = json.RootElement.GetProperty("sprite").ToString();
    }
    public int CompareTo(TextObject obj)
    {
        return Layer.CompareTo(obj.Layer);
    }

    [Signal] public delegate void TextObjectChangedEventHandler();
}
