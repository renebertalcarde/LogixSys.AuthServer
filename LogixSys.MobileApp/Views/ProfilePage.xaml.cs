using Microsoft.Maui.Controls;
using LogixSys.MobileApp.Services;

namespace LogixSys.MobileApp.Views;

public partial class ProfilePage : ContentPage
{
    private IAuthenticationService? _auth;

    public ProfilePage()
    {
        InitializeComponent();
        LogoutButton.Clicked += LogoutButton_Clicked;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_auth == null)
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _auth = mauiContext?.Services.GetService(typeof(IAuthenticationService)) as IAuthenticationService;
        }

        if (!await _auth!.IsLoggedInAsync())
        {
            try
            {
                await _auth.LoginAsync();
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Login failed: " + ex.Message;
                return;
            }
        }

        var profile = await _auth.GetProfileAsync();
        if (profile is null)
        {
            StatusLabel.Text = "No profile available";
            return;
        }

        StatusLabel.Text = "Signed in";
        ClaimsList.ItemsSource = profile.Select(kv => new { Key = kv.Key, Value = kv.Value }).ToList();
    }

    private async void LogoutButton_Clicked(object? sender, EventArgs e)
    {
        if (_auth != null)
            await _auth.LogoutAsync();
        StatusLabel.Text = "Signed out";
        ClaimsList.ItemsSource = null;
    }
}
