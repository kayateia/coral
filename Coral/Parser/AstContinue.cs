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

/// <summary>
/// Loops back to the beginning of a for loop.
/// </summary>
class AstContinue : AstNode
{
	public override bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "ContinueStmt" )
		{
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute here by searching up the stack for the for loop block
		// marker, then unwinding up to it.
		state.pushAction( new Step( this, st =>
		{
			st.unwindActions( step => AstFor.IsBlockMarker( step ) || AstWhile.IsBlockMarker( step ), true );
		}, "continue: stack unwinder" ) );
	}

	public override string ToString()
	{
		return "<continue>";
	}
}

}
