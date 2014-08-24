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
///
/// TODO: This is not currently working.
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

	// Takes care of LValue resolution.
/*	void readLValues( State state, ActionCallback next )
	{
		state.pushAction( new Step( this, st =>
		{
			// This will process once all three are done.
			st.pushAction( new Step( this, next ) );

			// Pull all three values off.
			object[] items = new object[] {
				st.popResult(),
				st.popResult(),
				st.popResult()
			};

			// Check if they're LValues and act appropriately.
			for( int i=2; i>=0; --i )
			{
				if( items[i] is LValue )
					((LValue)items[i]).read( st );
				else
					st.pushResultPusher( this, items[i] );
			}
		} ) );
	}

	void runCommon( State state )
	{
		readLValues( state, st2 =>
		{
			object source = st2.popResult();
			int begin = Util.CoerceNumber( st2.popResult() );
			int end = Util.CoerceNumber( st2.popResult() );

			if( source is List<object> )
			{
				var slist = (List<object>)source;
				Util.ArraySlice( slist.Count, begin, end, out begin, out end );
				var result = new List<object>( slist.Skip( begin ).Take( end - begin ) );
				st2.pushResult( result );
			}
			else if( source is string )
			{
				st2.pushResult( StringObject.ArraySlice( (string)source, begin, end ) );
			}
			else
				throw new ArgumentException( "Can't array slice this type" );
		} );
	}
*/
	public override void run( State state )
	{
/*		runCommon( state );

		// These are different enough that we just break them out.
		this.source.run( state );
		if( this.begin != null && this.end != null )
		{
			this.begin.run( state );
			this.end.run( state );
		}
		else if( this.begin == null )
		{
			state.pushResultPusher( this, null );
			this.end.run( state );
		}
		else
		{
			this.begin.run( state );
			state.pushResultPusher( this, null );
		} */
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
