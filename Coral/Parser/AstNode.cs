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
/// Base class for all AST node types.
/// </summary>
public abstract class AstNode
{
	public AstNode()
	{
	}

	public virtual bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		this.frame = new StackTrace.StackFrame()
		{
			line = node.Span.Location.Line + 1,
			col = node.Span.Location.Column + 1,
			unitName = c.unitName
		};

		return false;
	}

	public virtual void run( State state ) { }

	/// <summary>
	/// Associated stack frame, if any.
	/// </summary>
	/// <remarks>
	/// This is for AstNodes to set; do not edit.
	/// </remarks>
	public StackTrace.StackFrame frame
	{
		get
		{
			if( _frame.isFuncNameDefault )
				_frame.funcName = this.ToStringI();
			return _frame;
		}

		set
		{
			_frame = value;
		}
	}

	StackTrace.StackFrame _frame;

	/// <summary>
	/// True if this node represents a top level statement.
	/// </summary>
	/// <remarks>
	/// This is for AstNodes to set; do not edit.
	/// </remarks>
	public bool statement { get; set; }
}

}
