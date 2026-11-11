# Portal de Utilidades Públicas (PUP)

Protótipo acadêmico de front-end para centralizar exemplos de informações sobre coleta de resíduos, obras, manutenção e comunicados de São Francisco Xavier.

> Os registros deste protótipo são fictícios. Não representam calendário, serviço, alerta ou comunicado oficial e não possuem integração com a Prefeitura.

## Executar

Abra `index.html` em um navegador moderno. O projeto usa HTML, CSS e JavaScript sem dependências ou etapa de build. Para testar com um servidor local, abra um terminal nesta pasta e execute `py -m http.server 5500`; depois acesse `http://localhost:5500`.

## Telas

- `index.html`: visão geral, atalhos, busca por serviço/categoria e comunicados recentes.
- `coleta.html`: serviços demonstrativos de coleta com busca, filtro de situação e localidade.
- `obras.html`: obras e intervenções com os mesmos filtros de consulta.
- `manutencao.html`: exemplos de manutenção e melhorias com filtros.
- `comunicados.html`: mural pesquisável e filtro por categoria.

## Organização

- `assets/css/app.css`: tokens visuais, layout, componentes e regras responsivas.
- `assets/js/data.js`: dados e utilitários de busca/formatação.
- `assets/js/components.js`: renderização reutilizável de cartões e comunicados.
- `assets/js/pages/`: controladores específicos de cada tela.
- `assets/js/app.js`: inicialização global e menu móvel.

Os scripts são carregados em ordem por cada página e usam `window.PUP` como namespace. A renderização de conteúdo usa `textContent`; ao conectar uma API, mantenha esse comportamento para dados não confiáveis.

## Migração futura para Blazor

- Transforme `data.js` em modelos/DTOs C# e substitua os arrays de demonstração por chamadas ao `PupApiClient`.
- Mapeie `components.js` para componentes Razor reutilizáveis, como cartões de serviço, status e comunicados.
- Mapeie cada HTML para uma página Razor com rota própria; converta os controladores em estado e eventos dos componentes.
- Reaproveite os tokens e estilos de `app.css` em `wwwroot/css/app.css` e mantenha os estados acessíveis, filtros e indicação de dados demonstrativos.
- Só remova o aviso de demonstração quando houver fonte, processo editorial e atualização de dados confirmados.

## Referência de produto

O recorte e os campos de consulta seguem o relatório técnico-científico e a documentação do PUP fornecidos para este projeto. A identidade combina navegação institucional azul, acentos inspirados na paleta pública brasileira, hierarquia direta e controles de interface familiar, sem reproduzir marcas oficiais.