using System;

namespace DNV.OAuth.Common;

public static class Throws
{
	public static T IfNull<T>(this T source, string? message = null)
		=> source.IfNull(new ArgumentNullException(message));

	public static T IfNull<T, TException>(this T source, TException exception)
		where TException : Exception
	{
		if (source == null)
		{
			throw exception;
		}

		return source;
	}
}
