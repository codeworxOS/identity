interface ExternalProvider {
    id: string;
    name: string;
    className: string;
    callbackUrl: string;
    url: string;
}

interface UserProfile {
    id: string;
    name: string;
}

class LoginForm {
    private readonly DONE: number = 4;
    private readonly OK: number = 200;

    private userNameControl: HTMLElement;
    private passwordControl: HTMLElement;
    private loginButton: HTMLElement;
    private form: HTMLFormElement;
    private externalProvidersControl: HTMLElement;
    private returnUrl?: string;

    constructor() {
        this.userNameControl = document.getElementById('username');
        this.passwordControl = document.getElementById('password');
        this.externalProvidersControl = document.getElementById('external');

        this.loginButton = document.getElementById('login');
        if (this.loginButton) {
            this.form = (<HTMLButtonElement>this.loginButton).form;
        }
    }

    public initialize(loggedin: boolean = false, returnUrl?: string): void {
        this.returnUrl = returnUrl;
        if (loggedin) {
            this.loadUsername();
        } else {
            this.form.onsubmit = (ev: Event) => this.login(ev);
        }

        this.loadProviders();
    }

    private loadUsername(): void {
        let url = 'me';
        this.getData<UserProfile>(url, p => this.renderUserName(p));
    }

    private renderUserName(profile: UserProfile): void {
        let input = this.userNameControl as HTMLInputElement;
        if (input) {
            input.value = profile.name;
        }
    }

    private loadProviders(username?: string): void {
        let url = 'providers?';
        if (username) {
            url = url + 'username=' + username + '&';
        }

        if (this.returnUrl) {
            url = url + 'returnUrl=' + this.returnUrl;
        }

        this.getData<ExternalProvider[]>(url, p => this.renderProvider(p));
    }

    private renderProvider(data: ExternalProvider[]): any {
        for (let i: number = this.externalProvidersControl.children.length - 1; i >= 0; i--) {
            this.externalProvidersControl.children[i].remove();
        }

        data.forEach(p => {
            let li = document.createElement('li');
            li.nodeValue = p.id;

            var button = document.createElement('button');
            button.type = 'button';
            button.innerText = p.name;
            button.onclick = p => this.navigateExternal(p);
            button.setAttribute('url', p.url);
            button.setAttribute('callback-url', p.callbackUrl);

            li.appendChild(button);

            this.externalProvidersControl.appendChild(li);
        });
    }

    private navigateExternal(ev: Event): any {
        ev = ev || window.event;
        let src: any = ev.target || ev.srcElement;
        let button = <HTMLButtonElement>src;

        var url = button.getAttribute('url');
        var callback = button.getAttribute('callback-url');

        alert(url);
        window.location.href = url;
    }

    private getData<T>(url: string, callback: (data: T) => any): void {
        this.get(url, p => callback(JSON.parse(p.responseText)));
    }

    private postData<T>(url: string, data: any, callback: (data: T) => any): void {
        this.post(url, data, p => callback(JSON.parse(p.responseText)));
    }

    private get(url: string, success: (request: XMLHttpRequest) => any, error?: (request: XMLHttpRequest) => any) {
        return this.request('GET', url, success, error);
    }

    private post(url: string, data: any, success: (request: XMLHttpRequest) => any, error?: (request: XMLHttpRequest) => any) {
        return this.request('POST', url, success, error, data);
    }

    private request(method: string, url: string, success: (request: XMLHttpRequest) => any, error?: (request: XMLHttpRequest) => any, data?: any) {
        var request = new XMLHttpRequest();

        if (data) {
            request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        }

        request.onreadystatechange = (ev: Event) => {
            if (request.readyState == this.DONE) {
                if (request.status == this.OK) {
                    success(request);
                } else {
                    if (error) {
                        error(request);
                    }
                }
            }
        }
        request.open(method, url, true);
        request.send(data);
    }

    private login(ev: Event): boolean {
        alert('login');
        var data = this.serialize(this.form);

        //this.postData<string>(window.location.href, data, p => alert(p));

        return false;
    };

    private serialize(form: HTMLFormElement) {
        var field: any, l, s = [];
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
                    } else if ((field.type != 'checkbox' && field.type != 'radio') || field.checked) {
                        s[s.length] = encodeURIComponent(field.name) + "=" + encodeURIComponent(field.value);
                    }
                }
            }
        }

        return s.join('&').replace(/%20/g, '+');
    }
}