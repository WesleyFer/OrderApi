using Order.Aplicacao.Respostas;
using MediatR;

namespace Order.Aplicacao.Contratos;

public interface IComando : IRequest<bool> { }

public interface IComando<TResultado> : IRequest<Resposta<TResultado>> { }