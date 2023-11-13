﻿using Microsoft.AspNetCore.Mvc;
using School_Login_SignUp.DatabaseServices;
using System.Reflection.Metadata.Ecma335;




namespace School_Login_SignUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Approvel : ControllerBase
    {
        ApprovelService approvelService = new ApprovelService();

        // GET api/<Approvel>/5
        [HttpGet("{id}")]
        public bool Get(string id)
        {
            return approvelService.GetApprovelStatus(id);
        }

        // POST api/<Approvel>
        [HttpPost("{id}")]
        public void Post(string id)
        {
            approvelService.SetApprovelStatus(id);
        }
    }
}
