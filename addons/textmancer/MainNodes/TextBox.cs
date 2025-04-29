using System;
using Godot;
using System.Collections.Generic;


[Tool]
public partial class TextBox : RichTextLabel
{
	[Export] public Vector2I resolution; // Bloquear negativos
	[Export] public char baseChar = '.';
	[Export] public int FrameRate = 15;
	public List<TextObject> _textObjects;
	private double _timeSinceLastUpdate = 0;
	[Export] public bool testing
	{
		get => false;
		set => UpdateTextObjects();
	}

	private string[] _screenLines;
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
	}
	
	public void UpdateBox()
	{
		GD.Print("UpdateBox");
		
		ClearScreenLines();
		
		foreach (TextObject textObject in _textObjects)
		{
			GD.Print($" Screen: {_screenLines.Length} - Pos: {textObject.ObjectPosition.Y}");
			char[] temp = _screenLines[textObject.ObjectPosition.Y].ToCharArray();

			for(int i = 0; i < textObject.Sprite.Length; i++)
			{
				temp[i + textObject.ObjectPosition.X] = textObject.Sprite[i];
			}
			
			_screenLines[textObject.ObjectPosition.Y] = new string(temp);
		}
		
		Text = String.Empty;
		GD.Print(_screenLines.Length);
		foreach (string Line in _screenLines)
		{
			Text += Line + "\n";
		}
		GD.Print(_screenLines.Length);
	}

	private void ClearScreenLines()
	{
		_screenLines = new string[resolution.Y];
		string baseLine = new string(baseChar, resolution.X);

		for (int i = 0; i < resolution.Y; i++)
		{
			_screenLines[i] = baseLine;
		}
	}
	
	private void UpdateTextObjects()
	{
		_textObjects = GetChilds<TextObject>();
		GD.Print(_textObjects.Count);
		_textObjects.Sort();
		UpdateBox();
	}
	
	public List<T> GetChilds<T>()
	{
		List<T> childs = new List<T>();
		
		foreach (Node child in GetChildren())
		{
			if (child is T tChild)
			{
				childs.Add(tChild);
			}
		}

		return childs;
	}
}
