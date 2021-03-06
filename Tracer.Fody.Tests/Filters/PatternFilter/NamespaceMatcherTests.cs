﻿using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Tracer.Fody.Filters.PatternFilter;

namespace Tracer.Fody.Tests.Filters.PatternFilter
{
    public class NamespaceMatcherTests
    {
        [Test]
        public void FullSpecMatch()
        {
            var matcher = new NamespaceMatcher("MyNamespace.Inner");
            matcher.IsMatch("MyNamespace.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Other").Should().BeFalse();
        }

        [Test]
        public void QuestionMarkMatch()
        {
            var matcher = new NamespaceMatcher("MyNam?space.Inner");
            matcher.IsMatch("MyNamespace.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamfspace.inner").Should().BeTrue();
            matcher.IsMatch("MyNam.space.Other").Should().BeFalse();
        }

        [Test]
        public void MultipleQuestionMarksMatch()
        {
            var matcher = new NamespaceMatcher("MyNam?space.Inn?r");
            matcher.IsMatch("MyNamespace.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamfspace.innEr").Should().BeTrue();
            matcher.IsMatch("MyNamfspace.innfr").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Inn?r").Should().BeFalse();
        }

        [Test]
        public void StarMarkMatch()
        {
            var matcher = new NamespaceMatcher("My*.Inner");
            matcher.IsMatch("MyNamespace.Inner").Should().BeTrue();
            matcher.IsMatch("My.Inner").Should().BeTrue();
            matcher.IsMatch("MyOther.inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Other").Should().BeFalse();
            matcher.IsMatch("MyNam.Space.Inner").Should().BeFalse();
            matcher.IsMatch("YourNamespace.Inner").Should().BeFalse();
            matcher.IsMatch("MyNamespace.Other.Inner").Should().BeFalse();
            matcher.IsMatch("My.Namespace.Inner").Should().BeFalse();
        }

        [Test]
        public void DoubleDotMatch()
        {
            var matcher = new NamespaceMatcher("MyNamespace..Inner");
            matcher.IsMatch("MyNamespace.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Some.inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Some.Other.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Other").Should().BeFalse();
            matcher.IsMatch("MyNamespace.Some.Other").Should().BeFalse();
        }

        [Test]
        public void MultipleDoubleDotMatch()
        {
            var matcher = new NamespaceMatcher("MyNamespace..Other..Inner");
            matcher.IsMatch("MyNamespace.Other.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Some.Other.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Some.Other.Some.Some.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Other.Some.Inner").Should().BeTrue();
            matcher.IsMatch("MyNamespace.Other").Should().BeFalse();
            matcher.IsMatch("MyNamespace.Some.Other").Should().BeFalse();
            matcher.IsMatch("MyNamespace.Inner").Should().BeFalse();
            matcher.IsMatch("MyNamespace.Inner.Other").Should().BeFalse();
        }

        [Test]
        public void SortTest_IncreasingNumberOfElements()
        {
            var matcher1 = new NamespaceMatcher("MyNamespace");
            var matcher2 = new NamespaceMatcher("MyNamespace.Other");
            var matcher3 = new NamespaceMatcher("MyNamespace.Other.Inner");
            var list = new List<NamespaceMatcher> { matcher2, matcher1, matcher3 };
            list.Sort();
            list[0].Should().Be(matcher3);
            list[1].Should().Be(matcher2);
            list[2].Should().Be(matcher1);
        }

        [Test]
        public void SortTest_StarIsLessSpecific()
        {
            var matcher1 = new NamespaceMatcher("MyNamespace.Other.Inner");
            var matcher2 = new NamespaceMatcher("MyNamespace.Other.*");
            var list = new List<NamespaceMatcher> { matcher2, matcher1 };
            list.Sort();
            list[0].Should().Be(matcher1);
            list[1].Should().Be(matcher2);
        }

        [Test]
        public void SortTest_DoubleDotIsLessSpecific()
        {
            var matcher1 = new NamespaceMatcher("MyNamespace.Other.Inner");
            var matcher2 = new NamespaceMatcher("MyNamespace..Inner");
            var list = new List<NamespaceMatcher> { matcher2, matcher1 };
            list.Sort();
            list[0].Should().Be(matcher1);
            list[1].Should().Be(matcher2);
        }
    }
}
