// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     Tzolkin.fs
//
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open System
open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Xamarin.Essentials
open System.Globalization

open RC.Maya


/// The module holds the IOS and Android app.
module TzolkinApp =


    /// App-wide constants.
    let version =
        sprintf "%s (Build %s)" VersionTracking.CurrentVersion VersionTracking.CurrentBuild

    let fontSize = FontSize.fromNamedSize NamedSize.Medium

    let backgroundLight = Color.Default

    let backgroundDark = Color.FromHex "#1F1B24"

    let foregroundLight = Color.Black

    let foregroundDark = Color.WhiteSmoke

    let numberPickList = List.map (fun x -> x.ToString ()) [ 1 .. 13 ]

    let glyphPickList = Array.toList TzolkinGlyph.glyphNames

    let localeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

    let localeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

    /// Record to hold the data needed to filter dates.
    type DateFilter = { day: int; month: int; year: string }

    /// The MVU model.
    type Model =
        { Date: System.DateTime
          ListTzolkinDate: TzolkinDate.T
          Filter: DateFilter
          IsDarkMode: bool
          IsLandscape: bool }

    /// MVU messages.
    type Msg =
        | SetDate of System.DateTime
        | SetListNumber of int
        | SetListGlyph of int
        | SetFilterDay of int
        | SetFilterMonth of int
        | SetFilterYear of string
        | DoResetFilter
        | ScrollListCenter
        | SetAppTheme of OSAppTheme
        | SetOrientation of float * float

    /// Instances of widgets needed to interact with.
    let dateListView = ViewRef<CustomListView> ()
    let dayPicker = ViewRef<Xamarin.Forms.Picker> ()
    let monthPicker = ViewRef<Xamarin.Forms.Picker> ()
    let yearPicker = ViewRef<Xamarin.Forms.Entry> ()

    let cmdScrollToCenter = ScrollListCenter |> Cmd.ofMsg

    let scrollToCenter model =
        match dateListView.TryValue with
        | None -> ()
        | Some listView ->
            let centerItem = List.ofSeq (Seq.cast<ViewElement> listView.ItemsSource)

            listView.ScrollTo (
                item = centerItem.Tail,
                position = ScrollToPosition.End,
                animated = true
            )

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

    /// Initial state of the MVU model.
    let initModel =
        { Date = System.DateTime.Today
          ListTzolkinDate =
              { number = TzolkinNumber.T.TzolkinNumber 8
                glyph = TzolkinGlyph.T.TzolkinGlyph 5 }
          Filter = { day = 0; month = 0; year = "" }
          IsDarkMode =
              if Application.Current.RequestedTheme = OSAppTheme.Dark then
                  true
              else
                  false
          IsLandscape = false }

    /// Initialize the model and commands.
    let init () = initModel, Cmd.none

    /// The update function of MVU.
    let update msg model =
        match msg with
        | SetDate date -> { model with Date = date }, Cmd.none

        | SetListNumber newNum ->
            { model with
                  ListTzolkinDate =
                      { model.ListTzolkinDate with
                            number = (TzolkinNumber.T.TzolkinNumber newNum) } },
            Cmd.none

        | SetListGlyph newGlyph ->
            { model with
                  ListTzolkinDate =
                      { model.ListTzolkinDate with
                            glyph = (TzolkinGlyph.T.TzolkinGlyph newGlyph) } },
            Cmd.none

        | SetFilterDay newday ->
            { model with
                  Filter = { model.Filter with day = newday } },
            Cmd.none

        | SetFilterMonth newMonth ->
            { model with
                  Filter = { model.Filter with month = newMonth } },
            Cmd.none

        | SetFilterYear newYear ->
            { model with
                  Filter = { model.Filter with year = newYear } },
            Cmd.none

        | DoResetFilter ->
            { model with
                  Filter = { day = 0; month = 0; year = "" } },
            Cmd.none

        | ScrollListCenter ->
            scrollToCenter model
            model, Cmd.none

        | SetAppTheme (theme: OSAppTheme) ->
            match theme with
            | OSAppTheme.Dark -> { model with IsDarkMode = true }
            | _ -> { model with IsDarkMode = false }
            , Cmd.none

        | SetOrientation (x, y) ->
            match x, y with
            | width, height when width > height -> { model with IsLandscape = true }, Cmd.none
            | _, _ -> { model with IsLandscape = false }, Cmd.none

    let foregroundColor isDark =
        match isDark with
        | true -> foregroundDark
        | false -> foregroundLight

    let backgroundColor isDark =
        match isDark with
        | true -> backgroundDark
        | false -> backgroundLight

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
                             fontSize = fontSize,
                             textColor = foregroundColor isDark,
                             backgroundColor = backgroundColor isDark,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.EndAndExpand)
                      .Row(1)
                      .Column (0)
                  View
                      .Label(text = tzolkinDate.glyph.ToString (),
                             horizontalTextAlignment = TextAlignment.Center,
                             fontSize = fontSize,
                             textColor = foregroundColor isDark,
                             backgroundColor = backgroundColor isDark,
                             verticalOptions = LayoutOptions.Start,
                             horizontalOptions = LayoutOptions.StartAndExpand)
                      .Row(1)
                      .Column (1) ]
        )

    let tzolkinDateViewFirst model isDark = tzolkinDateView (TzolkinDate.fromDate model.Date) isDark

    /// Select the Gregorian date and display the Tzolk’in date.
    let dateSelector model dispatch =
        [

          tzolkinDateViewFirst model model.IsDarkMode

          View.Frame (
              backgroundColor = backgroundColor model.IsDarkMode,
              content =
                  View.DatePicker (
                      minimumDate = DateTime.MinValue,
                      maximumDate = DateTime.MaxValue,
                      date = DateTime.Today,
                      format = localeFormat,
                      dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
                      width = 150.0,
                      verticalOptions = LayoutOptions.Fill,
                      textColor = foregroundColor model.IsDarkMode,
                      backgroundColor = backgroundColor model.IsDarkMode,
                      fontSize = fontSize,
                      horizontalOptions = LayoutOptions.CenterAndExpand
                  )
          ) ]

    /// Select a Tzolk’in date.
    let tzolkinSelector model dispatch =
        [ View.Picker (
            title = "Number:",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = int (model.ListTzolkinDate.number) - 1,
            items = numberPickList,
            selectedIndexChanged = (fun (i, item) -> dispatch (SetListNumber <| i + 1)),
            width = 35.0,
            fontSize = fontSize,
            textColor = foregroundColor model.IsDarkMode,
            backgroundColor = backgroundColor model.IsDarkMode,
            horizontalTextAlignment = TextAlignment.End
          )

          View.Picker (
              title = "Glyph:",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = int (model.ListTzolkinDate.glyph) - 1,
              items = glyphPickList,
              fontSize = fontSize,
              textColor = foregroundColor model.IsDarkMode,
              backgroundColor = backgroundColor model.IsDarkMode,
              selectedIndexChanged = (fun (i, item) -> dispatch (SetListGlyph <| i + 1))
          ) ]


    /// Separator line.
    let separator isL =
        match isL with
        | false ->
            View.BoxView (
                color = Color.Black,
                backgroundColor = Color.Black,
                height = 0.5,
                horizontalOptions = LayoutOptions.FillAndExpand
            )

        | true ->
            View.BoxView (
                color = Color.Black,
                backgroundColor = Color.Black,
                width = 0.5,
                verticalOptions = LayoutOptions.FillAndExpand
            )

    /// The Filter section
    let tzolkinFilter (model: Model) dispatch =
        [ View.Picker (
            title = "Day:",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = model.Filter.day,
            items = "" :: [ for i in 1 .. 31 -> i.ToString () ],
            selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterDay <| i)),
            fontSize = fontSize,
            textColor = foregroundColor model.IsDarkMode,
            backgroundColor = backgroundColor model.IsDarkMode,
            width = 35.0,
            ref = dayPicker
          )
          View.Picker (
              title = "Month:",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = model.Filter.month,
              items = "" :: [ for i in 1 .. 12 -> i.ToString () ],
              selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterMonth <| i)),
              fontSize = fontSize,
              textColor = foregroundColor model.IsDarkMode,
              backgroundColor = backgroundColor model.IsDarkMode,
              width = 35.0,
              ref = monthPicker
          )
          View.Entry (
              text = model.Filter.year,
              completed = (fun text -> SetFilterYear text |> dispatch),
              keyboard = Keyboard.Numeric,
              fontSize = fontSize,
              width = 100.0,
              textColor = foregroundColor model.IsDarkMode,
              backgroundColor = backgroundColor model.IsDarkMode,
              ref = yearPicker
          ) ]

    /// The view of MVU.
    let view (model: Model) dispatch =
        View.ContentPage (
            sizeChanged = (fun (width, height) -> dispatch (SetOrientation (width, height))),
            backgroundColor = backgroundColor model.IsDarkMode,
            content =
                View.StackLayout (
                    padding = Thickness 10.0,
                    orientation =
                        (if model.IsLandscape then
                             StackOrientation.Horizontal
                         else
                             StackOrientation.Vertical),
                    backgroundColor = backgroundColor model.IsDarkMode,
                    children =
                        [ View.StackLayout (
                            orientation = StackOrientation.Horizontal,
                            backgroundColor = backgroundColor model.IsDarkMode,
                            children = dateSelector model dispatch
                          )

                          separator model.IsLandscape

                          View.Grid (
                              backgroundColor = backgroundColor model.IsDarkMode,
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
                                  [ (tzolkinDateView model.ListTzolkinDate model.IsDarkMode)
                                      .Row(0)
                                      .Column (1)

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
                                        .Button(text = "Reset",
                                                command = (fun () -> dispatch DoResetFilter))
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
                                                  backgroundColor = backgroundColor model.IsDarkMode,
                                                  horizontalOptions = LayoutOptions.Start)
                                        .Row(0)
                                        .Column(0)
                                        .RowSpan (5)
                                    View
                                        .Label(text = version,
                                               fontSize = FontSize.fromNamedSize NamedSize.Micro,
                                               textColor = foregroundColor model.IsDarkMode,
                                               backgroundColor = backgroundColor model.IsDarkMode,
                                               verticalTextAlignment = TextAlignment.End,
                                               horizontalTextAlignment = TextAlignment.End,
                                               horizontalOptions = LayoutOptions.Fill,
                                               verticalOptions = LayoutOptions.Fill)
                                        .Row(5)
                                        .Column(0)
                                        .ColumnSpan (2)

                                    ]
                          ) ]
                )
        )

    // Note, this declaration is needed if you enable LiveUpdate
