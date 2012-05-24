function HistoryItemViewModel(id, name, action) {
    var self = this;

    self.id = ko.observable(id);

    self.name = ko.observable(name);

    self.action = ko.observable(action);
    self.actionString = ko.computed(function () {
        if (self.action() == 0)
            return "New";
        if (self.action() == 1)
            return "Updated";
        if (self.action() == 2)
            return "Deleted";
    });
}

function HistoryDayViewModel(date, items) {

    var self = this;

    self.date = ko.observable(new Date(parseInt(date.substr(6))));
    self.dateString = ko.computed(function () {
        return self.date().toFormattedString('yyyy-mm-dd');
    });

    self.items = ko.observableArray(items);
    
}

function HistoryListViewModel() {
    var self = this;

    self.historyItems = ko.observableArray([]);

    self.refresh = function () {
        $.ajax({
            url: "/api/hotel/history",
            success: function (result, status) {
                var dayItems = $.map(result.Items, function (dayItem) {
                    var items = $.map(dayItem.Items, function (item) {
                        return new HistoryItemViewModel(item.Id, item.HotelName, item.ActionType);
                    });
                    return new HistoryDayViewModel(dayItem.Date, items);
                });
                self.historyItems(dayItems);
            }
        });
    };
};
