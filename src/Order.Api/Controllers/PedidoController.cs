using Order.Api.Extensions;
using Order.Aplicacao.Comandos.Pedido;
using Order.Aplicacao.Dtos;
using Order.Aplicacao.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidoController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="mediator"></param>
        public PedidoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtem os pedidos
        /// </summary>
        /// <param name="termo"></param>
        /// <param name="paginacao"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync(
            [FromQuery] string? termo, 
            [FromQuery] PaginacaoRequest paginacao, 
            CancellationToken cancellationToken)
        {
            var resultado = await PedidoQuery.BuscarAsync(_mediator, termo, paginacao, cancellationToken);
            return resultado.ToAction();
        }

        /// <summary>
        /// Criar pedido
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync(
            [FromBody] PedidoRequest request, 
            CancellationToken cancellationToken)
        {
            var resultado = await CriarPedidoComando.CriarComandoAsync(_mediator, request, cancellationToken);
            return resultado.ToAction();
        }

        /// <summary>
        /// Atualizar pedido
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] Guid id, 
            [FromBody] AtualizarPedidoRequest request, 
            CancellationToken cancellationToken)
        {
            var resultado = await AtualizarPedidoComando.CriarComandoAsync(_mediator, id, request, cancellationToken);
            return resultado.ToAction();
        }

        /// <summary>
        /// Excluir pedido
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var resultado = await ExcluirPedidoComando.CriarComandoAsync(_mediator, id, cancellationToken);

            return resultado.ToAction();
        }

        [HttpPost("enviar-pedidos-rabbitmq")]
        public async Task<IActionResult> EnviarPedidos([FromBody] List<PedidoRequest> pedidos, CancellationToken cancellationToken)
        {
            var resultado = await EnviarPedidoComando.CriarComandoAsync(_mediator, pedidos, cancellationToken);
            return resultado.ToAction();
        }
    }
}
