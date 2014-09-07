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

/// <summary>
/// Contains various math utilities.
/// </summary>
public class MathObject
{
	static public void RegisterObject( ConstScope scope )
	{
		var pt = new Passthrough( new MathObject() );
		pt.registerConst( scope, "math" );
	}

	[CoralPassthrough]
	public int random( int max )
	{
		if( max == 0 )
			throw CoralException.GetArg( "Can't random() with a max of zero" );
		else
			return m_rand.Next() % max;
	}

	[CoralPassthrough]
	public string sha1( string input )
	{
		return input.Sha1Hash();
	}

	Random m_rand = new Random();
}

}
