using Armut.Sample.Messaging.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Model
{
    public class User : ModelBase, IValidatableObject
    {
        private string m_Password;
        private string m_RawPassword;

        public int UserID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "User Name is Invalid")]
        [RegularExpression(@"^[a-zA-Z 0-9_]*$", ErrorMessage = "Username can not include specaial characters")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email address is Invalid")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [JsonIgnore]
        [Required]
        [DataType(DataType.Password)]
        public string Password
        {
            get
            {
                return m_Password;
            }

            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    m_RawPassword = value;
                    m_Password = PasswordHelper.PasswordHash(value);
                }

                    
            }
        }

        [JsonIgnore]
        public ICollection<Message> SentMessages { get; set; }
        [JsonIgnore]
        public ICollection<Message> ReceivedMessages { get; set; }
        [JsonIgnore]
        public ICollection<Blocking> BlockedByMe { get; set; }
        [JsonIgnore]
        public ICollection<Blocking> UnWantsMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var user = (User)validationContext.ObjectInstance;

            MessagingContext dbContext = (MessagingContext) validationContext.GetService(typeof(MessagingContext));

            if(dbContext.Users.Any(u => (u.UserName == user.UserName) || (u.EmailAddress == user.EmailAddress)))
                yield return new ValidationResult("A user with the same username or email already exists");

            if(m_RawPassword.Count()<6)
                yield return new ValidationResult("Password at least should be 6 characters");
        }
    }
}
