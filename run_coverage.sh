#!/bin/bash
# SPDX-License-Identifier: MIT
# Copyright (C) 2021 Roland Csaszar
#
# Project:  Tzolkin
# File:     run_coverage.sh
# Date:     27.Apr.2021
###############################################################################

dotnet test -v n /p:AltCover=true /p:AltCoverReportFormat=OpenCover /p:AltCoverReport=./coverage.xml
