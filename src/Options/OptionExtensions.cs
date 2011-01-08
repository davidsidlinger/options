using System;
using System.Collections.Generic;
using System.Linq;

namespace Options
{
	///<summary>
	///	Extension methods designed to ease the pain of working with <see cref = "Option{TOption}" />.
	///</summary>
	public static class OptionExtensions
	{
		///<summary>
		///	Transforms an <see cref = "Option{TOption}" /> while retaining the protection it provides.
		///</summary>
		///<param name = "option">The <see cref = "Option{TOption}" /> being transformed</param>
		///<param name = "func">The function used to transform the internal value of <paramref name = "option" /></param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<typeparam name = "TResult">The type returned by <paramref name = "func" /></typeparam>
		///<returns>
		///	An <see cref = "Option{TOption}" /> of <typeparamref name = "TResult" /> internal type. It will 
		///	be constructed with the result of <paramref name = "func" />, if <paramref name = "option" /> contains a value.
		///</returns>
		///<remarks>
		///	If <paramref name = "func" /> is null, the returned <see cref = "Option{TOption}" /> will never contain a value.
		///</remarks>
		public static Option<TResult> Transform<TOption, TResult>(this Option<TOption> option, Func<TOption, TResult> func)
		{
			var funcOption = Option.AvoidNull(func);

			return funcOption.Handle(f => option.Handle(v => new Option<TResult>(f(v)),
			                                            () => new Option<TResult>()),
			                         () => new Option<TResult>());
		}

		///<summary>
		///	Retrieves the value contained in <paramref name = "option" /> or a supplied default.
		///</summary>
		///<param name = "option">An <see cref = "Option{TOption}" /> of <typeparamref name = "TOption" /> internal type</param>
		///<param name = "defaultValue">The <typeparamref name = "TOption" /> value to return, if <paramref name = "option" /> does not contain a value</param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<returns>A <typeparamref name = "TOption" /> value. The internal value of <paramref name = "option" /> or <paramref name = "defaultValue" /></returns>
		public static TOption GetValueOrDefault<TOption>(this Option<TOption> option, TOption defaultValue)
		{
			return option.Handle(v => v, () => defaultValue);
		}

		///<summary>
		///	Retrieves the value contained in <paramref name = "option" /> or throws an exception.
		///</summary>
		///<param name = "option">An <see cref = "Option{TOption}" /> of <typeparamref name = "TOption" /> internal type</param>
		///<param name = "createException">A function used to create an <typeparamref name = "TException" /> to throw</param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<typeparam name = "TException">The type of exception to throw</typeparam>
		///<returns>The internal value of <paramref name = "option" /></returns>
		///<exception><paramref name = "option" /> contains no value</exception>
		///<exception cref = "ArgumentNullException"><paramref name = "createException" /> is null</exception>
		public static TOption GetValueOrThrow<TOption, TException>(this Option<TOption> option,
		                                                           Func<TException> createException)
			where TException : Exception
		{
			if (createException == null)
			{
				throw new ArgumentNullException("createException");
			}
			return option.Handle(v => v, () => { throw createException(); });
		}

		///<summary>
		///	Retrieves the value contained in <paramref name = "option" /> or throws a <see cref = "NoneException" />.
		///</summary>
		///<param name = "option">An <see cref = "Option{TOption}" /> of <typeparamref name = "TOption" /> internal type</param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<returns>The internal value of <paramref name = "option" /></returns>
		///<exception cref = "NoneException"><paramref name = "option" /> contains no value</exception>
		public static TOption GetValueOrThrow<TOption>(this Option<TOption> option)
		{
			return option.Handle(v => v, () => { throw new NoneException(); });
		}

		///<summary>
		///	Returns a <see cref = "bool" /> indicating whether <paramref name = "option" /> contains a value.
		///</summary>
		///<param name = "option">The <see cref = "Option{TOption}" /> to test</param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<returns>True, if <paramref name = "option" /> contains a value; false, if not.</returns>
		public static bool IsSome<TOption>(this Option<TOption> option)
		{
			return option.Transform(v => true).GetValueOrDefault(false);
		}

