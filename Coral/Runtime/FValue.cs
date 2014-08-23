namespace Kayateia.Climoo.Scripting.Coral
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// An FValue represents a function that can be executed.
/// </summary>
public class FValue
{
	public FValue( AstFunc func )
	{
		this.func = func;
	}

	public FValue( MetalCallback metal )
	{
		this.metal = metal;
	}

	/// <summary>
	/// The function we're representing, if this is Coral code.
	/// </summary>
	public AstFunc func;

	public delegate void MetalCallback( State state, object[] args );

	/// <summary>
	/// The function we're representing, if this is metal code.
	/// </summary>
	public MetalCallback metal;
}

}
