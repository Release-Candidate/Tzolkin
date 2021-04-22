// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     Update.fs
// Date:     4/22/2021 12:00:09 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open System
open Fabulous
open Xamarin.Forms

open RC.Maya


/// The MVU update function.
[<AutoOpen>]
module Update=

    /// <summary>
    /// Message `SetCurrentPage`. Sets the current page to the first page `Home`
    /// or the filter view page `CalendarFilter`.
    /// </summary>
    /// <param name="model">The MVU model to update.</param>
    /// <param name="page">The new page to display.</param>
    /// <returns>The updated model and `cmdDateListViewHeight`</returns>
    let setCurrPage model page =
        let tzolkin = TzolkinDate.fromDate model.Date
        { model with CurrentPage = page
                     LastFilterListIdx = filterViewStartingIdx
                     ListTzolkinGlyph = Some tzolkin.Glyph
                     ListTzolkinNumber = Some tzolkin.Number
                     Filter = { model.Filter with Day = 0; Month = 0; Year = "" }
                     DateList = dateListTzolkin filterViewStartingIdx tzolkin model.Date },
        cmdDateListViewHeight

    /// <summary>
    /// Message `SetListNumber`. Sets the Tzolk’in day number to filter the
    /// dates on the filter view page.
    /// </summary>
    /// <param name="model">The MVU model to update.</param>
    /// <param name="newNum"></param>
    /// <returns>The updated model and `Cmd.none`.</returns>
    let setListNum model newNum =
        match newNum, model.ListTzolkinGlyph with
        | 0, None ->
            { model with
                ListTzolkinNumber = None
                DateList = [ for i in [-filterViewStartingIdx .. filterViewStartingIdx] ->
                                model.Date + TimeSpan.FromDays (float i)] },
             Cmd.none

        | _, None ->
            { model with
                    ListTzolkinNumber = Some <| TzolkinNumber.T.TzolkinNumber newNum
                    DateList = numListTzolkin
                                    filterViewStartingIdx
                                    (TzolkinNumber.T.TzolkinNumber newNum)
                                    model.Date },
            Cmd.none

        | 0, Some glyph ->
            { model with
                    ListTzolkinNumber = None
                    DateList = glyphListTzolkin filterViewStartingIdx glyph model.Date },
            Cmd.none

        | _, Some glyph ->
            { model with
                ListTzolkinNumber = Some <| TzolkinNumber.T.TzolkinNumber newNum
                DateList = dateListTzolkin
                                filterViewStartingIdx
                                (TzolkinDate.create (TzolkinNumber.T.TzolkinNumber newNum) glyph)
                                model.Date },
            Cmd.none

    /// <summary>
    /// Message `SetListGlyph`.
    /// </summary>
    /// <param name="model">THe MVU model.</param>
    /// <param name="newGlyph">The Tzolk’in day glyph to set the date list filter to.</param>
    /// <returns>The updated model and `Cmd.none`.</returns>
    let setListGly model newGlyph =
        match newGlyph, model.ListTzolkinNumber with
        | 0, None ->
            { model with
                ListTzolkinGlyph = None
                DateList = [ for i in [-filterViewStartingIdx .. filterViewStartingIdx] ->
                                model.Date + TimeSpan.FromDays (float i)] },
            Cmd.none

        | _, None ->
             { model with
                 ListTzolkinGlyph = Some <| TzolkinGlyph.T.TzolkinGlyph newGlyph
                 DateList = glyphListTzolkin
                                 filterViewStartingIdx
                                 (TzolkinGlyph.T.TzolkinGlyph newGlyph)
                                 model.Date },
             Cmd.none

        | 0, Some number ->
             { model with
                 ListTzolkinGlyph = None
                 DateList = numListTzolkin filterViewStartingIdx number model.Date },
             Cmd.none

        | _, Some number ->
             { model with
                 ListTzolkinGlyph = Some <| TzolkinGlyph.T.TzolkinGlyph newGlyph
                 DateList = dateListTzolkin
                                 filterViewStartingIdx
                                 (TzolkinDate.create number (TzolkinGlyph.T.TzolkinGlyph newGlyph))
                                 model.Date },
             Cmd.none

    /// <summary>
    /// Message `CarouselChanged`. Called, if the current item in the date view
    /// carousel has changed.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="args">The `PositionChangedEventArgs`.</param>
    /// <returns>The unchanged model and `Cmd.none`.</returns>
    let carChanged model (args: PositionChangedEventArgs) =
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
                    Date = model.Date + System.TimeSpan.FromDays (float direction) },
            Cmd.none

    /// <summary>
    /// Message `FilterCarouselHeight`. This message is send when the filter view
    /// carousel changes it's height, either because it came into view or the
    /// device orientation has changed. Sets the `PeekAreaInsets` of the
    /// carousel  view and scrolls to the center of the carousel view.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <returns>The unchanged model and `Cmd.none`.</returns>
    let heightCarouselChanged model =
        match dateListView.TryValue with
        | None -> model, Cmd.none
        | Some carousel ->
                carousel.PeekAreaInsets <- Thickness (carousel.Height /
                                            (setDateCarouselFactors model.IsLandscape))
                carousel.ScrollTo (
                                index = model.LastFilterListIdx,
                                position = ScrollToPosition.Center,
                                animate = false
                                )
                model, Cmd.none

    /// <summary>
    /// The update function of MVU.
    /// </summary>
    /// <param name="msg">The message to process.</param>
    /// <param name="model">The MVU model.</param>
    /// <returns>The updated model and a command to execute.</returns>
    let update msg model =
        match msg with

        | SetCurrentPage page -> setCurrPage model page

        | SetDate date -> { model with Date = date }, Cmd.none

        | SetListNumber newNum -> setListNum model newNum

        | SetListGlyph newGlyph -> setListGly model newGlyph

        | SetFilterDay newday ->
            { model with
                  Filter = { model.Filter with Day = newday } },
            Cmd.none

        | SetFilterMonth newMonth ->
            { model with
                  Filter = { model.Filter with Month = newMonth } },
            Cmd.none

        | SetFilterYear newYear ->
            { model with
                  Filter = { model.Filter with Year = newYear } },
            Cmd.none

        | DoResetFilter ->
            resetYear ()

            { model with
                  Filter = { Day = 0; Month = 0; Year = "" } },
            Cmd.none

        | SetAppTheme (theme: OSAppTheme) ->
            match theme with
            | OSAppTheme.Dark -> { model with IsDarkMode = true }, Cmd.none
            | _ -> { model with IsDarkMode = false }, Cmd.none

        | SetOrientation (x, y) ->
            match x, y with
            | width, height when width > height -> { model with IsLandscape = true }, Cmd.none
            | _, _ -> { model with IsLandscape = false }, Cmd.none

        | ShowSystemAppInfo doShow ->
            match doShow with
            | true -> { model with ShowSystemAppInfo = true }, Cmd.none
            | false -> { model with ShowSystemAppInfo = false }, Cmd.none

        | CarouselChanged args -> carChanged model args

        | OpenURL url -> model, cmdOpenUrl url

        | FilterCarouselHeight -> heightCarouselChanged model
