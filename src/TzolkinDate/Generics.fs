// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinDate
// File:     Generics.fs
// Date:     4/15/2021 9:20:59 PM
//==============================================================================

/// Namespace containing all Maya calendar libraries.
namespace RC.Maya

open System

/// Module holding generic functions to use with all 3 Tzolk’in types.
module Generics=

  /// Special 'modulo', always returns a positive number and `m`, if
  /// `n = 0 (mod m)`.
  let internal modulo m n =
      match n with
      | i when i >= 0 -> if n % m = 0 then m else n % m
      | _ -> if n % m = 0 then m else m + (n % m)

  /// Convert the given Gregorian date `gregorian` to a Tzolk’in day type.
  let inline internal fromDate
        (referenceDate: string * ^T)
        (gregorian: DateTime)
        : ^T
        =
        let (refDate, refTzolkin) = referenceDate
        let formatProvider = System.Globalization.DateTimeFormatInfo.InvariantInfo
        let reference = System.DateTime.ParseExact (refDate, "dd.MM.yyyy", formatProvider)
        let dayDiff: TimeSpan = gregorian - reference
        (^T: (static member ( + ) : ^T * TimeSpan -> ^T) refTzolkin, dayDiff)

  /// Return the next Gregorian date after `start` with a Tzolk’in day type.
  let inline internal getNext
        (referenceDate: string * ^T)
        (cycleLength: int)
        (tzolkinDate: ^T)
        (start: DateTime)
        =
        let startTzolkin = fromDate referenceDate start
        let dayDiff = if tzolkinDate - startTzolkin = 0 then cycleLength else tzolkinDate - startTzolkin
        start + TimeSpan.FromDays (float dayDiff)

  ///// Add a gregorian date of a Tzolk’in day to the given list, to a length of `length`.
  //let rec private addDate getTzolkin length num start list =
  //    let next = getTzolkin start
  //    let nextNum = num + 1
  //    if nextNum < length
  //        then addDate getTzolkin length nextNum next (next :: list)
  //        else List.rev (next :: list)

  /// Return the last Gregorian date before or the same as `start` with a Tzolk’in
  /// day of `tzolkinDate`.
  let inline internal getLast referenceDate (cycleLength: int) tzolkinDate start =
      let last =
        TimeSpan.FromDays (float -cycleLength)
        |> (+) (getNext referenceDate cycleLength tzolkinDate start)

      if last = start then last + TimeSpan.FromDays (float -cycleLength) else last

  /// Helper function.
  //let inline private getList getFunc referenceDate cycleLength numDates tzolkinDate start =
  //      let rec getNextTzolkin = addDate (getFunc referenceDate cycleLength tzolkinDate) numDates

  //      getNextTzolkin 0 start []

  /// Return a list of gregorian dates with the given Tzolk’in date.
  let inline internal getNextList referenceDate cycleLength numDates tzolkinDate start =
        let first = getNext referenceDate cycleLength tzolkinDate start
        [ for idx in 0. .. float (numDates - 1) -> first + TimeSpan.FromDays (idx * float cycleLength) ]


  /// Return a list of gregorian dates with the given Tzolk’in date.
  //let inline internal getLastList referenceDate cycleLength numDates tzolkinDate start =
  //      let lastList = getList getLast referenceDate cycleLength numDates tzolkinDate start
  //      if tzolkinDate = fromDate referenceDate start then
  //          start :: lastList
  //      else
  //          lastList

  let inline internal getLastList referenceDate cycleLength numDates tzolkinDate start =
        let first = if tzolkinDate = fromDate referenceDate start then
                        start
                    else
                        getLast referenceDate cycleLength tzolkinDate start
        [ for idx in 0. .. float (numDates - 1) -> first - TimeSpan.FromDays (idx * float cycleLength) ]
