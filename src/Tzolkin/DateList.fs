// SPDX-License-Identifier: MIT
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
open Xamarin.Forms
open System.Diagnostics
open System.Globalization
open Fabulous.XamarinForms

open RC.Maya


/// View to pick a Tzolk’in date and display and filter a list of Gregorian dates with the same
/// Tzolk’in date.
[<AutoOpen>]
module DateList =

    let dayFrame model dispatch =
        View.Frame (
            padding = Thickness 0.,
            backgroundColor = backgroundBrownLight,
            borderColor = backgroundBrownDark,
            content =
                View.Picker (
                    title = "día",
                    horizontalOptions = LayoutOptions.Start,
                    selectedIndex = model.Filter.Day,
                    items = "todos" :: [ for i in 1 .. 31 -> i.ToString () ],
                    selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterDay i)),
                    fontSize = Style.normalFontSize,
                    textColor = foregroundLight,
                    backgroundColor = backgroundBrownLight,
                    width = 60.0,
                    ref = dayPicker
                  )
          )

    let monthFrame model dispatch =
          View.Frame (
              padding = Thickness 0.,
              backgroundColor = backgroundBrownLight,
              borderColor = backgroundBrownDark,
              content =
                View.Picker (
                  title = "mes",
                  horizontalOptions = LayoutOptions.Start,
                  selectedIndex = model.Filter.Month,
                  items = "todos" :: [ for i in 1 .. 12 -> i.ToString () ],
                  selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterMonth i)),
                  fontSize = Style.normalFontSize,
                  textColor = foregroundLight,
                  backgroundColor = backgroundBrownLight,
                  width = 60.,
                  ref = monthPicker
              )
          )

    let yearFrame model dispatch =
          View.Frame (
            padding = Thickness 0.,
            backgroundColor = backgroundBrownLight,
            borderColor = backgroundBrownDark,
            content =
                  View.Entry (
                      text = "",
                      placeholder = "año",
                      textChanged = (fun text -> SetFilterYear text.NewTextValue |> dispatch),
                      completed = (fun text -> SetFilterYear text |> dispatch),
                      keyboard = Keyboard.Numeric,
                      fontSize = Style.normalFontSize,
                      width = 50.0,
                      textColor = foregroundLight,
                      backgroundColor = backgroundBrownLight,
                      ref = yearPicker
                  )
         )

    let getDateOrder model dispatch =
        let currentDateString = DateTimeFormatInfo.CurrentInfo.ShortDatePattern
        let dayIndex = currentDateString.IndexOf "d"
        let monthIndex = currentDateString.IndexOf "M"
        let yearIndex = currentDateString.IndexOf "y"
        let dateList =
            [ (dayIndex, dayFrame model dispatch)
              (monthIndex, monthFrame model dispatch)
              (yearIndex, yearFrame model dispatch) ]

        dateList
        |> List.sortBy (fun (idx, _) -> idx)
        |> List.map (fun (_, frame) -> frame)



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

    let dateLabelF labelFormat tzolkindate (date:DateTime) =
        View.Label(formattedText = labelFormat tzolkindate date,
                    lineBreakMode = LineBreakMode.WordWrap,
                    horizontalOptions = LayoutOptions.Center
         )

    let dateLabel = dateLabelF dateLabelFormat

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
        [ View.Frame (
            padding = Thickness 0.,
            backgroundColor = backgroundBrownLight,
            borderColor = backgroundBrownDark,
            content =
                View.Picker (
                    title = "número",
                    horizontalOptions = LayoutOptions.Start,
                    selectedIndex = modelNumToInt model,
                    items = numberPickList,
                    selectedIndexChanged = (fun (i, item) -> dispatch (SetListNumber i)),
                    width = 60.,
                    fontSize = Style.normalFontSize,
                    textColor = foregroundLight,
                    backgroundColor = backgroundBrownLight,
                    horizontalTextAlignment = TextAlignment.End
                )
          )

          View.Frame (
              padding = Thickness 0.,
              backgroundColor = backgroundBrownLight,
              borderColor = backgroundBrownDark,
              content =
                  View.Picker (
                      title = "glifo",
                      horizontalOptions = LayoutOptions.Start,
                      selectedIndex = modelGlyphToInt model,
                      items = glyphPickList,
                      fontSize = Style.normalFontSize,
                      textColor = foregroundLight,
                      backgroundColor = backgroundBrownLight,
                      selectedIndexChanged = (fun (i, item) -> dispatch (SetListGlyph i)),
                      width = 90.
                )
          ) ]

    /// The Filter section
    let tzolkinFilter (model: Model) dispatch =
        getDateOrder model dispatch

    ///
    let dateView model dispatch =
        View.StackLayout(
                backgroundColor = backgroundBrown,
                padding = Thickness 5.,
                orientation = StackOrientation.Vertical,
                children =
                    [
                      View.StackLayout(
                            orientation = StackOrientation.Horizontal,
                            children = (tzolkinSelector model dispatch) @
                                       (tzolkinFilter model dispatch)
                      )

                      View.CarouselView(
                            ref = dateListView,
                            loop = false,
                            itemsLayout = View.CarouselVerticalItemsLayout (
                                                itemSpacing = 0.0,
                                                snapPointsType = SnapPointsType.Mandatory
                                          ),
                            items = fillListViewFilter model
                            //positionChanged = (fun args -> dispatch <| FilterCarouselChanged args)
                            // position = 20
                      )
                     ]
            )
