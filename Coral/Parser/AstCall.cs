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
/// Function/method/lambda calls.
/// </summary>
class AstCall : AstNode
{
	public AstCall() { }

	public AstCall( AstNode source, AstNode[] parameters, StackTrace.StackFrame frame )
	{
		this.source = source;
		this.parameters = parameters;
		this.frame = frame;
	}

	/// <summary>
	/// The function's source expression. This may be an identifier or an expression, and
	/// it must result in an FValue.
	/// </summary>
	public AstNode source { get; private set; }

	/// <summary>
	/// The function call's parameters.
	/// </summary>
	public AstNode[] parameters { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "FunctionCall" )
		{
			this.source = c.convertNode( node.ChildNodes[0] );
			this.parameters = node.ChildNodes[1].ChildNodes.Select( n => c.convertNode( n ) ).ToArray();

			return true;
		}

		return false;
	}

	const string ScopeMarker = "call: scope";

	/// <summary>
	/// Returns true if the specified step is a function call scope marker.
	/// </summary>
	static public bool IsScopeMarker( Step s )
	{
		return s.description == ScopeMarker;
	}

	public override void run( State state )
	{
		// We execute here by evaulating each of the parameters as well as the source, and then
		// evaluating the source FValue; of course if we don't get an FValue, that's an error.
		state.pushAction( new Step( this, st =>
		{
			// Make sure the FValue isn't an LValue.
			object fvo = st.popResult();
			if( fvo is FValue )
			{
				// Everything's okay
			}
			else if( fvo is LValue )
				fvo = LValue.Deref( st, fvo );
			else
				throw CoralException.GetArg( "Attempted call to non-function" );
			if( fvo == null )
				throw CoralException.GetArg( "Attempted call to null function" );
			FValue fv = (FValue)fvo;

			// Gather arguments before we push any scopes.
			var argsArray = new List<object>();
			if( fv.func != null )
			{
				for( int i=0; i<this.parameters.Length; ++i )
				{
					object value = LValue.Deref( st );
					argsArray.Add( value );
				}
			}
			else
			{
				object[] args = this.parameters.Select( n => LValue.Deref( st ) ).ToArray();
				argsArray.AddRange( args );
			}

			// Push on a scope marker so that the function will run in its own scope.
			IScope oldScope;
			if( fv.scope == null )
				oldScope = st.constScope;
			else
				oldScope = fv.scope;
			st.pushActionAndScope( new Step( this, a => {}, ScopeMarker ), new StandardScope( oldScope ) );

			// The actual run action.
			if( fv.func != null )
			{
				// Push on a pusher for a null return value. This is to ensure that calls always return
				// something, for result stack sanity. This won't get executed if the function returns
				// a value on its own.
				st.pushAction( new Step( this, st2 => st2.pushResult( null ), "call: result pusher" ) );

				// Set a second scope with just the parameters.
				IScope callScope = new ParameterScope( st.scope, fv.func.parameters );
				st.pushActionAndScope( new Step( this, a => {}, "call: parameter scope" ), callScope );

				st.scope.set( "arguments", argsArray );
				for( int i=0; i<argsArray.Count; ++i )
					if( i < fv.func.parameters.Length )
					{
						string name = fv.func.parameters[i];
						st.scope.set( name, argsArray[i] );
					}
					else
						break;

				// Do the actual function call.
				fv.func.block.run( st );
			}
			else if( fv.metal != null )
			{
				fv.metal( st, argsArray.ToArray() );
			}
			else
				throw CoralException.GetArg( "Attempt to call null function" );
		} ) );
		this.source.run( state );
		foreach( AstNode p in this.parameters )
			p.run( state );
	}

	public override string ToString()
	{
		return "<call {0}({1})>".FormatI(
			this.source.ToString(),
			String.Join( ",", this.parameters.Select( p => p.ToString() ).ToArray() )
		);
	}
}

}
