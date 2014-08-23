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

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "MemberAccess" )
		{
			this.rvalue = Compiler.ConvertNode( node.ChildNodes[0] );
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
			object rval = st.popResult();
			if( rval is LValue )
				((LValue)rval).read( st );
			else
				st.pushResult( rval );
			st.pushAction( new Step( this, st2 =>
			{
				object rval2 = st2.popResult();
				if( rval2 is MetalObject )
				{
					// Metal object callbacks produce an LValue automatically.
					((MetalObject)rval2).memberLookup( st2, this.member );
				}
				else
				{
					// We must produce an LValue ourselves, that wraps the dictionary access.
					st2.pushResult( new LValue()
					{
						read = st3 =>
						{
							if( rval2 is Dictionary<object,object> )
							{
								var dict = (Dictionary<object,object>)rval2;
								st3.pushResult( dict[this.member] );
							}
							else
							{
								// TODO: Handle native object accesses.
								throw new ArgumentException( "Can't access value as object" );
							}
						},
						write = ( st3, val ) =>
						{
							if( rval2 is Dictionary<object,object> )
							{
								var dict = (Dictionary<object,object>)rval2;
								dict[this.member] = val;
							}
							else
							{
								// TODO: Handle native object accesses.
								throw new ArgumentException( "Can't access value as object" );
							}
						}
					} );
				}
			} ) );
		} ) );
		this.rvalue.run( state );
	}

	public override string ToString()
	{
		return "[{0}.{1}]".FormatI( this.rvalue, this.member );
	}
}

}
