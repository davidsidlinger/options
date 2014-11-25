using System.Linq;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using Options.Core;

namespace Options.Fixtures
{
	public static class TestExtenstions
	{
		public static void AssertNone<TOption>(this Option<TOption> option)
		{
			option.Handle(v =>
			              {
			              	Assert.Fail("Option was not supposed to contain a value");
			              	return false;
			              },
			              () => true);
		}

		public static void AssertSomeAnd<TOption>(this Option<TOption> option, IResolveConstraint constraint)
		{
			option.Handle(v =>
			              {
			              	Assert.That(v, constraint);
			              	return false;
			              },
			              () =>
			              {
			              	Assert.Fail("Option was supposed to contain a value.");
			              	return true;
			              });
		}

        public static void AssertNone<TOption>(this IOption<TOption> option)
        {
            option.Handle(v =>
            {
                Assert.Fail("Option was not supposed to contain a value");
                return false;
            },
                          () => true);
        }

        public static void AssertSomeAnd<TOption>(this IOption<TOption> option, IResolveConstraint constraint)
        {
            option.Handle(v =>
            {
                Assert.That(v, constraint);
                return false;
            },
                          () =>
                          {
                              Assert.Fail("Option was supposed to contain a value.");
                              return true;
                          });
        }
	}
}
