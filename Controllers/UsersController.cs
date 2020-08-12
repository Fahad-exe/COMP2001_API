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
        public IActionResult Get([FromBody] User usr)
        {
            bool Verified = getValidation(usr);

            return Ok(new string[] { "Verfied", Verified.ToString() });
        }

        

        // POST: api/Users
        [HttpPost]
        public IActionResult Post([FromBody] User usr)
        {
            string responseMessage;

            register(usr, out responseMessage);

            int ResponseCode = Convert.ToInt32(responseMessage);

            switch (ResponseCode)
            {
                case 404: return BadRequest();
                case 208: return StatusCode(208);

                default: return new JsonResult(new string[] { "UserID", responseMessage }) { StatusCode = StatusCodes.Status201Created };
            }
  
        }

        

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User usr)
        {
            
                _database.Update(usr, id);
                return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _database.Delete(id);
            return NoContent();
        }

        private bool getValidation(User usr)
        {
            bool validation = false;

            validation = (bool)_database.Validate(usr);

            return validation;

        }

        private void register(User usr, out string responseMessage)
        {
            try
            {
                _database.Register(usr, out responseMessage);
            }
            catch(Exception e)
            {
                responseMessage = e.Message;
            }

           
        }

        //private string getValidation(User usr)
        //{
        //    string validation = "Starting this getValidation ..";
        //    try
        //    {
        //        validation += _database.Validate(usr).ToString();
        //    }
        //    catch(Exception e)
        //    {
        //        validation += e.Message.ToString();
        //    }

        //    return validation;
        //}
    }
}
