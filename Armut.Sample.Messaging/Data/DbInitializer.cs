
using Armut.Sample.Messaging.Model;
using System;
using System.Linq;

namespace Armut.Sample.Messaging.Data
{
    internal class DbInitializer
    {
        internal static void Initialize(MessagingContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User() { UserName = "ilkercig", EmailAddress = "ilkercigdemal2@gmail.com", Password = "iddadgsad" },
                new User() { UserName = "rick", EmailAddress = "rick@gmail.com", Password = "asdasd12321" },
                new User() { UserName = "morty", EmailAddress = "morty@gmail.com", Password = "asdsda1312"  },
                new User() { UserName = "wes", EmailAddress = "wes@gmail.com", Password =  "adasd1231" },
                new User() { UserName = "tsubasa", EmailAddress = "tsubasa@gmail.com", Password = "12312dsad" },
                new User() { UserName = "rocket", EmailAddress = "rocket@gmail.com", Password = "asdad123123" },
                new User() { UserName = "zatsu", EmailAddress = "zatsu@gmail.com", Password = "132123ds" },
                new User() { UserName = "shikamaru", EmailAddress = "shikamaru@gmail.com", Password = "adaf5235j"   },
                new User() { UserName = "ryu", EmailAddress = "ryu@gmail.com", Password = "adssay34y"  },
                new User() { UserName = "burcu", EmailAddress = "burcu@gmail.com", Password = "asdasd41" },
            };

            foreach (var item in users)
            {
                context.Users.Add(item);
            }
            context.SaveChanges();

            var messages = new Message[]
            {
                new Message()
                {
                    Body ="hello",
                    SenderId = users.Single<User>(u=>u.UserName == "ilkercig").UserID,
                    ReceiverId = users.Single<User>(u=>u.UserName == "rick").UserID,
                    SentTime = DateTime.Now

                },
                new Message()
                {
                    Body ="hello",
                    SenderId = users.Single<User>(u=>u.UserName == "rick").UserID,
                    ReceiverId = users.Single<User>(u=>u.UserName == "ilkercig").UserID,
                    SentTime = DateTime.Now

                },

                new Message()
                {
                    Body ="merhaba",
                    SenderId = users.Single<User>(u=>u.UserName == "burcu").UserID,
                    ReceiverId = users.Single<User>(u=>u.UserName == "ilkercig").UserID,
                    SentTime = DateTime.Now

                },
                new Message()
                {
                    Body ="evet benim",
                    SenderId = users.Single<User>(u=>u.UserName == "burcu").UserID,
                    ReceiverId = users.Single<User>(u=>u.UserName == "rick").UserID,
                    SentTime = DateTime.Now
                }

            };

            foreach (var item in messages)
            {
                context.Messages.Add(item);
            }

            context.SaveChanges();


            var blockings = new Blocking[]
            {
                new Blocking(){ BlockerId = users.Single<User>(u=>u.UserName == "burcu").UserID, UnWantedId = users.Single<User>(u=>u.UserName == "morty").UserID},
                new Blocking(){ BlockerId = users.Single<User>(u=>u.UserName == "ilkercig").UserID, UnWantedId = users.Single<User>(u=>u.UserName == "rocket").UserID},
                new Blocking(){ BlockerId = users.Single<User>(u=>u.UserName == "rocket").UserID, UnWantedId = users.Single<User>(u=>u.UserName == "burcu").UserID}
            };

            foreach (var item in blockings)
            {
                context.Blockings.Add(item);
            }

            context.SaveChanges();

        }

    }
}