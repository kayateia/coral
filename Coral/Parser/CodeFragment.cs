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
/// Code fragment (or errors) returned from compilation.
/// </summary>
public class CodeFragment
{
	internal CodeFragment( AstNode root )
	{
		this.root = root;
		this.errors = null;
	}

	/// <summary>
	/// True if compilation was successful.
	/// </summary>
	public bool success
	{
		get
		{
			return this.errors == null;
		}
	}

	/// <summary>
	/// Represents a single error from compilation.
	/// </summary>
	public class Error
	{
		public int line, col;
		public string message;
	}

	/// <summary>
	/// List of all errors, if any. If we succeeded, this will be null.
	/// </summary>
	public Error[] errors
	{
		get; private set;
	}

	/// <summary>
	/// The name of this compilation unit.
	/// </summary>
	public string unitName
	{
		get; private set;
	}

	/// <summary>
	/// Returns true if this code fragment contains only function defs at its top level.
	/// </summary>
	/// <returns></returns>
	public bool verifyOnlyDefs()
	{
		if( this.root is AstFunc )
			return true;
		else if( this.root is AstStatements )
		{
			var stmts = (AstStatements)this.root;
			return stmts.children.All( s => s is AstFunc );
		}
		else
			return false;
	}

	/// <summary>
	/// If we already have errors, this does nothing. Otherwise if verifyOnlyDefs()
	/// returns false, we switch to an error mode with a single error stating that.
	/// </summary>
	public void errorIfNotOnlyDefs()
	{
		if( !this.success )
			return;

		if( !verifyOnlyDefs() )
		{
			this.errors = new Error[]
			{
				new Error()
				{
					line = 1,
					col = 1,
					message = "Script may not include anything beside function definitions"
				}
			};
		}
	}

	/// <summary>
	/// Returns true if this fragment contains any code at all; false if it's all comments and whitespace.
	/// </summary>
	public bool containsAnyCode
	{
		get
		{
			return this.root != null && this.root is AstStatements && ((AstStatements)this.root).children.Count > 0;
		}
	}

	internal CodeFragment( Irony.Parsing.ParseTree tree, string unitName )
	{
		var errors = new List<Error>();
		foreach( var msg in tree.ParserMessages )
		{
			errors.Add( new Error()
				{
					line = msg.Location.Line + 1,
					col = msg.Location.Column + 1,
					message = msg.Message
				}
			);
		}
		this.errors = errors.ToArray();
		this.unitName = unitName;
	}

	internal CodeFragment( CompilationException ex )
	{
		this.errors = new Error[]
		{
			new Error()
			{
				line = ex.line,
				col = ex.col,
				message = ex.Message
			}
		};
	}

	internal AstNode root { get; set; }
}

}
