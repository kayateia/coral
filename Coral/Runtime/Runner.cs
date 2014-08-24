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
	void runSync()
	{
		while( _state.getActionCount() > 0 )
		{
			var act = _state.popAction();
			act.action( _state );
		}
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
