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

/// <summary>
/// Functions. This includes both 'def' style and lambda style.
/// </summary>
class AstFunc : AstNode
{
	/// <summary>
	/// The function's name. This may be syntax junk (related to line numbers) for lambdas.
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// The names of the function's parameters.
	/// </summary>
	public string[] parameters { get; private set; }

	/// <summary>
	/// The code we'll run.
	/// </summary>
	public AstNode block { get; private set; }

	/// <summary>
	/// True if we're a lambda function.
	/// </summary>
	public bool lambda { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "FunctionDef" )
		{
			this.name = node.ChildNodes[1].Token.Text;
			this.parameters = node.ChildNodes[2].ChildNodes.Select( n => n.Token.Text ).ToArray();
			this.block = Compiler.ConvertNode( node.ChildNodes[3] );
			this.lambda = false;

			return true;
		}
		// TODO: Lambdas

		return false;
	}

	public override void run( State state )
	{
		// How we execute depends on whether we're a lambda or not. In the lambda case, we
		// just push an FValue on the result stack and call it done. For the non-lambda case,
		// we create a variable in the scope with an FValue in it. In either case, there's not
		// much actual executing being done here.
		if( this.lambda )
		{
			state.pushAction( new Step( this, st => st.pushResult( new FValue( this ) ) ) );
		}
		else
		{
			state.pushAction( new Step( this, st => st.scope.set( this.name, new FValue( this ) ) ) );
		}
	}

	public override string ToString()
	{
		return "<def {0}({1}): {2}>".FormatI(
			this.name,
			String.Join( ",", this.parameters ),
			this.block
		);
	}
}

}
