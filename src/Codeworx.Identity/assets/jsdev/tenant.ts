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
        this.tenantDropDown = <HTMLSelectElement>document.getElementById('tenants');

        if (this.tenantDropDown) {
            this.form = this.tenantDropDown.form;
        }
    }

    public initialize(returnUrl?: string): void {
        this.returnUrl = returnUrl;

        this.loadTenants();
    }

    private loadTenants(): void {
        this.getData<Array<TenantInfo>>('tenants', p => this.addToDropDown(p));
    }

    private addToDropDown(tenants: Array<TenantInfo>): void {
        for (let idx in tenants) {
            let element = document.createElement('option');

            element.text = tenants[idx].name;

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