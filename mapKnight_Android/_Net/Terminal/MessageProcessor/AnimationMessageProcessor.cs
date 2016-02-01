using System;
using System.Text;
using System.Collections.Generic;

using mapKnight.Android.CGL.Entity;
using mapKnight.Utils;

namespace mapKnight.Android.Net.MessageProcessor
{
	public class AnimationMessageProcessor: IMessageProcessor
	{
		private byte currentOperation;
		private List<byte[]> receivedComputeData;

		public AnimationMessageProcessor ()
		{
			receivedComputeData = new List<byte[]> ();
		}

#region IMessageProcessor implementation

		public int[] BeginCompute (byte[] data)
		{
			currentOperation = data [0];
			switch (data [0]) {
			case 0:
				// set anim of current character
				return new int[]{ BitConverter.ToInt32 (data, 1) };
			}

			return null;
		}

		public void FillComputeData (byte[] data)
		{
			receivedComputeData.Add (data);
			switch (currentOperation) {
			case 0:
				// only one additional information required
				EndCompute ();
				break;
			}
		}

		public void EndCompute ()
		{
			switch (currentOperation) {
			case 0:
				Content.Character.AddAnimation (new CGLAnimation (XMLElemental.Load (Encoding.UTF8.GetString (receivedComputeData [0]).UnZip ())));
				break;
			}

			receivedComputeData.Clear ();
		}

#endregion
	}
}

