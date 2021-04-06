// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TestMain.fs
//
//==============================================================================

/// Test module containing the main entry point for the `standalone` tests, when
/// called by `dotnet run` instead of `dotnet test`.
module Tests.TestMain

open Expecto

/// Main entry point of the tests, if called by `dotnet run` instead of
/// `dotnet test`.
[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
