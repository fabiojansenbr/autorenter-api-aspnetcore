﻿using System;

namespace AutoRenter.API.Models
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Miles { get; set; }
        public string Color { get; set; }
        public bool IsRentToOwn { get; set; }
        public Guid LocationId { get; set; }
    }
}