#if DEBUG
    let program =
        XamarinFormsProgram.mkProgram init update view
        |> Program.withConsoleTrace
#else
    let program = XamarinFormsProgram.mkProgram init update view
#endif


    type App () as app =
        inherit Application ()

        let themeChangedSub dispatch =
#if DEBUG
            Trace.TraceInformation (sprintf "themeChangedSub %A" Application.Current.RequestedTheme)
#endif

            // Why is this called instead of the handler function?
            dispatch (Msg.SetAppTheme Application.Current.RequestedTheme)

        //Application.Current.RequestedThemeChanged.AddHandler (
        //    EventHandler<AppThemeChangedEventArgs>
        //        (fun _ args -> dispatch (Msg.SetAppTheme args.RequestedTheme))
        //)

        let runner =
            program
            |> Program.withSubscription (fun _ -> Cmd.ofSub themeChangedSub)
            |> XamarinFormsProgram.run app

// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
        let modelId = "model"

        override __.OnSleep() =

            let json = Newtonsoft.Json.JsonConvert.SerializeObject (runner.CurrentModel)

            Console.WriteLine ("OnSleep: saving model into app.Properties, json = {0}", json)

            app.Properties.[modelId] <- json

        override __.OnResume() =
            Console.WriteLine "OnResume: checking for model in app.Properties"

            try
                match app.Properties.TryGetValue modelId with
                | true, (:? string as json) ->

                    Console.WriteLine (
                        "OnResume: restoring model from app.Properties, json = {0}",
                        json
                    )

                    let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model> (json)

                    Console.WriteLine (
                        "OnResume: restoring model from app.Properties, model = {0}",
                        (sprintf "%0A" model)
                    )

                    runner.SetCurrentModel (model, Cmd.none)

                | _ -> ()
            with ex ->
                App.program.onError ("Error while restoring model found in app.Properties", ex)

        override this.OnStart() =
            Console.WriteLine "OnStart: using same logic as OnResume()"
            this.OnResume ()
#endif
