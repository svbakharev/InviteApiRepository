using InviteApiContract.Commands;
using System;
using System.Linq;


namespace InviteApiTests.Data
{
    public class InviteCommandBuilder
    {
        private readonly InviteCommand _invite = new()
        {
            Message = "",
            Phones = Array.Empty<String>()
        };

        public InviteCommandBuilder Message(string message)
        {
            _invite.Message = message;
            return this;
        }

        public InviteCommandBuilder JoinMessage(string message, int repeat, string separator)
        {
            if (repeat < 1)
            {
                throw new ArgumentException($"{nameof(repeat)} should be greater or equal 1");
            }

            _invite.Message += String.Join(
                separator,
                Enumerable.Range(1, repeat).Select(i => message).ToArray());
            return this;
        }

        public InviteCommandBuilder Phones(params string[] phones)
        {
            _invite.Phones = phones;
            return this;
        }

        public InviteCommandBuilder Phones(int count)
        {
            var phones = new string[count];
            for (int i = 0; i < phones.Length; i++)
            {
                phones[i] = string.Format("7{0:d10}", i);
            }

            _invite.Phones = phones;
            return this;
        }
      
        public InviteCommand Build()
        {
            return _invite;
        }
    }
}
