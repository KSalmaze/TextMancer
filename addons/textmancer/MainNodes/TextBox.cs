// -----------------------------------------------------------------------
// <copyright file="TextBox.cs" company="Salmaze">
//     Copyright (c) Pedro Henrique Salmaze. All rights reserved.
//     Licensed under the MIT License.
// </copyright>
// <author>Pedro H. Salmaze - pedrohsalmaze@gmail.com</author>
// <date>2026-06-09</date>
// -----------------------------------------------------------------------

using System;
using Godot;
using System.Collections.Generic;
using System.Text;


[Tool]
public partial class TextBox : RichTextLabel
{
	// -------- Properties --------------------------------------------------
	
	private Vector2I _resolution = new Vector2I(320,180);
	[Export] public Vector2I Resolution
	{
		get => _resolution;
		set
		{
			_resolution = new Vector2I(Mathf.Max(1, value.X), Mathf.Max(1, value.Y));
			if (IsNodeReady()) RebuildAll();
		}
	}
	
	private char _baseChar = '.';
	[Export] public char BaseChar
	{
		get => _baseChar;
		set
		{
			_baseChar = value;
			if (IsNodeReady())
			{
				Array.Fill(_lineCopy, _baseChar);
				MarkAllDirty();
				NewUpdateBox();
			}
		}
	}
	
	
	[Export] public int FrameRate = 15; // Em breve vai ser controlado pelo Conductor
	private int _textSize = 10;
	[Export] public int TextSize {
		get => _textSize;
		set
		{
			if (value <= 1) return;
			_textSize = value;
			AddThemeFontSizeOverride("normal_font_size", _textSize);
		}
	} 
	// Adicionar Seter
	private List<TextObject> _textObjects;
	private double _timeSinceLastUpdate = 0;
	[Export] public bool testing
	{
		get => false;
		set
		{
			if (IsNodeReady())
			{
				UpdateTextObjects();
				NewUpdateBox();
			}
		}
	}

	[Export] public char emptySpaceChar = '◙';
	
	private List<TextObject>[] _lineToObjects;
	private readonly Dictionary<TextObject, (int top, int height)> _registered = new();
	
	//protected string[] _screenLines;
	//protected StringBuilder[] _screenLinesBuilder;
	protected char[][] _screenLines;
	protected char[] _lineCopy;
	private readonly StringBuilder _sb = new StringBuilder();
	
	private HashSet<int> _dirtyLines =  new HashSet<int>();
	
	// -------- Methods --------------------------------------------------
	
	private void AllocateBuffers()
	{
		_lineCopy = new char[_resolution.X];
		Array.Fill(_lineCopy, BaseChar);
		
		_screenLines = new char[_resolution.Y][];
		_lineToObjects = new List<TextObject>[_resolution.Y]; 
		for (int y = 0; y < _resolution.Y; y++)
		{
			_screenLines[y]   = (char[])_lineCopy.Clone();
			_lineToObjects[y] = new List<TextObject>();
		}
		
		_sb.EnsureCapacity(_resolution.Y * (_resolution.X + 1));
	}
	
	public override void _Ready()
	{
		ChildEnteredTree += OnChildEntered;
		ChildExitingTree += OnChildExiting;
		
		AllocateBuffers();
		SetupLabel();
		UpdateTextObjects();
		NewUpdateBox();   
	}
	
	private void SetupLabel()
	{
		BbcodeEnabled = false;
		AutowrapMode  = TextServer.AutowrapMode.Off;
		ScrollActive  = false;
		AddThemeFontSizeOverride("normal_font_size", _textSize);
	}
	
	public override void _Process(double delta)
	{
	}
	
	private void NewUpdateBox()
	{
		if (_dirtyLines.Count == 0) return;
		foreach (int y in _dirtyLines) ComposeLine(y);
		_dirtyLines.Clear();
		FlushToLabel();
	}

	private void ComposeLine(int y)
	{
		char[] dst = _screenLines[y];
		Array.Copy(_lineCopy, dst, _resolution.X);

		foreach (TextObject obj in _lineToObjects[y])   // já em ordem de camada
		{
			string src = obj.Sprite[y - obj.ObjectPosition.Y];
			if (src == null) continue;

			int x0    = obj.ObjectPosition.X;
			int start = Math.Max(0, -x0);                        // corta o que sai pela esquerda
			int end   = Math.Min(src.Length, _resolution.X - x0); // corta o que sai pela direita

			for (int i = start; i < end; i++)
			{
				char c = src[i];
				if (c != emptySpaceChar) dst[x0 + i] = c;
			}
		}
	}
	
	private void FlushToLabel()
	{
		_sb.Clear();
		for (int y = 0; y < _resolution.Y; y++)
		{
			_sb.Append(_screenLines[y]);
			if (y < _resolution.Y - 1) _sb.Append('\n');
		}
		Text = _sb.ToString();
	}
	
	
	// -------- Methods related to update of TextObjects --------------------------

