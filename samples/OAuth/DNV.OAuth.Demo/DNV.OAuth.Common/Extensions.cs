using System;

namespace DNV.OAuth.Common;

public static class Extensions
{
	public static T ThrowIfNull<T>(this T source, string? message = null)
	{
		ArgumentNullException.ThrowIfNull(source, message);
		return source;
	}

	public static T ThrowIfNull<T, TException>(this T source, TException exception)
		where TException : Exception
	{
		if (source == null)
		{
			throw exception;
		}

		return source;
	}

	public static string ThrowIfNullOrEmpty(this string source, string? message = null)
	{
		ArgumentException.ThrowIfNullOrEmpty(source, message);
		return source;
	}

	public static string ThrowIfNullOrEmpty<TException>(this string source, TException exception)
		where TException : Exception
	{
		if (string.IsNullOrEmpty(source))
		{
			throw exception;
		}

		return source;
	}

	public static string ThrowIfNullOrWhiteSpace(this string source, string? message = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(source, message);
		return source;
	}

	public static string ThrowIfNullOrWhiteSpace<TException>(this string source, TException exception)
		where TException : Exception
	{
		if (string.IsNullOrWhiteSpace(source))
		{
			throw exception;
		}

		return source;
	}
}
