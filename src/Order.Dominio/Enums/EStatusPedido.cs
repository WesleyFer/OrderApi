using System.ComponentModel;

namespace Order.Dominio.Enums;

public enum EStatusPedido
{
    [Description("Pendente")]
    Pendente = 1,

    [Description("Processando")]
    Processando = 2,

    [Description("Enviado")]
    Enviado = 3,

    [Description("Entregue")]
    Entregue = 4,

    [Description("Cancelado")]
    Cancelado = 5 
}