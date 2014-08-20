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
/// An identifier represents a variable in the scope.
/// </summary>
class AstIdentifier : AstNode
{
	/// <summary>
	/// The identifier's name.
	/// </summary>
	public string name { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "Identifier" )
		{
			this.name = node.Token.Text;
			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute by reading the identifier's value from the scope onto the result stack.
		state.pushAction(
			new Step( this, st => st.pushResult( st.scope.get( this.name ) ) )
		);
	}

	public override string ToString()
	{
		return "[{0}]".FormatI( this.name );
	}
}

}