		///<summary>
		///	Converts an <see cref = "Option{TOption}" /> to a nullable value type.
		///</summary>
		///<param name = "option">The <see cref = "Option{TOption}" /> to convert to a nullable <typeparamref name = "TOption" /></param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<returns>A nullable <typeparamref name = "TOption" /> which will contain a value if <paramref name = "option" /> does.</returns>
		public static TOption? AsNullable<TOption>(this Option<TOption> option)
			where TOption : struct
		{
			return option.Transform(v => (TOption?)v).GetValueOrDefault(null);
		}

		///<summary>
		///	Converts an <see cref = "Option{TOption}" /> to a reference type.
		///</summary>
		///<param name = "option">The <see cref = "Option{TOption}" /> to convert to a nullable <typeparamref name = "TOption" /></param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<returns>A(n) <typeparamref name = "TOption" /> which will be null if <paramref name = "option" /> does not contain a value.</returns>
		public static TOption AsUnprotected<TOption>(this Option<TOption> option)
			where TOption : class
		{
			return option.GetValueOrDefault(null);
		}

		///<summary>
		///	Combines 2 <see cref = "Option{TOption}" />s into an <see cref = "Option{TOption}" /> that contains a <see cref = "Tuple" /> of <typeparamref name = "T1" /> and <typeparamref name = "T2" />
		///</summary>
		///<param name = "first">The first <see cref = "Option{TOption}" /></param>
		///<param name = "second">The second <see cref = "Option{TOption}" /></param>
		///<typeparam name = "T1">The internal type of <paramref name = "first" /></typeparam>
		///<typeparam name = "T2">The internal type of <paramref name = "second" /></typeparam>
		///<returns>An <see cref = "Option{TOption}" /></returns>
		///<remarks>
		///	The <see cref = "Option{TOption}" /> returned will only contain a value if both parameters contain a value.
		///</remarks>
		public static Option<Tuple<T1, T2>> Intersect<T1, T2>(this Option<T1> first, Option<T2> second)
		{
			return first.Handle(v => second.Transform(v2 => Tuple.Create(v, v2)),
			                    () => new Option<Tuple<T1, T2>>());
		}

		///<summary>
		///	Returns the first <see cref = "Option{TOption}" /> that contains a value from <paramref name = "options" />.
		///</summary>
		///<param name = "options">An enumerable of <see cref = "Option{TOption}" /></param>
		///<typeparam name = "TOption">The internal type of the <see cref = "Option{TOption}" />s</typeparam>
		///<returns>The first non-empty <see cref = "Option{TOption}" /> yielded, or an empty <see cref = "Option{TOption}" /></returns>
		public static Option<TOption> Coalese<TOption>(this IEnumerable<Option<TOption>> options)
		{
			return options == null
			       	? Option.None<TOption>()
			       	: options.FirstOrDefault(o => o.Transform(v => true).GetValueOrDefault(false));
		}

		///<summary>
		///	&quot;Lifts&quot; one <see cref = "Option{TOption}" /> from another.
		///</summary>
		///<param name = "option">The original <see cref = "Option{TOption}" /></param>
		///<param name = "selector">A function which takes the value of <paramref name = "option" /> and returns another <see cref = "Option{TOption}" /></param>
		///<typeparam name = "TOption">The internal type of <paramref name = "option" /></typeparam>
		///<typeparam name = "TResult">The internal type of the returned <see cref = "Option{TOption}" /></typeparam>
		///<returns>An <see cref = "Option{TOption}" /> of <typeparamref name = "TResult" /> type</returns>
		public static Option<TResult> Lift<TOption, TResult>(this Option<TOption> option,
		                                                     Func<TOption, Option<TResult>> selector)
		{
			return selector == null
			       	? Option.None<TResult>()
			       	: option.Transform(selector).GetValueOrDefault(Option.None<TResult>());
		}
	}
}