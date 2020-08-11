using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using COMP2001_API.Models;

namespace COMP2001_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataAccess _database;

        public UsersController(DataAccess database)
        {
            _database = database;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<string> Get([FromBody] User usr)
        {
            bool Verified = getValidation(usr);

            return new string[] { "Verfied", Verified.ToString() };
        }

        


        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private bool getValidation(User usr)
        {
            bool validation = false;


            return validation;
           
        }
    }
}
