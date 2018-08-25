using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULocalization
{
	/// <summary>
	/// Container for localized text. This will resolve to a translation, if available.
	/// </summary>
	public class localstring : IEnumerable<object>
	{
		public readonly string _key;
		public readonly object[] _args;

		public string key => _key;

		public localstring(string key, params object[] values)
		{
			this._key = key;
			this._args = values;
		}

		public static implicit operator string(localstring localstring)
		{
			return localstring.ToString();
		}

		public override string ToString()
		{
			return Localization.Translate(this);
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return ((IEnumerable<object>)_args).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _args.GetEnumerator();
		}
	}
}