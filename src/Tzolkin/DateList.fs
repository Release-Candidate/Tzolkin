﻿// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     DateList.fs
// Date:     4/10/2021 8:57:04 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open Fabulous
open System
open Fabulous.XamarinForms

open RC.Maya
open Xamarin.Forms

/// View to pick a Tzolk’in date and display and filter a list of Gregorian dates with the same
/// Tzolk’in date.
[<AutoOpen>]
module DateList =

    let cmdScrollToCenter = ScrollListCenter |> Cmd.ofMsg

    /// Fills the list view with filtered dates.
    let fillListViewFilter (model: Model) =
        let lastList =
            TzolkinDate.getLastList 500 model.ListTzolkinDate DateTime.Today
            |> List.rev

        let nextList = TzolkinDate.getNextList 500 model.ListTzolkinDate DateTime.Today

        let dateList = lastList @ nextList

        let filterDay dateList =
            match model.Filter.day with
            | 0 -> dateList
            | day -> List.filter (fun (elem: DateTime) -> elem.Day = day) dateList

        let filterMonth dateList =
            match model.Filter.month with
            | 0 -> dateList
            | month -> List.filter (fun (elem: DateTime) -> elem.Month = month) dateList

        let filterYear dateList =
            match model.Filter.year with
            | "" -> dateList
            | year ->
                List.filter
                    (fun (elem: DateTime) ->
                        let yearStr = elem.Year.ToString ()
                        yearStr.Contains (year))
                    dateList

        dateList
        |> filterDay
        |> filterMonth
        |> filterYear
        |> List.map (fun elem -> View.TextCell (elem.ToShortDateString ()))



    /// Select a Tzolk’in date.
    let tzolkinSelector model dispatch =
        [ View.Picker (
            title = "Number:",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = int (model.ListTzolkinDate.number) - 1,
            items = numberPickList,
            selectedIndexChanged = (fun (i, item) -> dispatch (SetListNumber <| i + 1)),
            width = 35.0,
            fontSize = Style.fontSize,
            textColor = Style.foregroundColor model.IsDarkMode,
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            horizontalTextAlignment = TextAlignment.End
          )

          View.Picker (
              title = "Glyph:",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = int (model.ListTzolkinDate.glyph) - 1,
              items = glyphPickList,
              fontSize = Style.fontSize,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              selectedIndexChanged = (fun (i, item) -> dispatch (SetListGlyph <| i + 1))
          ) ]

    /// The Filter section
    let tzolkinFilter (model: Model) dispatch =
        [ View.Picker (
            title = "Day:",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = model.Filter.day,
            items = "" :: [ for i in 1 .. 31 -> i.ToString () ],
            selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterDay <| i)),
            fontSize = Style.fontSize,
            textColor = Style.foregroundColor model.IsDarkMode,
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            width = 35.0,
            ref = dayPicker
          )
          View.Picker (
              title = "Month:",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = model.Filter.month,
              items = "" :: [ for i in 1 .. 12 -> i.ToString () ],
              selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterMonth <| i)),
              fontSize = Style.fontSize,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              width = 35.0,
              ref = monthPicker
          )
          View.Entry (
              text = model.Filter.year,
              completed = (fun text -> SetFilterYear text |> dispatch),
              keyboard = Keyboard.Numeric,
              fontSize = Style.fontSize,
              width = 100.0,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              ref = yearPicker
          ) ]

    /// 
    let dateView model dispatch =
        View.Grid (
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            padding = Thickness 5.,
            rowdefs =
                [ Dimension.Auto
                  Dimension.Auto
                  Dimension.Auto
                  Dimension.Auto
                  Dimension.Star
                  Dimension.Absolute 15. ],
            coldefs =
                [ Dimension.Stars 0.4
                  Dimension.Stars 0.6 ],
            children =
                [ (tzolkinDateView model.ListTzolkinDate model.IsDarkMode).Row(0).Column (1)

                  View
                      .StackLayout(children = tzolkinSelector model dispatch,
                                   orientation = StackOrientation.Horizontal)
                      .Row(1)
                      .Column (1)

                  View
                      .StackLayout(orientation = StackOrientation.Horizontal,
                                   children = tzolkinFilter model dispatch)
                      .Row(2)
                      .Column (1)


                  View
                      .Button(text = "Reset", command = (fun () -> dispatch DoResetFilter))
                      .Row(3)
                      .Column (1)
                  //View
                  //    .BoxView(color = Color.Yellow,
                  //             verticalOptions = LayoutOptions.Start)
                  //    .Row(4)
                  //    .Column (1)
                  View
                      .ListView(ref = dateListView,
                                items = fillListViewFilter model,
                                backgroundColor = Style.backgroundColor model.IsDarkMode,
                                horizontalOptions = LayoutOptions.Start)
                      .Row(0)
                      .Column(0)
                      .RowSpan (5)
                  View
                      .Label(text = versionInfo,
                             fontSize = FontSize.fromNamedSize NamedSize.Micro,
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             verticalTextAlignment = TextAlignment.End,
                             horizontalTextAlignment = TextAlignment.End,
                             horizontalOptions = LayoutOptions.Fill,
                             verticalOptions = LayoutOptions.Fill)
                      .Row(5)
                      .Column(0)
                      .ColumnSpan (2)

                  ]
        )