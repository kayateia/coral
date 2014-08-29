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
using System.Text;
using Irony.Parsing;

/// <summary>
/// Handles compiling a raw ParseTree from Irony into a more civilized AstNode grammar,
/// which is more suitable for execution.
/// </summary>
public class Compiler
{
	/// <summary>
	/// Compile a code fragment into a CodeFragment, containing a compiled AST.
	/// </summary>
	static public CodeFragment Compile( string s )
	{
		if( s_parser == null )
			s_parser = new Parser( new CoralGrammar() );
		ParseTree tree = s_parser.Parse( s );
		if( tree.HasErrors() )
			return new CodeFragment( tree );

		try
		{
			AstNode node = ConvertNode( tree.Root );
			return new CodeFragment( node );
		}
		catch( CompilationException ex )
		{
			return new CodeFragment( ex );
		}
	}

	// Converts a single ParseTreeNode into an AstNode if possible.
	static internal AstNode ConvertNode( ParseTreeNode t )
	{
		if( t.Term.HasAstConfig() )
		{
			Type converter = t.Term.AstConfig.NodeType;
			AstNode n = (AstNode)Activator.CreateInstance( converter );
			if( n.convert( t ) )
				return n;
		}

		throw new CompilationException( "Can't convert node type {0}".FormatI( t.Term.Name ), t );
	}

	static Parser s_parser = null;
}

}
