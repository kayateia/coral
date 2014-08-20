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
using System.Collections.Generic;

/// <summary>
/// Lambda type for a continuation that will be called when the step is reached.
/// </summary>
public delegate void ActionCallback( State s );

/// <summary>
/// Represents a step pushed onto the continuation stack.
/// </summary>
public class Step
{
	public Step( AstNode n, ActionCallback a, string d = null )
	{
		this.node = n;
		this.action = a;
		this.description = d;
		if( this.description == null )
			this.description = n.ToString();
	}

	/// <summary>
	/// The AstNode associated with this step, if any. This will be used for
	/// line number and other context info.
	/// </summary>
	public AstNode node { get; set; }

	/// <summary>
	/// The actual callback lambda to be called when this step is reached.
	/// </summary>
	public ActionCallback action { get; set; }

	/// <summary>
	/// Description of the step. This will be filled from AstNode if not passed in.
	/// </summary>
	public string description { get; set; }

	public override string ToString()
	{
		return this.description;
	}
}

/// <summary>
/// State of the executing code fragment.
/// </summary>
public class State
{
	public State()
	{
		this.scope = new Scope();
		_stack = new Stack<Step>();
		_resultStack = new Stack<object>();
	}

	/// <summary>
	/// Variable scope. Eventually this will be a stack of scopes.
	/// </summary>
	public Scope scope
	{
		get;
		private set;
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
	/// Pop a single action from the step stack.
	/// </summary>
	public Step popAction()
	{
		return _stack.Pop();
	}

	/// <summary>
	/// Returns the number of steps on the stack.
	/// </summary>
	public int getActionCount()
	{
		return _stack.Count;
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
}

}
