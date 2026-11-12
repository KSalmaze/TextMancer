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
	[Export] public Vector2I resolution; // Bloquear negativos
	[Export] public char baseChar = '.';
	[Export] public int FrameRate = 15;
	private int _textSize = 10;
	[Export] public int TextSize {
		get => _textSize;
		set
		{
			if(value > 1){
				_textSize = value;
				UpdateTextObjects();
			}
		}
	} 
	// Adicionar Seter
	public List<TextObject> _textObjects;
	private double _timeSinceLastUpdate = 0;
	[Export] public bool testing
	{
		get => false;
		set => UpdateTextObjects();
	}

	[Export] public char emptySpaceChar = '◙';

	//protected string[] _screenLines;
	//protected StringBuilder[] _screenLinesBuilder;
	protected char[][] _screenLines;
	protected char[] _lineCopy; // Todo: deve ser atualizado quando a resolução X é atualizada

	/// <summary>
	/// Dictionary of layer to dictionary of line to List of TextObjets that the sprite is in that line and exactly layer.
	/// </summary>
	private Dictionary<int, Dictionary<int, List<TextObject>>> _layerToLinesToSprite = new Dictionary<int, Dictionary<int, List<TextObject>>>();

	private HashSet<int> dirtyLines =  new HashSet<int>();
	
	// OPTIMIZATION UPDATE

	//public Dictionary<int, List<TextObject>> TextObjects; // Dicionário : Linha -> sprites nesse linha
	// Dicinionário : Layer -> Dicionário : Linha -> sprites nessa linha e layer
	
	/*
	 *Nova função:
	 *
	 * -- Verificar quais linhas atualizar
	 * -- Para cada linhas necessária a atualização
	 *		-- Acessar as sprites nas linhas
	 *      -- Acessar apenas a linhas global necessária e atualizar a _screenLines certa
	 *
	 * -- Atualizar a tela

	  MAIS IDEIAS:
	  - Criar antes do processamento os sb
	  - Estrutura de dados onde multiplas chaves apontem para o mesmo objeto, para salvar posição X,Y -> Sprites
	 */
	
	// OPTIMIZATION UPDATE
	
	public override void _Ready()
	{
		UpdateTextObjects();

		// _lineCopy initialization
		_lineCopy = new string(baseChar, resolution.X).ToCharArray();
	}
	
	public override void _Process(double delta)
	{
	}
	
	private void NewUpdateBox()
	{
		foreach (int line in dirtyLines)
		{
			ClearScreenLine(line);

			foreach(TextObject textObject in linhaParaSprites[line])
			{
				string spriteLine = textObject.Sprite[line - textObject.ObjectPosition.Y];
				for (int i = 0; i < spriteLine.Length; i++)
				{
					if(textObject.ObjectPosition.X + i > resolution.X) break;
					if (spriteLine[i] == emptySpaceChar) continue;
					_screenLines[line][textObject.ObjectPosition.X + i] = spriteLine[i];
				}
			}
		}

		dirtyLines.Clear();
	}

	private void ClearScreen()
	{
		for (int i = 0; i < resolution.Y; i++)
		{
			ClearScreenLine(i);
		}
	}

	private void ClearScreenLine(int line)
	{
		Array.Copy(_lineCopy,_screenLines[line], resolution.X);
	}
	
	// Sempre que um TextObject vou incluido ou removido chamar essa função
	
	// Functions related to update of TextObjects

	protected void UpdateTextObjects()
	{
		_textObjects = GetChilds<TextObject>();

		if (_textObjects.Count == 0) return;
			
		foreach (var to in _textObjects)
		{
			to.OnTextObjectChangedPosition -= OnTextObjectPositionChanged;
			to.OnTextObjectChangedPosition += OnTextObjectPositionChanged;
		}
		GD.Print(_textObjects.Count);
		_textObjects.Sort();
		NewUpdateBox(); // É necessário
	}

	protected void OnTextObjectPositionChanged(int oldGlobalPositionY, TextObject textObject)
	{
		// Todo: quando a sprite nova mudar a quantidade de linhas, enviar também a quantidade de linhas antiga?
		// Dois sinais diferentes devem funcionr melhor, mais sinal em caso de mudança de layer
		int spriteHeight = textObject.Sprite.Length;

		int oldStart = oldGlobalPositionY;
		int oldEnd = oldGlobalPositionY + spriteHeight - 1;
		int newStart = textObject.ObjectPosition.Y;
		int newEnd = newStart + spriteHeight - 1;
	
		// Mark range as dirty and updating 
		for (int i = oldStart; i <= oldEnd; i++){
			dirtyLines.Add(i);
			_layerToLinesToSprite[textObject.Layer][i].Remove(textObject);
		}
		for (int i = newStart; i <= newEnd; i++){
			dirtyLines.Add(i);
			_layerToLinesToSprite[textObject.Layer][i].Add(textObject);
		}

		NewUpdateBox();
	}
	/*
	METHOD TextObjectChanged(oldGlobalPositionY, textObject)
    h = textObject.Sprite.Length
    newY = textObject.GlobalObjectPosition.Y

    oldStart = oldGlobalPositionY
    oldEnd   = oldGlobalPositionY + h - 1

    newStart = newY
    newEnd   = newY + h - 1

    MarkRangeDirty(min(oldStart, newStart), max(oldEnd, newEnd))

    overlapStart = max(oldStart, newStart)
    overlapEnd   = min(oldEnd, newEnd)

    IF oldStart < overlapStart
        RemoveObjectFromRange(oldStart, overlapStart - 1, textObject)

    IF overlapEnd < oldEnd
        RemoveObjectFromRange(overlapEnd + 1, oldEnd, textObject)

    IF newStart < overlapStart
        AddObjectToRange(newStart, overlapStart - 1, textObject)

    IF overlapEnd < newEnd
        AddObjectToRange(overlapEnd + 1, newEnd, textObject)
	*/
	
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
}
