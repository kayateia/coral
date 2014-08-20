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
/// A grouping of AstNode statements. These are independent execution units that
/// happen sequentially and which don't affect each other except through
/// modification of the scope.
/// </summary>
class AstStatements : AstNode
{
	/// <summary>
	/// All of our child AstNodes, representing statements.
	/// </summary>
	public IList<AstNode> children {
		get { return _children; }
	}

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name != "StmtList" )
			return false;

		foreach( var child in node.ChildNodes )
		{
			_children.Add( Compiler.ConvertNode( child ) );
		}

		return true;
	}

	public override void run( State state )
	{
		// We execute here by simply "running" each of the steps and letting them
		// push onto the stack. There is no branching possible, and these calls will
		// be shallow / non-blocking, so there's no need to do anything tricky or conditional.
		//
		// One point of interest is that we push a "clear results" step after each statement
		// just to verify that nothing was left on the results stack (this happens and is okay).
		foreach( AstNode c in ((IEnumerable<AstNode>)_children).Reverse() )
		{
			// Clear the result stack after each statement.
			state.pushAction( new Step( this, st2 => st2.clearResults(), "[Clear Results]" ) );
			c.run( state );
		};
	}

	public override string ToString()
	{
		return "<" + String.Join( "><", _children.Select( c => c.ToStringI() ).ToArray() ) + ">";
	}

	List<AstNode> _children = new List<AstNode>();
}

}
