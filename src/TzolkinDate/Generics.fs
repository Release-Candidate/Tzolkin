﻿// SPDX-License-Identifier: MIT
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


  /// Convert the given Gregorian date `gregorian` to a Tzolk’in day glyph.
  ///
  /// Params:
  ///         `gregorian` The Gregorian date to convert.
  ///
  /// Returns:
  ///          The Tzolk’in day glyph of the given Gregorian date.
  let fromDate (referenceDate: string * ^T when ^T: (static member ( + ) : ^T * TimeSpan -> ^T)) gregorian =
      let (refDate, refTzolkin) = referenceDate
      let formatProvider = System.Globalization.DateTimeFormatInfo.InvariantInfo
      let reference = System.DateTime.ParseExact (refDate, "dd.MM.yyyy", formatProvider)
      refTzolkin + (gregorian - reference)

  /// Return the next Gregorian date after `start` with a Tzolk’in day glyph of
  /// `tzolkinDate`.
  /// If `start` has a Tzolk’in day glyph of `tzolkinDate` return the next Gregorian
  /// date with a Tzolk’in day glyph of `tzolkinDate` (260 days later).
  ///
  /// Params:
  ///          `tzolkinDate` The Tzolk’in day glyph to search for.
  ///          `start` The Gregorian date to start the search.
  ///
  /// Returns:
  ///          The next Gregorian date (forward in time after the date `start` that
  ///          has a Tzolk’in day glyph of `tzolkinDate`.
  let getNext referenceDate tzolkinDate  (start: DateTime) =
      let startTzolkin = fromDate referenceDate start
      let dayDiff = tzolkinDate - startTzolkin
      start + System.TimeSpan.FromDays (float dayDiff.Days) // FIXXME

  /// Add a `TzolkinGlyph`to the given list of `TzolkinGlyph`, to a length of `length`.
  /// Helper function.
  let rec private addDate getTzolkin length num start list =
      let next = getTzolkin start
      let nextNum = num + 1
      if nextNum < length
          then addDate getTzolkin length nextNum next (next :: list)
          else List.rev (next :: list)

  /// Return a list of Gregorian dates after `start` with the same Tzolk’in day glyph
  /// `tzolkinDate`. The number of elements in the returned list is `numDates`.
  /// If `start` has a Tzolk’in day glyph of `tzolkinDate` the first element is the next
  /// Gregorian date with a Tzolk’in day number of `tzolkinDate` (260 days later).
  ///
  /// Params:
  ///          `numDates` The number of returned dates in the list.
  ///          `tzolkinDate` The Tzolk’in day glyph to search for.
  ///          `start` The Gregorian date to start the search.
  ///
  /// Returns:
  ///          A list with the next `numDates` Gregorian dates (forward in time after
  ///          the date `start`) that have the same Tzolk’in day glyph as `tzolkinDate`.
  let getNextList referenceDate numDates tzolkinDate start =
      let rec getNextTzolkin = addDate (getNext referenceDate tzolkinDate) numDates

      getNextTzolkin 0 start []

  /// Return the last Gregorian date before or the same as `start` with a Tzolk’in
  /// day glyph of `tzolkinDate`.
  /// If `start` has a Tzolk’in day glyph of `tzolkinDate` return the last Gregorian
  /// date with a Tzolk’in day glyph of `tzolkinDate` (260 days before).
  ///
  /// Params:
  ///          `tzolkinDate` The Tzolk’in day glyph to search for.
  ///          `start` The Gregorian date to start the search.
  ///
  /// Returns:
  ///          The last Gregorian date (backwards in time before the date `start` that
  ///          has a Tzolk’in day glyph of `tzolkinDate`.
  let getLast referenceDate tzolkinDate start =
      let last = System.TimeSpan.FromDays -260.0 |> (+) (getNext referenceDate tzolkinDate start)
      if last = start then last + System.TimeSpan.FromDays -260.0 else last

  /// Return a list of Gregorian dates before `start` with the same Tzolk’in day glyph
  /// `tzolkinDate`. The number of elements in the returned list is `numDates`.
  /// If `start` has a Tzolk’in day glyph of `tzolkinDate` the first element is the last
  /// Gregorian date with a Tzolk’in day glyph of `tzolkinDate` (260 days before `start`).
  ///
  /// Params:
  ///          `numDates` The number of returned dates in the list.
  ///          `tzolkinDate` The Tzolk’in day glyph to search for.
  ///          `start` The Gregorian date to start the search.
  ///
  /// Returns:
  ///          A list with the last `numDates` Gregorian dates (backwards in time
  ///          before the date `start`) that have the same Tzolk’in day glyph as
  ///          `tzolkinDate`.
  let getLastList referenceDate numDates tzolkinDate start =
      let rec getLastTzolkin = addDate (getLast referenceDate tzolkinDate) numDates

      getLastTzolkin 0 start []
