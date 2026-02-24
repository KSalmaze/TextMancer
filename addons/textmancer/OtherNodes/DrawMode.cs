using System;
using System.Linq;
using Godot;

namespace TextMancer.addons.textmancer.OtherNodes;

[Tool]
public partial class DrawMode: TextObject
{
	private string[] _fullSprite;
	
	public enum Mode
	{
		Vertical_TB,
		Diagonal_TL,
	}
	[Export] public Mode mode = Mode.Vertical_TB;
	[Export] public float tickTime = 0.5f;

	public override void _Ready()
	{
		GD.Print("DrawMode");
	
		// Conectar para testar
		if (HasSignal(TextObject.SignalName.TextObjectChanged))
		{
			Connect(TextObject.SignalName.TextObjectChanged, Callable.From(() => GD.Print("Signal emitido!")));
		}
		
		base._Ready();
		if(!Engine.IsEditorHint()){
			GD.Print("No EditorHint");
			_fullSprite = (string[])Sprite.Clone();
			Array.Clear(Sprite);
			EmitSignal(TextObject.SignalName.TextObjectChanged);
			Draw();
		}
	}

	public async void Draw()
	{
		GD.Print("Draw");
		switch (mode)
		{
			case Mode.Vertical_TB:
				Vertical_Top_Bottom();
				break;
			case Mode.Diagonal_TL:
				Diagonal_Top_Left();
				break;
		}
	}

	private async void Vertical_Top_Bottom()
	{
		int i = 0;
		while (i < _fullSprite.Length)
		{
			GD.Print($"Iteraction {i}");
			Sprite[i] = (string)_fullSprite[i].Clone();
			
			EmitSignal(TextObject.SignalName.TextObjectChanged);
			i++;
			await ToSignal(GetTree().CreateTimer(tickTime), SceneTreeTimer.SignalName.Timeout);
		}
	}
	
	private async void Diagonal_Top_Left()
	{
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
	}
	
	// EXPORT
	// Enum drawmode ("Horizontal L / R", "Vertical T / B", "Diagonal TL")
	// Tick Delay
	// Steps per tick
	
	// Sinais
	// OnStarDraw
	// OnEndDraw
}
