Windows loopback authentication (MAUI)

Overview
- The MAUI Windows build uses the system browser + loopback redirect pattern to perform OAuth2 Authorization Code + PKCE.
- The app listens on a loopback HTTP endpoint (default: http://127.0.0.1:7890/) to capture the authorization code and then exchanges it for tokens.

What you need to know
1) Redirect URI must be registered on the Identity Server
   - The API seeder was updated to register the following redirect/post-logout URIs for client id `maui-client`:
	 - http://127.0.0.1:7890/callback
	 - http://127.0.0.1:7890/signout-callback
   - If you change the port in the app (AuthenticationService.cs), update the server-seeded URIs to match.

2) Development vs Packaged app
   - During development (F5 / running unpackaged from Visual Studio) the loopback listener works without extra steps.
   - If you package the app (MSIX / store) the packaged app runs in an app container and loopback to localhost may be blocked.
	 - Option A (recommended for testing packaged builds): add a loopback exemption with the CheckNetIsolation tool:
	   CheckNetIsolation LoopbackExempt -a -n="<PackageFamilyName>"
	   Example: CheckNetIsolation LoopbackExempt -a -n="LogixSys.MobileApp_abcdefgh!App"
	   - To get the package family name use `Get-AppxPackage -Name "LogixSys.MobileApp"` in PowerShell, or check the Package.appxmanifest after packaging.
	 - Option B: use a custom URI scheme (io.identitymodel.native://callback) — this works for packaged apps if the scheme is registered in the package manifest and handled by the app.

3) Firewall / port conflicts
   - Ensure the chosen loopback port (default 7890) is not in use by another process.
   - Windows Firewall generally allows loopback on localhost; if you have strict rules, open the port for local traffic.

4) TLS / localhost certificate
   - The authorization endpoint used by the IdentityServer is HTTPS (https://localhost:7128). Ensure the development certificate is trusted on the Windows machine (dotnet dev-certs https --trust) so the browser won't block the request.

How to change the port
- Edit LogixSys.MobileApp/Services/AuthenticationService.cs and update the RedirectUri/PostLogoutRedirect and AuthenticateWithLoopbackAsync prefix port (default 7890) to your desired port.
- Update the server redirect URIs in LogixSys.AuthServer.Api/Data/OpenIddictSeeder.cs accordingly (or update the application record in the DB).

Troubleshooting
- Browser opens but the app times out waiting for the code:
  - Confirm the browser was redirected to http://127.0.0.1:7890/callback and that the query string contains `code=`.
  - Confirm listener port matches redirect URI and is not blocked.
- Token exchange fails with invalid_grant:
  - Verify the `redirect_uri` you POST in the token exchange exactly matches the redirect URI registered and used for the authorization request.
  - Verify the code_verifier used when exchanging matches the originally generated code_verifier.

Questions
- Do you want me to add a small README entry into the solution root linking to this file and briefly describing the Windows flow?