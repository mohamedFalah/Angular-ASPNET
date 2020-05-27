using System;

namespace webapp.API.Dtos
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }

        public string senderPhotoUrl { get; set; } = "";

        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public String Content { get; set; }

        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }
        
    }
}