(function (root) {
  const categories = { collection: "Coleta de resíduos", works: "Obras", maintenance: "Manutenção" };
  const services = [
    { id: "demo-001", title: "Coleta seletiva", category: categories.collection, description: "Exemplo de orientação sobre separação e destinação de recicláveis.", location: "Centro", status: "Programado", updatedAt: "2026-10-18", schedule: "Exemplo de programação", officialSourceUrl: null, isDemo: true },
    { id: "demo-002", title: "Coleta de resíduos comuns", category: categories.collection, description: "Registro ilustrativo de serviço de coleta para consulta por localidade.", location: "Zona rural", status: "Alterado", updatedAt: "2026-10-15", schedule: "Consulte os canais oficiais", officialSourceUrl: null, isDemo: true },
    { id: "demo-003", title: "Ponto de entrega de recicláveis", category: categories.collection, description: "Exemplo de informação sobre ponto de entrega e descarte adequado.", location: "Estrada do Varadouro", status: "Em andamento", updatedAt: "2026-10-11", schedule: "Horário não informado", officialSourceUrl: null, isDemo: true },
    { id: "demo-004", title: "Adequação de via", category: categories.works, description: "Exemplo de intervenção viária com situação e atualização destacadas.", location: "Estrada do Varadouro", status: "Em andamento", updatedAt: "2026-10-19", schedule: "Previsão não informada", officialSourceUrl: null, isDemo: true },
    { id: "demo-005", title: "Melhoria de drenagem", category: categories.works, description: "Registro demonstrativo de obra de infraestrutura em via local.", location: "Zona rural", status: "Programado", updatedAt: "2026-10-13", schedule: "Data a confirmar", officialSourceUrl: null, isDemo: true },
    { id: "demo-006", title: "Nivelamento de estrada", category: categories.maintenance, description: "Exemplo de manutenção de via para demonstrar acompanhamento por status.", location: "Zona rural", status: "Concluído", updatedAt: "2026-10-20", schedule: "Registro demonstrativo", officialSourceUrl: null, isDemo: true },
    { id: "demo-007", title: "Manutenção de iluminação", category: categories.maintenance, description: "Informação ilustrativa sobre manutenção em espaço público.", location: "Centro", status: "Programado", updatedAt: "2026-10-17", schedule: "Data a confirmar", officialSourceUrl: null, isDemo: true },
    { id: "demo-008", title: "Desobstrução de drenagem", category: categories.maintenance, description: "Exemplo de atividade de manutenção preventiva em área pública.", location: "Estrada do Varadouro", status: "Em andamento", updatedAt: "2026-10-09", schedule: "Registro demonstrativo", officialSourceUrl: null, isDemo: true }
  ];
  const announcements = [
    { id: "aviso-demo-001", title: "Atenção: programação de coleta", message: "Mensagem de demonstração. Consulte a Prefeitura para confirmar dias, horários e eventuais alterações do serviço.", category: "Coleta", publishedAt: "2026-10-21", officialSourceUrl: null, isDemo: true },
    { id: "aviso-demo-002", title: "Intervenção em via local", message: "Aviso fictício para ilustrar como uma alteração de circulação poderá ser apresentada após confirmação oficial.", category: "Obras", publishedAt: "2026-10-16", officialSourceUrl: null, isDemo: true },
    { id: "aviso-demo-003", title: "Serviço de manutenção", message: "Conteúdo demonstrativo. Verifique a situação atual nos canais oficiais antes de se deslocar.", category: "Manutenção", publishedAt: "2026-10-12", officialSourceUrl: null, isDemo: true },
    { id: "aviso-demo-004", title: "Sobre este portal", message: "Este protótipo acadêmico organiza exemplos de serviços públicos e não possui integração com sistemas municipais.", category: "Geral", publishedAt: "2026-10-08", officialSourceUrl: null, isDemo: true }
  ];
  function normalize(value) {
    return String(value || "").normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLocaleLowerCase("pt-BR").trim();
  }
  function formatDate(value) {
    if (!value) return "Data não informada";
    const parts = value.split("-");
    return parts.length === 3 ? `${parts[2]}/${parts[1]}/${parts[0]}` : value;
  }
  root.PUP = root.PUP || {};
  root.PUP.data = { categories, services, announcements };
  root.PUP.utils = { normalize, formatDate };
})(window);
