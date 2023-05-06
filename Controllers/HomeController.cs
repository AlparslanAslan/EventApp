using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventApp.Models;
using EventApp.DBConnections;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventApp.Controllers;

public static class CurrentUser 
{
    public static int userId{get;set;}
}
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Signin(User user)
    {
        var dbMethod = new DBMethods();
        CurrentUser.userId = dbMethod.AddUser(user);
        
        return View("Login");
    }
    [HttpPost]
    public IActionResult Login(User user)
    {
        var method = new DBMethods();
        User _user = method.GetUserForLogin(user.Email,user.Password);
        
        if(_user.Role == "admin")
        {
            return View("AdminHome");
        }
        else
        {
            return View();
        }
        
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    public IActionResult ShowEvents()
    {
        var method = new DBMethods();
        var events = method.ShowEvents(CurrentUser.userId);
        return View(events);
    }
    [HttpPost]
    public IActionResult ShowEvents(int Itemid)
    {
        var bilet = new Ticket()
        {
            UserNumeric=CurrentUser.userId,
            EventNumeric = Itemid,
            TicketNo = "UU134121254"
        };
        var method = new DBMethods();
        method.AddTicket(bilet);

        var events = method.ShowEvents(CurrentUser.userId);
        return View(events);
    }
    [HttpGet]
    public IActionResult CreateEvent()
    {
        List<string> cities = new List<string> { "Istanbul", "Izmir", "Ankara" };
        IEnumerable<string> citiesNumerable = cities;
        ViewData["Cities"] = new SelectList(citiesNumerable);

         List<string> categories = new List<string> { "Sinema", "Tiyatro", "Konser" };
        IEnumerable<string> categoriesNumerable = categories;
        ViewData["Categories"] = new SelectList(categoriesNumerable);

        return View();
    }
    [HttpPost]
    public IActionResult CreateEvent(Event _event)
    {
        _event.Aktif=1;
        _event.Onay=2;
        _event.OlusturanNumeric = CurrentUser.userId;
        var methode = new DBConnections.DBMethods();
        methode.AddEvent(_event);
        
        List<string> cities = new List<string> { "Istanbul", "Izmir", "Ankara" };
        IEnumerable<string> citiesNumerable = cities;
        ViewData["Cities"] = new SelectList(citiesNumerable);

        List<string> categories = new List<string> { "Sinema", "Tiyatro", "Konser" };
        IEnumerable<string> categoriesNumerable = categories;
        ViewData["Categories"] = new SelectList(categoriesNumerable);


        return View();
    }
    public IActionResult MyTickets()
    {
        var method = new DBMethods();
        IEnumerable<Ticket> tickets = method.GetTickets(CurrentUser.userId);
        return View(tickets);
    }
    public IActionResult MyProfile()
    {
        var method = new DBMethods();
        User _user= method.GetUser(CurrentUser.userId);
        ViewData["Name"] =_user.Name;
        ViewData["Surname"] =_user.Surname;
        ViewData["Email"] =_user.Email;
        ViewData["Password"] =_user.Password;
        return View(_user);
    }
    [HttpPost]
    public IActionResult MyProfile(User _user)
    {
        _user.Id=CurrentUser.userId;
        var method = new DBMethods();
        method.UpdateUser(_user);
        return View(_user);
    }
    [HttpGet]
    public IActionResult Onay()
    {
        var method= new DBMethods();
        var events = method.ShowEvents(0);
        return View(events);
    }
    [HttpPost]
    public IActionResult Onay(List<int> eventid, List<string> onay)
    {
        var method= new DBMethods();
        for(int i=0;i<onay.Count;i++)
        {
            var _event = new Event(){
                Onay=Convert.ToInt32(onay[i]),
                Id = eventid[i]
            };
            method.UpdateEvent(_event);
        }
        var events = method.ShowEvents(0);
        return View(events);
        
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
