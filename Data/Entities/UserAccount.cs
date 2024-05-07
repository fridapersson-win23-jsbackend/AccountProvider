using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserAccount : IdentityUser
{
    //Utöver identity's default kolumner, vill jag ha dessa columner
    [ProtectedPersonalData]
    public string FirstName { get; set; } = null!;

    [ProtectedPersonalData]
    public string LastName { get; set; } = null!;

    [ProtectedPersonalData]
    public string? Biography { get; set; }

    [ProtectedPersonalData]
    public string? ProfileImgUrl { get; set; }

    public bool IsExternalAccount { get; set; } = false;


    public int? AddressId { get; set; }
    public UserAddress? Address { get; set; }


    public virtual ICollection<SavedCoursesEntity>? SavedCourses { get; set; }

}
