using AutoMapper;
using noHRforIT.Business.Models;
using noHRforIT.Business.Models.AuthorizationModels;
using noHRforIT.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace noHRforIT.Business.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<RegisterModel, UserDTO>();
            CreateMap<AuthenticateModel, UserDTO>();
            //CreateMap<List<UserDTO>, List<User>>();
        }
    }
}
