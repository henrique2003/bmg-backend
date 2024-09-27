﻿namespace Bmg.API.Dtos.Clients
{
    public class ClientDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
