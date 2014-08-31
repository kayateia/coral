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
/// Breaks out of a for loop.
/// </summary>
class AstBreak : AstNode
{
	public override bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "BreakStmt" )
		{
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute here by searching up the stack for the for loop scope
		// marker, then unwinding past that.
		state.pushAction( new Step( this, st =>
		{
			st.unwindActions( step => AstFor.IsScopeMarker( step ) || AstWhile.IsLoopMarker( step ) );
		}, "break: stack unwinder" ) );
	}

	public override string ToString()
	{
		return "<break>";
	}
}

}
