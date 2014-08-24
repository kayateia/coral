namespace Kayateia.Climoo.Scripting.Coral
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// An LValue is a value that can either be read or written to. This includes
/// things like variables in the scope and member accesses.
/// </summary>
/// <remarks>
/// LValues as opposed to RValues.
/// 
/// LValues are, for example, variables and members of objects. RValues are,
/// for example, the result of an expression such as "4+5".
/// 
/// An LValue in this case is more or less equivalent to a reference in C++.
/// </remarks>
public class LValue
{
	/// <summary>
	/// Read the LValue's value, as if it were an RValue. This is processed immediately, not
	/// pushed on the action stack.
	/// </summary>
	public Func<State, object> read;

	/// <summary>
	/// Write the LValue's new value. This is processed immediately, not pushed on the action stack.
	/// </summary>
	public Action<State, object> write;

	/// <summary>
	/// Unpacks the specified LValue, if it's an LValue. Otherwise the value is returned again.
	/// </summary>
	static public object Deref( State state, object value )
	{
		if( value is LValue )
			return ((LValue)value).read( state );
		else
			return value;
	}

	/// <summary>
	/// Unpacks the LValue from the result stack, if it's an LValue. Otherwise the value is returned again.
	/// </summary>
	static public object Deref( State state )
	{
		object value = state.popResult();
		if( value is LValue )
			return ((LValue)value).read( state );
		else
			return value;
	}
}

}
