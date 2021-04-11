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


/// Holds the view `view` of MVU. The app's pages.
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
                    padding = Thickness 5.0,
                    orientation = setHorizontalIfLandscape model.IsLandscape,
                    backgroundColor = Style.backgroundColor model.IsDarkMode,
                    children =
                        [ View.StackLayout (
                            padding = Thickness (0.0, 10.0, 10.0, 10.0),
                            orientation = setVerticalIfLandscape model.IsLandscape,
                            backgroundColor = setBrown, // Style.backgroundColor model.IsDarkMode,
                            children = dateSelector model dispatch
                          )

                          separator model.IsLandscape model.IsDarkMode

                          dateView model dispatch ]
                )
        )
