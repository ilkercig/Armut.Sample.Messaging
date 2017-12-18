using Armut.Sample.Messaging.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Model
{
    public class Message : ModelBase, IValidatableObject
    {
        public int MessageID { get; set; }

        [StringLength(500)]
        [Required]
        public string Body { get; set; }


        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        [Required]
        public DateTime SentTime { get; set; }

        public DateTime? ReceiveTime { get; set; }


        public bool Read { get; set; }


        #region Navigation Properties

        [JsonIgnore]
        public User Sender { get; set; }

        [JsonIgnore]
        public User Receiver { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var message = (Message)validationContext.ObjectInstance;

            if (SenderId == ReceiverId)
                yield return new ValidationResult("You can not send message to yourself");

        }
    }
}
