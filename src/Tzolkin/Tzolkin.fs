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
open System.Globalization

open RC.Maya


/// The module holds the IOS and Android app.
module TzolkinApp =


    /// App-wide constants.
    let fontSize = FontSize.fromValue 16.0

    let numberPickList =
        List.map (fun x -> x.ToString()) [ 1 .. 13 ]

    let glyphPickList = Array.toList TzolkinGlyph.glyphNames

    let localeSeparator =
        CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

    let localeFormat =
        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

    /// Record to hold the data needed to filter dates.
    type DateFilter = { day: int; month: int; year: string }

    /// The MVU model.
    type Model =
        { Date: System.DateTime
          ListTzolkinDate: TzolkinDate.T
          Filter: DateFilter }

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

    /// Instances of widgets needed to interact with.
    let dateListView = ViewRef<CustomListView>()
    let dayPicker = ViewRef<Xamarin.Forms.Picker>()
    let monthPicker = ViewRef<Xamarin.Forms.Picker>()
    let yearPicker = ViewRef<Xamarin.Forms.Entry>()

    let cmdScrollToCenter = ScrollListCenter |> Cmd.ofMsg

    let scrollToCenter model =
        match dateListView.TryValue with
        | None -> ()
        | Some listView ->

            let centerItem =
                List.ofSeq (Seq.cast<ViewElement> listView.ItemsSource)

            listView.ScrollTo(item = centerItem.Tail, position = ScrollToPosition.End, animated = true)

    /// Fills the list view with filtered dates.
    let fillListViewFilter (model: Model) =
        let lastList =
            TzolkinDate.getLastList 500 model.ListTzolkinDate DateTime.Today
            |> List.rev

        let nextList =
            TzolkinDate.getNextList 500 model.ListTzolkinDate DateTime.Today

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
                        let yearStr = elem.Year.ToString()
                        yearStr.Contains(year))
                    dateList

        dateList
        |> filterDay
        |> filterMonth
        |> filterYear
        |> List.map (fun elem -> View.TextCell(elem.ToShortDateString()))

    /// Initial state of the MVU model.
    let initModel =
        { Date = System.DateTime.Today
          ListTzolkinDate =
              { number = TzolkinNumber.T.TzolkinNumber 8
                glyph = TzolkinGlyph.T.TzolkinGlyph 5 }
          Filter = { day = 0; month = 0; year = "" } }

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

    /// Select the Gregorian date and display the Tzolk’in date.
    let dateSelector model dispatch =
        [ View.Label(
            text = sprintf "Tzolk’in date:\n%s" ((TzolkinDate.fromDate model.Date).ToString()),
            horizontalTextAlignment = TextAlignment.Center,
            fontSize = fontSize,
            textColor = Color.Black
          )

          View.DatePicker(
              minimumDate = DateTime.MinValue,
              maximumDate = DateTime.MaxValue,
              date = DateTime.Today,
              format = localeFormat,
              dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
              width = 150.0,
              fontSize = fontSize
          ) ]

    /// Select a Tzolk’in date.
    let tzolkinSelector model dispatch =
        [ View.Picker(
            title = "Number:",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = int (model.ListTzolkinDate.number) - 1,
            items = numberPickList,
            selectedIndexChanged = (fun (i, item) -> dispatch (SetListNumber <| i + 1)),
            width = 35.0,
            fontSize = fontSize,
            horizontalTextAlignment = TextAlignment.End
          )

          View.Picker(
              title = "Glyph:",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = int (model.ListTzolkinDate.glyph) - 1,
              items = glyphPickList,
              fontSize = fontSize,
              selectedIndexChanged = (fun (i, item) -> dispatch (SetListGlyph <| i + 1))
          ) ]

    /// The Filter section
    let tzolkinFilter (model: Model) dispatch =
        [ View.Picker(
            title = "Day:",
            horizontalOptions = LayoutOptions.Start,
            selectedIndex = model.Filter.day,
            items = "" :: [ for i in 1 .. 31 -> i.ToString() ],
            selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterDay <| i)),
            fontSize = fontSize,
            width = 35.0,
            ref = dayPicker
          )
          View.Picker(
              title = "Month:",
              horizontalOptions = LayoutOptions.Start,
              selectedIndex = model.Filter.month,
              items = "" :: [ for i in 1 .. 12 -> i.ToString() ],
              selectedIndexChanged = (fun (i, item) -> dispatch (SetFilterMonth <| i)),
              fontSize = fontSize,
              width = 35.0,
              ref = monthPicker
          )
          View.Entry(
              text = model.Filter.year,
              completed = (fun text -> SetFilterYear(text) |> dispatch),
              keyboard = Keyboard.Numeric,
              fontSize = fontSize,
              width = 100.0,
              ref = yearPicker
          ) ]

    /// The view of MVU.
    let view (model: Model) dispatch =
        View.ContentPage(
            content =
                View.FlexLayout(
                    padding = Thickness 10.0,
                    justifyContent = FlexJustify.SpaceBetween,
                    alignItems = FlexAlignItems.Start,
                    wrap = FlexWrap.Wrap,
                    direction = FlexDirection.Row,
                    children =
                        [ View.Frame(
                            hasShadow = true,
                            content =
                                View.StackLayout(
                                    orientation = StackOrientation.Horizontal,
                                    children = dateSelector model dispatch
                                )
                          )
                          View.Grid(
                              verticalOptions = LayoutOptions.FillAndExpand,
                              rowdefs =
                                  [ Dimension.Auto
                                    Dimension.Auto
                                    Dimension.Auto
                                    Dimension.Auto
                                    Dimension.Auto
                                    Dimension.Auto ],
                              coldefs =
                                  [ Dimension.Stars 0.1
                                    Dimension.Stars 0.9 ],
                              children =
                                  [ View
                                      .StackLayout(children = tzolkinSelector model dispatch,
                                                   orientation = StackOrientation.Horizontal)

                                        .Row(
                                            1
                                        )
                                        .Column(2)

                                    View
                                        .StackLayout(orientation = StackOrientation.Horizontal,
                                                     children = tzolkinFilter model dispatch)
                                        .Row(2)
                                        .Column(2)


                                    View
                                        .Button(text = "Reset", command = (fun () -> dispatch DoResetFilter))
                                        .Row(5)
                                        .Column(2)
                                    View
                                        .ListView(ref = dateListView,
                                                  items = fillListViewFilter model,
                                                  horizontalOptions = LayoutOptions.Start)
                                        .Row(1)
                                        .Column(1)
                                        .RowSpan(6) ]

                          ) ]
                )
        )



    // Note, this declaration is needed if you enable LiveUpdate
    let program =
        XamarinFormsProgram.mkProgram init update view
#if DEBUG
        |> Program.withConsoleTrace
#endif


    type App() as app =
        inherit Application()

        let runner = program |> XamarinFormsProgram.run app


// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
        let modelId = "model"

        override __.OnSleep() =

            let json =
                Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)

            Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

            app.Properties.[modelId] <- json

        override __.OnResume() =
            Console.WriteLine "OnResume: checking for model in app.Properties"

            try
                match app.Properties.TryGetValue modelId with
                | true, (:? string as json) ->

                    Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)

                    let model =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                    Console.WriteLine(
                        "OnResume: restoring model from app.Properties, model = {0}",
                        (sprintf "%0A" model)
                    )

                    runner.SetCurrentModel(model, Cmd.none)

                | _ -> ()
            with ex -> App.program.onError ("Error while restoring model found in app.Properties", ex)

        override this.OnStart() =
            Console.WriteLine "OnStart: using same logic as OnResume()"
            this.OnResume()
#endif
