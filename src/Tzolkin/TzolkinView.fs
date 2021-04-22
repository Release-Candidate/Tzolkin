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

/// All expressions for the first page, the date selector and viewer of the
/// Tzolk’in date of the selected day.
[<AutoOpen>]
module TzolkinView =

    /// <summary>
    /// Return the `Span` containing the format for a Tzolk’in day glyph
    /// description's title.
    /// </summary>
    /// <param name="text">The title of the description.</param>
    /// <returns>A `Span` containing the format for the title of a Tzolk’in day
    /// glyph description.</returns>
    let formatGlyphTitle text =
        View.Span (text = sprintf "%s:\n" text,
                    fontAttributes = glyphDescFontAttrTitle,
                    textColor = glyphDescTextColorTitle,
                    fontSize = glyphDescFontSizeTitle)

    /// <summary>
    /// Return the `Span` containing the format for a Tzolk’in day glyph
    /// description.
    /// </summary>
    /// <param name="text">The description of the Tzolk’in day glyph.</param>
    /// <returns>A `Span` holding the format of a Tzolk’in day glyph
    /// description.</returns>
    let formatGlyphValue text =
        View.Span (text = sprintf "\t\t\t%s\n" text,
                    fontAttributes = glyphDescFontAttrValue,
                    fontSize = glyphDescFontSizeValue,
                    textColor = glyphDescTextColorValue)

    /// <summary>
    /// Format the Tzolk’in day glyph description, to use with a label.
    /// </summary>
    /// <param name="glyphDescription">The record holding the Tzolk’in day
    /// glyph description.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A `FormattedString` to use to format a Tzolk’in day glyph
    /// description on a label.</returns>
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

    /// <summary>
    /// UI to show a Tzolk’in date, the Tzolk’in day number and day glyph, as
    /// images with the text below.
    /// </summary>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="tzolkinDate">The Tzolk’in date to display.</param>
    /// <param name="isDark">Is the dark mode enabled?</param>
    /// <returns>A `Grid` holding the Tzolk’in day number and glyph as images
    /// and text below.</returns>
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

    /// <summary>
    /// Show the Tzolk’in date on the left and a date picker to select the date
    /// to display on the right.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The gregorian date to display as a Tzolk’in date.</param>
    /// <returns>A list of `Frame` to show the Tzolk’in date of the date
    /// selected in the second Frame.</returns>
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

    /// <summary>
    /// Return a label containing the Tzolk’in day glyph description.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The gregorian date to display the Tzolk’in day glyph
    /// of.</param>
    /// <returns>A `Label` holding the Tzolk’in day glyph description.</returns>
    let glyphDescription model dispatch date =
        let { TzolkinDate.Glyph = glyph } = TzolkinDate.fromDate date
        let glyphDesc = TzolkinGlyph.getDescription glyph

        View.Label (formattedText = formatGlypDescription glyphDesc dispatch,
                    lineBreakMode = LineBreakMode.WordWrap,
                    horizontalOptions = LayoutOptions.Center)


    /// <summary>
    /// Display a `Frame` containing the Tzolk’in date, the gregorian date and
    /// the Tzolk’in day glyph description of the gregorian date selected with
    /// the date picker.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The gregorian date to display as Tzolk’in date.</param>
    /// <returns>A `StackLayout` holding the Tzolk’in date, the gregorian date and
    /// the Tzolk’in day glyph description of the gregorian date selected with
    /// the date picker.</returns>
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


    /// <summary>
    /// Display the date view page, the Tzolk’in day information in a carousel
    /// view of consecutive days.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A list containing the date view page.</returns>
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
