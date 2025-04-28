using System;
using Godot;
using System.Collections.Generic;


[Tool]
public partial class TextBox : RichTextLabel
{
	[Export] public int FrameRate = 15;
	public List<TextObject> _textObjects;
	private double _timeSinceLastUpdate = 0;
	[Export] public bool testing
	{
		get => false;
		set => UpdateTextObjects();
	}
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
	}
	
	public void UpdateText()
	{
		Text = _textObjects[0].Sprite;
	}
	
	private void UpdateTextObjects()
	{
		_textObjects = GetChilds<TextObject>();
		GD.Print(_textObjects.Count);
		UpdateText();
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
