using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class Post
    {
        [Key]
        public int PostId {get;set;}
        [Required]
        [MinLength(3)]
        public string Title {get;set;}
        [Required]
        [MinLength(10)]
        [DataType(DataType.Text)]
        public string Message {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        // Link & Navigation
        [Required]
        public int UserId {get;set;}
        public User Author {get;set;}
        public ICollection<Link> Votes {get;set;}
    }
}