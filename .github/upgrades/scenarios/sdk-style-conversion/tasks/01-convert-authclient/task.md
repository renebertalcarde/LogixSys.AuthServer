# 01-convert-authclient: Convert LogixSys.AuthClient/LogixSys.AuthClient.csproj to SDK-style

   - Scope: single project conversion, migrate PackageReference if packages.config present, remove explicit Compile/Content includes, preserve custom imports
   - Risk: Medium — project may be MAUI or reference MAUI targets; must ensure correct Sdk attribute
   - Done when:
	 - Project file has Sdk attribute and uses SDK-style structure
	 - No packages.config remains in project folder
	 - Solution builds with zero errors and no warnings in projects modified
	 - progress-details.md written

