using AutoMapper;
using BlueNest.Core.Entities.RoomModule;
using BlueNest.Shared.DTOs.RoomDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.MappingProfiles
{
    public class RoomImageValueResolver : IValueResolver<Room, RoomDetailsDTO, List<string>>
    {
        private readonly IConfiguration _configuration;

        public RoomImageValueResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> Resolve(Room source, RoomDetailsDTO destination, List<string> destMember, ResolutionContext context)
        {
            return source.RoomImages.Select(R => $"{_configuration["URLs:BaseUrl"]}/images/rooms/{R.PictureUrl}")
                      .ToList();
        }
    }
}
