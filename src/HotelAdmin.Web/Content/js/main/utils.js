
ko.bindingHandlers.modalVisible = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).modal(ko.utils.unwrapObservable(value) ? 'show' : 'hide');
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, show or hide the modal box
        var value = valueAccessor();
        ko.utils.unwrapObservable(value) ? $(element).modal('show') : $(element).modal('hide');
    }
};

Date.prototype.getMonthName = function () {
    return this.toLocaleString().replace(/[^a-z]/gi, '');
};

//n.b. this is sooo not i18n safe :)
Date.prototype.getDayName = function () {
    switch (this.getDay()) {
        case 0: return 'Sunday';
        case 1: return 'Monday';
        case 2: return 'Tuesday';
        case 3: return 'Wednesday';
        case 4: return 'Thursday';
        case 5: return 'Friday';
        case 6: return 'Saturday';
    }
};

String.prototype.padLeft = function (value, size) {
    var x = this;
    while (x.length < size) { x = value + x; }
    return x;
};

Date.prototype.toFormattedString = function (f) {
    var nm = this.getMonthName();
    var nd = this.getDayName();
    f = f.replace(/yyyy/g, this.getFullYear());
    f = f.replace(/yy/g, String(this.getFullYear()).substr(2, 2));
    f = f.replace(/MMM/g, nm.substr(0, 3).toUpperCase());
    f = f.replace(/Mmm/g, nm.substr(0, 3));
    f = f.replace(/MM\*/g, nm.toUpperCase());
    f = f.replace(/Mm\*/g, nm);
    f = f.replace(/mm/g, String(this.getMonth() + 1).padLeft('0', 2));
    f = f.replace(/DDD/g, nd.substr(0, 3).toUpperCase());
    f = f.replace(/Ddd/g, nd.substr(0, 3));
    f = f.replace(/DD\*/g, nd.toUpperCase());
    f = f.replace(/Dd\*/g, nd);
    f = f.replace(/dd/g, String(this.getDate()).padLeft('0', 2));
    f = f.replace(/d\*/g, this.getDate());
    return f;
};