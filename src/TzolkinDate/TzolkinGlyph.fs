// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TzolkinGlyph.fs
//
//==============================================================================

/// Namespace containing all Maya calendar libraries.
namespace RC.Maya

open System

/// Module with the `TzolkinGlyph` type and expression.
///
/// Everything needed to use the Tzolk’in day glyphs, that are integers from 1 to 20
/// (including 1 and 20) and the glyph names.
module TzolkinGlyph =

    open System.Text.RegularExpressions

    /// The maximum value of `TzolkinGlyph`, 20.
    /// There exist 20 Tzolk’in day glyphs.
    let maximum = 20

    /// The 20 Tzolk’in day glyph names.
    let glyphNames =
        [| "Imix"
           "Ikʼ"
           "Akʼbʼal"
           "Kʼan"
           "Chikchan"
           "Kimi"
           "Manikʼ"
           "Lamat"
           "Muluk"
           "Ok"
           "Chuwen"
           "Ebʼ"
           "Bʼen"
           "Ix"
           "Men"
           "Kʼibʼ"
           "Kabʼan"
           "Etzʼnabʼ"
           "Kawak"
           "Ajaw" |]

    /// The 20 Tzolk’in day glyphs as Unicode symbols - works as soon as they are
    /// included in the standard.
    let glyphUnicode =
        [| "𕏢"
           "𕏧"
           "𕏩"
           "𕏬"
           "𕏯"
           "𕏲"
           "𕏵"
           "𕏷"
           "𕏻"
           "𕏿"
           "𕐃"
           "𕐆"
           "𕐊"
           "𕐌"
           "𕐏"
           "𕐒"
           "𕐖"
           "𕐚"
           "𕐝"
           "𕐟" |]

    /// A record to hold the information about each Tzolk’in day glyph.
    /// See https://arqueologiamexicana.mx/dias-mayas.
    type GlyphDescription =
        { Meaning: string
          ElementOrAnimal: string
          Direction: string
          Color: string
          God: string
          Url: string }

    /// Description of Tzolk’in day glyph Men.
    let private _descMen =
        { Meaning = "águila"
          ElementOrAnimal = "águila"
          Direction = "oeste"
          Color = "negro"
          God = "Ix Chel, diosa de la Luna"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Ikʼ.
    let private _descIk =
        { Meaning = "viento"
          ElementOrAnimal = "viento"
          Direction = "norte"
          Color = "blanco"
          God = "dios B o Chaac"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Chickchan.
    let private _descChikchan =
        { Meaning = "serpiente celeste"
          ElementOrAnimal = "serpiente"
          Direction = "este"
          Color = "rojo"
          God = "Chikchan, dios del número 9"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Imix.
    let private _descImix =
        { Meaning = "caimán, cocodrilo"
          ElementOrAnimal = "superficie terrestre"
          Direction = "este"
          Color = "rojo"
          God = "monstruo de la tierra"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Kawak.
    let private _descKawak =
        { Meaning = "tormenta"
          ElementOrAnimal = "lluvia, tormenta"
          Direction = "oeste"
          Color = "negro"
          God = "Itzamnaaj"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Ajaw.
    let private _descAjaw =
        { Meaning = "señor, Slnko, Kvet"
          ElementOrAnimal = "Sol"
          Direction = "sur"
          Color = "amarillo"
          God = "dios G o dios del Sol"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Etzʼnabʼ.
    let private _descEtznab =
        { Meaning = "pedernal, Nôž"
          ElementOrAnimal = "pedernal"
          Direction = "norte"
          Color = "blanco"
          God = "dios Q o dios de la guerra y los sacrificios"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Kabʼan.
    let private _descKaban =
        { Meaning = "Tierra, Pohyb"
          ElementOrAnimal = "Tierra y los temblores"
          Direction = "este"
          Color = "rojo"
          God = "diosa I o diosa de la sensualidad y el amor"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Kʼibʼ.
    let private _descKib =
        { Meaning = "cera, Sova, Sup"
          ElementOrAnimal = "venado, insectos"
          Direction = "sur"
          Color = "amarillo"
          God = "dios N o Pawahtún"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Ix.
    let private _descIx =
        { Meaning = "jaguar"
          ElementOrAnimal = "jaguar"
          Direction = "norte"
          Color = "blanco"
          God = "dios jaguar"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Bʼen.
    let private _descBen =
        { Meaning = "maíz verde, Rákosie"
          ElementOrAnimal = "maíz"
          Direction = "este"
          Color = "rojo"
          God = "dios E o dios del maíz"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Ebʼ.
    let private _descEb =
        { Meaning = "rocío, Cesta, Bylina"
          ElementOrAnimal = "gato del monte"
          Direction = "sur"
          Color = "amarillo"
          God = "dios de las lluvias dañinas"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Chuwen.
    let private _descChuwen =
        { Meaning = "mono"
          ElementOrAnimal = "mono"
          Direction = "oeste"
          Color = "negro"
          God = "dios C o K’u"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Ok.
    let private _descOk =
        { Meaning = "perro"
          ElementOrAnimal = "perro"
          Direction = "norte"
          Color = "blanco"
          God = "el perro en su advocación de dios del inframundo"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Muluk.
    let private _descMuluk =
        { Meaning = "jade, agua"
          ElementOrAnimal = "agua"
          Direction = "este"
          Color = "rojo"
          God = "pez xoc, jaguar"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Lamat.
    let private _descLamat =
        { Meaning = "Venus, Králik"
          ElementOrAnimal = "conejo"
          Direction = "sur"
          Color = "amarillo"
          God = "Lahun Chan, Venus"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Manikʼ.
    let private _descManik =
        { Meaning = "venado, Jeleň"
          ElementOrAnimal = "venado"
          Direction = "oeste"
          Color = "negro"
          God = "dios R o Buluk Ch’Abtan, dios de la Tierra"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Kimi.
    let private _descKimi =
        { Meaning = "muerte"
          ElementOrAnimal = "muerte"
          Direction = "norte"
          Color = "blanco"
          God = "dios A o dios de la muerte"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Kʼan.
    let private _descKan =
        { Meaning = "maíz maduro, Jašterica"
          ElementOrAnimal = "maíz y abundancia"
          Direction = "sur"
          Color = "amarillo"
          God = "dios E o dios del maíz"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// Description of Tzolk’in day glyph Akʼbʼal.
    let private _descAkbal =
        { Meaning = "oscuridad"
          ElementOrAnimal = "oscuridad, noche, jaguar"
          Direction = "oeste"
          Color = "negro"
          God = "Chaac Bolay, jaguar de nenúfar"
          Url = "https://arqueologiamexicana.mx/dias-mayas" }

    /// The descriptions of the 20 Tzolk’in day glyphs.
    let glyphDesc =
        [| _descImix
           _descIk
           _descAkbal
           _descKan
           _descChikchan
           _descKimi
           _descManik
           _descLamat
           _descMuluk
           _descOk
           _descChuwen
           _descEb
           _descBen
           _descIx
           _descMen
           _descKib
           _descKaban
           _descEtznab
           _descKawak
           _descAjaw |]

    /// Calculate Tzolk’in glyph 'modulo'.
    /// Calculate `n` % 20 and return 20 if `n` = 0 (mod 20), because a day 0 doesn't
    /// make sense and by returning 20 we still have a mathematical ring.
    ///
    /// Params:
    ///         `n` The integer to calculate the 'Tzolk’in day modulo' of.
    ///
    /// Returns:
    ///         20 if `n` = 0 (mod 20)
    ///         `n` % 20 else.
    let modulo20 = Generics.modulo 20

    /// The Tzolk’in day glyph type.
    type T =
        | TzolkinGlyph of int

        /// Convert a `TzolkinGlyph` to an `int`.
        static member op_Explicit tz =
            match tz with
            | (TzolkinGlyph n) -> n

        /// Add two `TzolkinGlyph`.
        static member ( + ) (glyph1: T, glyph2: T) =
            int glyph1 + int glyph2
            |> modulo20
            |> TzolkinGlyph

        /// Add an int to a `TzolkinGlyph`.
        static member ( + ) (glyph1: T, i: int) =
            let (TzolkinGlyph tz1) = glyph1
            tz1 + i
            |> modulo20
            |> TzolkinGlyph

        /// Add a `TzolkinGlyph` to an int.
        static member ( + ) (i: int, glyph1: T) =
            let (TzolkinGlyph tz1) = glyph1
            i + tz1
            |> modulo20
            |> TzolkinGlyph

        /// Add a `TimeSpan` to a `TzolkinGlyph`.
        static member ( + ) (glyph1: T, days: TimeSpan) =
            int glyph1 + days.Days
            |> modulo20
            |> TzolkinGlyph

        /// Add a `TzolkinGlyph` to a `TimeSpan`.
        static member ( + ) (days: TimeSpan, glyph: T) =
            days.Days + int glyph
            |> modulo20
            |> TzolkinGlyph

        /// Convert a `TzolkinGlyph` to a string.
        /// Now you can use `string` with a `TzolkinGlyph`, like
        /// `string (TzolkinGlyph.create 4)`
        override this.ToString () =
            match this with
            | (TzolkinGlyph i) -> glyphNames.[(modulo20 i) - 1]

    /// Reference Tzolk’in date. The 1st of January, 1970 is a Tzolk’in date of
    /// 13 Chikchan.
    let referenceDate = ("01.01.1970", TzolkinGlyph 5)

    /// Constructor from an `int`.
    /// Return `None`, if `n` is not positive, 20 if `n` mod 20 is 0 (day `0` doesn't
    /// make sense and `20 = 0 (mod 20)`, so we still have a mathematical ring)
    /// otherwise `n % 20`.
    ///
    /// Params:
    ///         `n` The integer to convert to a `TzolkinGlyph`
    ///
    /// Returns:
    ///         `None` if `n` is not positive
    ///         20 if `n` = 0 (mod 20)
    ///         n % 20 else
    let create n =
        match n with
        | i when i < 1 -> None
        | i -> Some (TzolkinGlyph (modulo20 i))

    type T with
        /// Subtract two `TzolkinGlyph`.
        static member ( - ) (tz1: T, tz2: T) =
            let day1 = int tz1
            let day2 = int tz2
            if day1 > day2 then
                  day1 - day2
            else
                  20 - day2 + day1

    /// Convert the given Gregorian date `gregorian` to a Tzolk’in day glyph.
    ///
    /// Params:
    ///         `gregorian` The Gregorian date to convert.
    ///
    /// Returns:
    ///          The Tzolk’in day glyph of the given Gregorian date.
    let fromDate gregorian =
        Generics.fromDate referenceDate gregorian

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
    let getNext tzolkinDate start =
        Generics.getNext referenceDate 20 tzolkinDate start


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
    let getNextList numDates tzolkinDate start =
        Generics.getNextList referenceDate 20 numDates tzolkinDate start

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
    let getLast tzolkinDate start =
        Generics.getLast referenceDate 20 tzolkinDate start

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
    let getLastList numDates tzolkinDate start =
       Generics.getLastList referenceDate 20 numDates tzolkinDate start

    /// Convert a `TzolkinGlyph` to an int.
    ///
    /// Params:
    ///         `tzolkin` The `TzolkinGlyph` to convert to an int.
    ///
    /// Returns:
    ///         The `TzolkinGlyph` as a int.
    let toInt (tzolkin: T) =
        int tzolkin

    /// Return the Tzolk’in day glyph's name as a string.
    ///
    /// Params:
    ///         `glyph` The Tzolk’in day glyph to convert.
    ///
    /// Returns:
    ///         The Tzolk’in day glyph as a string.
    let toString glyph =
        glyph.ToString ()

    /// Return the Tzolk’in day glyph as a Unicode symbol.
    /// This works as soon as the Tzolk’in day glyphs are included in the Unicode
    /// standard.
    ///
    /// Params:
    ///         `glyph` The Tzolk’in day glyph to convert.
    ///
    /// Returns:
    ///         The Tzolk’in day glyph as a Unicode symbol.
    let toUnicode glyph =
        match glyph with
        | (TzolkinGlyph i) -> glyphUnicode.[(modulo20 i) - 1]

    /// Return the description of the Tzolk’in day glyph as a `GlyphDescription`.
    ///
    /// Params:
    ///         `glyph` The Tzolk’in day glyph to convert.
    ///
    /// Returns:
    ///         The description of the Tzolk’in day glyph.
    let getDescription glyph =
        match glyph with
        | (TzolkinGlyph i) -> glyphDesc.[(modulo20 i) - 1]

    /// Return the Tzolk’in day glyph as a `TzolkinGlyph` of the given glyph name.
    /// See also `parseString`, if the string is not exactly one of `glyphNames`, like
    /// From user input.
    ///
    /// Params:
    ///         `name` The Tzolk’in day glyph's name, one of `glyphNames`.
    ///
    /// Returns:
    ///         The `TzolkinGlyph` of the given glyph name if the string `name` is a
    ///         valid Tzolk’in day glyph name.
    ///         `None` else.
    let fromString name =
        try
            Some (
                Array.findIndex (fun elem -> elem = name) glyphNames
                |> (+) 1
                |> TzolkinGlyph
            )
        with excp -> None

    /// Try to parse the given string `name` as a Tzolk’in day glyph and return the
    /// `TzolkinGlyph` of the given glyph name.
    /// Ignores characters of the following Unicode groups and categories:
    /// - `Lm`, Letter, Modifier
    /// - `Po`, Other Punctuation
    /// - `General Punctuation`
    /// - The Regex group `Not a word character` `\W`
    ///
    /// See also `fromString` if you have an exact name, one of the elements of
    /// `glyphNames`.
    ///
    /// Params:
    ///         `name` The string to try too parse as a Tzolk’in day glyph name.
    ///
    /// Returns:
    ///         The `TzolkinGlyph` of the given glyph name if the string `name` is a
    ///         valid Tzolk’in day glyph name.
    ///         `None` else.
    let parseString name =

        let removePunct text = Regex.Replace (text, @"[\W\p{Lm}\p{IsGeneralPunctuation}\p{Po}]", "")

        let sanitize text =
            text
            |> String.map System.Char.ToLowerInvariant
            |> removePunct

        try
            Some (
                Array.findIndex (fun elem -> sanitize elem = sanitize name) glyphNames
                |> (+) 1
                |> TzolkinGlyph
            )
        with excp -> None
