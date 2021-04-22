// SPDX-License-Identifier: MIT
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
open System.Globalization
open System
open System.Reflection
open Svg.Skia

open RC.Maya



/// Holds the most basic definitions, the MVU model type `Model`, the MVU message type `Msg`,
/// the MVU `init` and `update` functions.
[<AutoOpen>]
module Definitions =

    // App-wide constants =========================================================================

    /// <summary>
    /// App name and package name, from the Android / IOS manifest.
    /// </summary>
    let appNameInfo = sprintf "%s (Package %s)" AppInfo.Name AppInfo.PackageName

    /// <summary>
    /// Version and build number.
    /// </summary>
    let version = sprintf "%s (Build %s)" AppInfo.VersionString AppInfo.BuildString

    /// <summary>
    /// App name and version, all in one string.
    /// </summary>
    let versionInfo = sprintf "%s %s" appNameInfo version

    /// <summary>
    /// Density of the device's screen, pixels are density x units.
    /// </summary>
    let screenDensity = DeviceDisplay.MainDisplayInfo.Density

    /// <summary>
    /// Width of the device's screen, changes in landscape and portrait mode.
    /// </summary>
    let screenWidth () = DeviceDisplay.MainDisplayInfo.Width

    /// <summary>
    /// Height of the device's screen, changes in landscape and portrait mode.
    /// </summary>
    let screenHeight () = DeviceDisplay.MainDisplayInfo.Height

    /// <summary>
    /// All Tzolk’in day numbers, including "todos" for "all" in the first position, with index 0.
    /// </summary>
    let numberPickList = "todos" :: List.map (fun x -> x.ToString ()) [ 1 .. 13 ]

    /// <summary>
    /// All Tzolk’in day glyphs, including "todos" for "all" in the first position, with index 0.
    /// </summary>
    let glyphPickList = "todos" :: Array.toList TzolkinGlyph.glyphNames

    /// <summary>
    /// The current locale's date separator (like `/`, `.`, `-` ...).
    /// </summary>
    let localeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

    /// <summary>
    /// A string describing the current locale's short date format, like "MM/dd/yyyy".
    /// </summary>
    let localeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

    // The model ===================================================================================

    /// The pages of the App.
    type Pages =
        | Home
        | CalendarFilter

    /// Record to hold the data needed to filter dates.
    type DateFilter = { Day: int; Month: int; Year: string }

    /// The MVU model.
    type Model =
        { DateList: DateTime list
          Date: DateTime
          ListTzolkinNumber: TzolkinNumber.T option
          ListTzolkinGlyph: TzolkinGlyph.T option
          Filter: DateFilter
          CurrentPage: Pages
          IsDarkMode: bool
          IsLandscape: bool
          ShowSystemAppInfo: bool
          LastFilterListIdx: int }

    /// <summary>
    /// Return the `TzolkinNumber` stored in the model as int, 0 if it is `None`.
    /// </summary>
    /// <param name="model">The MVU's model.</param>
    /// <returns>0, if the given model holds no `TzolkinNumber`,
    /// the `TzolkinNumber` as int else.</returns>
    let modelNumToInt model =
        match model.ListTzolkinNumber with
        | None -> 0
        | Some tz -> int tz

    /// <summary>
    /// Return the `TzolkinGlyph` stored in the model as int, 0 if it is `None`.
    /// </summary>
    /// <param name="model">The MVU's model.</param>
    /// <returns>0, if the given model holds no `TzolkinGlyph`,
    /// the `TzolkinGlyph` as int else.</returns>
    let modelGlyphToInt model =
        match model.ListTzolkinGlyph with
        | None -> 0
        | Some tz -> int tz

    /// <summary>
    /// Return a list of gregorian dates with the sameTzolk’in date as `tzolkin`.
    /// </summary>
    /// <param name="getLastList">Function to get the list of dates before `date`.</param>
    /// <param name="getNextList">Function to get the list of dates after `date`</param>
    /// <param name="numElem">The number of elements in each list, the returned list has
    /// 2*`numElem` elements.</param>
    /// <param name="tzolkin">The `TzolkinDate` to return as a list of gregorian
    /// dates with the same Tzolk’in date as `tzolkin`.</param>
    /// <param name="date">The date to start the list, return `numElem` dates
    /// with the same Tzolk’in date as `tzolkin` before and after that date.</param>
    /// <returns>A list of gregorian dates with the same Tzolk’in date as `tzolkin`.</returns>
    let genericListTzolkin getLastList getNextList numElem tzolkin date =
        let lastList =
                   getLastList numElem tzolkin date
                   |> List.rev

        let nextList = getNextList numElem tzolkin date
        lastList @ nextList

    /// <summary>
    /// Return a list of gregorian dates with the same Tzolk’in date as ´tzolkin`.
    /// </summary>
    /// <param name="numElem">The number of elements in the "before" and "after"
    /// lists.</param>
    /// <param name="tzolkin">The Tzolk’in date to search for.</param>
    /// <param name="date">The starting date, the "center" date of the list.</param>
    /// <returns>A list of gregorian dates with the same Tzolk’in date as
    /// ´tzolkin`.</returns>
    let dateListTzolkin numElem tzolkin date =
        genericListTzolkin TzolkinDate.getLastList TzolkinDate.getNextList numElem tzolkin date

    /// <summary>
    /// Return a list of gregorian dates with the same Tzolk’in number as ´tzolkin`.
    /// </summary>
    /// <param name="numElem">The number of elements in the "before" and "after"
    /// lists.</param>
    /// <param name="tzolkin">The Tzolk’in date to search for.</param>
    /// <param name="date">The starting date, the "center" date of the list.</param>
    /// <returns>A list of gregorian dates with the same Tzolk’in day number as
    /// ´tzolkin`.</returns>
    let numListTzolkin numElem tzolkin date =
        genericListTzolkin TzolkinNumber.getLastList TzolkinNumber.getNextList numElem tzolkin date

    /// <summary>
    /// Return a list of gregorian dates with the same Tzolk’in number as ´tzolkin`.
    /// </summary>
    /// <param name="numElem">The number of elements in the "before" and "after"
    /// lists.</param>
    /// <param name="tzolkin">The Tzolk’in date to search for.</param>
    /// <param name="date">The starting date, the "center" date of the list.</param>
    /// <returns>A list of gregorian dates with the same Tzolk’in day glyph as
    /// ´tzolkin`.</returns>
    let glyphListTzolkin numElem tzolkin date =
           genericListTzolkin TzolkinGlyph.getLastList TzolkinGlyph.getNextList numElem tzolkin date


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
        | FilterCarouselHeight

    // Widget references ===========================================================================
    // Instances of widgets needed to interact with.

    /// <summary>
    /// Reference to the filter view's carousel view.
    /// </summary>
    let dateListView = ViewRef<CustomCarouselView> () //ViewRef<CollectionView> ()

    /// <summary>
    /// Reference to the day filter picker in the filter view.
    /// </summary>
    let dayPicker = ViewRef<Picker> ()

    /// <summary>
    /// Reference to the month filter picker in the filter view.
    /// </summary>
    let monthPicker = ViewRef<Picker> ()

    /// <summary>
    /// Reference to the year filter entry in the filter view.
    /// </summary>
    let yearPicker = ViewRef<Entry> ()

    // Commands ====================================================================================
    /// <summary>
    /// Opens a URL using the system's default browser.
    /// </summary>
    /// <param name="url">The URL to open as a string.</param>
    /// <returns>`Cmd.none`</returns>
    let cmdOpenUrl (url) =
              Launcher.OpenAsync (new Uri (url))
              |> Async.AwaitTask
              |> Async.StartImmediate
              Cmd.none

    /// <summary>
    /// Subscribe the message `FilterCarouselHeight` to a size change of the
    /// filter view's carousel view.
    /// </summary>
    /// <returns>`Cmd.ofSub` - the function subscribe to a size change of the
    /// filter view's carousel view.</returns>
    let cmdDateListViewHeight =
        Cmd.ofSub (fun dispatch ->
                   match dateListView.TryValue with
                   | None -> ()
                   | Some reference ->
                            reference.SizeChanged.Add (fun _ -> dispatch FilterCarouselHeight)
                   )

    // Widget related ==============================================================================

    /// <summary>
    /// The center index of the list of dates in the filter view.
    /// </summary>
    let filterViewStartingIdx = 250

    /// <summary>
    /// The height of a Tzolk’in day number oder day glyph image in the first page.
    /// </summary>
    let tzolkinImageHeight = 67.0

    // let streamNumber = new IO.MemoryStream ()

    // let streamGlyph = new IO.MemoryStream ()

    /// <summary>
    /// Return a stream of a resource's data.
    /// </summary>
    /// <param name="path">The path of the resource, like "TzolkinApp.images.number-02.svg".</param>
    /// <returns>A stream of the resource's data.</returns>
    let getResourceStream path =
         let assembly = IntrospectionExtensions.GetTypeInfo(typedefof<Model>).Assembly
         assembly.GetManifestResourceStream (path)

    /// <summary>
    /// Return a stream of a resource's data, given as filename in
    /// `TzolkinApp/images/`.
    /// </summary>
    /// <param name="filename">The filename of the resource, without the
    /// directory `TzolkinApp/images/`</param>
    /// <returns>A stream of the resource's data.</returns>
    let getImageStream filename =
        sprintf "TzolkinApp.images.%s" filename |> getResourceStream

    /// <summary>
    /// Return a stream of a SVG image's data.
    /// </summary>
    /// <param name="name">The name of the SVG to load, without extension ".svg".</param>
    /// <returns>A stream of the resource's data.</returns>
    let getSVGStream name =
        sprintf "TzolkinApp.images.%s.svg" name |> getResourceStream

    /// <summary>
    /// Return a stream of the SVG image of a Tzolk’in day glyph.
    /// </summary>
    /// <param name="glyph">The Tzolk’in day glyph to return the SVG of.</param>
    /// <returns>A stream of the resource's data.</returns>
    let getSVGGlyphStream (glyph:TzolkinGlyph.T) =
        int glyph
        |> sprintf "TzolkinApp.images.glyph_%02d.svg"
        |> getResourceStream

    /// <summary>
    /// Return a stream of the SVG image of a Tzolk’in day number.
    /// </summary>
    /// <param name="number">The Tzolk’in day number to return the SVG of.</param>
    /// <returns>A stream of the resource's data.</returns>
    let getSVGNumberStream (number:TzolkinNumber.T) =
        int number
        |> sprintf "TzolkinApp.images.number_%02d.svg"
        |> getResourceStream

    /// <summary>
    /// Convert a SVG image to a PNG image with the given height.
    /// </summary>
    /// <param name="spaceHeight">The height the PNG should have, in units
    /// (not pixels).</param>
    /// <param name="name">The name of the SVG to convert, without the ".svg"
    /// suffix</param>
    /// <returns>The image data of a SVG converted to a PNG</returns>
    let getPNGFromSVG spaceHeight name =
        let svg = new SKSvg ()
        let svgPicture = svg.Load (getSVGStream name)
        let height = float32 <| spaceHeight * screenDensity
        let scaleFac = height / svgPicture.CullRect.Height
        let bitmap1 = svgPicture.ToBitmap (SkiaSharp.SKColor.Empty,
                                    scaleFac,
                                    scaleFac,
                                    SkiaSharp.SKColorType.Rgba8888,
                                    SkiaSharp.SKAlphaType.Premul,
                                    SkiaSharp.SKColorSpace.CreateSrgb () )
        let image =  SkiaSharp.SKImage.FromBitmap (bitmap1)
        let data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100)
        let stream = data.AsStream (true)
        let data = Array.zeroCreate <| int stream.Length
        stream.Read (data, 0, data.Length) |> ignore
        data

    /// <summary>
    /// Return a PNG image data of the Tzolk’in day number `number`.
    /// </summary>
    /// <param name="number">The Tzolk’in day number.</param>
    /// <returns>The PNG image data of a Tzolk’in day number image.</returns>
    let getPNGStreamNumber number =
        int number
        |> sprintf "number_%02d"
        |> getPNGFromSVG tzolkinImageHeight

    /// <summary>
    /// Return a PNG image data of the Tzolk’in day glyph `glyph`.
    /// </summary>
    /// <param name="glyph">The Tzolk’in day glyph.</param>
    /// <returns>The PNG image data of a Tzolk’in day glyph image.</returns>
    let getPNGStreamGlyph glyph =
        int glyph
        |> sprintf "glyph_%02d"
        |> getPNGFromSVG tzolkinImageHeight

    /// <summary>
    /// The list of all Tzolk’in day glyph PNG images (as data arrays).
    /// </summary>
    let cacheGlyphs = [ for i in [1 .. 20] -> getPNGStreamGlyph <| TzolkinGlyph.T.TzolkinGlyph i ]

    /// <summary>
    /// The list of all Tzolk’in day number PNG images (as data arrays).
    /// </summary>
    let cacheNumbers = [ for i in [1 .. 13] -> getPNGStreamNumber <| TzolkinNumber.T.TzolkinNumber i ]

    // Init ========================================================================================

    /// <summary>
    /// Initial state of the MVU model.
    /// </summary>
    let initModel =
        { DateList = []
          Date = System.DateTime.Today
          ListTzolkinNumber = Some (TzolkinNumber.T.TzolkinNumber 8)
          ListTzolkinGlyph = Some (TzolkinGlyph.T.TzolkinGlyph 5)
          Filter = { Day = 0; Month = 0; Year = "" }
          CurrentPage = Home
          IsDarkMode =
              if Application.Current.RequestedTheme = OSAppTheme.Dark then
                  true
              else
                  false
          IsLandscape = false
          ShowSystemAppInfo = false
          LastFilterListIdx = filterViewStartingIdx }

    /// <summary>
    /// Initialize the model and commands.
    /// </summary>
    /// <returns>The initialized model and `Cmd.none` as tuple.</returns>
    let init () = initModel, Cmd.none

    // Functions needed by `update` ================================================================

    /// <summary>
    /// Reset the text in the year entry.
    /// </summary>
    /// <returns>`()`</returns>
    let resetYear () =
        match yearPicker.TryValue with
        | None -> ()
        | Some textEntry -> textEntry.Text <- ""

    /// <summary>
    /// Set the filter list view's carousel view's scale factors depending on the
    /// devices orientation. The scale factors are needed for the size of the
    /// `PeekAreaInsets`, to have a good looking distance between each row.
    /// </summary>
    /// <param name="isL">Is landscape orientation?</param>
    /// <returns>The scale factor for the carousel view's `PeekAreaInsets`,
    /// depending on the orientation of the device.</returns>
    let setDateCarouselFactors isL =
           if isL then 2.35 else 2.15

