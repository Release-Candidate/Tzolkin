// SPDX-License-Identifier: Apache-2.0
// Copyright 2018 Fabulous contributors.
// Copyright 2021 Roland Csaszar
//
// Project:  Tzolkin.iOS
// File:     AppDelegate.fs
//
//==============================================================================

namespace Tzolkin.iOS

open System
open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

open TzolkinApp



[<Register("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    override this.FinishedLaunching(app, options) =
        Forms.Init ()

        let appcore = new TzolkinApp.App ()
        this.LoadApplication (appcore)
        base.FinishedLaunching (app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main (args, null, "AppDelegate")
        0
