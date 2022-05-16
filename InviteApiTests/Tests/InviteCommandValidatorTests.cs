using FluentValidation.TestHelper;
using InviteApi.Validators;
using InviteApiContract.Commands;
using InviteApiTests.Data;
using Xunit;

namespace InviteApiTests.Tests
{
    public class InviteCommandValidatorTests
    {
        private readonly InviteCommandValidator _validator;

        public InviteCommandValidatorTests()
        {
            _validator = new InviteCommandValidator();
        }

        [Fact]
        public void MessageCorrectTest()
        {
            var invite = new InviteCommandBuilder()
                .Message("Test message")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void MessageIsEmpty_ValidatorRaiseMessageMissingError()
        {
            var invite = new InviteCommandBuilder().Build();

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Message)
                .WithErrorMessage("Invite message is missing");
        }

        [Fact]
        public void MessageContainsCyrillicLetters_Success()
        {
            var invite = new InviteCommandBuilder()
                .Message("message тест")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void MessageContainCharactersNotIncomingIn7BitGsmEncoding_ValidatorRaiseGsmError()
        {
            var invite = new InviteCommandBuilder()
                .Message("手五日")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Message)
                .WithErrorMessage("Invite message should contain only characters in 7 - bit GSM encoding or Cyrillic letters as well");
        }

        [Fact]
        public void MessageLessOrEqual160CharactersToLatin_Success()
        {
            var invite = new InviteCommandBuilder()
                .JoinMessage("a", 160, "")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldNotHaveValidationErrorFor(invite => invite.Message);
        }

        [Fact]
        public void MessageLessOrEqual128CharactersIfNotLatin_Success()
        {
            var invite = new InviteCommandBuilder()
                .JoinMessage("Ñ", 128, "")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldNotHaveValidationErrorFor(invite => invite.Message);
        }

        [Fact]
        public void MessageGreaterOrEqual160CharactersToLatin_ValidatorRaiseTooLongError()
        {
            var invite = new InviteCommandBuilder()
                .JoinMessage("a", 161, "")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Message)
                .WithErrorMessage("Invite message too long, should be less or equal to 128 characters of 7-bit GSM charset");
        }

        [Fact]
        public void MessageGreaterOrEqual128CharactersIfNotLatin_ValidatorRaiseTooLongError()
        {
            var invite = new InviteCommandBuilder()
                .JoinMessage("Ñ", 129, "")
                .Phones("70000000000", "70000000001", "70000000002")
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Message)
                .WithErrorMessage("Invite message too long, should be less or equal to 128 characters of 7-bit GSM charset");
        }

        [Theory]
        [InlineData(new object[] { new string[] { } })]
        [InlineData(null)]
        public void InviteCommandWithoutPhones_ValidatorRaisePhoneMissing(string[] phones)
        {
            var invite = new InviteCommand { Message = "Test", Phones = phones };

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Phones)
                .WithErrorMessage("Phone numbers are missing");
        }

        [Fact]
        public void InviteHave16Phones_Success()
        {
            var invite = new InviteCommandBuilder()
                .Message("Test")
                .Phones(16)
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void InviteCommandHave17Phones_ValidatorRaiseTooMuchPhoneNumbers()
        {
            var invite = new InviteCommandBuilder()
                .Message("Test")
                .Phones(17)
                .Build();

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Phones)
                .WithErrorMessage("Too much phone numbers, should be less or equal to 16 per request");
        }

        [Fact]
        public void InviteCommandContainsDuplicatePhones_ValidatorRaiseDuplicateNumbers()
        {
            var invite = new InviteCommandBuilder()
                .Message("Test")
                .Phones(3)
                .Build();
            invite.Phones[2] = invite.Phones[1];

            var result = _validator.TestValidate(invite);

            result.ShouldHaveValidationErrorFor(invite => invite.Phones)
                .WithErrorMessage("Duplicate numbers detected");
        }

        [Theory]
        [InlineData("80000000000")]
        [InlineData("+70000000000")]
        [InlineData("7000000000")]
        [InlineData("7 000 000000")]
        [InlineData("7(000)000000")]
        [InlineData("7-000-000000")]
        public void PhoneNumbersInvalidFormat_ValidatorRaiseInvalidPhoneFormat(string phone)
        {
            var model = new InviteCommandBuilder()
                .Message("Test")
                .Phones(phone)
                .Build();

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(invite => invite.Phones)      
                .WithErrorMessage("One or several phone numbers do not match with international format");
        }
    }
}
