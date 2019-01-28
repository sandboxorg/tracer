﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using NUnit.Framework;
using Tracer.Fody.Filters.PatternFilter;

namespace Tracer.Fody.Tests.Filters.PatternFilter
{
    public class PatternDefinitionTests
    {
        [Test]
        public void FullSpecifiedMatches()
        {
            var def = PatternDefinition.BuildUpDefinition("Mynamespace.MyClass.MyMethod", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.MyClass), "OtherMethod")).Should().BeFalse();
        }

        [Test]
        public void FullSpecifiedMultiNamespaceMatches()
        {
            var def = PatternDefinition.BuildUpDefinition("MyNamespace.Inner.AndMore.MyClass.MyMethod", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "OtherMethod")).Should().BeFalse();
        }

        [Test]
        public void MultiNamespaceUsingStar()
        {
            var def = PatternDefinition.BuildUpDefinition("MyNamespace.*.AndMore.MyClass.MyMethod", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Other.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.MyClass), "MyMethod")).Should().BeFalse();
        }

        [Test]
        public void MultiNamespaceTwoDottedAtEndMatches()
        {
            var def = PatternDefinition.BuildUpDefinition("MyNamespace..MyClass.MyMethod", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "OtherMethod")).Should().BeFalse();
        }

        [Test]
        public void MultiNamespaceTwoDottedAtBeginningMatches()
        {
            var def = PatternDefinition.BuildUpDefinition("..MyClass.MyMethod", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "OtherMethod")).Should().BeFalse();
        }

        [Test]
        public void MatchEverything()
        {
            var def = PatternDefinition.BuildUpDefinition("..*.*", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "OtherMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Other.AndMore.MyClass), "OtherMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.OtherClass), "MyMethod")).Should().BeTrue();
        }

        [Test]
        public void MatchEveryPublicClass()
        {
            var def = PatternDefinition.BuildUpDefinition("..[public]*.*", true);
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.Inner.AndMore.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.MyClass), "MyMethod")).Should().BeTrue();
            def.IsMatching(GetMethodDefinition(typeof(MyNamespace.OtherClass), "MyMethod")).Should().BeFalse();
        }

        private MethodDefinition GetMethodDefinition(Type runtimeType, string methodName)
        {
            var asmDef = AssemblyDefinition.ReadAssembly(runtimeType.Module.FullyQualifiedName);
            var types = asmDef.MainModule.GetAllTypes();
            var type = types.FirstOrDefault(it => it.FullName == runtimeType.FullName);
            return type.GetMethods().FirstOrDefault(it => it.Name.Equals(methodName));
        }
    }
}

namespace MyNamespace
{
    public class MyClass
    {
        public void MyMethod() { }

        private void OtherMethod() { }
    }

    class OtherClass
    {
        public void MyMethod() { }

        private void OtherMethod() { }
    }
}

namespace MyNamespace.Inner.AndMore
{
    public class MyClass
    {
        public void MyMethod() { }

        private void OtherMethod() { }
    }
}

namespace MyNamespace.Other.AndMore
{
    public class MyClass
    {
        public void MyMethod() { }

        private void OtherMethod() { }
    }
}