﻿namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class CodeDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, ErrorResponse>
    {
        public CodeDuplicatedResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public ErrorResponse Error { get; }
    }
}