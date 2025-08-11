using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.Utils;
using SkinHolderDesktop.Views.Partials;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace SkinHolderDesktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string steamPing = "-";
    [ObservableProperty] private string gamerPayPing = "-";
    [ObservableProperty] private string skinHolderDbPing = "-";
    [ObservableProperty] private string estadoSkinHolderDb = "-";
    [ObservableProperty] private string steamLast = "0.00";
    [ObservableProperty] private string gamerPayLast = "0.00";
    [ObservableProperty] private string csFloatLast = "0.00";

    [ObservableProperty] private Brush steamPingBrush = Brushes.White;
    [ObservableProperty] private Brush gamerPayPingBrush = Brushes.White;
    [ObservableProperty] private Brush skinHolderDbPingBrush = Brushes.White;
    [ObservableProperty] private Brush estadoSkinHolderDbBrush = Brushes.White;

    [ObservableProperty] private object? currentContent;

    private readonly GlobalViewModel _global;
    private readonly Brush _failBrush = new SolidColorBrush(Colors.DarkRed);
    private readonly Brush _primaryBrush;

    private readonly IServiceProvider _services;

    private readonly IRegistroService _registroService;

    public MainViewModel(GlobalViewModel global, IRegistroService registroService, IServiceProvider services)
    {
        _global = global;

        _registroService = registroService;

        _services = services;

        _primaryBrush = (Brush)Application.Current.Resources["PrimaryBrush"]!;

        CurrentContent = _services.GetRequiredService<Bienvenida>();

        _ = GetPingsAsync();
        _ = GetLastRegistroPrecioTotalAsync();
    }

    private async Task GetPingsAsync()
    {
        while (true)
        {
            var steam = ConnectionPing.GetPingTime("steamcommunity.com");
            var gamer = ConnectionPing.GetPingTime("gamerpay.gg");
            var db = ConnectionPing.GetPingTime("shapi.jagoba.dev");

            SteamPing = steam.ToString();
            GamerPayPing = gamer.ToString();
            SkinHolderDbPing = db.ToString();
            EstadoSkinHolderDb = db > -1 ? "ACTIVO" : "INACTIVO";

            SteamPingBrush = steam > -1 ? _primaryBrush : _failBrush;
            GamerPayPingBrush = gamer > -1 ? _primaryBrush : _failBrush;
            SkinHolderDbPingBrush = _primaryBrush;
            EstadoSkinHolderDbBrush = db > -1 ? _primaryBrush : _failBrush;

            await Task.Delay(1000);

            SteamPingBrush = Brushes.White;
            GamerPayPingBrush = Brushes.White;
            SkinHolderDbPingBrush = Brushes.White;
            EstadoSkinHolderDbBrush = Brushes.White;

            await Task.Delay(9000);
        }
    }

    private async Task GetLastRegistroPrecioTotalAsync()
    {
        var lastRegistro = await _registroService.GetLastRegistroAsync();
        
        SteamLast = lastRegistro.Totalsteam.ToString();
        GamerPayLast = lastRegistro.Totalgamerpay.ToString();
        CsFloatLast = lastRegistro.Totalcsfloat.ToString();
    }

    [RelayCommand]
    private void CargarRegistros() => CurrentContent = _services.GetRequiredService<Registros>();

    [RelayCommand]
    private void CargarItems() => CurrentContent = _services.GetRequiredService<UserItems>();

    [RelayCommand]
    private void CargarPerfil() => CurrentContent = _services.GetRequiredService<Bienvenida>();

    [RelayCommand]
    private static void Salir() => Application.Current.Shutdown();
}