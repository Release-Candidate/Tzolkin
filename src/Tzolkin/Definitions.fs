﻿// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     Definitions.fs
// Date:     4/10/2021 7:50:49 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp


open Xamarin.Essentials
open Fabulous.XamarinForms
open Xamarin.Forms
open Fabulous

open RC.Maya
open System.Globalization
open System.Diagnostics
open System

/// Holds the most basic definitions, the MVU model type `Model`, the MVU message type `Msg`,
/// the MVU `init` and `update` functions.
[<AutoOpen>]
module Definitions =

    /// App-wide constants =========================================================================

    let appNameInfo = sprintf "%s (Package %s)" AppInfo.Name AppInfo.PackageName

    let version = sprintf "%s (Build %s)" AppInfo.VersionString AppInfo.BuildString

    let versionInfo = sprintf "%s %s" appNameInfo version

    let numberPickList = "" :: List.map (fun x -> x.ToString ()) [ 1 .. 13 ]

    let glyphPickList = "" :: Array.toList TzolkinGlyph.glyphNames

    let localeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

    let localeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern


    // The model ===================================================================================

    /// The pages of the App.
    type Pages =
        | Home
        | CalendarFilter

    /// Record to hold the data needed to filter dates.
    type DateFilter = { day: int; month: int; year: string }

    /// The MVU model.
    type Model =
        { CurrentPage: Pages
          Date: System.DateTime
          ListTzolkinNumber: TzolkinNumber.T option
          ListTzolkinGlyph: TzolkinGlyph.T option
          Filter: DateFilter
          IsDarkMode: bool
          IsLandscape: bool
          ShowSystemAppInfo: bool
          CurrentTabIndex: int }

    let modelTzolkinDate model =
        match model.ListTzolkinNumber, model.ListTzolkinGlyph with
        | None, Some tz ->
            { TzolkinDate.number = TzolkinNumber.T.TzolkinNumber 1
              TzolkinDate.glyph = tz }
        | Some tz, None ->
            { TzolkinDate.number = tz
              TzolkinDate.glyph = TzolkinGlyph.T.TzolkinGlyph 1 }
        | Some tzn, Some tzg ->
            { TzolkinDate.number = tzn
              TzolkinDate.glyph = tzg }
        | _, _ ->
            { TzolkinDate.number = TzolkinNumber.T.TzolkinNumber 1
              TzolkinDate.glyph = TzolkinGlyph.T.TzolkinGlyph 1 }

    let modelNumToInt model =
        match model.ListTzolkinNumber with
        | None -> 0
        | Some tz -> int tz

    let modelGlyphToInt model =
        match model.ListTzolkinGlyph with
        | None -> 0
        | Some tz -> int tz

    // The messages ================================================================================

    /// MVU messages.
    type Msg =
        | SetCurrentPage of Pages
        | SetDate of System.DateTime
        | SetListNumber of int
        | SetListGlyph of int
        | SetFilterDay of int
        | SetFilterMonth of int
        | SetFilterYear of string
        | DoResetFilter
        | SetAppTheme of OSAppTheme
        | SetOrientation of float * float
        | ShowSystemAppInfo of bool
        | CarouselChanged of PositionChangedEventArgs
        | OpenURL of string

    // Commands ====================================================================================
    let cmdOpenUrl (url) =
              Launcher.OpenAsync (new Uri (url))
              |> Async.AwaitTask
              |> Async.StartImmediate
              Cmd.none

    // Widget instances ============================================================================

    /// Instances of widgets needed to interact with.
    let dateListView = ViewRef<CustomListView> ()
    let dayPicker = ViewRef<Xamarin.Forms.Picker> ()
    let monthPicker = ViewRef<Xamarin.Forms.Picker> ()
    let yearPicker = ViewRef<Xamarin.Forms.Entry> ()

    // Init ========================================================================================

    /// Initial state of the MVU model.
    let initModel =
        { CurrentPage = Home
          Date = System.DateTime.Today
          ListTzolkinNumber = Some (TzolkinNumber.T.TzolkinNumber 8)
          ListTzolkinGlyph = Some (TzolkinGlyph.T.TzolkinGlyph 5)
          Filter = { day = 0; month = 0; year = "" }
          IsDarkMode =
              if Application.Current.RequestedTheme = OSAppTheme.Dark then
                  true
              else
                  false
          IsLandscape = false
          ShowSystemAppInfo = false
          CurrentTabIndex = 0 }

    /// Initialize the model and commands.
    let init () = initModel, Cmd.none

    // Functions needed by `update` ================================================================

    /// Reset the text in the year entry.
    let resetYear model =
        match yearPicker.TryValue with
        | None -> ()
        | Some textEntry -> textEntry.Text <- ""

    // Update ======================================================================================

    /// The update function of MVU.
    let update msg model =
        match msg with

        | SetCurrentPage page ->
            Trace.TraceInformation "SetCurrentPage"
            { model with CurrentPage = page }, Cmd.none

        | SetDate date -> { model with Date = date }, Cmd.none

        | SetListNumber newNum ->
            match newNum with
            | 0 -> { model with ListTzolkinNumber = None }, Cmd.none
            | _ ->
                { model with
                      ListTzolkinNumber = Some <| TzolkinNumber.T.TzolkinNumber newNum },
                Cmd.none

        | SetListGlyph newGlyph ->
            match newGlyph with
            | 0 -> { model with ListTzolkinGlyph = None }, Cmd.none
            | _ ->
                { model with
                      ListTzolkinGlyph = Some <| TzolkinGlyph.T.TzolkinGlyph newGlyph },
                Cmd.none

        | SetFilterDay newday ->
            { model with
                  Filter = { model.Filter with day = newday } },
            Cmd.none

        | SetFilterMonth newMonth ->
            { model with
                  Filter = { model.Filter with month = newMonth } },
            Cmd.none

        | SetFilterYear newYear ->
            { model with
                  Filter = { model.Filter with year = newYear } },
            Cmd.none

        | DoResetFilter ->
            resetYear model

            { model with
                  Filter = { day = 0; month = 0; year = "" } },
            Cmd.none

        | SetAppTheme (theme: OSAppTheme) ->
            match theme with
            | OSAppTheme.Dark -> { model with IsDarkMode = true }
            | _ -> { model with IsDarkMode = false }
            , Cmd.none

        | SetOrientation (x, y) ->
            match x, y with
            | width, height when width > height -> { model with IsLandscape = true }, Cmd.none
            | _, _ -> { model with IsLandscape = false }, Cmd.none

        | ShowSystemAppInfo doShow ->
            match doShow with
            | true -> { model with ShowSystemAppInfo = true }, Cmd.none
            | false -> { model with ShowSystemAppInfo = false }, Cmd.none

        | CarouselChanged args ->
            let direction = args.CurrentPosition - args.PreviousPosition

            match args.PreviousPosition, args.CurrentPosition with
            | 0, 2 ->
                { model with
                      Date = model.Date + System.TimeSpan.FromDays -1. },
                Cmd.none
            | 2, 0 ->
                { model with
                      Date = model.Date + System.TimeSpan.FromDays 1. },
                Cmd.none
            | _, _ ->
                { model with
                      Date =
                          model.Date
                          + System.TimeSpan.FromDays (float direction) },
                Cmd.none

        | OpenURL url -> model, cmdOpenUrl url
