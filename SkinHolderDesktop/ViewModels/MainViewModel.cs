using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.Utils;
using SkinHolderDesktop.Views.Partials;
using System.Windows;
using System.Windows.Media;

namespace SkinHolderDesktop.ViewModels;

public class RefreshLastRegistroMessage { }

public partial class MainViewModel : ObservableObject, IRecipient<RefreshLastRegistroMessage>
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
    private readonly IMessenger _messenger;

    private readonly Brush _failBrush = new SolidColorBrush(Colors.DarkRed);
    private readonly Brush _primaryBrush;

    private readonly IServiceProvider _services;

    private readonly IRegistroService _registroService;

    public MainViewModel(GlobalViewModel global, IRegistroService registroService, IServiceProvider services, IMessenger messenger)
    {
        _registroService = registroService;

        _services = services;

        _primaryBrush = (Brush)Application.Current.Resources["PrimaryBrush"]!;

        CurrentContent = _services.GetRequiredService<Bienvenida>();
        _messenger = messenger;
        _messenger.Register(this);

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

    public async Task GetLastRegistroPrecioTotalAsync()
    {
        var lastRegistro = await _registroService.GetLastRegistroAsync();

        SteamLast = lastRegistro.Totalsteam.ToString();
        GamerPayLast = lastRegistro.Totalgamerpay.ToString();
        CsFloatLast = lastRegistro.Totalcsfloat.ToString();
    }

    public void Receive(RefreshLastRegistroMessage message)
    {
        _ = GetLastRegistroPrecioTotalAsync();
    }

    private void CargarVista<T>() where T : class
    {
        if (CurrentContent is T existente)
        {
            (existente as IDisposable)?.Dispose();
            CurrentContent = null;
            CurrentContent = _services.GetRequiredService<Bienvenida>();
            return;
        }

        CurrentContent = _services.GetRequiredService<T>();
    }

    [RelayCommand]
    private void CargarRegistros() => CargarVista<Registros>();

    [RelayCommand]
    private void CargarItems() => CargarVista<UserItems>();

    [RelayCommand]
    private void CargarPerfil() => CargarVista<UserSettings>();

    [RelayCommand]
    private static void Salir() => Application.Current.Shutdown();
}