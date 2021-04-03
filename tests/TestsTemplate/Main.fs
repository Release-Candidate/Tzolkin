// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     Main.fs
//
//==============================================================================

module Main

open Expecto


/// <summary>Hugo</summary>
[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
