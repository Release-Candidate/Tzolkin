// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TestTzolkinNumber.fs
//
//==============================================================================

namespace TestTzolkin


open Expecto
open System

open RC.Maya
open Generic

[<AutoOpen>]
module TestTzolkinNumber=

    let config = { config with
                    receivedArgs = fun _ name no args ->
                         loggerFuncDeb "TestTzolkinNumber" name no args }

    let configList = { configList with
                             receivedArgs = fun _ name no args ->
                                  loggerFuncInfo "TestTzolkinNumber" name no args }

    let referenceDates = [  ("01.01.1970", 13)
                            ("01.01.1800", 10)
                            ("12.12.1926", 4)
                            ("26.01.1958", 10)
                            ("15.03.1967", 4)
                            ("08.05.1975", 3)
                            ("17.02.1978", 5)
                            ("25.10.1986", 5)
                            ("13.05.1992", 4)
                            ("08.11.1997", 7)
                            ("01.01.2000", 11)
                            ("06.07.2005", 9)
                            ("01.10.2017", 7)
                            ("20.03.2021", 12)
                         ]
                         |> stringList2DateList TzolkinNumber.T.TzolkinNumber

    [<Tests>]
    let tests =
        testList
            "TzolkinNumber"
            [   testPropertyWithConfig config "addition with int is commutative"
                <| fun i j ->
                    testCommutativity TzolkinNumber.create i j

                testPropertyWithConfig config "addition with TimeSpan is commutative"
                <| fun i j ->
                    testCommutativity TzolkinNumber.create i (TimeSpan.FromDays (float j))

                testPropertyWithConfig config "addition with int has neutral element"
                <| fun i ->
                    testNeutralElement TzolkinNumber.create i 0

                testPropertyWithConfig config "addition with TimeSpan has neutral element"
                <| fun i ->
                    testNeutralElement TzolkinNumber.create i (TimeSpan.FromDays 0.)

                testPropertyWithConfig config "addition is associative with int"
                <| fun i j k ->
                    testAssociativity TzolkinNumber.create i j k

                testPropertyWithConfig config "addition is associative with TimeSpan"
                <| fun i j k ->
                     testAssociativity TzolkinNumber.create
                        i
                        (TimeSpan.FromDays (float j))
                        (TimeSpan.FromDays (float k))

                testPropertyWithConfig config "addition is the same with TimeSpan and int"
                <| fun i j ->
                     testIntTimeSpanSame TzolkinNumber.create
                        i
                        j
                        (TimeSpan.FromDays (float j))

                testPropertyWithConfig config "subtraction is same as addition"
                <| fun i j ->
                    testSubtraction TzolkinNumber.create i j TzolkinNumber.modulo13

                testCase "fromDate with reference date list"
                <| fun _ ->
                    testFromDate TzolkinNumber.fromDate referenceDates

                testCase "getNextDate with a reference date list"
                <| fun _ ->
                    testNextDate TzolkinNumber.getNext 13 referenceDates false

                testCase "getLastDate with a reference date list"
                <| fun _ ->
                    testNextDate TzolkinNumber.getLast 13 referenceDates true

                testPropertyWithConfig configList "getNextList with a reference date list"
                <| fun i ->
                        testNextList
                                TzolkinNumber.getNextList
                                TzolkinNumber.fromDate
                                (abs i)
                                13
                                referenceDates
                                false

                testPropertyWithConfig configList "getLastList with a reference date list"
                <| fun i ->
                        testNextList
                                TzolkinNumber.getLastList
                                TzolkinNumber.fromDate
                                (abs i)
                                13
                                referenceDates
                                true

            ]
