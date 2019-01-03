
using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace netcore_webapi.Models
{
  public class User
  {
    // [BsonId]
    // public ObjectId Id { get; set; }
    public BsonObjectId Id { get; set; }
    
    // [BsonElement("FirstName")]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

  }
}