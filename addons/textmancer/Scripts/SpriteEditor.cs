// -----------------------------------------------------------------------
// <copyright file="SpriteEditor.cs" company="Salmaze">
//     Copyright (c) Pedro Henrique Salmaze. All rights reserved.
//     Licensed under the MIT License.
// </copyright>
// <author>Pedro H. Salmaze - pedrohsalmaze@gmail.com</author>
// <date>2026-06-09</date>
// -----------------------------------------------------------------------

using Godot;
using System.Linq;

namespace TextMancer.addons.textmancer.Scripts;

[Tool]
public partial class SpriteEditor: TextBox
{
	public TextObject SpriteObject;
	[Export] public bool testingTwo
	{
		get => false;
		set => _Ready();
	}
	
	public override void _Ready()
	{
		base._Ready();
		SpriteObject = new TextObject();
		SpriteObject.Sprite = new[] { "SPR£TE" };
		AddChild(SpriteObject);

		_inputX = GetNode<LineEdit>("../../XLineEdit");
		_inputY = GetNode<LineEdit>("../../YLineEdit");
		_gridSizeButton = GetNode<Button>("../../RedimencionarButton");
		_gridSizeButton.Pressed += UpdateGridSize;

		EditButton = GetNode<Button>("../../EditarButton");
		EditButton.Pressed += EditingClick;
		
		_increaseTextButton = GetNode<Button>("../../FMaisButton");
		_decreaseTextButton = GetNode<Button>("../../FMenosButton");

		_increaseTextButton.Pressed += IncreaseTextSize;
		_decreaseTextButton.Pressed += DecreaseTextSize;
		
		GetNode<Button>("../../RIButton").Pressed += () => { SpriteObject.ObjectPosition += new Vector2I(1, 0); };
		GetNode<Button>("../../LEButton").Pressed += () => { SpriteObject.ObjectPosition += new Vector2I(-1, 0); };
		GetNode<Button>("../../UPButton").Pressed += () => { SpriteObject.ObjectPosition += new Vector2I(0, -1); };
		GetNode<Button>("../../DOWNButton").Pressed += () => { SpriteObject.ObjectPosition += new Vector2I(0, 1); };
	}

	// SOLUÇÂO TEMPORÁRIA
	// Espaço real: £ { Ctrl + Alt + 4 }
	
	// Primeira parte
	
	// Botao inciar captura de caracteres
	public Button EditButton;
	public bool _captura = false;
	public TextObject Selector;

	public void EditingClick()
	{
		if (_captura)
		{
			EditButton.Text = "Editar Sprite";
			Selector.QueueFree();
			_captura = false;
			return;
		}

		_captura = true;
		//CaptureKeys();
		Selector = new TextObject();

		Selector.Sprite = new[]{"S"};//{"\u2588"};
		Selector.Position = new Vector2(0, 0);
		AddChild(Selector);
		EditButton.Text = "Parar Edição";
		UpdateTextObjects();
	}
	
	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (!_captura) return;
	
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			// Verificar ações mapeadas
			if (Input.IsActionJustPressed("ui_left"))
			{
				Selector.ObjectPosition += new Vector2I(-1, 0);
				UpdateTextObjects();
			}
			else if (Input.IsActionJustPressed("ui_right"))
			{
				Selector.ObjectPosition += new Vector2I(1, 0);
				UpdateTextObjects();
			}
			else if (Input.IsActionJustPressed("ui_up"))
			{
				Selector.ObjectPosition += new Vector2I(0, -1);
				UpdateTextObjects();
			}
			else if (Input.IsActionJustPressed("ui_down"))
			{
				Selector.ObjectPosition += new Vector2I(0, 1);
				UpdateTextObjects();
			}
			else
			{
				// Processar caracteres normais
				char caractere = (char)keyEvent.Unicode;
				if (char.IsLetterOrDigit(caractere) || char.IsPunctuation(caractere) || 
					char.IsSymbol(caractere) || caractere == ' ')
				{
					GD.Print($"Caractere digitado: '{caractere}'");
					
					char[] line = _screenLines[Selector.ObjectPosition.Y].ToCharArray();
					line[Selector.ObjectPosition.X] = caractere;
					_screenLines[Selector.ObjectPosition.Y] = new string(line);
					
					ExtrairSprite();
					// Extrair SPRITE e atualizar sprite do OBJ
					
					Selector.ObjectPosition += new Vector2I(1, 0);
					UpdateTextObjects();
				}
			}
		}
	}

	private void ExtrairSprite()
	{
		

		string[] novaSprite = _screenLines
			.Where(linha => linha.Any(c => c != '.'))
			.Select(linha => linha.Replace('.', ' ').Trim())
			.Where(linha => !string.IsNullOrWhiteSpace(linha))
			.ToArray();

		GD.Print("Nova Sprite:");
		
		foreach (string line in novaSprite)
		{
			GD.Print($"->{line}<-");
		}

		// Printar sprite

		// Limpeza final
	}
	
	// Botao Redimencionar malha
	private LineEdit _inputX;
	private LineEdit _inputY;
	private Button _gridSizeButton;
	public void UpdateGridSize()
	{
		GD.Print($"UpdateGridSize: X:{_inputX.Text} Y:{_inputY.Text}");
		int x = int.Parse(_inputX.Text);
		int y = int.Parse(_inputY.Text);
		resolution = new Vector2I(x, y);
		UpdateTextObjects();
	}
	
	// Aumentar e diminuir fonte
	private Button _increaseTextButton;
	private Button _decreaseTextButton;
	public void IncreaseTextSize()
	{
		TextSize++;
	}
	public void DecreaseTextSize()
	{
		if (TextSize > 1)
		{
			TextSize--;
		}
	}
	
	// Botao Limpar
	public void ClearSprite()
	{
		SpriteObject.Sprite = new[] { "X" };
		UpdateTextObjects();
	}
	
	// Botao Salvar
	public void SaveSprite()
	{
		// Encontrar a pasta de sprites (Pegar pelo editor fds)
		
		// Processamento
		// - Redizir ao máxima
		// - Trocar pontos por £
		
		// Criar o JSON e salvar
	}
	
	// Segunda parte
	// Trocar para collider
}
