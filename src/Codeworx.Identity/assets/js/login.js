var LoginForm = /** @class */ (function () {
    function LoginForm() {
        this.DONE = 4;
        this.OK = 200;
        this.userNameControl = document.getElementById('username');
        this.passwordControl = document.getElementById('password');
        this.externalProvidersControl = document.getElementById('external');
        this.loginButton = document.getElementById('login');
        if (this.loginButton) {
            this.form = this.loginButton.form;
        }
    }
    LoginForm.prototype.initialize = function (loggedin, returnUrl) {
        var _this = this;
        if (loggedin === void 0) { loggedin = false; }
        this.returnUrl = returnUrl;
        if (loggedin) {
            this.loadUsername();
        }
        else {
            this.form.onsubmit = function (ev) { return _this.login(ev); };
        }
        this.loadProviders();
    };
    LoginForm.prototype.loadUsername = function () {
        var _this = this;
        var url = 'me';
        this.getData(url, function (p) { return _this.renderUserName(p); });
    };
    LoginForm.prototype.renderUserName = function (profile) {
        var input = this.userNameControl;
        if (input) {
            input.value = profile.name;
        }
    };
    LoginForm.prototype.loadProviders = function (username) {
        var _this = this;
        var url = 'providers?';
        if (username) {
            url = url + 'username=' + username + '&';
        }
        if (this.returnUrl) {
            url = url + 'returnUrl=' + this.returnUrl;
        }
        this.getData(url, function (p) { return _this.renderProvider(p); });
    };
    LoginForm.prototype.renderProvider = function (data) {
        var _this = this;
        for (var i = this.externalProvidersControl.children.length - 1; i >= 0; i--) {
            this.externalProvidersControl.children[i].remove();
        }
        data.forEach(function (p) {
            var li = document.createElement('li');
            li.nodeValue = p.id;
            var button = document.createElement('button');
            button.type = 'button';
            button.innerText = p.name;
            button.onclick = function (p) { return _this.navigateExternal(p); };
            button.setAttribute('url', p.url);
            button.setAttribute('callback-url', p.callbackUrl);
            li.appendChild(button);
            _this.externalProvidersControl.appendChild(li);
        });
    };
    LoginForm.prototype.navigateExternal = function (ev) {
        ev = ev || window.event;
        var src = ev.target || ev.srcElement;
        var button = src;
        var url = button.getAttribute('url');
        var callback = button.getAttribute('callback-url');
        alert(url);
        window.location.href = url;
    };
    LoginForm.prototype.getData = function (url, callback) {
        this.get(url, function (p) { return callback(JSON.parse(p.responseText)); });
    };
    LoginForm.prototype.postData = function (url, data, callback) {
        this.post(url, data, function (p) { return callback(JSON.parse(p.responseText)); });
    };
    LoginForm.prototype.get = function (url, success, error) {
        return this.request('GET', url, success, error);
    };
    LoginForm.prototype.post = function (url, data, success, error) {
        return this.request('POST', url, success, error, data);
    };
    LoginForm.prototype.request = function (method, url, success, error, data) {
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
    LoginForm.prototype.login = function (ev) {
        alert('login');
        var data = this.serialize(this.form);
        this.postData(window.location.href, data, function (p) { return alert(p); });
        return false;
    };
    LoginForm.prototype.serialize = function (form) {
        var field, l, s = [];
        if (typeof form == 'object' && form.nodeName == "FORM") {
            var len = form.elements.length;
            for (var i = 0; i < len; i++) {
                field = form.elements[i];
                if (field.name && !field.disabled && field.type != 'file' && field.type != 'reset' && field.type != 'submit' && field.type != 'button') {
                    if (field.type == 'select-multiple') {
                        l = field.options.length;
                        for (var j = 0; j < l; j++) {
                            if (field.options[j].selected)
                                s[s.length] = encodeURIComponent(field.name) + "=" + encodeURIComponent(field.options[j].value);
                        }
                    }
                    else if ((field.type != 'checkbox' && field.type != 'radio') || field.checked) {
                        s[s.length] = encodeURIComponent(field.name) + "=" + encodeURIComponent(field.value);
                    }
                }
            }
        }
        return s.join('&').replace(/%20/g, '+');
    };
    return LoginForm;
}());
