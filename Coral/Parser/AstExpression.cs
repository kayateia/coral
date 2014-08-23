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
	static public bool CoerceBool( object o )
	{
		if( o is int )
			return ((int)o) != 0;
		if( o is bool )
			return (bool)o;
		if( o is string )
			return ((string)o).IsNullOrEmpty();
		return o != null;
	}

	static public int CoerceNumber( object o )
	{
		if( o is int )
			return (int)o;
		if( o is bool )
			return ((bool)o) ? 1 : 0;
		if( o is string )
			return int.Parse( (string)o, CultureFree.Culture );

		throw new ArgumentException( "Can't coerce value into a number" );
	}

	static public string CoerceString( object o )
	{
		return o.ToStringI();
	}

	// Not all operations are represented here yet.
	Dictionary<string, Func<object,object,object>> s_operations = new Dictionary<string, Func<object,object,object>>()
	{
		{ "-", (a,b) => CoerceNumber( a ) - CoerceNumber( b ) },
		{ "+", (a,b) => plus( a, b ) },
		{ "*", (a,b) => CoerceNumber( a ) * CoerceNumber( b ) },
		{ "/", (a,b) => CoerceNumber( a ) / CoerceNumber( b ) },
		{ "<", (a,b) => CoerceNumber( a ) < CoerceNumber( b ) },
		{ ">", (a,b) => CoerceNumber( a ) > CoerceNumber( b ) },
		{ "<=", (a,b) => CoerceNumber( a ) <= CoerceNumber( b ) },
		{ ">=", (a,b) => CoerceNumber( a ) >= CoerceNumber( b ) },
		{ "==", (a,b) => CoerceNumber( a ) == CoerceNumber( b ) },
		{ "!=", (a,b) => CoerceNumber( a ) != CoerceNumber( b ) },
		{ "||", (a,b) => CoerceBool( a ) || CoerceBool( b ) },
		{ "&&", (a,b) => CoerceBool( a ) && CoerceBool( b ) }
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
		if( node.Term.Name != "BinExpr" )
			return false;

		this.left = Compiler.ConvertNode( node.ChildNodes[0] );
		this.right = Compiler.ConvertNode( node.ChildNodes[2] );
		this.op = node.ChildNodes[1].Term.Name;
		return true;
	}

	static object plus( object l, object r )
	{
		// Either side being a string means the result is a string.
		if( l is string || r is string )
		{
			l = CoerceString( l );
			r = CoerceString( r );
			return (string)l + (string)r;
		}

		// Can't add bools.
		if( l is bool || r is bool )
			throw new ArgumentException( "Can't implicitly convert bool to number" );

		// The remaining option is that they're numbers.
		return (int)l + (int)r;
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
					if( this.op == "+=" )
					{
						// This one is a bit trickier since we do different things based on different
						// types, and the left side needs to be an LValue.
						if( !(left2 is LValue) )
							throw new ArgumentException( "Left-hand side of += must be LValue" );

						LValue lv = (LValue)left2;
						st2.pushAction( new Step( this, st3 =>
						{
							// We need to write the LValue back, but keep the result value on
							// the result stack since this can be used as a normal expression too.
							object val = st3.popResult();
							val = plus( val, right2 );
							lv.write( st3, val );
							st3.pushResult( val );
						} ) );
						lv.read( st2 );
					}
					else
						st.pushResult( s_operations[this.op]( left2, right2 ) );
				} ) );

				object right = st.popResult();
				object left = st.popResult();

				if( left is LValue && this.op != "+=" )
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
		return "({0} {1} {2})".FormatI( this.left, this.op, this.right );
	}
}

}
