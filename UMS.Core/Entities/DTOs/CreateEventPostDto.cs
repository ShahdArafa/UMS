﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{


    public class CreateEventPostDto
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public IFormFile? Image { get; set; } // الصورة اختيارية

    }
}

