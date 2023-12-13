using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAuthentication.Models
{
  public class User
  {
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phonenumber { get; set; }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required(ErrorMessage = "This is a required field")]
    public string Profession { get; set; }

    public string? City { get; set; }

    [Range(1, 150)]
    public int? Age { get; set; }
    public string? ImageUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.Normal;

    [Required(ErrorMessage = "This is a required field")]

    public string Password { get; set; }

    public User(string Email, string Phonenumber, string Profession, UserRole? userRole)
    {
      this.Email = Email;
      this.Phonenumber = Phonenumber;
      this.Profession = Profession;
      if (userRole != null) Role = (UserRole)userRole;
    }
  }
}