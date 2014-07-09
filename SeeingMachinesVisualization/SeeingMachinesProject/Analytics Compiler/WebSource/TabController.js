
function Tab(container) {

	this.element = $("<div>");
	this.element.addClass('tab');

	this.element.click(function () {
		TabManager.selectTab(this);
	}.bind(this));

	container.append(this.element);

	this.contentElement = $("<div>").get(0);

	this.getName = function () {
		return this.element.text();
	}

	this.setName = function (name) {
		this.element.empty();
		this.element.append(name);
	}
}

TabManager = {

	_tabs: [],
	_activeTab: null,

	selectTab: function (tabObject) {
		if (this._activeTab == tabObject)
			return;

		if (this._activeTab != null)
		{
			this._activeTab.element.removeAttr("id");
			$("#ContentContainer").empty();
		}

		this._activeTab = tabObject;
		tabObject.element.attr("id", "tab_selected");
		$("#ContentContainer").append(tabObject.contentElement);

		/* Timeout so that DOM changes can be finalized before input binding */
		setTimeout(function () {
			/* Need to re-enable the click() callback since input event handlers are removed from
				elements once they're removed from the page's DOM. */
			$(".ProcedureLabel").click(function () {
				$(this).next().slideToggle(200);
			});
		}, 10);
	},

	getTabByName: function (tabName) {
		for (var i = 0; i < this._tabs.length; i++)
		{
			if (this._tabs[i].getName() == tabName)
				return this._tabs[i];
		}

		return null;
	},

	getTabByIndex: function (index) {
		return this._tabs[index];
	},

	allocateTabs: function (count) {
		this._tabs = [];

		for (var i = 0; i < count; i++) {
			var tab = new Tab($("#TabsContainer"));
			tab.setName("Tab " + i);

			this._tabs.push(tab);
		}

		this.selectTab(this._tabs[0]);
	}
};