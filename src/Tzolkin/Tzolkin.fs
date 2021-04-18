// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     Tzolkin.fs
//
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace TzolkinApp

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms


/// The module holds the IOS and Android app.
module TzolkinApp =


    // Main App ====================================================================================

#if DEBUG
    let program =
        XamarinFormsProgram.mkProgram init update View.view
        |> Program.withConsoleTrace
#else
    let program = XamarinFormsProgram.mkProgram init update View.view
#endif


    type App () as app =
        inherit Application ()

        let themeChangedSub dispatch =
#if DEBUG
            Trace.TraceInformation (sprintf "themeChangedSub %A" Application.Current.RequestedTheme)
#endif

            // Why is this called instead of the handler function?
            dispatch (Msg.SetAppTheme Application.Current.RequestedTheme)

            //Application.Current.RequestedThemeChanged.Add (fun args ->
            //    dispatch (Msg.SetAppTheme args.RequestedTheme))

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
