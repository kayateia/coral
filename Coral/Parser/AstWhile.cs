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
/// While loops.
/// </summary>
class AstWhile : AstNode
{
	/// <summary>
	/// The test expression for each iteration.
	/// </summary>
	public AstNode test { get; private set; }

	/// <summary>
	/// The code we'll run per loop.
	/// </summary>
	public AstNode block { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "WhileStmt" )
		{
			// Get our loop variable.
			this.test = c.convertNode( node.ChildNodes[1] );

			// Convert the inner block.
			this.block = c.convertNode( node.ChildNodes[2] );

			return true;
		}

		return false;
	}

	// This IEnumerable implementation will be very inefficient for large arrays.
	void oneIteration( State state )
	{
		state.pushAction( new Step( this, st => 
			{
				object resultObj = LValue.Deref( st );
				bool result = Util.CoerceBool( resultObj );
				if( result )
				{
					st.pushAction( new Step( this, st2 =>
						{
							oneIteration( st2 );
						}, "while: next block runner" ) );
					this.block.run( state );
				}
			}, "while: test checker" ) );
		this.test.run( state );
	}

	const string LoopMarker = "while: loop marker";

	/// <summary>
	/// Returns true if this step represents the marker that ends the whole while loop.
	/// </summary>
	static public bool IsLoopMarker( Step step )
	{
		return step.description == LoopMarker;
	}

	const string BlockMarker = "while: next block runner";

	/// <summary>
	/// Returns true if this step represents the marker that will start a new loop iteration.
	/// </summary>
	/// <remarks>The marker itself should be left on.</remarks>
	static public bool IsBlockMarker( Step step )
	{
		return step.description == BlockMarker;
	}

	public override void run( State state )
	{
		// We execute by first evaluating the test, and then if that's true, pushing one
		// iteration of the loop onto the action stack. Each iteration, if it completes
		// successfully, will push the next on. We also push a while loop marker on so
		// that break and continue work.
		state.pushAction( new Step( this, a => {}, LoopMarker ) );
		oneIteration( state );
	}

	public override string ToString()
	{
		return "<while {0} do {1}>".FormatI( this.test, this.block );
	}
}

}
