# PUP-04 — Dados de exemplo alinhados ao front

**Depende de: PUP-03**
Relatório: `2026-11-11_PUP-04_seed-alinhado-ao-front-RELATORIO.md`

## Objetivo

O front da dupla tem as localidades fixas no `<select>` (`Centro`, `Zona rural`, `Estrada do Varadouro`) e espera
pelo menos 2 itens por tela. Os dados de exemplo do back passam a usar essas mesmas localidades, para os filtros
do front funcionarem já na primeira subida.

## Arquivos

- `src/Pup.Application/Features/Inicializacao/DadosExemplo.cs`
- `README.md` (só a frase sobre dados de exemplo, se precisar)

## O que fazer

Reescreva o conteúdo de `SemearAsync` mantendo a assinatura, as constantes `NoticiaColeta2023`/`NoticiaColeta2024`,
o uso dos repos e o **único** `SalvarAsync` no fim. Use as fábricas do domínio (`Regiao.Criar`, `ColetaResiduos.Criar`,
`PontoDescarte.Criar`, `Intervencao.Criar`, `Comunicado.Criar`). Datas sempre relativas a `hoje` (`relogio.Hoje`).
Tudo que é inventado começa com `[Exemplo]` (o portal tira a marca e mostra o aviso de demonstração).

### Regiões (nomes exatamente assim)

| Nome | Tipo | Descrição |
| --- | --- | --- |
| `Centro` | Urbana | Área urbana central do distrito de São Francisco Xavier. |
| `Zona rural` | Rural | Bairros e estradas rurais atendidos por abrigos ecológicos. |
| `Estrada do Varadouro` | Rural | [Exemplo] Estrada rural usada nos exemplos do protótipo. |

### Coletas

| Região | Modalidade | Dias | Horário | Observação | Fonte |
| --- | --- | --- | --- | --- | --- |
| Centro | Comum | Ter, Qui, Sáb | 07:00–15:20 | Confirme dias e horários atualizados nos canais oficiais da Prefeitura/Urbam. | 2023 |
| Centro | Seletiva | Ter, Qui, Sáb | 07:00–15:20 | Separe papel, plástico, metal e vidro limpos. Horário de referência; confirme nos canais oficiais. | 2024 |
| Zona rural | Seletiva | Qua | 08:00–14:00 | [Exemplo] Recolhimento nos abrigos ecológicos (lixeiras de comum e seletiva lado a lado). | 2024 |
| Estrada do Varadouro | Comum | Ter, Sáb | 08:00–12:00 | [Exemplo] Coleta nos abrigos ecológicos ao longo da estrada. | — |

A coleta da Estrada do Varadouro recebe logo depois de criada:
`AlterarTemporariamente(SituacaoColeta.TemporariamenteAlterada, "[Exemplo] Por causa das chuvas, a coleta desta semana passa para quarta-feira.", hoje.AddDays(7), hoje)`.

### Pontos de descarte (tipo `AbrigoEcologico`, sem coordenadas, fonte 2024)

| Nome | Região | Referência | Orientações |
| --- | --- | --- | --- |
| [Exemplo] Abrigo ecológico do Varadouro | Estrada do Varadouro | [Exemplo] Ao lado do ponto de ônibus, km 2 | Lixeira verde: recicláveis limpos. Lixeira cinza: lixo comum ensacado. Não deixe entulho, móveis ou poda. |
| [Exemplo] Abrigo ecológico da capela | Zona rural | [Exemplo] Em frente à capela do bairro | A Prefeitura informou abrigos ecológicos em 88 pontos da zona rural do distrito (2024). |

### Obras e manutenções (sem fonte oficial)

| Tipo | Título | Região | Local | Situação final | Início | Previsão | Impacto na via |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Obra | [Exemplo] Adequação de via | Estrada do Varadouro | Trecho de terra perto do km 2 | EmAndamento | hoje-20 | hoje+40 | Meia pista em alguns trechos durante o dia. |
| Obra | [Exemplo] Melhoria de drenagem | Zona rural | — | Programado | hoje+10 | hoje+60 | — |
| Obra | [Exemplo] Revitalização da praça central | Centro | Praça central | Programado | hoje+15 | hoje+90 | — |
| Manutenção | [Exemplo] Manutenção de iluminação | Centro | — | Programado | hoje+5 | hoje+7 | — |
| Manutenção | [Exemplo] Desobstrução de drenagem | Estrada do Varadouro | — | EmAndamento | hoje-3 | hoje+4 | Possíveis bloqueios rápidos. |
| Manutenção | [Exemplo] Nivelamento de estrada | Zona rural | Estradas rurais | Concluido | hoje-10 | hoje-2 | — |

Descrições curtas e objetivas (ex.: "Cascalhamento, nivelamento e melhoria da drenagem em trecho de terra.").
O "Nivelamento de estrada" é criado como `EmAndamento` e depois recebe
`MudarStatus(StatusIntervencao.Concluido, "[Exemplo] Serviço concluído e via liberada.", hoje)`
(o domínio não deixa cadastrar já concluído).

### Comunicados

| Título | Categoria | Região | Validade | Destaque | Fonte |
| --- | --- | --- | --- | --- | --- |
| Coleta seletiva também na zona rural | Coleta | distrito | — | sim | 2024 (mantenha o texto atual) |
| Solicitações pelo 156 | Geral | distrito | — | sim | — (mantenha o texto atual) |
| [Exemplo] Interdição parcial na Estrada do Varadouro | Alerta | Estrada do Varadouro | hoje+15 | não | — |
| Sobre este portal | Geral | distrito | — | não | — |

Texto do "Sobre este portal": resumo "Protótipo acadêmico do Projeto Integrador II (UNIVESP)." e conteúdo
"Protótipo acadêmico do Projeto Integrador II (UNIVESP). Organiza informações públicas de São Francisco Xavier e não
substitui os canais oficiais da Prefeitura de São José dos Campos."

## Como conferir

O seed só roda com o banco vazio. Recrie o banco: `docker compose down -v`, `docker compose up -d`, suba a API e:
- `GET /api/regioes` → exatamente `Centro`, `Zona rural`, `Estrada do Varadouro`;
- `GET /api/portal/servicos` → 4 coletas + 2 pontos + 3 obras + 3 manutenções = **12 itens**;
- `GET /api/portal/comunicados` → **4 itens**.
Escreva no relatório o que cada rota devolveu (quantidade e títulos).

Commit `PUP-04: dados de exemplo alinhados ao front`.

## Pronto quando

- [ ] As contagens acima batem
- [ ] Build, testes e smoke verdes
