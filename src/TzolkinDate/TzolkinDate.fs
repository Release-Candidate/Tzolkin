// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TzolkinDate.fs
//
//==============================================================================

/// Namespace containing all Maya calendar libraries.
namespace RC.Maya

/// Module with the `TzolkinDate` type and expressions.
///
/// Contains everything needed to use `TzolkinDate`.
module TzolkinDate =

    open System
    open System.Text.RegularExpressions

    open TzolkinNumber

    open TzolkinGlyph

    /// The number of days in a Tzolk’in year, 260, that is 13 (number of day numbers)
    /// times 20, the number of day glyphs.
    let daysInYear = 260

    /// Calculate Tzolk’in day in a year 'modulo'.
    /// Calculate `n` % 260 and return 260 if `n` = 0 (mod 260), because a day 0 doesn't
    /// make sense and by returning 260 we still have a mathematical ring.
    ///
    /// Params:
    ///         `n` The integer to calculate the 'Tzolk’in day modulo' of.
    ///
    /// Returns:
    ///         260 if `n` = 0 (mod20)
    ///         `n` % 260 else.
    let modulo260 n = if n % 260 = 0 then 260 else n % 260

    /// The `TzolkinDate` type, holding the Tzolk’in day number in `number` and day
    /// glyph in `glyph`.
    type T =
        { Number: TzolkinNumber.T
          Glyph: TzolkinGlyph.T }

        /// Convert a `TzolkinDate` to a string, containing the day number and the day
        /// glyph as a string. Like "1 Imix" or "20 Ajaw".
        /// Because of that you can use `string` with a `TzolkinDate`, like
        /// `string TzolkinDate.create 134`
        override this.ToString() =
            match this with
            | { Number = num; Glyph = glph } -> $"{string num} {string glph}"

        /// Add days to a `TzolkinDate`.
        static member ( + ) (tzolkinDate, i: int) =
            { Number = tzolkinDate.Number + i
              Glyph = tzolkinDate.Glyph + i }

        /// Add days to a `TzolkinDate`, order of `int` and `TzolkinDate` changed.
        static member ( + ) (i: int, tzolkinDate) =
            { Number = tzolkinDate.Number + i
              Glyph = tzolkinDate.Glyph + i }

        /// Add two `TzolkinDate`. Doesn't really make sense, what would that be
        /// conceptually? But define it anyway.
        static member ( + )(tzolkinDate1, tzolkinDate2) =
            { Number = tzolkinDate1.Number + tzolkinDate2.Number
              Glyph = tzolkinDate1.Glyph + tzolkinDate2.Glyph }

        /// Add a `System.TimeSpan` to a `TzolkinDate`.
        /// Only makes sense with (at least) days, not hours, minutes, or seconds.
        static member ( + ) (tzolkinDate, timeSpan: System.TimeSpan) =
            { Number = tzolkinDate.Number + timeSpan.Days
              Glyph = tzolkinDate.Glyph + timeSpan.Days }

        /// Add a `System.TimeSpan` to a `TzolkinDate`, other order of `TimeSpan` and
        /// `TzolkinDate`.
        /// Only makes sense with (at least) days, not hours, minutes, or seconds.
        static member ( + ) (timeSpan: System.TimeSpan, tzolkinDate) =
            { Number = tzolkinDate.Number + timeSpan.Days
              Glyph = tzolkinDate.Glyph + timeSpan.Days }


    /// A map of all 260 Tzolk’in days in a year.
    /// The key is the number of the `TzolkinDate` in the Tzolk’in year, a number
    /// between 1 and 260 (including both), the value is the `TzolkinDate` of this day.
    let yearMap =
        Map [ for i in [ 1 .. 260 ] ->
                  (i,
                   { Number = (i |> TzolkinNumber.modulo13 |> TzolkinNumber)
                     Glyph = (i |> TzolkinGlyph.modulo20 |> TzolkinGlyph) }) ]

    /// Return all 260 Tzolk’in days in a year as strings in a map.
    /// The keys of this map are the number of the day in the Tzolk’in year, the value
    /// is the string of the Tzolk’in day.
    let yearStringMap () =
        Map.map (fun key value -> value.ToString()) yearMap

    /// Convert a `TzolkinDate` to a string, containing the day number and the day
    /// glyph as a string. Like "1 Imix" or "20 Ajaw".
    ///
    /// Params:
    ///         `tzolkin` The `TzolkinDate` to convert to a string.
    ///
    /// Returns:
    ///         The `TzolkinDate` converted to a string.
    let toString tzolkin =
        tzolkin.ToString ()

    /// Create a `TzolkinDate` from the `TzolkinNumber`and the
    /// `TzolkinGlyph` of this Tzolk’in day.
    ///
    /// Params:
    ///          `tzolkinNumber` The Tzolk’in day number of the Tzolk’in day, a integer
    ///                          between 1 and 13 (including both).
    ///          `tzolkinGlyph` The Tzolk’in day glyph of the Tzolk’in day, a integer
    ///                         between 1 and 20, including 1 and 20.
    ///
    /// Returns:
    ///          The `TzolkinDate` with the Tzolk’in day number `tzolkinNumber` and day
    ///           glyph `tzolkinGlyph` if such a date exists, `None` else.
    let create tzolkinNumber tzolkinGlyph =
        { Number = tzolkinNumber
          Glyph = tzolkinGlyph }

    /// Create a `TzolkinDate` from two ints. The first one `number` is the Tzolk’in day
    /// number, the second one, `glyph` the Tzolk’in day glyph.
    ///
    /// Params:
    ///          `number` The Tzolk’in day number of the Tzolk’in day, a integer between
    ///                   1 and 13 (including both).
    ///          `glyph` The Tzolk’in day glyph of the Tzolk’in day, a integer between
    ///                   1 and 20, including 1 and 20.
    ///
    /// Returns:
    ///          The `TzolkinDate` with the Tzolk’in day number `number` and day glyph
    ///          `glyph` if such a date exists, `None` else.
    let fromInts number glyph =
        let tzolkinNumber = TzolkinNumber.create number
        let tzolkinGlyph = TzolkinGlyph.create glyph

        match (tzolkinNumber, tzolkinGlyph) with
        | (None, _) -> None
        | (_, None) -> None
        | (Some tn, Some tg) -> Some { Number = tn; Glyph = tg }

    /// Create a `TzolkinDate` from two strings. The first one `number` is the Tzolk’in day
    /// number, the second one, `glyph` the Tzolk’in day glyph.
    /// In the day glyph string `name`, many punctuation characters are ignored and the
    /// case (lower- or uppercase) is ignored too.
    /// So, this function is suitable to parse user input.
    ///
    /// Params:
    ///         `number` The Tzolk’in day number of the Tzolk’in day, a integer
    ///                   between 1 and 13 (including both).
    ///          `glyph` The Tzolk’in day glyph of the Tzolk’in day, one of
    ///                   `TzolkinGlyph.glyphNames`.
    ///
    /// Returns:
    ///          A (`Some`) `TzolkinDate` on success.
    ///         `None`, if the input has been invalid.
    let fromStrings number name =
        let parseString (text: string) =
            try
                Some(int text)
            with excp -> None

        let parsedNumber = parseString number
        let parsedName = TzolkinGlyph.parseString name

        match (parsedNumber, parsedName) with
        | (None, _) -> None
        | (_, None) -> None
        | (Some i, Some t) -> fromInts i (int t)

    /// Create a `TzolkinDate` from one string. The first part of `dayName` is the
    /// Tzolk’in day number, the second one the Tzolk’in day glyph.
    /// In the day glyph string, many punctuation characters are ignored and the
    /// case (lower- or uppercase) is ignored too.
    /// The day number and the day glyph can either be separated by whitespace or a
    /// punctuation character, or not separated at all.
    /// So, this function is suitable to parse user input.
    ///
    /// Params:
    ///         `dayName` The Tzolk’in day number and glyph name of the Tzolk’in day.
    ///
    /// Returns:
    ///          A (`Some`) `TzolkinDate` on success.
    ///         `None`, if the input has been invalid.
    let fromString dayName =
        let result =
            Regex.Match(dayName, @"([0-9]+)[\s\p{P}\p{Pc}]*(.*)")

        if result.Success then
            fromStrings (result.Groups.[1].ToString()) (result.Groups.[2].ToString())
        else
            None


    /// Reference Tzolk’in date. The 1st of January, 1970 is a Tzolk’in date of
    /// 13 Chikchan.
    let referenceDate =
        ("01.01.1970", { Number = TzolkinNumber 13; Glyph = TzolkinGlyph 5 })

    /// Convert the given Gregorian date `gregorian` to a Tzolk’in date.
    ///
    /// Params:
    ///         `gregorian` The Gregorian date to convert.
    ///
    /// Returns:
    ///          The Tzolk’in day of the given Gregorian date.
    let fromDate gregorian =
        Generics.fromDate referenceDate gregorian

    /// Get the Tzolk’in date of today.
    let today = fromDate System.DateTime.Now

    /// Convert the given Gregorian date string `gregorianStr` to a Tzolk’in date.
    ///
    /// Examples:
    ///           to get the Tzolk’in date of the 5th of April, 2021
    ///           TzolkinDate.fromDateString "05.04.2021" "dd.MM.yyyy"
    ///           TzolkinDate.fromDateString "2021-04-05" "yyyy-MM-dd"
    ///           TzolkinDate.fromDateString "04/05/2021" "MM/dd/yyyy"
    ///
    /// Params:
    ///         `gregorianStr` The Gregorian date string to convert.
    ///         `format` The format string to use to parse the date.
    ///
    /// Returns:
    ///          The Tzolk’in day of the given Gregorian date.
    let fromDateString gregorianStr format =
        let formatProvider =
            System.Globalization.DateTimeFormatInfo.InvariantInfo

        System.DateTime.ParseExact(gregorianStr, format, formatProvider)
        |> fromDate

    /// Convert the given ISO Gregorian date string `isoDateStr` to a Tzolk’in date.
    /// A ISO date has the format "yyyy-MM-dd", like "2021-04-25" for the 25th of April,
    /// 2021.
    ///
    /// Params:
    ///         `isoDateStr` The ISO Gregorian date string to convert.
    ///
    /// Returns:
    ///          The Tzolk’in day of the given Gregorian date.
    let fromISOString isoDateStr =
        let formatProvider =
            System.Globalization.DateTimeFormatInfo.InvariantInfo

        System.DateTime.ParseExact(isoDateStr, "yyyy-MM-dd", formatProvider)
        |> fromDate


    /// Return the day in the Tzolk’in year, an integer between 1 and 260
    /// (including 1 and 260).
    /// Same as `dayInYear`.
    ///
    /// Params:
    ///         `tzolkinDate` The Tzolk’in date to return the day in the Tzolk’in year
    ///         of.
    ///
    /// Returns:
    ///          The day in the Tzolk’in year of the given Tzolk’in date `tzolkinDate`.
    ///          A integer between 1 and 260, including 1 and 260.
    let toInt (tzolkinDate: T) =
        Map.findKey (fun key v -> v = tzolkinDate) yearMap

    /// Return the day in the Tzolk’in year, an integer between 1 and 260
    /// (including 1 and 260).
    /// Same as `toInt`.
    ///
    /// Params:
    ///         `tzolkinDate` The Tzolk’in date to return the day in the Tzolk’in year
    ///         of.
    ///
    /// Returns:
    ///          The day in the Tzolk’in year of the given Tzolk’in date `tzolkinDate`.
    ///          A integer between 1 and 260, including 1 and 260.
    let dayInYear tzolkinDate = toInt tzolkinDate


    type T with
        /// Calculate the difference in days between two Tzolk’in days.
        /// `tzolkinDate1 - tzolkinDate2` returns the days between `tzolkinDate1` and
        /// `tzolkinDate2` if `tzolkinDate1` comes after `tzolkinDate2` in time.
        /// If `tzolkinDate1` is earlier in time than `tzolkinDate2`,
        /// `tzolkinDate1 - tzolkinDate2` returns `260 - date2 + date1` (the same as
        /// `260 - (date2 - date1)`) where `date1` and `date2` are the numbers of the
        /// Tzolk’in days `tzolkinDate1`and ``tzolkinDate2` in the Tzolk’in year.
        ///
        /// Example:
        ///         returns 12 for `tzolkinDate2` = 4 Manikʼ and
        ///                         `tzolkinDate1` = 3 Kawak
        ///         returns 250 for `tzolkinDate2` = 8 Chuwen and
        ///                         `tzolkinDate1` = 11 Imix
        static member ( - )(tzolkinDate1, tzolkinDate2) =
            let day2 = toInt tzolkinDate2
            let day1 = toInt tzolkinDate1

            if day1 > day2 then
                day1 - day2
            else
                260 - day2 + day1

    /// Return the next Gregorian date after `start` with a Tzolk’in date of
    /// `tzolkinDate`.
    /// If `start` has a Tzolk’in date of `tzolkinDate` return the next Gregorian
    /// date with a Tzolk’in date of `tzolkinDate` (260 days later).
    ///
    /// Params:
    ///          `tzolkinDate` The Tzolk’in day to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          The next Gregorian date (forward in time after the date `start` that
    ///          has a Tzolk’in date of `tzolkinDate`.
    let getNext tzolkinDate start =
        Generics.getNext referenceDate 260 tzolkinDate start

    /// Return a list of Gregorian dates after `start` with the same Tzolk’in date
    /// `tzolkinDate`. The number of elements in the returned list is `numDates`.
    /// If `start` has a Tzolk’in date of `tzolkinDate` the first element is the next
    /// Gregorian date with a Tzolk’in date of `tzolkinDate` (260 days later).
    ///
    /// Params:
    ///          `numDates` The number of returned dates in the list.
    ///          `tzolkinDate` The Tzolk’in day to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          A list with the next `numDates` Gregorian dates (forward in time after
    ///          the date `start`) that have the same Tzolk’in dates as `tzolkinDate`.
    let getNextList numDates tzolkinDate start =
        Generics.getNextList referenceDate 260 numDates tzolkinDate start

    /// Return the last Gregorian date before or the same as `start` with a Tzolk’in
    /// date of `tzolkinDate`.
    /// If `start` has a Tzolk’in date of `tzolkinDate` return the last Gregorian
    /// date with a Tzolk’in date of `tzolkinDate` (260 days before).
    ///
    /// Params:
    ///          `tzolkinDate` The Tzolk’in day to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          The last Gregorian date (backwards in time before the date `start` that
    ///          has a Tzolk’in date of `tzolkinDate`.
    let getLast tzolkinDate start =
        Generics.getLast referenceDate 260 tzolkinDate start

    /// Return a list of Gregorian dates before `start` with the same Tzolk’in date
    /// `tzolkinDate`. The number of elements in the returned list is `numDates`.
    /// If `start` has a Tzolk’in date of `tzolkinDate` the first element is the last
    /// Gregorian date with a Tzolk’in date of `tzolkinDate` (260 days before `start`).
    ///
    /// Params:
    ///          `numDates` The number of returned dates in the list.
    ///          `tzolkinDate` The Tzolk’in day to search for.
    ///          `start` The Gregorian date to start the search.
    ///
    /// Returns:
    ///          A list with the last `numDates` Gregorian dates (backwards in time
    ///          before the date `start`) that have the same Tzolk’in dates as
    ///          `tzolkinDate`.
    let getLastList numDates tzolkinDate start =
        Generics.getLastList referenceDate 260 numDates tzolkinDate start


    /// Filter the given list of Gregorian dates by the string `filterStr`.
    /// If `filterStr` is contained in the  locale short date ("dd.MM.yyyy" or
    /// "MM/dd/yyyy" or ...), this date is returned in the list.
    ///
    /// Params:
    ///         `filterStr` The string that must be contained in a date for it to be
    ///         included in the returned list.
    ///         `list` the list of dates to filter.
    ///
    /// Returns:
    ///          A list of all dates as strings, that contain `filterStr`.
    let filterDateList (filterStr: string) list =
        list
        |> List.map (fun (d: DateTime) -> d.ToShortDateString())
        |> List.filter (fun s -> s.Contains filterStr)
