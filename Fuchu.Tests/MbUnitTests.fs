﻿namespace Fuchu

module MbUnitTests = 
    open Fuchu
    open Fuchu.MbUnit
    open Fuchu.MbUnitTestTypes
    open NUnit.Framework
    
    [<Tests>]
    let tests =
        "From MbUnit" =>> [
            "nothing" =>
                fun () ->
                    let test = MbUnitTestToFuchu typeof<string>
                    match test with
                    | TestList [] -> ()
                    | _ -> Assert.Fail(sprintf "Should have been TestList [], but was %A" test)

            "basic" =>> [
                let test = MbUnitTestToFuchu typeof<ATestFixture>
                let result = lazy evalSilent test
                yield "read tests" =>
                    fun () ->
                        Assert.AreEqual(2, result.Value.Length)
                        let testName s = sprintf "%s/%s" typeof<ATestFixture>.FullName s
                        Assert.AreEqual(testName "ATest", result.Value.[0].Name)
                        Assert.AreEqual(testName "AnotherTest", result.Value.[1].Name)
                yield "executed tests" =>
                    fun () ->
                        Assert.True(TestResult.isPassed result.Value.[0].Result)
                        Assert.True(TestResult.isFailed result.Value.[1].Result)
            ]

            "with StaticTestFactory" =>
                fun() ->
                    let testType = typeof<ATestFixtureWithStaticTestFactories>
                    let test = MbUnitTestToFuchu testType
                    let testName = testType.Name
                    match test with
                    | TestList 
                      [
                        TestLabel(_, TestList 
                                     [
                                        TestLabel("suite name", 
                                                  TestList [
                                                    TestLabel("test 1", TestCase(_))
                                                    TestLabel("test 2", TestCase(_))
                                                  ])
                                     ])
                      ] -> ()
                    | _ -> Assert.Fail(sprintf "unexpected %A" test)
        ]
