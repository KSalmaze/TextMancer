using Godot;

namespace TextMancer.TextMancer.MainNodes;

public partial class Conductor: Control
{
	private string _nome;
	public string Nome
	{
		get { return _nome; }
		set { _nome = value; }
	}
}
