using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Web.Test.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.Web.Test.Controller
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "JWT")]
    public class DemoController
    {
        private readonly DemoContext _db;

        public DemoController(DemoContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var query = _db.Orders;

            var result = await query.ToListAsync();

            return result;
        }

        [HttpPut]
        public async Task<Order> InsertOrderAsync([FromBody] Order order)
        {
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            return order;
        }

        [HttpDelete("{id}")]
        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _db.Orders.FirstAsync(p => p.Id == id);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<Order> UpdateOrderAsync([FromBody] Order order)
        {
            var dbOrder = await _db.Orders.FirstAsync(p => p.Id == order.Id);

            dbOrder.OrderDate = order.OrderDate;
            dbOrder.OrderDescription = order.OrderDescription;
            dbOrder.OrderNumber = order.OrderNumber;

            await _db.SaveChangesAsync();

            return dbOrder;
        }
    }
}
