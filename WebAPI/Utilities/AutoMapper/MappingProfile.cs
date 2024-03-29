﻿using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace WebAPI.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDtoForUpdate, Book>().ReverseMap();

            CreateMap<Book, BookDto>().ReverseMap();

            CreateMap<BookDtoForInsertion, Book>().ReverseMap();

            CreateMap<UserForRegistrationDto, User>().ReverseMap();
        }
    }
}
