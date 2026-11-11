using Pup.Domain.Comum;

namespace Pup.Domain.Intervencoes;

/// <summary>
/// Obra ou serviço de manutenção (estradas, iluminação, poda...). Os dois têm os mesmos campos
/// no relatório (nome, local, descrição, situação, atualização e previsão), por isso uma entidade só.
/// </summary>
public sealed class Intervencao : Entidade
{
    private readonly List<AtualizacaoIntervencao> _historico = [];

    public TipoIntervencao Tipo { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public Guid RegiaoId { get; private set; }
    /// <summary>Estrada, rua ou ponto de referência dentro da região.</summary>
    public string? Local { get; private set; }
    public StatusIntervencao Status { get; private set; } = StatusIntervencao.Programado;
    public DateOnly? DataInicio { get; private set; }
    public DateOnly? PrevisaoConclusao { get; private set; }
    public DateOnly? DataConclusao { get; private set; }
    /// <summary>Interdição, meia pista, desvio... (o que o morador precisa saber para se preparar).</summary>
    public string? ImpactoNaVia { get; private set; }
    public string? FonteOficialUrl { get; private set; }

    public IReadOnlyList<AtualizacaoIntervencao> Historico => _historico;

    private Intervencao() { }

    public static Intervencao Criar(TipoIntervencao tipo, string titulo, string descricao, Guid regiaoId, string? local,
        StatusIntervencao status, DateOnly? dataInicio, DateOnly? previsaoConclusao, string? impactoNaVia, string? fonteOficialUrl)
    {
        Exigir.Definido(tipo, "tipo");
        Exigir.Definido(status, "situação");
        if (status is StatusIntervencao.Concluido or StatusIntervencao.Cancelado)
            throw new DomainException("Cadastre a obra/manutenção como programada ou em andamento e depois atualize a situação.");

        var i = new Intervencao { Tipo = tipo, Status = status };
        i.Preencher(titulo, descricao, regiaoId, local, dataInicio, previsaoConclusao, impactoNaVia, fonteOficialUrl);
        i._historico.Add(new AtualizacaoIntervencao(i.Id, status, "Cadastrado no portal."));
        return i;
    }

    public void AtualizarDados(string titulo, string descricao, Guid regiaoId, string? local,
        DateOnly? dataInicio, DateOnly? previsaoConclusao, string? impactoNaVia, string? fonteOficialUrl)
    {
        Preencher(titulo, descricao, regiaoId, local, dataInicio, previsaoConclusao, impactoNaVia, fonteOficialUrl);
        Tocar();
    }

    /// <summary>Muda a situação e registra no histórico o que aconteceu (sempre com uma nota para o morador).</summary>
    public AtualizacaoIntervencao MudarStatus(StatusIntervencao novo, string nota, DateOnly hoje)
    {
        Exigir.Definido(novo, "situação");
        var texto = Exigir.Texto(nota, "uma nota explicando a atualização", 500);
        if (novo == Status && Status is StatusIntervencao.Concluido or StatusIntervencao.Cancelado)
            throw new DomainException("Este registro já está encerrado.");

        Status = novo;
        if (novo == StatusIntervencao.EmAndamento && DataInicio is null) DataInicio = hoje;
        DataConclusao = novo == StatusIntervencao.Concluido ? hoje : null;

        var atualizacao = new AtualizacaoIntervencao(Id, novo, texto);
        _historico.Add(atualizacao);
        Tocar();
        return atualizacao;
    }

    /// <summary>Previsão vencida e ainda não concluída: a tela destaca como "previsão vencida".</summary>
    public bool Atrasada(DateOnly hoje) =>
        PrevisaoConclusao is { } p && p < hoje &&
        Status is StatusIntervencao.Programado or StatusIntervencao.EmAndamento or StatusIntervencao.TemporariamenteAlterado;

    private void Preencher(string titulo, string descricao, Guid regiaoId, string? local,
        DateOnly? dataInicio, DateOnly? previsaoConclusao, string? impactoNaVia, string? fonteOficialUrl)
    {
        Exigir.Id(regiaoId, "a região");
        if (dataInicio is { } ini && previsaoConclusao is { } fim && fim < ini)
            throw new DomainException("A previsão de conclusão não pode ser antes do início.");

        Titulo = Exigir.Texto(titulo, "o título", 200);
        Descricao = Exigir.Texto(descricao, "a descrição", 2000);
        RegiaoId = regiaoId;
        Local = Exigir.TextoOpcional(local, "o local", 300);
        DataInicio = dataInicio;
        PrevisaoConclusao = previsaoConclusao;
        ImpactoNaVia = Exigir.TextoOpcional(impactoNaVia, "o impacto na via", 300);
        FonteOficialUrl = Exigir.Url(fonteOficialUrl);
    }
}
