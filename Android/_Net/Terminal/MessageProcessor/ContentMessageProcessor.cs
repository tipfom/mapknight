using System;

namespace mapKnight.Android.Net.MessageProcessor
{
	public class ContentMessageProcessor : IMessageProcessor
	{
		public ContentMessageProcessor ()
		{
		}

#region IMessageProcessor implementation

		public int[] BeginCompute (byte[] data)
		{
			throw new NotImplementedException ();
		}

		public void FillComputeData (byte[] data)
		{
			throw new NotImplementedException ();
		}

		public void EndCompute ()
		{
			throw new NotImplementedException ();
		}

#endregion
	}
}

