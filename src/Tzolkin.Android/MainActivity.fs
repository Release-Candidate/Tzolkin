// SPDX-License-Identifier: MIT
// Copyright 2018 Fabulous contributors.
// Copyright 2021 Roland Csaszar
//
// Project:  Tzolkin.Android
// File:     MainActivity.fs
//
//==============================================================================

namespace Tzolkin.Android

open System
open Android.App
open Android.Content.PM
open Android.Runtime
open Android.OS
open Xamarin.Forms.Platform.Android

open TzolkinApp

[<Activity(Label = "Tzolkin",
           Icon = "@mipmap/icon",
           RoundIcon = "@mipmap/icon_round",
           Theme = "@style/MainTheme",
           MainLauncher = true,
           ScreenOrientation = ScreenOrientation.User,
           ConfigurationChanges = (ConfigChanges.ScreenSize
                                   ||| ConfigChanges.Orientation))>]
type MainActivity () =
    inherit FormsAppCompatActivity ()

    override this.OnCreate(bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- Resources.Layout.Toolbar

        base.OnCreate (bundle)
        Xamarin.Essentials.Platform.Init (this, bundle)
        Xamarin.Forms.Forms.Init (this, bundle)
        //x Xamarin.Essentials.VersionTracking.Track ()

        this.LoadApplication (TzolkinApp.App ())

    override this.OnRequestPermissionsResult
        (
            requestCode: int,
            permissions: string [],
            [<GeneratedEnum>] grantResults: Android.Content.PM.Permission []
        ) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult (
            requestCode,
            permissions,
            grantResults
        )

        base.OnRequestPermissionsResult (requestCode, permissions, grantResults)
