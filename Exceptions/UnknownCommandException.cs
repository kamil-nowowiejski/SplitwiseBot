using System;

namespace SplitwiseBot.Exceptions
{
	public class UnknownCommandException : Exception
	{
		public string Command { get; }

		public UnknownCommandException(string command)
		{
			Command = command;
		}
	}
}