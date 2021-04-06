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

open RC.Maya

/// The module holds the IOS and Android app.
module App =

    /// The MVU model.
    type Model =
        { Date: System.DateTime
          ListTzolkinDate: TzolkinDate.T
          FilterTzolkinDate: TzolkinDate.T
          FilterString: string }

    /// MVU messages.
    type Msg =
        | SetDate of System.DateTime
        | SetListNumber of int
        | SetListGlyph of int
        | SetFilterNumber of int
        | SetFilterGlyph of int
        | SetFilterString of string


    /// Initial state of the MVU model.
    let initModel =
        { Date = System.DateTime.Today
          ListTzolkinDate =
              { number = TzolkinNumber.T.TzolkinNumber 8
                glyph = TzolkinGlyph.T.TzolkinGlyph 5 }
          FilterTzolkinDate =
              { number = TzolkinNumber.T.TzolkinNumber 6
                glyph = TzolkinGlyph.T.TzolkinGlyph 15 }
          FilterString = "" }

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
        | SetFilterNumber newNum ->
            { model with
                  FilterTzolkinDate =
                      { model.FilterTzolkinDate with
                            number = (TzolkinNumber.T.TzolkinNumber newNum) } },
            Cmd.none
        | SetFilterGlyph newGlyph ->
            { model with
                  FilterTzolkinDate =
                      { model.FilterTzolkinDate with
                            glyph = (TzolkinGlyph.T.TzolkinGlyph newGlyph) } },
            Cmd.none
        | SetFilterString newStr -> { model with FilterString = newStr }, Cmd.none

    /// Fills the list view with 21 dates that have the same Tzolk’in date.
    let fillListView model =
        let lastList =
            TzolkinDate.getLastList 10 model.FilterTzolkinDate DateTime.Today
            |> List.map (fun t -> t.ToShortDateString())
            |> List.rev

        let nextList =
            TzolkinDate.getNextList 10 model.FilterTzolkinDate DateTime.Today
            |> List.map (fun t -> t.ToShortDateString())

        let strList = lastList @ nextList

        List.map (fun elem -> View.TextCell elem) strList

    let fillListViewFilter model =
        let lastList =
            TzolkinDate.filterDateList
                model.FilterString
                (TzolkinDate.getLastList 500 model.ListTzolkinDate DateTime.Today)
            |> List.rev

        let nextList =
            TzolkinDate.filterDateList
                model.FilterString
                (TzolkinDate.getNextList 500 model.ListTzolkinDate DateTime.Today)

        let strList = lastList @ nextList

        List.map (fun elem -> View.TextCell elem) strList

    /// The view of MVU.
    let view (model: Model) dispatch =
        View.ContentPage(
            content =
                View.StackLayout(
                    padding = Thickness 20.0,
                    verticalOptions = LayoutOptions.Center,
                    children =
                        [ View.Label(
                            text = sprintf "Tzolk’in date: %s" ((TzolkinDate.fromDate model.Date).ToString()),
                            horizontalOptions = LayoutOptions.Center,
                            width = 200.0,
                            horizontalTextAlignment = TextAlignment.Center
                          )
                          View.DatePicker(
                              minimumDate = DateTime.MinValue,
                              maximumDate = DateTime.MaxValue,
                              date = DateTime.Today,
                              format = "dd-MM-yyyy",
                              dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
                              horizontalOptions = LayoutOptions.Center
                          )
                          View.Label(
                              text = sprintf "Tzolk’in date: %s" (model.ListTzolkinDate.ToString()),
                              horizontalOptions = LayoutOptions.Center,
                              width = 200.0,
                              horizontalTextAlignment = TextAlignment.Center
                          )
                          View.Slider(
                              minimumMaximum = (1.0, 13.0),
                              minimumTrackColor = Color.Fuchsia,
                              thumbColor = Color.Fuchsia,
                              value = double (int model.ListTzolkinDate.number),
                              valueChanged =
                                  (fun args ->
                                      SetListNumber(int (args.NewValue + 0.5))
                                      |> dispatch),
                              horizontalOptions = LayoutOptions.FillAndExpand
                          )
                          View.Slider(
                              minimumMaximum = (1.0, 20.0),
                              minimumTrackColor = Color.Aqua,
                              thumbColor = Color.Aqua,
                              value = double (int model.ListTzolkinDate.glyph),
                              valueChanged =
                                  (fun args ->
                                      SetListGlyph(int (args.NewValue + 0.5))
                                      |> dispatch),
                              horizontalOptions = LayoutOptions.FillAndExpand
                          )
                          View.ListView(items = fillListView model)
                          View.Label(
                              text = sprintf "Tzolk’in date: %s" (model.FilterTzolkinDate.ToString()),
                              horizontalOptions = LayoutOptions.Center,
                              width = 200.0,
                              horizontalTextAlignment = TextAlignment.Center
                          )
                          View.Slider(
                              minimumMaximum = (1.0, 13.0),
                              minimumTrackColor = Color.Fuchsia,
                              thumbColor = Color.Fuchsia,
                              value = double (int model.FilterTzolkinDate.number),
                              valueChanged =
                                  (fun args ->
                                      SetFilterNumber(int (args.NewValue + 0.5))
                                      |> dispatch),
                              horizontalOptions = LayoutOptions.FillAndExpand
                          )
                          View.Slider(
                              minimumMaximum = (1.0, 20.0),
                              minimumTrackColor = Color.Aqua,
                              thumbColor = Color.Aqua,
                              value = double (int model.FilterTzolkinDate.glyph),
                              valueChanged =
                                  (fun args ->
                                      SetFilterGlyph(int (args.NewValue + 0.5))
                                      |> dispatch),
                              horizontalOptions = LayoutOptions.FillAndExpand
                          )
                          View.Entry(
                              text = model.FilterString,
                              textChanged = (fun args -> dispatch (SetFilterString args.NewTextValue)),
                              completed = (fun text -> dispatch (SetFilterString text))
                          )
                          View.ListView(items = fillListViewFilter model) ]
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

    let runner =
        App.program |> XamarinFormsProgram.run app

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

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel(model, Cmd.none)

            | _ -> ()
        with ex -> App.program.onError ("Error while restoring model found in app.Properties", ex)

    override this.OnStart() =
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif
