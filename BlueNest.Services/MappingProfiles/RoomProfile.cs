using AutoMapper;
using BlueNest.Core.Entities.RoomModule;
using BlueNest.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.MappingProfiles
{
    public class RoomProfile:Profile
    {
        public RoomProfile()
        {
            CreateMap<Room,RoomDTO>();

            CreateMap<Room, RoomDetailsDTO>()
                .ForMember(
                dest=>dest.ImageUrls,
                opt=>opt.MapFrom<RoomImageValueResolver>()
                );

            CreateMap<Room, RoomForAdminDTO>();

            CreateMap<RoomToCreateDTO, Room>();

            CreateMap<RoomToUpdateDTO, Room>();
        }
    }
}
