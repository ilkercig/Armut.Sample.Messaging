using Armut.Sample.Messaging.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Model
{
    public class Blocking : ModelBase, IValidatableObject
    {
        public int BlockerId { get; set; }
        public int UnWantedId { get; set; }

        [JsonIgnore]
        public User Blocker { get; set; }
        [JsonIgnore]
        public User UnWanted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var blocking = (Blocking)validationContext.ObjectInstance;

            MessagingContext dbContext = (MessagingContext)validationContext.GetService(typeof(MessagingContext));

            if (dbContext.Blockings.Any(b => (b.BlockerId == blocking.BlockerId) && (b.UnWantedId == blocking.UnWantedId)))
                yield return new ValidationResult("User already blocker");

        }
    }
}
