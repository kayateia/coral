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
/// State of the executing code fragment.
/// </summary>
public class State
{
	public State()
	{
		_constScope = new ConstScope();
		_lookupScope = new LookupScope( _constScope );
		_rootScope = new StandardScope( _lookupScope );
		_baggage = new StandardScope();
		_stack = new Stack<Step>();
		_resultStack = new Stack<object>();
	}

	/// <summary>
	/// Variable scope.
	/// </summary>
	public IScope scope
	{
		get
		{
			foreach( Step s in _stack )
			{
				if( s.scope != null )
					return s.scope;
			}

			return _rootScope;
		}
	}

	/// <summary>
	/// The root constant scope.
	/// </summary>
	public ConstScope constScope
	{
		get
		{
			return _constScope;
		}
	}

	/// <summary>
	/// A miscellaneous scope that you can set arbitrary values into, to keep
	/// client app metadata along with the state.
	/// </summary>
	public IScope baggage
	{
		get
		{
			return _baggage;
		}
	}

	/// <summary>
	/// Sets a callback that will be called whenever a scope can't find an object.
	/// </summary>
	/// <param name="lookup"></param>
	public void setScopeCallback( LookupScope.LookupDelegate lookup )
	{
		_lookupScope.lookup = lookup;
	}

	/// <summary>
	/// Push a set of actions onto the step stack. They are pushed in order.
	/// </summary>
	public void pushActions( Step[] actions )
	{
		foreach( Step a in actions )
			_stack.Push( a );
	}

	/// <summary>
	/// Push a single action onto the step stack.
	/// </summary>
	public void pushAction( Step action )
	{
		_stack.Push( action );
	}

	/// <summary>
	/// Push a single action onto the step stack, creating a new variable scope.
	/// </summary>
	public void pushActionAndScope( Step action, IScope scope )
	{
		action.scope = scope;
		_stack.Push( action );
	}

	/// <summary>
	/// Pop a single action from the step stack.
	/// </summary>
	public Step popAction()
	{
		if( !_stack.Any() )
			throw new InvalidOperationException( "Action stack is empty" );
		return _stack.Pop();
	}

	/// <summary>
	/// Unwinds the stack until the unwindChecker returns true; that remaining step will
	/// also have been discarded. If the unwindChecker does not return true before the
	/// stack runs out, it's an error.
	/// </summary>
	public void unwindActions( Func<Step, bool> unwindChecker, bool keepMarker = false )
	{
		Step s = popAction();
		while( !unwindChecker( s ) )
			s = popAction();
		if( keepMarker )
			pushAction( s );
	}

	/// <summary>
	/// Returns the number of steps on the stack.
	/// </summary>
	public int getActionCount()
	{
		return _stack.Count;
	}

	/// <summary>
	/// Returns the first step we find that passes the checker, or null.
	/// </summary>
	public Step findAction( Func<Step, bool> checker )
	{
		foreach( Step s in _stack )
			if( checker( s ) )
				return s;
		return null;
	}

	/// <summary>
	/// Push a result onto the result stack.
	/// </summary>
	public void pushResult( object value )
	{
		_resultStack.Push( value );
	}

	/// <summary>
	/// Pop a result from the result stack.
	/// </summary>
	public object popResult()
	{
		return _resultStack.Pop();
	}

	/// <summary>
	/// Clear all the results from the result stack.
	/// </summary>
	public void clearResults()
	{
		_resultStack.Clear();
	}

	Stack<Step> _stack;
	Stack<object> _resultStack;
	IScope _rootScope;
	LookupScope _lookupScope;
	ConstScope _constScope;
	IScope _baggage;
}

}
