using Godot;
using System;
using System.Text.Json;

[Tool]
public partial class TextObject : Control, IComparable<TextObject>
{
    [Signal] public delegate void TextObjectChangedEventHandler();
    [Export] public bool testing
    {
        get => false;
        set => AcessJson();
    }
    
    [ExportGroup("Transform")]
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


    [ExportGroup("Sprite")] 
    
    private string _sprite;

    public string[] Sprite;
    [Export] private string SpritePreview
    {
        get => _sprite;
        set
        {
            _sprite = value;
            EmitSignal(SignalName.TextObjectChanged);
        }
    }
    
    private string _jsonFilePath;

    [Export(PropertyHint.File, "*.json")] 
    public string JsonFilePath 
    { 
        get => _jsonFilePath;
        set 
        {
            _jsonFilePath = value;
            
            AcessJson();
        }
    }

    
    [Export] public int Layer;
    
    [ExportGroup("Collision")]
    [Export(PropertyHint.File, "*.json")] public string CollisionJsonFilePath; 
    
    public override void _Ready()
    {
        AcessJson();
    }

    public bool VerifyPosition()
    {
        return true;
    }
    
    public void AcessJson()
    {
        GD.Print("AcessJson");
        var file = FileAccess.Open(JsonFilePath, FileAccess.ModeFlags.Read);

        if (file == null)
        {
            Sprite[0] = "JSON ERROR";
            return;
        }
        string jsonText = file.GetAsText();
        file.Close();
        
        JsonDocument json = JsonDocument.Parse(jsonText);
        JsonElement elements = json.RootElement.GetProperty("sprite");
        
        Sprite = new string[elements.GetArrayLength()];

        int counter = 0;
        foreach (JsonElement element in elements.EnumerateArray())
        {
            GD.Print("Line read: " + element.GetString());
            Sprite[counter] = element.GetString();
            counter++;
        }
    }
    
    // Interfaces
    public int CompareTo(TextObject obj)
    {
        return Layer.CompareTo(obj.Layer);
    }
}
