﻿using System;
using Arnolyzer.Analyzers.PureFunctionAnalyzers;
using Arnolyzer.Tests.DiagnosticVerification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuccincT.Options;

namespace Arnolyzer.Tests.Analyzers.PureFunctionAnalyzersTests
{
    [TestClass]
    public class AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzerTests
    {
        [TestMethod]
        public void NoCode_ShouldYieldNoDiagnostics() =>
            DiagnosticVerifier.VerifyDiagnostics<AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer>(TestFiles.EmptyFile);

        [TestMethod]
        public void StaticMethodWithNoParams_YieldsADiagnostic()
        {
            var commonExpected =
                new DiagnosticResultCommonProperties(new AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer());
            var expected =
                new DiagnosticResult(commonExpected,
                                     String.Format(Resources.AA1001StaticMethodsShouldHaveAtLeastOneParameterMessageFormat,
                                            "ReturnTrue"),
                                     Option<DiagnosticLocation>.Some(new DiagnosticLocation(5, 28, 38)));

            DiagnosticVerifier.VerifyDiagnostics<AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer>(
                TestFiles.CodeToTestAtLeastOneParameterAnalyzer,
                expected);
        }

        [TestMethod]
        public void StaticMethodWithNoParamsInGeneratedCode_YieldsNoDiagnostics()
        {
            DiagnosticVerifier.VerifyDiagnostics<AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer>(
                TestFiles.AutogeneratedNoParameterCode);
        }

        [TestMethod]
        public void CodeWithStaticGetters_YieldsNoDiagnostics()
        {
            DiagnosticVerifier.VerifyDiagnostics<AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer>(
                TestFiles.CodeToTestAtLeastOneParameterAnalyzerIgnoresGetters);
        }

        [TestMethod]
        public void TwoStaticMethodsWithNoParamsOneWithAttribute_YieldsOneDiagnostic()
        {
            var commonExpected =
                new DiagnosticResultCommonProperties(new AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer());
            var expected =
                new DiagnosticResult(commonExpected,
                                     String.Format(Resources.AA1001StaticMethodsShouldHaveAtLeastOneParameterMessageFormat,
                                            "ReturnTrue"),
                                     Option<DiagnosticLocation>.Some(new DiagnosticLocation(8, 28, 38)));

            DiagnosticVerifier.VerifyDiagnostics<AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer>(
                TestFiles.CodeToTestAtLeastOneParameterAnalyzerRespectsAttributes,
                expected);
        }
    }
}