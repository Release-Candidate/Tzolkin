// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TzolkinGlyph.fs
//
//==============================================================================

/// Namespace containing all Maya calendar libraries.
namespace RC.Maya

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
        { meaning: string
          elementOrAnimal: string
          direction: string
          color: string
          god: string
          url: string }

    /// Description of Tzolk’in day glyph Men.
    let descMen =
        { meaning = "águila"
          elementOrAnimal = "águila"
          direction = "oeste"
          color = "negro"
          god = "Ix Chel, diosa de la Luna"
          url = "" }

    /// Description of Tzolk’in day glyph Ikʼ.
    let descIk =
        { meaning = "viento"
          elementOrAnimal = "viento"
          direction = "norte"
          color = "blanco"
          god = "dios B o Chaac"
          url = "" }

    /// Description of Tzolk’in day glyph Chickchan.
    let descChikchan =
        { meaning = "serpiente celeste"
          elementOrAnimal = "serpiente"
          direction = "este"
          color = "rojo"
          god = "Chikchan, dios del número 9"
          url = "" }

    /// Description of Tzolk’in day glyph Imix.
    let descImix =
        { meaning = "caimán, cocodrilo"
          elementOrAnimal = "superficie terrestre"
          direction = "este"
          color = "rojo"
          god = "monstruo de la tierra"
          url = "" }

    /// Description of Tzolk’in day glyph Kawak.
    let descKawak =
        { meaning = "tormenta"
          elementOrAnimal = "lluvia, tormenta"
          direction = "oeste"
          color = "negro"
          god = "Itzamnaaj"
          url = "" }

    /// Description of Tzolk’in day glyph Ajaw.
    let descAjaw =
        { meaning = "señor"
          elementOrAnimal = "Sol"
          direction = "sur"
          color = "amarillo"
          god = "dios G o dios del Sol"
          url = "" }

    /// Description of Tzolk’in day glyph Etzʼnabʼ.
    let descEtznab =
        { meaning = "pedernal"
          elementOrAnimal = "pedernal"
          direction = "norte"
          color = "blanco"
          god = "dios Q o dios de la guerra y los sacrificios"
          url = "" }

    /// Description of Tzolk’in day glyph Kabʼan.
    let descKaban =
        { meaning = "Tierra"
          elementOrAnimal = "Tierra y los temblores"
          direction = "sur"
          color = "amarillo"
          god = "diosa I o diosa de la sensualidad y el amor"
          url = "" }

    /// Description of Tzolk’in day glyph Kʼibʼ.
    let descKib =
        { meaning = "“cera”"
          elementOrAnimal = "venado, insectos"
          direction = "sur"
          color = "amarillo"
          god = "dios N o Pawahtún"
          url = "" }

    /// Description of Tzolk’in day glyph Ix.
    let descIx =
        { meaning = "jaguar"
          elementOrAnimal = "jaguar"
          direction = "este"
          color = "rojo"
          god = "dios jaguar"
          url = "" }

    /// Description of Tzolk’in day glyph Bʼen.
    let descBen =
        { meaning = "maíz verde"
          elementOrAnimal = "maíz"
          direction = "este"
          color = "rojo"
          god = "dios E o dios del maíz"
          url = "" }

    /// Description of Tzolk’in day glyph Ebʼ.
    let descEb =
        { meaning = "rocío"
          elementOrAnimal = ""
          direction = "sur"
          color = "amarillo"
          god = "dios de las lluvias dañinas"
          url = "" }

    /// Description of Tzolk’in day glyph Chuwen.
    let descChuwen =
        { meaning = "mono"
          elementOrAnimal = "mono"
          direction = "oeste"
          color = "negro"
          god = "dios C o K’u"
          url = "" }

    /// Description of Tzolk’in day glyph Ok.
    let descOk =
        { meaning = "perro"
          elementOrAnimal = "perro"
          direction = "norte"
          color = "blanco"
          god = "el perro en su advocación de dios del inframundo"
          url = "" }

    /// Description of Tzolk’in day glyph Muluk.
    let descMuluk =
        { meaning = "jade, agua"
          elementOrAnimal = "agua"
          direction = "este"
          color = "rojo"
          god = "pez xoc, jaguar"
          url = "" }

    /// Description of Tzolk’in day glyph Lamat.
    let descLamat =
        { meaning = "Venus"
          elementOrAnimal = "conejo"
          direction = "sur"
          color = "amarillo"
          god = "Lahun Chan, Venus"
          url = "" }

    /// Description of Tzolk’in day glyph Manikʼ.
    let descManik =
        { meaning = "venado"
          elementOrAnimal = "venado"
          direction = "oeste"
          color = "negro"
          god = "dios R o Buluk Ch’Abtan, dios de la Tierra"
          url = "" }

    /// Description of Tzolk’in day glyph Kimi.
    let descKimi =
        { meaning = "muerte"
          elementOrAnimal = "muerte"
          direction = "norte"
          color = "blanco"
          god = "dios A o dios de la muerte"
          url = "" }

    /// Description of Tzolk’in day glyph Kʼan.
    let descKan =
        { meaning = "maíz maduro"
          elementOrAnimal = "maíz y abundancia"
          direction = "sur"
          color = "amarillo"
          god = "dios E o dios del maíz"
          url = "" }

    /// Description of Tzolk’in day glyph Akʼbʼal.
    let descAkbal =
        { meaning = "oscuridad"
          elementOrAnimal = "oscuridad, noche, jaguar"
          direction = "oeste"
          color = "negro"
          god = "Chaac Bolay, jaguar de nenúfar"
          url = "" }

    /// The descriptions of the 20 Tzolk’in day glyphs.
    let glyphDesc =
        [| descImix
           descIk
           descAkbal
           descKan
           descChikchan
           descKimi
           descManik
           descLamat
           descMuluk
           descOk
           descChuwen
           descEb
           descBen
           descIx
           descMen
           descKib
           descKaban
           descEtznab
           descKawak
           descAjaw |]

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
    let modulo20 n =
        match n with
        | i when i >= 0 -> if n % 20 = 0 then 20 else n % 20
        | _ -> if n % 20 = 0 then 20 else 20 + (n % 20)

    /// The Tzolk’in day glyph type.
    type T =
        | TzolkinGlyph of int

        /// Convert a `TzolkinGlyph` to an `int`.
        static member op_Explicit tz =
            match tz with
            | (TzolkinGlyph n) -> n

        /// Add two `TzolkinGlyph`.
        static member (+)(glyph1: T, glyph2: T) =
            int glyph1 + int glyph2
            |> modulo20
            |> TzolkinGlyph

        /// Add an int to a `TzolkinGlyph`.
        static member (+)(glyph1: T, i: int) = int glyph1 + i |> modulo20 |> TzolkinGlyph

        /// Add a `TzolkinGlyph` to an int.
        static member (+)(i: int, glyph1: T) = int glyph1 + i |> modulo20 |> TzolkinGlyph

        /// Convert a `TzolkinGlyph` to a string.
        /// Now you can use `string` with a `TzolkinGlyph`, like
        /// `string (TzolkinGlyph.create 4)`
        override this.ToString() =
            match this with
            | (TzolkinGlyph i) -> glyphNames.[(modulo20 i) - 1]

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

    /// Reference Tzolk’in date. The 1st of January, 1970 is a Tzolk’in date of
    /// 13 Chikchan.
    let referenceDate = ("01.01.1970", TzolkinGlyph 5)

    /// Return the Tzolk’in day glyph's name as a string.
    ///
    /// Params:
    ///         `glyph` The Tzolk’in day glyph to convert.
    ///
    /// Returns:
    ///         The Tzolk’in day glyph as a string.
    let toString glyph =
        match glyph with
        | (TzolkinGlyph i) -> glyphNames.[(modulo20 i) - 1]

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
