using System;
using System.Collections.Generic;
using System.Data;

using AndreyTalanin0x00.DbNullHelpers;

namespace JapaneseLanguageTools.Data.SqlServer.Extensions;

/// <summary>
/// Provides a set of extension methods for common-table-type-mapped collections.
/// </summary>
internal static class CommonTableTypesExtensions
{
    /// <summary>
    /// Creates a <see cref="DataTable" /> from a collection of <see cref="byte" /> values.
    /// </summary>
    /// <param name="values">The <see cref="byte" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with a single <c>Value</c> column.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToDataTable(this IEnumerable<byte> values)
    {
        return values.ToDataTable<byte>();
    }

    /// <summary>
    /// Creates an ordered <see cref="DataTable" /> from a collection of <see cref="byte" /> values.
    /// </summary>
    /// <param name="values">The <see cref="byte" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with the <c>Value</c> and <c>Order</c> columns.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToOrderedDataTable(this IEnumerable<byte> values)
    {
#pragma warning disable IDE0001
        return values.ToDataTable<byte>(ordered: true);
#pragma warning restore IDE0001
    }

    /// <summary>
    /// Creates a <see cref="DataTable" /> from a collection of <see cref="short" /> values.
    /// </summary>
    /// <param name="values">The <see cref="short" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with a single <c>Value</c> column.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToDataTable(this IEnumerable<short> values)
    {
        return values.ToDataTable<short>();
    }

    /// <summary>
    /// Creates an ordered <see cref="DataTable" /> from a collection of <see cref="short" /> values.
    /// </summary>
    /// <param name="values">The <see cref="short" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with the <c>Value</c> and <c>Order</c> columns.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToOrderedDataTable(this IEnumerable<short> values)
    {
#pragma warning disable IDE0001
        return values.ToDataTable<short>(ordered: true);
#pragma warning restore IDE0001
    }

    /// <summary>
    /// Creates a <see cref="DataTable" /> from a collection of <see cref="int" /> values.
    /// </summary>
    /// <param name="values">The <see cref="int" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with a single <c>Value</c> column.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToDataTable(this IEnumerable<int> values)
    {
        return values.ToDataTable<int>();
    }

    /// <summary>
    /// Creates an ordered <see cref="DataTable" /> from a collection of <see cref="int" /> values.
    /// </summary>
    /// <param name="values">The <see cref="int" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with the <c>Value</c> and <c>Order</c> columns.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToOrderedDataTable(this IEnumerable<int> values)
    {
#pragma warning disable IDE0001
        return values.ToDataTable<int>(ordered: true);
#pragma warning restore IDE0001
    }

    /// <summary>
    /// Creates a <see cref="DataTable" /> from a collection of <see cref="long" /> values.
    /// </summary>
    /// <param name="values">The <see cref="long" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with a single <c>Value</c> column.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToDataTable(this IEnumerable<long> values)
    {
        return values.ToDataTable<long>();
    }

    /// <summary>
    /// Creates an ordered <see cref="DataTable" /> from a collection of <see cref="long" /> values.
    /// </summary>
    /// <param name="values">The <see cref="long" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with the <c>Value</c> and <c>Order</c> columns.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToOrderedDataTable(this IEnumerable<long> values)
    {
#pragma warning disable IDE0001
        return values.ToDataTable<long>(ordered: true);
#pragma warning restore IDE0001
    }

    /// <summary>
    /// Creates a <see cref="DataTable" /> from a collection of <see cref="Guid" /> values.
    /// </summary>
    /// <param name="values">The <see cref="Guid" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with a single <c>Value</c> column.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToDataTable(this IEnumerable<Guid> values)
    {
        return values.ToDataTable<Guid>();
    }

    /// <summary>
    /// Creates an ordered <see cref="DataTable" /> from a collection of <see cref="Guid" /> values.
    /// </summary>
    /// <param name="values">The <see cref="Guid" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with the <c>Value</c> and <c>Order</c> columns.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToOrderedDataTable(this IEnumerable<Guid> values)
    {
#pragma warning disable IDE0001
        return values.ToDataTable<Guid>(ordered: true);
#pragma warning restore IDE0001
    }

    /// <summary>
    /// Creates a <see cref="DataTable" /> from a collection of <see cref="string" /> values.
    /// </summary>
    /// <param name="values">The <see cref="string" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with a single <c>Value</c> column.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToDataTable(this IEnumerable<string> values)
    {
        return values.ToDataTable<string>();
    }

    /// <summary>
    /// Creates an ordered <see cref="DataTable" /> from a collection of <see cref="string" /> values.
    /// </summary>
    /// <param name="values">The <see cref="string" /> collection.</param>
    /// <returns>
    /// A new <see cref="DataTable" /> instance with the <c>Value</c> and <c>Order</c> columns.
    /// The rows of the data table contain values from the provided collection.
    /// </returns>
    public static DataTable ToOrderedDataTable(this IEnumerable<string> values)
    {
#pragma warning disable IDE0001
        return values.ToDataTable<string>(ordered: true);
#pragma warning restore IDE0001
    }

    private static DataTable ToDataTable<T>(this IEnumerable<T> values, bool ordered = false)
    {
        DataTable dataTable = new();

        dataTable.Columns.Add("Value", typeof(T));
        if (ordered)
            dataTable.Columns.Add("Order", typeof(int));

        void AddRow(T value) => dataTable.Rows.Add(DbValueConvert.ToDbValue(value));
        void AddRowOrdered(T value, int order) => dataTable.Rows.Add(DbValueConvert.ToDbValue(value), DbValueConvert.ToDbValue(order));

        int counter = 0;
        foreach (T value in values)
        {
            if (ordered)
                AddRowOrdered(value, counter++);
            else
                AddRow(value);

            ;
        }

        return dataTable;
    }
}
