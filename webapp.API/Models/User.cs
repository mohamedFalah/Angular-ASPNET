using System;
using System.Collections.Generic;

namespace webapp.API.Models
{
    public class User
    {
        public User(int id, string username, string gender, DateTime dateOfBirth, DateTime lastActive, string introduction, string lookingFor, string interests, string city, string country, DateTime created)
        {
            this.Id = id;
            this.Username = username;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.LastActive = lastActive;
            this.Introduction = introduction;
            this.LookingFor = lookingFor;
            this.Interests = interests;
            this.City = city;
            this.Country = country;
            this.Created = created;

        }
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime LastActive { get; set; }

        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }

        public DateTime Created { get; set; }

        public ICollection<Add> Adders  { get; set; }

        public ICollection<Add> Addeds  { get; set; }


}

}