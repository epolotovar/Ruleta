using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.roulette.entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace App.roulette.service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouletteController : ControllerBase
    {
        [HttpGet]
        [Route("NewRoulette")]
        public ActionResult<int> NewRoulette() {
            return Ok(App.roulette.business.Core.Instance.Newroulette());
        }

        [HttpGet]
        [Route("OpenRoulette/{Id}")]
        public ActionResult<string> OpenRoulette(int Id) {
            if(App.roulette.business.Core.Instance.Openroulette(Id))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        [Route("Wager")]
        public ActionResult<string> Wager(ClsBet bet)
        {
            if (ModelState.IsValid) {
                string User = string.Empty;
                var headers = Response.Headers;
                if (headers.TryGetValue("User", out StringValues Value)) {
                    User = Value.FirstOrDefault();
                    string err = string.Empty;
                    if (App.roulette.business.Core.Instance.Betvalidation(bet, out err)) {
                        Clsbets _bets = new Clsbets();
                        _bets.Idroulette = bet.Idroulette;
                        _bets.IdUser = User;
                        if (bet.Number != null)
                            _bets.Number = bet.Number;
                        _bets.Negro = bet.Negro;
                        _bets.Rojo = bet.Rojo;
                        _bets.Money = bet.Money;
                        if (App.roulette.business.Core.Instance.Wager(_bets))
                            return Ok();
                        else
                            return BadRequest();
                    } else { return BadRequest(err); }
                } else { return NotFound("Usuario no Encontrado"); }
            } else { return BadRequest(ModelState); }
        }

        [HttpGet]
        [Route("CloseRoulette/{Id}")]
        public ActionResult CloseRoulette(int Id)
        {
            List<Clsbets> bets = App.roulette.business.Core.Instance.closeroulette(Id);
            if (bets.Count > 0)
                return Ok(bets);
            else
                return NotFound("No se encontraron apuestas");
        }

        [HttpGet]
        [Route("ListRoulette")]
        public ActionResult ListRoulette()
        {
            return Ok(App.roulette.business.Core.Instance.listroulette());
        }
    }
}