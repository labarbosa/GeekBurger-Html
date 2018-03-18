using System;
using System.Linq;
using GeekBurger.Orders.Contract;
using GeekBurger_HTML.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger_HTML.Controllers
{
    public class MockController :Controller
    {
        [HttpPost("api/face")]
        public IActionResult Face() {
            return Ok();
        }

        [HttpPost("/api/FoodRestrictions")]
        public IActionResult FoodRestrictions()
        {
            return Ok();
        }

        [HttpPost("/api/Order/Pay")]
        public IActionResult Pay()
        {
            return Ok();
        }

        [HttpPost("/api/Order")]
        public IActionResult Order([FromBody] OrderToPost order)
        {
            //I can't use UI Order now because it contains only one product
            return Json(new {OrderId = Guid.NewGuid() ,
                Total = string.Format("{0:C}", order.Products.Sum(x=> x.Price))});
        }
    }
}
