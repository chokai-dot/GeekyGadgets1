using GeekyGadgets.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Domain.ViewModels.Account
{
    public class AuthenticateResponse
    {
       

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(GeekyGadgets.Domain.Entity.User user, string token)
        {
            Id = user.Id;
            Name= user.Name;
            Email = user.Email;
            Token = token;
        }

    }
}
