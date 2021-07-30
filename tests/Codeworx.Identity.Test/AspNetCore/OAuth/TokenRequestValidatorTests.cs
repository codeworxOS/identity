// TODO fix
////using System;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.OAuth.Validation.Token;
////using NUnit.Framework;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class TokenRequestValidatorTests
////    {
////        [Test]
////        public async Task IsValid_ValidRequest_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_ClientIdEmpty_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().WithClientId(string.Empty)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_ClientIdNull_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().WithClientId(null)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_ClientIdInvalid_ReturnsError()
////        {
////            var request = new TokenRequestBuilder().WithClientId("\u0019")
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.IsType<ClientIdInvalidResult>(result);
////        }

////        [Test]
////        public async Task IsValid_GrantTypeEmpty_ReturnsError()
////        {
////            var request = new TokenRequestBuilder().WithGrantType(string.Empty)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.IsType<GrantTypeInvalidResult>(result);
////        }

////        [Test]
////        public async Task IsValid_GrantTypeNull_ReturnsError()
////        {
////            var request = new TokenRequestBuilder().WithGrantType(null)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.IsType<GrantTypeInvalidResult>(result);
////        }

////        [Test]
////        public async Task IsValid_GrantTypeInvalid_ReturnsError()
////        {
////            var request = new TokenRequestBuilder().WithGrantType("ä")
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.IsType<GrantTypeInvalidResult>(result);
////        }

////        [Test]
////        public async Task IsValid_RedirectUriEmpty_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().WithRedirectUri(string.Empty)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_RedirectUriNull_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().WithRedirectUri(null)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_RedirectUriInvalid_ReturnsError()
////        {
////            var request = new TokenRequestBuilder().WithRedirectUri("ä")
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.IsType<RedirectUriInvalidResult>(result);
////        }

////        [Test]
////        public async Task IsValid_ClientSecretEmpty_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().WithClientSecret(string.Empty)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_ClientSecretNull_ReturnsNoError()
////        {
////            var request = new TokenRequestBuilder().WithClientSecret(null)
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Test]
////        public async Task IsValid_ClientSecretInvalid_ReturnsError()
////        {
////            var request = new TokenRequestBuilder().WithClientSecret("\u0019")
////                                                   .Build();

////            var instance = new TokenRequestValidator();

////            var result = await instance.IsValid(request);

////            Assert.IsType<ClientSecretInvalidResult>(result);
////        }
////    }
////}
