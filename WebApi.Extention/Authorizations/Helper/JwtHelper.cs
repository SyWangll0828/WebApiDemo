using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common.Helper;

namespace WebApi.Extention.Authorizations
{
    public class JwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string IssueJwt(TokenModelJwt tokenModel)
        {
            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = AppSecretConfig.Audience_Secret_String;

            //var claims = new Claim[] //old
            var claims = new List<Claim>
         {
          /*
          * 特别重要：
            1、这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 uid从 Token 中取出来，请看下边的SerializeJwt() 方法，或者在整个解决方案，搜索这个方法，看哪里使用了！
            2、你也可以研究下 HttpContext.User.Claims ，具体的你可以看看 Policys/PermissionHandler.cs 类中是如何使用的。
          */

             

         new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
         new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
         new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
         //这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
         new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
         new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(1000).ToString()),
         new Claim(JwtRegisteredClaimNames.Iss,iss),
         new Claim(JwtRegisteredClaimNames.Aud,aud),
         
         //new Claim(ClaimTypes.Role,tokenModel.Role),//为了解决一个用户多个角色(比如：Admin,System)，用下边的方法
        };

            // 可以将一个用户的多个角色全部赋予；
            // 作者：DX 提供技术支持；
            claims.AddRange(tokenModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));



            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: iss,
                claims: claims,
                signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        public static string getJwtToken()
        {
            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = AppSecretConfig.Audience_Secret_String;

            // 创建声明数组
            var claims = new List<Claim>
            {
                // 两种方式
                new Claim(ClaimTypes.Role,"User"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(JwtRegisteredClaimNames.Email,"")
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 实例化token对象
            // 3+2
            var jwt = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                signingCredentials: creds,
                claims: claims,
                expires: DateTime.Now.AddDays(3));

            // 生成token
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }


        /// <summary>
        /// 令牌
        /// </summary>
        public class TokenModelJwt
        {
            /// <summary>
            /// Id
            /// </summary>
            public long Uid { get; set; }
            /// <summary>
            /// 角色
            /// </summary>
            public string Role { get; set; }
            /// <summary>
            /// 职能
            /// </summary>
            public string Work { get; set; }

        }


        public class AppSecretConfig
        {
            private static string Audience_Secret = Appsettings.app(new string[] { "Audience", "Secret" });
            private static string Audience_Secret_File = Appsettings.app(new string[] { "Audience", "SecretFile" });


            public static string Audience_Secret_String => InitAudience_Secret();


            private static string InitAudience_Secret()
            {
                var securityString = DifDBConnOfSecurity(Audience_Secret_File);
                if (!string.IsNullOrEmpty(Audience_Secret_File) && !string.IsNullOrEmpty(securityString))
                {
                    return securityString;
                }
                else
                {
                    return Audience_Secret;
                }

            }

            private static string DifDBConnOfSecurity(params string[] conn)
            {
                foreach (var item in conn)
                {
                    try
                    {
                        if (File.Exists(item))
                        {
                            return File.ReadAllText(item).Trim();
                        }
                    }
                    catch (System.Exception) { }
                }

                return conn[conn.Length - 1];
            }

        }
    }
}
