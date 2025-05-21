namespace Order.Aplicacao.Respostas;
public class Paginacao
{
    public Paginacao(int pagina, int totalPorPagina, long total)
    {
        Pagina = pagina;
        TotalPorPagina = totalPorPagina;
        Total = total;
    }

    public int Pagina { get; }

    public int TotalPorPagina { get; }

    public long Total { get; }
}
