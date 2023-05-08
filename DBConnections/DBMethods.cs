using Dapper;
using System.Data;
using System.Data.SqlClient;
using EventApp.Models;

namespace EventApp.DBConnections
{
    public class DBMethods
    {
        private static string connectionString= @"
 Server=172.17.0.2;Database=test;User Id=sa;Password=;
";

        public int AddUser(User user)
        {
            string sql = @"
                insert into EventUser values(@Name,@Surname,@Email,@Password,'User');
                SELECT CAST(SCOPE_IDENTITY() as int)
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteScalar<int>(sql, user);
            }

        }
        public int AddCity(string  sehir)
        {
            string sql = @"
                insert into Sehir values(@Name);
                SELECT CAST(SCOPE_IDENTITY() as int)
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteScalar<int>(sql, new{Name=sehir});
            }

        }
        public  int AddCategory(string kategori)
        {
            string sql = @"
               insert into Kategori values(@Name);
                SELECT CAST(SCOPE_IDENTITY() as int)
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteScalar<int>(sql, new{Name=kategori});
            }

        }
        public int AddEvent(Event _event)
        {
            string sql = @"
                declare @Sehirid int = (select Id from Sehir where Name=@Sehir)
                declare @Kategoriid int = (select Id from Kategori where Name = @Kategori)
                insert into Event 
                values(@Baslik,@Aciklama,@Tarih,@Kontenjan,@Kategoriid,@Sehirid,@OlusturanNumeric,@Onay,@Aktif,@Adres);
                SELECT CAST(SCOPE_IDENTITY() as int)
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteScalar<int>(sql,_event);
            }

        }
        public int AddTicket(Ticket ticket)
        {
            string sql = @"
                insert into Ticket values(@TicketNo,@UserNumeric,@EventNumeric)
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteScalar<int>(sql,ticket);
            }
        }
        public int UpdateUser(User _user)
        {
            string sql = @"
                update EventUser set Name=@Name,Surname=@Surname,Password=@Password  where Id=@Id;
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Execute(sql,_user);
            }
        }
         public int UpdateEvent(Event _event)
        {
            string sql = @"
                update Event set Onay=@Onay where Id=@Id;
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Execute(sql,_event);
            }
        }
         public int UpdateEventDetail(Event _event)
        {
            string sql = @"
                update Event set kontenjan=@Kontenjan , adres=@Adres where Id=@Id;
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Execute(sql,_event);
            }
        }
         public User GetUser(int userId)
        {
            string sql = @"
                select Name,Surname,Email,Password from EventUser where Id=@userId
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.QueryFirst<User>(sql,new {userId=userId});
            }
        }
         public Event GetEventByEventId(int eventid)
        {
            string sql = @"
                select * from Event where Id=@eventid
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.QueryFirstOrDefault<Event>(sql,new {eventid=eventid});
            }
        }
        public User GetUserForLogin(string email,string password)
        {
            string sql = @"
                select Id,Name,Surname,Email,Password,Role from EventUser where Email=@Email and Password=@Password
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.QueryFirstOrDefault<User>(sql,new {Email=email,Password=password});
            }
        }
        public  IEnumerable<Event> ShowEvents(int UserId)
        {
            string sql = @"
                select e.onay Onay,e.Id,e.baslik Baslik,e.aciklama Aciklama,e.tarih Tarih ,e.kontenjan-isnull(t.toplam,0) Kontenjan,k.Name Kategori,s.Name Sehir,concat(eu.Name,' ',eu.Surname) Olusturan,e.adres
                from Event e
                left join Sehir s on s.Id=e.sehirid
                left join Kategori k on k.Id=e.kategoriid
                left join EventUser eu on eu.Id=e.olusturanid 
                 left join (select eventid,count(*) toplam from Ticket  group by eventid) t on t.eventid=e.id
                where e.Id not in (select EventId from Ticket where UserId = @UserId)  and e.Onay=1
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Event>(sql,new {UserId=UserId});
            }

        }   
        public  IEnumerable<Event> ShowEventsForOnay()
        {
            string sql = @"
                select e.onay Onay,e.Id,e.baslik Baslik,e.aciklama Aciklama,e.tarih Tarih ,e.kontenjan Kontenjan,k.Name Kategori,s.Name Sehir,concat(eu.Name,' ',eu.Surname) Olusturan,e.adres
                from Event e
                left join Sehir s on s.Id=e.sehirid
                left join Kategori k on k.Id=e.kategoriid
                left join EventUser eu on eu.Id=e.olusturanid 
                 
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Event>(sql);
            }

        } 
        public  IEnumerable<Event> FilterEvents(DateTime? Tarih , string Kategori, string Sehir)
        {
            string sql = @"
                if(@Sehir='')
                select @Sehir=null
                if(@Kategori='')
                select @Kategori=null
                select e.onay,e.Id,e.baslik Baslik,e.aciklama Aciklama,e.tarih Tarih ,e.kontenjan-isnull(t.toplam,0) Kontenjan,k.Name Kategori,s.Name Sehir,concat(eu.Name,' ',eu.Surname) Olusturan,e.adres
                from Event e
                left join Sehir s on s.Id=e.sehirid
                left join Kategori k on k.Id=e.kategoriid
                left join EventUser eu on eu.Id=e.olusturanid 
                left join (select eventid,count(*) toplam from Ticket  group by eventid) t on t.eventid=e.id
                where k.Name = isnull(@Kategori,k.Name) and s.Name =isnull(@Sehir,s.Name) and e.tarih=isnull(@Tarih,e.tarih) and e.Onay=1
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Event>(sql,new {Kategori=Kategori , Sehir=Sehir,Tarih=Tarih});
            }

        }      
        public  IEnumerable<string> GetCities()
        {
            string sql = @"
                select '' Name union
                select Name from Sehir
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<string>(sql);
            }

        } 
        public  IEnumerable<string> GetCategories()
        {
            string sql = @"
                select '' Name union
                select Name from Kategori
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<string>(sql);
            }

        } 
        public  IEnumerable<Event> GetEventsById(int Id)
        {
            string sql = @"
                select e.Id,e.baslik Baslik,e.aciklama Aciklama,e.tarih Tarih ,e.kontenjan Kontenjan,k.Name Kategori,s.Name Sehir,'Siz' Olusturan,e.adres
                from Event e
                left join Sehir s on s.Id=e.sehirid
                left join Kategori k on k.Id=e.kategoriid
                left join EventUser eu on eu.Id=e.olusturanid 
                where olusturanid=@Id
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Event>(sql,new{Id=Id});
            }

        } 
        public  IEnumerable<Ticket> GetTickets(int userid)
        {
            string sql = @"
               select t.BiletNo TicketNo,e.Id EventId , e.baslik Baslik , e.aciklama Aciklama , e.tarih Tarih
                ,e.kontenjan Kontenjan,k.Name Kategori,s.Name Sehir,eu.Name Olusturan,e.adres Adres
                from Event e
                inner join Ticket t on t.EventId=e.Id
                inner join Sehir s on s.Id=e.sehirid
                inner join Kategori k on k.Id = e.kategoriid
                left join EventUser eu on eu.Id =e.olusturanid
                where t.UserId=@Userid
            ";
             using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Ticket>(sql,new {Userid=userid});
            }

        }
    }
}
