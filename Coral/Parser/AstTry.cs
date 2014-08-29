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
/// Try/Except/Finally exception handling.
/// </summary>
class AstTry : AstNode
{
	/// <summary>
	/// The code we'll run in the try block.
	/// </summary>
	public AstNode tryBlock { get; private set; }

	/// <summary>
	/// The code we'll run in the except block.
	/// </summary>
	public AstNode exceptBlock { get; private set; }

	/// <summary>
	/// The name of the identifier that will take on the throw exception, if any.
	/// </summary>
	public string exceptIdentifer { get; private set; }

	/// <summary>
	/// The code we'll run in the finally block, if any.
	/// </summary>
	public AstNode finallyBlock { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "TryStmt" )
		{
			this.tryBlock = Compiler.ConvertNode( node.ChildNodes[1] );

			if( node.ChildNodes.Count == 2 )
				throw new InvalidOperationException( "Can't have a try block without except/finally" );

			var except = node.ChildNodes[2];
			if( except.ChildNodes.Count == 2 )
				this.exceptBlock = Compiler.ConvertNode( except.ChildNodes[1] );
			else
			{
				this.exceptIdentifer = except.ChildNodes[1].Token.Text;
				this.exceptBlock = Compiler.ConvertNode( except.ChildNodes[2] );
			}

			if( node.ChildNodes.Count > 3 )
			{
				var fin = node.ChildNodes[3];
				this.finallyBlock = Compiler.ConvertNode( fin.ChildNodes[1] );
			}

			return true;
		}

		return false;
	}

	const string TryMarker = "try: stack unwind marker";

	static bool IsTryMarker( Step step )
	{
		return step.description == TryMarker;
	}

	/// <summary>
	/// Throws an exception.
	/// </summary>
	/// <param name="thrown">
	/// The object to throw. It's recommended that this be a dictionary with a
	/// "name" parameter at the least.
	/// </param>
	public void throwException( State state, object thrown )
	{
		state.pushAction( new Step( this, st =>
			{
				// We'll want to hold on to the finally runner if there is one. This will
				// let us properly handle nested exceptions.
				Step finallyStep = null;
				st.unwindActions( step => IsTryMarker( step ), this.finallyBlock != null );
				if( this.finallyBlock != null )
					finallyStep = st.popAction();

				// The except block may have a parameter.
				if( this.exceptIdentifer != null )
				{
					IScope exceptScope = new ParameterScope( st.scope, new string[] { this.exceptIdentifer } );
					exceptScope.set( this.exceptIdentifer, thrown );
					state.pushActionAndScope( new Step( this, a => {}, "except: scope" ), exceptScope );
				}
				this.exceptBlock.run( st );

				if( finallyStep != null )
				{
					finallyStep.description = "finally block";
					st.pushAction( finallyStep );
				}
			},
			"throw" )
		);
	}

	/// <summary>
	/// Works like throwException(), except we search for the right try clause.
	/// </summary>
	static public void ThrowException( State state, object thrown )
	{
		Step tryStep = state.findAction( step => IsTryMarker( step ) );
		if( tryStep == null )
			// TODO: make this include a stack trace.
			throw new CoralException( thrown );

		AstTry node = (AstTry)tryStep.node;
		node.throwException( state, thrown );
	}

	public override void run( State state )
	{
		// We execute by pushing on a stack unwind marker step, then executing our
		// try block. If nothing happens, the unwind marker will execute any finally block.
		// If an exception is thrown, then we'll unwind the stack until we hit the stack
		// marker, then execute any except block. We'll then execute any finally block.
		//
		// Because exceptions can happen deep in the stack and/or from random sources,
		// we provide a throwException method to take care of the dirty work.
		state.pushAction( new Step( this, a =>
			{
				if( this.finallyBlock != null )
					this.finallyBlock.run( state );
			},
			TryMarker )
		);
		this.tryBlock.run( state );
	}

	public override string ToString()
	{
		string rv = "<try: {0}".FormatI( this.tryBlock );
		if( this.exceptIdentifer != null )
			rv += " except {0}: {1}".FormatI( this.exceptIdentifer, this.exceptBlock );
		else
			rv += " except";
		if( this.finallyBlock != null )
			rv += " finally: {0}".FormatI( this.finallyBlock );
		rv += ">";

		return rv;
	}
}

}
