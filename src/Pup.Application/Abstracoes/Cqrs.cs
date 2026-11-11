namespace Pup.Application.Abstracoes;

/// <summary>Comando (altera estado).</summary>
public interface ICommand<TResult>;

/// <summary>Consulta (somente leitura).</summary>
public interface IQuery<TResult>;

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct);
}

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct);
}

/// <summary>Resultado vazio para comandos que não retornam nada.</summary>
public readonly record struct Nada
{
    public static readonly Nada Valor = new();
    public static Task<Nada> Concluida { get; } = System.Threading.Tasks.Task.FromResult(Valor);
}

/// <summary>Ponto único usado pelos Controllers para executar slices.</summary>
public interface IDispatcher
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default);
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default);
}
