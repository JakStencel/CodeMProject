using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace noHRforIT.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public bool CheckIfModelIsValid<T>(T model) where T : class
        {
            return ModelState.IsValid
                   && model != null;
        }
    }
}