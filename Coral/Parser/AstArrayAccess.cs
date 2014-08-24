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
/// Represents an array access.
/// </summary>
class AstArrayAccess : AstNode
{
	/// <summary>
	/// The source of the array we're going to index.
	/// </summary>
	public AstNode source { get; private set; }

	/// <summary>
	/// The index within the array we're accessing.
	/// </summary>
	public AstNode index { get; private set; }

	public AstArrayAccess()
	{
	}

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "ArrayAccess" )
		{
			this.source = Compiler.ConvertNode( node.ChildNodes[0] );
			this.index = Compiler.ConvertNode( node.ChildNodes[2] );

			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute by getting the indexer value and the source, looking the
		// value in question up, and then pushing the result on the result stack.
		state.pushAction( new Step( this, st =>
		{
			object source = LValue.Deref( st );
			object index = LValue.Deref( st );

			if( source is Dictionary<object,object> )
			{
				var sourcedict = (Dictionary<object,object>)source;
				st.pushResult( sourcedict[index] );
			}
			else if( source is List<object> && index is int )
			{
				var sourcelist = (List<object>)source;
				int indexint = (int)index;
				st.pushResult( sourcelist[indexint] );
			}
			else if( source is string && index is int )
			{
				st.pushResult( StringObject.ArrayAccess( (string)source, (int)index ) );
			}
			else if( source is MetalObject && ((MetalObject)source).indexLookup != null )
			{
				((MetalObject)source).indexLookup( st, index );
			}
			else
				throw new ArgumentException( "Attempt to index non-list and non-dictionary" );
		} ) );
		this.source.run( state );
		this.index.run( state );
	}

	public override string ToString()
	{
		return "[{0}[{1}]]".FormatI( this.source, this.index );
	}
}

}
