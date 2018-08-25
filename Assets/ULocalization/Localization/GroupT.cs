using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULocalization
{
	public class Group<T> : IEnumerable<Group<T>>
	{
		List<Group<T>> children = new List<Group<T>>();
		T value;

		public bool IsLeafNode => children.Count == 0;
		public int ChildCount => children.Count;
		public T Value => value;

		public Group<T> this[int index]
		{
			get { return children[index]; }
		}

		public Group<T> Add(T value)
		{
			Group<T> childGroup = new Group<T>();
			childGroup.value = value;

			children.Add(childGroup);

			return childGroup;
		}

		public bool Contains(Func<Group<T>, bool> predicate, out Group<T> child)
		{
			foreach (Group<T> potentialChild in children)
			{
				if (predicate(potentialChild))
				{
					child = potentialChild;
					return true;
				}
			}

			child = null;
			return false;
		}

		IEnumerator<Group<T>> IEnumerable<Group<T>>.GetEnumerator()
		{
			return children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return children.GetEnumerator();
		}
	}
}