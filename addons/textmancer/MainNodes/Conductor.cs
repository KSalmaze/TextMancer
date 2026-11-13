// -----------------------------------------------------------------------
// <copyright file="Conductor.cs" company="Salmaze">
//     Copyright (c) Pedro Henrique Salmaze. All rights reserved.
//     Licensed under the MIT License.
// </copyright>
// <author>Pedro H. Salmaze - pedrohsalmaze@gmail.com</author>
// <date>2026-06-09</date>
// -----------------------------------------------------------------------

using Godot;

namespace TextMancer.addons.textmancer.MainNodes;

[Tool]
public partial class Conductor: Control
{
	[Export] public char TransparentChar = '◙';
}
