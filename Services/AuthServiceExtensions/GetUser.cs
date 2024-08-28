using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TodoApp.Models.Auth;

namespace TodoApp.Services.Auth
{
    public static class UserExtensions
    {   
        //added my own GetUser function to ClaimsPrincipal .NET to check the identity of the current user. 
        //Basically, returns the guid of the user from the table AspNetusers. 
        //I can then use the GUID to get todo's for a currently signed in user when doing a query
        public static async Task<Guid?> GetUser(this ClaimsPrincipal user, UserManager<UserAccountModel> userManager)
        {
            //didn't find user
            if (user == null)
            {
                return null;
            }
            // OK, got the user but is he authenticated??
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            //seems yes, find me the username

            var userName = user.Identity.Name;

            
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            //got the username, find me the guid
            var identityUser = await userManager.FindByNameAsync(userName);

            if (identityUser == null)
            {

                return null;
            }

            //return the guid
            return identityUser.Id;
        }
    }
}