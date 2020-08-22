using App.roulette.entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace App.roulette.business
{
    public sealed class Core
    {
        private static Core _instance = null;
        private DateTime DateTimeLastValidation { get; set; }
        private static readonly object roulettelock = new object();
        
        private bool ValidateDate(DateTime date){
            int hours = (int)(DateTime.Now - (date)).TotalHours;
            return (hours > 0 || hours < 0);
        }

        private Core() { }
        
        public static Core Instance {
            get {
                if (_instance == null || _instance.ValidateDate(_instance.DateTimeLastValidation)) {
                    _instance = new Core();
                }

                return _instance;
            }
        }
        public int Newroulette()
        {
            try{
                int _Id = 0;
                lock (roulettelock){
                    string roulettes = App.roulette.data.DataAccess.Instance.GetInformation("roulette");
                    if (roulettes != null) {
                        XmlDocument _data = new XmlDocument();
                        _data.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(roulettes));
                        List<Clsroulette> lstRoulette = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsroulette>>(_data, typeof(List<Clsroulette>));
                        int _id = lstRoulette[lstRoulette.Count - 1].Id;
                        _Id = _id + 1;
                        lstRoulette.Add(new Clsroulette(){ Id = _Id, Name = "Roulette Mro: " + _Id, Create = DateTime.Now, Status = false });
                        string _rouletteCompress = App.roulette.utility.Utilities.Instance.CompressGZip(App.roulette.utility.Utilities.Instance.SerializeToXmlDocument(lstRoulette).InnerXml);
                        bool _sw = App.roulette.data.DataAccess.Instance.SetInformation("roulette", _rouletteCompress);
                    } else {
                        _Id = 1;
                        List<Clsroulette> lstRoulette = new List<Clsroulette>();
                        lstRoulette.Add(new Clsroulette() { Id = _Id, Name = "Roulette Mro: " + _Id, Create = DateTime.Now, Status = false });
                        string _rouletteCompress = App.roulette.utility.Utilities.Instance.CompressGZip(App.roulette.utility.Utilities.Instance.SerializeToXmlDocument(lstRoulette).InnerXml);
                        bool _sw = App.roulette.data.DataAccess.Instance.SetInformation("roulette", _rouletteCompress);
                    }
                }

                return _Id;
            }catch (Exception){
                throw;
            }
        }
        public bool Openroulette(int Id) {
            try {
                bool state = false;
                string roulettes = App.roulette.data.DataAccess.Instance.GetInformation("roulette");
                if (roulettes != null) {
                    XmlDocument _data = new XmlDocument();
                    _data.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(roulettes));
                    List<Clsroulette> lstRoulette = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsroulette>>(_data, typeof(List<Clsroulette>));
                    int index = lstRoulette.FindIndex(x => x.Id.Equals(Id));
                    if (index >= 0) {
                        if (!lstRoulette[index].Status) {
                            lstRoulette[index].Open = DateTime.Now;
                            lstRoulette[index].Status = true;
                            state = true;
                            string _rouletteCompress = App.roulette.utility.Utilities.Instance.CompressGZip(App.roulette.utility.Utilities.Instance.SerializeToXmlDocument(lstRoulette).InnerXml);
                            state = App.roulette.data.DataAccess.Instance.SetInformation("roulette", _rouletteCompress);
                        }
                    }
                }

                return state;
            } catch (Exception) {
                throw;
            }
        }
        public bool Wager(Clsbets bets) {
            try{
                bool state = false;

                string roulettes = App.roulette.data.DataAccess.Instance.GetInformation("roulette");
                if (roulettes != null) {
                    XmlDocument _xmlroulette = new XmlDocument();
                    _xmlroulette.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(roulettes));
                    List<Clsroulette> lstRoulette = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsroulette>>(_xmlroulette, typeof(List<Clsroulette>));
                    int index = lstRoulette.FindIndex(x => x.Id.Equals(bets.Idroulette));
                    if (index >= 0) {
                        if (lstRoulette[index].Status) {
                            string _Wager = App.roulette.data.DataAccess.Instance.GetInformation("bets");
                            if (roulettes != null) {
                                XmlDocument _xmlBets = new XmlDocument();
                                _xmlBets.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(roulettes));
                                List<Clsbets> _bets = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsbets>>(_xmlBets, typeof(List<Clsroulette>));
                                _bets.Add(bets);
                                string _betsCompress = App.roulette.utility.Utilities.Instance.CompressGZip(App.roulette.utility.Utilities.Instance.SerializeToXmlDocument(_bets).InnerXml);
                                state = App.roulette.data.DataAccess.Instance.SetInformation("bets", _betsCompress);
                            } else {
                                List<Clsbets> _bets = new List<Clsbets>();
                                _bets.Add(bets);
                                string _betsCompress = App.roulette.utility.Utilities.Instance.CompressGZip(App.roulette.utility.Utilities.Instance.SerializeToXmlDocument(_bets).InnerXml);
                                state = App.roulette.data.DataAccess.Instance.SetInformation("bets", _betsCompress);
                            }
                        }
                    }
                }
                return state;
            } catch (Exception) {
                throw;
            }
        }
        public List<Clsbets> closeroulette(int Id)
        {
            try
            {
                List<Clsbets> _response = null;
                string roulettes = App.roulette.data.DataAccess.Instance.GetInformation("roulette");
                if (roulettes != null){
                    XmlDocument _data = new XmlDocument();
                    _data.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(roulettes));
                    List<Clsroulette> lstRoulette = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsroulette>>(_data, typeof(List<Clsroulette>));
                    int index = lstRoulette.FindIndex(x => x.Id.Equals(Id));
                    if (index >= 0){
                        if (lstRoulette[index].Status){
                            lstRoulette[index].Closed = DateTime.Now;
                            lstRoulette[index].Status = false;
                            string _Wager = App.roulette.data.DataAccess.Instance.GetInformation("bets");
                            if (_Wager != null) {
                                XmlDocument _xmlBet = new XmlDocument();
                                _xmlBet.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(_Wager));
                                List<Clsbets> lstBets = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsbets>>(_data, typeof(List<Clsbets>));
                                _response = lstBets.FindAll(x => x.Idroulette.Equals(Id));
                            }
                        }
                    }
                }

                return _response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Clsroulette> listroulette()
        {
            try
            {
                List<Clsroulette> _response = null;
                string roulettes = App.roulette.data.DataAccess.Instance.GetInformation("roulette");
                if (roulettes != null)
                {
                    XmlDocument _data = new XmlDocument();
                    _data.LoadXml(App.roulette.utility.Utilities.Instance.DecompressGZip(roulettes));
                    _response = App.roulette.utility.Utilities.Instance.Deserialize<List<Clsroulette>>(_data, typeof(List<Clsroulette>));
                }

                return _response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Betvalidation(ClsBet bet, out string err) {
            try
            {
                err = string.Empty;
                if (bet.Number >= 0 && (bet.Negro || bet.Rojo)) {
                    err = "No puede apostar a un número y a un color al tiempo";
                    return false;
                }

                if (bet.Number == null && (bet.Negro && bet.Rojo)) {
                    err = "No puede apostar a los dos colores al tiempo";
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
