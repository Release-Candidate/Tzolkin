#!/bin/sh
# SPDX-License-Identifier: MIT
# Copyright (C) 2021 Roland Csaszar
#
# Project:  Tzolkin
# File:     run_sharplint.sh
#
################################################################################

# install dotnet tool install -g dotnet-fsharplint

dotnet fsharplint lint FSHARP_TEMPLATE.sln