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

/// Module holding colors, font sizes and other style related constants and functions.
[<AutoOpen>]
module Style =

    let fontSize = FontSize.fromNamedSize NamedSize.Medium

    let setBrown = Color.FromHex "#F2D8B8"

    let backgroundLight = Color.Default

    let backgroundDark = Color.FromHex "#1F1B24"

    let foregroundLight = Color.Black

    let foregroundDark = Color.WhiteSmoke

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
