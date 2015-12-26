using System;
using System.Collections.Generic;

using Android.Content;

namespace mapKnight.Android
{
	public static partial class Content
	{
		private static List<ContentObject> contentObjects = new List<ContentObject> ();

		public interface ContentObject
		{
			public ContentObject ()
			{
				contentObjects.Add (this);
			}

			public virtual void OnUpdate ()
			{

			}

			public virtual void OnInit (Context GameContext)
			{

			}
		}
	}
}