	private void UpdateTextObjects()
	{
		_textObjects?.ForEach(Unsubscribe);
		_registered.Clear();
		foreach (var lista in _lineToObjects)
		{
			lista.Clear();
		}
		
		_textObjects = GetChilds<TextObject>();
			
		foreach (var to in _textObjects)
		{
			Subscribe(to);
			AddToLines(to, to.ObjectPosition.Y, to.Sprite?.Length ?? 0);
		}
		
		GD.Print(_textObjects.Count);

		MarkAllDirty();
	}
	
	private void Subscribe(TextObject obj)
	{
		obj.OnTextObjectPositionChanged += OnPositionChanged;
		obj.OnTextObjectLayerChanged    += OnLayerChanged;
		obj.OnTextObjectSpriteChanged   += OnSpriteChanged;
	}

	private void Unsubscribe(TextObject obj)
	{
		if (!IsInstanceValid(obj)) return;   // objeto já liberado: acessar dá ObjectDisposedException
		obj.OnTextObjectPositionChanged -= OnPositionChanged;
		obj.OnTextObjectLayerChanged    -= OnLayerChanged;
		obj.OnTextObjectSpriteChanged   -= OnSpriteChanged;
	}
	
	private void AddToLines(TextObject obj, int top, int height)
	{
		int start = Math.Max(top, 0);
		int end   = Math.Min(top + height - 1, _resolution.Y - 1);
		for (int y = start; y <= end; y++)          // se start > end, não entra
		{
			InsertSorted(_lineToObjects[y], obj);
			_dirtyLines.Add(y);
		}
		_registered[obj] = (top, height);
	}
	
	private void RemoveFromLines(TextObject obj)
	{
		if (!_registered.Remove(obj, out var r)) return;
		int start = Math.Max(r.top, 0);
		int end   = Math.Min(r.top + r.height - 1, _resolution.Y - 1);
		for (int y = start; y <= end; y++)
		{
			_lineToObjects[y].Remove(obj);
			_dirtyLines.Add(y);
		}
	}

	private void RebuildAll()
	{
		AllocateBuffers();
		UpdateTextObjects();   // limpa _registered e reindexa tudo
		MarkAllDirty();        // for y em 0..Y-1: _dirtyLines.Add(y)
		NewUpdateBox();
	}

	private void MarkAllDirty()
	{
		for (int i = 0; i < _screenLines.Length; i++)
		{
			_dirtyLines.Add(i);
		}
	}
	
	private void MarkRangeDirty(int top, int height)
	{
		int start = Math.Max(top, 0);
		int end   = Math.Min(top + height - 1, _resolution.Y - 1);
		for (int y = start; y <= end; y++)
		{
			_dirtyLines.Add(y);
		}
	}
	
	private static void InsertSorted(List<TextObject> list, TextObject obj)
	{
		int pos = list.BinarySearch(obj);
		if (pos >= 0) return; // já está na lista
		list.Insert(~pos, obj);
	}
	
	/// <summary>
	/// Retorna uma lista com todos os filhos do tipo T
	/// </summary>
	/// <typeparam name="T">Tipo de busca</typeparam>
	/// <returns></returns>
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
	
	// -------- Observers ---------------------------------------------------------
	
	private void OnPositionChanged(TextObject obj)
	{
		var r = _registered[obj];
		int h = obj.Sprite?.Length ?? 0;

		if (r.top == obj.ObjectPosition.Y && r.height == h)
		{
			MarkRangeDirty(r.top, r.height);
		}
		else
		{
			RemoveFromLines(obj);
			AddToLines(obj, obj.ObjectPosition.Y, h);
		}
		NewUpdateBox();
	}

	private void OnLayerChanged(TextObject obj)
	{
		// Mesmas linhas, só a ordem dentro delas muda
		var r = _registered[obj];
		int start = Math.Max(r.top, 0);
		int end   = Math.Min(r.top + r.height - 1, _resolution.Y - 1);
		for (int y = start; y <= end; y++)
		{
			_lineToObjects[y].Remove(obj);         // por referência, não depende da ordem
			InsertSorted(_lineToObjects[y], obj);  // reinsere com a camada nova
			_dirtyLines.Add(y);
		}
		NewUpdateBox();
	}

	private void OnSpriteChanged(TextObject obj)
	{
		// A altura pode ter mudado: reindexa completo
		RemoveFromLines(obj);
		AddToLines(obj, obj.ObjectPosition.Y, obj.Sprite?.Length ?? 0);
		NewUpdateBox();
	}

	private void OnChildEntered(Node node)
	{
		if (node is not TextObject obj || !IsNodeReady()) return;
		Subscribe(obj);
		_textObjects.Add(obj);
		AddToLines(obj, obj.ObjectPosition.Y, obj.Sprite?.Length ?? 0);
		NewUpdateBox();
	}

	private void OnChildExiting(Node node)
	{
		if (node is not TextObject obj) return;
		Unsubscribe(obj);
		_textObjects.Remove(obj);
		RemoveFromLines(obj);
		NewUpdateBox();
	}
}
