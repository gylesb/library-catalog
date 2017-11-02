using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using LibraryCatalog.Models;
using LibraryCatalog;

namespace LibraryCatalog.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet("/book/add")]
    public ActionResult AddBook()
    {
      return View();
    }

    [HttpPost("/book/view-all")]
    public ActionResult ViewBooks(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Book newBook = new Book(Request.Form["book-title"], Int32.Parse(Request.Form["book-copies"]);

      int authorValue = Int32.Parse(Request.Form["number-loop"]);
      for(var i=1;i<=authorValue;i++)
      {
        Author newAuthor = new Author(Request.Form["author-name"+i])
        if(newAuthor.IsNewAuthor() == true)
        {
          newAuthor.Save();
        }
        newbook.AddAuthor(newAuthor);
      }
      List<Author> bookAuthors = newBook.GetAuthors();

      model.Add("book", newBook);
      model.Add("authors", bookAuthors);

      return View("ViewBooks",model);
    }
  }
}
