using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using Microsoft.IdentityModel.Logging;
using Xamarin.Essentials;

namespace AsteriskMenu.Helpers
{
    public class Helper
    {
        
        public static string GetBearToken()
        {
            return  Preferences.Get("BearToken", "");
        }

        public static DateTime GetBearExpire(DateTime now)
        {
            return Preferences.Get("BearExpire", now);
        }

        public static void SetBearToken(string token)
        {
            string keyBearToken = "BearToken";
            string keyBearExpire = "BearExpire";
            if (String.IsNullOrWhiteSpace(token))
            {
                Preferences.Remove(keyBearToken);
                Preferences.Remove(keyBearExpire);
            }
            else
            {
                Preferences.Set(keyBearToken, token);
                /**
                 * TODO buscar -> DateTime.UtcNow.AddMinutes(_appSettings.TokenMinutesExpire),
                 */
                Preferences.Set(keyBearExpire, DateTime.Now.AddMinutes(60000));
            }
        }
        
        public static string GetClaim(string claimName)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(GetBearToken());
                var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
                return claimValue;
            }
            catch (Exception)
            {
                LogHelper.LogWarning("Error on try GetClaim");
                return "";
            }
        }
    }
}