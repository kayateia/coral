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

/// <summary>
/// Access of a member variable. This is different from a simple identifier
/// because it is a multi-stage process more like an expression than a simple name.
/// </summary>
class AstMemberAccess : AstNode
{
	/// <summary>
	/// The RValue we must evaluate to get to the object whose member we want to access.
	/// </summary>
	public AstNode rvalue { get; private set; }

	/// <summary>
	/// The name of the member to access.
	/// </summary>
	public string member { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "MemberAccess" )
		{
			if( node.ChildNodes[0].Token.Text == "$" )
				this.rvalue = new AstIdentifier( "$" );
			else
				this.rvalue = c.convertNode( node.ChildNodes[0] );
			this.member = node.ChildNodes[2].Token.Text;
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute by evaulating the RValue portion first, then producing an LValue. If
		// the RValue is itself an LValue, we need to deref that first.
		state.pushAction( new Step( this, st =>
		{
			object rval = LValue.Deref( st );
			if( rval is MetalObject )
			{
				// Metal object callbacks produce an LValue automatically.
				((MetalObject)rval).memberLookup( st, this.member );
			}
			else
			{
				// We must produce an LValue ourselves, that wraps the dictionary access.
				st.pushResult( new LValue()
				{
					read = st3 =>
					{
						if( rval is Dictionary<object,object> )
						{
							var dict = (Dictionary<object,object>)rval;
							if( dict.ContainsKey( this.member ) )
								return dict[this.member];
							else
								return null;
						}
						else if( rval is List<object> )
						{
							if( this.member == "length" )
							{
								return new FValue( (st4, args) =>
									{
										st4.pushResult( ((List<object>)rval).Count );
									}
								);
							}
							else
								return null;
						}
						else if( rval is string )
						{
							return StringObject.Method( st3, (string)rval, this.member );
						}
						else
						{
							return null;
						}
					},
					write = ( st3, val ) =>
					{
						if( rval is Dictionary<object,object> )
						{
							var dict = (Dictionary<object,object>)rval;
							dict[this.member] = val;
						}
						else
						{
							throw CoralException.GetArg( "Can't access value as object for write" );
						}
					}
				} );
			}
		} ) );
		this.rvalue.run( state );
	}

	public override string ToString()
	{
		return "[{0}.{1}]".FormatI( this.rvalue, this.member );
	}
}

}
