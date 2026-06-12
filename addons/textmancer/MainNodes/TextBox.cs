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

	//protected string[] _screenLines;
	//protected StringBuilder[] _screenLinesBuilder;
	protected char[][] _screenLines;
	protected char[] _lineCopy; // Todo: deve ser atualizado quando a resolução X é atualizada

	private Dictionary<int, List<TextObject>> linhaParaSprites = new Dictionary<int, List<TextObject>>(); // Lista deve estar ordenada por layer
	private List<int> dirtyLines =  new List<int>();
	
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
	
	public void UpdateBox()
	{
		//resolution = new Vector2I(100, 25);
		GD.Print("UpdateBox");
		
		ClearScreenLines();
		
		//GD.Print($"Text Object Count: {_textObjects.Count}");
		// Para cada objeto
		foreach (TextObject textObject in _textObjects)
		{
			// criar stringBuilder para cada linha da tela e ir atualizando as mesmo conforme necessário
			// Provavelmente deixará o código mais rápido
			// Usar lazy ou o conceito de lazy para criar apenas StringBuilders para linha que precisam

			//GD.Print($"Obj: {textObject.Name}");
			// Para cada linha na sprite do objeto
			for (int j = 0; j < textObject.Sprite.Length; j++)
			{
				// Passando para a próxima linha caso a linha estiver vazia
				if (textObject.Sprite[j] == null) continue;

				int line = textObject.ObjectPosition.Y + j;
				if (line < 0) continue; // Caso a linha esteja a cima da tela, (Quando calcularmos as linhas sujas, isso vai sumir)
				if (line >= resolution.Y) break; // Caso a linha atual esteja abaixo da tela todas as outras vão estar também
				
				//GD.Print($" Screen: {_screenLines.Length} - Pos: {line}");
				char[] temp = _screenLines[line].ToCharArray();


				for(int i = 0; i < textObject.Sprite[j].Length; i++)
				{
					int coord = i + textObject.ObjectPosition.X;
				
					if(coord >= resolution.X) break;
					if(coord < 0 || textObject.Sprite[j][i] == ' ') continue;
					if (textObject.Sprite[j][i] == '£')
					{
						temp[coord] = ' ';
					}
					else
					{
						temp[coord] = textObject.Sprite[j][i];
					}
				}
				
				_screenLines[line] = new string(temp);
			}
		}
		
		// Test performace with string screenText = string.Join('\n', _screenLines);
		Text = String.Empty;
		StringBuilder screen_sb = new StringBuilder();
		//GD.Print(_screenLines.Length);
		foreach (string Line in _screenLines)
		{
			screen_sb.AppendLine(Line);
		}
		
		Text = $"[font_size={TextSize}]" + screen_sb + "[/font_size]";
		//GD.Print(_screenLines.Length);
	}

	// Uma função tão sensivel ser Public?
	public void NewUpdateBox()
	{
		foreach (int line in dirtyLines)
		{
			ClearScreenLine(line);

			foreach(TextObject textObject in linhaParaSprites[line])
			{
				string spriteLine = textObject.Sprite[line - textObject.GlobalObjectPosition.Y];
				for (int i = 0; i < spriteLine.Length - 1; i++)
				{
					if(textObject.GlobalObjectPosition.X + i > resolution.X) break;
					_screenLines[line][i] = spriteLine[i];
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
	
	protected void UpdateTextObjects()
	{
		_textObjects = GetChilds<TextObject>();

		if (_textObjects.Count == 0) return;
			
		foreach (var to in _textObjects)
		{
			to.TextObjectChanged += UpdateBox;
		}
		GD.Print(_textObjects.Count);
		_textObjects.Sort();
		UpdateBox();// vai mudar pra textObjectChanged
	}

	protected void OnTextObjectChanged(int oldGlobalPositionY, TextObject textObject)
	{
		int spriteHeight = textObject.Sprite.Length;

		int oldStart = oldGlobalPositionY;
		int oldEnd = oldGlobalPositionY + spriteHeight - 1;
		int newStart = textObject.ObjectPosition.Y;
		int newEnd = newStart + spriteHeight - 1;

		// Range A: oldGlobalPositionY e oldGlobalPositionY + textObject.Sprite.Lenght
		// Range B: textObject.GlobalObjectPosition.Y e textObject.GlobalObjectPosition.Y + textObject.Sprite.Lenght

		// Set lines as dirt
		// tudo no Range A
		// tudo no Range B

		// Update line to sprites dictionary;
		// para o Range A: Dict<i>.Remove(textObject);
		// para o Range B: Dict<i>.Add(textObject); 
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
