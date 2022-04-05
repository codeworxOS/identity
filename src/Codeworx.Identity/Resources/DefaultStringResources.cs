namespace Codeworx.Identity.Resources
{
    public class DefaultStringResources : IStringResources
    {
        public virtual string GetResource(StringResource resource)
        {
            switch (resource)
            {
                case StringResource.UsernameAlreadyTaken:
                    return Translation.UsernameAlreadyTaken;
                case StringResource.Username:
                    return Translation.Username;
                case StringResource.Password:
                    return Translation.Password;
                case StringResource.RememberMe:
                    return Translation.RememberMe;
                case StringResource.ForgotPassword:
                    return Translation.ForgotPassword;
                case StringResource.Login:
                    return Translation.Login;
                case StringResource.ChangePassword:
                    return Translation.ChangePassword;
                case StringResource.ConfirmPassword:
                    return Translation.ConfirmPassword;
                case StringResource.Save:
                    return Translation.Save;
                case StringResource.JavascriptErrorMessage:
                    return Translation.JavascriptErrorMessage;
                case StringResource.Continue:
                    return Translation.Continue;
                case StringResource.OidcFormPostResponsePageTitle:
                    return Translation.OidcFormPostResponsePageTitle;
                case StringResource.Http401PageTitle:
                    return Translation.Http401PageTitle;
                case StringResource.Http401Header:
                    return Translation.Http401Header;
                case StringResource.Http401Description:
                    return Translation.Http401Description;
                case StringResource.ReturnToLogin:
                    return Translation.ReturnToLogin;
                case StringResource.ForgotPasswordPageTitle:
                    return Translation.ForgotPasswordPageTitle;
                case StringResource.ResetPassword:
                    return Translation.ResetPassword;
                case StringResource.ForgotPasswordCompletedMessage:
                    return Translation.ForgotPasswordCompletedMessage;
                case StringResource.AcceptInvitationPageTitle:
                    return Translation.AcceptInvitationPageTitle;
                case StringResource.LoginPageTitle:
                    return Translation.LoginPageTitle;
                case StringResource.ChangePasswordPageTitle:
                    return Translation.ChangePasswordPageTitle;
                case StringResource.SetPasswordPageTitle:
                    return Translation.SetPasswordPageTitle;
                case StringResource.CurrentPassword:
                    return Translation.CurrentPassword;
                case StringResource.NewPassword:
                    return Translation.NewPassword;
                case StringResource.ProfilePageTitle:
                    return Translation.ProfilePageTitle;
                case StringResource.SelectTenantPageTitle:
                    return Translation.SelectTenantPageTitle;
                case StringResource.Select:
                    return Translation.Select;
                case StringResource.SetAsDefault:
                    return Translation.SetAsDefault;
                case StringResource.OrLoginWith:
                    return Translation.OrLoginWith;
                case StringResource.OrConnectWith:
                    return Translation.OrConnectWith;
                case StringResource.SetPassword:
                    return Translation.SetPassword;
                case StringResource.InvitationCodeInvalidError:
                    return Translation.InvitationCodeInvalidError;
                case StringResource.InvitationCodeExpiredError:
                    return Translation.InvitationCodeExpiredError;
                case StringResource.InvitationCodeRedeemedError:
                    return Translation.InvitationCodeRedeemedError;
                case StringResource.UnknownLoginProviderError:
                    return Translation.UnknownLoginProviderError;
                case StringResource.InvitationNotSupportedError:
                    return Translation.InvitationNotSupportedError;
                case StringResource.ExternalAccountAlreadyLinkedError:
                    return Translation.ExternalAccountAlreadyLinkedError;
                case StringResource.ExternalAccountNotLinkedError:
                    return Translation.ExternalAccountNotLinkedError;
                case StringResource.UserForClientP0NotFoundError:
                    return Translation.UserForClientP0NotFoundError;
                case StringResource.DefaultAuthenticationError:
                    return Translation.DefaultAuthenticationError;
                case StringResource.GenericLoginError:
                    return Translation.GenericLoginError;
                case StringResource.PasswordChangeWrongPasswordError:
                    return Translation.PasswordChangeWrongPasswordError;
                case StringResource.PasswordChangeNotMatchingError:
                    return Translation.PasswordChangeNotMatchingError;
                case StringResource.PasswordChangeSamePasswordError:
                    return Translation.PasswordChangeSamePasswordError;
                case StringResource.PasswordChangeEqualToLoginError:
                    return Translation.PasswordChangeEqualToLoginError;
                case StringResource.PasswordChangePasswordReuseError:
                    return Translation.PasswordChangePasswordReuseError;
                case StringResource.LanguageCode:
                    return Translation.LanguageCode;
                case StringResource.MaxFailedLoginAttemptsReached:
                    return Translation.MaxFailedLoginAttemptsReached;
                case StringResource.ConfirmationCodeInvalid:
                    return Translation.ConfirmationCodeInvalid;
                case StringResource.ConfirmationPageTitle:
                    return Translation.ConfirmationPageTitle;
                case StringResource.ConfirmationMessage:
                    return Translation.ConfirmationMessage;
                case StringResource.AccountConfirmationPending:
                    return Translation.AccountConfirmationPending;
                case StringResource.EmailLoginDescription:
                    return Translation.EmailLoginDescription;
                default:
                    throw new MissingResourceException(resource);
            }
        }
    }
}