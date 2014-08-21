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
	/// Read the LValue's value, as if it were an RValue. This will push what's
	/// needed onto the continuation stack in order to result in an RValue on
	/// the result stack.
	/// </summary>
	public Action<State> read;

	/// <summary>
	/// Write the LValue's new value. This will push what's needed onto the
	/// continuation and result stacks in order to result in an assignment.
	/// </summary>
	public Action<State, object> write;
}

}
