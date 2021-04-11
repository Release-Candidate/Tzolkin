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
            padding = Thickness 0.0,
            rowdefs =
                [ Dimension.Absolute 67.
                  Dimension.Absolute 25. ],
            coldefs =
                [ Dimension.Absolute 62.
                  Dimension.Absolute 80. ],
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
                             textColor = Color.Black,
                             backgroundColor = Color.Default,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.EndAndExpand)
                      .Row(1)
                      .Column (0)
                  View
                      .Label(text = tzolkinDate.glyph.ToString (),
                             horizontalTextAlignment = TextAlignment.Center,
                             fontSize = Style.fontSize,
                             textColor = Color.Black,
                             backgroundColor = Color.Default,
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
              backgroundColor = setBrown, //Style.backgroundColor model.IsDarkMode,
              content =
                  View.DatePicker (
                      minimumDate = DateTime.MinValue,
                      maximumDate = DateTime.MaxValue,
                      date = model.Date,
                      format = localeFormat,
                      dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
                      width = 120.0,
                      verticalOptions = LayoutOptions.Fill,
                      textColor = Color.Black,
                      backgroundColor = Color.Default,
                      fontSize = Style.fontSize,
                      horizontalOptions = LayoutOptions.CenterAndExpand
                  )
          ) ]


    /// The day glyph description.
    let glyphDescription model dispatch =
        let { TzolkinDate.glyph = glyph } = TzolkinDate.fromDate model.Date
        let glyphDesc = TzolkinGlyph.getDescription glyph

        View.Grid (
            backgroundColor = Style.backgroundColor model.IsDarkMode,
            verticalOptions = LayoutOptions.FillAndExpand,
            horizontalOptions = LayoutOptions.FillAndExpand,
            padding = Thickness 5.,
            rowdefs =
                [ Dimension.Auto
                  Dimension.Auto
                  Dimension.Auto
                  Dimension.Auto
                  Dimension.Auto
                  Dimension.Auto
                  Dimension.Star
                  Dimension.Absolute 15. ],
            coldefs = [ Dimension.Auto; Dimension.Auto ],
            children =
                [ View
                    .Button(text = "-1",
                            textColor = Style.foregroundColor model.IsDarkMode,
                            fontSize = Style.fontSize,
                            command = (fun () -> dispatch (AddDays -1)))
                      .Row(0)
                      .Column (0)
                  View
                      .Button(text = "+1",
                              textColor = Style.foregroundColor model.IsDarkMode,
                              fontSize = Style.fontSize,
                              command = (fun () -> dispatch (AddDays 1)))
                      .Row(0)
                      .Column (1)
                  View
                      .Label(text = "Significado:",
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(1)
                      .Column (0)
                  View
                      .Label(text = sprintf "%s" (glyphDesc.meaning),
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(1)
                      .Column (1)
                  View
                      .Label(text = "Elementos o animal asociados:",
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(2)
                      .Column(0)
                      .ColumnSpan (2)
                  View
                      .Label(text = sprintf "%s" (glyphDesc.elementOrAnimal),
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(3)
                      .Column (1)
                  View
                      .Label(text = "Rumbo asociado:",
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(4)
                      .Column (0)
                  View
                      .Label(text = sprintf "%s" (glyphDesc.direction),
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(4)
                      .Column (1)
                  View
                      .Label(text = "Color asociado:",
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(5)
                      .Column (0)
                  View
                      .Label(text = sprintf "%s" (glyphDesc.color),
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(5)
                      .Column (1)
                  View
                      .Label(text = "Dioses patronos:",
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(6)
                      .Column (0)
                  View
                      .Label(text = sprintf "%s" (glyphDesc.god),
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             fontSize = Style.fontSize)
                      .Row(6)
                      .Column (1)
                  View
                      .Label(text = versionInfo,
                             fontSize = FontSize.fromNamedSize NamedSize.Micro,
                             textColor = Style.foregroundColor model.IsDarkMode,
                             backgroundColor = Style.backgroundColor model.IsDarkMode,
                             verticalTextAlignment = TextAlignment.End,
                             horizontalTextAlignment = TextAlignment.End,
                             horizontalOptions = LayoutOptions.Fill,
                             verticalOptions = LayoutOptions.Fill)
                      .Row(7)
                      .Column(0)
                      .ColumnSpan (2) ]
        )
