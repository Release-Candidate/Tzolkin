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

    /// Separator line.
    let separator isL =
        match isL with
        | false ->
            View.BoxView (
                color = Color.Black,
                backgroundColor = Color.Black,
                height = 0.5,
                horizontalOptions = LayoutOptions.FillAndExpand
            )

        | true ->
            View.BoxView (
                color = Color.Black,
                backgroundColor = Color.Black,
                width = 0.5,
                verticalOptions = LayoutOptions.FillAndExpand
            )
