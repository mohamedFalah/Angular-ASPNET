namespace webapp.API.Models
{
    public class Add
    {
        public int AdderId { get; set; }
        public int AddedId { get; set; }

        public User Adder { get; set; }

        public User Added { get; set; }


    }
}