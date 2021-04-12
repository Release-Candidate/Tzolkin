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
    let homePage model dispatch =

        View
            .ContentPage(title = "Calendario",
                         backgroundColor = Style.backgroundColor model.IsDarkMode,
                         appearing = (fun () -> dispatch <| SetCurrentPage Home),
                         content = View.StackLayout (
                             backgroundColor = Style.backgroundBrownDark,//Style.backgroundColor model.IsDarkMode,
                             children = tzolkinPage model dispatch
                         )


            )
            .HasNavigationBar(true)
            .HasBackButton (false)

    /// The second tab of the app.
    let calendarFilter model dispatch =
        View
            .ContentPage(title = "calendarFilter",
                         backgroundColor = Style.backgroundColor model.IsDarkMode,
                         content = View.StackLayout (
                             padding = Thickness 5.0,
                             orientation = setHorizontalIfLandscape model.IsLandscape,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             children = [ dateView model dispatch ]
                         ))
            .HasNavigationBar(true)
            .HasBackButton (true)

    /// The view of MVU.
    let view model dispatch =

        match model.ShowSystemAppInfo with
        | true -> AppInfo.ShowSettingsUI ()
        | false -> ()

        View.NavigationPage (
            sizeChanged = (fun (width, height) -> dispatch (SetOrientation (width, height))),
            useSafeArea = true,
            barBackgroundColor = tabBackgroundColor,
            barTextColor = tabForegroundColor,
            pages =
                match model.CurrentPage with
                | Home -> [ homePage model dispatch ]

                | CalendarFilter ->
                    [ homePage model dispatch
                      calendarFilter model dispatch ]
        )
