var TenantForm = (function () {
    function TenantForm() {
        this.DONE = 4;
        this.OK = 200;
        this.tenantDropDown = document.getElementById('tenantSelection');
        if (this.tenantDropDown) {
            this.form = this.tenantDropDown.form;
        }
    }
    TenantForm.prototype.initialize = function (showDefault, returnUrl) {
        this.returnUrl = returnUrl;
        if (showDefault === 'True') {
            var div = document.createElement('div');
            var checkbox = document.createElement('input');
            checkbox.id = 'setDefault';
            checkbox.type = 'checkbox';
            checkbox.name = 'setDefault';
            var label = document.createElement('label');
            label.htmlFor = 'setDefault';
            label.innerText = 'Set as default';
            div.appendChild(checkbox);
            div.appendChild(label);
            this.form.append(div);
        }
        this.loadTenants();
    };
    TenantForm.prototype.loadTenants = function () {
        var _this = this;
        this.getData('tenants', function (p) { return _this.addToDropDown(p); });
    };
    TenantForm.prototype.addToDropDown = function (tenants) {
        for (var idx in tenants) {
            var element = document.createElement('option');
            element.text = tenants[idx].name;
            element.value = tenants[idx].key;
            this.tenantDropDown.add(element);
        }
    };
    TenantForm.prototype.getData = function (url, callback) {
        this.get(url, function (p) { return callback(JSON.parse(p.responseText)); });
    };
    TenantForm.prototype.get = function (url, success, error) {
        return this.request('GET', url, success, error);
    };
    TenantForm.prototype.request = function (method, url, success, error, data) {
        var _this = this;
        var request = new XMLHttpRequest();
        if (data) {
            request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        }
        request.onreadystatechange = function (ev) {
            if (request.readyState == _this.DONE) {
                if (request.status == _this.OK) {
                    success(request);
                }
                else {
                    if (error) {
                        error(request);
                    }
                }
            }
        };
        request.open(method, url, true);
        request.send(data);
    };
    return TenantForm;
}());
