using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace TodoApp.Models.Auth
{
    public class UserAccountModel : IdentityUser<Guid>
    {      
        //Nisam htio previše komplicirati s loginom, nadam se da je jednostavan primjer OK. 
        // UN + PW registracija i social login - Google (dodan je također).

        [StringLength(350, MinimumLength = 5, ErrorMessage = usernameMessage)]
        public override string? UserName { get; set; }
            
        private const string usernameMessage =
       "Username should be at least 5 characters long.";        
           
    }
}