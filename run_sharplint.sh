#!/bin/sh
# SPDX-License-Identifier: MIT
# Copyright (C) 2021 Roland Csaszar
#
# Project:  FSHARP_TEMPLATE
# File:     run_sharplint.sh
#
################################################################################

# install dotnet tool install -g dotnet-fsharplint

dotnet fsharplint lint FsharpTemplate.sln