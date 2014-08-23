namespace Kayateia.Climoo.Scripting.Coral
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// An FValue represents a function that can be executed.
/// </summary>
class FValue
{
	public FValue( AstFunc func )
	{
		this.func = func;
	}

	/// <summary>
	/// The function we're representing.
	/// </summary>
	public AstFunc func;
}

}
