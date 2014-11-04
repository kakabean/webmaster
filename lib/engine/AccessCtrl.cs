using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;

namespace WebMaster.lib.engine
{
    /// <summary>
    /// server access control and CS communication utilities. e.g login/out, data access and so on 
    /// </summary>
    public class AccessCtrl
    {
        /// <summary>
        /// return a UserProfile that indicate the login status, the Response field will indicate the response status. 
        /// </summary>
        /// <param name="name">real username</param>
        /// <param name="pwd">real password</param>
        /// <param name="verifyCode">use to enhance the security acess, to be used</param>
        /// <returns></returns>
        public static UserProfile login(string name, string pwd, string verifyCode) {
            UserProfile user = new UserProfile();
            user.Response = RESPONSE.CONNECT_ERROR;
            user.Name = name;
            user.Password = pwd;
            user.VerifyCode = verifyCode;
            // update the user info. 
            doLogin(user);
            return user;
        }        
        /// <summary>
        /// logout the user login, return true if logout sucessfully. 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool logout(UserProfile user) {
            bool logout = doLogout(user);
            return true;
        }
        /// <summary>
        /// update the user object with proper data
        /// </summary>
        /// <param name="user"></param>
        private static void doLogin(UserProfile user) {
            //TODO 
            if (user != null) { 
                user.Response = RESPONSE.LOGIN_SUCCESS;
            }
        }
        private static bool doLogout(UserProfile user) {
            return true;
        }
    }
}
