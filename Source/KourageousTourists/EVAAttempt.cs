using System;

namespace KourageousTourists
{
	public class EVAAttempt
	{
		public bool status { get; set; }
		public String message { get; set; }

		public EVAAttempt(String message, bool status)
		{
			this.message = message;
			this.status = status;
		}
	}
}

