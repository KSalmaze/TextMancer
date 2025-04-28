using Godot;

namespace TextMancer.addons.textmancer.MainNodes;

[Tool]
public partial class Conductor: Control
{
	[Export] public bool HyperThreadingEnabled = false;
}
