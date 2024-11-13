using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ShoppingCartApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

public class AccountController : Controller
{
    private UserManager<ApplicationUser> _userManager;
    private ApplicationDbContext _context;

    public AccountController()
    {
        _context = new ApplicationDbContext();
        _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
    }

    // GET: /Account/Register
    [HttpGet]
    public ActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Sign in the user
                var authManager = HttpContext.GetOwinContext().Authentication;
                var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                return RedirectToAction("Index", "Home");
            }

            AddErrors(result);
        }

        return View(model);
    }

    // GET: /Account/Login
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (await _userManager.IsLockedOutAsync(user.Id))
                {
                    ModelState.AddModelError("", "Your account is locked. Please try again later.");
                    return View(model);
                }

                var result = await _userManager.CheckPasswordAsync(user, model.Password);
                if (result)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
        }

        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Logout()
    {
        var authManager = HttpContext.GetOwinContext().Authentication;
        authManager.SignOut();

        return RedirectToAction("Index", "Home");
    }

    // User Profile
    [Authorize]
    public ActionResult UserProfile()
    {
        var userId = User.Identity.GetUserId();
        var user = _userManager.FindById(userId);
        return View(user);
    }

    [HttpPost]
    [Authorize]
    public ActionResult UpdateProfile(ApplicationUser model)
    {
        if (ModelState.IsValid)
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            _userManager.Update(user);
            return RedirectToAction("UserProfile");
        }
        return View("UserProfile", model);
    }

    [HttpPost]
    [Authorize]
    public ActionResult ChangePassword(string oldPassword, string newPassword)
    {
        var result = _userManager.ChangePassword(User.Identity.GetUserId(), oldPassword, newPassword);
        if (result.Succeeded)
        {
            return RedirectToAction("UserProfile");
        }
        else
        {
            ModelState.AddModelError("", "Error while changing password");
            return View("UserProfile");
        }
    }

    // User Orders
    [Authorize]
    public ActionResult MyOrders()
    {
        var userId = User.Identity.GetUserId();
        var orders = _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems.Select(oi => oi.Product))
            .ToList();

        return View(orders);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
        base.Dispose(disposing);
    }
}