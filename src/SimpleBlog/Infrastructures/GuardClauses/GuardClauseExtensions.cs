using Microsoft.AspNetCore.Http;
using SimpleBlog.Infrastructures.Exceptions;
using System.Collections;

namespace SimpleBlog.Infrastructures.GuardClauses
{
    public static class GuardClauseExtensions
    {
        /// <exception cref="StatusCodeException"></exception>
        public static void NullThrow400BadRequest(this IGuardClause guard, object input, string parameterName)
        {
            if (input is null)
                throw new StatusCodeException(StatusCodes.Status400BadRequest,
                    $"Required {parameterName} was null.");
        }

        /// <exception cref="StatusCodeException"></exception>
        public static void NullThrow404NotFound(this IGuardClause guard, object input, string parameterName)
        {
            if (input is null)
                throw new StatusCodeException(StatusCodes.Status404NotFound,
                    $"Requested {parameterName} was not found.");
        }

        /// <exception cref="StatusCodeException"></exception>
        public static void NullOrEmptyThrow400BadRequest(this IGuardClause guard, object input, string parameterName)
        {
            NullOrEmptyThrow(input,
                new StatusCodeException(StatusCodes.Status400BadRequest,
                    $"Required {parameterName} was null or empty."));
        }

        /// <exception cref="StatusCodeException"></exception>
        public static void NullOrEmptyThrow404NotFound(this IGuardClause guard, object input, string parameterName)
        {
            NullOrEmptyThrow(input,
                new StatusCodeException(StatusCodes.Status404NotFound,
                    $"Requested {parameterName} was null or empty."));
        }

        private static void NullOrEmptyThrow(object input, StatusCodeException exception)
        {
            switch (input)
            {
                case string s when string.IsNullOrEmpty(s):
                case ICollection c when c.Count == 0:
                case null:
                    throw exception;
            }
        }
    }
}
