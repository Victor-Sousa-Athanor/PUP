(function (root) {
  function start() {
    const form = document.querySelector("#home-filter");
    if (!form) return;
    const search = document.querySelector("#home-search");
    const category = document.querySelector("#home-category");
    const results = document.querySelector("#home-services");
    const empty = document.querySelector("#home-empty");
    const count = document.querySelector("#home-count");
    const announcements = document.querySelector("#home-announcements");
    function update() {
      const query = root.PUP.utils.normalize(search.value);
      const selectedCategory = category.value;
      const filtered = root.PUP.data.services.filter((item) => {
        const searchable = root.PUP.utils.normalize(`${item.title} ${item.description} ${item.location} ${item.status} ${item.category}`);
        return (!query || searchable.includes(query)) && (!selectedCategory || item.category === selectedCategory);
      });
      root.PUP.components.renderList(results, filtered.slice(0, 4), root.PUP.components.serviceCard);
      count.textContent = `${filtered.length} ${filtered.length === 1 ? "serviço encontrado" : "serviços encontrados"} · exemplos demonstrativos`;
      empty.hidden = filtered.length !== 0;
      results.hidden = filtered.length === 0;
    }
    form.addEventListener("submit", (event) => { event.preventDefault(); update(); });
    form.addEventListener("reset", () => window.setTimeout(update, 0));
    search.addEventListener("input", update);
    category.addEventListener("change", update);
    root.PUP.components.renderList(announcements, root.PUP.data.announcements.slice(0, 2), root.PUP.components.announcementItem, true);
    const params = new URLSearchParams(window.location.search);
    if (params.has("q")) search.value = params.get("q");
    update();
  }
  root.PUP = root.PUP || {};
  root.PUP.pages = root.PUP.pages || {};
  root.PUP.pages.home = { start };
})(window);
