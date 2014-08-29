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

	public AstCall( AstNode source, AstNode[] parameters )
	{
		this.source = source;
		this.parameters = parameters;
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

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "FunctionCall" )
		{
			this.source = Compiler.ConvertNode( node.ChildNodes[0] );
			this.parameters = node.ChildNodes[1].ChildNodes.Select( n => Compiler.ConvertNode( n ) ).ToArray();

			return true;
		}

		return false;
	}

	void runFunction( State st, object fvo )
	{
		if( !(fvo is FValue) )
			throw CoralException.GetArg( "Attempted call to non-function" );
		FValue fv = (FValue)fvo;

		if( fv.func != null )
		{
			// Set a second scope with just the parameters.
			IScope callScope = new ParameterScope( st.scope, fv.func.parameters );
			st.pushActionAndScope( new Step( this, a => {}, "call: parameter scope" ), callScope );

			var argsArray = new List<object>();
			st.scope.set( "arguments", argsArray );
			for( int i=0; i<this.parameters.Length; ++i )
			{
				object value = LValue.Deref( st );
				if( i < fv.func.parameters.Length )
				{
					string name = fv.func.parameters[i];
					st.scope.set( name, value );
				}
				argsArray.Add( value );
			}

			// Do the actual function call.
			fv.func.block.run( st );
		}
		else if( fv.metal != null )
		{
			object[] param = this.parameters.Select( n => LValue.Deref( st ) ).ToArray();
			fv.metal( st, param );
		}
		else
			throw CoralException.GetArg( "Attempt to call null function" );
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
			st.pushResult( fvo );

			// Push on a scope marker so that the function will run in its own scope.
			st.pushActionAndScope( new Step( this, a => {}, ScopeMarker ), new StandardScope( st.scope ) );

			if( ((FValue)fvo).func != null )
			{
				// Push on a pusher for a null return value. This is to ensure that calls always return
				// something, for result stack sanity. This won't get executed if the function returns
				// a value on its own.
				st.pushAction( new Step( this, st2 => st2.pushResult( null ), "call: result pusher" ) );
			}

			// The actual run action.
			st.pushAction( new Step( this, st2 => runFunction( st2, st.popResult() ) ) );
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
