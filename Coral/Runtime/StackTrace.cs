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
/// A stack trace, information about function calls.
/// </summary>
public class StackTrace
{
	public StackTrace()
	{
		this.frames = new StackFrame[0];
	}

	/// <summary>
	/// One stack frame.
	/// </summary>
	public class StackFrame
	{
		public StackFrame()
		{
			this.unitName = "<anon>";
			this.funcName = "<anon>";
		}

		static public StackFrame Copy( StackFrame other )
		{
			return (StackFrame)other.MemberwiseClone();
		}

		/// <summary>
		/// The line number, starting with 1.
		/// </summary>
		public int line { get; set; }

		/// <summary>
		/// The column number, starting with 1.
		/// </summary>
		public int col { get; set; }

		/// <summary>
		/// The name of the compilation unit.
		/// </summary>
		public string unitName { get; set; }

		/// <summary>
		/// The name of the function/method, if applicable.
		/// </summary>
		public string funcName { get; set; }

		/// <summary>
		/// True if the function name has not been set.
		/// </summary>
		public bool isFuncNameDefault
		{
			get
			{
				return this.funcName == "<anon>";
			}
		}

		public override string ToString()
		{
			return " {0}:{1},{2} {3}".FormatI( this.unitName, this.line, this.col, this.funcName );
		}
	}

	/// <summary>
	/// The list of stack frames, from most recent to least.
	/// </summary>
	public StackFrame[] frames
	{
		get; set;
	}

	public override string ToString()
	{
		if( this.frames == null )
			return "<empty>";
		return String.Join( "\n", this.frames.Select( f => "  at " + f.ToString() ).ToArray() );
	}
}

}
