using System;
using System.Collections.Generic;
using System.Text;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Appends the line with indent.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="text">The text.</param>
        /// <param name="level">The level.</param>
        public static void AppendLine(this StringBuilder builder, string text, int level)
        {
            for (int i = 0; i < level; i++)
            {
                builder.Append("\t");
            }

            builder.Append(text);
            builder.Append(Environment.NewLine);
        }

        /// <summary>
        /// Appends the format.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="text">The text.</param>
        /// <param name="level">The level.</param>
        public static void Append(this StringBuilder builder, string text, int level)
        {
            for (int i = 0; i < level; i++)
            {
                builder.Append("\t");
            }

            builder.Append(text);
        }

        /// <summary>
        /// Appends the line format.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="text">The text.</param>
        /// <param name="level">The level.</param>
        /// <param name="args">The args.</param>
        public static void AppendLineFormat(this StringBuilder builder, string text, int level, params object[] args)
        {
            builder.AppendLine(String.Format(text, args), level);
        }

        /// <summary>
        /// Linq extension which executes specified action for each element in collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }
    }
}