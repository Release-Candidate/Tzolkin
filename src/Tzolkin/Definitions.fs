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
open System.Diagnostics
open System
open System.Reflection
open Fabulous.XamarinForms.SkiaSharp
open Svg.Skia

open RC.Maya
open Xamarin.Forms



/// Holds the most basic definitions, the MVU model type `Model`, the MVU message type `Msg`,
/// the MVU `init` and `update` functions.
[<AutoOpen>]
module Definitions =

    /// App-wide constants =========================================================================

    let appNameInfo = sprintf "%s (Package %s)" AppInfo.Name AppInfo.PackageName

    let version = sprintf "%s (Build %s)" AppInfo.VersionString AppInfo.BuildString

    let versionInfo = sprintf "%s %s" appNameInfo version

    let screenDensity = DeviceDisplay.MainDisplayInfo.Density

    let screenWidth () = DeviceDisplay.MainDisplayInfo.Width

    let screenHeight () = DeviceDisplay.MainDisplayInfo.Height

    let numberPickList = "todos" :: List.map (fun x -> x.ToString ()) [ 1 .. 13 ]

    let glyphPickList = "todos" :: Array.toList TzolkinGlyph.glyphNames

    let localeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

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
        { DateList: list<DateTime>
          Date: System.DateTime
          ListTzolkinNumber: TzolkinNumber.T option
          ListTzolkinGlyph: TzolkinGlyph.T option
          Filter: DateFilter
          CurrentPage: Pages
          IsDarkMode: bool
          IsLandscape: bool
          ShowSystemAppInfo: bool
          DateViewScrollDir: int }

    let modelTzolkinDate model =
        match model.ListTzolkinNumber, model.ListTzolkinGlyph with
        | None, Some tz ->
            { TzolkinDate.Number = TzolkinNumber.T.TzolkinNumber 1
              TzolkinDate.Glyph = tz }
        | Some tz, None ->
            { TzolkinDate.Number = tz
              TzolkinDate.Glyph = TzolkinGlyph.T.TzolkinGlyph 1 }
        | Some tzn, Some tzg ->
            { TzolkinDate.Number = tzn
              TzolkinDate.Glyph = tzg }
        | _, _ ->
            { TzolkinDate.Number = TzolkinNumber.T.TzolkinNumber 1
              TzolkinDate.Glyph = TzolkinGlyph.T.TzolkinGlyph 1 }

    let modelNumToInt model =
        match model.ListTzolkinNumber with
        | None -> 0
        | Some tz -> int tz

    let modelGlyphToInt model =
        match model.ListTzolkinGlyph with
        | None -> 0
        | Some tz -> int tz

    let genericLlistTzolkin getLastList getNextList numElem tzolkin date =
        let lastList =
                   getLastList numElem tzolkin date
                   |> List.rev

        let nextList = getNextList numElem tzolkin date
        lastList @ nextList

    let dateListTzolkin numElem tzolkin date =
        genericLlistTzolkin TzolkinDate.getLastList TzolkinDate.getNextList numElem tzolkin date

    let numListTzolkin numElem tzolkin date =
        genericLlistTzolkin TzolkinNumber.getLastList TzolkinNumber.getNextList numElem tzolkin date

    let glyphListTzolkin numElem tzolkin date =
           genericLlistTzolkin TzolkinGlyph.getLastList TzolkinGlyph.getNextList numElem tzolkin date


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
        | HasScrolled of ItemsViewScrolledEventArgs
        | NewDateViewItemsNeeded


    // Widget references ===========================================================================
    /// Instances of widgets needed to interact with.
    let dateListView = ViewRef<CollectionView> ()
    let dayPicker = ViewRef<Xamarin.Forms.Picker> ()
    let monthPicker = ViewRef<Xamarin.Forms.Picker> ()
    let yearPicker = ViewRef<Xamarin.Forms.Entry> ()

    // Commands ====================================================================================
    let cmdOpenUrl (url) =
              Launcher.OpenAsync (new Uri (url))
              |> Async.AwaitTask
              |> Async.StartImmediate
              Cmd.none

    let cmdCollectionView =
        Cmd.ofSub (fun dispatch ->
                   match dateListView.TryValue with
                   | None -> ()
                   | Some reference -> reference.Scrolled.Add (fun args -> dispatch <| HasScrolled args)
                  )

    // Widget related ==============================================================================

    let tzolkinImageHeight = 67.0

    let streamNumber = new IO.MemoryStream ()

    let streamGlyph = new IO.MemoryStream ()

    let getResourceStream path =
         let assembly = IntrospectionExtensions.GetTypeInfo(typedefof<Model>).Assembly
         assembly.GetManifestResourceStream (path)

    let getImageStream filename =
        sprintf "TzolkinApp.images.%s" filename |> getResourceStream

    let getSVGStream name =
        sprintf "TzolkinApp.images.%s.svg" name |> getResourceStream

    let getSVGGlyphStream (glyph:TzolkinGlyph.T) =
        int glyph
        |> sprintf "TzolkinApp.images.glyph_%02d.svg"
        |> getResourceStream

    let getSVGNumberStream (number:TzolkinNumber.T) =
        int number
        |> sprintf "TzolkinApp.images.number_%02d.svg"
        |> getResourceStream

    let getPNGFromSVG spaceHeight name =
        let svg = new SKSvg ()
        let svgPicture = svg.Load (getSVGStream name)
        let height = float32 <| spaceHeight * screenDensity
        let scaleFac = height / svgPicture.CullRect.Height
        let bitmap1 = svgPicture.ToBitmap (SkiaSharp.SKColor.Empty,
                                            scaleFac, scaleFac,
                                            SkiaSharp.SKColorType.Rgba8888,
                                            SkiaSharp.SKAlphaType.Premul,
                                            SkiaSharp.SKColorSpace.CreateSrgb ()  )
        let image =  SkiaSharp.SKImage.FromBitmap (bitmap1)
        let data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100)
        let stream = data.AsStream (true)
        let data = Array.zeroCreate <| int stream.Length
        stream.Read (data, 0, data.Length) |> ignore
        data

    let getPNGStreamNumber number =
        int number
        |> sprintf "number_%02d"
        |> getPNGFromSVG tzolkinImageHeight

    let getPNGStreamGlyph glyph =
        int glyph
        |> sprintf "glyph_%02d"
        |> getPNGFromSVG tzolkinImageHeight

    let cacheGlyphs = [ for i in [1 .. 20] -> getPNGStreamGlyph <| TzolkinGlyph.T.TzolkinGlyph i ]

    let cacheNumbers = [ for i in [1 .. 13] -> getPNGStreamNumber <| TzolkinNumber.T.TzolkinNumber i ]

    // Init ========================================================================================

    /// Initial state of the MVU model.
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
          DateViewScrollDir = 1 }

    /// Initialize the model and commands.
    let init () = initModel, Cmd.none

    // Functions needed by `update` ================================================================

    /// Reset the text in the year entry.
    let resetYear model =
        match yearPicker.TryValue with
        | None -> ()
        | Some textEntry -> textEntry.Text <- ""



    // Update ======================================================================================

    /// The update function of MVU.
    let update msg model =
        match msg with

        | SetCurrentPage page ->
            let tzolkin = TzolkinDate.fromDate model.Date
            { model with CurrentPage = page
                         ListTzolkinGlyph = Some tzolkin.Glyph
                         ListTzolkinNumber = Some tzolkin.Number
                         DateList = dateListTzolkin 20 tzolkin model.Date },
            cmdCollectionView

        | SetDate date -> { model with Date = date }, Cmd.none

        | SetListNumber newNum ->
            match newNum, model.ListTzolkinGlyph with
            | 0, None -> { model with ListTzolkinNumber = None
                                      DateList = [ for i in [-20 .. 20] ->
                                                    model.Date + TimeSpan.FromDays (float i)] },
                         Cmd.none

            | _, None ->
                { model with
                      ListTzolkinNumber = Some <| TzolkinNumber.T.TzolkinNumber newNum
                      DateList = numListTzolkin
                                      20
                                      (TzolkinNumber.T.TzolkinNumber newNum)
                                      model.Date },
                Cmd.none

            | 0, Some glyph ->
                { model with
                      ListTzolkinNumber = None
                      DateList = glyphListTzolkin 20 glyph model.Date },
                Cmd.none

            | _, Some glyph ->
                { model with
                    ListTzolkinNumber = Some <| TzolkinNumber.T.TzolkinNumber newNum
                    DateList = dateListTzolkin
                                    20
                                    (TzolkinDate.create (TzolkinNumber.T.TzolkinNumber newNum) glyph)
                                    model.Date },
                Cmd.none

        | SetListGlyph newGlyph ->
           match newGlyph, model.ListTzolkinNumber with
           | 0, None -> { model with ListTzolkinGlyph = None
                                     DateList = [ for i in [-20 .. 20] ->
                                                        model.Date + TimeSpan.FromDays (float i)] },
                        Cmd.none

           | _, None ->
                { model with
                    ListTzolkinGlyph = Some <| TzolkinGlyph.T.TzolkinGlyph newGlyph
                    DateList = glyphListTzolkin
                                    20
                                    (TzolkinGlyph.T.TzolkinGlyph newGlyph)
                                    model.Date },
                Cmd.none

           | 0, Some number ->
                { model with
                    ListTzolkinGlyph = None
                    DateList = numListTzolkin 20 number model.Date },
                Cmd.none

           | _, Some number ->
                { model with
                    ListTzolkinGlyph = Some <| TzolkinGlyph.T.TzolkinGlyph newGlyph
                    DateList = dateListTzolkin
                                    20
                                    (TzolkinDate.create number (TzolkinGlyph.T.TzolkinGlyph newGlyph))
                                    model.Date },
                Cmd.none

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
            resetYear model

            { model with
                  Filter = { Day = 0; Month = 0; Year = "" } },
            Cmd.none

        | SetAppTheme (theme: OSAppTheme) ->
            match theme with
            | OSAppTheme.Dark -> { model with IsDarkMode = true }
            | _ -> { model with IsDarkMode = false }
            , Cmd.none

        | SetOrientation (x, y) ->
            match x, y with
            | width, height when width > height -> { model with IsLandscape = true }, Cmd.none
            | _, _ -> { model with IsLandscape = false }, Cmd.none

        | ShowSystemAppInfo doShow ->
            match doShow with
            | true -> { model with ShowSystemAppInfo = true }, Cmd.none
            | false -> { model with ShowSystemAppInfo = false }, Cmd.none

        | CarouselChanged args ->
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

        | OpenURL url -> model, cmdOpenUrl url

        | HasScrolled arg ->
            { model with DateViewScrollDir = if arg.VerticalDelta > 0. then 1 else -1 }, Cmd.none

        | NewDateViewItemsNeeded -> model, Cmd.none

