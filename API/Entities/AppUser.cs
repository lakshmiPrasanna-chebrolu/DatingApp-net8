using System;

namespace API.Entities;// should be physical folder location
//EntiryFrameWork uses only properties having the public access modifiers.
//class represents table and properties represent the column

public class AppUser
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required  byte[] PasswordHash{ get; set; }
    public required byte[] PasswordSalt { get; set; }

}
