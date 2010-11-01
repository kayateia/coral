namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptNET.Runtime;

/// <summary>
/// Thin subclass of ScriptContext that lets us associate non-S#-accessible
/// items from a later method call.
/// </summary>
public class ContextWithBaggage : ScriptContext {
	public ContextWithBaggage() { }

	public void baggageSet(string item, object val) {
		_baggage[item] = val;
	}

	public object baggageGet(string item) {
		return _baggage[item];
	}

	public void baggageDel(string item) {
		_baggage.Remove(item);
	}

	Dictionary<string, object> _baggage = new Dictionary<string,object>();
}

}
