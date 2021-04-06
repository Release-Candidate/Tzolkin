// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Tzolkin
// File:     TestTzolkinGlyph.fs
//
//==============================================================================

module Tests.TestTzolkinGlyph

open Expecto
open Swensen.Unquote

open RC.Maya.TzolkinGlyph

[<Tests>]
let tests =
    testList
        "samples"
        [ testCase "universe exists (╭ರᴥ•́)"
          <| fun _ ->
              let subject = true
              test <@ subject = true @>
          //Expect.isTrue subject "I compute, therefore I am."

          testCase "when true is not (should fail)"
          <| fun _ ->
              let subject = false
              subject =! true
          //Expect.isTrue subject "I should fail because the subject is false"

          testCase "I'm skipped (should skip)"
          <| fun _ -> Tests.skiptest "Yup, waiting for a sunny day..."

          testCase "I'm always fail (should fail)"
          <| fun _ -> Tests.failtest "This was expected..."

          testCase "contains things"
          <| fun _ -> Expect.containsAll [| 2; 3; 4 |] [| 2; 4 |] "This is the case; {2,3,4} contains {2,4}"

          testCase "contains things (should fail)"
          <| fun _ -> Expect.containsAll [| 2; 3; 4 |] [| 2; 4; 1 |] "Expecting we have one (1) in there"

          testCase "Sometimes I want to ༼ノಠل͟ಠ༽ノ ︵ ┻━┻"
          <| fun _ -> Expect.equal "abcdëf" "abcdef" "These should equal"

          (*test "I am (should fail)" {
      "╰〳 ಠ 益 ಠೃ 〵╯" |> Expect.equal true false
    }*)
          ]
