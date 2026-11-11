(function (root) {
  function make(tag, className, text) {
    const element = document.createElement(tag);
    if (className) element.className = className;
    if (text !== undefined) element.textContent = text;
    return element;
  }
  function makeSource(item) {
    const source = make("p", "source-line");
    if (item.officialSourceUrl) {
      const link = make("a", "text-link", "Consultar fonte oficial");
      link.href = item.officialSourceUrl;
      link.target = "_blank";
      link.rel = "noopener noreferrer";
      source.append(link);
    } else source.textContent = item.isDemo ? "Registro ilustrativo, sem fonte oficial vinculada" : "Fonte oficial não informada";
    return source;
  }
  function serviceCard(item) {
    const card = make("article", "service-card");
    card.dataset.serviceId = item.id;
    card.dataset.category = item.category;
    const top = make("div", "card-topline");
    top.append(make("span", "category-label", item.category));
    top.append(make("span", `status status-${item.status.toLocaleLowerCase("pt-BR").replaceAll(" ", "-")}`, item.status));
    const details = make("dl", "card-details");
    [["Localidade", item.location], ["Atualizado em", root.PUP.utils.formatDate(item.updatedAt)], ["Programação", item.schedule]].forEach(([term, value]) => {
      const group = make("div", "detail-item");
      group.append(make("dt", "", term), make("dd", "", value));
      details.append(group);
    });
    card.append(top, make("h3", "card-title", item.title), make("p", "card-description", item.description), details, makeSource(item));
    return card;
  }
  function announcementItem(item, compact) {
    const article = make("article", compact ? "announcement-item announcement-compact" : "announcement-item");
    const meta = make("div", "announcement-meta");
    meta.append(make("span", "category-label", item.category), make("time", "announcement-date", root.PUP.utils.formatDate(item.publishedAt)));
    article.append(meta, make("h3", "announcement-title", item.title), make("p", "announcement-message", item.message));
    if (!compact) article.append(makeSource(item));
    else if (item.isDemo) article.append(make("span", "demo-inline", "Demonstração"));
    return article;
  }
  function renderList(container, items, renderItem, compact) {
    container.replaceChildren(...items.map((item) => renderItem(item, compact)));
  }
  root.PUP = root.PUP || {};
  root.PUP.components = { serviceCard, announcementItem, renderList };
})(window);
