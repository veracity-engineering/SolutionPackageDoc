using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace DNV.Web.Swagger;

public static class OpenApiExtensions
{
	/// <summary>
	/// Sets the nullable property on a schema and on its items if it is an array
	/// </summary>
	/// <param name="schema"></param>
	/// <param name="nullable"></param>
	public static void SetNullable(this OpenApiSchema? schema, bool nullable)
	{
		if (schema == null) return;

		schema.Nullable = nullable;
		schema.Extensions["x-nullable"] = new OpenApiBoolean(nullable);

		if (schema.Type == "array")
		{
			schema.Items.SetNullable(nullable);
		}
	}

	/// <summary>
	/// Null-safe ForEach extension method for IEnumerable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="action"></param>
	public static void ForEach<T>(this IEnumerable<T>? source, Action<T> action)
	{
		if (source == null) return;

		foreach (var item in source) action(item);
	}
}
