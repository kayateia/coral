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
/// Return early (or with value) from function calls.
/// </summary>
class AstReturn : AstNode
{
	/// <summary>
	/// The value we'll return (or null). Note that functions in Coral
	/// always return a value on the value stack, even if it's null.
	/// </summary>
	public AstNode value { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "ReturnStmt" )
		{
			this.value = Compiler.ConvertNode( node.ChildNodes[1] );
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute here by searching up the stack for the previous function call
		// scope marker, and unwinding past that, then pushing on our return value.
		state.pushAction( new Step( this, st => 
		{
			st.pushAction( new Step( this, st2 =>
			{
				// Make sure we don't leak LValues.
				st2.pushResult( LValue.Deref( st2 ) );

				// And do the stack unwind.
				st2.unwindActions( step => AstCall.IsScopeMarker( step ) );
			}, "return: stack unwinder" ) );

			if( this.value != null )
				this.value.run( st );
			else
				st.pushResult( null );
		} ) );
	}

	public override string ToString()
	{
		if( this.value != null )
			return "<return {0}>".FormatI( this.value );
		else
			return "<return>";
	}
}

}
