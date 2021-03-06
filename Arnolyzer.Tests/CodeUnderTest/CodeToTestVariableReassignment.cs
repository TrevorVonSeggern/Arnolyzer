﻿using Arnolyzer.RuleExceptionAttributes;
using System;

namespace Arnolyzer.Tests.CodeUnderTest
{
    public class CodeToTestVariableReassignment
    {
        private int _f;

        [MutatesParameter]
        [MutableVariable("y", "z")]
        public void F1()
        {
            var a = 1;
            string b;
            a = 2;
            b = "";
            foreach (var c in "123456".ToCharArray())
            {
                var x = c;
                x = ' ';
                _f = 1;
                b = $"{c}{_f}";
            }
            Console.WriteLine($"{a}{b}");
        }

        [MutableVariable("a")]
        public void F2()
        {
            var a = 1;
            a = 2;
            var b = $"{a}";
        }
    }
}