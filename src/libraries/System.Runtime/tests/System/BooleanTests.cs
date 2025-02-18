// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace System.Tests
{
    public partial class BooleanTests
    {
        [Fact]
        public void TrueString_Get_ReturnsTrue()
        {
            Assert.Equal("True", bool.TrueString);
        }

        [Fact]
        public void FalseString_Get_ReturnsFalse()
        {
            Assert.Equal("False", bool.FalseString);
        }

        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            yield return new object[] { "True", true };
            yield return new object[] { "true", true };
            yield return new object[] { "TRUE", true };
            yield return new object[] { "tRuE", true };
            yield return new object[] { "  True  ", true };
            yield return new object[] { "True\0", true };
            yield return new object[] { " \0 \0  True   \0 ", true };
            yield return new object[] { "False", false };
            yield return new object[] { "false", false };
            yield return new object[] { "FALSE", false };
            yield return new object[] { "fAlSe", false };
            yield return new object[] { "False  ", false };
            yield return new object[] { "False\0", false };
            yield return new object[] { "  False \0\0\0  ", false };
        }

        [Theory]
        [MemberData(nameof(Parse_Valid_TestData))]
        public void Parse_ValidValue_ReturnsExpected(string value, bool expected)
        {
            Assert.True(bool.TryParse(value, out bool result));
            Assert.Equal(expected, result);

            Assert.Equal(expected, bool.Parse(value));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            yield return new object[] { null, typeof(ArgumentNullException) };
            yield return new object[] { "", typeof(FormatException) };
            yield return new object[] { " ", typeof(FormatException) };
            yield return new object[] { "Garbage", typeof(FormatException) };
            yield return new object[] { "True\0Garbage", typeof(FormatException) };
            yield return new object[] { "True\0True", typeof(FormatException) };
            yield return new object[] { "True True", typeof(FormatException) };
            yield return new object[] { "True False", typeof(FormatException) };
            yield return new object[] { "False True", typeof(FormatException) };
            yield return new object[] { "Fa lse", typeof(FormatException) };
            yield return new object[] { "T", typeof(FormatException) };
            yield return new object[] { "0", typeof(FormatException) };
            yield return new object[] { "1", typeof(FormatException) };
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public void Parse_InvalidValue_ThrowsException(string value, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => bool.Parse(value));

            Assert.False(bool.TryParse(value, out bool result));
            Assert.False(result);
        }
        
        public static IEnumerable<object[]> Parse_ValidWithOffsetCount_TestData()
        {
            foreach (object[] inputs in Parse_Valid_TestData())
            {
                yield return new object[] { inputs[0], 0, ((string)inputs[0]).Length, inputs[1] };
            }

            yield return new object[] { " \0 \0  TrueFalse   \0 ", 6, 4, true };
            yield return new object[] { " \0 \0  TrueFalse   \0 ", 10, 5, false };
        }

        [Theory]
        [MemberData(nameof(Parse_ValidWithOffsetCount_TestData))]
        public static void Parse_Span_Valid(string value, int offset, int count, bool expected)
        {
            Assert.Equal(expected, bool.Parse(value.AsSpan(offset, count)));

            Assert.True(bool.TryParse(value.AsSpan(offset, count), out bool result));
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Span_Invalid(string value, Type exceptionType)
        {
            if (value != null)
            {
                Assert.Throws(exceptionType, () => bool.Parse(value.AsSpan()));

                Assert.False(bool.TryParse(value.AsSpan(), out bool result));
                Assert.False(result);
            }
        }

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public void ToString_Invoke_ReturnsExpected(bool value, string expected)
        {
            Assert.Equal(expected, value.ToString());
        }

        [Theory]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 1)]
        [InlineData(true, null, 1)]
        [InlineData(false, false, 0)]
        [InlineData(false, true, -1)]
        [InlineData(false, null, 1)]
        public void CompareTo_Other_ReturnsExpected(bool b, object obj, int expected)
        {
            if (obj is bool boolValue)
            {
                Assert.Equal(expected, Math.Sign(b.CompareTo(boolValue)));
            }

            Assert.Equal(expected, Math.Sign(b.CompareTo(obj)));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, "true")]
        [InlineData(false, 0)]
        [InlineData(false, "false")]
        public void CompareTo_ObjectNotBool_ThrowsArgumentException(bool b, object obj)
        {
            AssertExtensions.Throws<ArgumentException>(null, () => b.CompareTo(obj));
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, "1", false)]
        [InlineData(true, "True", false)]
        [InlineData(true, null, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, "0", false)]
        [InlineData(false, "False", false)]
        [InlineData(false, null, false)]
        public void Equals_Other_ReturnsExpected(bool b1, object obj, bool expected)
        {
            if (obj is bool boolValue)
            {
                Assert.Equal(expected, b1.Equals(boolValue));
                Assert.Equal(expected, b1.GetHashCode().Equals(obj.GetHashCode()));
            }

            Assert.Equal(expected, b1.Equals(obj));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetHashCode_Invoke_ReturnsExpected(bool value, int expected)
        {
            Assert.Equal(expected, value.GetHashCode());
        }

        [Fact]
        public void GetTypeCode_Invoke_ReturnsBoolean()
        {
            Assert.Equal(TypeCode.Boolean, true.GetTypeCode());
        }

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public static void TryFormat(bool i, string expected)
        {
            char[] actual;
            int charsWritten;

            // Just right
            actual = new char[expected.Length];
            Assert.True(i.TryFormat(actual.AsSpan(), out charsWritten));
            Assert.Equal(expected.Length, charsWritten);
            Assert.Equal(expected, new string(actual));

            // Longer than needed
            actual = new char[expected.Length + 1];
            Assert.True(i.TryFormat(actual.AsSpan(), out charsWritten));
            Assert.Equal(expected.Length, charsWritten);
            Assert.Equal(expected, new string(actual, 0, charsWritten));

            // Too short
            if (expected.Length > 0)
            {
                actual = new char[expected.Length - 1];
                Assert.False(i.TryFormat(actual.AsSpan(), out charsWritten));
                Assert.Equal(0, charsWritten);
            }
        }
    }
}
