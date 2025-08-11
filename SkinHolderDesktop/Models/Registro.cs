namespace SkinHolderDesktop.Models;

public class Registro
{
    public long Registroid { get; set; } = 0;

    public DateTime Fechahora { get; set; } = DateTime.MinValue;

    public decimal Totalsteam { get; set; } = 0;

    public decimal Totalgamerpay { get; set; } = 0;

    public decimal Totalcsfloat { get; set; } = 0;

    public int Userid { get; set; } = 0;
}
