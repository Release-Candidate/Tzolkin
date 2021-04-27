// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TestTzolkin
// File:     Generic.fs
// Date:     4/24/2021 1:11:17 PM
//==============================================================================


namespace TestTzolkin

open Expecto
open Swensen.Unquote
open System
open FsCheck
open Expecto.Logging

open RC.Maya

[<AutoOpen>]
module Generic=

    let private logger = Log.create "Tzolkin"

    let private loggerFunc logFunc moduleName name no args =
         logFunc (
            Message.eventX "{module} '{test}' #{no}, generated '{args}'"
            >> Message.setField "module" moduleName
            >> Message.setField "test" name
            >> Message.setField "no" no
            >> Message.setField "args" args )

    let loggerFuncDeb moduleName name no args =
        loggerFunc logger.debugWithBP moduleName name no args

    let loggerFuncInfo moduleName name no args =
        loggerFunc logger.infoWithBP moduleName name no args

    let config = { FsCheckConfig.defaultConfig with
                        maxTest = 10000
                        endSize = 1000000 }

    let configList = { FsCheckConfig.defaultConfig with
                            maxTest = 15
                            endSize = 500 }

    let configFasterThan = { FsCheckConfig.defaultConfig with
                                    maxTest = 100
                                    endSize = 1000000 }

    // Rules to calculate with `TzolkinNumber` and `TzolkinGlyph`. =================================

    let inline testCommutativityType constructor i j =
        let tzolkin1 = constructor i
        let tzolkin2 = constructor j
        match tzolkin1, tzolkin2 with
        | None, _ -> test <@ i < 1 @>
        | _, None -> test <@ j < 1 @>
        | Some tz1, Some tz2 -> test <@ tz1 + tz2 = tz2 + tz1 @>

    let inline testCommutativityTypeDate constructor i j k l =
        let tzolkin1 = constructor i j
        let tzolkin2 = constructor k l
        match tzolkin1, tzolkin2 with
        | None, _ -> test <@ i < 1 || j < 1 @>
        | _, None -> test <@ k < 1 || l < 1 @>
        | Some tz1, Some tz2 -> test <@ tz1 + tz2 = tz2 + tz1 @>

    let inline testCommutativity constructor i days =
        let tzolkin = constructor i
        match tzolkin with
        | None -> test <@ i < 1 @>
        | Some tz1 -> test <@ tz1 + days = days + tz1 @>

    let inline testCommutativityDate constructor i j days =
        let tzolkin = constructor i j
        match tzolkin with
        | None -> test <@ i < 1 || j < 1 @>
        | Some tz1 -> test <@ tz1 + days = days + tz1 @>

    let inline testNeutralElement constructor i neutral =
        let tzolkin = constructor i
        match tzolkin with
        | None -> test <@ i < 1 @>
        | Some tz1 -> test <@ tz1 + neutral = tz1 @>

    let inline testNeutralElementDate constructor i j neutral =
        let tzolkin = constructor i j
        match tzolkin with
        | None -> test <@ i < 1 || j < 1 @>
        | Some tz1 -> test <@ tz1 + neutral = tz1 @>

    let inline testAssociativity constructor i days1 days2 =
        let tzolkin = constructor i
        match tzolkin with
        | None -> test <@ i < 1 @>
        | Some tz1 -> test <@ (tz1 + days1) + days2 = tz1 + (days1 + days2) @>

    let inline testAssociativityDate constructor i j days1 days2 =
        let tzolkin = constructor i j
        match tzolkin with
        | None -> test <@ i < 1 || j < 1 @>
        | Some tz1 -> test <@ (tz1 + days1) + days2 = tz1 + (days1 + days2) @>

    let inline testIntTimeSpanSame constructor i daysI daysTS =
        let tzolkin = constructor i
        match tzolkin with
        | None -> test <@ i < 1 @>
        | Some tz1 -> test <@ tz1 + daysI = tz1 + daysTS @>

    let inline testIntTimeSpanSameDate constructor i j daysI daysTS =
           let tzolkin = constructor i j
           match tzolkin with
           | None -> test <@ i < 1 || j < 1 @>
           | Some tz1 -> test <@ tz1 + daysI = tz1 + daysTS @>

    let inline testSubtraction constructor i days modulo =
        let tzolkin = constructor i
        match tzolkin with
        | None -> test <@ i < 1 @>
        | Some tz1 ->
            let modDays = modulo days
            let tz2 = tz1 + modDays
            test <@ tz2 - tz1 = modDays @>

    let inline testSubtractionDate constructor i j days modulo =
           let tzolkin = constructor i j
           match tzolkin with
           | None -> test <@ i < 1 || j < 1 @>
           | Some tz1 ->
               let modDays = modulo days
               let tz2 = tz1 + modDays
               test <@ tz2 - tz1 = modDays @>

    let inline addTimespan (tzolkin:^T) days =
          (^T: (static member ( + ) : ^T * TimeSpan -> ^T) tzolkin, (TimeSpan.FromDays (float days)) )

    let inline addToTimespan days (tzolkin: ^T) =
          (^T: (static member ( + ) : TimeSpan * ^T -> ^T) (TimeSpan.FromDays (float days)), tzolkin)

    // date calculations ===========================================================================

    let inline stringToDate refString =
        let formatProvider = System.Globalization.DateTimeFormatInfo.InvariantInfo
        System.DateTime.ParseExact (refString, "dd.MM.yyyy", formatProvider)

    let inline stringList2DateList constructor list =
        List.map (fun (date, tzolkin) -> stringToDate date, constructor tzolkin) list

    // Test fromDate ===============================================================================
    let inline testFromDate fromDate referenceDates =
        let testFromDateHelper (fromDate: DateTime -> 'T) referenceDate date =
            test <@ fromDate date = referenceDate @>

        List.iter (fun elem ->
                        let date, tzolkin = elem
                        testFromDateHelper fromDate tzolkin date) referenceDates

    // Test getNext and getLast ====================================================================
    let inline testNextDate nextDate cycleLength referenceDates isLast =
        let testNextDateHelper2 nextDate cycleLength referenceDate days isLast =
            let (date:DateTime), tzolkin = referenceDate
            let start = date + TimeSpan.FromDays (float days)
            if isLast then
                test <@ nextDate tzolkin start = date @>
            else
                test <@ nextDate tzolkin start = date + TimeSpan.FromDays (float cycleLength) @>

        let testNextDateHelper1 nextDate cycleLength referenceDate isLast=
            if isLast then
                Gen.choose (1, cycleLength) |> Gen.sample 0 100
            else
                Gen.choose (0, cycleLength - 1) |> Gen.sample 0 100
            |> List.iter (fun i -> testNextDateHelper2 nextDate cycleLength referenceDate i isLast)

        referenceDates
        |> List.iter (fun elem -> testNextDateHelper1 nextDate cycleLength elem isLast)

    // Test getNextList and getLastList ============================================================
    let inline testNextList nextDateList fromDate numDates cycleLength referenceDates isLast =
        let rec testList list fromDate tzolkin difference =
            match list with
            | head :: (head2 :: tail) ->
                    test <@ head2 - head = TimeSpan.FromDays difference @>
                    test <@ fromDate head = tzolkin @>
                    testList (head2 :: tail) fromDate tzolkin difference
            | [head] -> test <@ fromDate head = tzolkin @>
            | [] -> ()

        let testNextListHelper2 fromDate numDates nextDateList cycleLength referenceDate days isLast =
            let (date:DateTime), tzolkin = referenceDate
            let start = date + TimeSpan.FromDays (float days)
            let listToTest: DateTime list = nextDateList numDates tzolkin start
            match listToTest, isLast with
            | [], _ -> test <@ numDates = 0 @>
            | _, true ->
                test <@ listToTest.Head = date @>
                testList listToTest fromDate tzolkin (float -cycleLength)
            | _, false ->
                test <@ listToTest.Head = date + TimeSpan.FromDays (float cycleLength) @>
                testList listToTest fromDate tzolkin (float cycleLength)

        let testNextListHelper1 numDates fromDate nextDateList cycleLength referenceDate isLast =
            Gen.choose (0, cycleLength - 1) |> Gen.sample 0 10
            |> List.iter (fun i -> testNextListHelper2
                                        fromDate
                                        numDates
                                        nextDateList
                                        cycleLength
                                        referenceDate
                                        i
                                        isLast)

        referenceDates
        |> List.iter (fun elem -> testNextListHelper1
                                        numDates
                                        fromDate
                                        nextDateList
                                        cycleLength
                                        elem
                                        isLast)

    // Conversion tests ============================================================================

    let inline testToInt constructor modulo toInt i =
        let tzolkin = constructor i
        match tzolkin with
        | None -> test <@ i < 1 @>
        | Some tz ->
                test <@ toInt tz = int tz @>
                test <@ toInt tz = modulo i @>

    let genGlyphTestString i =
        let glyphString = TzolkinGlyph.glyphNames.[TzolkinGlyph.modulo20 i - 1]
        let pointGen num =
                Gen.elements [" "; "\t"; "."; ","; ";"; "#"; "'"; "´"; "`"; "’" ]
                |> Gen.sample 0 num
        Seq.map
            (fun elem -> sprintf "%c%c" elem (pointGen 1 |> List.head |> Seq.head))
            glyphString
        |> String.Concat, glyphString
