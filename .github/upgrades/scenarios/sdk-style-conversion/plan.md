# Plan: SDK-style Conversion

## Overview
Convert LogixSys.AuthClient project to SDK-style project format without changing target frameworks. Tasks are ordered to minimize dependency breakage.

## Tasks
1. 01-convert-authclient — Convert LogixSys.AuthClient/LogixSys.AuthClient.csproj to SDK-style
   - Scope: single project conversion, migrate PackageReference if packages.config present, remove explicit Compile/Content includes, preserve custom imports
   - Risk: Medium — project may be MAUI or reference MAUI targets; must ensure correct Sdk attribute
   - Done when:
	 - Project file has Sdk attribute and uses SDK-style structure
	 - No packages.config remains in project folder
	 - Solution builds with zero errors and no warnings in projects modified
	 - progress-details.md written


Proceeding to start task 01-convert-authclient.
