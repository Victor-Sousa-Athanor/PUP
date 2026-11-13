(function (root) {
  const STORAGE_KEY = "pup_admin_sessao";
  const DIAS_BITS = { Domingo: 1, Segunda: 2, Terca: 4, Quarta: 8, Quinta: 16, Sexta: 32, Sabado: 64 };
  const ROTULOS = {
    Comum: "Coleta comum",
    Seletiva: "Coleta seletiva",
    Regular: "Regular",
    TemporariamenteAlterada: "Temporariamente alterada",
    Suspensa: "Suspensa",
    AbrigoEcologico: "Abrigo ecológico",
    LixeiraComunitaria: "Lixeira comunitária",
    Ecoponto: "Ecoponto",
    Programado: "Programado",
    EmAndamento: "Em andamento",
    Concluido: "Concluído",
    Cancelado: "Cancelado",
    Geral: "Geral",
    Coleta: "Coleta",
    Obras: "Obras",
    Manutencao: "Manutenção",
    Alerta: "Alerta",
    Urbana: "Urbana",
    Rural: "Rural"
  };

  let sessao = lerSessao();
  let cacheRegioes = [];

  function $(sel, ctx = document) {
    return ctx.querySelector(sel);
  }

  function el(tag, className, text) {
    const node = document.createElement(tag);
    if (className) node.className = className;
    if (text !== undefined && text !== null) node.textContent = String(text);
    return node;
  }

  function rotulo(valor) {
    return ROTULOS[valor] || String(valor || "");
  }

  function textoOuNulo(valor) {
    const t = String(valor || "").trim();
    return t.length ? t : null;
  }

  function formatarDataHora(iso) {
    if (!iso) return "";
    const d = new Date(iso);
    return Number.isNaN(d.getTime()) ? String(iso) : d.toLocaleString("pt-BR");
  }

  function formatarData(valor) {
    return valor ? root.PUP.utils.formatDate(String(valor)) : "Não informada";
  }

  function lerSessao() {
    try {
      const raw = root.sessionStorage.getItem(STORAGE_KEY);
      if (!raw) return null;
      const parsed = JSON.parse(raw);
      return parsed && parsed.token ? parsed : null;
    } catch {
      return null;
    }
  }

  function salvarSessao(nova) {
    sessao = nova;
    if (nova) root.sessionStorage.setItem(STORAGE_KEY, JSON.stringify(nova));
    else root.sessionStorage.removeItem(STORAGE_KEY);
  }

  function feedback(mensagem, tipo = "ok") {
    const box = $("#admin-feedback");
    if (!mensagem) {
      box.hidden = true;
      box.textContent = "";
      box.className = "admin-alert";
      return;
    }
    box.hidden = false;
    box.className = `admin-alert ${tipo === "erro" ? "admin-alert-erro" : "admin-alert-ok"}`;
    box.textContent = mensagem;
    box.scrollIntoView({ behavior: "smooth", block: "nearest" });
  }

  async function api(path, options = {}) {
    const headers = { Accept: "application/json", ...(options.headers || {}) };
    if (options.body !== undefined) headers["Content-Type"] = "application/json";
    if (sessao && sessao.token) headers.Authorization = `Bearer ${sessao.token}`;

    const resp = await fetch(root.PUP.utils.apiBase() + path, {
      method: options.method || "GET",
      headers,
      body: options.body !== undefined ? JSON.stringify(options.body) : undefined
    });

    if (resp.status === 401 && path !== "/api/auth/login") {
      salvarSessao(null);
      atualizarEstadoSessao();
      throw new Error("Sua sessão expirou. Faça login novamente.");
    }

    if (resp.status === 204) return null;

    const text = await resp.text();
    let payload = null;
    if (text) {
      try { payload = JSON.parse(text); } catch { payload = null; }
    }

    if (!resp.ok) {
      const msg = (payload && (payload.mensagem || payload.title)) || `Erro HTTP ${resp.status}`;
      throw new Error(msg);
    }
    return payload;
  }

  function confirmarExclusao(descricao) {
    return root.confirm(`Confirma excluir "${descricao}"? Essa ação não pode ser desfeita.`);
  }

  function botao(texto, classe, onClick) {
    const btn = el("button", `button button-sm ${classe}`, texto);
    btn.type = "button";
    btn.addEventListener("click", onClick);
    return btn;
  }

  // ---- Regiões ----
  async function carregarRegioes() {
    cacheRegioes = await api("/api/regioes?incluirInativas=true");
    document.querySelectorAll("[data-regiao-select]").forEach((select) => {
      const atual = select.value;
      const opcional = select.dataset.regiaoOpcional === "true";
      const opcoes = cacheRegioes.map((r) => new Option(r.ativa ? r.nome : `${r.nome} (inativa)`, r.id));
      if (opcional) opcoes.unshift(new Option("Distrito inteiro (todas as regiões)", ""));
      else if (opcoes.length === 0) opcoes.push(new Option("Cadastre uma região primeiro", ""));
      select.replaceChildren(...opcoes);
      if (atual && [...select.options].some((o) => o.value === atual)) select.value = atual;
    });
    renderListaRegioes();
  }

  function renderListaRegioes() {
    const box = $("#lista-regioes");
    if (!cacheRegioes.length) {
      box.replaceChildren(el("p", "empty-state", "Nenhuma região cadastrada."));
      return;
    }
    box.replaceChildren(...cacheRegioes.map((r) => {
      const item = el("article", "admin-item");
      const topo = el("div", "admin-item-header");
      topo.append(el("strong", "", r.nome), el("span", `status ${r.ativa ? "status-concluído" : "status-cancelado"}`, r.ativa ? "Ativa" : "Inativa"));
      const meta = el("p", "admin-item-meta", `Tipo: ${rotulo(r.tipo)}${r.descricao ? " · " + r.descricao : ""}`);
      const acoes = el("div", "admin-actions");
      acoes.append(botao("Editar", "button-secondary", () => preencherFormRegiao(r)));
      item.append(topo, meta, acoes);
      return item;
    }));
  }

  function preencherFormRegiao(r) {
    $("#regiao-id").value = r ? r.id : "";
    $("#regiao-nome").value = r ? r.nome : "";
    $("#regiao-tipo").value = r ? r.tipo : "Urbana";
    $("#regiao-descricao").value = r && r.descricao ? r.descricao : "";
    $("#regiao-ativa").checked = r ? Boolean(r.ativa) : true;
    $("#regiao-form-title").textContent = r ? `Editar região: ${r.nome}` : "Nova região / estrada";
    $("#regiao-cancelar").hidden = !r;
  }

  function configurarFormRegiao() {
    $("#regiao-cancelar").addEventListener("click", () => preencherFormRegiao(null));
    $("#form-regiao").addEventListener("submit", async (e) => {
      e.preventDefault();
      const id = $("#regiao-id").value;
      const body = {
        nome: $("#regiao-nome").value,
        tipo: $("#regiao-tipo").value,
        descricao: textoOuNulo($("#regiao-descricao").value),
        ativa: $("#regiao-ativa").checked
      };
      try {
        if (id) await api(`/api/regioes/${id}`, { method: "PUT", body });
        else await api("/api/regioes", { method: "POST", body });
        preencherFormRegiao(null);
        await carregarRegioes();
        feedback(id ? "Região atualizada com sucesso." : "Região cadastrada com sucesso.");
      } catch (err) {
        feedback(err.message, "erro");
      }
    });
  }

  // ---- Coletas ----
  function lerDiasSelecionados() {
    return [...document.querySelectorAll("#coleta-dias input[type=checkbox]:checked")].map((c) => c.value);
  }

  function marcarDias(diasValor) {
    const ativos = new Set();
    if (typeof diasValor === "number") {
      Object.entries(DIAS_BITS).forEach(([nome, bit]) => { if ((diasValor & bit) === bit) ativos.add(nome); });
    } else if (typeof diasValor === "string") {
      diasValor.split(",").map((s) => s.trim()).forEach((n) => { if (n) ativos.add(n); });
    }
    document.querySelectorAll("#coleta-dias input[type=checkbox]").forEach((cb) => {
      cb.checked = ativos.has(cb.value);
    });
  }

  function horaParaApi(valor) {
    const v = String(valor || "").trim();
    return v.length === 5 ? `${v}:00` : v;
  }

  function preencherFormColeta(c) {
    $("#coleta-id").value = c ? c.id : "";
    if (c) $("#coleta-regiao").value = c.regiaoId;
    $("#coleta-modalidade").value = c ? c.modalidade : "Comum";
    marcarDias(c ? c.dias : "");
    $("#coleta-inicio").value = c ? String(c.horarioInicio).slice(0, 5) : "07:00";
    $("#coleta-fim").value = c ? String(c.horarioFim).slice(0, 5) : "11:00";
    $("#coleta-observacao").value = c && c.observacao ? c.observacao : "";
    $("#coleta-fonte").value = c && c.fonteOficialUrl ? c.fonteOficialUrl : "";
    $("#coleta-form-title").textContent = c ? `Editar coleta (${c.regiaoNome})` : "Nova coleta de resíduos";
    $("#coleta-cancelar").hidden = !c;
  }

  async function carregarColetas() {
    const lista = await api("/api/coletas");
    const box = $("#lista-coletas");
    if (!lista.length) {
      box.replaceChildren(el("p", "empty-state", "Nenhuma coleta cadastrada."));
      return;
    }
    box.replaceChildren(...lista.map((c) => {
      const item = el("article", "admin-item");
      const topo = el("div", "admin-item-header");
      topo.append(
        el("strong", "", `${rotulo(c.modalidade)} · ${c.regiaoNome}`),
        el("span", `status ${c.situacao === "Regular" ? "status-programado" : "status-alterado"}`, rotulo(c.situacao))
      );
      const horario = `${String(c.horarioInicio).slice(0, 5)} às ${String(c.horarioFim).slice(0, 5)}`;
      const meta = el("p", "admin-item-meta", `Dias: ${c.dias} · Horário: ${horario}${c.observacao ? " · " + c.observacao : ""}`);
      item.append(topo, meta);

      if (c.avisoAlteracao) {
        const ate = c.alteracaoAte ? ` (até ${formatarData(c.alteracaoAte)})` : "";
        item.append(el("p", "admin-item-meta", `Aviso temporário: ${c.avisoAlteracao}${ate}`));
      }

      const subpanel = el("div", "admin-subpanel");
      subpanel.hidden = true;

      const acoes = el("div", "admin-actions");
      acoes.append(
        botao("Editar", "button-secondary", () => preencherFormColeta(c)),
        botao("Alterar situação", "button-warning", () => abrirSubpanelAlteracaoColeta(subpanel, c))
      );
      if (c.situacao !== "Regular") {
        acoes.append(botao("Normalizar", "button-secondary", async () => {
          try {
            await api(`/api/coletas/${c.id}/alteracao`, { method: "DELETE" });
            await carregarColetas();
            feedback("Coleta normalizada para o calendário regular.");
          } catch (err) { feedback(err.message, "erro"); }
        }));
      }
      acoes.append(botao("Excluir", "button-danger", async () => {
        if (!confirmarExclusao(`${rotulo(c.modalidade)} - ${c.regiaoNome}`)) return;
        try {
          await api(`/api/coletas/${c.id}`, { method: "DELETE" });
          await carregarColetas();
          feedback("Coleta removida.");
        } catch (err) { feedback(err.message, "erro"); }
      }));

      item.append(acoes, subpanel);
      return item;
    }));
  }

  function abrirSubpanelAlteracaoColeta(subpanel, c) {
    if (!subpanel.hidden) {
      subpanel.hidden = true;
      return;
    }
    const form = el("form", "admin-form");
    const selSit = el("select");
    selSit.append(new Option("Temporariamente alterada", "TemporariamenteAlterada"), new Option("Suspensa", "Suspensa"));
    if (c.situacao !== "Regular") selSit.value = c.situacao;

    const inpAte = el("input");
    inpAte.type = "date";
    inpAte.value = c.alteracaoAte || "";

    const inpAviso = el("input");
    inpAviso.type = "text";
    inpAviso.required = true;
    inpAviso.maxLength = 500;
    inpAviso.placeholder = "Explique o motivo e a nova orientação para os moradores";
    inpAviso.value = c.avisoAlteracao || "";

    const wrap = (labelTxt, ctrl, full = false) => {
      const l = el("label", full ? "field field-full" : "field");
      l.append(el("span", "field-label", labelTxt), ctrl);
      return l;
    };

    const actions = el("div", "field-full admin-actions");
    const btnSalvar = el("button", "button button-primary button-sm", "Aplicar alteração");
    btnSalvar.type = "submit";
    actions.append(btnSalvar, botao("Fechar", "button-secondary", () => { subpanel.hidden = true; }));

    form.append(wrap("Nova situação *", selSit), wrap("Válido até (opcional)", inpAte), wrap("Aviso aos moradores *", inpAviso, true), actions);
    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      try {
        await api(`/api/coletas/${c.id}/alteracao`, {
          method: "PUT",
          body: { situacao: selSit.value, aviso: inpAviso.value, ate: textoOuNulo(inpAte.value) }
        });
        await carregarColetas();
        feedback("Alteração temporária registrada.");
      } catch (err) { feedback(err.message, "erro"); }
    });

    subpanel.replaceChildren(form);
    subpanel.hidden = false;
  }

  function configurarFormColeta() {
    $("#coleta-cancelar").addEventListener("click", () => preencherFormColeta(null));
    $("#form-coleta").addEventListener("submit", async (e) => {
      e.preventDefault();
      const dias = lerDiasSelecionados();
      if (!dias.length) {
        feedback("Selecione pelo menos um dia da semana para a coleta.", "erro");
        return;
      }
      const id = $("#coleta-id").value;
      const body = {
        regiaoId: $("#coleta-regiao").value,
        modalidade: $("#coleta-modalidade").value,
        dias: dias.join(", "),
        horarioInicio: horaParaApi($("#coleta-inicio").value),
        horarioFim: horaParaApi($("#coleta-fim").value),
        observacao: textoOuNulo($("#coleta-observacao").value),
        fonteOficialUrl: textoOuNulo($("#coleta-fonte").value)
      };
      try {
        if (id) await api(`/api/coletas/${id}`, { method: "PUT", body });
        else await api("/api/coletas", { method: "POST", body });
        preencherFormColeta(null);
        await carregarColetas();
        feedback(id ? "Coleta atualizada." : "Coleta cadastrada.");
      } catch (err) { feedback(err.message, "erro"); }
    });
  }

  // ---- Pontos de Descarte ----
  function preencherFormPonto(p) {
    $("#ponto-id").value = p ? p.id : "";
    $("#ponto-nome").value = p ? p.nome : "";
    $("#ponto-tipo").value = p ? p.tipo : "AbrigoEcologico";
    if (p) $("#ponto-regiao").value = p.regiaoId;
    $("#ponto-referencia").value = p ? p.referencia : "";
    $("#ponto-lat").value = p && p.latitude !== null ? p.latitude : "";
    $("#ponto-lng").value = p && p.longitude !== null ? p.longitude : "";
    $("#ponto-orientacoes").value = p && p.orientacoes ? p.orientacoes : "";
    $("#ponto-fonte").value = p && p.fonteOficialUrl ? p.fonteOficialUrl : "";
    $("#ponto-ativo").checked = p ? Boolean(p.ativo) : true;
    $("#ponto-form-title").textContent = p ? `Editar ponto: ${p.nome}` : "Novo ponto de descarte";
    $("#ponto-cancelar").hidden = !p;
  }

  async function carregarPontos() {
    const lista = await api("/api/pontos-descarte?incluirInativos=true");
    const box = $("#lista-pontos");
    if (!lista.length) {
      box.replaceChildren(el("p", "empty-state", "Nenhum ponto de descarte cadastrado."));
      return;
    }
    box.replaceChildren(...lista.map((p) => {
      const item = el("article", "admin-item");
      const topo = el("div", "admin-item-header");
      topo.append(el("strong", "", `${p.nome} (${rotulo(p.tipo)})`), el("span", `status ${p.ativo ? "status-concluído" : "status-cancelado"}`, p.ativo ? "Ativo" : "Inativo"));
      const meta = el("p", "admin-item-meta", `${p.regiaoNome} · ${p.referencia}${p.orientacoes ? " — " + p.orientacoes : ""}`);
      const acoes = el("div", "admin-actions");
      acoes.append(
        botao("Editar", "button-secondary", () => preencherFormPonto(p)),
        botao("Excluir", "button-danger", async () => {
          if (!confirmarExclusao(p.nome)) return;
          try {
            await api(`/api/pontos-descarte/${p.id}`, { method: "DELETE" });
            await carregarPontos();
            feedback("Ponto de descarte removido.");
          } catch (err) { feedback(err.message, "erro"); }
        })
      );
      item.append(topo, meta, acoes);
      return item;
    }));
  }

  function configurarFormPonto() {
    $("#ponto-cancelar").addEventListener("click", () => preencherFormPonto(null));
    $("#form-ponto").addEventListener("submit", async (e) => {
      e.preventDefault();
      const id = $("#ponto-id").value;
      const lat = textoOuNulo($("#ponto-lat").value);
      const lng = textoOuNulo($("#ponto-lng").value);
      const body = {
        nome: $("#ponto-nome").value,
        tipo: $("#ponto-tipo").value,
        regiaoId: $("#ponto-regiao").value,
        referencia: $("#ponto-referencia").value,
        latitude: lat !== null ? Number(lat) : null,
        longitude: lng !== null ? Number(lng) : null,
        orientacoes: textoOuNulo($("#ponto-orientacoes").value),
        fonteOficialUrl: textoOuNulo($("#ponto-fonte").value),
        ativo: $("#ponto-ativo").checked
      };
      try {
        if (id) await api(`/api/pontos-descarte/${id}`, { method: "PUT", body });
        else await api("/api/pontos-descarte", { method: "POST", body });
        preencherFormPonto(null);
        await carregarPontos();
        feedback(id ? "Ponto atualizado." : "Ponto cadastrado.");
      } catch (err) { feedback(err.message, "erro"); }
    });
  }

  // ---- Intervenções (Obras e Manutenções compartilhando a mesma lógica) ----
  function criarGerenciadorIntervencoes(tipoApi, prefixoId, rotuloSingular, rotuloPlural) {
    const f = (campo) => $(`#${prefixoId}-${campo}`);

    function preencherForm(item) {
      f("id").value = item ? item.id : "";
      f("titulo").value = item ? item.titulo : "";
      if (item) f("regiao").value = item.regiaoId;
      f("local").value = item && item.local ? item.local : "";
      f("status").value = item ? item.status : "Programado";
      f("status-wrap").hidden = Boolean(item);
      f("inicio").value = item && item.dataInicio ? item.dataInicio : "";
      f("previsao").value = item && item.previsaoConclusao ? item.previsaoConclusao : "";
      f("impacto").value = item && item.impactoNaVia ? item.impactoNaVia : "";
      f("fonte").value = item && item.fonteOficialUrl ? item.fonteOficialUrl : "";
      f("descricao").value = item ? item.descricao : "";
      f("form-title").textContent = item ? `Editar ${rotuloSingular}: ${item.titulo}` : `Nova ${rotuloSingular}`;
      f("cancelar").hidden = !item;
    }

    async function carregar() {
      const lista = await api(`/api/intervencoes?tipo=${tipoApi}`);
      const box = $(`#lista-${prefixoId === "obra" ? "obras" : "manutencao"}`);
      if (!lista.length) {
        box.replaceChildren(el("p", "empty-state", `Nenhuma ${rotuloSingular} cadastrada.`));
        return;
      }
      box.replaceChildren(...lista.map((it) => {
        const card = el("article", "admin-item");
        const topo = el("div", "admin-item-header");
        topo.append(
          el("strong", "", `${it.titulo} · ${it.regiaoNome}`),
          el("span", `status status-${rotulo(it.status).toLocaleLowerCase("pt-BR").replaceAll(" ", "-")}`, rotulo(it.status))
        );
        const datas = `Previsão: ${formatarData(it.previsaoConclusao)}${it.atrasada ? " (vencida)" : ""}`;
        const meta = el("p", "admin-item-meta", `${it.descricao}${it.local ? " · Local: " + it.local : ""} · ${datas}`);
        const subpanel = el("div", "admin-subpanel");
        subpanel.hidden = true;

        const acoes = el("div", "admin-actions");
        acoes.append(
          botao("Editar dados", "button-secondary", async () => {
            try {
              const detalhe = await api(`/api/intervencoes/${it.id}`);
              preencherForm(detalhe);
              f("titulo").focus();
            } catch (err) { feedback(err.message, "erro"); }
          }),
          botao("Mudar situação", "button-warning", () => abrirSubpanelStatus(subpanel, it)),
          botao("Histórico", "button-secondary", () => abrirSubpanelHistorico(subpanel, it)),
          botao("Excluir", "button-danger", async () => {
            if (!confirmarExclusao(it.titulo)) return;
            try {
              await api(`/api/intervencoes/${it.id}`, { method: "DELETE" });
              await carregar();
              feedback(`${rotuloSingular[0].toUpperCase() + rotuloSingular.slice(1)} removida.`);
            } catch (err) { feedback(err.message, "erro"); }
          })
        );
        card.append(topo, meta, acoes, subpanel);
        return card;
      }));
    }

    function abrirSubpanelStatus(subpanel, it) {
      if (!subpanel.hidden && subpanel.dataset.mode === "status") {
        subpanel.hidden = true;
        return;
      }
      subpanel.dataset.mode = "status";
      const form = el("form", "admin-form");
      const sel = el("select");
      [
        ["Programado", "Programado"],
        ["EmAndamento", "Em andamento"],
        ["TemporariamenteAlterado", "Temporariamente alterado"],
        ["Concluido", "Concluído"],
        ["Cancelado", "Cancelado"]
      ].forEach(([val, txt]) => sel.append(new Option(txt, val)));
      sel.value = it.status;

      const inpNota = el("input");
      inpNota.type = "text";
      inpNota.required = true;
      inpNota.maxLength = 500;
      inpNota.placeholder = "Nota obrigatória para o histórico (ex.: Equipe iniciou o serviço)";

      const wrap = (lbl, ctrl, full = false) => {
        const l = el("label", full ? "field field-full" : "field");
        l.append(el("span", "field-label", lbl), ctrl);
        return l;
      };
      const actions = el("div", "field-full admin-actions");
      const btn = el("button", "button button-primary button-sm", "Gravar nova situação");
      btn.type = "submit";
      actions.append(btn, botao("Fechar", "button-secondary", () => { subpanel.hidden = true; }));

      form.append(wrap("Nova situação *", sel), wrap("Nota de atualização *", inpNota, true), actions);
      form.addEventListener("submit", async (e) => {
        e.preventDefault();
        try {
          await api(`/api/intervencoes/${it.id}/status`, { method: "POST", body: { status: sel.value, nota: inpNota.value } });
          await carregar();
          feedback(`Situação da ${rotuloSingular} atualizada e registrada no histórico.`);
        } catch (err) { feedback(err.message, "erro"); }
      });

      subpanel.replaceChildren(form);
      subpanel.hidden = false;
    }

    async function abrirSubpanelHistorico(subpanel, it) {
      if (!subpanel.hidden && subpanel.dataset.mode === "hist") {
        subpanel.hidden = true;
        return;
      }
      subpanel.dataset.mode = "hist";
      try {
        const detalhe = await api(`/api/intervencoes/${it.id}`);
        const ul = el("ul", "admin-history-list");
        (detalhe.historico || []).forEach((h) => {
          ul.append(el("li", "", `${formatarDataHora(h.data)} — ${rotulo(h.status)}: ${h.nota}`));
        });
        const fechar = botao("Fechar histórico", "button-secondary", () => { subpanel.hidden = true; });
        subpanel.replaceChildren(el("strong", "", `Histórico (${rotuloPlural}):`), ul, fechar);
        subpanel.hidden = false;
      } catch (err) { feedback(err.message, "erro"); }
    }

    function configurar() {
      f("cancelar").addEventListener("click", () => preencherForm(null));
      $(`#form-${prefixoId}`).addEventListener("submit", async (e) => {
        e.preventDefault();
        const id = f("id").value;
        const comum = {
          titulo: f("titulo").value,
          descricao: f("descricao").value,
          regiaoId: f("regiao").value,
          local: textoOuNulo(f("local").value),
          dataInicio: textoOuNulo(f("inicio").value),
          previsaoConclusao: textoOuNulo(f("previsao").value),
          impactoNaVia: textoOuNulo(f("impacto").value),
          fonteOficialUrl: textoOuNulo(f("fonte").value)
        };
        try {
          if (id) {
            await api(`/api/intervencoes/${id}`, { method: "PUT", body: comum });
          } else {
            await api("/api/intervencoes", { method: "POST", body: { ...comum, tipo: tipoApi, status: f("status").value } });
          }
          preencherForm(null);
          await carregar();
          feedback(id ? `Dados da ${rotuloSingular} atualizados.` : `${rotuloSingular[0].toUpperCase() + rotuloSingular.slice(1)} cadastrada.`);
        } catch (err) { feedback(err.message, "erro"); }
      });
    }

    return { carregar, configurar };
  }

  // ---- Comunicados ----
  function preencherFormComunicado(c) {
    $("#comunicado-id").value = c ? c.id : "";
    $("#comunicado-titulo").value = c ? c.titulo : "";
    $("#comunicado-categoria").value = c ? c.categoria : "Geral";
    $("#comunicado-regiao").value = c && c.regiaoId ? c.regiaoId : "";
    $("#comunicado-validade").value = c && c.validoAte ? c.validoAte : "";
    $("#comunicado-resumo").value = c ? c.resumo : "";
    $("#comunicado-conteudo").value = c ? c.conteudo : "";
    $("#comunicado-fonte").value = c && c.fonteOficialUrl ? c.fonteOficialUrl : "";
    $("#comunicado-destaque").checked = c ? Boolean(c.destaque) : false;
    $("#comunicado-form-title").textContent = c ? `Editar comunicado: ${c.titulo}` : "Novo comunicado";
    $("#comunicado-cancelar").hidden = !c;
  }

  async function carregarComunicados() {
    const lista = await api("/api/comunicados?vigentes=false");
    const box = $("#lista-comunicados");
    if (!lista.length) {
      box.replaceChildren(el("p", "empty-state", "Nenhum comunicado cadastrado."));
      return;
    }
    box.replaceChildren(...lista.map((c) => {
      const item = el("article", "admin-item");
      const topo = el("div", "admin-item-header");
      topo.append(
        el("strong", "", `${c.titulo} (${rotulo(c.categoria)})`),
        el("span", `status ${c.vigente ? "status-concluído" : "status-cancelado"}`, c.vigente ? (c.destaque ? "Vigente · Destaque" : "Vigente") : "Vencido")
      );
      const local = c.regiaoNome || "Distrito inteiro";
      const validade = c.validoAte ? ` · Válido até ${formatarData(c.validoAte)}` : "";
      const meta = el("p", "admin-item-meta", `${local}${validade} — ${c.resumo}`);
      const acoes = el("div", "admin-actions");
      acoes.append(
        botao("Editar", "button-secondary", () => preencherFormComunicado(c)),
        botao("Excluir", "button-danger", async () => {
          if (!confirmarExclusao(c.titulo)) return;
          try {
            await api(`/api/comunicados/${c.id}`, { method: "DELETE" });
            await carregarComunicados();
            feedback("Comunicado removido.");
          } catch (err) { feedback(err.message, "erro"); }
        })
      );
      item.append(topo, meta, acoes);
      return item;
    }));
  }

  function configurarFormComunicado() {
    $("#comunicado-cancelar").addEventListener("click", () => preencherFormComunicado(null));
    $("#form-comunicado").addEventListener("submit", async (e) => {
      e.preventDefault();
      const id = $("#comunicado-id").value;
      const body = {
        titulo: $("#comunicado-titulo").value,
        resumo: $("#comunicado-resumo").value,
        conteudo: $("#comunicado-conteudo").value,
        categoria: $("#comunicado-categoria").value,
        regiaoId: textoOuNulo($("#comunicado-regiao").value),
        validoAte: textoOuNulo($("#comunicado-validade").value),
        destaque: $("#comunicado-destaque").checked,
        fonteOficialUrl: textoOuNulo($("#comunicado-fonte").value)
      };
      try {
        if (id) await api(`/api/comunicados/${id}`, { method: "PUT", body });
        else await api("/api/comunicados", { method: "POST", body });
        preencherFormComunicado(null);
        await carregarComunicados();
        feedback(id ? "Comunicado atualizado." : "Comunicado publicado.");
      } catch (err) { feedback(err.message, "erro"); }
    });
  }

  // ---- Páginas e Rotas ----
  let cachePaginas = [];

  async function carregarPaginas() {
    cachePaginas = await api("/api/paginas");
    renderListaPaginas();
  }

  function renderListaPaginas() {
    const box = $("#lista-paginas");
    if (!cachePaginas.length) {
      box.replaceChildren(el("p", "empty-state", "Nenhuma página cadastrada."));
      return;
    }
    box.replaceChildren(...cachePaginas.map((p) => {
      const item = el("article", "admin-item");
      const topo = el("div", "admin-item-header");
      const badge = p.publicada ? "Publicada" : "Rascunho";
      const cls = p.publicada ? "status-concluído" : "status-cancelado";
      topo.append(el("strong", "", `${p.titulo} (/${p.rota})`), el("span", `status ${cls}`, badge));
      const tipo = p.ehSistema ? "Fixa do sistema" : "Página dinâmica";
      const menuTxt = p.exibirNoMenu ? "No menu" : "Oculta do menu";
      const meta = el("p", "admin-item-meta", `Ordem: ${p.ordem} · ${tipo} · ${menuTxt}${p.resumo ? " · " + p.resumo : ""}`);
      const acoes = el("div", "admin-actions");
      const abrir = el("a", "button button-secondary button-sm", "Abrir ↗");
      abrir.href = `/${p.rota}`;
      abrir.target = "_blank";
      abrir.rel = "noopener";
      acoes.append(abrir, botao("Editar", "button-secondary", () => preencherFormPagina(p)));
      if (!p.ehSistema) {
        acoes.append(botao("Excluir", "button-danger", async () => {
          if (!window.confirm(`Excluir a página '${p.titulo}' (/${p.rota})?`)) return;
          try {
            await api(`/api/paginas/${p.id}`, { method: "DELETE" });
            await carregarPaginas();
            if (root.PUP.refreshMenu) await root.PUP.refreshMenu();
            feedback("Página removida.");
          } catch (err) { feedback(err.message, "erro"); }
        }));
      }
      item.append(topo, meta, acoes);
      return item;
    }));
  }

  function preencherFormPagina(p) {
    $("#pagina-id").value = p ? p.id : "";
    $("#pagina-titulo").value = p ? p.titulo : "";
    $("#pagina-rota").value = p ? p.rota : "";
    $("#pagina-rota").readOnly = Boolean(p && p.ehSistema);
    $("#pagina-ordem").value = p ? p.ordem : 10;
    $("#pagina-fonte").value = p && p.fonteOficialUrl ? p.fonteOficialUrl : "";
    $("#pagina-resumo").value = p && p.resumo ? p.resumo : "";
    $("#pagina-conteudo").value = p && p.conteudo ? p.conteudo : "";
    $("#pagina-menu").checked = p ? Boolean(p.exibirNoMenu) : true;
    $("#pagina-publicada").checked = p ? Boolean(p.publicada) : true;
    $("#pagina-form-title").textContent = p ? `Editar página: ${p.titulo}` : "Nova página / rota";
    $("#pagina-cancelar").hidden = !p;
  }

  function configurarFormPagina() {
    $("#pagina-cancelar").addEventListener("click", () => preencherFormPagina(null));
    $("#form-pagina").addEventListener("submit", async (e) => {
      e.preventDefault();
      const id = $("#pagina-id").value;
      const body = {
        titulo: $("#pagina-titulo").value,
        rota: $("#pagina-rota").value,
        resumo: textoOuNulo($("#pagina-resumo").value),
        conteudo: textoOuNulo($("#pagina-conteudo").value),
        ordem: Number($("#pagina-ordem").value || 0),
        exibirNoMenu: $("#pagina-menu").checked,
        publicada: $("#pagina-publicada").checked,
        fonteOficialUrl: textoOuNulo($("#pagina-fonte").value)
      };
      try {
        if (id) await api(`/api/paginas/${id}`, { method: "PUT", body });
        else await api("/api/paginas", { method: "POST", body });
        preencherFormPagina(null);
        await carregarPaginas();
        if (root.PUP.refreshMenu) await root.PUP.refreshMenu();
        feedback(id ? "Página atualizada com sucesso." : "Página criada com sucesso.");
      } catch (err) { feedback(err.message, "erro"); }
    });
  }

  // ---- Abas, Sessão e Inicialização ----
  const gerObras = criarGerenciadorIntervencoes("Obra", "obra", "obra", "obras");
  const gerManutencao = criarGerenciadorIntervencoes("Manutencao", "manutencao", "manutenção", "manutenções");

  async function carregarTudo() {
    await carregarRegioes();
    await Promise.all([
      carregarColetas(),
      carregarPontos(),
      gerObras.carregar(),
      gerManutencao.carregar(),
      carregarComunicados(),
      carregarPaginas()
    ]);
  }

  function atualizarEstadoSessao() {
    const autenticado = Boolean(sessao && sessao.token);
    $("#admin-login-section").hidden = autenticado;
    $("#admin-workspace").hidden = !autenticado;
    if (autenticado) {
      $("#admin-user-name").textContent = sessao.nome || "Administrador";
      $("#admin-token-expiry").textContent = sessao.expiraEm ? ` · válida até ${formatarDataHora(sessao.expiraEm)}` : "";
    }
  }

  function ativarAba(nome) {
    document.querySelectorAll(".admin-tab").forEach((btn) => {
      btn.setAttribute("aria-selected", String(btn.dataset.tab === nome));
    });
    document.querySelectorAll(".admin-panel").forEach((panel) => {
      panel.hidden = panel.id !== `panel-${nome}`;
    });
  }

  function start() {
    if (!document.querySelector("#admin-login-form")) return;

    document.querySelectorAll(".admin-tab").forEach((btn) => {
      btn.addEventListener("click", () => ativarAba(btn.dataset.tab));
    });

    $("#admin-login-form").addEventListener("submit", async (e) => {
      e.preventDefault();
      feedback(null);
      try {
        const res = await api("/api/auth/login", {
          method: "POST",
          body: { email: $("#login-email").value, senha: $("#login-senha").value }
        });
        salvarSessao(res);
        $("#login-senha").value = "";
        atualizarEstadoSessao();
        await carregarTudo();
        feedback(`Bem-vindo(a), ${res.nome}.`);
      } catch (err) { feedback(err.message, "erro"); }
    });

    $("#admin-logout").addEventListener("click", () => {
      salvarSessao(null);
      atualizarEstadoSessao();
      feedback("Sessão encerrada.");
    });

    $("#admin-refresh-all").addEventListener("click", async () => {
      try {
        await carregarTudo();
        feedback("Listas atualizadas.");
      } catch (err) { feedback(err.message, "erro"); }
    });

    configurarFormRegiao();
    configurarFormColeta();
    configurarFormPonto();
    gerObras.configurar();
    gerManutencao.configurar();
    configurarFormComunicado();
    configurarFormPagina();

    atualizarEstadoSessao();
    if (sessao && sessao.token) {
      carregarTudo().catch((err) => feedback(err.message, "erro"));
    }
  }

  root.PUP = root.PUP || {};
  root.PUP.pages = root.PUP.pages || {};
  root.PUP.pages.admin = { start };
})(window);
