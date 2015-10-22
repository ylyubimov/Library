using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

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
            db.SaveChanges();
        }
    }
}


