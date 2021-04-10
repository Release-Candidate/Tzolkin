// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     View.fs
// Date:     4/10/2021 9:03:21 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open Fabulous.XamarinForms
open Xamarin.Forms
open Xamarin.Essentials


/// Holds the view `View` of MVU. The app's pages.
[<AutoOpen>]
module View =

    /// The view of MVU.
    let view model dispatch =

        match model.ShowSystemAppInfo with
        | true -> AppInfo.ShowSettingsUI ()
        | false -> ()

        View.ContentPage (
            sizeChanged = (fun (width, height) -> dispatch (SetOrientation (width, height))),
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            content =
                View.StackLayout (
                    padding = Thickness 10.0,
                    orientation =
                        (if model.IsLandscape then
                             StackOrientation.Horizontal
                         else
                             StackOrientation.Vertical),
                    backgroundColor = Style.backgroundColor model.IsDarkMode,
                    children =
                        [ View.StackLayout (
                            orientation = StackOrientation.Horizontal,
                            backgroundColor = Style.backgroundColor model.IsDarkMode,
                            children = dateSelector model dispatch
                          )

                          separator model.IsLandscape

                          dateView model dispatch ]
                )
        )
