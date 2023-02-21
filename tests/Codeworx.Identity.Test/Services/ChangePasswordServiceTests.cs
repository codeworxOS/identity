namespace Codeworx.Identity.Test.Services
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Codeworx.Identity.Account;
    using Codeworx.Identity.Configuration;
    using Codeworx.Identity.Model;
    using Codeworx.Identity.Resources;
    using Codeworx.Identity.Test.Provider;
    using Microsoft.Extensions.Options;
    using Moq;
    using NUnit.Framework;

    public class ChangePasswordServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _changePasswordServiceMock = new Mock<IChangePasswordService>();
            _changePasswordServiceMock.Setup(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()));

            _optionsSnapshotMock = new Mock<IOptionsSnapshot<IdentityOptions>>();
            _optionsSnapshotMock.Setup(p => p.Value).Returns(new IdentityOptions());

            _testIdentity = new ClaimsIdentity();
            _testIdentity.AddClaim(new Claim(Constants.Claims.Id, TestConstants.Users.DefaultAdmin.UserId));
            _testIdentity.AddClaim(new Claim(Constants.Claims.Upn, TestConstants.Users.DefaultAdmin.UserName));

            _testIdentityWithoutPassword = new ClaimsIdentity();
            _testIdentityWithoutPassword.AddClaim(new Claim(Constants.Claims.Id, TestConstants.Users.NoPassword.UserId));
            _testIdentityWithoutPassword.AddClaim(new Claim(Constants.Claims.Upn, TestConstants.Users.NoPassword.UserName));

            _changeService = new PasswordChangeService(new DummyUserService(), new DummyPasswordValidator(), new DummyPasswordPolicyProvider(), new DefaultStringResources(), _optionsSnapshotMock.Object, _changePasswordServiceMock.Object);
        }

        private ClaimsIdentity _testIdentity;
        private ClaimsIdentity _testIdentityWithoutPassword;

        private PasswordChangeService _changeService;

        private Mock<IChangePasswordService> _changePasswordServiceMock;
        private Mock<IOptionsSnapshot<IdentityOptions>> _optionsSnapshotMock;

        [Test]
        public void ChangePasswordWithInvalidPassword_ExpectsError()
        {
            Assert.ThrowsAsync<ErrorResponseException<PasswordChangeViewResponse>>(
                () => _changeService.ProcessChangePasswordAsync(
                    new ProcessPasswordChangeRequest(_testIdentity, "invalidPassword", "", "")));

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ChangePasswordWithInvalidNewPassword_ExpectsError()
        {
            var exception = Assert.ThrowsAsync<ErrorResponseException<PasswordChangeViewResponse>>(
                () => _changeService.ProcessChangePasswordAsync(
                    new ProcessPasswordChangeRequest(_testIdentity, TestConstants.Users.DefaultAdmin.Password, "1234", "1234")));

            Assert.NotNull(exception);

            var passwordChangeViewResponse = exception.TypedResponse;
            Assert.AreEqual(DummyPasswordPolicyProvider.Description["en"], passwordChangeViewResponse.Error);

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ChangePasswordWithNewPasswordMissmatch_ExpectsError()
        {
            Assert.ThrowsAsync<ErrorResponseException<PasswordChangeViewResponse>>(
                () => _changeService.ProcessChangePasswordAsync(
                    new ProcessPasswordChangeRequest(_testIdentity, TestConstants.Users.DefaultAdmin.Password, "1234", "5647")));

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ChangePasswordWithValidPassword_ExpectsSuccess()
        {
            var returnUrl = "/account/me";
            var prompt = "prompt text";
            var response = await _changeService.ProcessChangePasswordAsync(
                               new ProcessPasswordChangeRequest(_testIdentity, TestConstants.Users.DefaultAdmin.Password, "1111", "1111", returnUrl, prompt));

            Assert.NotNull(response);
            Assert.AreEqual(returnUrl, response.ReturnUrl);
            Assert.AreEqual(prompt, response.Prompt);

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ChangePasswordWithSamePassword_ExpectsError()
        {
            Assert.ThrowsAsync<ErrorResponseException<PasswordChangeViewResponse>>(
                () => _changeService.ProcessChangePasswordAsync(
                    new ProcessPasswordChangeRequest(_testIdentity, TestConstants.Users.DefaultAdmin.Password, TestConstants.Users.DefaultAdmin.Password, TestConstants.Users.DefaultAdmin.Password)));

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task SetPasswordWithValidPassword_ExpectsSuccess()
        {
            var returnUrl = "/account/me";
            var prompt = "prompt text";
            var response = await _changeService.ProcessChangePasswordAsync(
                               new ProcessPasswordChangeRequest(_testIdentityWithoutPassword, null, "1111", "1111", returnUrl, prompt));

            Assert.NotNull(response);
            Assert.AreEqual(returnUrl, response.ReturnUrl);
            Assert.AreEqual(prompt, response.Prompt);

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SetPasswordWithNewPasswordMissmatch_ExpectsError()
        {
            Assert.ThrowsAsync<ErrorResponseException<PasswordChangeViewResponse>>(
                () => _changeService.ProcessChangePasswordAsync(
                    new ProcessPasswordChangeRequest(_testIdentityWithoutPassword, null, "1234", "5647")));

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}
