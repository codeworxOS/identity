import { componentFactoryName } from "@angular/compiler";
import { Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { TenantInfo } from './tenant-info';

@Component({
    selector: 'tenant-selector',
    templateUrl: './tenant-selector.component.html'
})
export class TenantSelectorComponent {
    public tenants: TenantInfo[] = [];
    private _selectedTenant: TenantInfo;

    constructor(private oauthService: OAuthService) {
        if (this.oauthService.hasValidAccessToken()) {
            let claims: any = oauthService.getIdentityClaims();
            for (let tenant in claims.tenant) {
                let tenantInfo: TenantInfo = { key: tenant, name: claims.tenant[tenant] }

                this.tenants.push(tenantInfo);

                if (tenant == claims.current_tenant) {
                    this._selectedTenant = tenantInfo;
                }
            }
        }
    }

    public set selectedTenant(value: TenantInfo) {
        this._selectedTenant = value;

        this.oauthService.scope = 'openid profile tenant ' + this._selectedTenant.key;
        this.oauthService.logOut(true);
        this.oauthService.initLoginFlow();
    }

    public get selectedTenant(): TenantInfo {
        return this._selectedTenant;
    }
}