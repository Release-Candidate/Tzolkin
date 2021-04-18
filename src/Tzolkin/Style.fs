// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     Style.fs
// Date:     4/10/2021 8:38:40 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open Fabulous.XamarinForms
open Xamarin.Forms
open RC.Maya

/// Module holding colors, font sizes and other style related constants and functions.
[<AutoOpen>]
module Style =

    // Global ==================================================================

    let normalFontSize = FontSize.fromNamedSize NamedSize.Medium

    let backgroundBrownDark = Color.FromHex "#BFAB91"

    let backgroundBrown = Color.FromHex "#F2D8B8"

    let backgroundBrownLight = Color.FromHex "#F6DCBC"

    let backgroundNone = Color.Default

    let backgroundLight = Color.Default

    let backgroundDark = Color.FromHex "#1F1B24"

    let foregroundLight = Color.Black

    let foregroundDark = Color.WhiteSmoke

    let accentDarkRed = Color.FromHex "#8B2A02"

    let linkColorBlue = Color.Blue

    let linkSymbol = "\U0001F517"

    let foregroundColor isDark =
        match isDark with
        | true -> foregroundDark
        | false -> foregroundLight

    let backgroundColor isDark =
        match isDark with
        | true -> backgroundDark
        | false -> backgroundLight

    let setHorizontalIfLandscape isL =
        if isL then
            StackOrientation.Horizontal
        else
            StackOrientation.Vertical

    let setVerticalIfLandscape isL =
        if isL then
            StackOrientation.Vertical
        else
            StackOrientation.Horizontal



    let setDateHeightCarouselFactors isL =
           if isL then 2.35 * 0.275 else 2.15 * 0.347

    // Pages ===================================================================

    let tabBackgroundColor = backgroundBrownDark

    let tabForegroundColor = Color.Black

    // Glyph Descriptions ======================================================

    let glyphDescFontSizeTitle = FontSize.fromNamedSize NamedSize.Medium

    let glyphDescTextColorTitle = foregroundLight

    let glyphDescFontAttrTitle = FontAttributes.Bold

    let glyphDescFontSizeValue = FontSize.fromNamedSize NamedSize.Medium

    let glyphDescTextColorValue = accentDarkRed

    let glyphDescFontAttrValue = FontAttributes.Bold

    let glyphDescColorLink = linkColorBlue

    // Date list ===============================================================

    let dateListFontSize = FontSize.fromNamedSize NamedSize.Large

    let dateListFontAttr = FontAttributes.None

    /// Separator line.
    let separator isL isDark =
        match isL with
        | false ->
            View.BoxView (
                color = foregroundColor isDark,
                backgroundColor = foregroundColor isDark,
                height = 0.5,
                horizontalOptions = LayoutOptions.FillAndExpand
            )

        | true ->
            View.BoxView (
                color = foregroundColor isDark,
                backgroundColor = foregroundColor isDark,
                width = 0.5,
                verticalOptions = LayoutOptions.FillAndExpand
            )
