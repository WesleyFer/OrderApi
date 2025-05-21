using Order.Aplicacao.Respostas;
using Microsoft.AspNetCore.Mvc;

namespace Order.Api.Extensions;

public static class RespostaExtensao
{
    public static IActionResult ToAction(this Resposta resposta)
    {
        if (resposta is null)
        {
            return new NotFoundResult();
        }

        var response = new
        {
            sucesso = resposta.Sucesso,
            errors = resposta.Sucesso ? null : new { mensagem = "Erro desconhecido" }
        };

        return resposta.Sucesso
            ? new OkObjectResult(response)
            : new BadRequestObjectResult(response);
    }

    public static IActionResult ToAction<TRetorno>(this Resposta<TRetorno> resposta)
    {
        if (resposta is null)
        {
            return new NotFoundResult();
        }

        var response = new
        {
            sucesso = resposta.Sucesso,
            retorno = resposta.Retorno,
            paginacao = resposta.Paginacao,
            errors = resposta.Sucesso ? null : resposta.Errors.Select(c => c.Message)
        };

        return resposta.Sucesso
            ? (resposta.Retorno is not null ? new OkObjectResult(response) : new NoContentResult())
            : new BadRequestObjectResult(response);
    }

    public static IActionResult FirstOrDefaultToAction<TRetorno>(this Resposta<IEnumerable<TRetorno>> resposta)
    {
        if (resposta?.Retorno == null || !resposta.Retorno.Any())
        {
            return new NotFoundResult();
        }

        var response = new
        {
            sucesso = true,
            retorno = resposta.Retorno.FirstOrDefault(),
            paginacao = new { }
        };

        return new OkObjectResult(response);
    }
}
