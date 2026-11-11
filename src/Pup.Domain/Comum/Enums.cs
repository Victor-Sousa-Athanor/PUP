namespace Pup.Domain.Comum;

public enum TipoRegiao
{
    Urbana = 1,
    Rural = 2
}

public enum ModalidadeColeta
{
    Comum = 1,
    Seletiva = 2
}

/// <summary>Dias da semana combináveis (ex.: Terca | Quinta | Sabado).</summary>
[Flags]
public enum DiasSemana
{
    Nenhum = 0,
    Domingo = 1,
    Segunda = 2,
    Terca = 4,
    Quarta = 8,
    Quinta = 16,
    Sexta = 32,
    Sabado = 64
}

public enum SituacaoColeta
{
    Regular = 1,
    TemporariamenteAlterada = 2,
    Suspensa = 3
}

public enum TipoPontoDescarte
{
    AbrigoEcologico = 1,
    LixeiraComunitaria = 2,
    Ecoponto = 3
}

public enum TipoIntervencao
{
    Obra = 1,
    Manutencao = 2
}

/// <summary>Termos sugeridos no relatório (seção 2.3.5) para o cidadão entender a situação.</summary>
public enum StatusIntervencao
{
    Programado = 1,
    EmAndamento = 2,
    Concluido = 3,
    TemporariamenteAlterado = 4,
    Cancelado = 5
}

public enum CategoriaComunicado
{
    Geral = 1,
    Coleta = 2,
    Obras = 3,
    Manutencao = 4,
    Alerta = 5
}

/// <summary>Categorias do portal (cartões da página inicial, busca e "últimas atualizações").</summary>
public enum CategoriaServico
{
    Coleta = 1,
    PontoDescarte = 2,
    Obra = 3,
    Manutencao = 4,
    Comunicado = 5
}
