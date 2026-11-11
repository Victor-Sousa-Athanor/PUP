(function (root) {
  function start() {
    const form = document.querySelector("#announcement-filter");
    if (!form) return;
    const search = document.querySelector("#announcement-search");
    const category = document.querySelector("#announcement-category");
    const results = document.querySelector("#announcement-results");
    const empty = document.querySelector("#announcement-empty");
    const count = document.querySelector("#announcement-count");
    function update() {
      const query = root.PUP.utils.normalize(search.value);
      const filtered = root.PUP.data.announcements.filter((item) => {
        const searchable = root.PUP.utils.normalize(`${item.title} ${item.message} ${item.category}`);
        return (!query || searchable.includes(query)) && (!category.value || item.category === category.value);
      });
      root.PUP.components.renderList(results, filtered, root.PUP.components.announcementItem, false);
      const suffix = root.PUP.data.source !== "api" ? " · todos demonstrativos" : "";
      count.textContent = `${filtered.length} ${filtered.length === 1 ? "comunicado encontrado" : "comunicados encontrados"}${suffix}`;
      empty.hidden = filtered.length !== 0;
      results.hidden = filtered.length === 0;
    }
    form.addEventListener("submit", (event) => { event.preventDefault(); update(); });
    form.addEventListener("reset", () => window.setTimeout(update, 0));
    search.addEventListener("input", update);
    category.addEventListener("change", update);
    update();
  }
  root.PUP = root.PUP || {};
  root.PUP.pages = root.PUP.pages || {};
  root.PUP.pages.announcements = { start };
})(window);
