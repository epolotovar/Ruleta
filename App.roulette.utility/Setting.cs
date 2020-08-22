using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.roulette.utility
{
    public sealed class Setting
    {
        private static Setting _instance = null;
        private DateTime DateTimeLastValidation { get; set; }
        private static readonly object padlock = new object();
        private IConfigurationSection configurationSection;

        private bool ValidateDate(DateTime date)
        {
            int hours = (int)(DateTime.Now - (date)).TotalHours;
            return (hours > 0 || hours < 0);
        }
        private Setting()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            configurationSection = configuration.GetSection("RedisCache");
        }

        public static Setting Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null || _instance.ValidateDate(_instance.DateTimeLastValidation))
                    {
                        _instance = new Setting();
                    }

                    return _instance;
                }
            }
        }

        public string GetConnectionString() {
            return configurationSection["ConnectionString"];
        }
    }
}
