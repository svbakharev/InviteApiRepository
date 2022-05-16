using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InviteApi.Data
{
    public class Invite
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public int ApiId { get; set; }
        public string Message { get; set; }
        public ICollection<InvitePhone> Phones { get; set; }
    }

    public class InvitePhone
    {
        public int Id { get; set; }
        public string Phone { get; set; }
    }
}