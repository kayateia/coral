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

/// <summary>
/// Conditional statement. This includes if, elif, and else clauses.
/// </summary>
class AstIf : AstNode
{
	/// <summary>
	/// A single "if clause". This can be an if, elif, or else clause.
	/// </summary>
	public class IfClause {
		public IfClause( AstNode condition, AstStatements block )
		{
			this.condition = condition;
			this.block = block;
		}

		/// <summary>
		/// The condition, if it's not an else clause.
		/// </summary>
		public AstNode condition { get; private set; }

		/// <summary>
		/// The block of statements to run if the condition is true.
		/// </summary>
		public AstStatements block { get; private set; }

		public override string ToString()
		{
			if( this.condition == null )
				return "else " + this.block.ToString();
			else
				return "if " + this.condition.ToString() + " then " + this.block.ToString();
		}
	}

	/// <summary>
	/// All of our if clauses.
	/// </summary>
	public IfClause[] clauses { get; private set; }

	public override bool convert( Irony.Parsing.ParseTreeNode node, Compiler c )
	{
		base.convert( node, c );
		if( node.Term.Name == "IfStmt" )
		{
			var cl = new List<IfClause>();

			// Add in the main if clause first.
			cl.Add( new IfClause(
				condition: c.convertNode( node.ChildNodes[1] ),
				block: (AstStatements)c.convertNode( node.ChildNodes[2] )
			) );

			// Do we have Elif or Else clauses?
			for( int i=3; i<node.ChildNodes.Count; ++i )
			{
				if( node.ChildNodes[i].Term.Name == "ElifClauses" )
				{
					foreach( var n in node.ChildNodes[i].ChildNodes )
					{
						// Each of these should be an ElifClause.
						if( n.Term.Name != "ElifClause" )
							throw new CompilationException( "ElifClause expected", n );

						cl.Add( new IfClause(
							condition: c.convertNode( n.ChildNodes[1] ),
							block: (AstStatements)c.convertNode( n.ChildNodes[2] )
						) );
					}
				}
				else if( node.ChildNodes[i].Term.Name == "ElseClause" )
				{
					cl.Add( new IfClause(
						condition: null,
						block: (AstStatements)c.convertNode( node.ChildNodes[i].ChildNodes[1] )
					) );
				}
				else
				{
					throw new CompilationException( "ElifClauses or ElseClause expected", node.ChildNodes[i] );
				}
			}

			this.clauses = cl.ToArray();

			return true;
		}

		return false;
	}

	// Pulls the result of a conditional comparison and either queues the attached
	// block, or queues the next comparison.
	void ifRunner( State st, int clauseIndex )
	{
		object result = LValue.Deref( st );
		bool conv = Util.CoerceBool( result );

		if( conv )
			this.clauses[clauseIndex].block.run( st );
		else
		{
			++clauseIndex;
			if( clauseIndex >= this.clauses.Length )
				return;

			if( this.clauses[clauseIndex].condition == null )
			{
				this.clauses[clauseIndex].block.run( st );
			}
			else
			{
				st.pushAction( new Step( this, st2 => ifRunner( st2, clauseIndex ), this.clauses[clauseIndex].ToString() ) );
				this.clauses[clauseIndex].condition.run( st );
			}
		}
	}

	public override void run( State state )
	{
		// We execute here by executing the condition, then interpreting the
		// outcome of that expression to determin if we should push on the contents
		// of the block.
		//
		// In order to handle multiple elif clauses, we curry a "clause index" value
		// in the continuations so that it can chain to the next elif or else if
		// the condition isn't true.
		state.pushAction( new Step( this, st => ifRunner( st, 0 ), this.clauses[0].ToString() ) );
		this.clauses[0].condition.run( state );
	}

	public override string ToString()
	{
		bool first = true;
		string str = "";
		foreach( IfClause i in this.clauses )
		{
			if( first )
			{
				str += "<" + i.ToString() + ">";
				first = false;
			}
			else
			{
				if( i.condition == null )
					str += "<" + i.ToString() + ">";
				else
					str += "<el" + i.ToString() + ">";
			}
		}

		return str;
	}
}

}
