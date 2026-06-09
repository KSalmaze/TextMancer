using Godot;

namespace TextMancer.addons.textmancer.MainNodes;

[Tool]
public partial class Conductor: Control
{
	/// <summary>
	/// Indica se o processamento gráfico preditivo será ou não utilizado para TextBoxs filhas.
	/// Mais informações disponível na documentação: <link:HYPERTHREADING>
	/// </summary>
	[Export] public bool HyperThreadingEnabled = false;
	// Todo : Renomear variavel
	// Todo : 
}
