
function FactTypeViewModel(id, name, code, isNew) {
    var self = this;

    self.id = ko.observable(id);

    self.isNew = ko.observable(isNew);
    self.isDirty = ko.observable(false);
    self.isUpdating = ko.observable(false);

    self.name = ko.observable(name);
    self.name.subscribe(function (val) {
        self.isDirty(true);
    });

    self.code = ko.observable(code);
    self.code.subscribe(function (val) {
        self.isDirty(true);
    });

    self.submitActionName = ko.computed(function () {
        if (self.isNew()) {
            return "Add";
        } else {
            return "Update";
        }
    });

    self.onUpdated = function () {
    };

    self.update = function () {
        self.isUpdating(true);

        $.ajax({
            url: "/api/facttype/" + self.id(),
            type: "POST",
            contentType: "application/json",
            data: ko.toJSON(self),
            success: function (result, status) {
                self.id(result.FactTypeId);
                self.isDirty(false);
                self.isUpdating(false);
                self.isNew(false);

                self.onUpdated();
            }
        });
    };
};

function FactTypeListViewModel() {
    var self = this;

    self.factTypes = ko.observableArray([]);
    self.selectedFactType = ko.observable();

    self.unselect = function () {
        self.selectedFactType(undefined);
    };

    self.addFactType = function () {
        var factType = self.buildFactType(0, '', '', true);
        self.factTypes.push(factType);
        self.selectedFactType(factType);
    };

    self.buildFactType = function(id, name, code, isNew) {
        var factType = new FactTypeViewModel(id, name, code, isNew);

        factType.select = function () {
            self.selectedFactType(factType);
        };
        factType.onUpdated = function () {
            self.selectedFactType(undefined);
        };
        return factType;
    };

    self.refresh = function () {
        $.ajax({
            url: "/api/facttype",
            success: function (result, status) {
                var factsList = $.map(result, function (item, no) {
                    return self.buildFactType(item.Id, item.Name, item.Code, false);
                });
                self.factTypes(factsList);
            }
        });
    };

};
