﻿using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class UserAddress
{
    [Key]
    public int Id { get; set; }
    public string AddressLine_1 { get; set; } = null!;
    public string? AddressLine_2 { get; set; }
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string? AddressType { get; set; }

    //public string? UserId { get; set; } = null!;
    //public ICollection<UserAccount>? Users { get; set; } = [];
}
