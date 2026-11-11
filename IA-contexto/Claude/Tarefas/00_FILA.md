# Fila do Flash — PUP

Status: **A fazer** · **Fazendo** · **Feito** (build + testes + smoke passando, com commit) · **Bloqueado** (motivo na Nota).
Pegue sempre a primeira linha "A fazer" cujas dependências estejam todas "Feito".

| # | Tarefa | Depende de | Status | Nota | Arquivo |
| --- | --- | --- | --- | --- | --- |
| 1 | PUP-01 | — | Feito | Build, 20 testes e smoke verdes | `2026-11-11_PUP-01_primeiro-build.md` |
| 2 | PUP-02 | PUP-01 | Feito | Front servido na raiz, 404 nos .md, smoke verde | `2026-11-11_PUP-02_servir-o-front.md` |
| 3 | PUP-03 | PUP-01 | Feito | 37 testes + /api/portal/* verdes (smoke ok) | `2026-11-11_PUP-03_rotas-do-portal.md` |
| 4 | PUP-04 | PUP-03 | Feito | Seed recriado: 3 regiões, 12 serviços, 4 comunicados | `2026-11-11_PUP-04_seed-alinhado-ao-front.md` |
| 5 | PUP-05 | PUP-02, PUP-03, PUP-04 | Feito | Front integrado à API + fallback offline + smoke verde | `2026-11-11_PUP-05_front-consome-a-api.md` |
