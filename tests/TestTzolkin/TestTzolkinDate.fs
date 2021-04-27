// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TestTzolkinDate.fs
//
//==============================================================================

namespace TestTzolkin

open Expecto
open System
open Swensen.Unquote

open RC.Maya


[<AutoOpen>]
module TestTzolkinDate=

    let config = { config with
                       receivedArgs = fun _ name no args ->
                            loggerFuncDeb "TestTzolkinDate" name no args }

    let configList = { configList with
                            receivedArgs = fun _ name no args ->
                                 loggerFuncInfo "TestTzolkinDate" name no args }

    let inline stringList2DateListDate list =
        List.map (fun (date, number, glyph) ->
                        stringToDate date,
                        { TzolkinDate.Number = TzolkinNumber.T.TzolkinNumber number
                          TzolkinDate.Glyph = TzolkinGlyph.T.TzolkinGlyph glyph }
                  )
                  list

    let referenceDates = [  ("01.01.1800", 10, 14)
                            ("12.12.1926", 4, 19)
                            ("26.01.1958", 10, 7)
                            ("15.03.1967", 4, 2)
                            ("01.01.1970", 13, 5)
                            ("08.05.1975", 3, 18)
                            ("17.02.1978", 5, 14)
                            ("25.10.1986", 5, 6)
                            ("13.05.1992", 4, 13)
                            ("08.11.1997", 7, 18)
                            ("01.01.2000", 11, 2)
                            ("06.07.2005", 9, 15)
                            ("01.10.2017", 7, 5)
                            ("20.03.2021", 12, 11)
                         ]
                         |> stringList2DateListDate

    [<Tests>]
    let tests =
        testList
            "TzolkinDate"
            [   testPropertyWithConfig config "addition is commutative"
                <| fun i j k l ->
                    testCommutativityTypeDate TzolkinDate.fromInts i j k l

                testPropertyWithConfig config "addition with int is commutative"
                <| fun i j (k: int) ->
                    testCommutativityDate TzolkinDate.fromInts i j k

                testPropertyWithConfig config "addition with TimeSpan is commutative"
                <| fun i j k ->
                    testCommutativityDate TzolkinDate.fromInts i j (TimeSpan.FromDays (float k))

                testPropertyWithConfig config "addition with int has neutral element"
                <| fun i j ->
                    testNeutralElementDate TzolkinDate.fromInts i j 0

                testPropertyWithConfig config "addition with TimeSpan has neutral element"
                <| fun i j ->
                    testNeutralElementDate TzolkinDate.fromInts i j (TimeSpan.FromDays 0.)

                testPropertyWithConfig config "addition is associative with int"
                <| fun i j (k: int) (l: int) ->
                    testAssociativityDate TzolkinDate.fromInts i j k l

                testPropertyWithConfig config "addition is associative with TimeSpan"
                <| fun i j k l ->
                     testAssociativityDate TzolkinDate.fromInts
                        i
                        j
                        (TimeSpan.FromDays (float k))
                        (TimeSpan.FromDays (float l))

                testPropertyWithConfig config "addition is the same with TimeSpan and int"
                <| fun i j k ->
                     testIntTimeSpanSameDate TzolkinDate.fromInts
                        i
                        j
                        k
                        (TimeSpan.FromDays (float k))

                testPropertyWithConfig config "subtraction is same as addition"
                <| fun i j k ->
                    testSubtractionDate TzolkinDate.fromInts i j k TzolkinDate.modulo260

                testCase "fromDate with reference date list"
                <| fun _ ->
                    testFromDate TzolkinDate.fromDate referenceDates

                testCase "getNextDate with a reference date list"
                <| fun _ ->
                    testNextDate TzolkinDate.getNext 260 referenceDates false

                testCase "getLastDate with a reference date list"
                <| fun _ ->
                    testNextDate TzolkinDate.getLast 260 referenceDates true

                //testPropertyWithConfig config "getNextList2 is faster"
                //<| fun i ->
                //        Expect.isFasterThan (fun () ->
                //                                 let date, tzolkin = referenceDates.Head
                //                                 TzolkinDate.getNextList2
                //                                            (abs i)
                //                                            tzolkin
                //                                            date )
                //                            (fun () ->
                //                                 let date, tzolkin = referenceDates.Head
                //                                 TzolkinDate.getNextList
                //                                            (abs i)
                //                                            tzolkin
                //                                            date )
                //
                //                            "getNextList2 is faster"

                testPropertyWithConfig configList "getNextList with a reference date list"
                <| fun i ->
                        testNextList
                                TzolkinDate.getNextList
                                TzolkinDate.fromDate
                                (abs i)
                                260
                                referenceDates
                                false

                testPropertyWithConfig configList "getLastList with a reference date list"
                <| fun i ->
                        testNextList
                                TzolkinDate.getLastList
                                TzolkinDate.fromDate
                                (abs i)
                                260
                                referenceDates
                                true

                testPropertyWithConfig configFasterThan "toString"
                <| fun i j ->
                    let tzolkin = TzolkinDate.create
                                        (TzolkinNumber.T.TzolkinNumber <| TzolkinNumber.modulo13 i)
                                        (TzolkinGlyph.T.TzolkinGlyph <| TzolkinGlyph.modulo20 j)

                    test <@ tzolkin.ToString () = TzolkinDate.toString tzolkin @>
                    test <@ TzolkinDate.toString tzolkin = sprintf "%d %s"
                                (TzolkinNumber.modulo13 i)
                                TzolkinGlyph.glyphNames.[TzolkinGlyph.modulo20 j - 1] @>


                testPropertyWithConfig configFasterThan "fromStrings"
                <| fun i j ->
                        let number = TzolkinNumber.T.TzolkinNumber <| TzolkinNumber.modulo13 i
                        let glyphStr, glyph = genGlyphTestString j
                        let tzolkin = TzolkinDate.create
                                            number
                                            (TzolkinGlyph.T.TzolkinGlyph <| TzolkinGlyph.modulo20 j)
                        test <@ TzolkinDate.fromStrings (number.ToString ()) glyphStr = Some tzolkin @>
                        test <@ TzolkinDate.fromString (sprintf "%d %s" (int number) glyphStr) =
                                    Some tzolkin @>

                        test <@ TzolkinDate.fromStrings "FOO" "bar" = None @>
                        test <@ TzolkinDate.fromStrings "6" "bar" = None @>
                        test <@ TzolkinDate.fromString "bar" = None @>

                testPropertyWithConfig configFasterThan "fromDate* yield the same"
                <| fun i j k ->
                        let day = (abs i) % 28 + 1
                        let month = (abs j) % 12 + 1
                        let year = (abs k) % 2000 + 1000
                        test <@ TzolkinDate.fromDate (DateTime (year, month, day) ) =
                                    TzolkinDate.fromDateString
                                            (sprintf "%02d %02d %04d" day month year)
                                            "dd MM yyyy"
                                            @>
                        test <@ TzolkinDate.fromDate (DateTime (year, month, day) ) =
                            TzolkinDate.fromISOString
                                    (sprintf "%04d-%02d-%02d" year month day)
                                    @>


                testPropertyWithConfig configFasterThan "dayInYear"
                <| fun i ->
                          let tzolkinDay = (abs i) % 260 + 1
                          let tzolkinMap = TzolkinDate.yearStringMap ()
                          let tzolkinStr = tzolkinMap.[tzolkinDay]
                          let tzolkin = TzolkinDate.fromString tzolkinStr
                          match tzolkin with
                          | None -> test <@ "Should never happen" = "x" @>
                          | Some tz -> test <@ TzolkinDate.dayInYear tz = tzolkinDay @>

                testPropertyWithConfig configFasterThan "filterDateList"
                <| fun i j ->
                        let start = DateTime (1970, 01, 01)
                        let tzolkin = TzolkinDate.create
                                        (TzolkinNumber.T.TzolkinNumber <| TzolkinNumber.modulo13 i)
                                        (TzolkinGlyph.T.TzolkinGlyph <| TzolkinGlyph.modulo20 j)
                        let dateList = TzolkinDate.getNextList 5000 tzolkin start
                        let filterList str =
                                TzolkinDate.filterDateList str dateList
                        test <@ filterList "" = List.map
                                                    (fun (e: DateTime) -> e.ToShortDateString ())
                                                    dateList @>
                        let filterStr = "20"
                        List.iter
                            (fun (e: string) -> test <@ e.Contains filterStr @>)
                            (filterList filterStr)
            ]
