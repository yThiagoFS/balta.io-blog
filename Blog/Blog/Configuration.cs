using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Blog
{
    public static class Configuration
    {
        public static string JwtKey = "";
        public static string ApiKeyName = "";
        public static string ApiKey = "";
        public static SmtpConfiguration Smtp;

        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public static void Initialize(IConfiguration configuration)
        {
            //validar erros e lógica.
            foreach (var p in typeof(Configuration).GetTypeInfo().GetFields().Where(x => x.IsStatic))
            {
                if (p.FieldType.IsNested)
                {
                    var instance = Activator.CreateInstance(p.FieldType);

                    configuration.GetSection($"{p.Name}").Bind(instance);

                    p.SetValue(null, instance);
                }
                else 
                    p.SetValue(null, configuration.GetValue(p.FieldType, $"{p.Name}"));
            }
        }
    }
}
