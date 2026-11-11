using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pup.Application.Abstracoes;

/// <summary>Dispatcher simples (sem MediatR): resolve o handler pelo tipo do comando/consulta.</summary>
internal sealed class Dispatcher(IServiceProvider provider) : IDispatcher
{
    private static readonly ConcurrentDictionary<(Type, Type, bool), (Type HandlerType, MethodInfo Metodo)> Cache = new();

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default) =>
        Invocar<TResult>(command, typeof(ICommandHandler<,>), true, ct);

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default) =>
        Invocar<TResult>(query, typeof(IQueryHandler<,>), false, ct);

    private Task<TResult> Invocar<TResult>(object mensagem, Type handlerAberto, bool comando, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(mensagem);

        var (handlerType, metodo) = Cache.GetOrAdd((mensagem.GetType(), typeof(TResult), comando), chave =>
        {
            var tipo = handlerAberto.MakeGenericType(chave.Item1, chave.Item2);
            return (tipo, tipo.GetMethod("HandleAsync")!);
        });

        var handler = provider.GetRequiredService(handlerType);
        try
        {
            return (Task<TResult>)metodo.Invoke(handler, [mensagem, ct])!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }
}
