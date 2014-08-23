namespace Kayateia.Climoo.Scripting.Coral
{

/// <summary>
/// An object/array/dictionary which is accessed by calling back into native code.
/// </summary>
public class MetalObject
{
	/// <summary>
	/// Represents an index lookup. This should push an LValue on the results stack.
	/// </summary>
	public IndexLookup indexLookup;
	public delegate void IndexLookup( State state, object index );

	/// <summary>
	/// Represents a member lookup. This should push an LValue on the results stack.
	/// </summary>
	public MemberLookup memberLookup;
	public delegate void MemberLookup( State state, string name );
}

}
