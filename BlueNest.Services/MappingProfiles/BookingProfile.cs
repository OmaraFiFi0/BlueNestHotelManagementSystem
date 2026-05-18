using AutoMapper;
using BlueNest.Core.Entities.BookingModule;
using BlueNest.Shared.DTOs.BookingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.MappingProfiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.GuestFullName, opt => opt.MapFrom(src => src.HotelUser.FullName))
                .ForMember(dest => dest.GustEmail, opt => opt.MapFrom(src => src.HotelUser.Email));
            CreateMap<Booking, MyBookingDTO>()
                .ForMember(dest => dest.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate))
                .ForMember(dest => dest.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate));

        }
    }
}
