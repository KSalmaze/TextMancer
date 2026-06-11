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
	[Export] public string baseChar = ".";
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

	protected string[] _screenLines;
	
	// OPTIMIZATION UPDATE

	public Dictionary<int, List<TextObject>> TextObjects; // Dicionário : Linha -> sprites nesse linha
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

	private void ClearScreenLines()
	{
		_screenLines = new string[resolution.Y];
		string baseLine = new string(baseChar[0], resolution.X);

		for (int i = 0; i < resolution.Y; i++)
		{
			_screenLines[i] = baseLine;
		}
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
