﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrpSelectionHelper.Dto
{
    class SelectedCourse
    {
        public SelectedCourse()
        {

        }
        public string Id { get; set; }
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Selectable { get; set; }
    }
}
