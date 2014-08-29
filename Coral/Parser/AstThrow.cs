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
/// Throws an exception.
/// </summary>
/// <remarks>
/// You can throw any object (including null), but it is recommended to follow
/// the convention of throwing a dictionary/object with a "name" parameter specifying
/// the exception, and a "message" parameter specifying more info.
/// </remarks>
class AstThrow : AstNode
{
	/// <summary>
	/// Optional parameter with what to throw.
	/// </summary>
	public AstNode param { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "ThrowStmt" )
		{
			if( node.ChildNodes.Count > 1 )
				this.param = Compiler.ConvertNode( node.ChildNodes[1] );
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// If we have a parameter, execute that first.
		state.pushAction( new Step( this, st =>
			AstTry.ThrowException( state, state.popResult() ) ) );
		if( this.param != null )
			this.param.run( state );
		else
			state.pushResult( null );
	}

	public override string ToString()
	{
		if( this.param != null )
			return "<throw {0}".FormatI( this.param );
		else
			return "<throw>";
	}
}

}
