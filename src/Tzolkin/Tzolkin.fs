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
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open System.Globalization

open RC.Maya


/// The module holds the IOS and Android app.
module TzolkinApp =

    /// The MVU model.
    type Model =
        { Date: System.DateTime
          ListTzolkinDate: TzolkinDate.T
          FilterString: string
          DateList: ViewElement list }

    /// MVU messages.
    type Msg =
        | SetDate of System.DateTime
        | SetListNumber of int
        | SetListGlyph of int
        | SetFilterString of string

    /// Fills the list view with filtered dates.
    let fillListViewFilter filterString model =
        let lastList =
            TzolkinDate.filterDateList filterString (TzolkinDate.getLastList 500 model.ListTzolkinDate DateTime.Today)
            |> List.rev

        let nextList =
            TzolkinDate.filterDateList filterString (TzolkinDate.getNextList 500 model.ListTzolkinDate DateTime.Today)

        let strList = lastList @ nextList

        List.map (fun elem -> View.TextCell elem) strList


    /// Initial state of the MVU model.
    let initModel =
        { Date = System.DateTime.Today
          ListTzolkinDate =
              { number = TzolkinNumber.T.TzolkinNumber 8
                glyph = TzolkinGlyph.T.TzolkinGlyph 5 }
          FilterString = ""
          DateList = [] }



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
                            number = (TzolkinNumber.T.TzolkinNumber newNum) }
                  DateList = fillListViewFilter model.FilterString model },
            Cmd.none
        | SetListGlyph newGlyph ->
            { model with
                  ListTzolkinDate =
                      { model.ListTzolkinDate with
                            glyph = (TzolkinGlyph.T.TzolkinGlyph newGlyph) }

                  DateList = fillListViewFilter model.FilterString model },
            Cmd.none
        | SetFilterString newStr ->
            { model with
                  FilterString = newStr
                  DateList = fillListViewFilter newStr model },
            Cmd.none

    /// Fills the list view with 21 dates that have the same Tzolk’in date.
    let fillListView model =
        let lastList =
            TzolkinDate.getLastList 10 model.ListTzolkinDate DateTime.Today
            |> List.map (fun t -> t.ToShortDateString())
            |> List.rev

        let nextList =
            TzolkinDate.getNextList 10 model.ListTzolkinDate DateTime.Today
            |> List.map (fun t -> t.ToShortDateString())

        let strList = lastList @ nextList



        List.map (fun elem -> View.TextCell elem) strList



    let numberPickList =
        List.map (fun x -> x.ToString()) [ 1 .. 13 ]

    let glyphPickList = Array.toList TzolkinGlyph.glyphNames

    let localeSeparator =
        CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

    let localeFormat =
        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

    let dateSelector model dispatch =
        [ View.Label(text = sprintf "Tzolk’in date: %s" ((TzolkinDate.fromDate model.Date).ToString()))

          View.DatePicker(
              minimumDate = DateTime.MinValue,
              maximumDate = DateTime.MaxValue,
              date = DateTime.Today,
              format = localeFormat,
              dateSelected = (fun args -> SetDate args.NewDate |> dispatch)
          ) ]

    /// The view of MVU.
    let view (model: Model) dispatch =
        View.ContentPage(
            content =
                View.StackLayout(
                    padding = Thickness 10.0,
                    children =
                        [ View.Frame(
                            hasShadow = true,
                            content =
                                View.StackLayout(
                                    orientation = StackOrientation.Horizontal,
                                    children = dateSelector model dispatch
                                )
                          )
                          View.Label(
                              text = sprintf "Tzolk’in date: %s" (model.ListTzolkinDate.ToString()),
                              horizontalOptions = LayoutOptions.Center,
                              width = 200.0,
                              horizontalTextAlignment = TextAlignment.Center
                          )
                          View.Picker(
                              title = "Number:",
                              horizontalOptions = LayoutOptions.Start,
                              selectedIndex = int (model.ListTzolkinDate.number) - 1,
                              items = numberPickList,
                              selectedIndexChanged = (fun (i, item) -> dispatch (SetListNumber <| i + 1))
                          )
                          View.Picker(
                              title = "Glyph:",
                              horizontalOptions = LayoutOptions.Start,
                              selectedIndex = int (model.ListTzolkinDate.glyph) - 1,
                              items = glyphPickList,
                              selectedIndexChanged = (fun (i, item) -> dispatch (SetListGlyph <| i + 1))
                          )
                          //View.Label(
                          //    text = sprintf "Tzolk’in date: %s" (model.ListTzolkinDate.ToString()),
                          //    horizontalOptions = LayoutOptions.Center,
                          //    width = 200.0,
                          //    horizontalTextAlignment = TextAlignment.Center
                          //)
                          //View.Entry(
                          //    text = model.FilterString,
                          //    textChanged = (fun args -> dispatch (SetFilterString args.NewTextValue)),
                          //    completed = (fun text -> dispatch (SetFilterString text))
                          //)
                          View.SearchBar(
                              placeholder = sprintf "Filter the dates, like with %i%s" model.Date.Month localeSeparator,
                              textChanged = (fun text -> dispatch (SetFilterString text.NewTextValue)),
                              keyboard = Keyboard.Url
                          )
                          View.ListView(
                              items = model.DateList,
                              selectedItem =
                                  if List.length model.DateList > 0 then
                                      Some(List.length model.DateList / 2)
                                  else
                                      None
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

#if DEBUG
// Uncomment this line to enable live update in debug mode.
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
//
//do runner.EnableLiveUpdate()
#endif

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
