using System;
using System.Collections.Generic;
using System.Linq;

namespace InviteApiContract.Commands
{
    public static class InviteErrors
    {
        private static readonly IReadOnlyDictionary<int, string> s_errors = new Dictionary<int, string>()
        {
            { 400, "One or several phone numbers do not match with international format" },
            { 401, "Phone numbers are missing" },
            { 402, "Too much phone numbers, should be less or equal to 16 per request" },
            { 403, "Too much phone numbers, should be less or equal to 128 per day" },
            { 404, "Duplicate numbers detected" },
            { 405, "Invite message is missing" },
            { 406, "Invite message should contain only characters in 7 - bit GSM encoding or Cyrillic letters as well" },
            { 407, "Invite message too long, should be less or equal to 128 characters of 7-bit GSM charset" },
        };

        private static readonly IReadOnlyDictionary<string, int> s_messages = new Dictionary<string, int>()
        {
            { "One or several phone numbers do not match with international format", 400 },
            { "Phone numbers are missing", 401 },
            { "Too much phone numbers, should be less or equal to 16 per request", 402 },
            { "Too much phone numbers, should be less or equal to 128 per day", 403 },
            { "Duplicate numbers detected", 404 },
            { "Invite message is missing", 405 },
            { "Invite message should contain only characters in 7 - bit GSM encoding or Cyrillic letters as well", 406 },
            { "Invite message too long, should be less or equal to 128 characters of 7-bit GSM charset", 407 }
        };

        public static IReadOnlyDictionary<int, string> Errors => s_errors;

        public static string GetErrorMessage(InviteErrorCodes errorType)
        {
            return Errors[(int)errorType];
        }
        public static int GetErrorCode(string errorMessage)
        {
            if (s_messages.TryGetValue(errorMessage, out int code))
            {
                return code;
            }
            else
            {
                return 400;
            }
        }
    }
    
    public enum InviteErrorCodes
    {
        InvalidPhoneNumberFormat = 400,
        EmptyPhoneNumber = 401,
        TooMuchPhones = 402,
        TooMuchInvitePerDay = 403,
        DuplicateNumbers = 404,
        MessageIsMissing = 405,
        InvalidMessageFormat = 406,
        InviteMessageTooLong = 407
    }
}
