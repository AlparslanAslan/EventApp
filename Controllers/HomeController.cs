using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventApp.Models;
using EventApp.DBConnections;
using Microsoft.AspNetCore.Mvc.Rendering;
using EventApp.Operations;
using System.IO;

namespace EventApp.Controllers;

public static class CurrentUser 
{
    public static int userId{get;set;}=-1;
    public static string? userRole {get;set;}
}
public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;
    public HomeController(IConfiguration config)
    {
        this._configuration = config;
    }
    public IActionResult Index()
    {
        var method = new DBMethods(_configuration);
        var sonuc = method.IsTablesExist();
        if(sonuc == 0)
            return View("AddTables");
        else
            return View();
    }
    [HttpPost]
    public IActionResult Signin(User user)
    {
        var val = new ValidationControl();
        string isValid = val.IsUserValid(user);
        if(isValid != "Valid")
        {
            ViewBag.MessageSignUp=isValid;
            return View("Index");
        }
        string hashPassword = PasswordEncrypter.EncryptPassword(user.Password);
        user.Password = hashPassword;
       
        var dbMethod = new DBMethods(this._configuration);
        var sonuc = dbMethod.AddUser(user);
        if(sonuc>0)
        {
            CurrentUser.userId =sonuc;
            CurrentUser.userRole="User";
            return View("Login");
        }
        if(sonuc==-1)
        {
            ViewBag.MessageSignUp="Bu Email ile açılmış hesap zaten bulunuyor.";
            return View("Index");

        }
        return View("Index");
        
    
        

        
        
        
    }
    [HttpPost]
    public IActionResult Login(User user)
    {
        var method = new DBMethods(this._configuration);
        User _user = method.GetUserForLogin(user.Email,user.Password);
        if(_user == null)
            return Content("User bulunamadı");
        var passwordVerified = PasswordEncrypter.VerifyPassword(user.Password,_user.Password);
        if(_user == null || !passwordVerified)
        {
            ViewBag.MessageLogin = "Kullanıcı Maili ya da Şifre Hatalı!";
            return View("Index");
        }
        
        CurrentUser.userId =_user.Id;
        CurrentUser.userRole =_user.Role;

        if(_user.Role == "admin")
            return View("AdminHome");
        else
            return View();
    }
    [HttpGet]
    public IActionResult Login()
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");
        else
            return View();
    }
    public IActionResult ShowEvents()
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız.");
        var method = new DBMethods(this._configuration);
        var eventsToShow = new EventsForUpdate(){_events=method.ShowEvents(CurrentUser.userId)};
        IEnumerable<string> citiesNumerable = method.GetCities() ;
        
        ViewData["Cities"] = new SelectList(citiesNumerable);

        IEnumerable<string> categoriesNumerable = method.GetCategories();
        
        ViewData["Categories"] = new SelectList(categoriesNumerable);
        
        return View(eventsToShow);
    }
    public IActionResult Arama(Event _event)
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");
        var method = new DBMethods(this._configuration);
        var events = method.FilterEvents(_event.Tarih,_event.Kategori,_event.Sehir,CurrentUser.userId);
        IEnumerable<string> citiesNumerable = method.GetCities() ;
        ViewData["Cities"] = new SelectList(citiesNumerable);

        IEnumerable<string> categoriesNumerable = method.GetCategories();
        ViewData["Categories"] = new SelectList(categoriesNumerable);
        var eventsToShow = new EventsForUpdate(){_events=events};
        return View("ShowEvents",eventsToShow);
    }
    [HttpPost]
    public IActionResult ShowEvents(int Itemid, int kontenjan)
    {
        var method = new DBMethods(this._configuration);
        if(kontenjan == 0)
        {
            ViewBag.Message ="Kontenjan Yetersiz. Katılım Sağlayamazsınız.";
        }
        else
        {
            var bilet = new Ticket()
            {
                UserNumeric=CurrentUser.userId,
                EventNumeric = Itemid,
                TicketNo = Operation.GetTicketNumber()
            };
            method.AddTicket(bilet);
        }
        IEnumerable<string> citiesNumerable = method.GetCities() ;
        ViewData["Cities"] = new SelectList(citiesNumerable);

        IEnumerable<string> categoriesNumerable = method.GetCategories();
        ViewData["Categories"] = new SelectList(categoriesNumerable);
        var eventsToShow = new EventsForUpdate(){_events=method.ShowEvents(CurrentUser.userId)};
        return View(eventsToShow);
    }
    [HttpGet]
    public IActionResult CreateEvent()
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");

        var method = new DBMethods(this._configuration);
        var eventUpdate = new EventsForUpdate();
        eventUpdate._events = method.GetEventsById(CurrentUser.userId);
        IEnumerable<string> citiesNumerable = method.GetCities() ;
        ViewData["Cities"] = new SelectList(citiesNumerable);

        IEnumerable<string> categoriesNumerable = method.GetCategories();
        ViewData["Categories"] = new SelectList(categoriesNumerable);

        return View(eventUpdate);
    }
    [HttpPost]
    public IActionResult CreateEvent(Event _event,int eventidKaldir)
    {
        var method = new DBConnections.DBMethods(this._configuration);
        if(eventidKaldir > 0 )
        {
            int sonuc = method.DateControl(eventidKaldir);
            if(sonuc == 0)
                ViewBag.Message  = "5 günden az zaman kala değişiklik yapamazsınız.";
            else
                method.DeleteEvent(eventidKaldir);
        }
        else
        {
            var val = new ValidationControl();
            var sonuc = val.EventControl(_event);
            if(sonuc != "Valid")
            {
                ViewBag.Message = sonuc;
                return View("CreateEvent");
            }
            _event.Aktif=1;
            _event.Onay=false;
            _event.OlusturanNumeric = CurrentUser.userId;
            method.AddEvent(_event);
        }
        

        var eventUpdate = new EventsForUpdate();
        eventUpdate._events = method.GetEventsById(CurrentUser.userId);
        IEnumerable<string> citiesNumerable = method.GetCities() ;
        ViewData["Cities"] = new SelectList(citiesNumerable);

        IEnumerable<string> categoriesNumerable = method.GetCategories();
        ViewData["Categories"] = new SelectList(categoriesNumerable);
        

        return View(eventUpdate);
    }
    public IActionResult MyTickets()
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");
        var method = new DBMethods(this._configuration);
        IEnumerable<Ticket> tickets = method.GetTickets(CurrentUser.userId);
        return View(tickets);
    }
    [HttpPost]
    public IActionResult MyTickets(string ticketNo)
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");
        var method = new DBMethods(this._configuration);
        method.DeleteTicket(ticketNo);
        IEnumerable<Ticket> tickets = method.GetTickets(CurrentUser.userId);
        return View(tickets);
    }
    public IActionResult MyProfile()
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");
        var method = new DBMethods(this._configuration);
        User _user= method.GetUser(CurrentUser.userId);
        ViewData["Name"] =_user.Name;
        ViewData["Surname"] =_user.Surname;
        ViewData["Email"] =_user.Email;
        ViewData["Password"] ="";
        return View(_user);
    }
    [HttpPost]
    public IActionResult MyProfile(User _user)
    {
        _user.Id=CurrentUser.userId;
        var method = new DBMethods(this._configuration);
        _user.Password = PasswordEncrypter.EncryptPassword(_user.Password);
        method.UpdateUser(_user);
        return View(_user);
    }
    [HttpGet]
    public IActionResult Onay()
    {
        // if(CurrentUser.userId ==-1)
        //     return Content("Giriş Yapmadınız");
        // if(CurrentUser.userRole !="admin")
        //     return Content("Sadece Admin Görüntüleyebilir");
        var method= new DBMethods(this._configuration);
        var events = method.ShowEventsForOnay();
        return View(events);
    }
    [HttpPost]
    public IActionResult Onay(string mainid,bool onaylimi)
    {
        var method= new DBMethods(this._configuration);
        method.UpdateEvent(new Event(){Id=Convert.ToInt32(mainid) , Onay=onaylimi});
        var events = method.ShowEventsForOnay();
        return View("Onay",events);
    }
    [HttpPost]
    public IActionResult AddCity(string sehir)
    {
        var method = new DBMethods(this._configuration);
        var CitiesAndCategories = new Info();
        CitiesAndCategories.Sehirler = method.GetCities();
        CitiesAndCategories.Kategoriler = method.GetCategories();
        if(CitiesAndCategories.Sehirler.Contains(sehir))
            ViewBag.Message = "Var olan bir şehri tekrar ekleyemezsiniz";
        else
            method.AddCity(sehir);
        
        return View("SetInformation",CitiesAndCategories);
    }
      [HttpPost]
    public IActionResult CreateTables()
    {
        var method = new DBMethods(this._configuration);
        string scriptPath = "DBConnections/Package/";
        method.RunScript(System.IO.File.ReadAllText(scriptPath+"Event.table.sql"));
        method.RunScript(System.IO.File.ReadAllText(scriptPath+"EventUser.table.sql"));
        method.RunScript(System.IO.File.ReadAllText(scriptPath+"Kategori.table.sql"));
        method.RunScript(System.IO.File.ReadAllText(scriptPath+"Sehir.table.sql"));
        method.RunScript(System.IO.File.ReadAllText(scriptPath+"Ticket.table.sql"));
        method.RunScript(System.IO.File.ReadAllText(scriptPath+"add_admin.post.sql"));

        
        return View("Index");
    }
    [HttpPost]
    public IActionResult DeleteCity(string sehir)
    {
        var method = new DBMethods(this._configuration);
        method.DeleteCity(sehir);

        var CitiesAndCategories = new Info();
        CitiesAndCategories.Sehirler = method.GetCities();
        CitiesAndCategories.Kategoriler = method.GetCategories();
        return View("SetInformation",CitiesAndCategories);
    }
    [HttpPost]
    public IActionResult DeleteCategory(string category)
    {
        var method = new DBMethods(this._configuration);
        method.DeleteCategory(category);

        var CitiesAndCategories = new Info();
        CitiesAndCategories.Sehirler = method.GetCities();
        CitiesAndCategories.Kategoriler = method.GetCategories();

      

        return View("SetInformation",CitiesAndCategories);
    }
     public IActionResult SetInformation()
    {
        if(CurrentUser.userId ==-1)
            return Content("Giriş Yapmadınız");
        if(CurrentUser.userRole !="admin")
            return Content("Sadece Admin Görüntüleyebilir");
        var CitiesAndCategories = new Info();
        var method = new DBMethods(this._configuration);
        CitiesAndCategories.Sehirler = method.GetCities();
        CitiesAndCategories.Kategoriler = method.GetCategories();
        return View(CitiesAndCategories);
    }
    [HttpPost]
    public IActionResult AddCategory(string kategori)
    {
        var method = new DBMethods(this._configuration);
        var CitiesAndCategories = new Info();
        CitiesAndCategories.Sehirler = method.GetCities();
        CitiesAndCategories.Kategoriler = method.GetCategories();
        if(CitiesAndCategories.Kategoriler.Contains(kategori))
            ViewBag.Message = "Var olan bir kategoriyi tekrar ekleyemezsiniz";
        else
            method.AddCategory(kategori);

        return View("SetInformation",CitiesAndCategories);
        
    }
    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost]
    public IActionResult UpdateEvent(int eventid,Event _event)
    {
        if(eventid > 0)
        {
            ViewBag.eventid = eventid;
            var method = new DBMethods(this._configuration);
            Event eve = method.GetEventByEventId(eventid);
            return View(eve);
        }
        else
        {
            
            
            var method = new DBMethods(this._configuration);
            
            int sonuc = method.DateControl(_event.Id);
            if(sonuc == 0)
                ViewBag.Message  = "5 günden az zaman kala değişiklik yapamazsınız.";
            else
                method.UpdateEventDetail(_event);
           
            var eventUpdate = new EventsForUpdate();
            eventUpdate._events = method.GetEventsById(CurrentUser.userId);
            IEnumerable<string> citiesNumerable = method.GetCities() ;
            ViewData["Cities"] = new SelectList(citiesNumerable);
    
            IEnumerable<string> categoriesNumerable = method.GetCategories();
            ViewData["Categories"] = new SelectList(categoriesNumerable);
    
            return View("CreateEvent",eventUpdate);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
