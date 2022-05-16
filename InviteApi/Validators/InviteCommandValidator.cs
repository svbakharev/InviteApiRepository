using System.Linq;
using Cyrillic.Convert;
using FluentValidation;
using System.Text;
using System.Text.RegularExpressions;
using InviteApiContract.Commands;

namespace InviteApi.Validators
{
    public class InviteCommandValidator : AbstractValidator<InviteCommand>
    {
        private const string PhonePattern = @"^7\d{10}$";
        public InviteCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(invite => invite.Message).NotEmpty()
                .WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.MessageIsMissing));

            Transform(from: invite => invite.Message, to: originalMessage => originalMessage.ToRussianLatin())
                .Must(message => HasOnlyGsmCharacters(message))
                .WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.InvalidMessageFormat))
                .DependentRules(() =>
                {
                    When(invite => !Regex.IsMatch(invite.Message, @"\P{IsBasicLatin}"), () =>
                    {
                        RuleFor(invite => invite.Message)
                            .MaximumLength(160)
                            .WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.InviteMessageTooLong));
                    }).Otherwise(() =>
                    {
                        RuleFor(invite => invite.Message)
                            .MaximumLength(128)
                            .WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.InviteMessageTooLong));
                    });
                });

            RuleFor(invite => invite.Phones)
                .NotEmpty().WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.EmptyPhoneNumber))
                .Must(phones => phones.Length > 0).WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.EmptyPhoneNumber))
                .Must(phones => phones.Length <= 16).WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.TooMuchPhones))
                .Must(phones => phones.Length == phones.Distinct().Count()).WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.DuplicateNumbers))
                .DependentRules(() =>
                {
                    RuleForEach(invite => invite.Phones)
                        .Matches(PhonePattern).WithMessage(InviteErrors.GetErrorMessage(InviteErrorCodes.InvalidPhoneNumberFormat));
                });
        }

        private bool HasOnlyGsmCharacters(string message)
        {
            Encoding gsmEnc = new Mediaburst.Text.GSMEncoding();
            byte[] gsmEncBytes = gsmEnc.GetBytes(message);
            string actualMessage = gsmEnc.GetString(gsmEncBytes);
            return actualMessage == message;
        }
    }
}
