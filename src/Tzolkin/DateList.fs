// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     DateList.fs
// Date:     4/10/2021 8:57:04 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open System
open Xamarin.Forms
open System.Globalization
open Fabulous.XamarinForms

open RC.Maya


/// View to pick a Tzolk’in date and display and filter a list of Gregorian dates with the same
/// Tzolk’in date.
[<AutoOpen>]
module DateList =

    /// <summary>
    /// A Frame holding the day filter picker.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A `Frame` instance holding the day filter picker.</returns>
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
                    selectedIndexChanged = (fun (i, _) -> dispatch (SetFilterDay i)),
                    fontSize = normalFontSize,
                    textColor = foregroundLight,
                    backgroundColor = backgroundBrownLight,
                    width = 60.0,
                    ref = dayPicker
                  )
          )

    /// <summary>
    /// A Frame holding the month filter picker.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A `Frame` instance with the month filter picker.</returns>
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
                  selectedIndexChanged = (fun (i, _) -> dispatch (SetFilterMonth i)),
                  fontSize = normalFontSize,
                  textColor = foregroundLight,
                  backgroundColor = backgroundBrownLight,
                  width = 60.,
                  ref = monthPicker
              )
          )

    /// <summary>
    /// A Frame containing the year filter picker (an `Entry`).
    /// </summary>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A `Frame`instance holding the year filter picker.</returns>
    let yearFrame dispatch =
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
                      fontSize = normalFontSize,
                      width = 50.0,
                      textColor = foregroundLight,
                      backgroundColor = backgroundBrownLight,
                      ref = yearPicker
                  )
         )

    /// <summary>
    /// Return the day, month and year frame in the order of the current locale.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A list containing the day, month and year filter frame, in the
    /// same order as the short date of the current locale.</returns>
    let getDateOrder model dispatch =
        let currentDateString = DateTimeFormatInfo.CurrentInfo.ShortDatePattern
        let dayIndex = currentDateString.IndexOf "d"
        let monthIndex = currentDateString.IndexOf "M"
        let yearIndex = currentDateString.IndexOf "y"
        let dateList =
            [ (dayIndex, dayFrame model dispatch)
              (monthIndex, monthFrame model dispatch)
              (yearIndex, yearFrame dispatch) ]

        dateList
        |> List.sortBy (fun (idx, _) -> idx)
        |> List.map (fun (_, frame) -> frame)



    /// <summary>
    /// Return a `FormattedString` of the given Tzolk’in date and gregorian date
    /// to use in a label.
    /// </summary>
    /// <param name="tzolkindate">The Tzolk’in date of the given `date`.</param>
    /// <param name="date">The gregorian date of `tzolkindate`.</param>
    /// <returns>A `FormattedString` of a Tzolk’in and gregorian date, to use
    /// with a label. </returns>
    let dateLabelFormat (tzolkindate:TzolkinDate.T) (date:DateTime) =
        View.FormattedString (
            [ View.Span(text = (sprintf "%s%s " (TzolkinNumber.toUnicode tzolkindate.Number)
                                                (TzolkinGlyph.toUnicode tzolkindate.Glyph)),
                          fontFamily = "Tzolkin",
                          textColor = accentDarkRed,
                          fontSize = dateListFontSize,
                          fontAttributes = dateListFontAttr)
              View.Span(text = sprintf "%s " (tzolkindate.ToString ()),
                            textColor = accentDarkRed,
                            fontSize = dateListFontSize,
                            fontAttributes = dateListFontAttr)
              View.Span(text = sprintf "%s" (date.ToShortDateString ()),
                           textColor = foregroundLight,
                           fontSize = dateListFontSize,
                           fontAttributes = dateListFontAttr)

        ])

    /// <summary>
    /// Return a formatted label containing the date in Tzolk’in and gregorian
    /// form.
    /// </summary>
    /// <param name="labelFormat">Function that returns a `FormattedString` of
    /// the given Tzolk’in and gregorian date.</param>
    /// <param name="tzolkindate">The Tzolk’in date of the given `date`.</param>
    /// <param name="date">The gregorian date of `tzolkindate`.</param>
    /// <returns></returns>
    let dateLabelF labelFormat tzolkindate (date:DateTime) =
        View.Label(formattedText = labelFormat tzolkindate date,
                    lineBreakMode = LineBreakMode.WordWrap,
                    horizontalOptions = LayoutOptions.Center
         )

    /// <summary>
    /// Return a formatted label of the given Tzolk’in and gregorian date.
    /// </summary>
    /// <returns>A formatted label of the given Tzolk’in and gregorian date.</returns>
    let dateLabel = dateLabelF dateLabelFormat

    /// <summary>
    /// Filters the given list of days by the day, month and year set in the day
    /// month and year filter. Return the filtered list as labels containing
    /// Tzolk’in and gregorian date of the list'S element.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dateList">The list of dates to filter.</param>
    /// <returns>The filtered date list as Tzolk’in and gregorian date labels.</returns>
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

    /// <summary>
    /// Fills the carousel view with filtered dates.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <returns>The filtered list as Tzolk’in and gregorian day labels.</returns>
    let fillListViewFilter (model: Model) =
        fullFilterList model model.DateList

    /// <summary>
    /// Select a Tzolk’in date using a Tzolk’in day number and glyph picker.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>The list of Frames holding the Tzolk’in day number and glyph
    /// pickers.</returns>
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
                    selectedIndexChanged = (fun (i, _) -> dispatch (SetListNumber i)),
                    width = 60.,
                    fontSize = normalFontSize,
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
                      fontSize = normalFontSize,
                      textColor = foregroundLight,
                      backgroundColor = backgroundBrownLight,
                      selectedIndexChanged = (fun (i, _) -> dispatch (SetListGlyph i)),
                      width = 90.
                )
          ) ]

    /// <summary>
    /// Return the Filter section, day, month and year pickers in the order of the
    /// current locale.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A list of `Frame`, containing the day, month and year filter
    /// picker in the order of the current locale.</returns>
    let tzolkinFilter (model: Model) dispatch =
        getDateOrder model dispatch

    /// <summary>
    /// The date filter page.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A `StackLayout` containing the date filter page.</returns>
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
                      )
                     ]
            )
