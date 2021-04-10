// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TzolkinApp
// File:     TzolkinView.fs
// Date:     4/10/2021 8:49:43 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open Fabulous.XamarinForms
open Xamarin.Forms
open System

open RC.Maya


/// All expressions for the upper part of the main page, the date selector and viewer of the
/// Tzolk’in date of the selected day.
[<AutoOpen>]
module TzolkinView =

    /// UI to show a Tzolk’in date, the Tzolk’in day number and day glyph, as images with the
    /// text below.
    let tzolkinDateView tzolkinDate isDark =
        let { TzolkinDate.T.number = (TzolkinNumber.T.TzolkinNumber tzNumInt)
              TzolkinDate.T.glyph = (TzolkinGlyph.T.TzolkinGlyph tzGlyphInt) } =
            tzolkinDate

        let numImgName = sprintf "number_%02d.png" tzNumInt
        let glyphImgName = sprintf "glyph_%02d.png" tzGlyphInt

        View.Grid (
            verticalOptions = LayoutOptions.FillAndExpand,
            horizontalOptions = LayoutOptions.FillAndExpand,
            columnSpacing = 5.,
            rowSpacing = 0.,
            rowdefs =
                [ Dimension.Absolute 67.
                  Dimension.Absolute 25. ],
            coldefs =
                [ Dimension.Absolute 67.
                  Dimension.Absolute 100. ],
            children =
                [ View
                    .Image(source = Image.fromPath numImgName,
                           scale = 1.0,
                           aspect = Aspect.AspectFill,
                           verticalOptions = LayoutOptions.Start,
                           horizontalOptions = LayoutOptions.End)
                      .Row(0)
                      .Column (0)
                  View
                      .Image(source = Image.fromPath glyphImgName,
                             scale = 1.0,
                             aspect = Aspect.AspectFill,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.Start)
                      .Row(0)
                      .Column (1)
                  View
                      .Label(text = tzolkinDate.number.ToString (),
                             horizontalTextAlignment = TextAlignment.Center,
                             fontSize = Style.fontSize,
                             textColor = Style.foregroundColor isDark,
                             backgroundColor = Style.backgroundColor isDark,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.EndAndExpand)
                      .Row(1)
                      .Column (0)
                  View
                      .Label(text = tzolkinDate.glyph.ToString (),
                             horizontalTextAlignment = TextAlignment.Center,
                             fontSize = Style.fontSize,
                             textColor = Style.foregroundColor isDark,
                             backgroundColor = Style.backgroundColor isDark,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.StartAndExpand)
                      .Row(1)
                      .Column (1) ]
        )

    let tzolkinDateViewFirst model isDark = tzolkinDateView (TzolkinDate.fromDate model.Date) isDark

    /// Select the Gregorian date and display the Tzolk’in date.
    let dateSelector model dispatch =
        [ tzolkinDateViewFirst model model.IsDarkMode

          View.Frame (
              backgroundColor = Style.backgroundColor model.IsDarkMode,
              content =
                  View.DatePicker (
                      minimumDate = DateTime.MinValue,
                      maximumDate = DateTime.MaxValue,
                      date = DateTime.Today,
                      format = localeFormat,
                      dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
                      width = 130.0,
                      verticalOptions = LayoutOptions.Fill,
                      textColor = Style.foregroundColor model.IsDarkMode,
                      backgroundColor = Style.backgroundColor model.IsDarkMode,
                      fontSize = Style.fontSize,
                      horizontalOptions = LayoutOptions.CenterAndExpand
                  )
          ) ]
