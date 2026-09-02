using Microsoft.Maui.Controls;
using LogixSys.AuthClient.Services;

namespace LogixSys.AuthClient;

public partial class MainPage : ContentPage
{
    readonly IAuthenticationService _authService;

    public MainPage(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    async void OnLoginClicked(object sender, EventArgs e)
    {
        var result = await _authService.LoginAsync();
        if (result.IsError)
        {
            StatusLabel.Text = $"Login failed: {result.Error}";
        }
        else
        {
            StatusLabel.Text = $"Hello {result.UserName}";
        }
    }

    async void OnLogoutClicked(object sender, EventArgs e)
    {
        await _authService.LogoutAsync();
        StatusLabel.Text = "Not authenticated";
    }
}
