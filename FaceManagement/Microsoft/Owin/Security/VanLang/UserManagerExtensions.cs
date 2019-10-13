using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Microsoft.Owin.Security.VanLang
{
    public class IdentityException : Exception
    {
        public IdentityResult IdentityResult { get; set; }
    }

    public static class UserManagerExtensions
    {
        public static async Task<TUser> CreateAddLoginAsync<TUser>(this UserManager<TUser> userManager, string email) where TUser : class, IUser<string>
        {
            var user = Activator.CreateInstance(typeof(TUser)) as TUser;
            user.GetType().GetProperty("Email").SetValue(user, email);
            var user1 = userManager.FindByEmail(email);
            //xu lý 
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var apiUrl = Constants.BaseUrl + "/API/getUserInfo?email=" + email;
                // get id from api then set id to below
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd().ToString();
                response.Close();
                readStream.Close();
                UserProfile AspNetUser = JsonConvert.DeserializeObject<UserProfile>(data);

                var infoLogin = new UserLoginInfo(Constants.DefaultAuthenticationName, AspNetUser.UserID);
                result = await userManager.AddLoginAsync(user.Id, infoLogin);
                if (result.Succeeded)
                    return user;
                else
                    throw new IdentityException { IdentityResult = result };
            }
            else
                throw new IdentityException { IdentityResult = result };
        }

        public class UserProfile
        {
            public string StID { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public Nullable<System.DateTime> DoB { get; set; }
            public string UserID { get; set; }
            public string Avatar { get; set; }

        }
    }
}