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
	// Not all operations are represented here yet.
	Dictionary<string, Func<object,object,object>> s_operations = new Dictionary<string, Func<object,object,object>>()
	{
		{ "-", (a,b) => minus( a, b ) },
		{ "+", (a,b) => plus( a, b ) },
		{ "*", (a,b) => Util.CoerceNumber( a ) * Util.CoerceNumber( b ) },
		{ "/", (a,b) => Util.CoerceNumber( a ) / Util.CoerceNumber( b ) },
		{ "<", (a,b) => Util.CoerceNumber( a ) < Util.CoerceNumber( b ) },
		{ ">", (a,b) => Util.CoerceNumber( a ) > Util.CoerceNumber( b ) },
		{ "<=", (a,b) => Util.CoerceNumber( a ) <= Util.CoerceNumber( b ) },
		{ ">=", (a,b) => Util.CoerceNumber( a ) >= Util.CoerceNumber( b ) },
		{ "==", (a,b) => Util.CoerceNumber( a ) == Util.CoerceNumber( b ) },
		{ "!=", (a,b) => Util.CoerceNumber( a ) != Util.CoerceNumber( b ) },
		{ "||", (a,b) => Util.CoerceBool( a ) || Util.CoerceBool( b ) },
		{ "&&", (a,b) => Util.CoerceBool( a ) && Util.CoerceBool( b ) }
	};

	/// <summary>
	/// The operator we represent.
	/// </summary>
	public string op { get; private set; }

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
		if( node.Term.Name == "BinExpr" )
		{
			this.left = Compiler.ConvertNode( node.ChildNodes[0] );
			this.right = Compiler.ConvertNode( node.ChildNodes[2] );
			this.op = node.ChildNodes[1].Term.Name;

			return true;
		}
		else if( node.Term.Name == "UnExpr" )
		{
			this.op = node.ChildNodes[0].Term.Name;
			this.right = Compiler.ConvertNode( node.ChildNodes[1] );
		}

		return true;
	}

	static object plus( object l, object r )
	{
		// Either side being a string means the result is a string.
		if( l is string || r is string )
		{
			l = Util.CoerceString( l );
			r = Util.CoerceString( r );
			return (string)l + (string)r;
		}

		// Can't add bools.
		if( l is bool || r is bool )
			throw new ArgumentException( "Can't implicitly convert bool to number" );

		// The remaining option is that they're numbers.
		return (int)l + (int)r;
	}

	static object minus( object l, object r )
	{
		// This handles things like "-3".
		int li = 0, ri = 0;
		if( l != null )
			li = Util.CoerceNumber( l );
		if( r != null )
			ri = Util.CoerceNumber( r );
		return li - ri;
	}

	public override void run( State state )
	{
		// We execute by running the "lhs" and "rhs", and then applying the operator to them,
		// producing another value. If the result of either is an LValue, we have to dereference it.

		state.pushAction(
			new Step( this, st =>
			{
				object right = LValue.Deref( st );

				if( this.op == "+=" )
				{
					object left = st.popResult();

					// This one is a bit trickier since we do different things based on different
					// types, and the left side needs to be an LValue.
					if( !(left is LValue) )
						throw new ArgumentException( "Left-hand side of += must be LValue" );

					// We need to write the LValue back, but keep the result value on
					// the result stack since this can be used as a normal expression too.
					LValue lv = (LValue)left;
					object val = lv.read( st );
					val = plus( val, right );
					lv.write( st, val );
					st.pushResult( val );
				}
				else
				{
					object left = LValue.Deref( st );
					st.pushResult( s_operations[this.op]( left, right ) );
				}
			} )
		);
		this.right.run( state );
		if( this.left != null )
			this.left.run( state );
		else
			state.pushResult( null );
	}

	public override string ToString()
	{
		return "({0} {1} {2})".FormatI( this.left, this.op, this.right );
	}
}

}
