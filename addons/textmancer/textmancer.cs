// -----------------------------------------------------------------------
// <copyright file="textmancer.cs" company="Salmaze">
//     Copyright (c) Pedro Henrique Salmaze. All rights reserved.
//     Licensed under the MIT License.
// </copyright>
// <author>Pedro H. Salmaze - pedrohsalmaze@gmail.com</author>
// <date>2026-06-09</date>
// -----------------------------------------------------------------------

#if TOOLS
using Godot;
using System;

[Tool]
public partial class textmancer : EditorPlugin
{
	public override void _EnterTree()
	{
		Script conductor = GD.Load<Script>("res://addons/textmancer/MainNodes/Conductor.cs");
		Texture2D conductorTexture = GD.Load<Texture2D>("res://addons/textmancer/MainNodesSprites/Conductor.png");
		
		Script textbox = GD.Load<Script>("res://addons/textmancer/MainNodes/TextBox.cs");
		Texture2D textboxTexture = GD.Load<Texture2D>("res://addons/textmancer/MainNodesSprites/TextBox.png");

		Script textObject = GD.Load<Script>("res://addons/textmancer/MainNodes/TextObject.cs");
		Texture2D textObjectTexture = GD.Load<Texture2D>("res://addons/textmancer/MainNodesSprites/TextObject.png");
		
		AddCustomType("Conductor", "Control", conductor, conductorTexture);
		AddCustomType("TextBox", "RichTextLabel", textbox, textboxTexture);
		AddCustomType("TextObject", "Control", textObject, textObjectTexture);
	}

	public override void _ExitTree()
	{
		RemoveCustomType("Conductor");
		RemoveCustomType("TextBox");
		RemoveCustomType("TextObject");
	}
}
#endif
