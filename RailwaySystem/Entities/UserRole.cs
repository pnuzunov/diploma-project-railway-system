﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.Entities
{
    public class UserRole : BaseEntity
    {
        public string Name { get; set; }
        public int LevelOfAccess { get; set; }
    }
}