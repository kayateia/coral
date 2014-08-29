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
class AstArraySlice : AstNode
{
	/// <summary>
	/// The source of the array we're going to index.
	/// </summary>
	public AstNode source { get; private set; }

	/// <summary>
	/// The begin index for slices. May be null.
	/// </summary>
	public AstNode begin { get; private set; }

	/// <summary>
	/// The end index for slices. May be null.
	/// </summary>
	public AstNode end { get; private set; }

	public AstArraySlice()
	{
	}

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "ArraySliceFull" )
		{
			this.source = Compiler.ConvertNode( node.ChildNodes[0] );
			this.begin = Compiler.ConvertNode( node.ChildNodes[2] );
			this.end = Compiler.ConvertNode( node.ChildNodes[3] );

			return true;
		}
		else if( node.Term.Name == "ArraySliceFromStart" )
		{
			this.source = Compiler.ConvertNode( node.ChildNodes[0] );
			this.end = Compiler.ConvertNode( node.ChildNodes[2] );

			return true;
		}
		else if( node.Term.Name == "ArraySliceFromEnd" )
		{
			this.source = Compiler.ConvertNode( node.ChildNodes[0] );
			this.begin = Compiler.ConvertNode( node.ChildNodes[2] );

			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		state.pushAction( new Step( this, st =>
		{
			object source = LValue.Deref( st );
			object obegin = LValue.Deref( st );
			object oend = LValue.Deref( st );

			int? begin = obegin == null ? (int?)null : Util.CoerceNumber( obegin );
			int? end = oend == null ? (int?)null : Util.CoerceNumber( oend );

			if( source is List<object> )
			{
				var slist = (List<object>)source;
				int ibegin, iend;
				Util.ArraySlice( slist.Count, begin, end, out ibegin, out iend );
				var result = new List<object>( slist.Skip( ibegin ).Take( iend - ibegin ) );
				st.pushResult( result );
			}
			else if( source is string )
			{
				st.pushResult( StringObject.ArraySlice( (string)source, begin, end ) );
			}
			else
				throw CoralException.GetArg( "Can't array slice this type" );
		} ) );

		// These are different enough that we just break them out.
		this.source.run( state );
		if( this.begin != null && this.end != null )
		{
			this.begin.run( state );
			this.end.run( state );
		}
		else if( this.begin == null )
		{
			state.pushAction( new Step( this, st => st.pushResult( null ), "slice: null pusher" ) );
			this.end.run( state );
		}
		else
		{
			this.begin.run( state );
			state.pushAction( new Step( this, st => st.pushResult( null ), "slice: null pusher" ) );
		}
	}

	public override string ToString()
	{
		if( this.begin != null && this.end != null )
			return "[{0}[{1}:{2}]]".FormatI( this.source, this.begin, this.end );
		else if( this.begin == null )
			return "[{0}[:{1}]]".FormatI( this.source, this.end );
		else
			return "[{0}[{1}:]]".FormatI( this.source, this.begin );
	}
}

}
