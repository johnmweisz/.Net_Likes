using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class Link
    {
        [Key]
        public int LinkId {get;set;}
        [Required]
        public int PostId {get;set;}
        [Required]
        public int UserId {get;set;}
        [Required]
        public bool isUp {get;set;}
        // Not Mapped
        [NotMapped]
        public bool isDown
        {
            get
            {
                return !isUp;
            }
        }
        // ThenInclude.
        public User User {get;set;}
        public Post Post {get;set;}
    }
}