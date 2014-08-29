﻿#region License
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
/// C# exception used for interpreter and in-language Coral exceptions.
/// </summary>
public class CoralException : System.Exception
{
	public CoralException( object obj )
		: base( "Coral Exception" )
	{
		_obj = obj;
	}

	// These are some standard types thrown by the runtime while executing.
	public const string ArgumentException = "arg_exception";
	public const string InvalidOperationException = "invop_exception";
	public const string InvalidProgramException = "invprog_exception";

	static public CoralException GetArg( string message )
	{
		return GetForName( ArgumentException, message );
	}

	static public CoralException GetInvOp( string message )
	{
		return GetForName( InvalidOperationException, message );
	}

	static public CoralException GetInvProg( string message )
	{
		return GetForName( InvalidOperationException, message );
	}

	static public CoralException GetForName( string name, string message )
	{
		return new CoralException( new Dictionary<object,object>
			{
				{ "name", name },
				{ "message", message }
			}
		);
	}

	/// <summary>
	/// If something was thrown, it will be contained here.
	/// </summary>
	public object data
	{
		get
		{
			return _obj;
		}
	}

	/// <summary>
	/// If the thrown data was a dictionary and it has a name member, this returns it.
	/// </summary>
	public string name
	{
		get
		{
			if( _obj != null && _obj is Dictionary<object,object> )
			{
				var dict = (Dictionary<object,object>)_obj;
				if( dict.ContainsKey( "name" ) )
					return Util.CoerceString( dict["name"] );
				else
					return null;
			}
			else
				return null;
		}
	}

	public override string Message
	{
		get
		{
			string name = this.name;
			if( name != null )
				return name;
			else
				return base.Message;
		}
	}

	object _obj;
}

}
