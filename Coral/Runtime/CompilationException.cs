#region License
/*
	CliMOO - Multi-User Dungeon, Object Oriented for the web
	Copyright (C) 2010-2014 Kayateia

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
namespace Kayateia.Climoo.Scripting.Coral
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// C# exception used for compilation errors in Coral. This generally shouldn't
/// happen, and it generally signifies a code error in Coral, but we want to be
/// proper about it and not just crash.
/// </summary>
public class CompilationException : System.Exception
{
	public CompilationException( string message, Irony.Parsing.ParseTreeNode node )
		: base( message )
	{
		this.line = node.Span.Location.Line + 1;
		this.col = node.Span.Location.Column + 1;
	}

	public int line { get; private set; }

	public int col { get; private set; }
}

}
