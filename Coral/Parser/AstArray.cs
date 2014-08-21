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
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Represents a code-constructed array.
/// </summary>
class AstArray : AstNode
{
	public List<AstNode> values { get; private set; }

	public AstArray()
	{
		this.values = new List<AstNode>();
	}

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "ArrayExpr" )
		{
			var elems = node.ChildNodes[1];
			if( elems.Term.Name != "ArrayElements" )
				throw new ArgumentException( "Expected ArrayElements" );

			foreach( var child in elems.ChildNodes )
				this.values.Add( Compiler.ConvertNode( child ) );

			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute by pushing all the code for building the values onto the
		// action stack, and finish off with one that will combine them all.
		state.pushAction(
			new Step( this, s =>
			{
				var arr = new List<object>();
				for( int i=0; i<this.values.Count; ++i )
				{
					object value = s.popResult();
					arr.Add( value );
				}

				s.pushResult( arr );
			} )
		);
		foreach( AstNode n in this.values )
			n.run( state );
	}

	public override string ToString()
	{
		return "[{0}]".FormatI(
			String.Join( ",",
				this.values.Select( i => i.ToString() ).ToArray()
			)
		);
	}
}

}
