using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [TestClass()]
    public class StringUtilitiesTests
    {
        const int defaultKeyChars = 5;
        const char defaultKeyFiller = 'x';

        const int defaultSplitMaxLength = 5;
        const char defaultSplitChar = ' ';

        [TestMethod()]
        public void KeyIgnoreTest()
        {
            var firstResult = StringUtilities.StringToKey("Some string", defaultKeyChars, defaultKeyFiller);
            var secondResult = StringUtilities.StringToKey("Some other string", defaultKeyChars, defaultKeyFiller);

            Assert.AreEqual(firstResult, secondResult, "Characters beyond limit should be ignored");
        }

        [TestMethod()]
        public void KeyPadTest()
        {
            var firstResult = StringUtilities.StringToKey("AAA", defaultKeyChars, defaultKeyFiller);
            var secondResult = StringUtilities.StringToKey("AAA", defaultKeyChars, defaultKeyFiller);

            Assert.AreEqual(firstResult, secondResult, "Identical short strings should produce the same key");
        }

        [TestMethod()]
        public void KeySimilarityTest()
        {
            var firstResult = StringUtilities.StringToKey("Peter", defaultKeyChars, defaultKeyFiller);
            var secondResult = StringUtilities.StringToKey("Petyr", defaultKeyChars, defaultKeyFiller);
            var thirdResult = StringUtilities.StringToKey("Maria", defaultKeyChars, defaultKeyFiller);
            var diff1And2 = Math.Abs(firstResult - secondResult);
            var diff1And3 = Math.Abs(firstResult - thirdResult);

            Assert.IsTrue(diff1And3 > diff1And2, "Very similar strings should have similar keys");
        }

        [TestMethod()]
        public void KeyLengthTest()
        {
            var longKeyChars = 100;
            StringUtilities.StringToKey("AAA", longKeyChars, defaultKeyFiller);

            // Does not throw -> pass
        }

        [TestMethod()]
        public void KeyEmptyTest()
        {
            var result = StringUtilities.StringToKey("", defaultKeyChars, defaultKeyFiller);

            Assert.AreEqual(0, result, "An empty string should produce the key 0");
        }

        [TestMethod()]
        public void KeyNullTest()
        {
            var result = StringUtilities.StringToKey(null, defaultKeyChars, defaultKeyFiller);

            Assert.AreEqual(0, result, "A null string should produce the key 0");
        }

        [TestMethod()]
        public void SplitSimpleTest()
        {
            var result = StringUtilities.SplitString("Aaa Bbb", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "Aaa", "Bbb" };

            CollectionAssert.AreEqual(expected, result, "String should be split at the only possible position");
        }

        [TestMethod()]
        public void SplitBeforeLimitTest()
        {
            var result = StringUtilities.SplitString("Aaa Bb b", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "Aaa", "Bb b" };

            CollectionAssert.AreEqual(expected, result, "Nearest splitting point should be chosen");
        }

        [TestMethod()]
        public void SplitAtLimitTest()
        {
            var result = StringUtilities.SplitString("Aaa   Bbb", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "Aaa ", " Bbb" };

            CollectionAssert.AreEqual(expected, result, "Nearest splitting point should be chosen");
        }

        [TestMethod()]
        public void SplitAfterLimitTest()
        {
            var result = StringUtilities.SplitString("Aa aB bb", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "Aa aB", "bb" };

            CollectionAssert.AreEqual(expected, result, "Nearest splitting point should be chosen");
        }

        [TestMethod()]
        public void SplitNoSpacesTest()
        {
            var result = StringUtilities.SplitString("AaaBbb", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "AaaBbb" };

            CollectionAssert.AreEqual(expected, result, "A string with no splitting points should not be modified");
        }

        [TestMethod()]
        public void SplitOneSpaceTest()
        {
            var result = StringUtilities.SplitString("AaaBb b", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "AaaBb", "b" };

            CollectionAssert.AreEqual(expected, result, "A single splitting point near the end should be chosen");
        }

        [TestMethod()]
        public void SplitEmptyTest()
        {
            var result = StringUtilities.SplitString("", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "" };

            CollectionAssert.AreEqual(expected, result, "Empty strings should not be modified");
        }
        
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SplitNullTest()
        {
            StringUtilities.SplitString(null, defaultSplitMaxLength, defaultSplitChar).ToList();
        }

        [TestMethod()]
        public void SplitInitialTest()
        {
            var result = StringUtilities.SplitString(" AaaBbb", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { " AaaBbb" };

            CollectionAssert.AreEqual(expected, result, "Parts should have at least one character");
        }

        [TestMethod()]
        public void SplitFinalTest()
        {
            var result = StringUtilities.SplitString("AaaBbb ", defaultSplitMaxLength, defaultSplitChar).ToList();
            var expected = new List<string>() { "AaaBbb " };

            CollectionAssert.AreEqual(expected, result, "Parts should have at least one character");
        }
    }
}