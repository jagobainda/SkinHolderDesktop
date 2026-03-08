using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkinHolderDesktop.Core;
using SkinHolderDesktop.Services;
using SkinHolderDesktop.Utils;
using SkinHolderDesktop.Views.Partials;
using System.Windows;
using System.Windows.Media;

namespace SkinHolderDesktop.ViewModels;

public class RefreshLastRegistroMessage { }

public partial class MainViewModel : ObservableObject, IRecipient<RefreshLastRegistroMessage>, IDisposable
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

    private readonly IRegistroService _registroService;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILoginService _loginService;
    private readonly CancellationTokenSource _cts = new();
    private readonly Func<Bienvenida> _bienvenidaFactory;
    private readonly Func<Registros> _registrosFactory;
    private readonly Func<UserItems> _userItemsFactory;
    private readonly Func<UserSettings> _userSettingsFactory;

    public MainViewModel(ITokenProvider tokenProvider, ILoginService loginService, IRegistroService registroService, IMessenger messenger,
        Func<Bienvenida> bienvenidaFactory, Func<Registros> registrosFactory, Func<UserItems> userItemsFactory, Func<UserSettings> userSettingsFactory)
    {
        _registroService = registroService;
        _tokenProvider = tokenProvider;
        _loginService = loginService;
        _bienvenidaFactory = bienvenidaFactory;
        _registrosFactory = registrosFactory;
        _userItemsFactory = userItemsFactory;
        _userSettingsFactory = userSettingsFactory;

        _primaryBrush = (Brush)Application.Current.Resources["PrimaryBrush"]!;

        CurrentContent = _bienvenidaFactory();
        _messenger = messenger;
        _messenger.Register(this);
    }

    public async Task InitializeAsync()
    {
        _ = Task.Run(() => GetPingsAsync(_cts.Token), _cts.Token);
        _ = Task.Run(() => ValidateSessionTokenAsync(_cts.Token), _cts.Token);
        await GetLastRegistroPrecioTotalAsync();
    }

    private async Task GetPingsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
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

            await Task.Delay(1000, cancellationToken);

            SteamPingBrush = Brushes.White;
            GamerPayPingBrush = Brushes.White;
            SkinHolderDbPingBrush = Brushes.White;
            EstadoSkinHolderDbBrush = Brushes.White;

            await Task.Delay(9000, cancellationToken);
        }
    }

    private async Task ValidateSessionTokenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var db = ConnectionPing.GetPingTime("shapi.jagoba.dev");

            if (db > 0 && _tokenProvider.Token is not null)
            {
                var isValid = await _loginService.ValidateToken(_tokenProvider.Token);
                if (!isValid && !Application.Current.Dispatcher.HasShutdownStarted)
                {
                    ProcessUtils.SaveErrorMessageToFile("Sesión expirada");
                    ProcessUtils.RestartApplication();
                }
            }

            await Task.Delay(6000000, cancellationToken);
        }
    }

    private async Task GetLastRegistroPrecioTotalAsync()
    {
        var lastRegistro = await _registroService.GetLastRegistroAsync();

        SteamLast = lastRegistro.Totalsteam.ToString();
        GamerPayLast = lastRegistro.Totalgamerpay.ToString();
        CsFloatLast = lastRegistro.Totalcsfloat.ToString();
    }

    public void Receive(RefreshLastRegistroMessage message)
    {
        _ = Task.Run(GetLastRegistroPrecioTotalAsync, _cts.Token);
    }

    private void CargarVista<T>(Func<T> factory) where T : class
    {
        if (CurrentContent is T existente)
        {
            (existente as IDisposable)?.Dispose();
            CurrentContent = null;
            CurrentContent = _bienvenidaFactory();
            return;
        }

        CurrentContent = factory();
    }

    [RelayCommand]
    private void CargarRegistros() => CargarVista(_registrosFactory);

    [RelayCommand]
    private void CargarItems() => CargarVista(_userItemsFactory);

    [RelayCommand]
    private void CargarPerfil() => CargarVista(_userSettingsFactory);

    [RelayCommand]
    private static void Salir() => Application.Current.Shutdown();

    public void Dispose()
    {
        _messenger.UnregisterAll(this);
        _cts.Cancel();
        _cts.Dispose();
    }
}