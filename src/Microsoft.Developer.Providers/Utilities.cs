/**
 *  Copyright (c) Microsoft Corporation.
 *  Licensed under the MIT License.
 */

namespace Microsoft.Developer.Providers;

public static class Utilities
{
    /// <summary>
    /// Generates a random string of the specified length.  Useful for generating random tokens or state.
    /// </summary>
    /// <param name="length">The length of the string in characters.</param>
    /// <returns>A random String</returns>
    public static string RandomString(int length = 32)
    {
        Random rnd = new(Guid.NewGuid().GetHashCode());
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }
}