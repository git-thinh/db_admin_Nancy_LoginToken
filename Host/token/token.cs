using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace host
{
    public static class tokenLogin
    {
        public static string securekey = "123";

        public static string decode_get_username(string token)
        {
            string username = "";
            try
            {
                //Claims don't deserialize :(
                //var jwttoken = JsonWebToken.DecodeToObject<JwtToken>(token, configProvider.GetAppSetting("securekey"));

                //var decodedtoken = JsonWebToken.DecodeToObject(token, configProvider.GetAppSetting("securekey")) as Dictionary<string, object>;

                var decodedtoken = JsonWebToken.DecodeToObject(token, securekey) as Dictionary<string, object>;

                var jwttoken = new JwtToken()
                {
                    Audience = (string)decodedtoken["Audience"],
                    Issuer = (string)decodedtoken["Issuer"],
                    Expiry = DateTime.Parse(decodedtoken["Expiry"].ToString()),
                };

                if (decodedtoken.ContainsKey("Claims"))
                {
                    //var claims = new List<Claim>();

                    for (int i = 0; i < ((ArrayList)decodedtoken["Claims"]).Count; i++)
                    {
                        Dictionary<string, object> ds = (Dictionary<string, object>)(((ArrayList)decodedtoken["Claims"])[i]);
                        if (ds.Count > 0)
                        {
                            object u = null;
                            if (ds.TryGetValue(ClaimTypes.Name, out u))
                                username = u.ToString();
                        }

                        //var type = ((Dictionary<string, object>)((ArrayList)decodedtoken["Claims"])[i])["Type"].ToString();
                        //username = ((Dictionary<string, object>)((ArrayList)decodedtoken["Claims"])[i])["Value"].ToString();

                        //claims.Add(new Claim(type, username));
                    }

                    //jwttoken.Claims = claims;
                }

                if (jwttoken.Expiry < DateTime.UtcNow)
                    username = "";
                ////////TODO Tidy on 3.8 Mono release
                //////var claimsPrincipal = new ClaimsPrincipal();
                //////var claimsIdentity = new ClaimsIdentity("Token");
                //////claimsIdentity.AddClaims(jwttoken.Claims);
                //////claimsPrincipal.AddIdentity(claimsIdentity);
                //////return claimsPrincipal;
            }
            catch (Exception)
            {
                return "";
            }

            return username;
        }

    }
}
