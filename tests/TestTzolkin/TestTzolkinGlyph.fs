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
open Swensen.Unquote

open RC.Maya



[<AutoOpen>]
module TestTzolkinGlyph=

    let config = { config with
                          receivedArgs = fun _ name no args ->
                               loggerFuncDeb "TestTzolkinGlyph" name no args }

    let configList = { configList with
                               receivedArgs = fun _ name no args ->
                                    loggerFuncInfo "TestTzolkinGlyph" name no args }

    let configFromString = { configList with
                                receivedArgs = fun _ name no args ->
                                     loggerFuncDeb "TestTzolkinGlyph" name no args }

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
            [   testPropertyWithConfig config "addition is commutative"
                <| fun i j ->
                    testCommutativityType TzolkinGlyph.create i j

                testPropertyWithConfig config "addition with int is commutative"
                <| fun i (j: int) ->
                    testCommutativity TzolkinGlyph.create i j

                //testPropertyWithConfig config "addition with int is faster than timespan"
                //<| fun i j ->
                //    Expect.isFasterThan (fun () ->
                //                            Gen.choose (1, 1000) |> Gen.sample 0 10000
                //                            |> List.iter (fun elem ->
                //                                            (TzolkinGlyph.T.TzolkinGlyph i) + elem |> ignore)
                //                        )
                //                        (fun () ->
                //                            Gen.choose (1, 1000) |> Gen.sample 0 10000
                //                            |> List.iter (fun elem ->
                //                                            (TzolkinGlyph.T.TzolkinGlyph i) +
                //                                                TimeSpan.FromDays (float elem) |> ignore )
                //                        )
                //                        "int is faster than TimeSpan"

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
                <| fun i (j: int) (k: int) ->
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

                testPropertyWithConfig configFasterThan "toString"
                <| fun i ->
                    let tzolkin = TzolkinGlyph.create i
                    match tzolkin with
                    | None -> test <@ i < 1 @>
                    | Some tz ->
                            test <@ tz.ToString () = TzolkinGlyph.toString tz @>
                            test <@ tz.ToString () =
                                TzolkinGlyph.glyphNames.[TzolkinGlyph.modulo20 i - 1] @>

                testPropertyWithConfig configFasterThan "toInt"
                <| fun i ->
                    testToInt TzolkinGlyph.create TzolkinGlyph.modulo20 TzolkinGlyph.toInt i

                testPropertyWithConfig config "toUnicode"
                <| fun i ->
                    let tzolkin = TzolkinGlyph.create i
                    match tzolkin with
                    | None -> test <@ i < 1 @>
                    | Some tz ->
                            test <@ TzolkinGlyph.toUnicode tz =
                                        TzolkinGlyph.glyphUnicode.[TzolkinGlyph.modulo20 i - 1] @>

                testPropertyWithConfig configFasterThan "getDescription"
                <| fun i ->
                    let tzolkin = TzolkinGlyph.create i
                    match tzolkin with
                    | None -> test <@ i < 1 @>
                    | Some tz ->
                            test <@ TzolkinGlyph.getDescription tz =
                                        TzolkinGlyph.glyphDesc.[TzolkinGlyph.modulo20 i - 1] @>

                testPropertyWithConfig configFromString "fromString"
                <| fun i (name: string) ->
                    let tzolkinString = TzolkinGlyph.glyphNames.[TzolkinGlyph.modulo20 i - 1]
                    let tzolkin = TzolkinGlyph.fromString tzolkinString
                    match tzolkin with
                    | None -> test <@ "Should never happen" = "x" @>
                    | Some tz ->
                            test <@ tz = TzolkinGlyph.T.TzolkinGlyph (TzolkinGlyph.modulo20 i)  @>

                    let tzolkin1 = TzolkinGlyph.fromString name
                    match tzolkin1 with
                    | Some _ -> test <@ "Should never happen" = "x" @>
                    | None -> test <@ true @>


                testPropertyWithConfig configFasterThan  "parseString"
                <| fun i ->
                        let testStr, glyphStr = genGlyphTestString i
                        let tzolkin = TzolkinGlyph.parseString testStr
                        match tzolkin with
                        | None -> test <@ "Should never happen" = testStr @>
                        | Some tz -> test <@ TzolkinGlyph.toString tz = glyphStr @>


            ]
