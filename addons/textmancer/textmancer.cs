#if TOOLS
using Godot;
using System;

[Tool]
public partial class textmancer : EditorPlugin
{
	public override void _EnterTree()
	{
		Script conductor = GD.Load<Script>("res://*/conductor.tscn");
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
