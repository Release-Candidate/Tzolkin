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

    /// <summary>
    /// The first page of the app. The date view, in the module `TzolkinView`.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function.</param>
    /// <returns>The page instance of the first page.</returns>
    let homePage model dispatch =
        View
            .ContentPage(title = "Calendario",
                         backgroundColor = Style.backgroundColor model.IsDarkMode,
                         appearing = (fun () -> dispatch <| SetCurrentPage Home),
                         content = View.StackLayout (
                             backgroundColor = Style.backgroundBrownDark,
                             children = tzolkinPage model dispatch
                         )


            )
            .HasNavigationBar(true)
            .HasBackButton (false)

    /// <summary>
    /// The second tab of the app. The filter view, in the module `DateList`.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function.</param>
    /// <returns>The page instance of the second page.</returns>
    let calendarFilter model dispatch =
        View
            .ContentPage(title = "Filtrar",
                         backgroundColor = backgroundBrownDark,
                         content = View.StackLayout (
                             padding = Thickness 5.0,
                             orientation = setHorizontalIfLandscape model.IsLandscape,
                             backgroundColor = backgroundBrownDark,
                             children = [ dateView model dispatch ]
                         ))
            .HasNavigationBar(true)
            .HasBackButton (true)

    /// <summary>
    /// The view of MVU.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function.</param>
    /// <returns>The app's main navigation page instance.</returns>
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
