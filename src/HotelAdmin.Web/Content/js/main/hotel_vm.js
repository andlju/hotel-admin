
function HotelViewModel(id, name, resortName, description, isNew, facts) {
    var self = this;

    self.id = ko.observable(id);

    self.isNew = ko.observable(isNew);
    self.isDirty = ko.observable(false);

    self.name = ko.observable(name);
    self.name.subscribe(function(val) {
        self.isDirty(true);
    });

    self.resortName = ko.observable(resortName);

    self.description = ko.observable(description);
    self.description.subscribe(function(val) {
        self.isDirty(true);
    });

    self.facts = ko.observableArray([]);
    self.factsAreDirty = ko.computed(function () {
        for (var i = 0; i < self.facts().length; i++) {
            if (self.facts()[i].isDirty())
                return true;
        }
        return false;
    });
    self.factsAreDirty.subscribe(function (val) {
        if (val) {
            self.isDirty(true);   
        }
    });
    
    if (facts) {
        self.facts($.map(facts, function(item, no) {
            return new HotelFactViewModel(item.Id, item.Code, item.Name, item.Value, item.Details);
        }));
    }

    self.newFact = ko.observable(new NewHotelFactViewModel());

    self.addNewFact = function () {
        var factType = self.newFact().factType();
        self.facts.push(new HotelFactViewModel(factType.id(), factType.code(), factType.name(), self.newFact().value(), self.newFact().details()));
        self.newFact(new NewHotelFactViewModel());
        self.isDirty(true);
    };

    self.deleteFact = function (fact) {
        self.facts.remove(fact);
    };
    
    self.onUpdated = function() {
    };

    self.update = function () {
        $.ajax({
            url: "/api/hotel/" + self.id(),
            type: "POST",
            contentType: "application/json",
            data: ko.toJSON(self),
            success: function (result, status) {
                self.id(result.HotelId);

                self.isDirty(false);
                for (var i = 0; i < self.facts().length; i++) {
                    self.facts()[i].isDirty(false);
                }
                self.isNew(false);

                self.onUpdated();
            }
        });
    };
};

function HotelFactViewModel(id, code, name, value, details) {
    var self = this;

    self.isDirty = ko.observable(false);
    
    self.id = ko.observable(id);

    self.code = ko.observable(code);

    self.name = ko.observable(name);
    
    self.value = ko.observable(Boolean(value));
    self.value.subscribe(function (val) {
        self.isDirty(true);
    });

    self.details = ko.observable(details);
    self.details.subscribe(function (val) {
        self.isDirty(true);
    });
}

function NewHotelFactViewModel() {
    var self = this;

    self.factType = ko.observable();

    self.value = ko.observable(true);

    self.details = ko.observable();
}

function QueryFactViewModel(id, code, name, value) {
    var self = this;

    self.id = ko.observable(id);

    self.code = ko.observable(code);

    self.name = ko.observable(name);

    self.value = ko.observable(Boolean(value));
    
}

function HotelListViewModel() {
    var self = this;

    self.searchResult = ko.observableArray([]);
    self.query = ko.observable();
    self.query.subscribe(function (value) {
        // Changing the Query? You'll want to go back to the first page too.
        self.pageNo(0);
    });

    self.pageNo = ko.observable(0);
    self.pageNo.subscribe(function () {
        self.search();
    });

    self.totalPages = ko.observable(0);

    self.allPages = ko.computed(function () {
        var arr = new Array();
        var startPage = Math.max(Math.min(self.totalPages() - 4, self.pageNo() - 2), 0);
        var endPage = Math.min(self.totalPages(), startPage + 4);

        for (var i = startPage; i <= endPage; i++) {
            arr.push(i);
        }
        return arr;
    });

    self.delayedQuery = ko.computed(self.query).extend({ throttle: 500 });

    self.delayedQuery.subscribe(function (val) {
        self.doSearch(val, self.pageNo(), []);
    }, this);

    self.selectedHotelViewModel = ko.observable();

    self.unselect = function () {
        self.selectedHotelViewModel(undefined);
    };

    self.addNewHotel = function () {
        var newHotel = self.createHotelViewModel(0, '', '', '', true);
        self.searchResult.push(newHotel);
        newHotel.select();
    };
    
    self.peekHotelViewModel = ko.observable();

    self.unpeek = function () {
        self.peekHotelViewModel(undefined);
    };

    self.queryFacts = ko.observableArray([]);

    self.selectedQueryFactCodes = ko.computed(function () {
        var arr = new Array();
        for (var i = 0; i < self.queryFacts().length; i++) {
            var fact = self.queryFacts()[i];
            if (fact.value()) {
                arr.push(fact.code());
            }
        }
        return arr;
    });

    self.selectedQueryFactCodes.subscribe(function (val) {
        // When filter is changed, search again
        self.pageNo(0);
        self.search();
    });
    
    self.refreshQueryFacts = function () {
        $.ajax({
            url: "/api/facttype",
            success: function(result, status) {
                var factsList = $.map(result, function(item, no) {
                    return new QueryFactViewModel(item.Id, item.Code, item.Name, item.Value);
                });
                self.queryFacts(factsList);
            }
        });
    };
    
    self.doSearch = function (query, page, queryFacts) {
        $.ajax({
            url: "/api/hotel",
            data: {
                "q": query,
                "p": page,
                "f": queryFacts.join(",")
            },
            success: function (result, status) {

                self.totalPages(result.TotalNumberOfPages);

                var hotelList = $.map(result.Items, function (item, no) {
                    return self.createHotelViewModel(item.Id, item.Name, item.ResortName, item.Description, false, item.Facts);
                });
                self.searchResult(hotelList);
            }
        });
    };

    self.createHotelViewModel = function(id, name, resortName, description, isNew, facts) {
        var hotelViewModel = new HotelViewModel(id, name, resortName, description, isNew, facts);

        hotelViewModel.select = function () {
            self.selectedHotelViewModel(hotelViewModel);
        };

        hotelViewModel.peek = function () {
            self.peekHotelViewModel(hotelViewModel);
        };

        hotelViewModel.onUpdated = function () {
            self.selectedHotelViewModel(undefined);
        };

        return hotelViewModel;
    };

    self.search = function () {

        self.doSearch(self.query(), self.pageNo(), self.selectedQueryFactCodes());
    };
};
