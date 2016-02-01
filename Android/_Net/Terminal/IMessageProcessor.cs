using System;

namespace mapKnight.Android.Net
{
	public interface IMessageProcessor
	{
		int[] BeginCompute (byte[] data);

		void FillComputeData (byte[] data);

		void EndCompute ();
	}
}

