// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  FSHARP_TEMPLATE
// File:     Main.fs
//
//==============================================================================

module Main

open Expecto


/// <summary>Hugo</summary>
[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
