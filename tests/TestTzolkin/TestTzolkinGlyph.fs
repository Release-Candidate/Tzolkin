// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TestTzolkinGlyph.fs
//
//==============================================================================

namespace TestTzolkin

open Expecto
open System

open RC.Maya


[<AutoOpen>]
module TestTzolkinGlyph=

    let config = { config with
                          receivedArgs = fun _ name no args ->
                               loggerFuncDeb "TestTzolkinGlyph" name no args }

    let configList = { configList with
                               receivedArgs = fun _ name no args ->
                                    loggerFuncInfo "TestTzolkinGlyph" name no args }

    let referenceDates = [  ("01.01.1800", 14)
                            ("12.12.1926", 19)
                            ("26.01.1958", 7)
                            ("15.03.1967", 2)
                            ("01.01.1970", 5)
                            ("08.05.1975", 18)
                            ("17.02.1978", 14)
                            ("25.10.1986", 6)
                            ("13.05.1992", 13)
                            ("08.11.1997", 18)
                            ("01.01.2000", 2)
                            ("06.07.2005", 15)
                            ("01.10.2017", 5)
                            ("20.03.2021", 11)
                         ]
                         |> stringList2DateList TzolkinGlyph.T.TzolkinGlyph

    [<Tests>]
    let tests =
        testList
            "TzolkinGlyph"
            [   testPropertyWithConfig config "addition with int is commutative"
                <| fun i j ->
                    testCommutativity TzolkinGlyph.create i j

                testPropertyWithConfig config "addition with TimeSpan is commutative"
                <| fun i j ->
                    testCommutativity TzolkinGlyph.create i (TimeSpan.FromDays (float j))

                testPropertyWithConfig config "addition with int has neutral element"
                <| fun i ->
                    testNeutralElement TzolkinGlyph.create i 0

                testPropertyWithConfig config "addition with TimeSpan has neutral element"
                <| fun i ->
                    testNeutralElement TzolkinGlyph.create i (TimeSpan.FromDays 0.)

                testPropertyWithConfig config "addition is associative with int"
                <| fun i j k ->
                    testAssociativity TzolkinGlyph.create i j k

                testPropertyWithConfig config "addition is associative with TimeSpan"
                <| fun i j k ->
                    testAssociativity TzolkinGlyph.create
                        i
                        (TimeSpan.FromDays (float j))
                        (TimeSpan.FromDays (float k))

                testPropertyWithConfig config "addition is the same with TimeSpan and int"
                <| fun i j ->
                    testIntTimeSpanSame TzolkinGlyph.create
                        i
                        j
                        (TimeSpan.FromDays (float j))

                testPropertyWithConfig config "subtraction is same as addition"
                <| fun i j ->
                    testSubtraction TzolkinGlyph.create i j TzolkinGlyph.modulo20

                testCase "fromDate with reference date list"
                <| fun _ ->
                    testFromDate TzolkinGlyph.fromDate referenceDates

                testCase "getNextDate with a reference date list"
                <| fun _ ->
                    testNextDate TzolkinGlyph.getNext 20 referenceDates false

                testCase "getLastDate with a reference date list"
                <| fun _ ->
                    testNextDate TzolkinGlyph.getLast 20 referenceDates true

                testPropertyWithConfig configList "getNextList with a reference date list"
                <| fun i ->
                        testNextList
                            TzolkinGlyph.getNextList
                            TzolkinGlyph.fromDate
                            (abs i)
                            20
                            referenceDates
                            false

                testPropertyWithConfig configList "getLastList with a reference date list"
                <| fun i ->
                        testNextList
                                TzolkinGlyph.getLastList
                                TzolkinGlyph.fromDate
                                (abs i)
                                20
                                referenceDates
                                true

            ]
