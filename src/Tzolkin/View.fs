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


    /// The first tab of the app.
    let tab1 model dispatch =
        View.ContentPage (
            title = "1",
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
                            backgroundColor = brownBackground,
                            children = dateSelector model dispatch
                          )

                          separator model.IsLandscape model.IsDarkMode

                          glyphDescription model dispatch ]

                )
        )

    /// The second tab of the app.
    let tab2 model dispatch =
        View.ContentPage (
            title = "2",
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            content =
                View.StackLayout (
                    padding = Thickness 5.0,
                    orientation = setHorizontalIfLandscape model.IsLandscape,
                    backgroundColor = Style.backgroundColor model.IsDarkMode,
                    children = [ dateView model dispatch ]
                )
        )

    /// The view of MVU.
    let view model dispatch =

        match model.ShowSystemAppInfo with
        | true -> AppInfo.ShowSettingsUI ()
        | false -> ()

        View.TabbedPage (
            sizeChanged = (fun (width, height) -> dispatch (SetOrientation (width, height))),
            useSafeArea = true,
            barBackgroundColor = tabBackgroundColor,
            barTextColor = tabForegroundColor,
            currentPageChanged =
                (fun index ->
                    match index with
                    | None -> ()
                    | Some idx ->
                        printfn "Tab changed : %i" idx
                        dispatch (SetTabIndex idx)),
            currentPage = model.CurrentTabIndex,
            children =
                [ tab1 model dispatch
                  tab2 model dispatch ]
        )
