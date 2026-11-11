using Pup.Domain.Comum;

namespace Pup.Domain.Coleta;

/// <summary>
/// Calendário de coleta (comum ou seletiva) de uma região: dias, horário e eventuais
/// alterações temporárias (feriado, chuva, troca de empresa...).
/// </summary>
public sealed class ColetaResiduos : Entidade
{
    public Guid RegiaoId { get; private set; }
    public ModalidadeColeta Modalidade { get; private set; }
    public DiasSemana Dias { get; private set; }
    public TimeOnly HorarioInicio { get; private set; }
    public TimeOnly HorarioFim { get; private set; }
    public string? Observacao { get; private set; }
    public SituacaoColeta Situacao { get; private set; } = SituacaoColeta.Regular;
    public string? AvisoAlteracao { get; private set; }
    /// <summary>Último dia em que a alteração vale. Depois disso a coleta volta a ser exibida como regular.</summary>
    public DateOnly? AlteracaoAte { get; private set; }
    public string? FonteOficialUrl { get; private set; }

    private ColetaResiduos() { }

    public static ColetaResiduos Criar(Guid regiaoId, ModalidadeColeta modalidade, DiasSemana dias,
        TimeOnly inicio, TimeOnly fim, string? observacao, string? fonteOficialUrl)
    {
        var c = new ColetaResiduos();
        c.Preencher(regiaoId, modalidade, dias, inicio, fim, observacao, fonteOficialUrl);
        return c;
    }

    public void Atualizar(Guid regiaoId, ModalidadeColeta modalidade, DiasSemana dias,
        TimeOnly inicio, TimeOnly fim, string? observacao, string? fonteOficialUrl)
    {
        Preencher(regiaoId, modalidade, dias, inicio, fim, observacao, fonteOficialUrl);
        Tocar();
    }

    /// <summary>Registra uma mudança temporária no cronograma (ou suspensão) com aviso ao morador.</summary>
    public void AlterarTemporariamente(SituacaoColeta situacao, string aviso, DateOnly? ate, DateOnly hoje)
    {
        if (situacao == SituacaoColeta.Regular)
            throw new DomainException("Para voltar ao normal use \"Normalizar coleta\".");
        Exigir.Definido(situacao, "situação");
        if (ate is { } dia && dia < hoje)
            throw new DomainException("A data final da alteração não pode estar no passado.");

        Situacao = situacao;
        AvisoAlteracao = Exigir.Texto(aviso, "o aviso para os moradores", 500);
        AlteracaoAte = ate;
        Tocar();
    }

    public void Normalizar()
    {
        if (Situacao == SituacaoColeta.Regular) throw new DomainException("A coleta já está regular.");
        Situacao = SituacaoColeta.Regular;
        AvisoAlteracao = null;
        AlteracaoAte = null;
        Tocar();
    }

    /// <summary>Situação que o morador deve ver hoje (alterações vencidas não aparecem mais).</summary>
    public SituacaoColeta SituacaoEm(DateOnly hoje) =>
        Situacao != SituacaoColeta.Regular && AlteracaoAte is { } ate && ate < hoje
            ? SituacaoColeta.Regular
            : Situacao;

    private void Preencher(Guid regiaoId, ModalidadeColeta modalidade, DiasSemana dias,
        TimeOnly inicio, TimeOnly fim, string? observacao, string? fonteOficialUrl)
    {
        Exigir.Id(regiaoId, "a região");
        Exigir.Definido(modalidade, "modalidade de coleta");
        if (dias == DiasSemana.Nenhum || ((int)dias & ~127) != 0)
            throw new DomainException("Selecione pelo menos um dia da semana.");
        if (fim <= inicio)
            throw new DomainException("O horário final precisa ser depois do horário inicial.");

        RegiaoId = regiaoId;
        Modalidade = modalidade;
        Dias = dias;
        HorarioInicio = inicio;
        HorarioFim = fim;
        Observacao = Exigir.TextoOpcional(observacao, "a observação", 500);
        FonteOficialUrl = Exigir.Url(fonteOficialUrl);
    }
}
