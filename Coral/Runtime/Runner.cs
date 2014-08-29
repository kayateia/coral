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

/// <summary>
/// Handles running parsed code snippets.
/// </summary>
public class Runner
{
	public Runner()
	{
		_state = new State();
	}

	public Runner( State st )
	{
		_state = st;
	}

	/// <summary>
	/// Run an AstNode tree synchronously. This will run the code from
	/// beginning to end, and then return.
	/// </summary>
	public void runSync( CodeFragment code )
	{
		setupConstants();
		code.root.run( _state );
		runSync();
	}

	/// <summary>
	/// Executes what's already on the stack. Assumes an already-set-up
	/// environment with scopes and all.
	/// </summary>
	/// <exception cref="CoralException">Can be thrown if an exception "escapes" the script.</exception>
	void runSync()
	{
		while( _state.getActionCount() > 0 )
		{
			var act = _state.popAction();
			try
			{
				act.action( _state );
			}
			catch( CoralException ex )
			{
				// Something went wrong while executing that instruction. Cause a throw.
				AstTry.ThrowException( _state, ex.data );
			}
			catch( UnhandledException ex )
			{
				// This "escaped" the script, so re-throw it here manually to avoid hitting
				// the CoralException catcher above.
				throw ex.InnerException;
			}
			catch( Exception ex )
			{
				// Wrap anything else (e.g. div by zero) as an invalid operation exception and do normal processing.
				AstTry.ThrowException( _state, CoralException.GetInvOp( ex.Message ).data );
			}
		}
	}

	/// <summary>
	/// Adds a value to the state's scope.
	/// </summary>
	/// <remarks>
	/// Coral-native types are added as-is, while other types are Passthrough wrapped.
	/// </remarks>
	public void addToScope( string name, object value )
	{
		if( value == null || value is int || value is bool || value is string || value is string[] )
			_state.scope.set( name, Util.CoerceFromDotNet( value ) );
		else
		{
			var wrapped = new Passthrough( value );
			wrapped.registerScope( _state.scope, name );
		}
	}

	/// <summary>
	/// Sets a callback that will be called whenever a scope can't find an object.
	/// </summary>
	/// <param name="lookup"></param>
	public void setScopeCallback( Func<string, object> lookup )
	{
		_state.setScopeCallback( s => lookup( s ) );
	}

	/// <summary>
	/// Calls a Coral function and returns its return value.
	/// </summary>
	public object callFunction( string name, object[] args, System.Type returnType )
	{
		// Convert the arguments.
		AstNode[] wrapped = new AstNode[args.Length];
		for( int i=0; i<args.Length; ++i )
			wrapped[i] = new WrapperAstObject( Util.CoerceFromDotNet( args[i] ) );

		// Construct a synthetic AstIdentifier for the name.
		AstIdentifier id = new AstIdentifier( name );

		// Construct a synthetic AstCall.
		AstCall call = new AstCall( id, wrapped );

		call.run( _state );
		runSync();

		return Util.CoerceToDotNet( returnType, _state.popResult() );
	}

	public void setupConstants()
	{
		// Boolean values.
		_state.constScope.setConstant( "false", false );
		_state.constScope.setConstant( "true", true );
		_state.constScope.setConstant( "null", null );

		// Built-in objects.
		StringObject.RegisterObject( _state.constScope );
	}

	/// <summary>
	/// The state used while running code.
	/// </summary>
	public State state
	{
		get
		{
			return _state;
		}
	}

	State _state;
}

}
