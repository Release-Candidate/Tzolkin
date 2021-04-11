// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TzolkinNumber.fs
//
//==============================================================================

/// Namespace containing all Maya calendar libraries.
namespace RC.Maya

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
    let modulo13 n =
        match n with
        | i when i >= 0 -> if n % 13 = 0 then 13 else n % 13
        | _ -> if n % 13 = 0 then 13 else 13 + (n % 13)

    /// The Tzolk’in day number type.
    type T =
        | TzolkinNumber of int

        /// Convert a `TzolkinNumber` to an `int`.
        static member op_Explicit tz =
            match tz with
            | (TzolkinNumber n) -> n

        /// Add two `TzolkinNumber`.
        static member (+)(tz1: T, tz2: T) = int tz1 + int tz2 |> modulo13 |> TzolkinNumber

        /// Add an int to a `TzolkinNumber`.
        static member (+)(tz1: T, i: int) = int tz1 + i |> modulo13 |> TzolkinNumber

        /// Add a `TzolkinNumber` to an int.
        static member (+)(i: int, tz1: T) = int tz1 + i |> modulo13 |> TzolkinNumber

        /// Convert the `TzolkinNumber` to a string.
        /// Now you can use `string` with a `TzolkinNumber`, like
        /// `string (TzolkinNumber.create 8)`
        override this.ToString() = int this |> string

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
        let modulo20 n =
            match n with
            | i when i < 1 -> if i % 20 = 0 then 20 else abs (i) % 20
            | i -> if i % 20 = 0 then 20 else i % 20

        numberUnicode.[(modulo20 number) - 1]
