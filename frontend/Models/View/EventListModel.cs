using System;
using System.Collections.Generic;
using Eburgan.Frontend.Models.Api;

namespace Eburgan.Frontend.Models.View
{
    public class EventListModel
    {
        public IEnumerable<Event> Events { get; set; }
        public int NumberOfItems { get; set; }
    }
}
