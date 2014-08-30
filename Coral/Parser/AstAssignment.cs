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
using Irony.Parsing;

/// <summary>
/// Represents an assignment of some value to a variable.
/// </summary>
class AstAssignment : AstNode
{
	/// <summary>
	/// The "left hand side", or LValue.
	/// </summary>
	public AstNode lhs { get; private set; }

	/// <summary>
	/// The "right hand side", or RValue.
	/// </summary>
	public AstNode rhs { get; private set; }

	public override bool convert( ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "AssignmentStmt" )
		{
			this.lhs = c.convertNode( node.ChildNodes[0] );
			this.rhs = c.convertNode( node.ChildNodes[2] );
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute by executing the "lhs" and "rhs" code, then using the lhs's
		// LValue callbacks to set the value.
		state.pushAction(
			new Step( this, s =>
			{
				LValue lv = (LValue)s.popResult();
				object rv = LValue.Deref( s );
				lv.write( s, rv );
			} )
		);
		this.lhs.run( state );
		this.rhs.run( state );
	}

	public override string ToString()
	{
		return "<{0} = {1}>".FormatI( this.lhs, this.rhs );
	}
}

}
