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
using System;
using System.Linq;

/// <summary>
/// Represents a code-constructed dictionary, an associative array.
/// </summary>
class AstDictionary : AstNode
{
	public class Pair
	{
		public AstNode key;
		public AstNode value;
	}
	public List<Pair> pairs { get; private set; }

	public AstDictionary()
	{
		this.pairs = new List<Pair>();
	}

	public override bool convert( Irony.Parsing.ParseTreeNode node )
	{
		if( node.Term.Name == "DictExpr" )
		{
			var elems = node.ChildNodes[1];
			if( elems.Term.Name != "DictElements" )
				throw new ArgumentException( "Expected DictElements" );

			foreach( var child in elems.ChildNodes )
			{
				if( child.Term.Name != "DictElement" )
					throw new ArgumentException( "Expected DictElement" );

				Pair p = new Pair()
				{
					key = Compiler.ConvertNode( child.ChildNodes[0] ),
					value = Compiler.ConvertNode( child.ChildNodes[1] )
				};

				this.pairs.Add( p );
			}

			return true;
		}

		return false;
	}

	public override void run( State state )
	{
		// We execute by pushing all the code for building the keys and values onto the
		// action stack, and finish off with one that will combine them all.
		state.pushAction(
			new Step( this, s =>
			{
				var dict = new Dictionary<object, object>();
				for( int i=0; i<this.pairs.Count; ++i )
				{
					object key = s.popResult();
					object value = s.popResult();
					dict[key] = value;
				}

				s.pushResult( dict );
			} )
		);
		foreach( Pair p in this.pairs )
		{
			p.key.run( state );
			p.value.run( state );
		}
	}

	public override string ToString()
	{
		return "{{{0}}}".FormatI(
			String.Join( ",",
				this.pairs.Select( i => "{0}:{1}".FormatI( i.key.ToString(), i.value.ToString() ) ).ToArray()
			)
		);
	}
}

}
