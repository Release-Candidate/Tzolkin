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
open Fabulous.XamarinForms.SkiaSharp
open Svg.Skia

open RC.Maya

/// All expressions for the upper part of the main page, the date selector and viewer of the
/// Tzolk’in date of the selected day.
[<AutoOpen>]
module TzolkinView =

    /// Helper.
    let formatGlyphTitle text =
        View.Span (text = sprintf "%s:\n" text,
                    fontAttributes = glyphDescFontAttrTitle,
                    textColor = glyphDescTextColorTitle,
                    fontSize = glyphDescFontSizeTitle)

    /// Helper.
    let formatGlyphValue text =
        View.Span (text = sprintf "\t\t\t%s\n" text,
                    fontAttributes = glyphDescFontAttrValue,
                    fontSize = glyphDescFontSizeValue,
                    textColor = glyphDescTextColorValue)

    /// Formats the Glyph description label.
    let formatGlypDescription (glyphDescription:TzolkinGlyph.GlyphDescription) dispatch =
        View.FormattedString (
            [   formatGlyphTitle "Significado"
                formatGlyphValue glyphDescription.Meaning

                formatGlyphTitle "Elementos o animal asociados"
                formatGlyphValue glyphDescription.ElementOrAnimal

                formatGlyphTitle "Rumbo asociado"
                formatGlyphValue glyphDescription.Direction

                formatGlyphTitle "Color asociado"
                formatGlyphValue glyphDescription.Color

                formatGlyphTitle "Dioses patronos"
                View.Span (text = sprintf "\t\t\t%s" glyphDescription.God,
                            fontAttributes = glyphDescFontAttrValue,
                            fontSize = glyphDescFontSizeValue,
                            textColor = glyphDescTextColorValue,
                            gestureRecognizers = [
                                View.TapGestureRecognizer(
                                    command = (fun () -> dispatch <| OpenURL glyphDescription.Url)
                                    )
                            ]
                 )
                View.Span (text = sprintf " %s" Style.linkSymbol,
                            fontAttributes = glyphDescFontAttrValue,
                            fontSize = glyphDescFontSizeValue,
                            textColor = glyphDescColorLink,
                            textDecorations = TextDecorations.Underline
                 )
                View.Span "\n  "
            ]
        )

    /// UI to show a Tzolk’in date, the Tzolk’in day number and day glyph, as images with the
    /// text below.
    let tzolkinDateView dispatch (tzolkinDate: TzolkinDate.T) isDark =
        View.Grid (
            columnSpacing = 5.,
            rowSpacing = 0.,
            padding = Thickness 0.0,
            gestureRecognizers =
                [ View.TapGestureRecognizer (
                      command = (fun () -> dispatch <| SetCurrentPage CalendarFilter)
                  ) ],
            rowdefs =
                [ Dimension.Absolute tzolkinImageHeight
                  Dimension.Absolute 25. ],
            coldefs =
                [ Dimension.Absolute 51.
                  Dimension.Absolute 95. ],
            children =
                [ View
                    .Image(source = Image.fromBytes cacheNumbers.[int tzolkinDate.Number - 1],
                           scale = 1.0,
                           aspect = Aspect.AspectFill,
                           verticalOptions = LayoutOptions.Start,
                           horizontalOptions = LayoutOptions.End)
                      .Row(0)
                      .Column (0)
                  View
                      .Image(source = Image.fromBytes cacheGlyphs.[int tzolkinDate.Glyph - 1],
                             scale = 1.0,
                             aspect = Aspect.AspectFill,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.Start)
                      .Row(0)
                      .Column (1)
                  View
                      .Label(text = tzolkinDate.Number.ToString (),
                             horizontalTextAlignment = TextAlignment.Center,
                             fontSize = Style.normalFontSize,
                             textColor = Style.foregroundLight,
                             backgroundColor = Style.backgroundNone,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.EndAndExpand)
                      .Row(1)
                      .Column (0)
                  View
                      .Label(text = tzolkinDate.Glyph.ToString (),
                             horizontalTextAlignment = TextAlignment.Center,
                             fontSize = Style.normalFontSize,
                             textColor = Style.foregroundLight,
                             backgroundColor = Style.backgroundNone,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.StartAndExpand)
                      .Row(1)
                      .Column (1) ]
        )

    /// Select the Gregorian date and display the Tzolk’in date.
    let dateSelector model dispatch date =
        [ View.Frame (
            backgroundColor = backgroundBrownLight,
            borderColor = backgroundBrownDark,
            hasShadow = true,
            padding = Thickness (0., 5.),
            content = tzolkinDateView dispatch (TzolkinDate.fromDate date) model.IsDarkMode
          )

          View.Frame (
              backgroundColor = backgroundBrownLight,
              borderColor = backgroundBrownDark,
              hasShadow = true,
              padding = Thickness (5., 5.),
              content =
                  View.DatePicker (
                      minimumDate = DateTime.MinValue,
                      maximumDate = DateTime.MaxValue,
                      date = date,
                      format = localeFormat,
                      dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
                      width = 100.0,
                      verticalOptions = LayoutOptions.Fill,
                      textColor = Style.foregroundLight,
                      backgroundColor = Style.backgroundNone,
                      fontSize = Style.normalFontSize,
                      horizontalOptions = LayoutOptions.EndAndExpand
                  )
          ) ]


    /// The day glyph description.
    let glyphDescription model dispatch date =
        let { TzolkinDate.Glyph = glyph } = TzolkinDate.fromDate date
        let glyphDesc = TzolkinGlyph.getDescription glyph

        View.Label (formattedText = formatGlypDescription glyphDesc dispatch,
                    lineBreakMode = LineBreakMode.WordWrap,
                    horizontalOptions = LayoutOptions.Center)


    let tzolkinCard model dispatch date =

        View.StackLayout (
            horizontalOptions = LayoutOptions.Center,
            padding = Thickness 5.,
            children =
                [ View.Frame (
                      backgroundColor = backgroundBrown,
                      cornerRadius = 20.,
                      padding = Thickness (0., 10., 0., 10.),
                      hasShadow = true,
                      content =
                          View.StackLayout (
                              padding = Thickness 10.,
                              orientation = setHorizontalIfLandscape model.IsLandscape,
                              horizontalOptions = LayoutOptions.Center,
                              verticalOptions = LayoutOptions.Center,
                              backgroundColor = Style.backgroundBrown,
                              children =
                                  [ View.StackLayout (
                                      padding = Thickness (5.0, 10.0, 5.0, 10.0),
                                      orientation = setVerticalIfLandscape model.IsLandscape,
                                      backgroundColor = Style.backgroundBrown,
                                      horizontalOptions = LayoutOptions.Center,
                                      children = dateSelector model dispatch date
                                    )

                                    //  separator model.IsLandscape model.IsDarkMode

                                    glyphDescription model dispatch date ]
                          )
                  ) ]
        )


    let tzolkinPage model dispatch =
        [ View.CarouselView (
            peekAreaInsets = Thickness 20.,
            loop = true,
            position = 1,
            backgroundColor = Style.backgroundBrownDark,
            verticalOptions = LayoutOptions.Center,
            horizontalOptions = LayoutOptions.Center,
            positionChanged = (fun args -> dispatch <| CarouselChanged args),
            items =
                [ tzolkinCard model dispatch model.Date
                  tzolkinCard model dispatch model.Date
                  tzolkinCard model dispatch model.Date ]
          )

          View.Label (
              text = versionInfo,
              fontSize = FontSize.fromNamedSize NamedSize.Micro,
              textColor = Style.foregroundColor model.IsDarkMode,
              backgroundColor = Style.backgroundBrownDark,//Style.backgroundColor model.IsDarkMode,
              verticalTextAlignment = TextAlignment.End,
              horizontalTextAlignment = TextAlignment.End,
              horizontalOptions = LayoutOptions.Fill,
              verticalOptions = LayoutOptions.Fill
          ) ]
