// -----------------------------------------------------------------------
// <copyright file="TextObject.cs" company="Salmaze">
//     Copyright (c) Pedro Henrique Salmaze. All rights reserved.
//     Licensed under the MIT License.
// </copyright>
// <author>Pedro H. Salmaze - pedrohsalmaze@gmail.com</author>
// <date>2026-06-09</date>
// -----------------------------------------------------------------------

using Godot;
using System;
using System.Text.Json;

[Tool]
public partial class TextObject : Control, IComparable<TextObject>
{
    [Signal] public delegate void OnTextObjectPositionChangedEventHandler(TextObject thisObj);
    [Signal] public delegate void OnTextObjectLayerChangedEventHandler(TextObject thisObj);
    [Signal] public delegate void OnTextObjectSpriteChangedEventHandler(TextObject thisObj);
    
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
            if (value == _position) return;
            if (VerifyPosition())
            {
                _position = value;
                EmitSignal(SignalName.OnTextObjectPositionChanged, this);
            }
        }
    }

    
    [ExportGroup("Sprite")] 
    private string _sprite;
    public string[] Sprite;
    [Export] private bool SolidSpaces;
    
    private string _jsonFilePath;

    [Export(PropertyHint.File, "*.json")] 
    public string JsonFilePath 
    { 
        get => _jsonFilePath;
        set 
        {
            _jsonFilePath = value;
            
            AcessJson();
            
            EmitSignal(SignalName.OnTextObjectSpriteChanged, this);
        }
    }

    
    private int _layer;

    [Export]
    public int Layer
    {
        get => _layer;
        set
        {
            _layer = value;
            EmitSignal(SignalName.OnTextObjectLayerChanged, this);
        }
    }
    
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
            EmitSignal(SignalName.OnTextObjectSpriteChanged, this);
            return;
        }
        string jsonText = file.GetAsText();
        file.Close();
        
        using JsonDocument json = JsonDocument.Parse(jsonText);
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
    public int CompareTo(TextObject other)
    {
        int c = Layer.CompareTo(other.Layer);
        if (c != 0) return c;
        return GetIndex().CompareTo(other.GetIndex()); // ordem na árvore como desempate
    }
}
