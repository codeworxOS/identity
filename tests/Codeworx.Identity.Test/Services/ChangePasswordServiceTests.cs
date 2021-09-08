namespace Codeworx.Identity.Test.Services
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Codeworx.Identity.Account;
    using Codeworx.Identity.Model;
    using Codeworx.Identity.Test.Provider;
    using Moq;
    using NUnit.Framework;

    public class ChangePasswordServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _changePasswordServiceMock = new Mock<IChangePasswordService>();
            _changePasswordServiceMock.Setup(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()));

            _testIdentity = new ClaimsIdentity();
            _testIdentity.AddClaim(new Claim(Constants.Claims.Id, Constants.DefaultAdminUserId));
            _testIdentity.AddClaim(new Claim(Constants.Claims.Upn, Constants.DefaultAdminUserName));
            _changeService = new PasswordChangeService(new DummyUserService(), new DummyPasswordValidator(), new DummyPasswordPolicyProvider(), _changePasswordServiceMock.Object);
        }

        private ClaimsIdentity _testIdentity;

        private PasswordChangeService _changeService;

        private Mock<IChangePasswordService> _changePasswordServiceMock;

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
                    new ProcessPasswordChangeRequest(_testIdentity, Constants.DefaultAdminUserName, "1234", "1234")));

            Assert.NotNull(exception);

            var passwordChangeViewResponse = exception.TypedResponse;
            Assert.AreEqual(DummyPasswordPolicyProvider.Description, passwordChangeViewResponse.Error);

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ChangePasswordWithNewPasswordMissmatch_ExpectsError()
        {
            Assert.ThrowsAsync<ErrorResponseException<PasswordChangeViewResponse>>(
                () => _changeService.ProcessChangePasswordAsync(
                    new ProcessPasswordChangeRequest(_testIdentity, Constants.DefaultAdminUserName, "1234", "5647")));

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ChangePasswordWithValidPassword_ExpectsSuccess()
        {
            var returnUrl = "/account/me";
            var prompt = "prompt text";
            var response = await _changeService.ProcessChangePasswordAsync(
                               new ProcessPasswordChangeRequest(_testIdentity, Constants.DefaultAdminUserName, "1111", "1111", returnUrl, prompt));

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
                    new ProcessPasswordChangeRequest(_testIdentity, Constants.DefaultAdminUserName, Constants.DefaultAdminUserName, Constants.DefaultAdminUserName)));

            _changePasswordServiceMock.Verify(x => x.SetPasswordAsync(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}
