// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TzolkinNumber.fs
//
//==============================================================================

/// Namespace containing all Maya calendar libraries.
namespace RC.Maya

open System

/// Module with the `TzolkinNumber` type and expressions.
///
/// Everything needed to use Tzolk’in day numbers, that are integers from 1 to 13
/// (including 1 and 13).
module TzolkinNumber =

    /// The maximum valid `TzolkinNumber`, 13.
    /// There exist 13 Tzolk’in day numbers.
    let maximum = 13


    /// The 13 Tzolk’in day numbers as Unicode symbols - works as soon as they are
    /// included in the standard. Actually that are the numbers from 1 to 20.
    let numberUnicode =
        [| "𕎍"
           "𕎐"
           "𕎓"
           "𕎖"
           "𕎙"
           "𕎜"
           "𕎟"
           "𕎢"
           "𕎥"
           "𕎨"
           "𕎫"
           "𕎮"
           "𕎱"
           "𕎴"
           "𕎶"
           "𕎸"
           "𕎺"
           "𕎼"
           "𕎾"
           "𕏀" |]

    /// Calculate Tzolk’in day 'modulo'.
    /// Calculate `n` % 13 and return 13 if `n` = 0 (mod 13), because a day 0 doesn't
    /// make sense and by returning 13 we still have a mathematical ring.
    ///
    /// Params:
    ///         `n` The integer to calculate the 'Tzolk’in day modulo' of.
    ///
    /// Returns:
    ///         13 if `n` = 0 (mod13)
    ///         `n` % 13 else.
    let modulo13 = Generics.modulo 13

    /// The Tzolk’in day number type.
    type T =
        | TzolkinNumber of int

        /// Convert a `TzolkinNumber` to an `int`.
        static member op_Explicit tz =
            match tz with
            | (TzolkinNumber n) -> n

        /// Add two `TzolkinNumber`.
        static member ( + ) (tz1: T, tz2: T) =
            int tz1 + int tz2
            |> modulo13
            |> TzolkinNumber

        /// Add an int to a `TzolkinNumber`.
        static member ( + ) (tz1: T, i: int) =
            int tz1 + i
            |> modulo13
            |> TzolkinNumber

        /// Add a `TzolkinNumber` to an int.
        static member ( + ) (i: int, tz1: T) =
            int tz1 + i
            |> modulo13
            |> TzolkinNumber

        /// Add a `TimeSpan` to a `TzolkinNumber`.
        static member (+) (tz1:T, days:TimeSpan) =
            int tz1 + days.Days
            |> modulo13
            |> TzolkinNumber

        /// Add a `TzolkinNumber` to a `TimeSpan`.
        static member (+) (days:TimeSpan, tz1:T) =
            days.Days + int tz1
            |> modulo13
            |> TzolkinNumber

        /// Convert the `TzolkinNumber` to a string.
        /// Now you can use `string` with a `TzolkinNumber`, like
        /// `string (TzolkinNumber.create 8)`
        override this.ToString() =
            int this |> string

    /// Reference Tzolk’in date. The 1st of January, 1970 is a Tzolk’in date of
    /// 13 Chikchan.
    let referenceDate = ("01.01.1970", TzolkinNumber 13)

    /// Constructor from an `int`.
    /// Return `None`, if `n` is not positive, 13 if `n` mod 13 is 0 (day `0` doesn't
    /// make sense and `13 = 0 (mod 13)`, so we still have a mathematical ring)
    /// otherwise `n % 13`.
    ///
    /// Params:
    ///         `n` The integer to convert to a `TzolkinNumber`
    /// Returns:
    ///         `None` if `n` is not positive
    ///         13 if `n` = 0 (mod 13)
    ///         n % 13 else
    let create n =
        match n with
        | i when i < 1 -> None
        | i -> Some (TzolkinNumber (modulo13 i))

    type T with
          /// Subtract two `TzolkinNumber`.
          static member ( - ) (tz1:T, tz2:T) =
              (int tz1) - (int tz2)

    /// Convert the given Gregorian date `gregorian` to a Tzolk’in day number.
    ///
    /// Params:
    ///         `gregorian` The Gregorian date to convert.
    ///
    /// Returns:
    ///          The Tzolk’in day number of the given Gregorian date.
    let fromDate gregorian =
        Generics.fromDate referenceDate gregorian

    /// Return the next Gregorian date after `start` with a Tzolk’in day number of
    /// `tzolkinDate`.
    /// If `start` has a Tzolk’in day number of `tzolkinDate` return the next Gregorian
    /// date with a Tzolk’in day number of `tzolkinDate` (260 days later).
    ///
    /// Params:
    ///          `tzolkinDate` The Tzolk’in day number to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          The next Gregorian date (forward in time after the date `start` that
    ///          has a Tzolk’in day number of `tzolkinDate`.
    let getNext tzolkinDate start =
        Generics.getNext referenceDate 13 tzolkinDate start


    /// Return a list of Gregorian dates after `start` with the same Tzolk’in day number
    /// `tzolkinDate`. The number of elements in the returned list is `numDates`.
    /// If `start` has a Tzolk’in day number of `tzolkinDate` the first element is the next
    /// Gregorian date with a Tzolk’in day number of `tzolkinDate` (260 days later).
    ///
    /// Params:
    ///          `numDates` The number of returned dates in the list.
    ///          `tzolkinDate` The Tzolk’in day number to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          A list with the next `numDates` Gregorian dates (forward in time after
    ///          the date `start`) that have the same Tzolk’in day numbers as `tzolkinDate`.
    let getNextList numDates tzolkinDate start =
        let rec getNextTzolkin = Generics.addDate (getNext tzolkinDate) numDates

        getNextTzolkin 0 start []

    /// Return the last Gregorian date before or the same as `start` with a Tzolk’in
    /// day number of `tzolkinDate`.
    /// If `start` has a Tzolk’in day number of `tzolkinDate` return the last Gregorian
    /// date with a Tzolk’in day number of `tzolkinDate` (13 days before).
    ///
    /// Params:
    ///          `tzolkinDate` The Tzolk’in day number to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          The last Gregorian date (backwards in time before the date `start` that
    ///          has a Tzolk’in day number of `tzolkinDate`.
    let getLast tzolkinDate start =
        let last =
            System.TimeSpan.FromDays -13.0
            |> (+) (getNext tzolkinDate start)

        if last = start then last + System.TimeSpan.FromDays -13.0 else last

    /// Return a list of Gregorian dates before `start` with the same Tzolk’in day number
    /// `tzolkinDate`. The number of elements in the returned list is `numDates`.
    /// If `start` has a Tzolk’in day number of `tzolkinDate` the first element is the last
    /// Gregorian date with a Tzolk’in day number of `tzolkinDate` (260 days before `start`).
    ///
    /// Params:
    ///          `numDates` The number of returned dates in the list.
    ///          `tzolkinDate` The Tzolk’in day number to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          A list with the last `numDates` Gregorian dates (backwards in time
    ///          before the date `start`) that have the same Tzolk’in day number as
    ///          `tzolkinDate`.
    let getLastList numDates tzolkinDate start =
        let rec getLastTzolkin = Generics.addDate (getLast tzolkinDate) numDates

        getLastTzolkin 0 start []

    /// Return the Tzolk’in day number as a Unicode symbol.
    /// This works as soon as the Tzolk’in day numbers are included in the Unicode
    /// standard.
    ///
    /// Params:
    ///         `number` The Tzolk’in day number to convert.
    ///
    /// Returns:
    ///         The Tzolk’in day number as a Unicode symbol.
    let toUnicode number =
        match number with
        | (TzolkinNumber i) -> numberUnicode.[(modulo13 i) - 1]

    /// Return a number between 1 and 20 as a Unicode symbol.
    /// This works as soon as the Tzolk’in numbers are included in the Unicode
    /// standard.
    ///
    /// Params:
    ///         `number` The number to convert.
    ///
    /// Returns:
    ///         The Maya number as a Unicode symbol.
    let toUnicodeNum number =
        let modulo20 = Generics.modulo 20

        numberUnicode.[(modulo20 number) - 1]
