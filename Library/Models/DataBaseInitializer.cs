using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Library.Models;

namespace Library.Models
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<LibraryContext>
    {
        protected override void Seed(LibraryContext db)
        {
            // Добавление издательств
            Publisher newPublisher = new Publisher { PublisherName = "Физтех", Address = "Долгопрудный" };
            db.Publishers.Add(newPublisher);
            db.Publishers.Add(new Publisher { PublisherName = "Неизвестное издательство" });
            db.Publishers.Add(new Publisher { PublisherName = "ЛучшийМир", Address = "Земля", Number = "8-800-555-35-35" });
            db.Publishers.Add(new Publisher { PublisherName = "Издательство", Email = "izd@mail.ru" });
            // Добавление книг
            Record newRecord = new Record { RecordName = "Матанчик 3 часть", RecordDescription = "Матан для физтехов", AuthorName = "Иванов" };
            newRecord.RecordPublisher = newPublisher;
            db.Records.Add(newRecord);

            var userS = new UserManager<Admin>(new UserStore<Admin>(db));
            Admin newAdmin = new Admin { Name = "abacaba", UserName = "abacaba" };
            userS.Create(newAdmin, "dabacaba");

            db.Admins.Add(newAdmin);

            db.SaveChanges();
        }
    }
}


