interface TenantInfo {
    key: string;
    name: string;
}

class TenantForm {
    private readonly DONE: number = 4;
    private readonly OK: number = 200;

    private returnUrl?: string;
    private form: HTMLFormElement;
    private tenantDropDown: HTMLSelectElement;

    constructor() {
        this.tenantDropDown = <HTMLSelectElement>document.getElementById('tenantSelection');

        if (this.tenantDropDown) {
            this.form = this.tenantDropDown.form;
        }
    }

    public initialize(showDefault: string, returnUrl?: string): void {
        this.returnUrl = returnUrl;

        if (showDefault === 'True') {
            let div = document.createElement('div');

            let checkbox = document.createElement('input');
            checkbox.id = 'setDefault';
            checkbox.type = 'checkbox';
            checkbox.name = 'setDefault';

            let label = document.createElement('label');
            label.htmlFor = 'setDefault';
            label.innerText = 'Set as default';

            div.appendChild(checkbox);
            div.appendChild(label);

            this.form.append(div);
        }

        this.loadTenants();
    }

    private loadTenants(): void {
        this.getData<Array<TenantInfo>>('tenants', p => this.addToDropDown(p));
    }

    private addToDropDown(tenants: Array<TenantInfo>): void {
        for (let idx in tenants) {
            let element = document.createElement('option');

            element.text = tenants[idx].name;
            element.value = tenants[idx].key;

            this.tenantDropDown.add(element);
        }
    }

    private getData<TData>(url: string, callback: (data: TData) => any): void {
        this.get(url, p => callback(JSON.parse(p.responseText)));
    }

    private get(url: string, success: (request: XMLHttpRequest) => any, error?: (request: XMLHttpRequest) => any) {
        return this.request('GET', url, success, error);
    }

    private request(method: string,
        url: string,
        success: (request: XMLHttpRequest) => any,
        error?: (request: XMLHttpRequest) => any,
        data?: any) {
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
}