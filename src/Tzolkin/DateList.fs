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
open System.Diagnostics

/// View to pick a Tzolk’in date and display and filter a list of Gregorian dates with the same
/// Tzolk’in date.
[<AutoOpen>]
module DateList =

    let dateLabelFormat (tzolkindate:TzolkinDate.T) (date:DateTime) =
        View.FormattedString (
            [ View.Span(text = (sprintf "%s%s " (TzolkinNumber.toUnicode tzolkindate.Number)
                                                (TzolkinGlyph.toUnicode tzolkindate.Glyph)),
                          fontFamily = "Tzolkin",
                          textColor = Style.accentDarkRed,
                          fontSize = Style.dateListFontSize,
                          fontAttributes = Style.dateListFontAttr)
              View.Span(text = sprintf "%s " (tzolkindate.ToString ()),
                            textColor = Style.accentDarkRed,
                            fontSize = Style.dateListFontSize,
                            fontAttributes = Style.dateListFontAttr)
              View.Span(text = sprintf "%s" (date.ToShortDateString ()),
                           textColor = Style.foregroundLight,
                           fontSize = Style.dateListFontSize,
                           fontAttributes = Style.dateListFontAttr)

        ])

    let dateLabel tzolkindate (date:DateTime) =
        View.Label(formattedText = dateLabelFormat tzolkindate date,
                    lineBreakMode = LineBreakMode.WordWrap,
                    horizontalOptions = LayoutOptions.Center
         )

    let fullFilterList model dateList =
           let filterDay dateList =
               match model.Filter.Day with
               | 0 -> dateList
               | day -> List.filter (fun (elem: DateTime) -> elem.Day = day) dateList

           let filterMonth dateList =
               match model.Filter.Month with
               | 0 -> dateList
               | month -> List.filter (fun (elem: DateTime) -> elem.Month = month) dateList

           let filterYear dateList =
               match model.Filter.Year with
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
           |> List.map (fun elem -> dateLabel (TzolkinDate.fromDate elem) elem)

    /// Fills the list view with filtered dates.
    let fillListViewFilter (model: Model) =
        fullFilterList model model.DateList



    /// Select a Tzolk’in date.
    let tzolkinSelector model dispatch =
        [ View.Picker (
            title = "número",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = modelNumToInt model,
            items = numberPickList,
            selectedIndexChanged = (fun (i, item) -> dispatch (SetListNumber i)),
            width = 60.,
            fontSize = Style.normalFontSize,
            textColor = Style.foregroundColor model.IsDarkMode,
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            horizontalTextAlignment = TextAlignment.End
          )

          View.Picker (
              title = "glifo",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = modelGlyphToInt model,
              items = glyphPickList,
              fontSize = Style.normalFontSize,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              selectedIndexChanged = (fun (i, item) -> dispatch (SetListGlyph i)),
              width = 90.
          ) ]

    /// The Filter section
    let tzolkinFilter (model: Model) dispatch =
        [ View.Picker (
            title = "día",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = model.Filter.Day,
            items = "todos" :: [ for i in 1 .. 31 -> i.ToString () ],
            selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterDay i)),
            fontSize = Style.normalFontSize,
            textColor = Style.foregroundColor model.IsDarkMode,
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            width = 60.0,
            ref = dayPicker
          )
          View.Picker (
              title = "mes",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = model.Filter.Month,
              items = "todos" :: [ for i in 1 .. 12 -> i.ToString () ],
              selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterMonth i)),
              fontSize = Style.normalFontSize,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              width = 60.,
              ref = monthPicker
          )
          View.Entry (
              text = "",
              placeholder = "año",
              textChanged = (fun text -> SetFilterYear text.NewTextValue |> dispatch),
              completed = (fun text -> SetFilterYear text |> dispatch),
              keyboard = Keyboard.Numeric,
              fontSize = Style.normalFontSize,
              width = 65.0,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              ref = yearPicker
          ) ]

    let dateViewLayout = GridItemsLayout (ItemsLayoutOrientation.Vertical)


    ///
    let dateView model dispatch =
        dateViewLayout.Span <- 1
        dateViewLayout.HorizontalItemSpacing <- 10.
        dateViewLayout.VerticalItemSpacing <- 10.
        View.StackLayout(
                backgroundColor = backgroundBrown,
                padding = Thickness 5.,
                orientation = StackOrientation.Vertical,
                children =
                    [
                      View.StackLayout(
                            orientation = StackOrientation.Horizontal,
                            children = (tzolkinSelector model dispatch)
                                       @ (tzolkinFilter model dispatch)
                      )

                      View.CollectionView(
                          ref = dateListView,
                          remainingItemsThreshold = 4,
                          remainingItemsThresholdReachedCommand = (
                                fun () -> dispatch NewDateViewItemsNeeded),
                          itemsLayout = dateViewLayout,
                          items = fillListViewFilter model
                      )


                     ]
            )
