﻿namespace Calendar.BE.Dto
{
    public class EventDto
    {
        public string Summary { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
    }
}
