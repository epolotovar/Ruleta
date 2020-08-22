using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.roulette.data
{
    public sealed class DataAccess
    {
        private static DataAccess _instance = null;
        private DateTime DateTimeLastValidation { get; set; }
        private static readonly object padlock = new object();
        private IDatabase dababase;

        private bool ValidateDate(DateTime date)
        {
            int hours = (int)(DateTime.Now - (date)).TotalHours;
            return (hours > 0 || hours < 0);
        }

        private DataAccess() {
            string CnnectionString = App.roulette.utility.Setting.Instance.GetConnectionString();
            ConnectionMultiplexer Conn = ConnectionMultiplexer.Connect(CnnectionString);
            dababase = Conn.GetDatabase();
        }

        public static DataAccess Instance {
            get {
                lock (padlock) {
                    if (_instance == null || _instance.ValidateDate(_instance.DateTimeLastValidation)) {
                        _instance = new DataAccess();
                    }

                    return _instance;
                }
            }
        }
        public string GetInformation(string Key) {
            return dababase.StringGet(Key).ToString();
        }
        public bool SetInformation(string Key, string data)
        {
            return dababase.StringSet(Key, data);
        }
    }
}
