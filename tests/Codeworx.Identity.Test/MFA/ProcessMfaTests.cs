using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Mfa.Totp;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OtpNet;
using static Codeworx.Identity.Test.DummyUserService;

namespace Codeworx.Identity.Test.MFA
{
    public class ProcessMfaTests
    {
        [Test]
        public async Task GetMfaView_MfaRegistration_ExpectsLoginPage()
        {
            var sp = CreateDefaultServiceProvider();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(Constants.Claims.Id, TestConstants.Users.MfaTestUserWithMfaRequired.UserId));
            var mfaLoginRequest = new MfaLoginRequest(claimsIdentity, false, TestConstants.LoginProviders.TotpProvider.Id, null, false);

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaViewResponse = await mfaViewService.ShowLoginAsync(mfaLoginRequest);

            var registration = mfaViewResponse.Info;
            Assert.AreEqual(TotpConstants.Templates.LoginTotp, registration.Template);
        }

        [Test]
        public async Task GetMfaView_NoMfaRegistration_ExpectsRegisterPage()
        {
            var sp = CreateDefaultServiceProvider();

            var dummyUserService = (DummyUserService)sp.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.RemoveMfaRegistration();

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(Constants.Claims.Id, TestConstants.Users.MfaTestUserWithMfaRequired.UserId));
            var mfaLoginRequest = new MfaLoginRequest(claimsIdentity, false, TestConstants.LoginProviders.TotpProvider.Id, null, false);

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaViewResponse = await mfaViewService.ShowLoginAsync(mfaLoginRequest);

            var registration = mfaViewResponse.Info;
            Assert.AreEqual(TotpConstants.Templates.RegisterTotp, registration.Template);
        }

        [Test]
        public async Task PerformRegistration_NotRegisteredBefore_ExpectsMfaRegistration()
        {
            var sp = CreateDefaultServiceProvider();

            var dummyUserService = (DummyUserService)sp.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.RemoveMfaRegistration();

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaLoginRequest = CreateRegistrationParameters(TestConstants.Users.MfaTestUserWithMfaRequired.UserId, TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret);

            var mfaViewResponse = await mfaViewService.ProcessLoginAsync(mfaLoginRequest);

            Assert.IsTrue(mfaTestUser.HasMfaRegistration, "MFA registration");
        }

        [Test]
        public void PerformRegistration_RegisteredBefore_ExpectsException()
        {
            var sp = CreateDefaultServiceProvider();

            var dummyUserService = (DummyUserService)sp.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.RemoveMfaRegistration();

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();

            var mfaLoginRequest = CreateLoginParameters(TestConstants.Users.MfaTestUserWithMfaRequired.UserId, TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret);

            Assert.ThrowsAsync<AuthenticationException>(async () => await mfaViewService.ProcessLoginAsync(mfaLoginRequest));
        }

        [Test]
        public void PerformRegistration_WrongOneTimeCode_ExpectsException()
        {
            var sp = CreateDefaultServiceProvider();

            var dummyUserService = (DummyUserService)sp.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.RemoveMfaRegistration();

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaLoginRequest = CreateRegistrationParameters(TestConstants.Users.MfaTestUserWithMfaRequired.UserId, TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, useWrongOneTimeCode: true);

            Assert.ThrowsAsync<AuthenticationException>(async () => await mfaViewService.ProcessLoginAsync(mfaLoginRequest));
        }

        [Test]
        public async Task PerformRegistration_WrongSharedSecret_ExpectsException()
        {
            await Task.Yield();
            var sp = CreateDefaultServiceProvider();

            var dummyUserService = (DummyUserService)sp.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.RemoveMfaRegistration();

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaLoginRequest = CreateRegistrationParameters(TestConstants.Users.MfaTestUserWithMfaRequired.UserId, "sharedSecret", useWrongSharedSecred: true);

            Assert.ThrowsAsync<AuthenticationException>(async () => await mfaViewService.ProcessLoginAsync(mfaLoginRequest));
        }

        [Test]
        public void PerformRegistration_UseLoginParameters_ExpectsException()
        {
            var sp = CreateDefaultServiceProvider();

            var dummyUserService = (DummyUserService)sp.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.RemoveMfaRegistration();

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaLoginRequest = CreateLoginParameters(TestConstants.Users.MfaTestUserWithMfaRequired.UserId, TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret);

            Assert.ThrowsAsync<AuthenticationException>(async () => await mfaViewService.ProcessLoginAsync(mfaLoginRequest));
        }

        [Test]
        public void PerformLogin_UseRegistrationParameters_ExpectsException()
        {
            var sp = CreateDefaultServiceProvider();

            var mfaViewService = sp.GetRequiredService<IMfaViewService>();
            var mfaLoginRequest = CreateRegistrationParameters(TestConstants.Users.MfaTestUserWithMfaRequired.UserId, TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret);

            Assert.ThrowsAsync<AuthenticationException>(async () => await mfaViewService.ProcessLoginAsync(mfaLoginRequest));
        }

        private ServiceProvider CreateDefaultServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity()
                    .AddMfaTotp()
                    .UseTestSetup()
                    .LoginRegistrations<DummyLoginRegistrationProviderWithTotp>();
            var sp = services.BuildServiceProvider();
            return sp;
        }

        private MfaProcessLoginRequest CreateRegistrationParameters(string userId, string sharedSecret, bool useWrongOneTimeCode = false, bool useWrongSharedSecred = false)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(Constants.Claims.Id, userId));

            var oneTimeCode = "123456";
            if (!useWrongOneTimeCode)
            {
                var key = Mfa.Totp.Base32Encoding.ToBytes(useWrongSharedSecred ? "wrongSharedSecret" : sharedSecret);
                var otpProvider = new Totp(key);
                oneTimeCode = otpProvider.ComputeTotp();
            }

            var parameters = new TotpLoginRequest(
                TestConstants.LoginProviders.TotpProvider.Id,
                claimsIdentity,
                MfaAction.Register,
                null,
                oneTimeCode,
                sharedSecret);
            var mfaLoginRequest = new MfaProcessLoginRequest(TestConstants.LoginProviders.TotpProvider.Id, parameters, claimsIdentity, false, null, false);
            return mfaLoginRequest;
        }

        private MfaProcessLoginRequest CreateLoginParameters(string userId, string sharedSecret, bool useWrongOneTimeCode = false)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(Constants.Claims.Id, userId));

            var oneTimeCode = "123456";
            if (!useWrongOneTimeCode)
            {
                var key = Mfa.Totp.Base32Encoding.ToBytes(sharedSecret);
                var otpProvider = new Totp(key);
                oneTimeCode = otpProvider.ComputeTotp();
            }

            var parameters = new TotpLoginRequest(
                TestConstants.LoginProviders.TotpProvider.Id,
                claimsIdentity,
                MfaAction.Login,
                null,
                oneTimeCode);
            var mfaLoginRequest = new MfaProcessLoginRequest(TestConstants.LoginProviders.TotpProvider.Id, parameters, claimsIdentity, false, null, false);
            return mfaLoginRequest;
        }
    }
}
