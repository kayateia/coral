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
using Irony.Parsing;

/// <summary>
/// AST node for an operator (includes both binary and unary).
/// </summary>
class AstExpression : AstNode
{
	public enum Op
	{
		Minus,
		Plus,
		Not,
		Times,
		Div,
		Exp,
		Less,
		Greater,
		LessEqual,
		GreaterEqual,
		Equal,
		NotEqual,
		Decrement,
		Increment
	}

	Dictionary<string, Op> s_operators = new Dictionary<string,Op>()
	{
		{ "-", Op.Minus },
		{ "+", Op.Plus },
		{ "!", Op.Not },
		{ "*", Op.Times },
		{ "/", Op.Div },
		{ "**", Op.Exp },
		{ "<", Op.Less },
		{ ">", Op.Greater },
		{ "<=", Op.LessEqual },
		{ ">=", Op.GreaterEqual },
		{ "==", Op.Equal },
		{ "!=", Op.NotEqual },
		{ "--", Op.Decrement },
		{ "++", Op.Increment },
	};

	// Not all operations are represented here yet.
	Dictionary<Op, Func<object,object,object>> s_operations = new Dictionary<Op,Func<object,object,object>>()
	{
		{ Op.Minus, (a,b) => (int)a - (int)b },
		{ Op.Plus, (a,b) => (int)a + (int)b },
		{ Op.Times, (a,b) => (int)a * (int)b },
		{ Op.Div, (a,b) => (int)a / (int)b },
		{ Op.Less, (a,b) => (int)a < (int)b },
		{ Op.LessEqual, (a,b) => (int)a <= (int)b },
		{ Op.Greater, (a,b) => (int)a > (int)b },
		{ Op.GreaterEqual, (a,b) => (int)a >= (int)b },
		{ Op.Equal, (a,b) => (int)a == (int)b },
		{ Op.NotEqual, (a,b) => (int)a != (int)b },
	};

	/// <summary>
	/// The operator we represent.
	/// </summary>
	public Op op { get; private set; }

	/// <summary>
	/// The left argument, or null if this is a unary operation.
	/// </summary>
	public AstNode left { get; private set; }

	/// <summary>
	/// The right argument.
	/// </summary>
	public AstNode right { get; private set; }

	public override bool convert( ParseTreeNode node )
	{
		if( node.Term.Name != "BinExpr" )
			return false;

		this.left = Compiler.ConvertNode( node.ChildNodes[0] );
		this.right = Compiler.ConvertNode( node.ChildNodes[2] );
		this.op = s_operators[node.ChildNodes[1].Term.Name];
		return true;
	}

	public override void run( State state )
	{
		// We execute by running the "lhs" and "rhs", and then applying the operator to them,
		// producing another value. If the result of either is an LValue, we have to dereference it.

		state.pushAction(
			new Step( this, st =>
			{
				st.pushAction( new Step( this, st2 =>
				{
					object right2 = st.popResult();
					object left2 = st.popResult();
					st.pushResult( s_operations[this.op]( left2, right2 ) );
				} ) );

				object right = st.popResult();
				object left = st.popResult();

				if( left is LValue )
					((LValue)left).read( st );
				else
					st.pushResult( left );
				if( right is LValue )
					((LValue)right).read( st );
				else
					st.pushResult( right );
			} )
		);
		this.right.run( state );
		this.left.run( state );
	}

	public override string ToString()
	{
		return "({0} {1} {2})".FormatI( this.left, s_operators.Where( kv => kv.Value == this.op ).Select( kv => kv.Key ).First(), this.right );
	}
}

}
