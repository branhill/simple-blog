using SimpleBlog.Infrastructures.Exceptions;
using SimpleBlog.Infrastructures.GuardClauses;
using System.Collections.Generic;
using Xunit;

namespace SimpleBlog.Tests.UnitTests.Infrastructures.GuardClauses
{
    public class GuardClauseExtensionsTests
    {
        public static IEnumerable<object[]> NullOrEmptyData => new[]
        {
            new object[] { null },
            new object[] { "" },
            new object[] { new string[0] },
            new object[] { new List<object>() }
        };

        public static IEnumerable<object[]> NotEmptyData => new[]
        {
            new object[] { "a" },
            new object[] { new string[1] },
            new object[] { new List<object> {new object()} }
        };

        [Theory]
        [MemberData(nameof(NullOrEmptyData))]
        public void NullOrEmptyThrowInputNullOrEmpty(object input)
        {
            var e400 = Assert.Throws<StatusCodeException>(() => Guard.Against.NullOrEmptyThrow400BadRequest(input, nameof(input)));
            Assert.Equal(400, e400.StatusCode);

            var e404 = Assert.Throws<StatusCodeException>(() => Guard.Against.NullOrEmptyThrow404NotFound(input, nameof(input)));
            Assert.Equal(404, e404.StatusCode);
        }

        [Theory]
        [MemberData(nameof(NotEmptyData))]
        public void NullOrEmptyThrowInputNotEmpty(object input)
        {
            Guard.Against.NullOrEmptyThrow400BadRequest(input, nameof(input));
            Guard.Against.NullOrEmptyThrow404NotFound(input, nameof(input));
        }
    }
}
